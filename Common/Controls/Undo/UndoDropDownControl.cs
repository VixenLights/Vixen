using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Common.Controls
{
    public partial class UndoDropDownControl : UserControl
    {
        //TODO: May have to get rid of the list view. We can't control the scroll position!

        public UndoDropDownControl()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.ButtonType = UndoButtonType.UndoButton;
            
            listbox.MouseMove += listbox_MouseMove;
            listbox.MouseDown += listbox_MouseDown;
            listbox.MouseUp += listbox_MouseUp;
        }

        public event EventHandler<UndoMultipleItemsEventArgs> ItemChosen;

        #region Properties

        public UndoButtonType ButtonType { get; set; }

        public ListBox.ObjectCollection Items
        {
            get { return listbox.Items; }
        }

        #endregion

        public void Reset()
        {
            UpdateSelected(0);
        }

        private void UpdateSelected(int idx)
        {
            listbox.BeginUpdate();
            for (int i = 0; i < listbox.Items.Count; i++)
                listbox.SetSelected(i, (i <= idx));
            listbox.EndUpdate();

            idx++;
            bottomtext.Text = String.Format("{0} {1} Action{2}",
                ((ButtonType == UndoButtonType.UndoButton) ? "Undo" :
                (ButtonType == UndoButtonType.RedoButton) ? "Redo" : "???"),
                idx,
                ((idx == 1) ? "" : "s")
                );
        }


        #region Event handlers

        private void listbox_MouseMove(object sender, MouseEventArgs e)
        {
            int idx = listbox.IndexFromPoint(e.Location);
            if (idx < 0)
                idx = 0;
            UpdateSelected(idx);
        }

        private void listbox_MouseDown(object sender, MouseEventArgs e)
        {
            int idx = listbox.IndexFromPoint(e.Location);
            listbox.SetSelected(idx, true); // keep it selected
        }

        private void listbox_MouseUp(object sender, MouseEventArgs e)
        {
            int idx = listbox.IndexFromPoint(e.Location);
            if (ItemChosen != null) {
                ItemChosen(this, new UndoMultipleItemsEventArgs(idx + 1));
            }
        }

        #endregion



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
