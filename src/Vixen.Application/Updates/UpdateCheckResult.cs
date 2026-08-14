namespace VixenApplication.Updates
{
	internal sealed record UpdateCheckResult(
		string LatestVersion,
		int? LatestBuildNumber,
		bool IsUpdateAvailable,
		string? ReleaseNotes,
		Uri ReleasePageUri,
		DateTimeOffset? PublishedAt,
		Uri? InstallerDownloadUri);
}
