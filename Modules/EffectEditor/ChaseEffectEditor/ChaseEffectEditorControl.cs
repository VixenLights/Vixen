using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Sys;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;
using VixenModules.Effect.Chase;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.Reflection;
using Common.ValueTypes;

namespace VixenModules.EffectEditor.ChaseEffectEditor
{
	public partial class ChaseEffectEditorControl : UserControl, IEffectEditorControl
	{
		public ChaseEffectEditorControl()
		{
			InitializeComponent();
		}
		
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
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
				       		ColorHandling,
				       		PulseOverlap,
				       		DefaultLevel,
				       		StaticColor,
				       		ColorGradient,
				       		PulseCurve,
				       		ChaseMovement,
				       		DepthOfEffect,
							ExtendPulseToStart,
							ExtendPulseToEnd
				       	};
			}
			set
			{
				if (value.Length != 10) {
					Logging.Warn("Chase effect parameters set with " + value.Length + " parameters");
					return;
				}

				ColorHandling = (ChaseColorHandling) value[0];
				PulseOverlap = (int) value[1];
				DefaultLevel = (double) value[2];
				StaticColor = (Color) value[3];
				ColorGradient = (ColorGradient) value[4];
				PulseCurve = (Curve) value[5];
				ChaseMovement = (Curve) value[6];
				DepthOfEffect = (int) value[7];
				ExtendPulseToStart = (bool)value[8];
				ExtendPulseToEnd = (bool) value[9];
				
			}
		}

		public ChaseColorHandling ColorHandling
		{
			get
			{
				if (radioButtonStaticColor.Checked)
					return ChaseColorHandling.StaticColor;
				if (radioButtonGradientOverWhole.Checked)
					return ChaseColorHandling.GradientThroughWholeEffect;
				if (radioButtonGradientIndividual.Checked)
					return ChaseColorHandling.GradientForEachPulse;
				if (radioButtonGradientAcrossItems.Checked)
					return ChaseColorHandling.ColorAcrossItems;

				return ChaseColorHandling.StaticColor;
			}
			set
			{
				switch (value) {
					case ChaseColorHandling.StaticColor:
						radioButtonStaticColor.Checked = true;
						break;

					case ChaseColorHandling.GradientThroughWholeEffect:
						radioButtonGradientOverWhole.Checked = true;
						break;

					case ChaseColorHandling.GradientForEachPulse:
						radioButtonGradientIndividual.Checked = true;
						break;

					case ChaseColorHandling.ColorAcrossItems:
						radioButtonGradientAcrossItems.Checked = true;
						break;
				}
			}
		}

		public int PulseOverlap
		{
			get { return (int) numericUpDownPulseTimeOverlap.Value; }
			set
			{
				if (value < (int) numericUpDownPulseTimeOverlap.Minimum)
					value = (int) numericUpDownPulseTimeOverlap.Minimum;
				if (value > (int) numericUpDownPulseTimeOverlap.Maximum)
					value = (int) numericUpDownPulseTimeOverlap.Maximum;

				numericUpDownPulseTimeOverlap.Value = value;
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

		public Curve ChaseMovement
		{
			get { return curveTypeEditorControlChaseMovement.CurveValue; }
			set { curveTypeEditorControlChaseMovement.CurveValue = value; }
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

		public bool ExtendPulseToEnd
		{
			get
			{
				return chkExtendPulseToEnd.Checked;
			}

			set
			{
				chkExtendPulseToEnd.Checked = value;
			}
		}

		public bool ExtendPulseToStart
		{
			get
			{
				return chkExtendPulseToStart.Checked;
			}

			set
			{
				chkExtendPulseToStart.Checked = value;
			}
		}

		private void radioButtonEffectAppliesTo_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDownDepthOfEffect.Enabled = radioButtonApplyToLevel.Checked;
		}
	}
}