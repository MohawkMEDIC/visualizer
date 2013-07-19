using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace MARC.EHRS.Visualizer.Client.Silverlight.Client
{
    /// <summary>
    /// RESTful client base
    /// </summary>
    public abstract class RestClientBase
    {
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
        protected void GetResourceAsync<T>(Uri uri, EventHandler<ClientResponseReceivedEventArgs<T>> callback) where T : new()
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
        protected void GetRawAsync(Uri uri, EventHandler<ClientResponseReceivedEventArgs<Stream>> callback)
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

    }
}
