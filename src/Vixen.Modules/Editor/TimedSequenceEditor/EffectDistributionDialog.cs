using System.Globalization;
using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class EffectDistributionDialog : BaseForm
	{
		public EffectDistributionDialog(TimeSpan sequenceLength)
		{
			InitializeComponent();
			txtEndTime.Maximum = sequenceLength;
			txtStartTime.Maximum = sequenceLength;
			txtSpecifiedEffectDuration.Maximum = sequenceLength;
			txtSpacedPlacementDuration.Maximum = sequenceLength;
			txtEffectPlacementOverlap.Maximum = sequenceLength;

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
			set { txtStartTime.TimeSpan = value; }
			get
			{
				return txtStartTime.TimeSpan; 
			}
			
		}
		public TimeSpan EndTime
		{
			set { txtEndTime.TimeSpan = value; }
			get
			{
				return txtEndTime.TimeSpan; 
			}
			
		}
		public TimeSpan SpecifiedEffectDuration
		{
			set { txtSpecifiedEffectDuration.TimeSpan = value; }
			get
			{
				return txtSpecifiedEffectDuration.TimeSpan; 
			}
			
		}
		public TimeSpan SpacedPlacementDuration
		{
			set { txtSpacedPlacementDuration.TimeSpan = value; }
			get
			{
				return txtSpacedPlacementDuration.TimeSpan;
			}

		}
		public TimeSpan EffectPlacementOverlap
		{
			set { txtSpacedPlacementDuration.TimeSpan = value; }
			get
			{
				return txtEffectPlacementOverlap.TimeSpan;
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

			base.Dispose(disposing);
		}
		private void EffectDistributionDialog_Load(object sender, EventArgs e)
		{
		}

		#region Draw lines and GroupBox borders
		
		private void groupBoxes_Paint(object sender, PaintEventArgs e)
		{
			ThemeGroupBoxRenderer.GroupBoxesDrawBorder(sender, e, Font);
		}
		#endregion
	}
}
