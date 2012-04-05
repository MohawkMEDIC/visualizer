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

namespace MARC.EHRS.VisualizationClient.Silverlight.Config
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
