using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;

namespace Common.Controls
{
	public partial class ListSelectDialog : BaseForm
	{
		private string _formTitle;
		private List<KeyValuePair<string, object>> _items;

		public ListSelectDialog(string formTitle, List<KeyValuePair<string, object>> items)
		{
			_formTitle = formTitle;
			_items = items;
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
		}

		public Object SelectedItem
		{
			get
			{
				if (listBoxItems.SelectedIndex < 0)
					return null;
				else
					return listBoxItems.SelectedValue;
			}
		}

		public ListBox.SelectedObjectCollection SelectedItems
		{
			get
			{
				if (listBoxItems.SelectedIndex < 0)
					return null;
				else
					return listBoxItems.SelectedItems;
			}
		}

		public SelectionMode SelectionMode
		{
			get
			{
				return listBoxItems.SelectionMode;
			}

			set
			{
				listBoxItems.SelectionMode = value;
			}
		}

		private void ListSelectDialog_Load(object sender, EventArgs e)
		{
			listBoxItems.DisplayMember = "Key";
			listBoxItems.ValueMember = "Value";
			listBoxItems.DataSource = _items;
			listBoxItems.SelectedIndex = -1;
		}

		private void listBoxItems_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (listBoxItems.SelectedIndex >= 0) {
				buttonOk.Enabled = true;
			}
			else {
				buttonOk.Enabled = false;
			}
		}

		private void listBoxItems_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			if (listBoxItems.SelectedIndex >= 0) {
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
		}

		private void ListSelectDialog_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == (char) Keys.Enter && listBoxItems.SelectedIndex >= 0) {
				this.DialogResult = DialogResult.OK;
				this.Close();
			}
			else if (e.KeyChar == (char) Keys.Escape) {
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			}
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.Properties.Resources.ButtonBackgroundImage;

		}
	}
}