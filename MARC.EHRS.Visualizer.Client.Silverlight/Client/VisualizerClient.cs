using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Threading;
using System.Threading;
using MARC.EHRS.Visualizer.Client.Silverlight.Contract.Visualizer;

namespace MARC.EHRS.Visualizer.Client.Silverlight.Client
{
    /// <summary>
    /// Represents a client connection to the visualizer
    /// </summary>
    public class VisualizerClient : IDisposable
    {

        /// <summary>
        /// Visualizer client
        /// </summary>
        private VisualizerClient()
        {
        }

        /// <summary>
        /// Parse queue
        /// </summary>
        private Queue<String> m_parseQueue = new Queue<String>();

        /// <summary>
        /// Sync root
        /// </summary>
        private Object m_syncRoot = new object();

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
        /// Queue reader thread
        /// </summary>
        private Thread m_queueReaderThread;
 
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
        /// Queue reader
        /// </summary>
        private void QueueReader()
        {

            try
            {
                while (true)
                {
                    lock (this.m_syncRoot)
                        Monitor.Wait(this.m_syncRoot);

                    string packetData = string.Empty;
                    lock (this.m_syncRoot)
                        packetData = this.m_parseQueue.Dequeue();
                    List<String> packets = new List<string>();
                    try
                    {
                        
                        while (packetData.Length > 0)
                        {
                            int sPacket = packetData.IndexOf("<?xml"),
                                ePacket = packetData.IndexOf("<?xml", sPacket + 1);
                            // Substring packet
                            if (ePacket == -1)
                            {
                                packets.Add(packetData.Substring(sPacket));
                                packetData = String.Empty;
                            }
                            else
                            {
                                packets.Add(packetData.Substring(sPacket, ePacket - sPacket));
                                packetData = packetData.Remove(sPacket, ePacket - sPacket);
                            }

                        }

                        foreach (var pkt in packets)
                        {
                            var sr = new StringReader(pkt);
                            var eventData = this.m_visualizationSerializer.Deserialize(sr) as VisualizationEvent;
                            if (eventData != null && EventReceived != null)
                                this.m_dispatcher.BeginInvoke(EventReceived, new object[] { this, new VisualizationEventArgs() { Event = eventData } });
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }

        /// <summary>
        /// Create a visualizer client
        /// </summary>
        public static VisualizerClient CreateClient(Uri serverUrl, Dispatcher owner)
        {
            VisualizerClient retVal = new VisualizerClient();
            IPAddress ipadd = null;
            EndPoint serverEndpoint = null;
            if (IPAddress.TryParse(serverUrl.Host, out ipadd))
                serverEndpoint = new IPEndPoint(ipadd, serverUrl.Port);
            else
                serverEndpoint = new DnsEndPoint(serverUrl.Host, serverUrl.Port);
            retVal.ServerEndpoint = serverEndpoint;
            retVal.m_dispatcher = owner;
            retVal.m_queueReaderThread = new Thread(retVal.QueueReader);
            retVal.m_queueReaderThread.Start();

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
                this.m_dispatcher.BeginInvoke(ConnectionStateChanged, new object[] { this, new VisualizationEventArgs() {
                ErrorText = e.SocketError.ToString() }});

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
                this.m_dispatcher.BeginInvoke(ConnectionStateChanged, new object[] { this, 
                new VisualizationEventArgs() { ErrorText = e.SocketError.ToString() 
                }
                });

            StringReader sr = null;
            try
            {
                string packetData = Encoding.UTF8.GetString(e.Buffer, e.Offset, e.BytesTransferred);
                System.Diagnostics.Debug.WriteLine(packetData);
                lock (this.m_syncRoot)
                {
                    this.m_parseQueue.Enqueue(packetData);
                    Monitor.Pulse(this.m_syncRoot);
                }

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
        public event EventHandler<VisualizationEventArgs> ConnectionStateChanged;

        /// <summary>
        /// Fired whenever an event is received
        /// </summary>
        public event EventHandler<VisualizationEventArgs> EventReceived;


        #region IDisposable Members

        /// <summary>
        /// Dispose the thread
        /// </summary>
        public void Dispose()
        {
            this.m_queueReaderThread.Abort();
        }

        #endregion
    }
}
