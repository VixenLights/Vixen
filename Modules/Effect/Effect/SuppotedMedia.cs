using Vixen.Module.Effect;

namespace VixenModules.Effect.Effect
{
	public class SupportedMediaExtensions
	{
		public static string[] SupportedVideoExtensions
		{
			get { return new[] { ".mp4", ".avi", ".mov", ".MTS" }; }
		}

		public static string[] SupportedGlediatorExtensions
		{
			get { return new[] { ".gled" }; }
		}

		public static string[] SupportedImageExtensions
		{
			get { return new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" }; }
		}
	}
}
