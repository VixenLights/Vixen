using System;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	public class PreviewArchSetupControl : PreviewShapeBaseSetupControl
	{
		public PreviewArchSetupControl(PreviewLightBaseShape shape) : base(shape)
		{
		}

		protected override void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_Arch);
		}

	}
}