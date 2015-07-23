using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Net;

namespace MARC.EHRS.VisualizationServer.Notifier
{
    /// <summary>
    /// This class allows silverlight clients to connect to this service
    /// </summary>
    public class ClientAccessPolicyServer
    {

        // TCP Listener
        private TcpListener m_tcpListener = null;

        // The cap file
        private string m_capFile;

        // The policy file
        private byte[] m_policy;

        // The server thread
        private Thread m_serverThread;

        // Blocking for client
        private ManualResetEvent m_clientConnected = new ManualResetEvent(false);

        // Policy file request
        private string m_policyString = "<policy-file-request/>";

        /// <summary>
        /// Creates a new instance of the CAPS
        /// </summary>
        public ClientAccessPolicyServer(string capFile)
        {
            this.m_capFile = capFile;
        }

        /// <summary>
        /// Start the policy server
        /// </summary>
        public void Start()
        {

            try
            {
                using (FileStream fs = new FileStream(this.m_capFile, FileMode.Open))
                {
                    m_policy = new byte[fs.Length];
                    fs.Read(m_policy, 0, m_policy.Length);
                }
                m_serverThread = new Thread(new ThreadStart(StartSocketServer));
                m_serverThread.IsBackground = true;
                m_serverThread.Start();
            }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
        }

        /// <summary>
        /// Stop the client access policy service
        /// </summary>
        public void Stop()
        {
            this.m_serverThread.Abort();
        }
        
        /// <summary>
        /// Start the socket server
        /// </summary>
        private void StartSocketServer()
        {
            try
            {
                m_tcpListener = new TcpListener(IPAddress.Any, 943);
                m_tcpListener.Start();
                Trace.TraceInformation("Client Access Policy Server running on {0}", m_tcpListener.LocalEndpoint);
                while (true)
                {
                    m_clientConnected.Reset();
                    m_tcpListener.BeginAcceptTcpClient(new AsyncCallback(OnBeginAccept), null);
                    m_clientConnected.WaitOne();
                }
            }
            catch (ThreadAbortException)
            { }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
            finally
            {
                if(m_tcpListener != null)
                    m_tcpListener.Stop();
            }
        }

        /// <summary>
        /// Begin the tcp accept
        /// </summary>
        /// <param name="ar"></param>
        private void OnBeginAccept(IAsyncResult ar)
        {
            TcpClient client = null;
            try
            {
                // Accept the connection
                client = m_tcpListener.EndAcceptTcpClient(ar);
                m_clientConnected.Set();
                byte[] receiveBuffer = new byte[m_policyString.Length];

                int receivedBytes = 0;
                while (receivedBytes < receiveBuffer.Length)
                    receivedBytes += client.Client.Receive(receiveBuffer);
                string receiveString = Encoding.UTF8.GetString(receiveBuffer);
                if (StringComparer.InvariantCulture.Compare(receiveString, m_policyString) != 0)
                    return;
                // Send policy file
                client.Client.Send(m_policy);
            }
            catch (ObjectDisposedException)
            { }
            catch (Exception e)
            {
                Trace.TraceError(e.ToString());
            }
            finally
            {
                if(client != null)
                    client.Close();
            }
        }
    }
}
