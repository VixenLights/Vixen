using Newtonsoft.Json;

namespace VixenApplication.Updates
{
	internal sealed record GitHubRelease
	{
		[JsonProperty("tag_name")]
		public string? TagName { get; init; }

		[JsonProperty("body")]
		public string? Body { get; init; }

		[JsonProperty("draft")]
		public bool IsDraft { get; init; }

		[JsonProperty("prerelease")]
		public bool IsPrerelease { get; init; }

		[JsonProperty("published_at")]
		public DateTimeOffset? PublishedAt { get; init; }

		[JsonProperty("html_url")]
		public Uri? HtmlUrl { get; init; }

		[JsonProperty("assets")]
		public List<GitHubReleaseAsset> Assets { get; init; } = [];
	}
}
