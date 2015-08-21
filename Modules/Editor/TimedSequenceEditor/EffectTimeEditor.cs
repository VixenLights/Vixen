using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class EffectTimeEditor : Form
	{
		private const string timeFormat = @"mm\:ss\.fff";
		public EffectTimeEditor(TimeSpan start, TimeSpan duration)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Start = start;
			Duration = duration;
		}

		
		public TimeSpan Duration
		{
			get
			{
				TimeSpan duration;
				TimeSpan.TryParseExact(txtDuration.Text, timeFormat, CultureInfo.InvariantCulture, out duration);
				return duration;
			}
			set
			{
				txtDuration.Text = value.ToString(timeFormat);
			}
		}
		public TimeSpan Start
		{
			set
			{
				txtStartTime.Text = value.ToString(timeFormat);
			}
			get 
			{
				TimeSpan start;
				TimeSpan.TryParseExact(txtStartTime.Text, timeFormat, CultureInfo.InvariantCulture, out start);
				return start;	
			}
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}

			txtStartTime.MaskInputRejected -= new MaskInputRejectedEventHandler(txtStartTime_MaskInputRejected);
			txtStartTime.KeyDown -= new KeyEventHandler(txtStartTime_KeyDown);
			txtDuration.MaskInputRejected -= new MaskInputRejectedEventHandler(txtDuration_MaskInputRejected);
			txtDuration.KeyDown -= new KeyEventHandler(txtDuration_KeyDown);
			txtStartTime.KeyUp -= new KeyEventHandler(txtStartTime_KeyUp);
			txtDuration.KeyUp -= new KeyEventHandler(txtDuration_KeyUp);
			base.Dispose(disposing);
		}

		private void EffectTimeEditor_Load(object sender, EventArgs e)
		{
			txtStartTime.Mask = txtDuration.Mask = @"00:00.000";
			txtStartTime.MaskInputRejected += new MaskInputRejectedEventHandler(txtStartTime_MaskInputRejected);
			txtStartTime.KeyDown += new KeyEventHandler(txtStartTime_KeyDown);
			txtDuration.MaskInputRejected += new MaskInputRejectedEventHandler(txtDuration_MaskInputRejected);
			txtDuration.KeyDown += new KeyEventHandler(txtDuration_KeyDown);
			txtStartTime.KeyUp += new KeyEventHandler(txtStartTime_KeyUp);
			txtDuration.KeyUp += new KeyEventHandler(txtDuration_KeyUp);
		}

		private void txtStartTime_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtStartTime.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtStartTime, 100, -40, 3000);
			} else if (e.Position == txtStartTime.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtStartTime, 100, -40, 3000);
			} else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtStartTime, 100, -40, 3000);
			}
			btnOk.Enabled = false;

		}

		private void txtDuration_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtDuration.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtDuration, 100, -40, 3000);
			} else if (e.Position == txtStartTime.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtDuration, 100, -40, 3000);
			} else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtDuration, 100, -40, 3000);
			}
			btnOk.Enabled = false;
		}

		private void txtDuration_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtDuration);
			
		}

		private void txtStartTime_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtStartTime);
			btnOk.Enabled = true;
		}

		private void txtStartTime_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan start;
			if (TimeSpan.TryParseExact(txtStartTime.Text, timeFormat, CultureInfo.InvariantCulture, out start))
			{
				btnOk.Enabled = true;
			} else
			{
				btnOk.Enabled = false;
			}
			
		}

		private void txtDuration_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan duration;
			if (TimeSpan.TryParseExact(txtDuration.Text, timeFormat, CultureInfo.InvariantCulture, out duration))
			{
				btnOk.Enabled = true;
			} else
			{
				btnOk.Enabled = false;
			}
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;

		}
	}
}
