using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.EHRS.VisualizationServer.Syslog.TransportProtocol;
using MARC.HI.EHRS.SVC.Core.Services;

namespace MARC.EHRS.VisualizationServer.Syslog
{
    /// <summary>
    /// Represents a class that can handle syslog via a series of actions
    /// </summary>
    public interface ISyslogAction : IUsesHostContext
    {
        /// <summary>
        /// Handle a message received event
        /// </summary>
        void HandleMessageReceived(object sender, SyslogMessageReceivedEventArgs e);

        /// <summary>
        /// Handle an invalid message
        /// </summary>
        void HandleInvalidMessage(object sender, SyslogMessageReceivedEventArgs e);

    }
}
