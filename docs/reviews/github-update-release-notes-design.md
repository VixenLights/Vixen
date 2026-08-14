# GitHub update metadata and release-notes refactor

## Recommendation

Move remote update discovery out of `VersionInfo` and into an injectable GitHub-backed update
service. Return one result containing the latest tag/build, update availability, release body, and
release URL. The automatic startup check can ignore the body, while the Check for Updates dialog can
request and display it. One update-service operation then replaces the version lookup and all Jira
work in `PopulateChangeLog`; the common latest-release and development paths need one GitHub request.

Keep `VersionInfo` responsible for installed assembly version facts. It should not own HTTP,
GitHub JSON parsing, connectivity probing, update comparison, or release-note assembly.

For HTTP lifetime management, either of these approaches is valid:

1. Introduce a small Microsoft DI composition root in `Program` and use a named
   `IHttpClientFactory` client. This is the preferred long-term option if Vixen intends to use
   Microsoft DI for additional application services.
2. Inject a singleton `HttpClient` backed by `SocketsHttpHandler` with
   `PooledConnectionLifetime`. This is the lower-risk option if introducing a second DI container
   solely for this feature is not desirable.

Do not continue constructing and disposing a raw `HttpClient` per operation.

## Current behavior and findings

`VersionInfo.GetLatestBuildVersionAsync()` and `VersionInfo.GetLatestReleaseVersionAsync()` now query
GitHub, but each creates and disposes its own `HttpClient`. Each method also calls
`CheckForConnectionToWebsite()` first. That preflight still probes the old Jira host, creates another
short-lived client, and can reject an otherwise-valid GitHub request.

The manual update dialog then calls `PopulateChangeLog()`, which does more Jira work:

- Development builds make one JQL request for closed issues newer than the installed build.
- Release builds retrieve all Jira project versions and then issue one JQL request for every release
  newer than the installed release.
- `_newVersionAvailable` is set only while Jira issues are appended. An update with an empty release
  body could therefore be incorrectly treated as no update.

The GitHub release workflow already puts the Jira-generated `Release Notes.md` into the release
`body`. The observed formats align with the current UI needs:

- A development release such as `DevBuild-1501` has `prerelease: true`, contains the build number in
  `tag_name`, and has a cumulative body covering changes since the last full release.
- A full release such as `3.13` has `prerelease: false` and a body containing the categorized issues
  for that release.

The current development selection orders releases by `published_at`. That is not a safe definition
of the highest build. GitHub has releases whose publication order differs by a few seconds from build
number order. Parse valid `DevBuild-N` tags and choose the maximum `N` instead.

The Vixen release workflow guarantees at least one development build between full releases because a
development build is created before each full release. Therefore the existing bounded releases query
with `per_page=5` is sufficient to find the latest prerelease. Do not retrieve a large release list or
paginate through historical prereleases. Select the maximum valid `DevBuild-N` within that bounded
response.

The public XML comments on both lookup methods still describe Jira and, for the build method, describe
an empty-string failure despite the return type being `int`. Any implementation must update those
comments or remove the remote methods from `VersionInfo`.

## Release-note behavior decision

Use `GET /repos/VixenLights/Vixen/releases/latest`, returning both `tag_name` and `body`, and display
only the latest full release's notes. This is intentional even when an installed version has skipped
one or more full releases. The complete historical release notes installed with the application
remain available to users who need the intervening details.

Development builds require only the selected highest `DevBuild-N` body because that body is already
cumulative.

Do not enumerate older stable releases, aggregate multiple bodies, compare release dates, or request
releases by tag for changelog construction. Each release-channel check needs only the `/releases/latest`
response it already uses for the version. Each development-channel check uses the selected prerelease
object from its existing releases-list response. In both cases, version metadata and notes come from
the same GitHub response. The development request remains bounded to `per_page=5` and is never
paginated.

## API design options

### Option 1: Tuple-returning overloads

Add overloads that return `(Version, ReleaseNotes)` and retain the current scalar methods.

This is mechanically small, but the two methods need different version types, tuple members provide
a weak public contract, and failure/availability information remains implicit. It also leaves
networking and GitHub concerns in `VersionInfo`. This option is not recommended.

Async methods cannot use `out` or `ref` parameters. A callback used as a substitute would introduce
ordering and error-handling complexity. A static `LatestReleaseNotes` property would create stale
state and races between the startup check and the dialog. Neither should be used.

### Option 2: Result objects while retaining `VersionInfo`

Introduce new methods such as `GetLatestBuildAsync()` and `GetLatestReleaseAsync()` returning a
common immutable result. Keep the existing scalar methods as compatibility wrappers.

This provides a clean migration path, but `VersionInfo` remains responsible for local version facts
and remote provider operations. It is acceptable as an intermediate step.

If the caller must explicitly opt into notes, use an options record rather than another positional
boolean:

