using System.Reflection;
using System.ComponentModel;
using System.Diagnostics;

namespace Common.VixenHelp
{
	public class VixenHelp
	{
		public enum HelpStrings
        {
            [Description("https://www.vixenlights.com/docs/usage/preview/smart-objects/preview-icicles/")]
            Preview_Icicle,

			[Description("https://www.vixenlights.com/docs/usage/preview/smart-objects/preview-mega-tree/")]
			Preview_MegaTree,

			[Description("https://www.vixenlights.com/docs/usage/preview/smart-objects/preview-star/")]
			Preview_Star,

			[Description("https://www.vixenlights.com/docs/usage/preview/preview-linking-elements/")]
			Preview_LinkElements,

			[Description("https://www.vixenlights.com/docs/usage/preview/preview-main-screen/#background")]
			Preview_Background,

			[Description("http://www.vixenlights.com/docs/usage/preview/preview-main-screen/")]
			Preview_Main,

			[Description("https://www.vixenlights.com/docs/usage/preview/preview-basic-shapes/")]
			Preview_BasicShapes,

			[Description("https://www.vixenlights.com/docs/usage/preview/smart-objects/preview-net/")]
			Preview_Net,

			[Description("https://www.vixenlights.com/docs/usage/preview/smart-objects/preview-arch/")]
			Preview_Arch,

			[Description("https://www.vixenlights.com/docs/usage/preview/smart-objects/preview-candy-cane/")]
			Preview_Cane,

			[Description("https://www.vixenlights.com/docs/usage/preview/preview-main-screen/#resizing-the-background-image")]
			Preview_ResizeBackground,

			[Description("https://www.vixenlights.com/docs/usage/preview/custom-prop-editor/")]
			Preview_CustomShape,

			[Description("https://www.vixenlights.com/docs/usage/scheduler/scheduling-show/")]
			Scheduler_Main,

			[Description("https://www.vixenlights.com/docs/usage/scheduler/schedule-show-editor/")]
			Show_Editor,

			[Description("https://www.vixenlights.com/docs/usage/display-setup/")]
			Setup_Main,

	        [Description("https://www.vixenlights.com/docs/usage/display-setup/display-patching/patching-controllers/")]
	        Patching,

			[Description("https://www.youtube.com/user/VixenLightsSoftware")]
	        YouTubeChannel,

	        [Description("https://www.vixenlights.com/docs/usage/sequencer/")]
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
