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
