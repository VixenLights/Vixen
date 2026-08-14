namespace VixenApplication.Updates
{
	internal sealed class UnavailableUpdateService : IUpdateService
	{
		public Task<UpdateCheckResult?> CheckAsync(UpdateCheckRequest request, CancellationToken cancellationToken = default)
			=> Task.FromResult<UpdateCheckResult?>(null);
	}
}
