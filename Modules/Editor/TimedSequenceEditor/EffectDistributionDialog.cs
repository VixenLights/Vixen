using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class EffectDistributionDialog : BaseForm
	{
		private const string timeFormat = @"m\:ss\.fff";
		public EffectDistributionDialog()
		{
			InitializeComponent();
			ForeColor = ThemeColorTable.ForeColor;
			BackColor = ThemeColorTable.BackgroundColor;
			ThemeUpdateControls.UpdateControls(this);
		}
		public string ElementCount
		{
			set { labelElementCount.Text = string.Format("Effects selected: {0}", value); }
		}

		//public TimeSpan Start
		//{
		//	set
		//	{
		//		txtStartTime.Text = value.ToString(timeFormat);
		//	}
		//	get
		//	{
		//		TimeSpan start;
		//		TimeSpan.TryParseExact(txtStartTime.Text, timeFormat, CultureInfo.InvariantCulture, out start);
		//		return start;
		//	}
		//}

		
		public TimeSpan StartTime
		{
			set { txtStartTime.Text = value.ToString(timeFormat); }
			get
			{
				TimeSpan StartTime;
				TimeSpan.TryParseExact(txtStartTime.Text, timeFormat, CultureInfo.InvariantCulture, out StartTime);
				return StartTime; 
			}
			
		}
		public TimeSpan EndTime
		{
			set { txtEndTime.Text = value.ToString(timeFormat); }
			get
			{
				TimeSpan EndTime;
				TimeSpan.TryParseExact(txtEndTime.Text, timeFormat, CultureInfo.InvariantCulture, out EndTime);
				return EndTime; 
			}
			
		}
		public TimeSpan SpecifiedEffectDuration
		{
			set { txtSpecifiedEffectDuration.Text = value.ToString(timeFormat); }
			get
			{
				TimeSpan SpecifiedEffectDuration;
				TimeSpan.TryParseExact(txtSpecifiedEffectDuration.Text, timeFormat, CultureInfo.InvariantCulture, out SpecifiedEffectDuration);
				return SpecifiedEffectDuration; 
			}
			
		}
		public TimeSpan SpacedPlacementDuration
		{
			set { txtSpacedPlacementDuration.Text = value.ToString(timeFormat); }
			get
			{
				TimeSpan SpacedPlacementDuration;
				TimeSpan.TryParseExact(txtSpacedPlacementDuration.Text, timeFormat, CultureInfo.InvariantCulture, out SpacedPlacementDuration);
				return SpacedPlacementDuration;
			}

		}
		public TimeSpan EffectPlacementOverlap
		{
			set { txtSpacedPlacementDuration.Text = value.ToString(timeFormat); }
			get
			{
				TimeSpan EffectPlacementOverlap;
				TimeSpan.TryParseExact(txtEffectPlacementOverlap.Text, timeFormat, CultureInfo.InvariantCulture, out EffectPlacementOverlap);
				return EffectPlacementOverlap;
			}

		}
		public bool RadioEqualDuration
		{
			get { return radioEqualDuration.Checked; }
			set { radioEqualDuration.Checked = value; }
		}
		public bool RadioDoNotChangeDuration
		{
			get { return radioDoNotChangeDuration.Checked; }
			set { radioDoNotChangeDuration.Checked = value; }
		}
		public bool RadioSpecifiedDuration
		{
			get { return radioSpecifiedDuration.Checked; }
			set { radioSpecifiedDuration.Checked = value; }
		}
		public bool RadioStairStep
		{
			get { return radioStairStep.Checked; }
			set { radioStairStep.Checked = value; }
		}
		public bool RadioPlacementSpacedDuration
		{
			get { return radioPlacementSpacedDuration.Checked; }
			set { radioPlacementSpacedDuration.Checked = value; }
		}
		public bool RadioDeterminePointStart
		{
			get { return radioStartAtFirst.Checked; }
			set { radioStartAtFirst.Checked = value; }
		}
		public bool RadioDeterminePointEnd
		{
			get { return radioStartAtLast.Checked; }
			set { radioStartAtLast.Checked = value; }
		}
		public bool RadioEffectPlacementOverlap
		{
			get { return radioEffectPlacementOverlap.Checked; }
			set { radioEffectPlacementOverlap.Checked = value; }
		}
		public bool StartWithFirst
		{
			get { return radioStartAtFirst.Checked; }
			set { radioStartAtFirst.Checked = value; }
		}
		public bool StartWithLast
		{
			get { return radioStartAtLast.Checked;}
			set { radioStartAtLast.Checked = value; }
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
			txtStartTime.KeyUp -= new KeyEventHandler(txtStartTime_KeyUp);

			txtEndTime.MaskInputRejected -= new MaskInputRejectedEventHandler(txtEndTime_MaskInputRejected);
			txtEndTime.KeyDown -= new KeyEventHandler(txtEndTime_KeyDown);
			txtEndTime.KeyUp -= new KeyEventHandler(txtEndTime_KeyUp);

			txtSpecifiedEffectDuration.MaskInputRejected -= new MaskInputRejectedEventHandler(txtSpecifiedEffectDuration_MaskInputRejected);
			txtSpecifiedEffectDuration.KeyDown -= new KeyEventHandler(txtSpecifiedEffectDuration_KeyDown);
			txtSpecifiedEffectDuration.KeyUp -= new KeyEventHandler(txtSpecifiedEffectDuration_KeyUp);

			txtSpacedPlacementDuration.MaskInputRejected -= new MaskInputRejectedEventHandler(txtSpacedPlacementDuration_MaskInputRejected);
			txtSpacedPlacementDuration.KeyDown -= new KeyEventHandler(txtSpacedPlacementDuration_KeyDown);
			txtSpacedPlacementDuration.KeyUp -= new KeyEventHandler(txtSpacedPlacementDuration_KeyUp);

			txtEffectPlacementOverlap.MaskInputRejected -= new MaskInputRejectedEventHandler(txtEffectPlacementOverlap_MaskInputRejected);
			txtEffectPlacementOverlap.KeyDown -= new KeyEventHandler(txtEffectPlacementOverlap_KeyDown);
			txtEffectPlacementOverlap.KeyUp -= new KeyEventHandler(txtEffectPlacementOverlap_KeyUp);
			base.Dispose(disposing);
		}
		private void EffectDistributionDialog_Load(object sender, EventArgs e)
		{
			txtStartTime.Culture = txtEndTime.Culture = CultureInfo.InvariantCulture;
			txtStartTime.Mask = txtEndTime.Mask = "0:00.000";
			txtSpacedPlacementDuration.Culture = txtSpacedPlacementDuration.Culture =
				txtEffectPlacementOverlap.Culture = CultureInfo.InvariantCulture;
			txtSpecifiedEffectDuration.Mask = txtSpacedPlacementDuration.Mask = txtEffectPlacementOverlap.Mask = "0:00.000";
			txtSpecifiedEffectDuration.Text = txtSpacedPlacementDuration.Text = txtEffectPlacementOverlap.Text = "0:00.100";

			txtStartTime.MaskInputRejected += new MaskInputRejectedEventHandler(txtStartTime_MaskInputRejected);
			txtStartTime.KeyDown += new KeyEventHandler(txtStartTime_KeyDown);
			txtStartTime.KeyUp += new KeyEventHandler(txtStartTime_KeyUp);

			txtEndTime.MaskInputRejected += new MaskInputRejectedEventHandler(txtEndTime_MaskInputRejected);
			txtEndTime.KeyDown += new KeyEventHandler(txtEndTime_KeyDown);
			txtEndTime.KeyUp += new KeyEventHandler(txtEndTime_KeyUp);

			txtSpecifiedEffectDuration.MaskInputRejected += new MaskInputRejectedEventHandler(txtSpecifiedEffectDuration_MaskInputRejected);
			txtSpecifiedEffectDuration.KeyDown += new KeyEventHandler(txtSpecifiedEffectDuration_KeyDown);
			txtSpecifiedEffectDuration.KeyUp += new KeyEventHandler(txtSpecifiedEffectDuration_KeyUp);

			txtSpacedPlacementDuration.MaskInputRejected += new MaskInputRejectedEventHandler(txtSpacedPlacementDuration_MaskInputRejected);
			txtSpacedPlacementDuration.KeyDown += new KeyEventHandler(txtSpacedPlacementDuration_KeyDown);
			txtSpacedPlacementDuration.KeyUp += new KeyEventHandler(txtSpacedPlacementDuration_KeyUp);

			txtEffectPlacementOverlap.MaskInputRejected += new MaskInputRejectedEventHandler(txtEffectPlacementOverlap_MaskInputRejected);
			txtEffectPlacementOverlap.KeyDown += new KeyEventHandler(txtEffectPlacementOverlap_KeyDown);
			txtEffectPlacementOverlap.KeyUp += new KeyEventHandler(txtEffectPlacementOverlap_KeyUp);
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

		private void txtEndTime_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtEndTime.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtEndTime, 0, -20, 5000);
			}
			else if (e.Position == txtEndTime.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtEndTime, 0, -20, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtEndTime, 0, -20, 5000);
			}
			btnOK.Enabled = false;
		}
		private void txtEndTime_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtEndTime);
			btnOK.Enabled = true;
		}
		private void txtEndTime_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan EndTime;
			if (TimeSpan.TryParseExact(txtEndTime.Text, timeFormat, CultureInfo.InvariantCulture, out EndTime))
			{
				btnOK.Enabled = true;
			}
			else
			{
				btnOK.Enabled = false;
			}
		}
		private void txtSpecifiedEffectDuration_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtSpecifiedEffectDuration.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtSpecifiedEffectDuration, 0, -20, 5000);
			}
			else if (e.Position == txtSpecifiedEffectDuration.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtSpecifiedEffectDuration, 0, -20, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtSpecifiedEffectDuration, 0, -20, 5000);
			}
			btnOK.Enabled = false;
		}
		private void txtSpecifiedEffectDuration_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtSpecifiedEffectDuration);
			btnOK.Enabled = true;
		}
		private void txtSpecifiedEffectDuration_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan SpecifiedEffectDuration;
			if (TimeSpan.TryParseExact(txtSpecifiedEffectDuration.Text, timeFormat, CultureInfo.InvariantCulture, out SpecifiedEffectDuration) && SpecifiedEffectDuration.TotalSeconds > .009)
			{
				btnOK.Enabled = true;
			}
			else
			{
				if (TimeSpan.TryParseExact(txtSpecifiedEffectDuration.Text, timeFormat, CultureInfo.InvariantCulture, out SpecifiedEffectDuration) && SpecifiedEffectDuration.TotalSeconds < .01)
				{
					toolTip.ToolTipTitle = "Invalid Time";
					toolTip.Show("Minimum effect duration is .01 seconds.", txtSpecifiedEffectDuration, 0, -20, 5000);
				}
				btnOK.Enabled = false;
			}
		}
		private void txtSpacedPlacementDuration_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtSpacedPlacementDuration.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtSpacedPlacementDuration, 0, -20, 5000);
			}
			else if (e.Position == txtSpacedPlacementDuration.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtSpacedPlacementDuration, 0, -20, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtSpacedPlacementDuration, 0, -20, 5000);
			}
			btnOK.Enabled = false;
		}
		private void txtSpacedPlacementDuration_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtSpacedPlacementDuration);
			btnOK.Enabled = true;
		}
		private void txtSpacedPlacementDuration_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan SpacedPlacementDuration;
			if (TimeSpan.TryParseExact(txtSpacedPlacementDuration.Text, timeFormat, CultureInfo.InvariantCulture, out SpacedPlacementDuration))
			{
				btnOK.Enabled = true;
			}
			else
			{
				btnOK.Enabled = false;
			}
		}
		private void txtEffectPlacementOverlap_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
		{
			if (txtEffectPlacementOverlap.MaskFull)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot enter any more data into the time field.", txtEffectPlacementOverlap, 0, -20, 5000);
			}
			else if (e.Position == txtEffectPlacementOverlap.Mask.Length)
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You cannot add extra number to the end of this time.", txtEffectPlacementOverlap, 0, -20, 5000);
			}
			else
			{
				toolTip.ToolTipTitle = "Invalid Time";
				toolTip.Show("You can only add numeric characters (0-9) into this time field.", txtEffectPlacementOverlap, 0, -20, 5000);
			}
			btnOK.Enabled = false;
		}
		private void txtEffectPlacementOverlap_KeyDown(object sender, KeyEventArgs e)
		{
			toolTip.Hide(txtEffectPlacementOverlap);
			btnOK.Enabled = true;
		}
		private void txtEffectPlacementOverlap_KeyUp(object sender, KeyEventArgs e)
		{
			TimeSpan EffectPlacementOverlap;
			if (TimeSpan.TryParseExact(txtEffectPlacementOverlap.Text, timeFormat, CultureInfo.InvariantCulture, out EffectPlacementOverlap))
			{
				btnOK.Enabled = true;
			}
			else
			{
				btnOK.Enabled = false;
			}
		}

		#region Draw lines and GroupBox borders
		
		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
		#endregion

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
