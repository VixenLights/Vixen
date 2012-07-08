using System;
using System.Linq;
using System.Windows.Forms;
using Vixen.Module.Effect;
using Vixen.Module.EffectEditor;
using Vixen.Services;
using Vixen.Sys;

namespace BasicInputManagement {
	public partial class EffectParameterSetup : Form {
		private IEffectModuleDescriptor _descriptor;

		public EffectParameterSetup(IEffectModuleDescriptor descriptor, IEffectModuleInstance effect) {
			InitializeComponent();

			_descriptor = descriptor;

			IEffectEditorControl[] controls = ApplicationServices.GetEffectEditorControls(descriptor.TypeId).ToArray();
			for(int i=0; i<descriptor.Parameters.Count; i++) {
				IEffectEditorControl effectEditorControl = controls[i];
				Control control = effectEditorControl as Control;
				effectEditorControl.TargetEffect = effect;
				control.Dock = DockStyle.Top;
				tableLayoutPanel.Controls.Add(new Label { Text = descriptor.Parameters[i].Name });
				tableLayoutPanel.Controls.Add(control);
			}

			object[] effectParameters = (effect != null) ? effect.ParameterValues : new object[_descriptor.Parameters.Count];
			if(controls.Length == 1) {
				// One control for all parameters.
				controls[0].EffectParameterValues = effectParameters;
			} else {
				// One control per parameter.
				for(int i = 0; i < descriptor.Parameters.Count; i++) {
					controls[i].EffectParameterValues = new[] { effectParameters[i] };
				}
			}
		}

		public IEffectModuleInstance Effect { get; private set; }

		private void buttonOK_Click(object sender, EventArgs e) {
			Effect = ApplicationServices.Get<IEffectModuleInstance>(_descriptor.TypeId);
			Effect.ParameterValues = panelContainer.Controls.Cast<IEffectEditorControl>().SelectMany(x => x.EffectParameterValues).ToArray();
		}
	}
}
