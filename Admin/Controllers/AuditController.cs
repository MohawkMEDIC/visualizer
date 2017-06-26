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

using Admin.Attributes;
using Admin.DataAccess;
using Admin.Models;
using Admin.Util;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Web.Mvc;

namespace Admin.Controllers
{
	[TokenAuthorize]
	public class AuditController : Controller
	{
		//
		// GET: /Audit/
		public ActionResult Index()
		{
			return View();
		}

		private DateTime CalculateFirstDayOfWeek(int weekNumber, int year)
		{
			DateTime jan1 = new DateTime(year, 1, 1);
			int dayOffset = DayOfWeek.Monday - jan1.DayOfWeek;
			DateTime firstMonday = jan1.AddDays(dayOffset);
			var cal = CultureInfo.CurrentCulture.Calendar;
			int firstWeek = cal.GetWeekOfYear(jan1, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Sunday);
			if (firstWeek <= 1)
				weekNumber -= 1;
			return firstMonday.AddDays(weekNumber * 7);
		}

		// GET: /Audit/Stats
		public ActionResult Stats()
		{
			// Turn off caching
			Response.CacheControl = "no-cache";
			Response.AddHeader("Pragma", "no-cache");
			Response.Expires = -1;
			return PartialView();
		}

		// GET: /Audit/StatsData
		public ActionResult StatsData()
		{
			try
			{
				// First get a result set
				var entityContext = new AuditModelDataContext();
				NameValueCollection rootNvc = new NameValueCollection(),
									objectNvc = new NameValueCollection(),
									ptcptNvc = new NameValueCollection();

				// Do the query
				foreach (String key in Request.QueryString.Keys)
					if (key.StartsWith("Object::"))
						objectNvc.Add(key.Substring(key.LastIndexOf(":") + 1), Request.QueryString[key]);
					else if (key.StartsWith("Participant::"))
						ptcptNvc.Add(key.Substring(key.LastIndexOf(":") + 1), Request.QueryString[key]);
					else
						rootNvc.Add(key, Request.QueryString[key]);

				var records = this.DoQuery(entityContext.AuditSummaryVws, rootNvc, false);
				if (objectNvc.Count > 0)
				{
					ViewBag.ShowObjectParms = true;
					var objectResults = this.DoQuery(entityContext.AuditObjectSummaryVws, objectNvc, false);
					records = records.Where(a => objectResults.Count(o => o.AuditId == a.AuditId) > 0);
				}
				if (ptcptNvc.Count > 0)
				{
					ViewBag.ShowParticipantParms = true;
					var participantResult = this.DoQuery(entityContext.AuditParticipantSummaryVws, ptcptNvc, false);
					records = records.Where(a => participantResult.Count(p => p.AuditId == a.AuditId) > 0);
				}

				// Do some stats stuff
				// results by type
				Trace.WriteLine("Start By Type");
				var resultsByType = records.GroupBy(k => k.EventType).OrderByDescending(o => o.Count()).Take(10);
				StatisticsChart byTypeChart = new StatisticsChart();
				byTypeChart.AddLegendItems(resultsByType.Select(k => k.Key).AsEnumerable().ToArray());
				byTypeChart.data = new PiePlotData(byTypeChart);
				foreach (var itm in resultsByType)
					(byTypeChart.data as PiePlotData).AddData(itm.Key, itm.Count());
				Trace.WriteLine("End By Type");
				// results by day of week
				Trace.WriteLine("Start By Day");
				var resultsByDay = records.GroupBy(k => new { Week = k.EventTimestamp.DayOfWeek });
				StatisticsChart byDateChart = new StatisticsChart();
				//byDateChart.AddLegendItems(resultsByType.Select(k => k.Key).AsEnumerable().ToArray());
				byDateChart.AddLegendItem("Total", "rgba(0,0,0,0.5)");
				byDateChart.data = new BarPlotData(byDateChart);
				var labels = new List<string>(Enum.GetNames(typeof(DayOfWeek)));
				(byDateChart.data as BarPlotData).labels = labels;

				(byDateChart.data as BarPlotData).AddDataset("Total", labels.Select(d =>
				{
					var dow = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), d);
					var dayGroup = resultsByDay.Where(k => k.Key.Week == dow).FirstOrDefault();
					if (dayGroup == null) return 0;
					return dayGroup.Select(p => p.AuditId).Distinct().Count();
				}).ToArray());
				Trace.WriteLine("End By Day");

