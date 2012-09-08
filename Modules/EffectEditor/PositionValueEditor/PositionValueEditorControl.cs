using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Data.Value;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.PositionValueEditor {
	public partial class PositionValueEditorControl : UserControl, IEffectEditorControl {
		public PositionValueEditorControl() {
			InitializeComponent();
		}

		public object[] EffectParameterValues {
			get { return new object[] { new PositionValue(trackBar.Value * 0.01f) }; }
			set { trackBar.Value = (int)(((PositionValue)value[0]).Position * 100); }
		}

		public IEffect TargetEffect { get; set; }

		private void trackBar_ValueChanged(object sender, EventArgs e) {
			labelValue.Text = trackBar.Value + "%";
		}
	}
}
