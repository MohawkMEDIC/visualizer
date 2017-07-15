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
using System.Xml.Serialization;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Net;
using System.IO;

namespace MARC.EHRS.Visualization.Client.Silverlight.UI.Config
{
    /// <summary>
    /// Diagram class
    /// </summary>
    public class Diagram
    {

        private string m_src;
        private string m_controlSource;

        [XmlAttribute("src")]
        public string Src
        {
            get { return m_src; }
            set
            {
                m_src = value;
                WebClient wc = new WebClient();
                wc.OpenReadCompleted += new OpenReadCompletedEventHandler(wc_OpenReadCompleted);
                wc.OpenReadAsync(new Uri(m_src));
            }
        }

        /// <summary>
        /// Read the diagram
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void wc_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                StreamReader sr = new StreamReader(e.Result);
                try
                {
                    m_controlSource = sr.ReadToEnd();
                }
                finally
                {
                    sr.Dispose();
                }
            }
        }

        [XmlAttribute("name")]
        public string Name { get; set; }
        
        [XmlText]
        public string Description { get; set; }


        /// <summary>
        /// The Diagram
        /// </summary>
        [XmlIgnore]
        public UserControl PreviewControl
        {
            get { return CreateControl() as UserControl; }
        }

        /// <summary>
        /// Create control
        /// </summary>
        internal object CreateControl()
        {
            if(m_controlSource != null)
                return XamlReader.Load(m_controlSource);
            return new TextBlock() { Text = "Error" };
        }
    }
}
