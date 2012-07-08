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

		IEffect _targetEffect;
		public IEffect TargetEffect
		{
			get { return _targetEffect; }
			set { _targetEffect = value; }
		}

		public object[] EffectParameterValues
		{
			get
			{
				return new object[] {
					IndividualChannels,
					MinimumLevel,
					MaximumLevel,
					LevelVariation,
					AveragePulseTime,
					PulseTimeVariation,
					AverageCoverage,
					ColorHandling,
					StaticColor,
					ColorGradient
				};
			}
			set
			{
				if (value.Length != 10) {
					VixenSystem.Logging.Warning("TwinkleEffectEditorControl: param vales set without 10 params.");
					return;
				}

				IndividualChannels = (bool)value[0];
				MinimumLevel = (double)value[1];
				MaximumLevel = (double)value[2];
				LevelVariation = (int)value[3];
				AveragePulseTime = (int)value[4];
				PulseTimeVariation = (int)value[5];
				AverageCoverage = (int)value[6];
				ColorHandling = (TwinkleColorHandling)value[7];
				StaticColor = (Color)value[8];
				ColorGradient = (ColorGradient)value[9];
			}
		}

		public bool IndividualChannels
		{
			get { return radioButtonIndividualChannels.Checked; }
			set { if (value) radioButtonIndividualChannels.Checked = true; else radioButtonSynchronizedChannels.Checked = true; }
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
			get { return (int)numericUpDownLevelVariation.Value; }
			set { numericUpDownLevelVariation.Value = value; }
		}

		public int AveragePulseTime
		{
			get { return (int)numericUpDownAveragePulseTime.Value; }
			set { numericUpDownAveragePulseTime.Value = value; }
		}

		public int PulseTimeVariation
		{
			get { return (int)numericUpDownPulseTimeVariation.Value; }
			set { numericUpDownPulseTimeVariation.Value = value; }
		}

		public int AverageCoverage
		{
			get { return (int)numericUpDownCoverage.Value; }
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
				return TwinkleColorHandling.GradientForEachPulse;
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

	}
}
