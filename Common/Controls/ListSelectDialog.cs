using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Controls
{
	public partial class ListSelectDialog : Form
	{
		private string _formTitle;
		private List<KeyValuePair<string, object>> _items;

		public ListSelectDialog(string formTitle, List<KeyValuePair<string, object>> items)
		{
			_formTitle = formTitle;
			_items = items;
			InitializeComponent();
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
			} else {
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
			if (e.KeyChar == (char)Keys.Enter && listBoxItems.SelectedIndex >= 0) {
				this.DialogResult = DialogResult.OK;
				this.Close();
			} else if (e.KeyChar == (char)Keys.Escape) {
				this.DialogResult = DialogResult.Cancel;
				this.Close();
			}
		}
	}
}
