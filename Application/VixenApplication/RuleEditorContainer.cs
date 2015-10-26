using System;
using System.Drawing;
using System.Windows.Forms;
using Common.Controls;
using Common.Resources.Properties;

namespace VixenApplication
{
	public partial class RuleEditorContainer : BaseForm
	{
		public RuleEditorContainer(Control control)
		{
			InitializeComponent();
			Icon = Resources.Icon_Vixen3;

			ClientSize = new Size(Math.Max(ClientSize.Width, control.Width), Math.Max(ClientSize.Height, control.Height));
			control.Dock = DockStyle.Fill;
			panelContainer.Controls.Add(control);
		}
	}
}