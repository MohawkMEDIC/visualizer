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
using System.Collections.Generic;
using MARC.EHRS.Visualizer.Client.Silverlight.Contract.Debug;

namespace MARC.EHRS.Visualizer.Client.Silverlight.Client
{
    /// <summary>
    /// Debug service client
    /// </summary>
    public class DebugServiceClient : RestClientBase
    {


        /// <summary>
        /// Fired when the GetRawAuditMessage method is completed
        /// </summary>
        public event EventHandler<ClientResponseReceivedEventArgs<Stream>> GetMessageCompleted;


        /// <summary>
        /// Fired when the GetRawAuditMessage method is completed
        /// </summary>
        public event EventHandler<ClientResponseReceivedEventArgs<Stream>> GetResponseMessageCompleted;

        /// <summary>
        /// Fired when the GetRawAuditMessage method is completed
        /// </summary>
        public event EventHandler<ClientResponseReceivedEventArgs<MessageCollection>> FindMessagesCompleted;

        /// <summary>
        /// Find messages in a specific time range
        /// </summary>
        public void FindMessagesAsync(DateTime from, DateTime to)
        {
            Uri requestUri = new Uri(String.Format("{0}/message?from={1}&to={2}", this.BaseUri, from.ToString("yyyy-MM-ddTHH:mm:sszzz"), to.ToString("yyyy-MM-ddTHH:mm:sszzz")));
            this.GetResourceAsync(requestUri, delegate(object sender, ClientResponseReceivedEventArgs<MessageCollection> e)
            {
                if (this.FindMessagesCompleted != null)
                    this.FindMessagesCompleted(this, e);
            });
        }

        /// <summary>
        /// Get raw message response
        /// </summary>
        public void GetResponseMessageAsync(String messageId)
        {
            Uri requestUri = new Uri(String.Format("{0}/message/{1}/response", this.BaseUri, messageId));
            this.GetRawAsync(requestUri, delegate(object sender, ClientResponseReceivedEventArgs<Stream> e)
            {
                if (this.GetResponseMessageCompleted != null)
                    this.GetResponseMessageCompleted(this, e);
            });
        }

        /// <summary>
        /// Get a raw audit as received from the system
        /// </summary>
        public void GetMessageAsync(String messageId)
        {
            Uri requestUri = new Uri(String.Format("{0}/message/{1}", this.BaseUri, messageId));
            this.GetRawAsync(requestUri, delegate(object sender, ClientResponseReceivedEventArgs<Stream> e)
            {
                if (this.GetMessageCompleted != null)
                    this.GetMessageCompleted(this, e);
            });
        }
    }
}
