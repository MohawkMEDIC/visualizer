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
    public partial class StatusBox : UserControl
    {
        /// <summary>
        /// Caption of the message
        /// </summary>
        public String Content
        {
            get
            {
                return lblStatus.Text;
            }
            set
            {
                lblStatus.Text = value;
            }
        }


        /// <summary>
        /// On accept clicked
        /// </summary>
        public event EventHandler OnAccept;
        /// <summary>
        /// On cancel clicked
        /// </summary>
        public event EventHandler OnCancel;

        /// <summary>
        /// Allow approve
        /// </summary>
        public bool AllowApprove
        {
            get { return btnAccept.Visibility == System.Windows.Visibility.Collapsed; }
            set
            {
                if (value)
                    btnAccept.Visibility = System.Windows.Visibility.Visible;
                else
                    btnAccept.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Allow cancel
        /// </summary>
        public bool AllowCancel 
        {
            get { return btnCancel.Visibility == System.Windows.Visibility.Collapsed; }
            set {
                if (value)
                    btnCancel.Visibility = System.Windows.Visibility.Visible;
                else
                    btnCancel.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        public StatusBox()
        {
            InitializeComponent();
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
	        OnAccept?.Invoke(this, EventArgs.Empty);
	        this.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
			OnCancel?.Invoke(this, EventArgs.Empty);
			this.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
