/*
 * Copyright 2012-2017 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2012-6-15
 */
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
