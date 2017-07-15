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
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace MARC.EHRS.Audit
{
    [RunInstaller(true)]
    public class ServiceInstaller : System.Configuration.Install.Installer
    {

        private System.ServiceProcess.ServiceProcessInstaller m_serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller m_serviceInstaller;

        public ServiceInstaller()
        {
            // This call is required by the Designer.
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            this.m_serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.m_serviceInstaller = new System.ServiceProcess.ServiceInstaller();

            this.m_serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.m_serviceProcessInstaller.Password = null;
            this.m_serviceProcessInstaller.Username = null;
            this.m_serviceInstaller.ServiceName = "Visualization Service";
            this.m_serviceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;

            this.Installers.AddRange(
                new System.Configuration.Install.Installer[] 
                { 
                    this.m_serviceProcessInstaller, 
                    this.m_serviceInstaller
                });
        }
    }
}

