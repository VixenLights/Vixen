using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;
using VixenModules.Effect.Spin;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using Common.ValueTypes;

namespace VixenModules.EffectEditor.SpinEffectEditor
{
	public partial class SpinEffectEditorControl : UserControl, IEffectEditorControl
	{
		public SpinEffectEditorControl()
		{
			InitializeComponent();
		}

		private IEffect _targetEffect;

		public IEffect TargetEffect
		{
			get { return _targetEffect; }
			set
			{
				_targetEffect = value;
				//Ensure target effect is passed through as these editors need it.
				colorTypeEditorControlStaticColor.TargetEffect = _targetEffect;
				colorGradientTypeEditorControlGradient.TargetEffect = _targetEffect;
			}
		}

		public object[] EffectParameterValues
		{
			get
			{
				return new object[]
				       	{
				       		SpeedFormat,
				       		PulseLengthFormat,
				       		ColorHandling,
				       		RevolutionCount,
				       		RevolutionFrequency,
				       		RevolutionTime,
				       		PulseTime,
				       		PulsePercentage,
				       		DefaultLevel,
				       		StaticColor,
				       		ColorGradient,
				       		PulseCurve,
				       		ReverseSpin,
				       		DepthOfEffect
				       	};
			}
			set
			{
				if (value.Length != 14) {
					VixenSystem.Logging.Warning("Spin effect parameters set with " + value.Length + " parameters");
					return;
				}

				ColorHandling = (SpinColorHandling) value[2];
				RevolutionCount = (double) value[3];
				RevolutionFrequency = (double) value[4];
				RevolutionTime = (int) value[5];
				PulseTime = (int) value[6];
				PulsePercentage = (int) value[7];
				DefaultLevel = (double) value[8];
				StaticColor = (Color) value[9];
				ColorGradient = (ColorGradient) value[10];
				PulseCurve = (Curve) value[11];
				ReverseSpin = (bool) value[12];
				DepthOfEffect = (int) value[13];

				// set these last: setting them results in some of the other values being auto-calculated on the form.
				SpeedFormat = (SpinSpeedFormat) value[0];
				PulseLengthFormat = (SpinPulseLengthFormat) value[1];
			}
		}

		public SpinSpeedFormat SpeedFormat
		{
			get
			{
				if (radioButtonRevolutionCount.Checked)
					return SpinSpeedFormat.RevolutionCount;
				if (radioButtonRevolutionFrequency.Checked)
					return SpinSpeedFormat.RevolutionFrequency;
				if (radioButtonRevolutionTime.Checked)
					return SpinSpeedFormat.FixedTime;

				return SpinSpeedFormat.RevolutionCount;
			}
			set
			{
				switch (value) {
					case SpinSpeedFormat.RevolutionCount:
						radioButtonRevolutionCount.Checked = true;
						break;

					case SpinSpeedFormat.RevolutionFrequency:
						radioButtonRevolutionFrequency.Checked = true;
						break;

					case SpinSpeedFormat.FixedTime:
						radioButtonRevolutionTime.Checked = true;
						break;
				}
			}
		}

		public SpinPulseLengthFormat PulseLengthFormat
		{
			get
			{
				if (radioButtonPulseFixedTime.Checked)
					return SpinPulseLengthFormat.FixedTime;
				if (radioButtonPulsePercentage.Checked)
					return SpinPulseLengthFormat.PercentageOfRevolution;
				if (radioButtonPulseEvenlyDistributed.Checked)
					return SpinPulseLengthFormat.EvenlyDistributedAcrossSegments;

				return SpinPulseLengthFormat.EvenlyDistributedAcrossSegments;
			}
			set
			{
				switch (value) {
					case SpinPulseLengthFormat.EvenlyDistributedAcrossSegments:
						radioButtonPulseEvenlyDistributed.Checked = true;
						break;

					case SpinPulseLengthFormat.FixedTime:
						radioButtonPulseFixedTime.Checked = true;
						break;

					case SpinPulseLengthFormat.PercentageOfRevolution:
						radioButtonPulsePercentage.Checked = true;
						break;
				}
			}
		}

