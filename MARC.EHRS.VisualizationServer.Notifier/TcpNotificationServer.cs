using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MARC.HI.EHRS.SVC.Core.Services;
using System.Diagnostics;
using MARC.EHRS.VisualizationServer.Notifier.Configuration;
using System.Threading;
using System.Net.Sockets;
using System.Xml.Serialization;
using MARC.EHRS.Visualization.Core;
using System.Configuration;
using System.Net;
using System.IO;
using MARC.EHRS.Visualization.Core.Services;

namespace MARC.EHRS.VisualizationServer.Notifier
{
    /// <summary>
    /// Tcp Notification server
    /// </summary>
    public class TcpNotificationServer : IMessageHandlerService
    {
        // Configuration for the notification service
        private ConfigurationHandler m_configuration = null;

        // The thread the master server listens on
        private Thread m_serverThread = null;

        // CAP Server
        private ClientAccessPolicyServer m_policyServer;

        // TCP Listener
        private TcpListener m_tcpListener = null;

        // Manual Reset event ensures that one client can connect properly
        private ManualResetEvent m_clientConnected = new ManualResetEvent(false);

        // Event Serializer
        private XmlSerializer m_eventSerializer = new XmlSerializer(typeof(VisualizationEvent));
        
        // JF : Bug 2015 - Requires a list of active threads
        private List<Thread> m_activeConnectionThreads = new List<Thread>(10);

        // Synchronization object
        private Object m_syncObject = new object();

        /// <summary>
        /// Creates a new instance of the TcpNotificationService
        /// </summary>
        public TcpNotificationServer()
        {
            this.m_configuration = ConfigurationManager.GetSection("marc.ehrs.visualizationserver.notifier") as ConfigurationHandler;

           
        }

        /// <summary>
        /// Start the socket service
        /// </summary>
        private void StartSocketService()
        {
            // Listener for connections
            try
            {
                IPEndPoint localIp = new IPEndPoint(m_configuration.BindAddress, m_configuration.BindPort);
                this.m_tcpListener = new TcpListener(localIp);
                this.m_tcpListener.Start();
                Trace.TraceInformation("Started TCP listener on '{0}'", localIp);
                while (true)
                {
                    m_clientConnected.Reset();
                    this.m_tcpListener.BeginAcceptTcpClient(new AsyncCallback(OnBeginAccept), null);
                    m_clientConnected.WaitOne();
                }
            }
            catch (ThreadAbortException)
            {
            }
            catch (Exception e)
            {
                Trace.TraceError(e.Message);
            }
            finally
            {

                // Shut down threads
                Trace.TraceInformation("Shutting down client threads");
                foreach (var thd in this.m_activeConnectionThreads)
                    try
                    {
                        thd.Abort(); // Abort the thread
                    }
                    catch{}

                if (this.m_tcpListener != null)
                    this.m_tcpListener.Stop();
            }
        }

        /// <summary>
        /// Begin the acceptance of the client tcp connection
        /// </summary>
        private void OnBeginAccept(IAsyncResult ar)
        {

            TcpClient client = null;

            // JF - Bug 2015
            // Try to fork a new thread to handle the client connection
            try
            {
                // Accept the incoming request
                client = this.m_tcpListener.EndAcceptTcpClient(ar);

                // Allow waiting thread to proceed
                m_clientConnected.Set();

                // JF - Bug 2015 - Hand off the client connection to a new thread
                Thread clientThread = new Thread(MaintainClientConnection);
                clientThread.Start(client);
                lock (this.m_syncObject)
                    this.m_activeConnectionThreads.Add(clientThread);
            }
            catch (ObjectDisposedException)
            { } // Do nothing
            catch (Exception e)
            {
                if (client != null)
                    Trace.TraceError("Could not establish connection with '{0}'. Error: {1}", client.Client.RemoteEndPoint, e);
                else
                    Trace.TraceError("Should not be here, error occurred establishing connection : {0}", e);
            }
        }

        /// <summary>
        /// Maintains a client connection
        /// </summary>
        /// <remarks>Fixes Bug 2015</remarks>
        /// <param name="client">A pointer to a tcp client</param>
        private void MaintainClientConnection(object client)
        {
            
            // Sanity check
            if (!(client is TcpClient))
                throw new ArgumentException("Parameter must be an instance of TcpClient", "client");

            // Is there an implementation of the TcpNotificationService?
            TcpNotificationService notification = this.Context.GetService(typeof(INotificationService)) as TcpNotificationService;
            if (notification == null)
                throw new InvalidOperationException("Cannot establish connection with null notification service!");

            var tcpClient = client as TcpClient;

            // Log
            Trace.TraceInformation("Established connection with '{0}'...", tcpClient.Client.RemoteEndPoint);

            // Set a listener for this event received that will
            // enusre that all clients are broadcast a message
            var eventReceived = new EventHandler<VisualizationEventArgs>(delegate(object sender, VisualizationEventArgs evt)
            {
                try
                {
                    StreamWriter sw = new StreamWriter(tcpClient.GetStream());
                    this.m_eventSerializer.Serialize(sw, evt.Event);
                    sw.Flush();
                }
                catch { }
            });

            try
            {
                
                notification.EventReceived += eventReceived;
                while (tcpClient.Connected)
                    Thread.Sleep(10);
            }
            catch (ThreadAbortException)
            { }
            catch (Exception e)
            {
                Trace.TraceError("{0} : Fatal error occurred : {1}", tcpClient.Client.RemoteEndPoint, e);
            }
            finally
            {
                notification.EventReceived -= eventReceived;
                tcpClient.Close();
            }

        }


        #region INotificationService Members

        /// <summary>
        /// Start the TCP notification service
        /// </summary>
        public bool Start()
        {
            this.m_serverThread = new Thread(new ThreadStart(StartSocketService));
            this.m_serverThread.IsBackground = true;
            this.m_serverThread.Start();

            // Enabled CAP Server
            if (this.m_configuration.EnableCAPServer)
            {
                Trace.TraceInformation("Client Access Policy Service Enabled...");
                m_policyServer = new ClientAccessPolicyServer(this.m_configuration.CapServerPolicyFile);
                m_policyServer.Start();
            }
            return true;
        }

        /// <summary>
        /// Stop the TCP notification service
        /// </summary>
        public bool Stop()
        {
            if(this.m_policyServer != null)
                this.m_policyServer.Stop();
            this.m_serverThread.Abort();
            return true;
        }

        #endregion

       
        #region IUsesHostContext Members

        public IServiceProvider Context
        {
            get;
            set;
        }

        #endregion

    }
}
