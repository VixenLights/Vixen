using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Common.Controls;
using Timer = System.Timers.Timer;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class FormParameterPicker : BaseForm
	{
		private readonly Timer _timer = new Timer();

		/// <summary>
		/// Shows are parameter picker window.
		/// </summary>
		/// <param name="controls">The controls to render the items in the picker.</param>
		/// <param name="closeInterval">The auto cancel interval.</param>
		public FormParameterPicker(IEnumerable<EffectParameterPickerControl> controls, double closeInterval=8000)
		{
			InitializeComponent();

			foreach (EffectParameterPickerControl control in controls)
			{
				control.Click += ParameterControl_Clicked;
				flowLayoutPanel1.Controls.Add(control);
			}
			_timer.Interval = closeInterval;
			_timer.Elapsed += _timer_Elapsed;
			_timer.Start();
		}

		private void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			CloseForm(DialogResult.Cancel);
		}

		private void CloseForm(DialogResult result)
		{
			DialogResult = result;
			Close();
		}

		private void ParameterControl_Clicked(object sender, EventArgs e)
		{
			_timer.Stop();
			EffectParameterPickerControl control = (EffectParameterPickerControl)sender;
			PropertyInfo = control.PropertyInfo;
			SelectedControl = control;
			CloseForm(DialogResult.OK);
		}

		public PropertyDescriptor PropertyInfo { get; private set; }

		public int ParameterIndex { get; set; }

		public EffectParameterPickerControl SelectedControl { get; private set; }

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
				CloseForm(DialogResult.Cancel);
			}
			
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}
}