		public SpinColorHandling ColorHandling
		{
			get
			{
				if (radioButtonStaticColor.Checked)
					return SpinColorHandling.StaticColor;
				if (radioButtonGradientOverWhole.Checked)
					return SpinColorHandling.GradientThroughWholeEffect;
				if (radioButtonGradientIndividual.Checked)
					return SpinColorHandling.GradientForEachPulse;
				if (radioButtonGradientAcrossItems.Checked)
					return SpinColorHandling.ColorAcrossItems;

				return SpinColorHandling.StaticColor;
			}
			set
			{
				switch (value) {
					case SpinColorHandling.StaticColor:
						radioButtonStaticColor.Checked = true;
						break;

					case SpinColorHandling.GradientThroughWholeEffect:
						radioButtonGradientOverWhole.Checked = true;
						break;

					case SpinColorHandling.GradientForEachPulse:
						radioButtonGradientIndividual.Checked = true;
						break;

					case SpinColorHandling.ColorAcrossItems:
						radioButtonGradientAcrossItems.Checked = true;
						break;
				}
			}
		}

		public double RevolutionCount
		{
			get { return (double) numericUpDownRevolutionCount.Value; }
			set
			{
				if (value < (double) numericUpDownRevolutionCount.Minimum)
					value = (double) numericUpDownRevolutionCount.Minimum;
				if (value > (double) numericUpDownRevolutionCount.Maximum)
					value = (double) numericUpDownRevolutionCount.Maximum;

				numericUpDownRevolutionCount.Value = (decimal) value;
			}
		}

		public double RevolutionFrequency
		{
			get { return (double) numericUpDownRevolutionFrequency.Value; }
			set
			{
				if (value < (double) numericUpDownRevolutionFrequency.Minimum)
					value = (double) numericUpDownRevolutionFrequency.Minimum;
				if (value > (double) numericUpDownRevolutionFrequency.Maximum)
					value = (double) numericUpDownRevolutionFrequency.Maximum;

				numericUpDownRevolutionFrequency.Value = (decimal) value;
			}
		}

		public int RevolutionTime
		{
			get { return (int) numericUpDownRevolutionTime.Value; }
			set
			{
				if (value < (int) numericUpDownRevolutionTime.Minimum)
					value = (int) numericUpDownRevolutionTime.Minimum;
				if (value > (int) numericUpDownRevolutionTime.Maximum)
					value = (int) numericUpDownRevolutionTime.Maximum;

				numericUpDownRevolutionTime.Value = value;
			}
		}

		public int PulseTime
		{
			get { return (int) numericUpDownPulseTime.Value; }
			set
			{
				if (value < (int) numericUpDownPulseTime.Minimum)
					value = (int) numericUpDownPulseTime.Minimum;
				if (value > (int) numericUpDownPulseTime.Maximum)
					value = (int) numericUpDownPulseTime.Maximum;

				numericUpDownPulseTime.Value = value;
			}
		}

		public int PulsePercentage
		{
			get { return (int) numericUpDownPulsePercentage.Value; }
			set
			{
				if (value < (int) numericUpDownPulsePercentage.Minimum)
					value = (int) numericUpDownPulsePercentage.Minimum;
				if (value > (int) numericUpDownPulsePercentage.Maximum)
					value = (int) numericUpDownPulsePercentage.Maximum;

				numericUpDownPulsePercentage.Value = value;
			}
		}

		public double DefaultLevel
		{
			get { return levelTypeEditorControlDefaultLevel.LevelValue; }
			set { levelTypeEditorControlDefaultLevel.LevelValue = value; }
		}

		public Color StaticColor
		{
			get { return colorTypeEditorControlStaticColor.ColorValue; }
			set { colorTypeEditorControlStaticColor.ColorValue = value; }
		}

		public ColorGradient ColorGradient
		{
			get { return colorGradientTypeEditorControlGradient.ColorGradientValue; }
			set { colorGradientTypeEditorControlGradient.ColorGradientValue = value; }
		}

		public Curve PulseCurve
		{
			get { return curveTypeEditorControlEachPulse.CurveValue; }
			set { curveTypeEditorControlEachPulse.CurveValue = value; }
		}

		public bool ReverseSpin
		{
			get { return checkBoxReverse.Checked; }
			set { checkBoxReverse.Checked = value; }
		}

		public int DepthOfEffect
		{
			get
			{
				if (radioButtonApplyToAllElements.Checked)
					return 0;
				else
					return (int) numericUpDownDepthOfEffect.Value;
			}
			set
			{
				if (value == 0)
					radioButtonApplyToAllElements.Checked = true;
				else {
					radioButtonApplyToLevel.Checked = true;
					numericUpDownDepthOfEffect.Value = value;
				}
			}
		}

