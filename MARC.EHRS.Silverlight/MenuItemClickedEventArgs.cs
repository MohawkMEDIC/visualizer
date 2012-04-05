using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls.Primitives;

namespace MARC.EHRS.Silverlight
{
    /// <summary>
    /// Menu item has been clicked
    /// </summary>
    public class MenuItemClickedEventArgs : EventArgs
    {

        /// <summary>
        /// Menu Item has been clicked
        /// </summary>
        public ButtonBase MenuItem { get; set; }

    }
}
