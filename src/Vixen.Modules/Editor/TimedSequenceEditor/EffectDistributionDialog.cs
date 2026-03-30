using Common.Controls;
using Common.Controls.Theme;
using Common.Resources.Properties;
using System.ComponentModel;
using System.Globalization;

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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSpan StartTime
		{
			set { txtStartTime.TimeSpan = value; }
			get
			{
				return txtStartTime.TimeSpan; 
			}
			
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSpan EndTime
		{
			set { txtEndTime.TimeSpan = value; }
			get
			{
				return txtEndTime.TimeSpan; 
			}
			
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSpan SpecifiedEffectDuration
		{
			set { txtSpecifiedEffectDuration.TimeSpan = value; }
			get
			{
				return txtSpecifiedEffectDuration.TimeSpan; 
			}
			
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSpan SpacedPlacementDuration
		{
			set { txtSpacedPlacementDuration.TimeSpan = value; }
			get
			{
				return txtSpacedPlacementDuration.TimeSpan;
			}

		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TimeSpan EffectPlacementOverlap
		{
			set { txtSpacedPlacementDuration.TimeSpan = value; }
			get
			{
				return txtEffectPlacementOverlap.TimeSpan;
			}

		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RadioEqualDuration
		{
			get { return radioEqualDuration.Checked; }
			set { radioEqualDuration.Checked = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RadioDoNotChangeDuration
		{
			get { return radioDoNotChangeDuration.Checked; }
			set { radioDoNotChangeDuration.Checked = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RadioSpecifiedDuration
		{
			get { return radioSpecifiedDuration.Checked; }
			set { radioSpecifiedDuration.Checked = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RadioStairStep
		{
			get { return radioStairStep.Checked; }
			set { radioStairStep.Checked = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RadioPlacementSpacedDuration
		{
			get { return radioPlacementSpacedDuration.Checked; }
			set { radioPlacementSpacedDuration.Checked = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RadioDeterminePointStart
		{
			get { return radioStartAtFirst.Checked; }
			set { radioStartAtFirst.Checked = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RadioDeterminePointEnd
		{
			get { return radioStartAtLast.Checked; }
			set { radioStartAtLast.Checked = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool RadioEffectPlacementOverlap
		{
			get { return radioEffectPlacementOverlap.Checked; }
			set { radioEffectPlacementOverlap.Checked = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool StartWithFirst
		{
			get { return radioStartAtFirst.Checked; }
			set { radioStartAtFirst.Checked = value; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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
