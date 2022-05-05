using System;

namespace VixenModules.Preview.VixenPreview.Shapes
{
	public class PreviewIcicleSetupControl : PreviewShapeBaseSetupControl
	{
		public PreviewIcicleSetupControl(PreviewLightBaseShape shape) : base(shape)
		{
		}

		protected override void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_Icicle);
		}
		
	}
}