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

namespace MARC.EHRS.Visualizer.Client.Silverlight.Client
{
    /// <summary>
    /// Knowledge base client is responsible for retrieving KB posts from service
    /// </summary>
    public class KnowledgeBaseClient : RestClientBase
    {

        /// <summary>
        /// Fired when the KB article collection is completed
        /// </summary>
        public event EventHandler<ClientResponseReceivedEventArgs<TextElement>> GetKbArticleCompleted;

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
                        TextElement data = XamlReader.Load(rdr.ReadToEnd()) as TextElement;
                        this.GetKbArticleCompleted(this, new ClientResponseReceivedEventArgs<TextElement>(data));
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.ToString());
                    }
                }
            });
        }


    }
}