				// results by week
				Trace.WriteLine("Start By Week");

				var resultsByWeek = records.GroupBy(k => new { WeekYear = entityContext.fn_WeekOfYear(k.EventTimestamp), Year = k.EventTimestamp.Year });
				StatisticsChart byWeekChart = new StatisticsChart();
				byWeekChart.data = new BarPlotData(byWeekChart);
				byWeekChart.AddLegendItem("Access", null);
				byWeekChart.data = new BarPlotData(byWeekChart);
				labels = new List<string>(resultsByWeek.Select(k => k.Key).OrderBy(a => a.Year).ThenBy(a => a.WeekYear).AsEnumerable().Select(l => string.Format("W{0} {1}", l.WeekYear, l.Year)));
				(byWeekChart.data as BarPlotData).labels = labels;

				(byWeekChart.data as BarPlotData).AddDataset("Access", labels.Select(d =>
				{
					string[] token = d.Substring(1).Split(' ');
					int weekYear = Int32.Parse(token[0]) - 1,
						year = Int32.Parse(token[1]);

					var weekGroup = resultsByWeek.Where(k => k.Key.WeekYear == weekYear && k.Key.Year == year).FirstOrDefault();
					if (weekGroup == null) return 0;
					return weekGroup.Select(p => p.AuditId).Distinct().Count();
				}).ToArray());

				Trace.WriteLine("End By Week");

				// results by class
				Trace.WriteLine("Start By Class");

				var resultsByClass = records.GroupBy(k => k.EventCode);
				StatisticsChart byClassChart = new StatisticsChart();
				byClassChart.AddLegendItems(resultsByClass.Select(k => k.Key).AsEnumerable().ToArray());
				byClassChart.data = new PiePlotData(byClassChart);
				foreach (var itm in resultsByClass)
					(byClassChart.data as PiePlotData).AddData(itm.Key, itm.Count());
				Trace.WriteLine("End By Class");

				// results by outcome
				Trace.WriteLine("Start By Outcome");

				var resultsByOutcome = records.GroupBy(k => k.OutcomeCode);
				StatisticsChart byOutcomeChart = new StatisticsChart();
				byOutcomeChart.AddLegendItems(resultsByOutcome.Select(k => k.Key).AsEnumerable().ToArray());
				byOutcomeChart.data = new PiePlotData(byOutcomeChart);
				foreach (var itm in resultsByOutcome)
					(byOutcomeChart.data as PiePlotData).AddData(itm.Key, itm.Count());
				Trace.WriteLine("End By Outcome");

				//// results by user
				//Trace.WriteLine("Start By User");

				//var resultsByUser = entityContext.AuditDetailVws.Where(d=>records.Count(e=>e.AuditId == d.AuditId) > 0 && d.PtcptObjectRoleCodeDisplay == "User" && d.PtcptObjectRoleCodeMnemonic == "6").GroupBy(k => k.PtcptUserId);
				//StatisticsChart byUserChart = new StatisticsChart();
				//byUserChart.AddLegendItems(resultsByUser.Select(k => k.Key).AsEnumerable().ToArray());
				//byUserChart.data = new PiePlotData(byUserChart);
				//foreach (var itm in resultsByUser)
				//    (byUserChart.data as PiePlotData).AddData(itm.Key, itm.Select(p=>p.AuditId).Distinct().Count());
				//Trace.WriteLine("End By User");

				// results by outcome
				Trace.WriteLine("Start By Outcome");

				var resultsByAction = records.GroupBy(k => k.ActionCode);
				StatisticsChart byActionChart = new StatisticsChart();
				byActionChart.AddLegendItems(resultsByAction.Select(k => k.Key).AsEnumerable().ToArray());
				byActionChart.data = new PiePlotData(byActionChart);
				foreach (var itm in resultsByAction)
					(byActionChart.data as PiePlotData).AddData(itm.Key, itm.Count());
				Trace.WriteLine("Start By Outcome");

				// alerted results by week
				Trace.WriteLine("Start By Alert");

