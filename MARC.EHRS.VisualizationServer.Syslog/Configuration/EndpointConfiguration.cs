using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MARC.EHRS.VisualizationServer.Syslog.Configuration
{
    /// <summary>
    /// Listener configuration
    /// </summary>
    /// <remarks>
    /// The configuration for an endpoint:
    /// <example lang="xml">
    /// <![CDATA[
    ///     <endpoint name="{unique name}" address="{address}">
    ///         <attribute name="{attribute_name}" value="{attribute_value}"/>
    ///         <forward name="{unique name}" address="{address}">
    ///             <attribute name="{attribute_name}" value="{attribute_value}"/>
    ///         </forward>
    ///     </endpoint>
    /// ]]>
    /// </example>
    /// 
    /// </remarks>
    public class EndpointConfiguration
    {

        /// <summary>
        /// Creates a new endpoint configuration
        /// </summary>
        public EndpointConfiguration()
        {
            this.Actions = new List<Type>();
            this.Forward = new List<EndpointConfiguration>();
        }

        /// <summary>
        /// The address to listen on
        /// </summary>
        public Uri Address { get; set; }

        /// <summary>
        /// The name of the audit endpoint
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Maximum message size
        /// </summary>
        public int MaxSize { get; set; }

        /// <summary>
        /// Gets the handler of this endpoint
        /// </summary>
        public List<Type> Actions { get; private set; }

        /// <summary>
        /// The forwarding addresses
        /// </summary>
        public List<EndpointConfiguration> Forward { get; private set; }

        /// <summary>
        /// The list of additional attributes
        /// </summary>
        public List<KeyValuePair<String, String>> Attributes { get; set; }
    }
}
