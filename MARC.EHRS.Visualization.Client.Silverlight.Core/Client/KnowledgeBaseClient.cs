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
using System.IO;
using System.Windows.Markup;
using System.Diagnostics;

namespace MARC.EHRS.Visualization.Client.Silverlight.Client
{
    /// <summary>
    /// Knowledge base client is responsible for retrieving KB posts from service
    /// </summary>
    public class KnowledgeBaseClient : RestClientBase
    {

        /// <summary>
        /// Fired when the KB article collection is completed
        /// </summary>
        public event EventHandler<ClientResponseReceivedEventArgs<String>> GetKbArticleCompleted;

        /// <summary>
        /// Fired when the KB article collection is completed
        /// </summary>
        public event EventHandler<ClientResponseReceivedEventArgs<String>> PostKbArticleCompleted;

        /// <summary>
        /// Get a knowledgebase article 
        /// </summary>
        /// <param name="kbid">The identifier of the artifact to describe</param>
        /// <remarks>Can be anything from a system description to an ITI description. The knowledge base is
        /// stored in ~/KBART and relies on a knowledge base provider</remarks>
        public void GetKbArticleAsync(String kbid)
        {
            
            Uri requestUri = new Uri(String.Format("{0}/kb/{1}", this.BaseUri, kbid));
            this.GetRawAsync(requestUri, delegate(object sender, ClientResponseReceivedEventArgs<Stream> e)
            {
                if (this.GetKbArticleCompleted != null)
                {
                    // Parse the KB
                    try
                    {
                        StreamReader rdr = new StreamReader(e.Result);
                        this.GetKbArticleCompleted(this, new ClientResponseReceivedEventArgs<String>(rdr.ReadToEnd()));
                        
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                        this.GetKbArticleCompleted(this, new ClientResponseReceivedEventArgs<String>(null));
                    }
                }
            });
        }

        /// <summary>
        /// Post a Knowledgebase article
        /// </summary>
        public void PostKbArticle(String kbid, String xaml)
        {
            Uri requestUri = new Uri(String.Format("{0}/kb/{1}", this.BaseUri, kbid));
            // Write data to a stream
            MemoryStream data = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(xaml));
            this.PostRawAsync(requestUri, "application/xaml+xml", data, delegate(object sender, ClientResponseReceivedEventArgs<String> e)
            {
                if (this.PostKbArticleCompleted != null)
                    this.PostKbArticleCompleted(this, e);
            });
        }


    }
}