				StatisticsChart byWeekAlertsChart = new StatisticsChart();
				byWeekAlertsChart.AddLegendItem("Alert", "rgba(200,81,65,0.5)");
				byWeekAlertsChart.AddLegendItem("Non Alert", null);
				byWeekAlertsChart.data = new BarPlotData(byWeekAlertsChart);
				labels = new List<string>(resultsByWeek.Select(k => k.Key).OrderBy(a => a.Year).ThenBy(a => a.WeekYear).AsEnumerable().Select(l => string.Format("W{0} {1}", l.WeekYear, l.Year)));
				(byWeekAlertsChart.data as BarPlotData).labels = labels;
				{
					(byWeekAlertsChart.data as BarPlotData).AddDataset("Alert", labels.Select(d =>
					{
						string[] token = d.Substring(1).Split(' ');
						int weekYear = Int32.Parse(token[0]) - 1,
							year = Int32.Parse(token[1]);

						var weekGroup = resultsByWeek.Where(k => k.Key.WeekYear == weekYear && k.Key.Year == year).FirstOrDefault();
						if (weekGroup == null) return 0;
						return weekGroup.Where(p => p.IsAlert == true).Select(p => p.AuditId).Distinct().Count();
					}).ToArray());
					(byWeekAlertsChart.data as BarPlotData).AddDataset("Non Alert", labels.Select(d =>
					{
						string[] token = d.Substring(1).Split(' ');
						int weekYear = Int32.Parse(token[0]),
							year = Int32.Parse(token[1]);

						var weekGroup = resultsByWeek.Where(k => k.Key.WeekYear == weekYear && k.Key.Year == year).FirstOrDefault();
						if (weekGroup == null) return 0;
						return weekGroup.Where(p => p.IsAlert == false).Select(p => p.AuditId).Distinct().Count();
					}).ToArray());
				}
				Trace.WriteLine("End By Alert");

