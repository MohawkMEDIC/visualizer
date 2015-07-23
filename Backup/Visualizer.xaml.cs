using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using MARC.EHRS.VisualizationClient.Silverlight.Config;
using MARC.EHRS.Visualization.Core;
using System.IO;
using System.Xml.Serialization;
using System.Threading;
using System.Windows.Markup;
using System.Xml;

namespace MARC.EHRS.VisualizationClient.Silverlight
{
    public partial class Visualizer : UserControl
    {

        private delegate void AnimationDelegate(VisualizationEvent evt);

        private readonly string[] speeds = new string[]{
                                            "Slowest",
                                            "Slow",
                                            "Normal",
                                            "Fast",
                                            "Fastest"
                                        };
        // The visualizer client
        private VisualizerClient m_visClient;

        /// <summary>
        /// The current capture being shown
        /// </summary>
        private VisualizationCollection m_currentCapture;

        /// <summary>
        /// Speed
        /// </summary>
        private int m_inSpeedMode = 100;

        /// <summary>
        /// Current diagram
        /// </summary>
        private UserControl m_currentDiagram;

        /// <summary>
        /// Enable capture
        /// </summary>
        private bool m_enableCapture = true;

        /// <summary>
        /// Configuration
        /// </summary>
        public Configuration Configuration { get; set; }

        /// <summary>
        /// Playback timer
        /// </summary>
        private Timer m_playbackTimer;

        /// <summary>
        /// The current date being played back
        /// </summary>
        private DateTime m_playbackDate;

        public Visualizer()
        {
            InitializeComponent();
            m_playbackTimer = new Timer(new TimerCallback(playbackTimer_Elapsed), null, System.Threading.Timeout.Infinite, m_inSpeedMode);
            
        }


        /// <summary>
        /// Timer has elapsed
        /// </summary>
        private void playbackTimer_Elapsed(object state)
        {
            m_playbackDate = m_playbackDate.AddMilliseconds(100);

            // Time to play animations
            lock (m_currentCapture)
            {
                this.Dispatcher.BeginInvoke(new EventHandler(SetTimeline), null, EventArgs.Empty);
                var events = from evt in m_currentCapture.Events
                             where evt.CapturedAt < m_playbackDate && evt.CapturedAt > m_playbackDate.Subtract(new TimeSpan(0, 0, 0, 0, 99))
                             select evt;
                foreach (var evt in events)
                    this.Dispatcher.BeginInvoke(new AnimationDelegate(AnimateDiagram), evt);


            }
        }

        /// <summary>
        /// Show server selection
        /// </summary>
        private void ShowServerSelector()
        {
            selector.SelectedItem = null;
            sbShowSelector.Begin();
            selector.DataContext = Configuration.Servers;
        }

