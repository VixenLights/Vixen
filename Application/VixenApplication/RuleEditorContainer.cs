using System;
using System.Drawing;
using System.Windows.Forms;

namespace VixenApplication
{
	public partial class RuleEditorContainer : Form
	{
		public RuleEditorContainer(Control control)
		{
			InitializeComponent();

			ClientSize = new Size(Math.Max(ClientSize.Width, control.Width), Math.Max(ClientSize.Height, control.Height));
			control.Dock = DockStyle.Fill;
			panelContainer.Controls.Add(control);
		}
	}
}