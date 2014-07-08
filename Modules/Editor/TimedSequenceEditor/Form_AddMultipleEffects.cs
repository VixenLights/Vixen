using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class Form_AddMultipleEffects : Form
	{
		private const string timeFormat = @"m\:ss\.fff";
		public Form_AddMultipleEffects()
		{
			InitializeComponent();
		}

		private void Form_AddMultipleEffects_Load(object sender, EventArgs e)
		{
			txtStartTime.Mask = txtDuration.Mask = txtDurationBetween.Mask = "0:00.000";
		}
		
		public int EffectCount { 
			get 
			{
				return int.Parse(txtEffectCount.Text); 
			}

			set
			{
				txtEffectCount.Text = value.ToString();
			}
		}
		
		public TimeSpan StartTime
		{
			get
			{
				TimeSpan StartTime;
				TimeSpan.TryParseExact(txtStartTime.Text, timeFormat, CultureInfo.InvariantCulture, out StartTime);
				return StartTime;
			}
			set
			{ 
				txtStartTime.Text = value.ToString(timeFormat); 
			}
		}

		public TimeSpan Duration
		{
			get
			{
				TimeSpan Duration;
				TimeSpan.TryParseExact(txtDuration.Text, timeFormat, CultureInfo.InvariantCulture, out Duration);
				return Duration;
			}
			set
			{
				txtDuration.Text = value.ToString(timeFormat);
			}
		}

		public TimeSpan DurationBetween
		{
			get
			{
				TimeSpan DurationBetween;
				TimeSpan.TryParseExact(txtDurationBetween.Text, timeFormat, CultureInfo.InvariantCulture, out DurationBetween);
				return DurationBetween;
			}
			set
			{
				txtDurationBetween.Text = value.ToString(timeFormat);
			}
		}

		private void txtStartTime_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtStartTime);
			btnOK.Enabled = true;
		}

		private void txtStartTime_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan StartTime;
			if (TimeSpan.TryParseExact(txtStartTime.Text, timeFormat, CultureInfo.InvariantCulture, out StartTime))
			{
				btnOK.Enabled = true;
			}
			else
			{
				btnOK.Enabled = false;
			}
		}

		private void txtStartTime_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtStartTime.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtStartTime, 0, -20, 5000);
			}
			else if (e.Position == txtStartTime.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtStartTime, 0, -20, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtStartTime, 0, -20, 5000);
			}
			btnOK.Enabled = false;
		}

		private void txtDuration_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtDuration.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtDuration, 0, -20, 5000);
			}
			else if (e.Position == txtDuration.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtDuration, 0, -20, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtDuration, 0, -20, 5000);
			}
			btnOK.Enabled = false;
		}

		private void txtDuration_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtDuration);
			btnOK.Enabled = true;
		}

		private void txtDuration_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan Duration;
			if (TimeSpan.TryParseExact(txtDuration.Text, timeFormat, CultureInfo.InvariantCulture, out Duration) && Duration.TotalSeconds > .009)
			{
				btnOK.Enabled = true;
			}
			else
			{
				if (TimeSpan.TryParseExact(txtDuration.Text, timeFormat, CultureInfo.InvariantCulture, out Duration) && Duration.TotalSeconds < .01)
				{
					toolTip.ToolTipTitle = "Invalid Time";
					toolTip.Show("Minimum effect duration is .01 seconds.", txtDuration, 0, -20, 5000);
				}
				btnOK.Enabled = false;
			}
		}

		private void txtDurationBetween_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtDurationBetween.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtDurationBetween, 0, -20, 5000);
			}
			else if (e.Position == txtDurationBetween.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtDurationBetween, 0, -20, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtDurationBetween, 0, -20, 5000);
			}
			btnOK.Enabled = false;
		}

		private void txtDurationBetween_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtDurationBetween);
			btnOK.Enabled = true;
		}

		private void txtDurationBetween_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan DurationBetween;
			if (TimeSpan.TryParseExact(txtDurationBetween.Text, timeFormat, CultureInfo.InvariantCulture, out DurationBetween))
				btnOK.Enabled = true;
			else
				btnOK.Enabled = false;
		}

		private void txtEffectCount_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtEffectCount);
			btnOK.Enabled = true;
		}

		private void txtEffectCount_KeyUp(object sender, KeyEventArgs e)
		{

			int EffectCount;
			if (int.TryParse(txtEffectCount.Text, out EffectCount))
				btnOK.Enabled = true;
			else
				btnOK.Enabled = false;

		}
	}
}
