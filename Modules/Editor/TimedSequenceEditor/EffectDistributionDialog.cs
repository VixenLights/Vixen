using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public partial class EffectDistributionDialog : Form
	{
		public EffectDistributionDialog()
		{
			InitializeComponent();
		}
		public string ElementCount
		{
			set { labelElementCount.Text = string.Format("Effects selected: {0}", value); }
		}
		public string StartTime
		{
			get { return textBoxStartTime.Text; }
			set { textBoxStartTime.Text = value; }
		}
		public string EndTime
		{
			get { return textBoxEndTime.Text; }
			set { textBoxEndTime.Text = value; }
		}
		public string Duration
		{
			get { return textBoxDuration.Text; }
			set { textBoxDuration.Text = value; }
		}
		public string SpecifiedEffectDuration
		{
			get { return textBoxSpecifiedEffectDuration.Text; }
			set { textBoxSpecifiedEffectDuration.Text = value; }
		}
		public string SpacedPlacementDuration
		{
			get { return textBoxSpacedPlacementDuration.Text; }
			set { textBoxSpacedPlacementDuration.Text = value; }
		}
		public string EffectPlacementOverlap
		{
			get { return textBoxEffectPlacementOverlap.Text; }
			set { textBoxEffectPlacementOverlap.Text = value; }
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
			get;
			set;
		}
		public bool StartWithLast
		{
			get;
			set;
		}

		private void EffectDistributionDialog_Load(object sender, EventArgs e)
		{

		}
	}
}
