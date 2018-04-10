using System.Linq;

namespace VixenModules.Editor.TimedSequenceEditor
{
	public class TimeFormats
	{
		private static readonly string[] _positiveFormats =
		{
			@"m\:ss", @"m\:ss\.f", @"m\:ss\.ff", @"m\:ss\.fff",
			@"\:ss", @"\:ss\.f", @"\:ss\.ff", @"\:ss\.fff",
			@"%s", @"s\.f", @"s\.ff", @"s\.fff"
		};

		private static readonly string[] _negativeFormats =
		{
			@"\-m\:ss", @"\-m\:ss\.f", @"\-m\:ss\.ff", @"\-m\:ss\.fff",
			@"\-\:ss", @"\-\:ss\.f", @"\-\:ss\.ff", @"\-\:ss\.fff",
			@"\-%s", @"\-s\.f", @"\-s\.ff", @"\-s\.fff"
		};

		public static string[] AllFormats
		{
			get { return _negativeFormats.Concat(_positiveFormats).ToArray(); }
		}

		public static string[] PositiveFormats
		{
			get { return _positiveFormats; }
		}

		public static string[] NegativeFormats
		{
			get { return _negativeFormats; }
		}
	}
}