```csharp
internal sealed record UpdateCheckOptions
{
	public bool IncludeReleaseNotes { get; init; }
}
```

The GitHub API already returns `body`; `IncludeReleaseNotes` normally does not reduce response size.
It controls only whether notes are copied into the returned result. Cache the parsed GitHub response
so a startup version-only check can satisfy a subsequent dialog request without immediately repeating
the call.

### Option 3: Injectable update service

This is the recommended target. Suggested internal contracts are:

```csharp
internal enum UpdateChannel
{
	Release,
	Development
}

internal sealed record UpdateCheckRequest(
	UpdateChannel Channel,
	string InstalledVersion,
	bool IncludeReleaseNotes);

internal sealed record UpdateCheckResult(
	string LatestVersion,
	int? LatestBuildNumber,
	bool IsUpdateAvailable,
	string? ReleaseNotes,
	Uri ReleasePageUri,
	DateTimeOffset? PublishedAt);

internal interface IUpdateService
{
	Task<UpdateCheckResult?> CheckAsync(
		UpdateCheckRequest request,
		CancellationToken cancellationToken = default);
}
```

Use one type per file, matching repository conventions. Keep the contracts internal unless another
assembly has a demonstrated need for them. If they become public or protected, apply the project
`csharp-docs` skill and fully document them.

`GitHubUpdateService` should deserialize into a typed wire model instead of using `dynamic`. The
minimum useful GitHub fields are `tag_name`, `body`, `draft`, `prerelease`, `published_at`,
`html_url`, and optionally `assets` if download selection is moved into the service later.

The service should own these rules:

- Exclude drafts.
- Development: accept only prereleases with a valid `DevBuild-N` tag and choose the largest numeric
  build number from the bounded five-release response, not the newest timestamp. Do not paginate.
- Release: use the non-prerelease returned by `/releases/latest` and validate its Vixen release tag.
- Parse stable tags using a Vixen-specific comparison supporting `major.minor` and
  `major.minoruN`; do not compare release strings with `!=`.
- Set `IsUpdateAvailable` from semantic version/build comparison, independently of whether `body` is
  empty.
- Treat a missing body as empty notes, not as a failed version check.
- Normalize line endings for the WinForms text box at the presentation boundary.
- Return a distinguishable failure result or `null`; do not map network failure to build `0` or an
  empty version and then report that the installation is current.

If preserving the two existing public scalar methods is required, make them thin wrappers over this
service only during migration. Avoid resolving the service from a global service locator inside
`VersionInfo`; update the four known call sites instead.

## Caller flow

### Automatic startup check

`VixenApplication.PopulateVersionStringsAsync()` asks `IUpdateService` for the appropriate channel
with `IncludeReleaseNotes = false`. It uses only `IsUpdateAvailable` and the returned version/build
for the status strip.

### Manual Check for Updates dialog

`CheckForUpdates.CheckUpdates()` makes one service call with `IncludeReleaseNotes = true`. It then:

1. Sets `_currentVersion` and `_latestVersion` from local and returned metadata.
2. Sets `_newVersionAvailable` from `UpdateCheckResult.IsUpdateAvailable`.
3. Assigns `textBoxReleaseNotes.Text` from `UpdateCheckResult.ReleaseNotes`.
4. Shows or hides the update controls.
5. Displays an explicit unavailable/error state when the service result indicates failure.

`PopulateChangeLog()` should become a synchronous formatting/presentation helper or be removed. It
must not perform network I/O. Its current Jira parsing, task dictionary, `.Result` access after
`WhenAll`, and incremental `Text +=` construction all disappear.

`UpdatesMenu_Click()` should also stop calling `CheckForConnectionToWebsite()`. A preflight request
introduces a time-of-check/time-of-use race and currently checks the wrong host. Open the dialog and
let the actual GitHub operation report success or failure.

## `HttpClient` lifetime options

### Named `IHttpClientFactory` client

`Vixen.Application` already references `Microsoft.AspNetCore.App`, so the required
`Microsoft.Extensions.Http` and dependency-injection assemblies are present. The application does
not currently establish a Microsoft DI composition root; it constructs `VixenApplication` directly
and later registers two Catel services.

If adopting the factory, create and own a `ServiceProvider` in `Program` before constructing the main
form. Register a named client rather than capturing a typed client inside a long-lived singleton:

```csharp
services.AddHttpClient("GitHubReleases", client =>
{
	client.BaseAddress = new Uri("https://api.github.com/repos/VixenLights/Vixen/");
	client.Timeout = TimeSpan.FromSeconds(5);
	client.DefaultRequestHeaders.UserAgent.ParseAdd("Vixen-UpdateChecker/1.0");
	client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
	client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2026-03-10");
});

services.AddSingleton<IUpdateService, GitHubUpdateService>();
```

Inject `IHttpClientFactory` into the singleton service and call
`CreateClient("GitHubReleases")` for each operation. Factory-created `HttpClient` wrappers are
intended to be short-lived; the factory pools their underlying handlers, so disposing a created
client does not recreate the connection pool on every call.

