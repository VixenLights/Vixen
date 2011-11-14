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
using Vixen.Commands.KnownDataTypes;

namespace SampleEffectEditor {
	public partial class SampleEffectEditorControl : UserControl, IEffectEditorControl {
		public SampleEffectEditorControl() {
			InitializeComponent();
		}

		IEffect _targetEffect;
		public IEffect TargetEffect
		{
			get { return _targetEffect; }
			set { _targetEffect = value; }
		}

		public object[] EffectParameterValues {
			get { return new object[] { (Level)(double)numericUpDownLevel.Value }; }
			set {
				if(value[0] is Level) {
					numericUpDownLevel.Value = (decimal)(double)(Level)value[0];
				} else {
					numericUpDownLevel.Value = 0;
				}
			}
		}
	}
}
