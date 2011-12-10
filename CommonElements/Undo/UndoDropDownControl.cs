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
            this.ButtonType = UndoButtonType.UndoButton;
        }


        public UndoButtonType ButtonType { get; set; }


        public ListBox.ObjectCollection Items
        {
            get { return listbox.Items; }
        }

        private void UpdateSelected(Point location)
        {
            int idx = listbox.IndexFromPoint(location);

            // TODO: This fucks up the scrolling :-(
            // Select all items from 0 to idx
            listbox.BeginUpdate();
            for (int i = 0; i < listbox.Items.Count; i++)
                listbox.SetSelected(i, (i <= idx));
            listbox.EndUpdate();

            idx++;
            bottomtext.Text = String.Format("{0} {1} Action{2}",
                ((ButtonType == UndoButtonType.UndoButton) ? "Undo" : 
                (ButtonType == UndoButtonType.RedoButton) ? "Redo" : "???"),
                idx,
                ((idx > 1) ? "s" : "")
                );
        }

        private void listbox_MouseMove(object sender, MouseEventArgs e)
        {
            UpdateSelected(e.Location);
        }


        private void listbox_Click(object sender, MouseEventArgs e)
        {
            int idx = listbox.IndexFromPoint(e.Location);
            Debug.WriteLine("Click: " + idx.ToString());
        }


        public event EventHandler<UndoMultipleItemsEventArgs> ItemsClicked;

    }

    public class UndoMultipleItemsEventArgs : EventArgs
    {
        public UndoMultipleItemsEventArgs(int numItems)
        {
            NumItems = numItems;
        }
        public int NumItems { get; private set; }
    }

}