Build the provider once, dispose it after `Application.Run`, and pass `IUpdateService` through
constructors to `VixenApplication` and `CheckForUpdates`. Do not build a provider per request. Do not
hide the provider in `VersionInfo`, and do not capture a transient typed client in the application
singleton.

If WinForms designer support requires retaining a parameterless form constructor, keep the injected
constructor as the runtime path and isolate the designer fallback. Do not silently construct a second
production service graph from the form constructor.

### Shared `HttpClient`

If a new Microsoft DI container is too much scope, create one application-lifetime client with a
`SocketsHttpHandler`:

```csharp
var handler = new SocketsHttpHandler
{
	PooledConnectionLifetime = TimeSpan.FromMinutes(10)
};

var client = new HttpClient(handler)
{
	BaseAddress = new Uri("https://api.github.com/repos/VixenLights/Vixen/")
};
```

Configure the same headers once, inject that client into `GitHubUpdateService`, and dispose it only
when the application exits. This solves connection-pool churn and allows periodic DNS refresh without
introducing another container. It is a good fit for Vixen's low-frequency, single-host requests.

### Adjacent clients

Removing Jira from `PopulateChangeLog()` eliminates its short-lived clients. Removing the connectivity
preflight eliminates another. `CheckForUpdates.DownloadFile()` still creates a raw client and buffers
the entire installer with `GetByteArrayAsync`. If brought into scope, use a separate named download
client with a much longer timeout and stream the response to disk. Do not reuse the five-second API
client configuration for installer downloads.

`Common.WPFCommon.Services.DownloadService` also creates clients per call, but it is broader than this
refactor and should be handled separately unless the team chooses an application-wide HTTP-client
migration.

## Error handling and caching

- Pass a `CancellationToken` through all update-service methods. The dialog can cancel when it closes.
- Handle HTTP failure, timeout/cancellation, GitHub rate limiting, and malformed JSON distinctly in
  logs. Use structured NLog calls rather than concatenated exception strings.
- Avoid automatic retry storms. An update check is optional; one bounded retry for transient server
  failures is sufficient if resilience is added later, and `403`, `404`, and most `429` responses
  should not be retried immediately.
- Cache the last successful parsed release response in memory for a short interval. This avoids a
  startup check followed by a manual dialog consuming another unauthenticated request. Retain the body
  in the cache even if the first caller did not request notes because GitHub already sent it.
- ETag/conditional requests can be a later enhancement if rate limits become observable in practice.

## Validation

Add focused tests using a fake `HttpMessageHandler` or a fake `IUpdateService`:

- Development selection ignores stable releases, drafts, malformed tags, and timestamps that are out
  of build-number order, then selects the maximum valid `DevBuild-N`.
- Development selection uses one `per_page=5` request and performs no pagination when the most recent
  item is a full release.
- Stable parsing orders `3.13`, `3.13u1`, and later versions correctly.
- A missing or empty body does not suppress `IsUpdateAvailable`.
- `IncludeReleaseNotes = false` returns correct version metadata without exposing notes.
- `IncludeReleaseNotes = true` returns the exact GitHub body with normalized line endings.
- An installation that skipped stable releases receives only the latest stable release body.
- Timeout, non-success status, rate-limit response, and malformed JSON produce the defined failure
  state and do not report "latest installed."
- The manual dialog performs one update-service operation and `PopulateChangeLog` performs no HTTP.
- No production update-check path contains a Jira REST URL.

Because `Vixen.Tests` does not currently reference `Vixen.Application`, implementation must either add
that project reference plus `InternalsVisibleTo`, place the pure parsing/comparison code in an already
testable appropriate assembly, or create an equivalently focused test seam without making application
implementation types public merely for testing.

## Suggested implementation sequence

1. Add typed GitHub DTOs, Vixen tag parsers/comparers, and the update result contract with focused
   tests.
2. Add `IUpdateService` and `GitHubUpdateService` using either the named factory client or injected
   shared client.
3. Migrate the two automatic status checks to the service without requesting notes.
4. Migrate the manual dialog to one result-bearing service call and display the returned body.
5. Remove Jira logic from `PopulateChangeLog`, remove the connectivity preflight, and remove obsolete
   remote methods from `VersionInfo` or retain only documented compatibility wrappers.
6. Verify release, development, test-build, no-update, empty-body, offline, and skipped-release paths;
   the skipped-release case must display only the latest stable release body.
7. Optionally migrate installer download streaming as a separate, clearly scoped change.

## References

- [GitHub REST API endpoints for releases](https://docs.github.com/en/rest/releases/releases)
- [.NET `HttpClient` guidelines](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)
- [.NET `IHttpClientFactory` guidance](https://learn.microsoft.com/en-us/dotnet/core/extensions/httpclient-factory)
