using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Common.Controls
{
	public partial class DiscreteColorPickerItem : UserControl
	{
		private bool _singleColorOnly;

		public DiscreteColorPickerItem()
		{
			InitializeComponent();
		}

		public bool SingleColorOnly
		{
			get { return _singleColorOnly; }
			set
			{
				_singleColorOnly = value;
				radioButtonSelected.Visible = value;
				checkBoxSelected.Visible = !value;
			}
		}

		public bool Selected
		{
			get { return (SingleColorOnly ? radioButtonSelected.Checked : checkBoxSelected.Checked); }
			set
			{
				if (SingleColorOnly)
					radioButtonSelected.Checked = value;
				else
					checkBoxSelected.Checked = value;
			}
		}

		public Color Color
		{
			get { return panelColor.BackColor; }
			set { panelColor.BackColor = value; }
		}

		public event EventHandler<EventArgs> SelectedChanged;

		public void OnSelectedChanged()
		{
			if (SelectedChanged != null)
				SelectedChanged(this, new EventArgs());
		}

		private void radioButtonSelected_CheckedChanged(object sender, EventArgs e)
		{
			OnSelectedChanged();
		}

		private void checkBoxSelected_CheckedChanged(object sender, EventArgs e)
		{
			OnSelectedChanged();
		}

		private void panelColor_Click(object sender, EventArgs e)
		{
			Selected = !Selected;
		}
	}
}