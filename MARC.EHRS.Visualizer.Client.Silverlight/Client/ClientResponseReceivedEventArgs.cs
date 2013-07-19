using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MARC.EHRS.Visualizer.Client.Silverlight.Client
{
    /// <summary>
    /// Client response has been received
    /// </summary>
    public class ClientResponseReceivedEventArgs<T> : EventArgs
    {

        /// <summary>
        /// Creates a new client response received 
        /// </summary>
        public ClientResponseReceivedEventArgs(T result)
        {
            this.Result = result;
        }

        /// <summary>
        /// Result that was received
        /// </summary>
        public T Result { get; private set; }
    }
}
