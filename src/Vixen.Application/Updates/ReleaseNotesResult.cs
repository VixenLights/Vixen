namespace VixenApplication.Updates
{
	internal sealed record ReleaseNotesResult(
		string ReleaseTag,
		string ReleaseNotes,
		Uri ReleasePageUri,
		DateTimeOffset? PublishedAt);
}
