using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using MARC.EHRS.Visualization.Core;
using Microsoft.Win32;
using System.Windows.Markup;
using System.IO;
using System.Windows.Media.Animation;
using System.Timers;

namespace MARC.EHRS.VisualizationClient.Windows
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {

        /// <summary>
        /// Delegate for animation events
        /// </summary>
        /// <param name="evt"></param>
        private delegate void AnimationEventDelegate(VisualizationEvent evt);

        // Visualizer client
        private VisualizerClient m_visClient;

        // Visualization events
        private List<VisualizationEvent> m_visEvents = new List<VisualizationEvent>();

        /// <summary>
        /// Playback time
        /// </summary>
        private long m_playbackSequence;

        /// <summary>
        /// Playback timer
        /// </summary>
        private Timer m_playbackTimer = new Timer(100);

        public Window1()
        {
            InitializeComponent();

            // Test code
            m_visClient = VisualizerClient.CreateClient(new System.Net.IPEndPoint(IPAddress.Parse("192.168.0.103"), 4530));
            m_visClient.ConnectionStateChanged += new EventHandler(client_ConnectionStateChanged);
            m_visClient.EventReceived += new EventHandler<MARC.EHRS.Visualization.Core.VisualizationEventArgs>(m_visClient_EventReceived);
            m_playbackTimer.Elapsed += new ElapsedEventHandler(m_playbackTimer_Elapsed);
        }

        /// <summary>
        /// Playback timer has elapsed
        /// </summary>
        void m_playbackTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            m_playbackTimer.Enabled = false;
            m_playbackSequence++;
            // Find the visualization events that fall within this time frame

            lock (m_visEvents)
            {
                var visualizationEvents = m_visEvents.FindAll(o => o.Sequence == m_playbackSequence);
                foreach (var ev in visualizationEvents)
                {
                    AnimationEventDelegate del = new AnimationEventDelegate(AnimateDiagram);
                    this.Dispatcher.BeginInvoke(del, new object[] { ev });
                }
                m_playbackTimer.Enabled = true;
            }
        }

        /// <summary>
        /// Event has been received
        /// </summary>
        void m_visClient_EventReceived(object sender, MARC.EHRS.Visualization.Core.VisualizationEventArgs e)
        {
            if (btnServerDownload.IsChecked.Value)
            {
                m_visEvents.Add(e.Event);
                e.Event.CustomRepresentation = String.Format("{0}.png", e.Event.Name);
                lstMessage.Items.Add(e.Event);
                AnimateDiagram(e.Event);
            }
        }

        /// <summary>
        /// Animate diagram
        /// </summary>
        /// <param name="evt"></param>
        void AnimateDiagram(VisualizationEvent evt)
        {
            lstMessage.SelectedItem = evt;
            lstMessage.ScrollIntoView(evt);
            // Animate
            if (visualizerDiagram.Content != null)
            {
                var uc = visualizerDiagram.Content as UserControl;

                object sb = null;
                try
                {
                    sb = uc.FindResource(evt.MachineOID);
                    if (sb != null && sb is Storyboard)
                        (sb as Storyboard).Begin();
                }
                catch { }
                try
                {
                    sb = uc.FindResource("*");
                    if (sb != null && sb is Storyboard)
                        (sb as Storyboard).Begin();
                }
                catch { }
            }

            if (m_playbackTimer.Enabled && evt == m_visEvents.Last())
                btnReplay.IsChecked = false;
        }

        // Connection state has changed
        void client_ConnectionStateChanged(object sender, EventArgs e)
        {
            VisualizerClient client = sender as VisualizerClient;
            if (client.IsConnected)
            {
                statusText.Content = String.Format("Connected to {0}", client.ServerEndpoint);
                mnuConnect.Header = String.Format("Disconnect {0}", ((IPEndPoint)client.ServerEndpoint).Address);
            }
            else
            {
                statusText.Content = "Not Connected";
                mnuConnect.Header = "Connect...";
            }
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            if (contentGrid.ColumnDefinitions[2].Width.Value > 0)
            {
                contentGrid.ColumnDefinitions[2].Width = new GridLength(0);
                polyImage.Points.Clear();
                polyImage.Points.Add(new Point(0, 5));
                polyImage.Points.Add(new Point(5, 0));
                polyImage.Points.Add(new Point(5, 10));
            }
            else
            {
                contentGrid.ColumnDefinitions[2].Width = new GridLength(200);
                polyImage.Points.Clear();
                polyImage.Points.Add(new Point(0, 0));
                polyImage.Points.Add(new Point(5, 5));
                polyImage.Points.Add(new Point(0, 10));
            }
        }

        private void mnuConnect_Click(object sender, RoutedEventArgs e)
        {
            connector.Visibility = Visibility.Visible;
            contentGrid.Visibility = Visibility.Hidden;
        }

        private void btnServerDownload_Click(object sender, RoutedEventArgs e)
        {
            btnReplay.IsEnabled = sldSpeed.IsEnabled  = !btnServerDownload.IsChecked.Value;
        }

        private void mnuOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog odlg = new OpenFileDialog()
            {
                Filter = "Visualizer Diagram (*.xaml)|*.xaml|All Files (*.*)|*.*",
                Title = "Open Diagram"
            };

            // Show the open dialog
            if (odlg.ShowDialog().Value)
            {
                // Clear content
                visualizerDiagram.Content = null;

                // Load
                FileStream fs = null;
                try
                {
                    fs = File.OpenRead(odlg.FileName);
                    UserControl uc = XamlReader.Load(fs) as UserControl;
                    uc.Width = visualizerDiagram.ActualWidth;
                    uc.Height = visualizerDiagram.ActualHeight;
                    visualizerDiagram.Content = uc;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
            }
        }

        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (visualizerDiagram.Content != null)
            {
                var uc = (visualizerDiagram.Content as UserControl);
                uc.Width = e.NewSize.Width;
                uc.Height = e.NewSize.Height;
            }
        }

        /// <summary>
        /// Start the playback timer
        /// </summary>
        private void btnReplay_Checked(object sender, RoutedEventArgs e)
        {

            if (lstMessage.SelectedIndex != -1)
                m_playbackSequence = m_visEvents[lstMessage.SelectedIndex].Sequence;
            else
                m_playbackSequence = m_visEvents[0].Sequence;

            m_playbackTimer.Enabled = true;

        }

        private void btnReplay_Unchecked(object sender, RoutedEventArgs e)
        {
            m_playbackTimer.Enabled = false;
        }

        private void sldSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            m_playbackTimer.Interval = (sldSpeed.Maximum - sldSpeed.Value)+1;
        }

        private void connector_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (connector.Visibility == Visibility.Hidden)
            {
                contentGrid.Visibility = Visibility.Visible;
            }
        }

    }
}
