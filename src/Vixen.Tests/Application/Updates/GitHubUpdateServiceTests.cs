using System.Net;
using VixenApplication.Updates;
using Xunit;

namespace Vixen.Tests.Application.Updates;

public sealed class GitHubUpdateServiceTests
{
	[Fact]
	public async Task CheckAsync_Development_SelectsHighestValidNonDraftBuild()
	{
		// Arrange
		var handler = CreateHandler("""
		[
		  { "tag_name": "DevBuild-1500", "prerelease": true, "draft": false, "published_at": "2026-01-03T00:00:00Z", "body": "older build", "html_url": "https://example.test/1500" },
		  { "tag_name": "DevBuild-1502", "prerelease": true, "draft": false, "published_at": "2026-01-01T00:00:00Z", "body": "chosen", "html_url": "https://example.test/1502", "assets": [{ "name": "Vixen-DevBuild-0.0.1502-Setup-64bit.exe", "browser_download_url": "https://github.com/VixenLights/Vixen/releases/download/DevBuild-1502/Vixen-DevBuild-0.0.1502-Setup-64bit.exe" }] },
		  { "tag_name": "DevBuild-invalid", "prerelease": true, "draft": false },
		  { "tag_name": "DevBuild-9999", "prerelease": true, "draft": true },
		  { "tag_name": "3.13", "prerelease": false, "draft": false }
		]
		""");
		using var client = CreateClient(handler);
		var service = new GitHubUpdateService(client);

		// Act
		var result = await service.CheckAsync(new UpdateCheckRequest(UpdateChannel.Development, "1501", true), TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("1502", result.LatestVersion);
		Assert.Equal(1502, result.LatestBuildNumber);
		Assert.True(result.IsUpdateAvailable);
		Assert.Equal("chosen", result.ReleaseNotes);
		Assert.Equal("https://github.com/VixenLights/Vixen/releases/download/DevBuild-1502/Vixen-DevBuild-0.0.1502-Setup-64bit.exe", result.InstallerDownloadUri?.ToString());
		Assert.Single(handler.RequestUris);
		Assert.Equal("/repos/VixenLights/Vixen/releases?per_page=5", handler.RequestUris[0].PathAndQuery);
	}

	[Fact]
	public async Task CheckAsync_Release_UsesSemanticStableVersionAndLatestBodyOnly()
	{
		// Arrange
		var handler = CreateHandler("""
		{ "tag_name": "3.13u1", "prerelease": false, "draft": false, "body": "latest notes", "html_url": "https://example.test/3.13u1", "assets": [{ "name": "Vixen-3.13.1-Setup-64bit.exe", "browser_download_url": "https://github.com/VixenLights/Vixen/releases/download/3.13u1/Vixen-3.13.1-Setup-64bit.exe" }] }
		""");
		using var client = CreateClient(handler);
		var service = new GitHubUpdateService(client);

		// Act
		var result = await service.CheckAsync(new UpdateCheckRequest(UpdateChannel.Release, "3.13", true), TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsUpdateAvailable);
		Assert.Equal("3.13u1", result.LatestVersion);
		Assert.Equal("latest notes", result.ReleaseNotes);
		Assert.Equal("https://github.com/VixenLights/Vixen/releases/download/3.13u1/Vixen-3.13.1-Setup-64bit.exe", result.InstallerDownloadUri?.ToString());
		Assert.Single(handler.RequestUris);
		Assert.Equal("/repos/VixenLights/Vixen/releases/latest", handler.RequestUris[0].PathAndQuery);
	}

	[Fact]
	public async Task CheckAsync_ReleaseWithEmptyBody_ReportsAvailableWithoutExposingNotes()
	{
		// Arrange
		var handler = CreateHandler("""
		{ "tag_name": "3.14", "prerelease": false, "draft": false, "body": null, "html_url": "https://example.test/3.14" }
		""");
		using var client = CreateClient(handler);
		var service = new GitHubUpdateService(client);

		// Act
		var result = await service.CheckAsync(new UpdateCheckRequest(UpdateChannel.Release, "3.13u1", false), TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.True(result.IsUpdateAvailable);
		Assert.Null(result.ReleaseNotes);
	}

	[Fact]
	public async Task CheckAsync_SecondRequestUsesCachedBodyWhenNotesAreRequested()
	{
		// Arrange
		var handler = CreateHandler("""
		{ "tag_name": "3.14", "prerelease": false, "draft": false, "body": "cached notes", "html_url": "https://example.test/3.14" }
		""");
		using var client = CreateClient(handler);
		var service = new GitHubUpdateService(client);

		// Act
		var startupResult = await service.CheckAsync(new UpdateCheckRequest(UpdateChannel.Release, "3.13", false), TestContext.Current.CancellationToken);
		var dialogResult = await service.CheckAsync(new UpdateCheckRequest(UpdateChannel.Release, "3.13", true), TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(startupResult);
		Assert.NotNull(dialogResult);
		Assert.Null(startupResult.ReleaseNotes);
		Assert.Equal("cached notes", dialogResult.ReleaseNotes);
		Assert.Single(handler.RequestUris);
	}

	[Fact]
	public async Task GetReleaseNotesAsync_RunningRelease_UsesExactTagAndReturnsGitHubBody()
	{
		// Arrange
		var handler = CreateHandler("""
		{ "tag_name": "3.13u1", "prerelease": false, "draft": false, "body": "GitHub notes\r\nfor 3.13u1", "html_url": "https://example.test/3.13u1" }
		""");
		using var client = CreateClient(handler);
		var service = new GitHubUpdateService(client);

		// Act
		var result = await service.GetReleaseNotesAsync("3.13u1", TestContext.Current.CancellationToken);

		// Assert
		Assert.NotNull(result);
		Assert.Equal("3.13u1", result.ReleaseTag);
		Assert.Equal("GitHub notes\r\nfor 3.13u1", result.ReleaseNotes);
		Assert.Single(handler.RequestUris);
		Assert.Equal("/repos/VixenLights/Vixen/releases/tags/3.13u1", handler.RequestUris[0].PathAndQuery);
	}

	[Theory]
	[InlineData("3.13", "3.13u1", -1)]
	[InlineData("3.13u1", "3.14", -1)]
	[InlineData("3.14", "3.13u9", 1)]
	public void VixenReleaseVersion_CompareTo_OrdersStableTags(string first, string second, int expectedSign)
	{
		// Arrange
		Assert.True(VixenReleaseVersion.TryParse(first, out var firstVersion));
		Assert.True(VixenReleaseVersion.TryParse(second, out var secondVersion));

		// Act
		var comparison = firstVersion.CompareTo(secondVersion);

		// Assert
		Assert.Equal(expectedSign, Math.Sign(comparison));
	}

	[Theory]
	[InlineData(HttpStatusCode.Forbidden)]
	[InlineData(HttpStatusCode.TooManyRequests)]
	[InlineData(HttpStatusCode.InternalServerError)]
	public async Task CheckAsync_HttpFailure_ReturnsNull(HttpStatusCode statusCode)
	{
		// Arrange
		var handler = new RecordingHttpMessageHandler(_ => new HttpResponseMessage(statusCode));
		using var client = CreateClient(handler);
		var service = new GitHubUpdateService(client);

		// Act
		var result = await service.CheckAsync(new UpdateCheckRequest(UpdateChannel.Release, "3.13", false), TestContext.Current.CancellationToken);

		// Assert
		Assert.Null(result);
	}

	[Fact]
	public async Task CheckAsync_MalformedJson_ReturnsNull()
	{
		// Arrange
		var handler = CreateHandler("not JSON");
		using var client = CreateClient(handler);
		var service = new GitHubUpdateService(client);

		// Act
		var result = await service.CheckAsync(new UpdateCheckRequest(UpdateChannel.Release, "3.13", false), TestContext.Current.CancellationToken);

		// Assert
		Assert.Null(result);
	}

	private static HttpClient CreateClient(HttpMessageHandler handler)
		=> new(handler) { BaseAddress = new Uri("https://api.github.com/repos/VixenLights/Vixen/") };

	private static RecordingHttpMessageHandler CreateHandler(string content)
		=> new(_ => new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(content) });
}
