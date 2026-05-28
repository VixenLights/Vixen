namespace VixenModules.App.ExportWizard;

/// <summary>Validates FPP host name or IP address strings entered in the Export Wizard.</summary>
internal static class FppHostValidator
{
	/// <summary>
	/// Returns <see langword="true"/> if <paramref name="value"/> is a valid IPv4 or IPv6 address,
	/// or a syntactically valid DNS host name; <see langword="false"/> otherwise.
	/// </summary>
	/// <remarks>
	/// The method rejects strings that contain <c>/</c>, <c>\</c>, or <c>:</c> so that URIs
	/// with a scheme (e.g. <c>http://host</c>) and UNC paths are always rejected.
	/// </remarks>
	/// <param name="value">The candidate host or IP string to validate.</param>
	internal static bool IsHostAddressValid(string value)
	{
		if (string.IsNullOrWhiteSpace(value)) return false;

		// Accept any valid IP address (v4 or v6).
		if (System.Net.IPAddress.TryParse(value, out _)) return true;

		// Reject strings that look like URIs or UNC paths.
		if (value.Contains('/') || value.Contains('\\') || value.Contains(':')) return false;

		// Accept syntactically valid DNS host names (plain or FQDN).
		return Uri.CheckHostName(value) != UriHostNameType.Unknown;
	}
}
