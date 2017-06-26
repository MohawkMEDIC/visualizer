﻿/*
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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using MARC.HI.EHRS.SVC.Core.Services;
using MARC.HI.EHRS.SVC.Core;

namespace MARC.EHRS.Audit
{
    partial class VisualizationService : ServiceBase
    {

        // Start the message handler service
        IMessageHandlerService m_messageHandlerService = null;

        public VisualizationService()
        {
            // Service Name
            this.ServiceName = "Visualization Service";
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Trace.CorrelationManager.ActivityId = typeof(Program).GUID;
            Trace.TraceInformation("Starting host context on Console Presentation System at {0}", DateTime.Now);

            // Detect platform
            if (System.Environment.OSVersion.Platform != PlatformID.Win32NT)
                Trace.TraceWarning("Not running on WindowsNT, some features may not function correctly");

            // Do this because loading stuff is tricky ;)
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(Program.CurrentDomain_AssemblyResolve);

            try
            {
                // Initialize 
                HostContext context = new HostContext();

                Trace.TraceInformation("Getting default message handler service.");
                m_messageHandlerService = context.GetService(typeof(IMessageHandlerService)) as IMessageHandlerService;

                if (m_messageHandlerService == null)
                    Trace.TraceError("PANIC! Can't find a default message handler service: {0}", "No IMessageHandlerService classes are registered with this host context");
                else
                {
                    Trace.TraceInformation("Starting message handler service {0}", m_messageHandlerService);
                    if (m_messageHandlerService.Start())
                    {
                        Trace.TraceInformation("Service Started Successfully");
                        ExitCode = 0;
                    }
                    else
                    {
                        Trace.TraceError("No message handler service started. Terminating program");
                        ExitCode = 1911;
                        Stop();
                    }
                }
            }
            catch (Exception e)
            {
                Trace.TraceError("Fatal exception occurred: {0}", e.ToString());
                ExitCode = 1064;
                Stop();
            }
            finally
            {
            }
            

        }

        protected override void OnStop()
        {
            if (m_messageHandlerService != null)
            {
                Trace.TraceInformation("Stopping message handler service {0}", m_messageHandlerService);
                m_messageHandlerService.Stop();
            }

        }
    }
}
