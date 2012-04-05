using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MARC.EHRS.Visualization.Core.Services
{
    /// <summary>
    /// Notifies 
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Notify of an event
        /// </summary>
        void Notify(VisualizationEvent evt);

        /// <summary>
        /// Start the notification service
        /// </summary>
        void Start();

        /// <summary>
        /// Stop the notification service
        /// </summary>
        void Stop();
    }
}
