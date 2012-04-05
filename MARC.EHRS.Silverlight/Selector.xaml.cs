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

namespace MARC.EHRS.Silverlight
{
    public partial class Selector : UserControl
    {

        /// <summary>
        /// Gets the selected item
        /// </summary>
        public Object SelectedItem { get { return lstSelection.SelectedItem; } set { lstSelection.SelectedItem = value; } }

        /// <summary>
        /// Section has changed
        /// </summary>
        public event SelectionChangedEventHandler SelectionChanged;

        public Selector()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Scroll the selector left (up)
        /// </summary>
        private void SelectorScrollLeft(object sender, RoutedEventArgs e)
        {
            int nOffset = (int)lstSelectionScroll.VerticalOffset;
            nOffset -= (int)(lstSelectionScroll.ActualWidth / 2);
            lstSelectionScroll.ScrollToHorizontalOffset((double)nOffset);
        }

        /// <summary>
        /// Scroll the selector right (down)
        /// </summary>
        private void SelectorScrollRight(object sender, RoutedEventArgs e)
        {
            int nOffset = (int)lstSelectionScroll.VerticalOffset;
            nOffset += (int)(lstSelectionScroll.ActualWidth / 2);
            lstSelectionScroll.ScrollToHorizontalOffset((double)nOffset);

        }

        /// <summary>
        /// Selection has change
        /// </summary>
        private void lstSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectionChanged != null)
                SelectionChanged(this, e);
        }

        private void ContentPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ContentPresenter pres = sender as ContentPresenter;
            if (pres.Content != null && pres.Content is UserControl)
            {

                (pres.Content as UserControl).Width = e.NewSize.Width;
                (pres.Content as UserControl).Height = e.NewSize.Height;
            }
        }
    }
}
