using System;
using System.Windows.Controls.Primitives;

namespace Transmute.Wpf
{
    public class ScrollBarEventArgs : EventArgs
    {
        public ScrollBar VScrollBar;
        public ScrollBar HScrollBar;

        public double X
        {
            get
            {
                return HScrollBar.Value;
            }
            set
            {
                HScrollBar.Value = value;
            }
        }

        public double Y
        {
            get
            {
                return VScrollBar.Value;
            }
            set
            {
                VScrollBar.Value = value;
            }
        }

        public ScrollBarEventArgs(ScrollBar hScrollBar, ScrollBar vScrollBar)
        {
            HScrollBar = hScrollBar;
            VScrollBar = vScrollBar;
        }
    }
}
