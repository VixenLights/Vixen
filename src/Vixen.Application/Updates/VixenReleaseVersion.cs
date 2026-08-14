using System.Globalization;

namespace VixenApplication.Updates
{
	internal readonly record struct VixenReleaseVersion(int Major, int Minor, int Update) : IComparable<VixenReleaseVersion>
	{
		internal static bool TryParse(string? value, out VixenReleaseVersion version)
		{
			version = default;
			if (string.IsNullOrWhiteSpace(value))
			{
				return false;
			}

			var parts = value.Split('u');
			if (parts.Length is < 1 or > 2)
			{
				return false;
			}

			var releaseParts = parts[0].Split('.');
			if (releaseParts.Length != 2 ||
				!int.TryParse(releaseParts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var major) ||
				!int.TryParse(releaseParts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var minor) ||
				(parts.Length == 2 && (!int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var update) || update < 0)))
			{
				return false;
			}

			version = new VixenReleaseVersion(major, minor, parts.Length == 2 ? int.Parse(parts[1], CultureInfo.InvariantCulture) : 0);
			return true;
		}

		public int CompareTo(VixenReleaseVersion other)
		{
			var majorComparison = Major.CompareTo(other.Major);
			if (majorComparison != 0)
			{
				return majorComparison;
			}

			var minorComparison = Minor.CompareTo(other.Minor);
			return minorComparison != 0 ? minorComparison : Update.CompareTo(other.Update);
		}
	}
}