        /// <summary>
        /// Select a diagram
        /// </summary>
        private void ShowDiagramSelector()
        {
            selector.SelectedItem = null;
            sbShowSelector.Begin();
            selector.DataContext = Configuration.Diagrams;
        }


       
        /// <summary>
        /// Current diagram has changed
        /// </summary>
        private void lstSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0 && e.AddedItems[0] is Diagram)
            {
                m_currentDiagram = (e.AddedItems[0] as Diagram).CreateControl() as UserControl;
                m_currentDiagram.VerticalContentAlignment = m_currentDiagram.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
                m_currentDiagram.HorizontalContentAlignment = m_currentDiagram.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;

                visualizerContent.Children.Clear();
                visualizerContent.Children.Add(m_currentDiagram);
                sbHideSelector.Begin();
            }
            else if (e.AddedItems.Count > 0 && e.AddedItems[0] is Server)
            {
                connectionStatus.Visibility = System.Windows.Visibility.Visible;
                // btnHideConnection.Visibility = System.Windows.Visibility.Collapsed;

                connectionStatus.Content = String.Format("Connecting to {0} at {1}:{2}",
                    (e.AddedItems[0] as Server).Name,
                    (e.AddedItems[0] as Server).Address,
                    (e.AddedItems[0] as Server).Port);

                // Disconnect from the current service
                if (m_visClient != null)
                {
                    m_visClient.Close();
                    m_visClient.ConnectionStateChanged -= new EventHandler<VisualizationEventArgs>(m_visClient_ConnectionStateChanged);
                    m_visClient.EventReceived -= new EventHandler<Visualization.Core.VisualizationEventArgs>(m_visClient_EventReceived);
                }

                // Connect to new service
                m_visClient = (e.AddedItems[0] as Server).CreateClient(this.Dispatcher);
                m_visClient.ConnectionStateChanged += new EventHandler<VisualizationEventArgs>(m_visClient_ConnectionStateChanged);
                m_visClient.EventReceived += new EventHandler<Visualization.Core.VisualizationEventArgs>(m_visClient_EventReceived);
                m_visClient.Connect();

            }

        }

        void m_visClient_EventReceived(object sender, Visualization.Core.VisualizationEventArgs e)
        {
            if (m_enableCapture)
                AnimateDiagram(e.Event);
        }

        void m_visClient_ConnectionStateChanged(object sender, VisualizationEventArgs e)
        {
            if (m_visClient.IsConnected)
            {
                ToolTipService.SetToolTip(CaptureMenu.FindName("btnServer") as DependencyObject, String.Format("Change Infrastructure (Currently connected to '{0}')", m_visClient.ServerEndpoint));
                connectionStatus.Visibility = System.Windows.Visibility.Collapsed;
                sbHideSelector.Begin();
            }
            else
            {
                ToolTipService.SetToolTip(CaptureMenu.FindName("btnServer") as DependencyObject, "Change Infrastructure (Not Connected)");
                if (connectionStatus.Visibility == System.Windows.Visibility.Collapsed) // Retry connecton
                    m_visClient.Connect();
                else
                {
                    connectionStatus.Content = String.Format("Couldn't establish a connection please ensure the service is running and is available. '{0}'", e.ErrorText);
                    connectionStatus.AllowApprove = true;
                }
            }
        }

        private void connectionStatus_OnAccept(object sender, EventArgs e)
        {
            sbHideSelector.Begin();
        }

        /// <summary>
        /// Animate diagram
        /// </summary>
        /// <param name="evt"></param>
        private void AnimateDiagram(VisualizationEvent evt)
        {
            // Animate
            if (m_currentDiagram != null)
            {

                object sb = null;
                try
                {
                    // Play the event identifier first
                    sb = m_currentDiagram.Resources[evt.MachineOID + "^" + (evt.EventID ?? "")];
                    if (sb == null) // Event type?
                        sb = m_currentDiagram.Resources[evt.MachineOID + "^" + (evt.EventType ?? "")];
                    if (sb == null) // machine?
                        sb = m_currentDiagram.Resources[evt.MachineOID];

                    if (sb != null && sb is Storyboard)
                        (sb as Storyboard).Begin();
                }
                catch { }
                try
                {
                    sb = m_currentDiagram.Resources["*"];
                    if (sb != null && sb is Storyboard)
                        (sb as Storyboard).Begin();
                }
                catch { }
            }

            // Save 
            if (m_currentCapture != null && !m_currentCapture.IsSaved && m_enableCapture)
            {
                
                evt.CapturedAt = DateTime.Now;

                // Add to collection
                m_currentCapture.Events.Add(evt);
            }
        }

        private void SetTimeline(object s, EventArgs e)
        {
            // Update
            if(m_currentCapture != null)
            {
                
                // End of reel?
                if (m_playbackDate > m_currentCapture.Events.Last().CapturedAt)
                    StopPlayback();
                timeline.Value = m_playbackDate.Subtract(m_currentCapture.Events[0].CapturedAt).TotalMilliseconds / 100;
                position.Content = m_playbackDate.Subtract(m_currentCapture.Events[0].CapturedAt).ToString().Substring(0, 8);
            }
        }

        private void StartPlayback()
        {
            if (m_currentCapture == null) return;
           // m_playbackDate = m_currentCapture.Events[0].CapturedAt;
            timeline.Maximum = m_currentCapture.Events.Last().CapturedAt.Subtract(m_currentCapture.Events[0].CapturedAt).TotalMilliseconds / 100;
            m_playbackTimer.Change(m_inSpeedMode, m_inSpeedMode);
            state.Content = "Playing";
        }

        private void StopPlayback()
        {
            m_playbackTimer.Change(Timeout.Infinite, m_inSpeedMode);
            state.Content = "Paused";
            (PlaybackMenu.MenuPanel.FindName("btnReplay") as ToggleButton).IsChecked = false;
            PlaybackMenu.RefreshToggleButtonState((PlaybackMenu.MenuPanel.FindName("btnReplay")) as ToggleButton);
        }

        /// <summary>
        /// Rewind playback
        /// </summary>
        private void RewindPlayback()
        {
            if (m_currentCapture != null)
                m_playbackDate = m_currentCapture.Events[0].CapturedAt.Subtract(new TimeSpan(0, 0, 0, 0, 200));
        }

        /// <summary>
        /// Show capture
        /// </summary>
        private void ShowCaptureMenu()
        {
            m_enableCapture = true;
            sbHidePlaybackMenu.Begin();
            sbShowCaptureMenu.Begin();
        }


        /// <summary>
        /// Show playback
        /// </summary>
        private void ShowPlaybackMenu()
        {
            m_enableCapture = false;
            RewindPlayback();
            bool unsavedCapture = m_currentCapture != null && !m_currentCapture.IsSaved;

            (PlaybackMenu.MenuPanel.FindName("btnOpen") as FrameworkElement).Visibility = unsavedCapture ? Visibility.Collapsed : Visibility.Visible;
            (PlaybackMenu.MenuPanel.FindName("btnReplay") as FrameworkElement).Visibility = unsavedCapture ? Visibility.Visible : Visibility.Collapsed;
            (PlaybackMenu.MenuPanel.FindName("btnSave") as FrameworkElement).Visibility = unsavedCapture ? Visibility.Visible : Visibility.Collapsed;
            (PlaybackMenu.MenuPanel.FindName("btnSpeed") as FrameworkElement).Visibility = unsavedCapture ? Visibility.Visible : Visibility.Collapsed;


            sbHideCaptureMenu.Begin();
            sbShowPlaybackMenu.Begin();
        }

        /// <summary>
        /// Menu item has been clicked
        /// </summary>
        private void PopperMenu_MenuItemClicked(object sender, EHRS.Silverlight.MenuItemClickedEventArgs e)
        {

            switch (e.MenuItem.Name)
            {
                case "btnHelp":
                    sbShowHelp.Begin();
                    if (aboutText.Blocks.Count == 0)
                        LoadAboutText();
                    aboutText.Selection.Select(aboutText.ContentStart, aboutText.ContentStart);
                    break;
                case "btnSave":
                    SaveCapture();
                    break;
                case "btnOpen":
                    OpenCapture();
                    break;
                case "btnDiagram":
                    ShowDiagramSelector();
                    break;
                case "btnSpeed":
                    if ((e.MenuItem as ToggleButton).IsChecked.Value)
                    {
                        m_inSpeedMode = 1000;
                    }
                    else
                        m_inSpeedMode = 100;
                    if ((PlaybackMenu.FindName("btnReplay") as ToggleButton).IsChecked.Value)
                        m_playbackTimer.Change(m_inSpeedMode, m_inSpeedMode);
                    break;
                case "btnReplay":
                    if ((e.MenuItem as ToggleButton).IsChecked.Value)
                        StartPlayback();
                    else
                        StopPlayback();
                    break;
                case "btnServer":
                    ShowServerSelector();
                    break;
                case "btnPlaybackMode":
                    ShowPlaybackMenu();
                    break;
                case "btnCaptureMode":
                    if (m_currentCapture != null)
                    {
                        StopPlayback();
                        eraseCaptureStatus.Visibility = System.Windows.Visibility.Visible;
                    }
                    else
                        ShowCaptureMenu();
                    break;
                case "btnRecord":
                    if (m_currentCapture != null && !(e.MenuItem as ToggleButton).IsChecked.Value)
                    {
                        RepeatButton rp = CaptureMenu.MenuPanel.FindName("btnPlaybackMode") as RepeatButton;
                        rp.Visibility = System.Windows.Visibility.Visible;
                        ShowPlaybackMenu();
                    }
                    else if ((e.MenuItem as ToggleButton).IsChecked.Value)
                    {
                        RepeatButton rp = CaptureMenu.MenuPanel.FindName("btnPlaybackMode") as RepeatButton;
                        rp.Visibility = System.Windows.Visibility.Collapsed;
                        m_currentCapture = new VisualizationCollection();
                    }
                    break;
            }

        }

        private void LoadAboutText()
        {
            foreach (var xe in Configuration.About)
            {
                MemoryStream ms = new MemoryStream();
                XmlWriter writer = XmlWriter.Create(ms);
                xe.WriteTo(writer);
                writer.Flush();
                ms.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(ms);
                var data = XamlReader.Load(sr.ReadToEnd());

                if(data != null)
                    aboutText.Blocks.Add((Block)data);
            }
        }

        /// <summary>
        /// Open a capture
        /// </summary>
        private void OpenCapture()
        {

            OpenFileDialog ofd = new OpenFileDialog()
            {
                Filter = "Visualizer Captures (*.vcap)|*.vcap",
                Multiselect = false
            };
            if (ofd.ShowDialog() == true)
            {
                Stream strm = null;
                try
                {
                    strm = ofd.File.OpenRead();
                    XmlSerializer xsz = new XmlSerializer(typeof(VisualizationCollection));
                    m_currentCapture = xsz.Deserialize(strm) as VisualizationCollection;
                    m_currentCapture.IsSaved = true;
                    (PlaybackMenu.MenuPanel.FindName("btnReplay") as FrameworkElement).Visibility = Visibility.Visible;
                    (PlaybackMenu.MenuPanel.FindName("btnSpeed") as FrameworkElement).Visibility = Visibility.Visible;

                    // No diagram showing, so allow the user to pick one
                    if (m_currentDiagram == null)
                        ShowDiagramSelector();
                    RewindPlayback();
                }
                catch (Exception e)
                {
                    generalErrorAlert.Content = String.Format("Error occurred saving the file : {0}", e.Message);
                    generalErrorAlert.Visibility = System.Windows.Visibility.Visible;
                }
                finally
                {
                    if (strm != null)
                        strm.Close();
                }
            }
        }

        /// <summary>
        /// Save the current capture
        /// </summary>
        private void SaveCapture()
        {
            SaveFileDialog savd = new SaveFileDialog()
            {
                Filter = "Visualizer Captures (*.vcap)|*.vcap"
            };

            // Show and save
            if (savd.ShowDialog() == true)
            {
                Stream strm = null;
                try
                {
                    strm = savd.OpenFile();
                    XmlSerializer xsz = new XmlSerializer(typeof(VisualizationCollection));
                    xsz.Serialize(strm, m_currentCapture);
                    m_currentCapture.IsSaved = true;
                    
                }
                catch (Exception e)
                {
                    generalErrorAlert.Content = String.Format("Error occurred saving the file : {0}", e.Message);
                    generalErrorAlert.Visibility = System.Windows.Visibility.Visible;
                    
                }
                finally
                {
                    if (strm != null)
                        strm.Close();
                }
            }

        }

        /// <summary>
        /// Go back
        /// </summary>
        private void eraseCaptureStatus_OnAccept(object sender, EventArgs e)
        {
            m_currentCapture = null;
            ShowCaptureMenu();
        }

        private void timeline_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (m_currentCapture != null)
            {
                m_playbackDate = m_currentCapture.Events[0].CapturedAt.AddMilliseconds(e.NewValue * 100);
                position.Content = m_playbackDate.Subtract(m_currentCapture.Events[0].CapturedAt).ToString().Substring(0, 8);
            }
        }

        private void RepeatButton_Click(object sender, RoutedEventArgs e)
        {
            sbHideHelp.Begin();
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            this.m_visClient.Dispose();
        }

        
    }
}
