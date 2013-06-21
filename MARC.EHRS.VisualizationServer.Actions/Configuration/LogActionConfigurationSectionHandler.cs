using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Xml;

namespace MARC.EHRS.VisualizationServer.Actions.Configuration
{
    /// <summary>
    /// Log action configuration section handler
    /// </summary>
    public class LogActionConfigurationSectionHandler : IConfigurationSectionHandler
    {
        #region IConfigurationSectionHandler Members

        /// <summary>
        /// Create the configuration section
        /// </summary>
        public object Create(object parent, object configContext, System.Xml.XmlNode section)
        {
            XmlAttribute outputAtt = section.SelectSingleNode("./*[local-name() = 'output']/@file") as XmlAttribute;
            return new LogActionConfiguration() { FileLocation = outputAtt.Value };
        }

        #endregion
    }
}
