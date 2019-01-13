using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Windows.Forms.VisualStyles;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class EffectTimeEditor : BaseForm
	{
		private const string timeFormat = @"mm\:ss\.fff";
		private TimeSpan _sequenceLength;
		private TimeSpan _previousTime;

		public EffectTimeEditor(TimeSpan start, TimeSpan duration, TimeSpan sequenceLength)
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
			Icon = Resources.Icon_Vixen3;
			Start = start;
			Duration = duration;
			_sequenceLength = sequenceLength;
			End = Start + Duration;
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
		public TimeSpan End
		{
			set
			{
				txtEndTime.Text = value.ToString(timeFormat);
			}
			get
			{
				TimeSpan end;
				TimeSpan.TryParseExact(txtEndTime.Text, timeFormat, CultureInfo.InvariantCulture, out end);
				return end;
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
			txtEndTime.MaskInputRejected -= new MaskInputRejectedEventHandler(txtEndTime_MaskInputRejected);
			txtEndTime.KeyDown -= new KeyEventHandler(txtEndTime_KeyDown);
			txtStartTime.KeyUp -= new KeyEventHandler(txtStartTime_KeyUp);
			txtEndTime.KeyUp -= new KeyEventHandler(txtEndTime_KeyUp);
			txtDuration.KeyUp -= new KeyEventHandler(txtDuration_KeyUp);
			base.Dispose(disposing);
		}

		private void EffectTimeEditor_Load(object sender, EventArgs e)
		{
			txtStartTime.Culture = CultureInfo.InvariantCulture;
			txtStartTime.Mask = txtStartTime.Mask = @"00:00.000";
			txtStartTime.MaskInputRejected += new MaskInputRejectedEventHandler(txtStartTime_MaskInputRejected);
			txtStartTime.KeyDown += new KeyEventHandler(txtStartTime_KeyDown);
			txtDuration.MaskInputRejected += new MaskInputRejectedEventHandler(txtDuration_MaskInputRejected);
			txtDuration.KeyDown += new KeyEventHandler(txtDuration_KeyDown);
			txtDuration.Culture = CultureInfo.InvariantCulture;
			txtDuration.Mask = txtDuration.Mask = @"00:00.000";
			txtEndTime.Culture = CultureInfo.InvariantCulture;
			txtEndTime.Mask = txtEndTime.Mask = @"00:00.000";
			txtEndTime.MaskInputRejected += new MaskInputRejectedEventHandler(txtEndTime_MaskInputRejected);
			txtEndTime.KeyDown += new KeyEventHandler(txtEndTime_KeyDown);
			txtStartTime.KeyUp += new KeyEventHandler(txtStartTime_KeyUp);
			txtEndTime.KeyUp += new KeyEventHandler(txtEndTime_KeyUp);
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

		private void txtEndTime_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtEndTime.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtEndTime, 100, -40, 3000);
			}
			else if (e.Position == txtEndTime.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtEndTime, 100, -40, 3000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtEndTime, 100, -40, 3000);
			}
			btnOk.Enabled = false;
		}

		private void txtDuration_KeyDown(object sender, KeyEventArgs e)
		{
			if (Duration != TimeSpan.Zero)
				_previousTime = Duration;
			toolTip.Hide(txtDuration);
		}

		private void txtStartTime_KeyDown(object sender, KeyEventArgs e)
		{
			if (Start != TimeSpan.Zero)
				_previousTime = Start;
			toolTip.Hide(txtStartTime);
			btnOk.Enabled = true;
		}

		private void txtEndTime_KeyDown(object sender, KeyEventArgs e)
		{
			if (End != TimeSpan.Zero)
				_previousTime = End;
			toolTip.Hide(txtEndTime);
			btnOk.Enabled = true;
		}

		private void txtStartTime_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan start;

			if (TimeSpan.TryParseExact(txtStartTime.Text, timeFormat, CultureInfo.InvariantCulture, out start))
			{
				if (Start + Duration > _sequenceLength)
				{
					toolTip.ToolTipTitle = "Invalid Start Time";
					toolTip.Show("Adjusted End Time will be greater then the Sequence End Time.", txtStartTime, 100, -40, 4000);
					Start = _previousTime;
				}
				else
				{
					_previousTime = Start;
					End = Start + Duration;
					btnOk.Enabled = true;
					return;
				}
				Start = _previousTime;
			}
			btnOk.Enabled = false;
		}

		private void txtDuration_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan duration;

			if (TimeSpan.TryParseExact(txtDuration.Text, timeFormat, CultureInfo.InvariantCulture, out duration))
			{
				if (Duration + Start > _sequenceLength)
				{
					toolTip.ToolTipTitle = "Invalid Duration";
					toolTip.Show("Adjusted End Time will be greater then the Sequence End Time.", txtDuration, 100, -40, 4000);
					Duration = _previousTime;
				}
				else
				{
					End = Start + Duration;
					btnOk.Enabled = true;
					return;
				}
				Duration = _previousTime;
			}
			btnOk.Enabled = false;
		}

		private void txtEndTime_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan end;

			if (TimeSpan.TryParseExact(txtEndTime.Text, timeFormat, CultureInfo.InvariantCulture, out end))
			{
				if (End > _sequenceLength)
				{
					toolTip.ToolTipTitle = "Invalid End Time";
					toolTip.Show("End Time will be greater then the Sequence End Time.", txtEndTime, 100, -40, 4000);
					End = _previousTime;
				}
				else
				{
					Duration = End - Start;
					btnOk.Enabled = true;
					return;
				}
			}
			btnOk.Enabled = false;
		}

		private void buttonBackground_MouseLeave(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImage;
		}

		private void buttonBackground_MouseHover(object sender, EventArgs e)
		{
			var btn = (Button)sender;
			btn.BackgroundImage = Resources.ButtonBackgroundImageHover;
		}

		private void txtEndTime_Enter(object sender, EventArgs e)
		{
			_previousTime = End;
		}

		private void txtStartTime_Enter(object sender, EventArgs e)
		{
			_previousTime = Start;
		}

		private void txtDuration_Enter(object sender, EventArgs e)
		{
			_previousTime = Duration;
		}
	}
}
