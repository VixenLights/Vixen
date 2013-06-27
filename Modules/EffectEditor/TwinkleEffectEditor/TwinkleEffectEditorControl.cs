using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;
using VixenModules.Effect.Twinkle;
using VixenModules.App.ColorGradients;

namespace VixenModules.EffectEditor.TwinkleEffectEditor
{
	public partial class TwinkleEffectEditorControl : UserControl, IEffectEditorControl
	{
		public TwinkleEffectEditorControl()
		{
			InitializeComponent();
			ColorGradient = new ColorGradient();
		}

		private IEffect _targetEffect;

		public IEffect TargetEffect
		{
			get { return _targetEffect; }
			set { _targetEffect = value; }
		}

		public object[] EffectParameterValues
		{
			get
			{
				return new object[]
				       	{
				       		IndividualElements,
				       		MinimumLevel,
				       		MaximumLevel,
				       		LevelVariation,
				       		AveragePulseTime,
				       		PulseTimeVariation,
				       		AverageCoverage,
				       		ColorHandling,
				       		StaticColor,
				       		ColorGradient,
				       		DepthOfEffect
				       	};
			}
			set
			{
				if (value.Length != 11) {
					VixenSystem.Logging.Warning("TwinkleEffectEditorControl: param vales set without 11 params.");
					return;
				}

				IndividualElements = (bool) value[0];
				MinimumLevel = (double) value[1];
				MaximumLevel = (double) value[2];
				LevelVariation = (int) value[3];
				AveragePulseTime = (int) value[4];
				PulseTimeVariation = (int) value[5];
				AverageCoverage = (int) value[6];
				ColorHandling = (TwinkleColorHandling) value[7];
				StaticColor = (Color) value[8];
				ColorGradient = (ColorGradient) value[9];
				DepthOfEffect = (int) value[10];
			}
		}

		public bool IndividualElements
		{
			get { return radioButtonIndividualElements.Checked || radioButtonApplyToLevel.Checked; }
			set { UpdateElementHandlingGroupBox(DepthOfEffect, value); }
		}

		public double MinimumLevel
		{
			get { return levelTypeEditorControlMinValue.LevelValue; }
			set { levelTypeEditorControlMinValue.LevelValue = value; }
		}

		public double MaximumLevel
		{
			get { return levelTypeEditorControlMaxValue.LevelValue; }
			set { levelTypeEditorControlMaxValue.LevelValue = value; }
		}

		public int LevelVariation
		{
			get { return (int) numericUpDownLevelVariation.Value; }
			set { numericUpDownLevelVariation.Value = value; }
		}

		public int AveragePulseTime
		{
			get { return (int) numericUpDownAveragePulseTime.Value; }
			set { numericUpDownAveragePulseTime.Value = value; }
		}

		public int PulseTimeVariation
		{
			get { return (int) numericUpDownPulseTimeVariation.Value; }
			set { numericUpDownPulseTimeVariation.Value = value; }
		}

		public int AverageCoverage
		{
			get { return (int) numericUpDownCoverage.Value; }
			set { numericUpDownCoverage.Value = value; }
		}

		public TwinkleColorHandling ColorHandling
		{
			get
			{
				if (radioButtonStaticColor.Checked)
					return TwinkleColorHandling.StaticColor;
				if (radioButtonGradientOverWhole.Checked)
					return TwinkleColorHandling.GradientThroughWholeEffect;
				if (radioButtonGradientIndividual.Checked)
					return TwinkleColorHandling.GradientForEachPulse;

				return TwinkleColorHandling.ColorAcrossItems;
			}
			set
			{
				switch (value) {
					case TwinkleColorHandling.GradientForEachPulse:
						radioButtonGradientIndividual.Checked = true;
						break;

					case TwinkleColorHandling.GradientThroughWholeEffect:
						radioButtonGradientOverWhole.Checked = true;
						break;

					case TwinkleColorHandling.StaticColor:
						radioButtonStaticColor.Checked = true;
						break;

					case TwinkleColorHandling.ColorAcrossItems:
						radioButtonGradientAcrossItems.Checked = true;
						break;
				}
			}
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

		public int DepthOfEffect
		{
			get
			{
				if (this.radioButtonIndividualElements.Checked)
					return 0;
				else
					return (int) numericUpDownDepthOfEffect.Value;
			}
			set { UpdateElementHandlingGroupBox(value, IndividualElements); }
		}

		private void UpdateElementHandlingGroupBox(int depthOfEffect, bool individualElements)
		{
			if (depthOfEffect == 0 && individualElements)
				radioButtonIndividualElements.Checked = true;
			else if (individualElements) {
				radioButtonApplyToLevel.Checked = true;
				numericUpDownDepthOfEffect.Value = depthOfEffect;
			}
			else
				radioButtonSynchronizedElements.Checked = true;
		}


		private void radioButtonEffectAppliesTo_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDownDepthOfEffect.Enabled = radioButtonApplyToLevel.Checked;
		}
	}
}