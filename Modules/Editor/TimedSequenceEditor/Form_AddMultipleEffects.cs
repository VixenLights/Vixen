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
			CalculatePossibleEffects();
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

		public string EffectName
		{ 
			set
			{
				this.Text = string.Format("Add {0} effects",value);
			}
		}

		public TimeSpan SequenceLength { get; set; }

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

		private bool TimeExistsForAddition()
		{
			//Determine the end time of the last effect to be added, if it is beyond sequence length return false, else true
			TimeSpan LastEffectEndTime = StartTime + (TimeSpan.FromTicks(Duration.Ticks * EffectCount) + TimeSpan.FromTicks(DurationBetween.Ticks * (EffectCount -1)));
			return (LastEffectEndTime > SequenceLength ? false : true);
		}

		private void CalculatePossibleEffects()
		{
			//This gives us the available amount of time to fill.
			int i = 1;
			TimeSpan WorkingTime = SequenceLength - StartTime;
			TimeSpan UsesTime = (TimeSpan.FromTicks(Duration.Ticks * i) + TimeSpan.FromTicks(DurationBetween.Ticks * (i - 1))); ;
			while (UsesTime < WorkingTime)
			{
				i++;
				UsesTime = (TimeSpan.FromTicks(Duration.Ticks * i) + TimeSpan.FromTicks(DurationBetween.Ticks * (i - 1)));
			}
			//Get rid of the straw that broke the camels back.
			i--;
			txtEffectCount.Maximum = i;
			lblPossibleEffects.Text = string.Format("{0} effects possible.", i);
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
				CalculatePossibleEffects();
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
				CalculatePossibleEffects();
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
			{
				btnOK.Enabled = true;
				CalculatePossibleEffects();
			}
			else btnOK.Enabled = false;
		}

		private void txtEffectCount_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtEffectCount);
			btnOK.Enabled = true;
		}

		private void txtEffectCount_KeyUp(object sender, KeyEventArgs e)
		{
			int EffectCount;
			btnOK.Enabled = (int.TryParse(txtEffectCount.Text, out EffectCount) ? true : false);
		}
		
		private void btnOK_Click(object sender, EventArgs e)
		{
			//Double check for calculations
			if (!TimeExistsForAddition())
			{
				DialogResult proceedDialog = MessageBox.Show("At least one effect would be placed beyond the sequence length, and will not be added.\n\nWould you like to proceed anyway?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
				if (proceedDialog == DialogResult.No)
				{
					this.DialogResult = DialogResult.None;
				}
			}
		}
	}
}