				// Turn off caching
				Response.CacheControl = "no-cache";
				Response.AddHeader("Pragma", "no-cache");
				Response.Expires = -1;
				AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.Success, AtnaApi.Model.ActionType.Read, AtnaApi.Model.EventIdentifierType.Query, AtnaApi.Model.AuditableObjectLifecycle.Access, new List<object>() { });
				return Json(new
				{
					byType = byTypeChart,
					byDay = byDateChart,
					byClass = byClassChart,
					byWeek = byWeekChart,
					byOutcome = byOutcomeChart,
					byAlertWeek = byWeekAlertsChart,
					byAction = byActionChart,
					// byUser = byUserChart
				}, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.MinorFail, AtnaApi.Model.ActionType.Read, AtnaApi.Model.EventIdentifierType.Query, AtnaApi.Model.AuditableObjectLifecycle.Disclosure, new List<Object>() { e });
				return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);
			}
			finally
			{
			}
		}

		//
		// GET: /Audit/AuditView
		public ActionResult List()
		{
			try
			{
				var entityContext = new AuditModelDataContext();
				// Page?
				int pageId = 0, resultsPerPage = 20;
				if (Request.QueryString["page"] != null)
					pageId = Int32.Parse(Request.QueryString["page"]);
				if (Request.QueryString["count"] != null)
					pageId = Int32.Parse(Request.QueryString["count"]);

				var records = this.DoQuery(entityContext.AuditSummaryVws, Request.QueryString);

				Response.CacheControl = "no-cache";
				Response.AddHeader("Pragma", "no-cache");
				Response.Expires = -1;

				// View bag stuff for codes
				ViewBag.EventCode = this.GetAttributeValues("EventCode", null);
				ViewBag.ActionCode = this.GetAttributeValues("ActionCode", null);
				ViewBag.OutcomeCode = this.GetAttributeValues("OutcomeCode", null);
				ViewBag.EventType = this.GetAttributeValues("EventType", null);

				AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.Success, AtnaApi.Model.ActionType.Read, AtnaApi.Model.EventIdentifierType.Query, AtnaApi.Model.AuditableObjectLifecycle.Disclosure, records.Skip(pageId * resultsPerPage).Take(resultsPerPage).ToArray());
				return PartialView(new AuditSummaryCollectionViewModel(records));
			}
			catch (Exception e)
			{
				AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.MinorFail, AtnaApi.Model.ActionType.Read, AtnaApi.Model.EventIdentifierType.Query, AtnaApi.Model.AuditableObjectLifecycle.Disclosure, new List<Object>() { e });
				return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);
			}
			finally
			{
			}
		}

		/// <summary>
		/// Search
		/// </summary>
		// GET /Audit/Search
		public ActionResult Search()
		{
			try
			{
				// Page?
				int pageId = 0, resultsPerPage = 20;
				if (Request.QueryString["page"] != null)
					pageId = Int32.Parse(Request.QueryString["page"]);
				if (Request.QueryString["count"] != null)

					pageId = Int32.Parse(Request.QueryString["count"]);
				// View bag stuff for codes
				ViewBag.EventCode = this.GetAttributeValues("EventCode", null);
				ViewBag.ActionCode = this.GetAttributeValues("ActionCode", null);
				ViewBag.OutcomeCode = this.GetAttributeValues("OutcomeCode", null);
				ViewBag.EventType = this.GetAttributeValues("EventType", null);
				ViewBag.UserId = this.GetAttributeValues("PtcptUserId", null);
				ViewBag.ParticipantRoleCode = this.GetAttributeValues("PtcptObjectRoleCodeDisplay", null);
				ViewBag.ObjectTypeCode = this.GetAttributeValues("ObjTypeCode", null);
				ViewBag.ObjectLifecycle = this.GetAttributeValues("ObjLifecycle", null);
				ViewBag.ObjectRoleCode = this.GetAttributeValues("ObjRoleCode", null);
				ViewBag.ObjectIdTypeCode = this.GetAttributeValues("ObjIdTypeCode", null);
				ViewBag.ShowObjectParms = false;
				ViewBag.ShowParticipantParms = false;
				ViewBag.ShowRootParms = true;

				Response.CacheControl = "no-cache";
				Response.AddHeader("Pragma", "no-cache");
				Response.Expires = -1;

				// Search Results
				var entityContext = new AuditModelDataContext();
				NameValueCollection rootNvc = new NameValueCollection(),
					objectNvc = new NameValueCollection(),
					ptcptNvc = new NameValueCollection();

				foreach (String key in Request.QueryString.Keys)
					if (key.StartsWith("Object::"))
						objectNvc.Add(key.Substring(key.LastIndexOf(":") + 1), Request.QueryString[key]);
					else if (key.StartsWith("Participant::"))
						ptcptNvc.Add(key.Substring(key.LastIndexOf(":") + 1), Request.QueryString[key]);
					else
						rootNvc.Add(key, Request.QueryString[key]);

				// TODO: Filter the Participants
				// TODO: Filter the Audit Summary View

				var results = this.DoQuery(entityContext.AuditSummaryVws, rootNvc);
				// TODO: Filter the Objects
				if (objectNvc.Count > 0)
				{
					ViewBag.ShowObjectParms = true;
					var objectResults = this.DoQuery(entityContext.AuditObjectSummaryVws, objectNvc);
					results = results.Where(a => objectResults.Count(o => o.AuditId == a.AuditId) > 0);
				}
				if (ptcptNvc.Count > 0)
				{
					ViewBag.ShowParticipantParms = true;
					var participantResult = this.DoQuery(entityContext.AuditParticipantSummaryVws, ptcptNvc);
					results = results.Where(a => participantResult.Count(p => p.AuditId == a.AuditId) > 0);
				}

				if (rootNvc.Count == 1)
				{
					ViewBag.ShowRootParms = false;
					if (ptcptNvc.Count == 0 && objectNvc.Count == 0)
						results = results.Take(0);
				}

				// Audit
				if (results.Count() > 0)
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.Success, AtnaApi.Model.ActionType.Read, AtnaApi.Model.EventIdentifierType.Query, AtnaApi.Model.AuditableObjectLifecycle.Disclosure, results.Skip(pageId * resultsPerPage).Take(resultsPerPage).ToArray());

				return PartialView(new AuditSummaryCollectionViewModel(results));
			}
			catch (Exception e)
			{
				AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.MinorFail, AtnaApi.Model.ActionType.Read, AtnaApi.Model.EventIdentifierType.Query, AtnaApi.Model.AuditableObjectLifecycle.Disclosure, new List<Object>() { e });
				return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);
			}
		}

		/// <summary>
		/// Query
		/// </summary>
		private IQueryable<T> DoQuery<T>(IQueryable<T> source, NameValueCollection nvc, bool sort = true)
		{
			// Lamba parameter
			ParameterExpression parameter = Expression.Parameter(typeof(T), "a");
			List<KeyValuePair<String, Expression>> expressions = new List<KeyValuePair<string, Expression>>();

			// Loop through keys adding filters
			foreach (String key in nvc.Keys)
			{
				var physicalParameterName = key;
				while (physicalParameterName.StartsWith("@"))
					physicalParameterName = physicalParameterName.Substring(1);

				var keyProperty = typeof(T).GetProperty(physicalParameterName);
				// We have an equal expression so add it to the lambda
				if (keyProperty != null)
				{
					if (keyProperty.PropertyType == typeof(DateTime))
						foreach (var rawValue in nvc[key].Split(','))
						{
							if (rawValue.StartsWith("~")) // Estimate (between)
							{
								DateTime value = DateTime.Parse(rawValue.Substring(1));
								var left = Expression.Property(parameter, key);
								var rightLower = Expression.Constant(value.Date, keyProperty.PropertyType);
								var rightUpper = Expression.Constant(value.Date.AddDays(1), keyProperty.PropertyType);
								expressions.Add(new KeyValuePair<string, Expression>(key, Expression.AndAlso(Expression.GreaterThanOrEqual(left, rightLower), Expression.LessThan(left, rightUpper))));
							}
							else
							{
								var expr = this.BuildExpression<T>(rawValue, keyProperty, parameter);
								if (expr == null) continue;
								expressions.Add(new KeyValuePair<string, Expression>(key, expr));

								//// Now determine
								//DateTime value = DateTime.Parse(rawValue);
								//var left = Expression.Property(parameter, key);
								//var right = Expression.Constant(value, keyProperty.PropertyType);
								//expressions.Add(new KeyValuePair<string, Expression>(key, Expression.Equal(left, right)));
							}
						}
					else
						// Values ... yay!
						foreach (var rawValue in nvc[key].Split(','))
						{
							var expr = this.BuildExpression<T>(rawValue, keyProperty, parameter);
							if (expr == null) continue;
							expressions.Add(new KeyValuePair<string, Expression>(key, expr));
						}
				}
			}

			// Now sort by parameter name, OR common parameters, AND separate parameters
			Expression body = null;
			if (expressions.Count > 0) // do some heavy lifting
			{
				// Group common expressions
				var groupParms = expressions.GroupBy(p => p.Key);
				foreach (var grp in groupParms)
				{
					Expression orExpression = null;
					foreach (var memb in grp)
						if (orExpression == null)
							orExpression = memb.Value;
						else
							orExpression = Expression.OrElse(orExpression, memb.Value);
					if (body == null)
						body = orExpression;
					else
						body = Expression.AndAlso(body, orExpression);
				}
			}

			// Order by?
			IQueryable<T> records = null;
			if (body == null)
				records = source;
			else
				records = source.Where(Expression.Lambda<Func<T, bool>>(body, new ParameterExpression[] { parameter }));

			var sortField = nvc["sort"];
			var sortDirection = nvc["sortdir"];

			// Sort specified so use the default of newest first
			if (sortField == null)
			{
				sortField = "CreationTimestamp";
				sortDirection = "DESC";
			}
			// Order by clause?
			var pi = typeof(T).GetProperty(sortField);
			if (pi != null && sort)
			{
				var sortExpression = Expression.Property(parameter, sortField);
				var func = typeof(Func<,>).MakeGenericType(typeof(T), pi.PropertyType);
				var lamda = Expression.Lambda(func, sortExpression, parameter);
				string orderMethod = "OrderBy";
				if (sortDirection == "DESC")
					orderMethod = "OrderByDescending";

				records = (IQueryable<T>)typeof(Queryable).GetMethods().Single(
									method => method.Name == orderMethod
												&& method.IsGenericMethodDefinition
												&& method.GetGenericArguments().Length == 2
												&& method.GetParameters().Length == 2)
									.MakeGenericMethod(typeof(T), pi.PropertyType)
									.Invoke(null, new object[] { records, lamda });
			}

			return records;
		}

		/// <summary>
		/// Build expression for a key
		/// </summary>
		/// <param name="keyValue"></param>
		/// <param name="keyProperty"></param>
		/// <param name="parameter"></param>
		/// <returns></returns>
		private Expression BuildExpression<T>(string keyValue, System.Reflection.PropertyInfo keyProperty, ParameterExpression parameter)
		{
			// Prepare query parameters
			Dictionary<char, string> mods = new Dictionary<char, string>() {
					{ '!', "NotEqual" },
					{ '~', "Contains" },
					{ '<', "LessThan" },
					{ '>', "GreaterThan" }
					 };

			String value = keyValue;
			if (value.Length == 0) return null;
			String methodName = "";
			if (mods.TryGetValue(value[0], out methodName))
				value = value.Substring(1);
			else
				methodName = "Equal";
			Expression left = Expression.Property(parameter, keyProperty.Name);

			// Convert type?
			object rValue = value;
			Expression right = null;
			if (MARC.Everest.Connectors.Util.TryFromWireFormat(value, keyProperty.PropertyType, out rValue))
				right = Expression.Constant(rValue, keyProperty.PropertyType);

			// Invoke the appropriate method
			var mi = typeof(Expression).GetMethod(methodName, new Type[] { typeof(Expression), typeof(Expression) });
			if (mi == null)
			{
				mi = keyProperty.PropertyType.GetMethod(methodName, new Type[] { keyProperty.PropertyType });
				if (mi == null)
					return null;
				return Expression.Call(left, mi, right);
			}
			else
				return mi.Invoke(null, new object[] { left, right }) as Expression;
		}

		// GET: /Audit/View/{id}
		public ActionResult View(Int32 id)
		{
			using (var context = new AuditModelDataContext())
			{
				try
				{
					var audit = context.AuditSummaryVws.FirstOrDefault(a => a.AuditId == id);
					if (audit == null)
						throw new FileNotFoundException();

					int? version = null;
					if (audit.StatusCode == "NEW")
					{
						context.sp_SetAuditStatus(id, "ACTIVE", null, User.Identity.Name, ref version);
						AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.Success, AtnaApi.Model.ActionType.Update, AtnaApi.Model.EventIdentifierType.ResourceAssignment, AtnaApi.Model.AuditableObjectLifecycle.Amendment, new List<Object>() { audit, context.AuditStatus.First(s => s.AuditVersionId == version) });
					}
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.Success, AtnaApi.Model.ActionType.Read, AtnaApi.Model.EventIdentifierType.Disclosure, AtnaApi.Model.AuditableObjectLifecycle.Disclosure, new List<Object>() { audit });
					Response.CacheControl = "no-cache";
					Response.AddHeader("Pragma", "no-cache");
					Response.Expires = -1;

					// Show the audit
					return PartialView(new AuditViewModel(id, audit, AuditUtil.LoadAudit(audit.GlobalId)));
				}
				catch (FileNotFoundException)
				{
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.EpicFail, AtnaApi.Model.ActionType.Read, AtnaApi.Model.EventIdentifierType.Disclosure, AtnaApi.Model.AuditableObjectLifecycle.Disclosure, new List<Object>());
					return HttpNotFound();
				}
				catch (Exception)
				{
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.EpicFail, AtnaApi.Model.ActionType.Read, AtnaApi.Model.EventIdentifierType.Disclosure, AtnaApi.Model.AuditableObjectLifecycle.Disclosure, new List<Object>());
					return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError);
				}
			}
		}

		// POST: /Audit/ArchiveAll
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult ArchiveAll()
		{
			using (var entityContext = new AuditModelDataContext())
			{
				List<Object> auditObject = new List<object>();

				try
				{
					var records = this.DoQuery(entityContext.AuditSummaryVws, Request.Form);
					int c = 0;
					var fixedRecords = records.ToList();
					foreach (var rec in fixedRecords)
					{
						auditObject.Add(rec);
						Int32? auditVersion = null;
						entityContext.sp_SetAuditStatus(rec.AuditId, "ARCHIVED", null, User.Identity.Name, ref auditVersion);
						auditObject.Add(entityContext.AuditStatus.First(p => p.AuditVersionId == auditVersion));

						if (++c % 20 == 0)
						{
							entityContext.SubmitChanges();
							AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.Success, AtnaApi.Model.ActionType.Update, AtnaApi.Model.EventIdentifierType.ResourceAssignment, AtnaApi.Model.AuditableObjectLifecycle.Amendment, auditObject);
							auditObject.Clear();
						}
					}
					entityContext.SubmitChanges();
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.Success, AtnaApi.Model.ActionType.Update, AtnaApi.Model.EventIdentifierType.ResourceAssignment, AtnaApi.Model.AuditableObjectLifecycle.Amendment, auditObject);

					return Json("Ok");
				}
				catch
				{
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.SeriousFail, AtnaApi.Model.ActionType.Update, AtnaApi.Model.EventIdentifierType.ResourceAssignment, AtnaApi.Model.AuditableObjectLifecycle.Amendment, auditObject);
					return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Internal Server Error");
				}
			}
		}

		// POST: /Audit/DeleteAll
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteAll()
		{
			using (var entityContext = new AuditModelDataContext())
			{
				List<Object> auditObject = new List<object>();

				try
				{
					var records = this.DoQuery(entityContext.AuditSummaryVws, Request.Form);
					int c = 0;
					var fixedRecords = records.ToList();
					foreach (var rec in fixedRecords)
					{
						auditObject.Add(rec);
						Int32? auditVersion = null;
						entityContext.sp_SetAuditStatus(rec.AuditId, "OBSOLETE", false, User.Identity.Name, ref auditVersion);
						auditObject.Add(entityContext.AuditStatus.First(p => p.AuditVersionId == auditVersion));

						if (++c % 20 == 0)
						{
							entityContext.SubmitChanges();
							AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.Success, AtnaApi.Model.ActionType.Delete, AtnaApi.Model.EventIdentifierType.ResourceAssignment, AtnaApi.Model.AuditableObjectLifecycle.LogicalDeletion, auditObject);
							auditObject.Clear();
						}
					}
					entityContext.SubmitChanges();
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.Success, AtnaApi.Model.ActionType.Delete, AtnaApi.Model.EventIdentifierType.ResourceAssignment, AtnaApi.Model.AuditableObjectLifecycle.LogicalDeletion, auditObject);

					return Json("Ok");
				}
				catch
				{
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.SeriousFail, AtnaApi.Model.ActionType.Delete, AtnaApi.Model.EventIdentifierType.ResourceAssignment, AtnaApi.Model.AuditableObjectLifecycle.LogicalDeletion, auditObject);
					return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Internal Server Error");
				}
			}
		}

		// POST: /Audit/Hold/{id}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Hold(Int32 id)
		{
			using (var entityContext = new AuditModelDataContext())
			{
				try
				{
					int? auditVersion = 0;

					entityContext.sp_SetAuditStatus(id, "HELD", null, User.Identity.Name, ref auditVersion);
					entityContext.SubmitChanges();
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.Success, AtnaApi.Model.ActionType.Update, AtnaApi.Model.EventIdentifierType.ResourceAssignment, AtnaApi.Model.AuditableObjectLifecycle.Amendment, new List<Object>() { entityContext.Audits.First(a => a.AuditId == id), entityContext.AuditStatus.First(s => s.AuditVersionId == auditVersion) });
					return Json("Ok");
				}
				catch
				{
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.SeriousFail, AtnaApi.Model.ActionType.Update, AtnaApi.Model.EventIdentifierType.ResourceAssignment, AtnaApi.Model.AuditableObjectLifecycle.Amendment, new List<Object>() { entityContext.Audits.First(a => a.AuditId == id) });
					return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Internal Server Error");
				}
			}
		}

		// POST: /Audit/Delete/{id}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(Int32 id)
		{
			using (var entityContext = new AuditModelDataContext())
			{
				try
				{
					int? auditVersion = 0;
					entityContext.sp_SetAuditStatus(id, "OBSOLETE", false, User.Identity.Name, ref auditVersion);
					entityContext.SubmitChanges();
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.Success, AtnaApi.Model.ActionType.Delete, AtnaApi.Model.EventIdentifierType.ResourceAssignment, AtnaApi.Model.AuditableObjectLifecycle.LogicalDeletion, new List<Object>() { entityContext.Audits.First(a => a.AuditId == id), entityContext.AuditStatus.First(s => s.AuditVersionId == auditVersion) });
					return Json("Ok");
				}
				catch
				{
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.SeriousFail, AtnaApi.Model.ActionType.Delete, AtnaApi.Model.EventIdentifierType.ResourceAssignment, AtnaApi.Model.AuditableObjectLifecycle.LogicalDeletion, new List<Object>() { entityContext.Audits.First(a => a.AuditId == id) });
					return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Internal Server Error");
				}
			}
		}

		// POST: /Audit/Archive/{id}
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Archive(Int32 id)
		{
			using (var entityContext = new AuditModelDataContext())
			{
				try
				{
					int? auditVersion = 0;
					entityContext.sp_SetAuditStatus(id, "ARCHIVED", false, User.Identity.Name, ref auditVersion);
					entityContext.SubmitChanges();
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.Success, AtnaApi.Model.ActionType.Update, AtnaApi.Model.EventIdentifierType.ResourceAssignment, AtnaApi.Model.AuditableObjectLifecycle.Archiving, new List<Object>() { entityContext.Audits.First(a => a.AuditId == id), entityContext.AuditStatus.First(s => s.AuditVersionId == auditVersion) });
					return Json("Ok");
				}
				catch
				{
					AuditUtil.AuditAuditLogUsed(this, AtnaApi.Model.OutcomeIndicator.SeriousFail, AtnaApi.Model.ActionType.Update, AtnaApi.Model.EventIdentifierType.ResourceAssignment, AtnaApi.Model.AuditableObjectLifecycle.Archiving, new List<Object>() { entityContext.Audits.First(a => a.AuditId == id) });
					return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Internal Server Error");
				}
			}
		}

		/// <summary>
		/// Get attribute values
		/// </summary>
		private String[] GetAttributeValues(string propertyName, string term)
		{
			var pi = typeof(AuditDetailVw).GetProperty(propertyName);
			if (pi == null || pi.PropertyType != typeof(String))
				throw new InvalidOperationException("Cannot query on that type of code list");

			var parameter = Expression.Parameter(typeof(AuditDetailVw), "k");
			Expression selectBody = Expression.Property(parameter, pi.Name);
			var selectLambda = Expression.Lambda<Func<AuditDetailVw, String>>(selectBody, parameter);
			var whereBody = Expression.Call(selectBody, typeof(String).GetMethod("Contains"), Expression.Constant(term ?? ""));
			var whereLambda = Expression.Lambda<Func<AuditDetailVw, bool>>(whereBody, parameter);
			using (var entityContext = new AuditModelDataContext())
			{
				var rawValues = entityContext.AuditDetailVws.Where(whereLambda).Select(selectLambda).Distinct().ToArray();
				List<String> values = new List<string>(rawValues);
				for (int i = values.Count - 1; i >= 0; i--)
				{
					values.Add("!" + values[i]);
					values.Add("~" + values[i]);
				}
				return values.ToArray();
			}
		}

		// GET: /Audit/Count
		[HttpGet]
		public ActionResult Count()
		{
			try
			{
				Dictionary<String, Int32> counts = new Dictionary<string, int>();
				using (var entityContext = new AuditModelDataContext())
				{
					foreach (var cd in entityContext.AuditStatusCodeSummaryVws)
						counts.Add(cd.Name, cd.nAudits.Value);
					counts.Add("IsAlert", entityContext.AuditStatus.Count(s => s.IsAlert == true && !s.ObsoletionTimestamp.HasValue));
				}

				return Json(counts, JsonRequestBehavior.AllowGet);
			}
			catch (Exception e)
			{
				AuditUtil.AuditGenericError(this, AtnaApi.Model.OutcomeIndicator.MinorFail, AtnaApi.Model.EventIdentifierType.Query, new AtnaApi.Model.CodeValue<string>("110101", "DCM", "Audit Log Used"), Url.Action("Count"), e);
				return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, "Internal Server Error");
			}
		}
	}
}