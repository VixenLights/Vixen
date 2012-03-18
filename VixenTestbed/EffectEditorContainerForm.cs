using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;

namespace VixenTestbed {
	public partial class EffectEditorContainerForm : Form {
		private IEffectModuleInstance _effectModule;
		private IEffectEditorControl[] _controls;

		public EffectEditorContainerForm(IEffectModuleInstance effectModule) {
			if(effectModule == null) throw new ArgumentNullException();
			_effectModule = effectModule;

			InitializeComponent();
		}

		public object[] GetValues() {
			return _controls.SelectMany(x => x.EffectParameterValues).ToArray();
		}

		private void EffectEditorContainerForm_Load(object sender, EventArgs e) {
			IEnumerable<IEffectEditorControl> controls = ApplicationServices.GetEffectEditorControls(_effectModule.Descriptor.TypeId);
			if(controls == null) {
				MessageBox.Show("Appropriate effect editors could not be found.");
				DialogResult = DialogResult.Cancel;
			} else {
				_controls = controls.ToArray();
				tableLayoutPanel.RowCount = _controls.Length;
				for(int i = 0; i < _controls.Length; i++) {
					Control control = _controls[i] as Control;
					control.Anchor = AnchorStyles.Right;
					Label label = new Label { Text = _effectModule.Parameters[i].Name };
					label.Anchor = AnchorStyles.Left;
					tableLayoutPanel.Controls.Add(label, 0, i);
					tableLayoutPanel.Controls.Add(control, 1, i);
				}
			}
		}
	}
}
