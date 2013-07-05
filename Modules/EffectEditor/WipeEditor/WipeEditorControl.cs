using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module.EffectEditor;
using Vixen.Module.Effect;
using VixenModules.Effect.Wipe;
using VixenModules.App.Curves;
using VixenModules.App.ColorGradients;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Sys;

namespace VixenModules.EffectEditor.WipeEditor {
	public partial class WipeEditorControl : UserControl, IEffectEditorControl {
		public WipeEditorControl() {
			InitializeComponent();
		}

		public object[] EffectParameterValues {
			get {
				return new object[]{
					ColorGradient,
					WipeDirection,
					Curve,
					PulseTime,
					Color,
					ColorHandling

				};
			}
			set {

				if (value.Length != 6) {
					VixenSystem.Logging.Warning("Wipe effect parameters set with " + value.Length + " parameters");
					return;
				}

				ColorGradient = (ColorGradient)value[0];
				WipeDirection = (WipeDirection)value[1];
				Curve = (Curve)value[2];
				PulseTime = (int)value[3];
				Color = (RGB)value[4];
				ColorHandling = (WipeColorHandling)value[5];

			}
		}

		private IEffect _targetEffect;

		public IEffect TargetEffect {
			get { return _targetEffect; }
			set {
				_targetEffect = value;
				//Ensure target effect is passed through as these editors need it.
				colorTypeEditorControlStaticColor.TargetEffect = _targetEffect;
				colorGradientTypeEditorControlGradient.TargetEffect = _targetEffect;
			}
		}

		public WipeColorHandling ColorHandling {
			get {
				if (radioButtonStaticColor.Checked)
					return WipeColorHandling.StaticColor;
				if (radioButtonGradientOverWhole.Checked)
					return WipeColorHandling.GradientThroughWholeEffect;
				if (radioButtonGradientIndividual.Checked)
					return WipeColorHandling.GradientForEachPulse;
				if (radioButtonGradientAcrossItems.Checked)
					return WipeColorHandling.ColorAcrossItems;

				return WipeColorHandling.StaticColor;
			}
			set {
				switch (value) {
					case WipeColorHandling.StaticColor:
						radioButtonStaticColor.Checked = true;
						break;

					case WipeColorHandling.GradientThroughWholeEffect:
						radioButtonGradientOverWhole.Checked = true;
						break;

					case WipeColorHandling.GradientForEachPulse:
						radioButtonGradientIndividual.Checked = true;
						break;

					case WipeColorHandling.ColorAcrossItems:
						radioButtonGradientAcrossItems.Checked = true;
						break;
				}
			}
		}
		public RGB Color {
			get {
				return new RGB(colorTypeEditorControlStaticColor.ColorValue);
			}
			set {
				colorTypeEditorControlStaticColor.ColorValue = value;
			}
		}
		public ColorGradient ColorGradient {
			get { return this.colorGradientTypeEditorControlGradient.ColorGradientValue; }
			set { this.colorGradientTypeEditorControlGradient.ColorGradientValue = value; }
		}
		public int PulseTime {
			get { return (int)numericUpDownPulseLength.Value; }
			set { numericUpDownPulseLength.Value = value; }
		}
		public WipeDirection WipeDirection {
			get {
				if (radioWipeDown.Checked) return WipeDirection.Down;
				if (radioWipeUp.Checked) return WipeDirection.Up;
				if (radioWipeLeft.Checked) return WipeDirection.Left;
				else return WipeDirection.Right;

			}
			set {
				switch (value) {
					case WipeDirection.Up:
						radioWipeUp.Checked = true;
						break;
					case WipeDirection.Down:
						radioWipeDown.Checked = true;
						break;
					case WipeDirection.Right:
						radioWipeRight.Checked = true;
						break;
					case WipeDirection.Left:
						radioWipeLeft.Checked = true;
						break;

				}
			}
		}
		public Curve Curve {
			get { return curveTypeEditorControlEachPulse.CurveValue; }
			set { curveTypeEditorControlEachPulse.CurveValue = value; }
		}

	}
}
