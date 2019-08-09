using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Common.Controls.Theme;
using WeifenLuo.WinFormsUI.Docking;

namespace VixenModules.Preview.VixenPreview
{
	public partial class VixenPreviewSetupPropertiesDocument : DockContent
	{
		public VixenPreviewSetupPropertiesDocument()
		{
			InitializeComponent();
			ThemeUpdateControls.UpdateControls(this);
		}

		public void ShowSetupControl(VixenModules.Preview.VixenPreview.Shapes.DisplayItemBaseControl setupControl)
		{
			Controls.Clear();
			if (setupControl != null) {
				Controls.Add(setupControl);
				setupControl.Dock = DockStyle.Fill;
				Text = setupControl.Title;
			}
			else {
				Text = @"Properties";
			}
		}

		public void ClearSetupControl()
		{
			Controls.Clear();
			Text = @"Properties";
		}
	}
}