		private void radioButtonRevolutionItem_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonRevolutionCount.Checked) {
				numericUpDownRevolutionCount.Enabled = true;
				numericUpDownRevolutionFrequency.Enabled = false;
				numericUpDownRevolutionTime.Enabled = false;
			}
			if (radioButtonRevolutionFrequency.Checked) {
				numericUpDownRevolutionCount.Enabled = false;
				numericUpDownRevolutionFrequency.Enabled = true;
				numericUpDownRevolutionTime.Enabled = false;
			}
			if (radioButtonRevolutionTime.Checked) {
				numericUpDownRevolutionCount.Enabled = false;
				numericUpDownRevolutionFrequency.Enabled = false;
				numericUpDownRevolutionTime.Enabled = true;
			}

			RecalculateValues();
		}

		private void radioButtonPulseItem_CheckedChanged(object sender, EventArgs e)
		{
			if (radioButtonPulseFixedTime.Checked) {
				numericUpDownPulsePercentage.Enabled = false;
				numericUpDownPulseTime.Enabled = true;
			}
			if (radioButtonPulsePercentage.Checked) {
				numericUpDownPulsePercentage.Enabled = true;
				numericUpDownPulseTime.Enabled = false;
			}
			if (radioButtonPulseEvenlyDistributed.Checked) {
				numericUpDownPulsePercentage.Enabled = false;
				numericUpDownPulseTime.Enabled = false;
			}

			RecalculateValues();
		}

		private void RecalculateValues()
		{
			if (TargetEffect == null)
				return;

			//List<ElementNode> renderNodes = RGBModule.FindAllRenderableChildren(TargetEffect.TargetNodes);
			//int targetNodeCount = renderNodes.Count;

			double totalTime = TargetEffect.TimeSpan.TotalMilliseconds;
			double pulseConstant = 0; // how much of each pulse is a constant time
			double pulseFractional = 0; // how much of each pulse is a fraction of a single spin
			double revCount = 0; // number of revolutions
			double revTime = 0; // single revolution time (ms)

			// figure out the relative length of a individual pulse
			if (radioButtonPulseFixedTime.Checked) {
				pulseConstant = PulseTime;
			}
			else if (radioButtonPulsePercentage.Checked) {
				pulseFractional = PulsePercentage/100.0;
			}
			else if (radioButtonPulseEvenlyDistributed.Checked) {
				pulseFractional = 1.0d/TargetEffect.TargetNodes.Length;
			}

			// figure out either the revolution count or time, based on what data we have
			if (radioButtonRevolutionCount.Checked) {
				revCount = RevolutionCount;
				revTime = (totalTime - pulseConstant)/(revCount + pulseFractional);
			}
			else if (radioButtonRevolutionFrequency.Checked) {
				revTime = (1.0/RevolutionFrequency)*1000.0; // convert Hz to period ms
				revCount = ((totalTime - pulseConstant)/revTime) - pulseFractional;
			}
			else if (radioButtonRevolutionTime.Checked) {
				revTime = RevolutionTime;
				revCount = ((totalTime - pulseConstant)/revTime) - pulseFractional;
			}

			double pulTime = pulseConstant + (revTime*pulseFractional);
			double pulPercent = pulTime/revTime*100.0;

			// now update the appropriate other value boxes, based on which one is selected
			if (!radioButtonPulseFixedTime.Checked) {
				PulseTime = (int) Math.Round(pulTime);
			}
			if (!radioButtonPulsePercentage.Checked) {
				PulsePercentage = (int) Math.Round(pulPercent);
			}
			if (!radioButtonRevolutionCount.Checked) {
				RevolutionCount = revCount;
			}
			if (!radioButtonRevolutionFrequency.Checked) {
				RevolutionFrequency = 1.0/(revTime/1000.0);
			}
			if (!radioButtonRevolutionTime.Checked) {
				RevolutionTime = (int) Math.Round(revTime);
			}
		}

		private void numericUpDownAny_ValueChanged(object sender, EventArgs e)
		{
			RecalculateValues();
		}

		private void SpinEffectEditorControl_Load(object sender, EventArgs e)
		{
			RecalculateValues();
		}

		private void radioButtonEffectAppliesTo_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDownDepthOfEffect.Enabled = radioButtonApplyToLevel.Checked;
		}
	}
}