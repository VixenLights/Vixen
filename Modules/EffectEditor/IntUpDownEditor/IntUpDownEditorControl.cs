using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.IntUpDownEditor {
	public partial class IntUpDownEditorControl : UserControl, IEffectEditorControl {
		public IntUpDownEditorControl() {
			InitializeComponent();
		}

		public object[] EffectParameterValues {
			get { return new object[] { (int)nudValue.Value };}
			set { nudValue.Value = (int)value[0]; }
		}

		public IEffect TargetEffect { get; set; }
	}
}
