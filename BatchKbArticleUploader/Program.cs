using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using MARC.EHRS.Visualizer.Client.Silverlight.Client;
using MARC.EHRS.VisualizationServer.KnowledgeBase.Contract;
using System.Threading;

namespace BatchKbArticleUploader
{
    class Program
    {
        static Object syncObject = new object();

        static void Main(string[] args)
        {

            Uri uri = null;
            // Look for file
            if (args.Length != 2)
                Console.WriteLine("Need to specify file and url!");
            else if (!File.Exists(args[0]))
                Console.WriteLine("Can't find file!");
            else if (!Uri.TryCreate(args[1], UriKind.Absolute, out uri))
                Console.WriteLine("Invalid URI for service!");
            else
            {
                KnowledgeBaseClient client = new KnowledgeBaseClient();
                client.BaseUri = uri;
                client.PostKbArticleCompleted += new EventHandler<ClientResponseReceivedEventArgs<string>>(client_PostKbArticleCompleted);
                // Load the file
                using (var fs = File.OpenRead(args[0]))
                {
                    XmlSerializer xsz = new XmlSerializer(typeof(ArticleCollection));
                    var collection = xsz.Deserialize(fs) as ArticleCollection;
                    int tArticles = collection.Article.Count, tBatches = 0;
                    while (collection.Article.Count > 0) // Publish in 20 batches
                    {
                        ArticleCollection slice = new ArticleCollection();
                        int serSize = 0;
                        while (serSize < short.MaxValue)
                        {
                            using (MemoryStream ms = new MemoryStream())
                            {
                                if (collection.Article.Count == 0) // no more to take!
                                    break;
                                slice.Article.AddRange(collection.Article.Take(1));
                                collection.Article.RemoveAll(o => slice.Article.Contains(o));
                                xsz.Serialize(ms, slice);
                                ms.Flush();
                                serSize = (int)ms.Length;

                                // Put last back
                                if (serSize > short.MaxValue)
                                {
                                    if (slice.Article.Count > 1)
                                    {
                                        collection.Article.Insert(0, slice.Article.Last());
                                        slice.Article.Remove(slice.Article.Last());
                                    }
                                    break;
                                }
                            }
                        }

                        // Publish
                        lock (syncObject)
                        {
                            foreach (var art in slice.Article)
                                Console.WriteLine("Publishing {0}...", art.KbId);

                            tBatches++;
                            client.PostKbArticleBatch(slice);

                            Monitor.Wait(syncObject);
                        }
                    }
                    Console.WriteLine("Submitted {0} batches with a total of {1} articles", tBatches, tArticles);
                }
            }
        }

        /// <summary>
        /// Publish is completed
        /// </summary>
        static void client_PostKbArticleCompleted(object sender, ClientResponseReceivedEventArgs<string> e)
        {
            Console.WriteLine("Publish result : {0}", e.Result);
            lock(syncObject)
                Monitor.Pulse(syncObject);
        }
    }
}
