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
using jabber.client;
using jabber.connection;

namespace MARC.EHRS.Visualization.Silverlight
{
    /// <summary>
    /// Interaction logic for VisualizerHost.xaml
    /// </summary>
    public partial class VisualizerHost : UserControl
    {
        public VisualizerHost()
        {
            InitializeComponent();


        }

        void jc_OnAuthError(object sender, System.Xml.XmlElement rp)
        {
            this.label1.Content = rp.InnerText;
        }

        void jc_OnAuthenticate(object sender)
        {
            this.label1.Content = "Connected";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            JabberClient jc = new JabberClient();
            jc.OnAuthenticate += new bedrock.ObjectHandler(jc_OnAuthenticate);
            jc.OnAuthError += new jabber.protocol.ProtocolHandler(jc_OnAuthError);

            jc.User = "administrator";
            jc.Password = "@M4rcH1";
            jc.Port = 4502;
            jc.Server = "142.222.45.108";
            jc.Connection = ConnectionType.Socket;

            jc.Connect();
            jc.Presence(jabber.protocol.client.PresenceType.available, "Test", "Test", 0);

        }
    }
}
