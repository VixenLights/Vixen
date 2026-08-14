using Newtonsoft.Json;

namespace VixenApplication.Updates
{
	internal sealed record GitHubReleaseAsset
	{
		[JsonProperty("name")]
		public string? Name { get; init; }

		[JsonProperty("browser_download_url")]
		public Uri? BrowserDownloadUri { get; init; }
	}
}
