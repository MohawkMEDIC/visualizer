using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Web;
using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Data;
using System.Diagnostics;
using System.IO;

namespace MARC.EHRS.VisualizationServer.KnowledgeBase
{
    /// <summary>
    /// KB service behavior
    /// </summary>
    public class KnowledgeBaseServiceBehavior : IKnowledgeBaseServiceContract
    {

        #region IKnowledgeBaseServiceContract Members

        /// <summary>
        /// Get a KB article from the database
        /// </summary>
        public System.IO.Stream GetKbArticle(string kbid)
        {
            // HACK: This is bad form putting SQL code directly into the web service call but meh
            // TODO: De-hackify this


            // Message properties
            MessageProperties properties = OperationContext.Current.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string remoteEndpoint = "http://anonymous";
            if (endpoint != null)
                remoteEndpoint = endpoint.Address;
            string mimeType = WebOperationContext.Current.IncomingRequest.ContentType;

            Trace.TraceInformation("{1} fetching KB Article {0}...", kbid, remoteEndpoint);

            // Now store
            try
            {
                using (IDbConnection conn = KnowledgeBaseListener.Configuration.CreateConnection())
                {
                    conn.Open();
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "fnd_kb_art";
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parameters
                        IDataParameter pKey = cmd.CreateParameter();
                        pKey.Direction = ParameterDirection.Input;
                        pKey.DbType = DbType.String;
                        pKey.ParameterName = "kb_key_in";
                        pKey.Value = kbid;

                        // Add parameters
                        cmd.Parameters.Add(pKey);

                        // Execute
                        using (IDataReader rdr = cmd.ExecuteReader())
                            if (rdr.Read())
                            {
                                WebOperationContext.Current.OutgoingResponse.ContentType = Convert.ToString(rdr["kb_type"]);
                                WebOperationContext.Current.OutgoingResponse.LastModified = Convert.ToDateTime(rdr["kb_lup_utc"] == DBNull.Value ? rdr["kb_crt_utc"] : rdr["kb_lup_utc"]);
                                WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "max-age=3600");
                                WebOperationContext.Current.OutgoingResponse.Headers.Add("Expires", DateTime.Now.AddMilliseconds(3600).ToString());
                                return new MemoryStream((byte[])rdr["kb_text"]);
                            }
                            else
                                throw new WebFaultException(System.Net.HttpStatusCode.NotFound);
                    }
                }
            }
            catch (WebFaultException)
            {
                throw;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw new WebFaultException(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Publish a KB article to the database
        /// </summary>
        public void PublishKbArticle(string kbid, System.IO.Stream data)
        {
        
            // HACK: This is bad form putting SQL code directly into the web service call but meh
            // TODO: De-hackify this

            // First, are we allowed to post?
            try
            {

                long dataLength = WebOperationContext.Current.IncomingRequest.ContentLength;

                if (!KnowledgeBaseListener.Configuration.AllowPost)
                    throw new WebFaultException(System.Net.HttpStatusCode.MethodNotAllowed);
                else if (dataLength > short.MaxValue)
                    throw new WebFaultException(System.Net.HttpStatusCode.RequestEntityTooLarge);

                // Get all data as byte array
                byte[] bufferedData = new byte[dataLength];
                data.Read(bufferedData, 0, (int)dataLength);

                // Message properties
                MessageProperties properties = OperationContext.Current.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                string remoteEndpoint = "anonymous";
                if(endpoint != null)
                    remoteEndpoint = endpoint.Address;
                string mimeType = WebOperationContext.Current.IncomingRequest.ContentType;

                Trace.TraceInformation("{1} publishing KB Article {0}...", kbid, remoteEndpoint);

                // Now store
           
                using (IDbConnection conn = KnowledgeBaseListener.Configuration.CreateConnection())
                {
                    conn.Open();
                    using (IDbCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "pub_kb_art";
                        cmd.CommandType = CommandType.StoredProcedure;

                        // Parameters
                        IDataParameter pKey = cmd.CreateParameter(),
                            pType = cmd.CreateParameter(),
                            pData = cmd.CreateParameter(),
                            pAut = cmd.CreateParameter();
                        pKey.Direction = pType.Direction = pData.Direction = pAut.Direction = ParameterDirection.Input;
                        pKey.DbType = pType.DbType = pAut.DbType = DbType.String;
                        pData.DbType = DbType.Binary;
                        pKey.ParameterName = "kb_key_in";
                        pKey.Value = kbid;
                        pType.ParameterName = "kb_type_in";
                        pType.Value = mimeType;
                        pData.ParameterName = "kb_text_in";
                        pData.Value = bufferedData;
                        pAut.ParameterName = "kb_aut_in";
                        pAut.Value = remoteEndpoint;

                        // Add parameters
                        cmd.Parameters.Add(pKey);
                        cmd.Parameters.Add(pType);
                        cmd.Parameters.Add(pData);
                        cmd.Parameters.Add(pAut);

                        // Execute
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (WebFaultException)
            {
                throw;
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
                throw new WebFaultException(System.Net.HttpStatusCode.InternalServerError);
            }

        }

        #endregion
    }
}
