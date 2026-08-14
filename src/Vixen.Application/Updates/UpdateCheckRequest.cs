namespace VixenApplication.Updates
{
	internal sealed record UpdateCheckRequest(
		UpdateChannel Channel,
		string InstalledVersion,
		bool IncludeReleaseNotes);
}
