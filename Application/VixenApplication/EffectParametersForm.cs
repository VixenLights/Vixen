using System;
using System.Linq;
using System.Windows.Forms;
using Vixen.Module.EffectEditor;
using Vixen.Services;
using Vixen.Sys;

namespace VixenApplication
{
	public partial class EffectParametersForm : Form
	{
		public EffectParametersForm(Guid effectModuleId)
		{
			InitializeComponent();

			panelContainer.Controls.Clear();
			IEffectEditorControl[] controls = ApplicationServices.GetEffectEditorControls(effectModuleId).ToArray();
			foreach (Control control in controls) {
				control.Dock = DockStyle.Top;
				panelContainer.Controls.Add(control);
			}
		}

		public object[] EffectParameters { get; private set; }

		private void buttonOK_Click(object sender, EventArgs e)
		{
			EffectParameters =
				panelContainer.Controls.Cast<IEffectEditorControl>().SelectMany(x => x.EffectParameterValues).ToArray();
		}
	}
}