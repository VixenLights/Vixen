using System;
using System.Windows.Forms;
using Common.ValueTypes;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;

namespace VixenModules.EffectEditor.PercentageTypeEditor {
	public partial class PercentageTypeEditorControl : UserControl, IEffectEditorControl {
		public PercentageTypeEditorControl() {
			InitializeComponent();
		}

		public object[] EffectParameterValues {
			get { return new object[] { new Percentage(trackBar.Value * 0.01f) }; }
			set { trackBar.Value = (int)(((Percentage)value[0]) * 100); }
		}

		public IEffect TargetEffect { get; set; }

		private void trackBar_ValueChanged(object sender, EventArgs e) {
			labelValue.Text = trackBar.Value + "%";
		}
	}
}
