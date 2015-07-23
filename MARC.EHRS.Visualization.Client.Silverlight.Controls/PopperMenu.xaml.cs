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
using System.Windows.Markup;
using System.Collections;

namespace MARC.EHRS.Visualization.Client.Silverlight.Controls
{
    public partial class PopperMenu : UserControl
    {

        // Menu Items
        private StackPanel m_menuPanel;
     
        /// <summary>
        /// Play an animation
        /// </summary>
        private void PlayAnimation(ButtonBase but)
        {
            sbClick.Stop();
            Storyboard.SetTargetName(sbClick, but.Name);
            sbClick.Begin();
        }

        /// <summary>
        /// Menu item has been clicked
        /// </summary>
        public event EventHandler<MenuItemClickedEventArgs> MenuItemClicked;

        // Current storyboards
        Dictionary<ToggleButton, Storyboard> m_storyBoards = new Dictionary<ToggleButton, Storyboard>();

        /// <summary>
        /// Menu item stack panel
        /// </summary>
        public StackPanel MenuPanel
        {
            get { return m_menuPanel; }
            set
            {
                if (value == null || m_menuPanel != null)
                    LayoutRoot.Children.Clear();
                m_menuPanel = value;
                if (m_menuPanel != null)
                {
                    MenuBorder.Content = m_menuPanel;
                    Style s = this.LayoutRoot.Resources["MenuButtonStyle"] as Style;
                    if (this.m_menuPanel != null)
                    {
                        foreach (var itm in this.m_menuPanel.Children)
                        {

                            if (itm is ButtonBase)
                            {
                                (itm as ButtonBase).SetValue(ButtonBase.StyleProperty, s);
                                (itm as ButtonBase).Click += new RoutedEventHandler(itm_Click);
                                (itm as ButtonBase).RenderTransform = new CompositeTransform()
                                {
                                    TranslateX = 0,
                                    TranslateY = 0,
                                    ScaleX = 1,
                                    ScaleY = 1
                                };
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Create the popper menu
        /// </summary>
        public PopperMenu()
        {
            InitializeComponent();
        }


        public void RefreshToggleButtonState(ToggleButton tb)
        {
            Storyboard sb = null;

            // Get the created storyboard
            if (!m_storyBoards.TryGetValue(tb, out sb))
            {
                sb = CreateStoryboard(tb);
                m_storyBoards.Add(tb, sb);
            }

            // Now determine if we should play or stop
            if (tb.IsChecked.Value)
                sb.Begin();
            else
                sb.Stop();
        }

        /// <summary>
        /// Item has been clicked
        /// </summary>
        void itm_Click(object sender, RoutedEventArgs e)
        {
            ButtonBase btn = sender as ButtonBase;
            PlayAnimation(btn);

            // Determine if we're checked or not
            if (btn is ToggleButton)
                RefreshToggleButtonState(btn as ToggleButton);

            if (MenuItemClicked != null)
                MenuItemClicked(this, new MenuItemClickedEventArgs() { MenuItem = btn });
            
        }

        private Storyboard CreateStoryboard(ToggleButton tb)
        {
            DoubleAnimation scaleAnimX = new DoubleAnimation()
                    {
                        AutoReverse = true,
                        Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250)),
                        RepeatBehavior = RepeatBehavior.Forever,
                        To = 0.8
                    }, scaleAnimY = new DoubleAnimation()
                    {
                        AutoReverse = true,
                        Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250)),
                        RepeatBehavior = RepeatBehavior.Forever,
                        To = 0.8
                    }, translateAnimX = new DoubleAnimation()
                    {
                        AutoReverse = true,
                        Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250)),
                        RepeatBehavior = RepeatBehavior.Forever,
                        To = 3
                    }, translateAnimY = new DoubleAnimation()
                    {
                        AutoReverse = true,
                        Duration = new Duration(new TimeSpan(0, 0, 0, 0, 250)),
                        RepeatBehavior = RepeatBehavior.Forever,
                        To = 3
                    };

            // Set targets
            Storyboard.SetTarget(scaleAnimX, tb);
            Storyboard.SetTarget(translateAnimX, tb);
            Storyboard.SetTarget(translateAnimY, tb);
            Storyboard.SetTarget(scaleAnimY, tb);

            // Set properties
            scaleAnimX.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(ToggleButton.RenderTransform).(CompositeTransform.ScaleX)"));
            scaleAnimY.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(ToggleButton.RenderTransform).(CompositeTransform.ScaleY)"));
            translateAnimX.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(ToggleButton.RenderTransform).(CompositeTransform.TranslateX)"));
            translateAnimY.SetValue(Storyboard.TargetPropertyProperty, new PropertyPath("(ToggleButton.RenderTransform).(CompositeTransform.TranslateY)"));

            // Add to storyboard
            return new Storyboard()
            {
                Children =
                {
                    scaleAnimX, 
                    scaleAnimY,
                    translateAnimX,
                    translateAnimY
                }
            };

        }


    }
}
