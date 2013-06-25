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
		public DiscreteColorPickerItem()
		{
			InitializeComponent();
		}

		public bool Selected
		{
			get { return checkBoxSelected.Checked; }
			set { checkBoxSelected.Checked = value; }
		}

		public Color Color
		{
			get { return panelColor.BackColor; }
			set { panelColor.BackColor = value; }
		}
	}
}
