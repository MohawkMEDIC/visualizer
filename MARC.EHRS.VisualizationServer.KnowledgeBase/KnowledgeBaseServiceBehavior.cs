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
using MARC.EHRS.VisualizationServer.KnowledgeBase.Contract;
using System.Xml;

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
                                WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
                                WebOperationContext.Current.OutgoingResponse.Headers.Add("Expires", DateTime.Now.AddMilliseconds(3600).ToString());
                                WebOperationContext.Current.OutgoingResponse.ETag = Guid.NewGuid().ToString();
                                WebOperationContext.Current.OutgoingResponse.LastModified = DateTime.Now;
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
        public void PublishKbArticle(string kbid, XmlElement data)
        {
        
            // HACK: This is bad form putting SQL code directly into the web service call but meh
            // TODO: De-hackify this

            // First, are we allowed to post?
            try
            {

                long dataLength = WebOperationContext.Current.IncomingRequest.ContentLength;

                if (!KnowledgeBaseListener.Configuration.AllowPost)
                    throw new WebFaultException(System.Net.HttpStatusCode.MethodNotAllowed);
                else if (dataLength > ushort.MaxValue)
                    throw new WebFaultException(System.Net.HttpStatusCode.RequestEntityTooLarge);

                // Get all data as byte array
               

                // Message properties
                MessageProperties properties = OperationContext.Current.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                string remoteEndpoint = "anonymous";
                if(endpoint != null)
                    remoteEndpoint = endpoint.Address;
                string mimeType = WebOperationContext.Current.IncomingRequest.ContentType;

                Trace.TraceInformation("{1} publishing KB Article {0}...", kbid, remoteEndpoint);
                this.PublishArticle(new Article()
                {
                    Xaml = data,
                    Type = mimeType,
                    KbId = kbid
                }, remoteEndpoint);
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
        /// Find articles
        /// </summary>
        public Contract.ArticleCollection FindArticles()
        {
            // HACK: This is bad form putting SQL code directly into the web service call but meh
            // TODO: De-hackify this


            // Message properties
            ArticleCollection retVal = new ArticleCollection();
            MessageProperties properties = OperationContext.Current.IncomingMessageProperties;
            RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            string remoteEndpoint = "http://anonymous";
            if (endpoint != null)
                remoteEndpoint = endpoint.Address;

            Trace.TraceInformation("{0} searching KB Articles...", remoteEndpoint);
            WebOperationContext.Current.OutgoingResponse.ContentType = "text/xml";
            WebOperationContext.Current.OutgoingResponse.ETag = Guid.NewGuid().ToString();
            WebOperationContext.Current.OutgoingResponse.LastModified = DateTime.Now;
            WebOperationContext.Current.OutgoingResponse.Headers.Add("Cache-Control", "no-cache");
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
                        // Parameters
                        IDataParameter pKey = cmd.CreateParameter(),
                            pType = cmd.CreateParameter(),
                            pLup = cmd.CreateParameter(),
                            pAut = cmd.CreateParameter();
                        pKey.Direction = pType.Direction = pLup.Direction = pAut.Direction = ParameterDirection.Input;
                        pKey.DbType = pType.DbType = pAut.DbType = DbType.String;
                        pLup.DbType = DbType.DateTime;
                        pKey.ParameterName = "kb_key_in";
                        pType.ParameterName = "kb_type_in";
                        pLup.ParameterName = "kb_lup_in";
                        pAut.ParameterName = "aut_in";

                        // Filter - KBID
                        string filter = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters.Get("kbid");
                        if (!String.IsNullOrEmpty(filter))
                            pKey.Value = filter;
                        else
                            pKey.Value = DBNull.Value;

                        // Filter - format
                        filter = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters.Get("format");
                        if (!String.IsNullOrEmpty(filter))
                            pType.Value = filter;
                        else
                            pType.Value = DBNull.Value;

                        // Filter - LUP
                        filter = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters.Get("lup_utc");
                        if (!String.IsNullOrEmpty(filter))
                            pLup.Value = DateTime.Parse(filter);
                        else
                            pLup.Value = DBNull.Value;

                        // Filter - AUT
                        filter = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters.Get("aut");
                        if (!String.IsNullOrEmpty(filter))
                            pAut.Value = filter;
                        else
                            pAut.Value = DBNull.Value;

                        // Add parameters
                        cmd.Parameters.Add(pKey);
                        cmd.Parameters.Add(pType);
                        cmd.Parameters.Add(pLup);
                        cmd.Parameters.Add(pAut);

                        // Execute
                        using (IDataReader rdr = cmd.ExecuteReader())
                            while (rdr.Read())
                            {
                                Article art = new Article()
                                {
                                    KbId = Convert.ToString(rdr["kb_key"]),
                                    Type = Convert.ToString(rdr["kb_type"])
                                };
                                using (var ms = new MemoryStream((byte[])rdr["kb_text"]))
                                {
                                    XmlDocument doc = new XmlDocument();
                                    doc.Load(ms);
                                    art.Xaml = doc.DocumentElement;
                                }
                                retVal.Article.Add(art);
                            }
                        return retVal;
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
        /// Publish KB articles
        /// </summary>
        public void PublishKbArticleBatch(ArticleCollection data)
        {
            // HACK: This is bad form putting SQL code directly into the web service call but meh
            // TODO: De-hackify this

            // First, are we allowed to post?
            try
            {

                long dataLength = WebOperationContext.Current.IncomingRequest.ContentLength;

                if (!KnowledgeBaseListener.Configuration.AllowPost)
                    throw new WebFaultException(System.Net.HttpStatusCode.MethodNotAllowed);
                else if (dataLength > int.MaxValue)
                    throw new WebFaultException(System.Net.HttpStatusCode.RequestEntityTooLarge);

                // Get all data as byte array


                // Message properties
                MessageProperties properties = OperationContext.Current.IncomingMessageProperties;
                RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                string remoteEndpoint = "anonymous";
                if (endpoint != null)
                    remoteEndpoint = endpoint.Address;

                foreach (var art in data.Article)
                {
                    Trace.TraceInformation("{1} publishing KB Article {0}...", art.KbId, remoteEndpoint);
                    this.PublishArticle(art, remoteEndpoint);
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


        private void PublishArticle(Article art, string aut)
        {
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
                    pKey.Value = art.KbId;
                    pType.ParameterName = "kb_type_in";
                    pType.Value = art.Type;
                    pData.ParameterName = "kb_text_in";
                    pData.Value = System.Text.Encoding.UTF8.GetBytes(art.Xaml.OuterXml);
                    pAut.ParameterName = "kb_aut_in";
                    pAut.Value = aut;

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
    }
}
