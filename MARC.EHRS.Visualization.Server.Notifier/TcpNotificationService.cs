using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using MARC.EHRS.VisualizationServer.Notifier.Configuration;
using System.Configuration;
using MARC.EHRS.Visualization.Core.Services;
using System.Diagnostics;
using System.Threading;
using System.Net;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using MARC.EHRS.Visualization.Core;
using MARC.HI.EHRS.SVC.Core.Services;

namespace MARC.EHRS.VisualizationServer.Notifier
{
    /// <summary>
    /// TCP Notification Service
    /// </summary>
    public class TcpNotificationService : INotificationService
    {

        /// <summary>
        /// Fired when a visualization event has been received
        /// </summary>
        public event EventHandler<VisualizationEventArgs> EventReceived;

        /// <summary>
        /// Event was received
        /// </summary>
        /// <param name="evt"></param>
        public void Notify(MARC.EHRS.Visualization.Core.VisualizationEvent evt)
        {
            if (EventReceived != null)
                EventReceived(this, new VisualizationEventArgs() { Event = evt });

        }


    }
}
