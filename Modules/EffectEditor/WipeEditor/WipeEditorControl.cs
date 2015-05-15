using System;
using System.Windows.Forms;
using NLog;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Wipe;

namespace VixenModules.EffectEditor.WipeEditor {
	public partial class WipeEditorControl : UserControl, IEffectEditorControl {
		public WipeEditorControl() {
			InitializeComponent();
		}
		private static Logger Logging = LogManager.GetCurrentClassLogger();

		public object[] EffectParameterValues {
			get {
				return new object[]{
					ColorGradient,
					WipeDirection,
					Curve,
					PulseTime,
					WipeByCount,
					PassCount,
					PulsePercent
				};
			}
			set {

				if (value.Length != 7) {
					Logging.Warn("Wipe effect parameters set with " + value.Length + " parameters");
					return;
				}

				ColorGradient = (ColorGradient)value[0];
				WipeDirection = (WipeDirection)value[1];
				Curve = (Curve)value[2];
				PulseTime = (int)value[3];
				WipeByCount = (bool)value[4];
				PassCount = (int)value[5];
				PulsePercent = (double)value[6];
			}
		}

		private IEffect _targetEffect;

		public IEffect TargetEffect {
			get { return _targetEffect; }
			set {
				_targetEffect = value;
				//Ensure target effect is passed through as these editors need it.
				colorGradientTypeEditorControlGradient.TargetEffect = _targetEffect;
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

		public double PulsePercent { 
			get { return (double)numericUpDownPulseWidth.Value; }
			set { numericUpDownPulseWidth.Value = (decimal)value; } 
		}

		public int PassCount {
			get { return (int)numericUpDownNumPasses.Value; }
			set {
                //Added to correct divide by 0 exception when Number of passes is set to 0 
                //and it is selected in the UI
                if (value < 1)
                    numericUpDownNumPasses.Value = 1;
                else
                    numericUpDownNumPasses.Value = value; 
            }
 
		}

		public bool WipeByCount {
			get { return radioNumPasses.Checked; }
			set { radioNumPasses.Checked = value; } 
		}

		public WipeDirection WipeDirection {
			get {
				if (radioWipeDown.Checked) return WipeDirection.Down;
				if (radioWipeUp.Checked) return WipeDirection.Up;
				if (radioWipeLeft.Checked) return WipeDirection.Left;
				if (radioWipeIn.Checked) return WipeDirection.In;
				if (radioWipeOut.Checked) return WipeDirection.Out;
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
					case WipeDirection.In:
						radioWipeIn.Checked = true;
						break;
					case WipeDirection.Out:
						radioWipeOut.Checked = true;
						break;

				}
			}
		}
		public Curve Curve {
			get { return curveTypeEditorControlEachPulse.CurveValue; }
			set { curveTypeEditorControlEachPulse.CurveValue = value; }
		}

		private void radioNumPasses_CheckedChanged(object sender, EventArgs e)
		{
			numericUpDownNumPasses.Enabled = radioNumPasses.Checked;
            //Added logic go prevent divide by 0 exception.
            if (numericUpDownNumPasses.Value <= 0) {
                numericUpDownNumPasses.Value = 1;
            }
			numericUpDownPulseWidth.Enabled = radioNumPasses.Checked;
			numericUpDownPulseLength.Enabled = !radioNumPasses.Checked;
	}

}
}
