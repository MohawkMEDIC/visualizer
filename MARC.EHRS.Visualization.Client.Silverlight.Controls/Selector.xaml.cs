/*
 * Copyright 2012-2017 Mohawk College of Applied Arts and Technology
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej
 * Date: 2012-6-15
 */
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

namespace MARC.EHRS.Visualization.Client.Silverlight.Controls
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
