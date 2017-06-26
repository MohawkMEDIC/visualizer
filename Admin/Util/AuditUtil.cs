/*
 * Copyright 2015-2017 Mohawk College of Applied Arts and Technology
 *
 * Licensed under the Apache License, Version 2.0 (the "License"); you
 * may not use this file except in compliance with the License. You may
 * obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the
 * License for the specific language governing permissions and limitations under
 * the License.
 *
 * User: khannan
 * Date: 2017-6-15
 */

using Admin.DataAccess;
using Admin.Models;
using AtnaApi.Model;
using AtnaApi.Transport;
using MARC.EHRS.Visualization.Core.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using Admin.Models.Db;

namespace Admin.Util
{
	/// <summary>
	/// Audit utility used by the audit viewer to audit audit actions
	/// </summary>
	public class AuditUtil
	{
		/// <summary>
		/// Audit a generic error
		/// </summary>
		public static void AuditAuditLogUsed(Controller context, OutcomeIndicator outcome, ActionType action, EventIdentifierType eventId, AuditableObjectLifecycle lifecycle, System.Collections.IEnumerable recordsAffected)
		{
			var audit = CreateAuditMessageHeader(context, context.HttpContext, outcome, action, eventId);
			audit.EventIdentification.EventType.Clear();
			var tCode = new CodeValue<EventIdentifierType>(EventIdentifierType.AuditLogUsed);
			audit.EventIdentification.EventType.Add(new CodeValue<String>(tCode.Code, tCode.CodeSystem, tCode.DisplayName));

			foreach (Object itm in recordsAffected)
			{
				int id = 0;
				String scope = "Audit";
				if (itm is Audit)
					id = (itm as Audit).AuditId;
				else if (itm is AuditSummaryVw)
					id = (itm as AuditSummaryVw).AuditId;
				else if (itm is AuditStatus)
				{
					id = (itm as AuditStatus).AuditVersionId;
					scope = "AuditStatus";
				}

				var ao = new AuditableObject()
				{
					IDTypeCode = new CodeValue<AuditableObjectIdType>(AuditableObjectIdType.EncounterNumber),
					ObjectId = String.Format("{0}^^^{1}", id, scope),
					Role = AuditableObjectRole.SecurityResource,
					RoleSpecified = true,
					TypeSpecified = true,
					Type = AuditableObjectType.SystemObject,
					LifecycleTypeSpecified = true,
					LifecycleType = lifecycle,
					ObjectDetail = new List<ObjectDetailType>(),
					ObjectSpec = String.Format("{0} #{1}", scope, id),
					ObjectSpecChoice = ObjectDataChoiceType.ParticipantObjectName
				};

				if (itm is AuditStatus)
				{
					ao.LifecycleType = AuditableObjectLifecycle.Creation;
					ao.ObjectDetail.Add(CreateObjectDetail<AuditStatus>(itm as AuditStatus));
				}
				audit.AuditableObjects.Add(ao);
			}

			SendAudit(audit);
		}

		/// <summary>
		/// Audit a generic error
		/// </summary>
		public static void AuditGenericError(HttpContext context, OutcomeIndicator outcome, EventIdentifierType eventId, CodeValue<String> actionCode, String errorPath)
		{
			var audit = CreateAuditMessageHeader(null, new HttpContextWrapper(context), outcome, ActionType.Execute, eventId);
			audit.EventIdentification.EventType.Clear();
			audit.EventIdentification.EventType.Add(actionCode);
			// Add an object for the failed resource
			audit.AuditableObjects.Add(new AuditableObject()
			{
				IDTypeCode = new CodeValue<AuditableObjectIdType>(AuditableObjectIdType.Uri),
				ObjectId = String.Format("{0}://{1}:{2}/{3}", context.Request.Url.Scheme, context.Request.Url.Host, context.Request.Url.Port, errorPath),
				Role = AuditableObjectRole.Resource,
				RoleSpecified = true,
				Type = AuditableObjectType.SystemObject,
				TypeSpecified = true
			});
			SendAudit(audit);
		}

