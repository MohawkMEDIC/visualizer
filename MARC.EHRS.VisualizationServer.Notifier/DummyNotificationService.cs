using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using MARC.EHRS.Visualization.Core.Services;
using MARC.EHRS.Visualization.Core;

namespace MARC.EHRS.VisualizationServer.Notifier
{
    /// <summary>
    /// Represents a dummy notification service
    /// </summary>
    public class DummyNotificationService : INotificationService
    {
        #region INotificationService Members

        /// <summary>
        /// Notify the console of the event
        /// </summary>
        public void Notify(MARC.EHRS.Visualization.Core.VisualizationEvent evt)
        {
            StringWriter sw = new StringWriter();
            XmlSerializer xsz = new XmlSerializer(typeof(VisualizationEvent));
            xsz.Serialize(sw, evt);
            Trace.TraceInformation(sw.ToString());
        }

        #endregion

        #region INotificationService Members

        /// <summary>
        /// Start the dummy notification service
        /// </summary>
        public void Start()
        {
            Trace.TraceInformation("I am starting") ;
        }

        /// <summary>
        /// Stop the dummy notification service
        /// </summary>
        public void Stop()
        {
            Trace.TraceInformation("I am stopping");
        }

        #endregion
    }
}
