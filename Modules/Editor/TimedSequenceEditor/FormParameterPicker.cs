using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Common.Controls;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class FormParameterPicker : Form
	{
		public FormParameterPicker(IEnumerable<EffectParameterPickerControl> controls)
		{
			InitializeComponent();

			foreach (EffectParameterPickerControl control in controls)
			{
				control.Click += ParameterControl_Clicked;
				flowLayoutPanel1.Controls.Add(control);
			}
		}
		
		private void ParameterControl_Clicked(object sender, EventArgs e)
		{
			EffectParameterPickerControl control = (EffectParameterPickerControl)sender;
			ParameterIndex = control.ParameterIndex;
			ParameterListIndex = control.ParameterListIndex;
			DialogResult = DialogResult.OK;
			Close();
		}

		public int ParameterIndex { get; set; }

		public int ParameterListIndex { get; set; }

		private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
		{
			if (flowLayoutPanel1.BorderStyle != BorderStyle.FixedSingle) return;
			const int thickness = 3;
			const int halfThickness = thickness/2;
			using (Pen p = new Pen(Color.Black, thickness))
			{
				e.Graphics.DrawRectangle(p, new Rectangle(halfThickness,
					halfThickness,
					flowLayoutPanel1.ClientSize.Width - thickness,
					flowLayoutPanel1.ClientSize.Height - thickness));
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (keyData == Keys.Escape)
			{
				DialogResult = DialogResult.Cancel;
				Close();
			}
			
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
