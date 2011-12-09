using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace CommonElements
{
    public partial class UndoDropDownControl : UserControl
    {
        public UndoDropDownControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public string Caption
        {
            get { return bottomtext.Text; }
            set { bottomtext.Text = value; }
        }

        public ListBox.ObjectCollection Items
        {
            get { return listbox.Items; }
        }



        private void UpdateSelected(Point location)
        {
            int idx = listbox.IndexFromPoint(location);

            // Select all items from 0 to idx
            for (int i = 0; i < listbox.Items.Count; i++)
                listbox.SetSelected(i, (i <= idx));

            idx++;
            Caption = String.Format("Undo {0} Action{1}", idx, ((idx > 1) ? "s" : ""));
        }

        private void listbox_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateSelected(e.Location);
        }


        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_VSCROLL = 0x0115;


    }


}
