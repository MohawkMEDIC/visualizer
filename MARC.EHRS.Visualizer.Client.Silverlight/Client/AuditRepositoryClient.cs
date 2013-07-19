using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using MARC.EHRS.Visualizer.Client.Silverlight.Contract.Atna;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using System.Windows.Threading;
using System.Diagnostics;
using System.IO;

namespace MARC.EHRS.Visualizer.Client.Silverlight.Client
{
    /// <summary>
    /// Audit repository service core client
    /// </summary>
    public class AuditRepositoryClient
    {

        // Dispatcher
        private Dispatcher m_dispatcher;

        /// <summary>
        /// Creates a new instance of the audit repository client
        /// </summary>
        public AuditRepositoryClient(Dispatcher dispatcher)
        {
            this.m_dispatcher = dispatcher;
        }

        /// <summary>
        /// Gets or sets the base url of the restful audit repository service
        /// </summary>
        public Uri BaseUri { get; set; }

        /// <summary>
        /// Client credential
        /// </summary>
        public X509Certificate ClientCredential { get; set; }

        /// <summary>
        /// Get a resource from the specified Uri
        /// </summary>
        private void GetResourceAsync<T>(Uri uri, EventHandler<ClientResponseReceivedEventArgs<T>> callback) where T : new()
        {
            HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;
            try
            {
                request.Method = "GET";
                request.UserAgent = "x-visualizer-arclient";
                request.BeginGetResponse((AsyncCallback)delegate(IAsyncResult asyncResult)
                {
                    try
                    {
                        var response = request.EndGetResponse(asyncResult);
                        using (Stream s = response.GetResponseStream())
                        {
                            T retVal = default(T);

                            // Response stream processing
                            XmlSerializer xsz = new XmlSerializer(typeof(T));
                            retVal = (T)xsz.Deserialize(s);

                            // Callback
                            callback(this, new ClientResponseReceivedEventArgs<T>(retVal));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                        callback(this, new ClientResponseReceivedEventArgs<T>(default(T)));
                    }

                }, null);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                callback(this, new ClientResponseReceivedEventArgs<T>(default(T)));
            }
        }

        /// <summary>
        /// Get a resource from the specified Uri
        /// </summary>
        private void GetRawAsync(Uri uri, EventHandler<ClientResponseReceivedEventArgs<Stream>> callback) 
        {
            HttpWebRequest request = HttpWebRequest.Create(uri) as HttpWebRequest;
            try
            {
                request.Method = "GET";
                request.UserAgent = "x-visualizer-arclient";
                request.BeginGetResponse((AsyncCallback)delegate(IAsyncResult asyncResult)
                {
                    try
                    {
                        var response = request.EndGetResponse(asyncResult);
                        using (Stream s = response.GetResponseStream())
                        {
                            MemoryStream retVal = new MemoryStream();

                            int br = 0;
                            byte[] buffer = new byte[1024];
                            while (s.Position < response.ContentLength)
                            {
                                br = s.Read(buffer, 0, 1024);
                                retVal.Write(buffer, 0, br);
                            }
                            // Callback
                            callback(this, new ClientResponseReceivedEventArgs<Stream>(retVal));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                        callback(this, new ClientResponseReceivedEventArgs<Stream>(null));
                    }

                }, null);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                callback(this, new ClientResponseReceivedEventArgs<Stream>(null));
            }
        }

        /// <summary>
        /// Fired when the GetRawAuditMessage method is completed
        /// </summary>
        public event EventHandler<ClientResponseReceivedEventArgs<Stream>> GetRawAuditMessageCompleted;

        /// <summary>
        /// Get a raw audit as received from the system
        /// </summary>
        public void GetRawAuditMessageAsync(Guid correlationId)
        {
            Uri requestUri = new Uri(String.Format("{0}/message/{1}", this.BaseUri, correlationId));
            this.GetRawAsync(requestUri, delegate(object sender, ClientResponseReceivedEventArgs<Stream> e)
            {
                if (this.GetRawAuditMessageCompleted != null)
                    this.m_dispatcher.BeginInvoke(GetRawAuditMessageCompleted, this, e);
            });
        }

        /// <summary>
        /// Fired when the GetRawAuditMessage method is completed
        /// </summary>
        public event EventHandler<ClientResponseReceivedEventArgs<AuditMessage>> GetAuditMessageCompleted;

        /// <summary>
        /// Get the interpreted audit message from the system
        /// </summary>
        public void GetAuditMessageAsync(Guid correlationId)
        {
            Uri requestUri = new Uri(String.Format("{0}/audit/{1}", this.BaseUri, correlationId));
            this.GetResourceAsync<AuditMessage>(requestUri, delegate(object sender, ClientResponseReceivedEventArgs<AuditMessage> e)
            {
                if (this.GetAuditMessageCompleted != null)
                    this.m_dispatcher.BeginInvoke(GetAuditMessageCompleted, this, e);
            });
        }


    }
}
