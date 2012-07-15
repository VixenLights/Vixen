using System;
using System.Windows.Forms;

namespace VixenApplication {
	public partial class RuleEditorContainer : Form {
		public RuleEditorContainer(Control control) {
			InitializeComponent();
			control.Dock = DockStyle.Fill;
			panelContainer.Controls.Add(control);
		}
	}
}