		/// <summary>
		/// Audit a generic error
		/// </summary>
		public static void AuditGenericError(Controller context, OutcomeIndicator outcome, EventIdentifierType eventId, CodeValue<String> actionCode, String errorPath, Exception e)
		{
			var audit = CreateAuditMessageHeader(context, context.HttpContext, outcome, ActionType.Execute, eventId);
			audit.EventIdentification.EventType.Clear();
			audit.EventIdentification.EventType.Add(actionCode);
			// Add an object for the failed resource
			audit.AuditableObjects.Add(new AuditableObject()
			{
				IDTypeCode = new CodeValue<AuditableObjectIdType>(AuditableObjectIdType.Uri),
				ObjectId = String.Format("{0}://{1}:{2}/{3}", context.Request.Url.Scheme, context.Request.Url.Host, context.Request.Url.Port, errorPath),
				Role = AuditableObjectRole.Resource,
				RoleSpecified = true,
				Type = AuditableObjectType.SystemObject,
				TypeSpecified = true
			});

			if (e != null)
			{
				var dtl = AuditUtil.CreateObjectDetail(e);
				dtl.Type = typeof(Exception).AssemblyQualifiedName;
				audit.AuditableObjects.Add(new AuditableObject()
				{
					IDTypeCode = new CodeValue<AuditableObjectIdType>() { Code = "NA", CodeSystem = "NullFlavor", DisplayName = "Not Applicable" },
					ObjectId = e.GetHashCode().ToString(),
					Role = AuditableObjectRole.Resource,
					Type = AuditableObjectType.Other,
					RoleSpecified = true,
					TypeSpecified = true,
					ObjectSpec = e.GetType().Name,
					ObjectSpecChoice = ObjectDataChoiceType.ParticipantObjectName,
					ObjectDetail = new List<ObjectDetailType>()
					{
						dtl
					}
				});
			}
			SendAudit(audit);
		}

		/// <summary>
		/// Audit user event like a login/logout or user creation/deletion
		/// </summary>
		public static void AuditUserEvent(Controller context, OutcomeIndicator outcome, ActionType action, EventIdentifierType eventType, AuditableObjectLifecycle lifecycle, ApplicationUser user)
		{
			var audit = CreateAuditMessageHeader(context, context.HttpContext, outcome, action, EventIdentifierType.SecurityAlert);
			audit.EventIdentification.EventType.Clear();
			var tCode = new CodeValue<EventIdentifierType>(eventType);
			audit.EventIdentification.EventType.Add(new CodeValue<String>(tCode.Code, tCode.CodeSystem, tCode.DisplayName));

			// User affected
			if (user != null)
				audit.AuditableObjects.Add(new AuditableObject()
				{
					IDTypeCode = new CodeValue<AuditableObjectIdType>(AuditableObjectIdType.UserIdentifier),
					LifecycleType = lifecycle,
					LifecycleTypeSpecified = true,
					ObjectId = user.UserName,
					ObjectSpec = user.UserName,
					ObjectSpecChoice = ObjectDataChoiceType.ParticipantObjectName,
					Role = AuditableObjectRole.SecurityUser,
					RoleSpecified = true,
					Type = AuditableObjectType.SystemObject,
					TypeSpecified = true
				});

			SendAudit(audit);
		}

		/// <summary>
		/// Find a certificate
		/// </summary>
		public static X509Certificate2 FindCertificate(StoreName storeName, StoreLocation storeLocation, X509FindType findType, string findValue)
		{
			X509Store store = new X509Store(storeName, storeLocation);
			try
			{
				store.Open(OpenFlags.ReadOnly);
				var certs = store.Certificates.Find(findType, findValue, false);
				if (certs.Count > 0)
					return certs[0];
				else
					throw new InvalidOperationException("Cannot locate certificate");
			}
			finally
			{
				store.Close();
			}
		}

		/// <summary>
		/// Load audit message
		/// </summary>
		public static AuditMessageInfo LoadAudit(Guid auditId)
		{
			var persistenceService = new MARC.EHRS.Visualization.Server.Persistence.Ado.AdoAuditPersistenceService() { Context = null };
			return persistenceService.DePersistAuditMessage(auditId);
		}

		/// <summary>
		/// Make generic code
		/// </summary>
		internal static CodeValue<string> MakeGenericCode<T>(T code)
		{
			CodeValue<T> source = new CodeValue<T>(code);
			return new CodeValue<string>(source.Code, source.CodeSystem, source.DisplayName);
		}

