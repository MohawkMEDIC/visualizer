using System;
using System.Net;

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
