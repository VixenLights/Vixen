﻿using Common.Controls.Theme;

namespace Common.Controls
{
	public partial class UndoDropDownControl : UserControl
	{
		//TODO: May have to get rid of the list view. We can't control the scroll position!

		public UndoDropDownControl()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
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
			bottomtext.Text = string.Format("{0} {1} Action{2}",
			                                ((ButtonType == UndoButtonType.UndoButton)
			                                 	? "Undo"
			                                 	: (ButtonType == UndoButtonType.RedoButton) ? "Redo" : "???"),
			                                idx,
			                                ((idx == 1) ? string.Empty : "s")
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
			if (idx < 0)
				return;
			listbox.SetSelected(idx, true); // keep it selected
		}

		private void listbox_MouseUp(object sender, MouseEventArgs e)
		{
			int idx = listbox.IndexFromPoint(e.Location);
			if (idx < 0)
				return;
			if (ItemChosen != null)
			{
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