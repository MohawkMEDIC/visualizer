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

namespace MARC.EHRS.VisualizationServer.Syslog.Configuration
{
    /// <summary>
    /// Visualizer configuration
    /// </summary>
    public class VisualizerConfigurationSection
    {

        /// <summary>
        /// CTOR for the visualizer configuration section
        /// </summary>
        public VisualizerConfigurationSection()
        {
            this.Endpoints = new List<EndpointConfiguration>();
        }

        /// <summary>
        /// Listener configurations
        /// </summary>
        public List<EndpointConfiguration> Endpoints { get; private set; }


    }
}
