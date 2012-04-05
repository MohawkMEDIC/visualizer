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

namespace MARC.EHRS.VisualizationClient.Controls
{
    /// <summary>
    /// Interaction logic for Connect.xaml
    /// </summary>
    public partial class Connect : UserControl
    {

        // The visualizer client
        private VisualizerClient m_visualizerClient;

        /// <summary>
        /// Gets or sets the visualizer client
        /// </summary>
        public VisualizerClient VisualizerClient { get; set; }

        public Connect()
        {
            InitializeComponent();
        }

        public T FindVisualParent<T>() where T : DependencyObject 
        { 
            DependencyObject parent = VisualTreeHelper.GetParent( this ); 
            while ( parent != null ) 
            { 
                T typed = parent as T; 
                if ( typed != null ) 
                { 
                    return typed; 
                } 
                parent = VisualTreeHelper.GetParent( parent ); 
            } 
            return null; 
        }


        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                ContentPresenter cp = this.FindVisualParent<ContentPresenter>();
                Canvas.SetZIndex(cp, 99);
            }

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Visibility = Visibility.Hidden;
            
        }
    }
}
