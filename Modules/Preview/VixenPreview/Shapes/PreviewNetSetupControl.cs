using System;
using System.Windows.Forms;
using Common.Controls.Scaling;
using Common.Controls.Theme;
using Common.Resources;
using Common.Resources.Properties;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	public  class PreviewNetSetupControl : PreviewShapeBaseSetupControl
	{
		public PreviewNetSetupControl(PreviewBaseShape shape) : base(shape)
		{
		}

		protected override void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_Net);
		}

	}
}