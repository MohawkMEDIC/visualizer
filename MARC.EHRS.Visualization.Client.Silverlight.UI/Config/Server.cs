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
using System.Windows.Media.Imaging;
using System.Net;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using MARC.EHRS.Visualization.Client.Silverlight.UI.Core;

namespace MARC.EHRS.Visualization.Client.Silverlight.UI.Config
{
    public class Server
    {

        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("address")]
        public string Address { get; set; }
        [XmlAttribute("port")]
        public int Port { get; set; }
        [XmlAttribute("imageSrc")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// Create the visualizer client
        /// </summary>
        public VisualizerClient CreateClient(Dispatcher owner)
        {
            EndPoint ep = null;
            IPAddress ip = null;

            if (IPAddress.TryParse(Address, out ip))
                ep = new IPEndPoint(ip, Port);
            else
                ep = new DnsEndPoint(Address, Port);
            return VisualizerClient.CreateClient(ep, owner);
        }

        /// <summary>
        /// Preview control
        /// </summary>
        [XmlIgnore]
        public Image PreviewControl
        {
            get
            {
                if (String.IsNullOrEmpty(ImageUrl))
                    return null;
                Image img = new Image();
                img.Source = new BitmapImage(new Uri(ImageUrl, UriKind.RelativeOrAbsolute));
                img.Stretch = System.Windows.Media.Stretch.UniformToFill;
                return img;
            }
            set { }
        }
    }
}
