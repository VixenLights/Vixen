namespace VixenModules.Preview.VixenPreview.Shapes
{
	public class PreviewMegaTreeSetupControl(PreviewLightBaseShape shape) : PreviewShapeBaseSetupControl(shape)
	{
		protected override void buttonHelp_Click(object sender, EventArgs e)
		{
			Common.VixenHelp.VixenHelp.ShowHelp(Common.VixenHelp.VixenHelp.HelpStrings.Preview_MegaTree);
		}
	}
}