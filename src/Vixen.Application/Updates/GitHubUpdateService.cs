using System.Net;
using Newtonsoft.Json;
using NLog;
using LogManager = NLog.LogManager;

namespace VixenApplication.Updates
{
	internal sealed class GitHubUpdateService(HttpClient httpClient) : IUpdateService
	{
		private static readonly Logger Logging = LogManager.GetCurrentClassLogger();
		private static readonly Uri FallbackReleasePageUri = new("https://github.com/VixenLights/Vixen/releases");
		private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		private readonly SemaphoreSlim _cacheLock = new(1, 1);
		private readonly Dictionary<UpdateChannel, CachedRelease> _cache = [];

		public async Task<UpdateCheckResult?> CheckAsync(UpdateCheckRequest request, CancellationToken cancellationToken = default)
		{
			ArgumentNullException.ThrowIfNull(request);

			var release = await GetReleaseAsync(request.Channel, cancellationToken).ConfigureAwait(false);
			return release is null ? null : CreateResult(request, release);
		}

		private async Task<GitHubRelease?> GetReleaseAsync(UpdateChannel channel, CancellationToken cancellationToken)
		{
			if (_cache.TryGetValue(channel, out var cached) && cached.ExpiresAt > DateTimeOffset.UtcNow)
			{
				return cached.Release;
			}

			await _cacheLock.WaitAsync(cancellationToken).ConfigureAwait(false);
			try
			{
				if (_cache.TryGetValue(channel, out cached) && cached.ExpiresAt > DateTimeOffset.UtcNow)
				{
					return cached.Release;
				}

				var release = channel == UpdateChannel.Release
					? await GetLatestReleaseAsync(cancellationToken).ConfigureAwait(false)
					: await GetLatestDevelopmentReleaseAsync(cancellationToken).ConfigureAwait(false);
				if (release is not null)
				{
					_cache[channel] = new CachedRelease(release, DateTimeOffset.UtcNow.AddMinutes(5));
				}

				return release;
			}
			finally
			{
				_cacheLock.Release();
			}
		}

		private async Task<GitHubRelease?> GetLatestReleaseAsync(CancellationToken cancellationToken)
		{
			var release = await GetAsync<GitHubRelease>("releases/latest", cancellationToken).ConfigureAwait(false);
			if (release is null || release.IsDraft || release.IsPrerelease || !VixenReleaseVersion.TryParse(release.TagName, out _))
			{
				Logging.Warn("GitHub returned an invalid latest Vixen release.");
				return null;
			}

			return release;
		}

		private async Task<GitHubRelease?> GetLatestDevelopmentReleaseAsync(CancellationToken cancellationToken)
		{
			var releases = await GetAsync<List<GitHubRelease>>("releases?per_page=5", cancellationToken).ConfigureAwait(false);
			return releases?
				.Where(release => !release.IsDraft && release.IsPrerelease && TryGetDevelopmentBuildNumber(release.TagName, out _))
				.MaxBy(release => TryGetDevelopmentBuildNumber(release.TagName, out var buildNumber) ? buildNumber : -1);
		}

		private async Task<T?> GetAsync<T>(string relativeUri, CancellationToken cancellationToken)
		{
			try
			{
				using var response = await _httpClient.GetAsync(relativeUri, cancellationToken).ConfigureAwait(false);
				if (!response.IsSuccessStatusCode)
				{
					if (response.StatusCode == HttpStatusCode.Forbidden && response.Headers.TryGetValues("X-RateLimit-Remaining", out var values) && values.Contains("0"))
					{
						Logging.Warn("GitHub update check was rate limited.");
					}
					else
					{
						Logging.Warn("GitHub update check returned HTTP status {StatusCode}.", response.StatusCode);
					}

					return default;
				}

				var json = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
				return JsonConvert.DeserializeObject<T>(json);
			}
			catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
			{
				Logging.Warn("GitHub update check timed out.");
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (JsonException exception)
			{
				Logging.Warn(exception, "GitHub update check returned malformed JSON.");
			}
			catch (HttpRequestException exception)
			{
				Logging.Warn(exception, "GitHub update check failed.");
			}

			return default;
		}

		private static UpdateCheckResult? CreateResult(UpdateCheckRequest request, GitHubRelease release)
		{
			if (request.Channel == UpdateChannel.Development && TryGetDevelopmentBuildNumber(release.TagName, out var latestBuildNumber))
			{
				var hasInstalledBuildNumber = int.TryParse(request.InstalledVersion, out var installedBuildNumber);
				return new UpdateCheckResult(
					latestBuildNumber.ToString(),
					latestBuildNumber,
					hasInstalledBuildNumber && latestBuildNumber > installedBuildNumber,
					request.IncludeReleaseNotes ? release.Body ?? string.Empty : null,
					release.HtmlUrl ?? FallbackReleasePageUri,
					release.PublishedAt);
			}

			if (request.Channel == UpdateChannel.Release &&
				VixenReleaseVersion.TryParse(request.InstalledVersion, out var installedVersion) &&
				VixenReleaseVersion.TryParse(release.TagName, out var latestVersion))
			{
				return new UpdateCheckResult(
					release.TagName!,
					null,
					latestVersion.CompareTo(installedVersion) > 0,
					request.IncludeReleaseNotes ? release.Body ?? string.Empty : null,
					release.HtmlUrl ?? FallbackReleasePageUri,
					release.PublishedAt);
			}

			return null;
		}

		internal static bool TryGetDevelopmentBuildNumber(string? tagName, out int buildNumber)
		{
			const string prefix = "DevBuild-";
			buildNumber = 0;
			return tagName?.StartsWith(prefix, StringComparison.Ordinal) == true &&
				int.TryParse(tagName.AsSpan(prefix.Length), out buildNumber) && buildNumber >= 0;
		}
	}
}
