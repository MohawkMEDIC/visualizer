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
    public class AuditRepositoryClient : RestClientBase
    {


        /// <summary>
        /// Creates a new instance of the audit repository client
        /// </summary>
        public AuditRepositoryClient()
        {
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
                    this.GetRawAuditMessageCompleted(this, e);
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
                    this.GetAuditMessageCompleted(this, e);
            });
        }


    }
}
