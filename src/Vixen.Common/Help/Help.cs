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
            PreviewIcicle,

			[Description("https://www.vixenlights.com/docs/usage/preview/smart-objects/preview-mega-tree/")]
			PreviewMegaTree,

			[Description("https://www.vixenlights.com/docs/usage/preview/smart-objects/preview-star/")]
			PreviewStar,

			[Description("https://www.vixenlights.com/docs/usage/preview/preview-linking-elements/")]
			PreviewLinkElements,

			[Description("https://www.vixenlights.com/docs/usage/preview/preview-main-screen/#background")]
			PreviewBackground,

			[Description("https://www.vixenlights.com/docs/usage/preview/preview-main-screen/")]
			PreviewMain,

			[Description("https://www.vixenlights.com/docs/usage/preview/preview-basic-shapes/")]
			PreviewBasicShapes,

			[Description("https://www.vixenlights.com/docs/usage/preview/smart-objects/preview-net/")]
			PreviewNet,

			[Description("https://www.vixenlights.com/docs/usage/preview/smart-objects/preview-arch/")]
			PreviewArch,

			[Description("https://www.vixenlights.com/docs/usage/preview/smart-objects/preview-candy-cane/")]
			PreviewCane,

			[Description("http://www.vixenlights.com/docs/")] // Deprecated: No longer documented
			PreviewCustomShape,

			[Description("https://www.vixenlights.com/docs/usage/scheduler/")]
			SchedulerMain,

			[Description("https://www.vixenlights.com/docs/usage/scheduler/schedule-show-editor/")]
			ShowEditor,

			[Description("https://www.vixenlights.com/docs/usage/display-setup/")]
			SetupMain,

	        [Description("https://www.vixenlights.com/docs/usage/display-setup/display-patching/")]
	        Patching,

			[Description("https://www.youtube.com/user/VixenLightsSoftware")]
	        YouTubeChannel,

	        [Description("https://www.vixenlights.com/docs/usage/sequencer/")]
	        Sequencer,
	        
	        [Description("https://www.vixenlights.com/docs/")]
	        Documentation
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

			if (attributes.Length > 0)
			{
				return attributes[0].Description;
			}
			
			return value.ToString();
		}
	}
}
