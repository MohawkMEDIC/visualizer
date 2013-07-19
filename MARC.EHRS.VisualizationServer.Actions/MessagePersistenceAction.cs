using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.EHRS.VisualizationServer.Syslog;
using MARC.EHRS.VisualizationServer.Syslog.TransportProtocol;
using System.IO;
using MARC.HI.EHRS.SVC.Core.Services;

namespace MARC.EHRS.VisualizationServer.Actions
{
    /// <summary>
    /// An action that persists the raw message
    /// </summary>
    public class MessagePersistenceAction : ISyslogAction
    {
        #region ISyslogAction Members

        /// <summary>
        /// Handle the receiving of a message
        /// </summary>
        public void HandleMessageReceived(object sender, Syslog.TransportProtocol.SyslogMessageReceivedEventArgs e)
        {
            this.PersistMessage(e);
        }

        /// <summary>
        /// Handle the receiving of an invalid message
        /// </summary>
        public void HandleInvalidMessage(object sender, Syslog.TransportProtocol.SyslogMessageReceivedEventArgs e)
        {
            this.PersistMessage(e);
        }

        /// <summary>
        /// Persist the message
        /// </summary>
        private void PersistMessage(SyslogMessageReceivedEventArgs e)
        {
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(e.Message.Original)))
            {
                IMessagePersistenceService persist = this.Context.GetService(typeof(IMessagePersistenceService)) as IMessagePersistenceService;
                if (persist != null)
                    persist.PersistMessage(e.Message.CorrelationId.ToString(), ms);
            }
        }

        #endregion

        #region IUsesHostContext Members

        /// <summary>
        /// Gets or sets the host context
        /// </summary>
        public IServiceProvider Context
        {
            get;
            set;
        }

        #endregion
    }
}
