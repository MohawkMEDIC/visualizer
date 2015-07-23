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
            if (OnAccept != null)
                OnAccept(this, EventArgs.Empty);
            this.Visibility = System.Windows.Visibility.Collapsed;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (OnCancel != null)
                OnCancel(this, EventArgs.Empty);
            this.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