		/// <summary>
		/// Constructs an audit message header
		/// </summary>
		private static AuditMessage CreateAuditMessageHeader(Controller controller, HttpContextBase context, OutcomeIndicator outcome, ActionType action, EventIdentifierType type)
		{
			AuditMessage retVal = new AuditMessage(
				DateTime.Now,
				action,
				outcome,
				type,
				new CodeValue<string>(context.Request.HttpMethod, "urn:ietf:rfc:2616")
			);

			// Add for receiver
			retVal.Actors.Add(new AuditActorData()
			{
				UserName = Environment.UserName,
				UserIdentifier = context.Request.Url.Host,
				NetworkAccessPointId = Dns.GetHostName(),
				NetworkAccessPointType = AtnaApi.Model.NetworkAccessPointType.MachineName,
				NetworkAccessPointTypeSpecified = true,
				AlternativeUserId = System.Diagnostics.Process.GetCurrentProcess().Id.ToString(),
				ActorRoleCode = new List<CodeValue<string>>() {
					new CodeValue<String>("110152", "DCM")
				}
			});

			// Add for sender
			retVal.Actors.Add(new AuditActorData()
			{
				UserIdentifier = context.Request.ServerVariables["REMOTE_ADDR"],
				NetworkAccessPointId = context.Request.ServerVariables["REMOTE_ADDR"],
				NetworkAccessPointTypeSpecified = true,
				NetworkAccessPointType = NetworkAccessPointType.IPAddress,
				ActorRoleCode = new List<CodeValue<string>>(){
					new CodeValue<String>("110153", "DCM")
				},
				UserIsRequestor = true
			});

			// Add for authorization
			if (context.User != null && context.User.Identity.IsAuthenticated)
			{
				var actor = new AuditActorData()
				{
					UserIdentifier = context.User.Identity.Name,
					UserIsRequestor = true,
					AlternativeUserId = context.Request.Headers["Authorize"],
					NetworkAccessPointId = context.Request.ServerVariables["REMOTE_ADDR"],
					NetworkAccessPointTypeSpecified = true,
					NetworkAccessPointType = NetworkAccessPointType.IPAddress,
					ActorRoleCode = new List<CodeValue<string>>()
						{
							new CodeValue<String>("6", "AuditableObjectRole")
						}
				};

				if (controller != null)
				{
					var authorize = controller.GetType().GetCustomAttributes(typeof(AuthorizeAttribute), false);
					if (authorize.Length > 0)
						foreach (AuthorizeAttribute aut in authorize)
						{
							foreach (var role in aut.Roles.Split(','))
								actor.ActorRoleCode.Add(new CodeValue<string>(role, "UserAuthRole", "OpAdmin User Role"));
						}
				}
				retVal.Actors.Add(actor);
			}
			else
				retVal.Actors.Add(new AuditActorData()
				{
					UserIdentifier = "Anonymous",
					UserIsRequestor = true,
					AlternativeUserId = context.Request.Headers["Authorize"],
					NetworkAccessPointId = context.Request.ServerVariables["REMOTE_ADDR"],
					NetworkAccessPointTypeSpecified = true,
					NetworkAccessPointType = NetworkAccessPointType.IPAddress,
					ActorRoleCode = new List<CodeValue<string>>()
						{
							new CodeValue<String>("6", "AuditableObjectRole")
						}
				});
			retVal.SourceIdentification.Add(new AuditSourceIdentificationType()
			{
				AuditEnterpriseSiteID = ConfigurationManager.AppSettings["enterpriseSiteId"],
				AuditSourceTypeCode = new List<CodeValue<AuditSourceType>>(){
					new CodeValue<AuditSourceType>(AuditSourceType.WebServerProcess)
				},
				AuditSourceID = String.Format("{0} on {1}", System.Diagnostics.Process.GetCurrentProcess().ProcessName, Environment.MachineName)
			});

			try
			{
				using (MemoryStream ms = new MemoryStream())
				{
					var dtl = new ObjectDetailType();
					dtl.Type = "HTTPMessage";
					using (StreamWriter sw = new StreamWriter(ms, System.Text.Encoding.UTF8))
					{
						sw.WriteLine("<?xml version=\"1.0\"?><Request><![CDATA[");

						sw.WriteLine("{0} {1} HTTP/1.1", context.Request.HttpMethod, context.Request.RawUrl);
						for (int i = 0; i < context.Request.Headers.Count; i++)
							sw.WriteLine("{0}: {1}", context.Request.Headers.Keys[i], context.Request.Headers[i]);

						using (StreamReader sr = new StreamReader(context.Request.InputStream))
							sw.WriteLine("\r\n{0}", sr.ReadToEnd());
						sw.WriteLine("]]></Request>");
						sw.Flush();
						dtl.Value = ms.GetBuffer().Take((int)ms.Length).ToArray();
					}

					retVal.AuditableObjects.Add(new AuditableObject()
					{
						IDTypeCode = new CodeValue<AuditableObjectIdType>(AuditableObjectIdType.Uri),
						ObjectId = context.Request.Url.ToString(),
						RoleSpecified = true,
						Role = AuditableObjectRole.Query,
						Type = AuditableObjectType.SystemObject,
						TypeSpecified = true,
						ObjectSpec = context.Request.Url.ToString(),
						ObjectSpecChoice = ObjectDataChoiceType.ParticipantObjectQuery,
						ObjectDetail = new List<ObjectDetailType>()
					{
						dtl
					}
					});
				}
			}
			catch (Exception)
			{ }

			return retVal;
		}

