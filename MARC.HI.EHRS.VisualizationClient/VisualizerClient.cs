using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Xml.Serialization;
using MARC.EHRS.Visualization.Core;
using System.IO;
using System.Windows.Threading;

namespace MARC.EHRS.VisualizationClient
{
    /// <summary>
    /// Represents a client connection to the visualizer
    /// </summary>
    public class VisualizerClient
    {

        /// <summary>
        /// Visualizer client
        /// </summary>
        private VisualizerClient()
        {
        }

        // The client connection
        private Socket m_clientConnection;

        // Dispatcher
        private Dispatcher m_dispatcher;

        /// <summary>
        /// Visualization serializer
        /// </summary>
        private XmlSerializer m_visualizationSerializer = new XmlSerializer(typeof(VisualizationEvent));

        /// <summary>
        /// Gets or sets the endpoint to the server
        /// </summary>
        public EndPoint ServerEndpoint { get; private set; }

        /// <summary>
        /// Returns true if the client is currently connected to a visualizer
        /// </summary>
        public bool IsConnected
        {
            get
            {
                return m_clientConnection != null &&
                    m_clientConnection.Connected;
            }
        }

        /// <summary>
        /// Create a visualizer client
        /// </summary>
        public static VisualizerClient CreateClient(EndPoint serverEndpoint)
        {
            VisualizerClient retVal = new VisualizerClient();
            retVal.ServerEndpoint = serverEndpoint;
            retVal.m_dispatcher = Dispatcher.CurrentDispatcher;

            return retVal;
        }

        /// <summary>
        /// Opens the connection to the server
        /// </summary>
        public void Connect()
        {
            // Create the client connection
            this.m_clientConnection = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            SocketAsyncEventArgs args = new SocketAsyncEventArgs();
            args.UserToken = this.m_clientConnection;
            args.RemoteEndPoint = this.ServerEndpoint;
            args.Completed += new EventHandler<SocketAsyncEventArgs>(OnSocketConnectionCompleted);
            this.m_clientConnection.ConnectAsync(args);
        }

        /// <summary>
        /// Socket has connected to the server
        /// </summary>
        void OnSocketConnectionCompleted(object sender, SocketAsyncEventArgs e)
        {
            Socket socket = (Socket)e.UserToken;

            // Fire connected event
            if (ConnectionStateChanged != null)
                this.m_dispatcher.Invoke(ConnectionStateChanged, new object[] { this, EventArgs.Empty });

            if (!socket.Connected)
                return;

            // Create the response
            byte[] buffer = new byte[2048];
            e.SetBuffer(buffer, 0, buffer.Length);
            e.Completed -= new EventHandler<SocketAsyncEventArgs>(OnSocketConnectionCompleted);
            e.Completed += new EventHandler<SocketAsyncEventArgs>(OnSocketReceive);
            
            
            socket.ReceiveAsync(e);
        }

        /// <summary>
        /// Socket has received data
        /// </summary>
        void OnSocketReceive(object sender, SocketAsyncEventArgs e)
        {
            Socket socket = (Socket)e.UserToken;

            if (!socket.Connected)
                this.m_dispatcher.Invoke(ConnectionStateChanged, new object[] { this, EventArgs.Empty });

            StringReader sr = null;
            try
            {
                string packetData = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                sr = new StringReader(packetData);
                var eventData = this.m_visualizationSerializer.Deserialize(sr) as VisualizationEvent;
                if (eventData != null && EventReceived != null)
                    this.m_dispatcher.Invoke(EventReceived, new object[] { this, new VisualizationEventArgs() { Event = eventData } });
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if(socket.Connected)
                    socket.ReceiveAsync(e);
            }

        }

        /// <summary>
        /// Close this listener
        /// </summary>
        public void Close()
        {
            this.m_clientConnection.Close(10);
        }


        /// <summary>
        /// Fired when the socket has connected
        /// </summary>
        public event EventHandler ConnectionStateChanged;

        /// <summary>
        /// Fired whenever an event is received
        /// </summary>
        public event EventHandler<VisualizationEventArgs> EventReceived;
    }
}
