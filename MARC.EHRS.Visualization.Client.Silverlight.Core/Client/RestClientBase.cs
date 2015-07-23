using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;

namespace MARC.EHRS.Visualization.Client.Silverlight.Client
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
        /// Post a raw stream
        /// </summary>
        protected void PostRawAsync(Uri uri, String mimeType, Stream data, EventHandler<ClientResponseReceivedEventArgs<String>> callback)
        {
            WebClient client = new WebClient();
            try
            {
                client.Headers[HttpRequestHeader.ContentType] = mimeType;
                client.OpenWriteCompleted += delegate(object sender, OpenWriteCompletedEventArgs result)
                {
                    try
                    {
                        using (Stream s = result.Result)
                        {
                            byte[] buffer = new byte[1024];
                            int br = 1024;
                            while(br == 1024)
                            {
                                br = data.Read(buffer, 0, 1024);
                                s.Write(buffer, 0, br);
                            }

                            // Raise an event
                            callback(this, new ClientResponseReceivedEventArgs<String>("Ok"));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                        callback(this, new ClientResponseReceivedEventArgs<String>(e.Message));
                    }
                };
                client.OpenWriteAsync(uri, "POST");
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                callback(this, new ClientResponseReceivedEventArgs<String>(e.Message));
            }
        }

        /// <summary>
        /// Get a resource from the specified Uri
        /// </summary>
        protected void GetResourceAsync<T>(Uri uri, EventHandler<ClientResponseReceivedEventArgs<T>> callback) where T : new()
        {
            WebClient client = new WebClient();
            try
            {
                client.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs result)
                {
                    try
                    {
                        using (Stream s = result.Result)
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
                };
                client.OpenReadAsync(new Uri(String.Format("{0}{1}_format=text/xml", uri, !String.IsNullOrEmpty(uri.Query) ? "&" : "?")));
                
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
            WebClient client = new WebClient();
            try
            {
                
                client.OpenReadCompleted += delegate(object sender, OpenReadCompletedEventArgs result)
                {
                    try
                    {
                        using (Stream s = result.Result)
                        {
                            MemoryStream retVal = new MemoryStream();

                            int br = 0;
                            byte[] buffer = new byte[1024];
                            while (s.Position < result.Result.Length)
                            {
                                br = s.Read(buffer, 0, 1024);
                                retVal.Write(buffer, 0, br);
                            }
                            retVal.Seek(0, SeekOrigin.Begin);
                            // Callback
                            callback(this, new ClientResponseReceivedEventArgs<Stream>(retVal));
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.WriteLine(e.ToString());
                        callback(this, new ClientResponseReceivedEventArgs<Stream>(null));
                    }
                };
                client.OpenReadAsync(new Uri(String.Format("{0}{1}_format=application/xaml+xml", uri, !String.IsNullOrEmpty(uri.Query) ? "&" : "?")));

            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
                callback(this, new ClientResponseReceivedEventArgs<Stream>(null));
            }
        }

    }
}
