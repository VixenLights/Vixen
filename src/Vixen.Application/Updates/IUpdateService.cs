namespace VixenApplication.Updates
{
	internal interface IUpdateService
	{
		Task<UpdateCheckResult?> CheckAsync(
			UpdateCheckRequest request,
			CancellationToken cancellationToken = default);

		Task<ReleaseNotesResult?> GetReleaseNotesAsync(
			string releaseTag,
			CancellationToken cancellationToken = default);
	}
}
