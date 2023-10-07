using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;

namespace Common.VixenHelp
{
	public class VixenHelp
	{
		public enum HelpStrings
        {
            [Description("http://www.vixenlights.com/vixen-3-documentation/preview/adding-items-to-the-preview/smart-objects/icicles/")]
            Preview_Icicle,

			[Description("http://www.vixenlights.com/vixen-3-documentation/preview/adding-items-to-the-preview/smart-objects/mega-tree/")]
			Preview_MegaTree,

			[Description("http://www.vixenlights.com/vixen-3-documentation/preview/adding-items-to-the-preview/smart-objects/star/")]
			Preview_Star,

			[Description("http://www.vixenlights.com/vixen-3-documentation/preview/how-to/linking-elements/")]
			Preview_LinkElements,

			[Description("https://www.vixenlights.com/docs/usage/preview/preview-main-screen/#background")]
			Preview_Background,

			[Description("http://www.vixenlights.com/vixen-3-documentation/preview/main-preview-screen/")]
			Preview_Main,

			[Description("http://www.vixenlights.com/vixen-3-documentation/preview/adding-items-to-the-preview/basic-shapes/")]
			Preview_BasicShapes,

			[Description("http://www.vixenlights.com/vixen-3-documentation/preview/adding-items-to-the-preview/smart-objects/net/")]
			Preview_Net,

			[Description("http://www.vixenlights.com/vixen-3-documentation/preview/adding-items-to-the-preview/smart-objects/arch/")]
			Preview_Arch,

			[Description("http://www.vixenlights.com/vixen-3-documentation/preview/adding-items-to-the-preview/smart-objects/candy-cane/")]
			Preview_Cane,

			[Description("http://www.vixenlights.com/vixen-3-documentation/preview/custom-props/")] // TODO: Broken Link
			Preview_CustomShape,

			[Description("http://www.vixenlights.com/vixen-3-documentation/scheduling-a-show/show-scheduler/")]
			Scheduler_Main,

			[Description("http://www.vixenlights.com/vixen-3-documentation/scheduling-a-show/show-editor/")]
			Show_Editor,

			[Description("http://www.vixenlights.com/vixen-3-documentation/setup-configuration/")]
			Setup_Main,

	        [Description("http://www.vixenlights.com/vixen-3-documentation/setup-configuration/link-elements-to-controllers/")]
	        Patching,

			[Description("https://www.youtube.com/user/VixenLightsSoftware")]
	        YouTubeChannel,

	        [Description("http://www.vixenlights.com/vixen-3-documentation/sequencer/")]
	        Sequencer
		}
		
		public static void ShowHelp(HelpStrings helpString)
		{
			var psi = new ProcessStartInfo()
			{
				FileName = GetEnumDescription(helpString),
				UseShellExecute = true
			}; 
			
			Process.Start(psi);
		}

		public static string GetEnumDescription(Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());

			DescriptionAttribute[] attributes =
				(DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

			if (attributes != null && attributes.Length > 0)
				return attributes[0].Description;
			else
				return value.ToString();
		}
	}
}
