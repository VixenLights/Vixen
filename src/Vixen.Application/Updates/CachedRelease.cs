namespace VixenApplication.Updates
{
	internal sealed record CachedRelease(GitHubRelease Release, DateTimeOffset ExpiresAt);
}
