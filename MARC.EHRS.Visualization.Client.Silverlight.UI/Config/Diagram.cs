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