		/// <summary>
		/// Create execution information for later retrieval in case the table data changes
		/// </summary>
		private static ObjectDetailType CreateObjectDetail<T>(T detail)
		{
			ObjectDetailType retVal = new ObjectDetailType();
			using (MemoryStream ms = new MemoryStream())
			{
				XmlWriter writer = XmlWriter.Create(ms);
				writer.WriteStartElement(typeof(T).Name);
				WriteObject(writer, detail);
				writer.WriteEndDocument();

				writer.Close();
				retVal.Value = ms.GetBuffer().Take((int)ms.Length).ToArray();
			}
			retVal.Type = typeof(T).AssemblyQualifiedName;
			return retVal;
		}

		/// <summary>
		/// Send the audit
		/// </summary>
		private static void SendAudit(AuditMessage message)
		{
			var endpoint = new Uri(ConfigurationManager.AppSettings["arrEndpoint"]);
			IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(endpoint.Host), endpoint.Port);
			ITransporter transport = null;
			switch (endpoint.Scheme)
			{
				case "udp":
					transport = new UdpSyslogTransport(ipep);
					break;

				case "tcp":
					transport = new TcpSyslogTransport(ipep);
					break;

				case "stcp":
					String localCertHash = ConfigurationManager.AppSettings["myCertificateHash"],
					remoteCertHash = ConfigurationManager.AppSettings["remoteCertificateHash"];

					// Certs
					X509Certificate2 localCert = null, remoteCert = null;
					if (localCertHash != null)
					{
						string[] parts = localCertHash.Split(',');
						StoreLocation loc = (StoreLocation)Enum.Parse(typeof(StoreLocation), parts[0]);
						StoreName name = (StoreName)Enum.Parse(typeof(StoreName), parts[1]);

						localCert = FindCertificate(name, loc, X509FindType.FindByThumbprint, parts[2]);
					}
					if (remoteCertHash != null)
					{
						string[] parts = remoteCertHash.Split(',');
						StoreLocation loc = (StoreLocation)Enum.Parse(typeof(StoreLocation), parts[0]);
						StoreName name = (StoreName)Enum.Parse(typeof(StoreName), parts[1]);

						remoteCert = FindCertificate(name, loc, X509FindType.FindByThumbprint, parts[2]);
					}

					transport = new STcpSyslogTransport(ipep)
					{
						ClientCertificate = localCert,
						ServerCertificate = remoteCert
					};

					break;
			}

			// Now now now... we send
			transport.SendMessage(message);
		}

		/// <summary>
		/// Write object
		/// </summary>
		private static void WriteObject(XmlWriter writer, object detail)
		{
			foreach (var prop in detail.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
			{
				if (prop.GetIndexParameters().Length != 0) continue;
				object value = prop.GetValue(detail);
				if (value == null) continue;
				if (prop.PropertyType.IsPrimitive ||
					prop.PropertyType == typeof(String) ||
					prop.PropertyType == typeof(Guid) ||
					prop.PropertyType.FullName.StartsWith("System.Nullable") ||
					prop.PropertyType == typeof(System.Collections.Specialized.NameValueCollection))
				{
					writer.WriteStartElement(prop.Name);

					writer.WriteValue(value.ToString());

					writer.WriteEndElement();
				}
			}
		}
	}
}