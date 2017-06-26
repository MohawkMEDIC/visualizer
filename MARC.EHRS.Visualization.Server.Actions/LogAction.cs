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
using MARC.EHRS.VisualizationServer.Syslog;
using MARC.EHRS.VisualizationServer.Actions.Configuration;
using System.Configuration;
using System.Diagnostics;
using System.IO;

namespace MARC.EHRS.VisualizationServer.Actions
{
    /// <summary>
    /// Syslog action
    /// </summary>
    public class LogAction : ISyslogAction
    {

        // Lock object for file
        private static object m_syncObject = new object();

        // Configuration of the log action
        private LogActionConfiguration m_configuration;

        /// <summary>
        /// Log action configuration
        /// </summary>
        public LogAction()
        {
            this.m_configuration = ConfigurationManager.GetSection("marc.ehrs.visualizationserver.action.file") as LogActionConfiguration;
        }

        #region ISyslogAction Members

        /// <summary>
        /// Message has been received
        /// </summary>
        public void HandleMessageReceived(object sender, Syslog.TransportProtocol.SyslogMessageReceivedEventArgs e)
        {
            lock (m_syncObject)
            {
                try
                {
                    using(var writer = File.AppendText(this.m_configuration.FileLocation))
                        writer.WriteLine(" {0}\t{1:yyyy-MM-dd HH:mm:ss}\t<{2}>\t{3}\t{4}\t{5}\t{6}", sender.GetType().Name, DateTime.Now, e.Message.Facility, e.SolicitorEndpoint.Host, e.Message.ProcessId, e.Message.ProcessName, e.Message.Original);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Invalid message has been received
        /// </summary>
        public void HandleInvalidMessage(object sender, Syslog.TransportProtocol.SyslogMessageReceivedEventArgs e)
        {
            lock (m_syncObject)
            {
                try
                {
                    using (var writer = File.AppendText(this.m_configuration.FileLocation))
                        writer.WriteLine("*{0}\t{1:yyyy-MM-dd HH:mm:ss}\t{2}\t{3}\t{4}", sender.GetType().Name, DateTime.Now, e.Message.Facility, e.SolicitorEndpoint.Host, e.Message.Original);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.ToString());
                }
            }
        }

        #endregion

        #region IUsesHostContext Members

        /// <summary>
        /// Gets or sets the context
        /// </summary>
        public IServiceProvider Context
        {
            get;
            set;
        }

        #endregion
    }
}
