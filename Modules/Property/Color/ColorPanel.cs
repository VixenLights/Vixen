using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.ColorManagement.ColorModels;
using Common.Controls.ColorManagement.ColorPicker;

namespace VixenModules.Property.Color
{
	public partial class ColorPanel : UserControl
	{
		public ColorPanel()
		{
			InitializeComponent();
		}

		public ColorPanel(System.Drawing.Color color)
			: this()
		{
			_color = color;
			BackColor = color;
		}

		private void ColorPanel_Click(object sender, EventArgs e)
		{
			using (ColorPicker picker = new ColorPicker()) {
				picker.Color = XYZ.FromRGB(Color);
				if (picker.ShowDialog() == DialogResult.OK) {
					Color = picker.Color.ToRGB();
				}
			}
		}

		private System.Drawing.Color _color = System.Drawing.Color.Black;

		public System.Drawing.Color Color
		{
			get { return _color; }
			set
			{
				_color = value;
				BackColor = value;
				OnColorChanged(value);
			}
		}


		public event EventHandler<ColorPanelEventArgs> ColorChanged;

		public void OnColorChanged(System.Drawing.Color args)
		{
			if (ColorChanged != null)
				ColorChanged(this, new ColorPanelEventArgs(args));
		}
	}

	public class ColorPanelEventArgs : EventArgs
	{
		public ColorPanelEventArgs(System.Drawing.Color value)
		{
			Value = value;
		}

		public System.Drawing.Color Value { get; private set; }
	}
}