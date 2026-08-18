using System.Globalization;

namespace VixenModules.Editor.TimedSequenceEditor.Forms.WPF.MarksDocker.Services
{
	internal static class PangolinBeyondMarkParser
	{
		private const string Header = "#,Name,Start,Color";
		private static readonly string[] TimeFormats = [@"mm\:ss\.fff", @"hh\:mm\:ss\.fff"];

		internal static bool TryParse(string csv, out IReadOnlyList<PangolinBeyondMarkRecord> marks, out string error)
		{
			marks = [];
			error = string.Empty;

			if (string.IsNullOrEmpty(csv))
			{
				error = "The Pangolin Beyond CSV file is empty.";
				return false;
			}

			var lines = csv.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n').ToList();
			if (lines[^1].Length == 0)
			{
				lines.RemoveAt(lines.Count - 1);
			}

			if (lines.Count == 0 || !string.Equals(lines[0], Header, StringComparison.Ordinal))
			{
				error = "Line 1 must be the Pangolin Beyond header '#,Name,Start,Color'.";
				return false;
			}

			var parsedMarks = new List<PangolinBeyondMarkRecord>(lines.Count - 1);
			for (var index = 1; index < lines.Count; index++)
			{
				var lineNumber = index + 1;
				var fields = lines[index].Split(',');
				if (fields.Length != 4)
				{
					error = $"Line {lineNumber} must contain exactly four comma-separated columns.";
					return false;
				}

				if (!TimeSpan.TryParseExact(fields[2], TimeFormats, CultureInfo.InvariantCulture, out var startTime))
				{
					error = $"Line {lineNumber} has an invalid start time '{fields[2]}'.";
					return false;
				}

				if (!TryParseBgrColor(fields[3], out var color))
				{
					error = $"Line {lineNumber} has an invalid BGR color '{fields[3]}'.";
					return false;
				}

				parsedMarks.Add(new PangolinBeyondMarkRecord(fields[1], startTime, color));
			}

			marks = parsedMarks;
			return true;
		}

		private static bool TryParseBgrColor(string value, out Color color)
		{
			color = Color.Empty;
			if (value.Length != 6 || value.Any(character => !Uri.IsHexDigit(character)))
			{
				return false;
			}

			if (!byte.TryParse(value.AsSpan(0, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var blue)
				|| !byte.TryParse(value.AsSpan(2, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var green)
				|| !byte.TryParse(value.AsSpan(4, 2), NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out var red))
			{
				return false;
			}

			color = Color.FromArgb(red, green, blue);
			return true;
		}
	}
}
