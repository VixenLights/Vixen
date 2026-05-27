# FPP REST API Client Module

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`,
`Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.
Maintain this document in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

Vixen currently has no way to talk to a Falcon Player (FPP) device over its REST API. After
this change, any Vixen module can acquire an `IFppClient` instance pointed at an FPP host and
call methods to upload sequences, upload audio and video files, query playlists, and retrieve
system information — all without knowing any HTTP details. The client is delivered as a
self-contained Vixen App module so Vixen's plugin loader picks it up automatically, yet it
is fully usable as a plain C# library without the Vixen module system.

A developer can verify the feature is working by running the test suite (described in the
Validation section) and seeing all tests pass. A user of the library verifies it by pointing
it at a live FPP instance and calling `GetSystemInfoAsync` — they should receive back the
host's name, platform, and version string.

## Progress

- [x] (2026-05-26) Milestone 1 — Create JIRA issue covering the full feature (VIX-3921)
- [x] (2026-05-26) Milestone 2 — Project scaffolding: create the csproj, module boilerplate, add to solution
- [x] (2026-05-26) Milestone 3 — Interfaces and models: define `IFppClient`, configuration record, and all response model records
- [x] (2026-05-27) Milestone 4 — Client implementation: `FppClient` class with all required methods
- [x] (2026-05-27) Milestone 5 — Test suite: add FPPClient tests to `Vixen.Tests`

## Surprises & Discoveries

- Observation: `dotnet sln add` created duplicate solution folder hierarchy entries
  (`src`, `Vixen.Modules`, `App`) instead of nesting the project under the existing
  `App` solution folder that other App modules live in. The `NestedProjects` section
  pointed FPPClient into these new folders rather than the existing one.
  Evidence: Three extra `Project` entries with type GUID `{2150E333...}` (Solution Folder)
  appeared in the sln; corrected manually by removing the spurious folder entries and
  updating the `NestedProjects` entry to reference the pre-existing App folder GUID
  `{18C30343-DC00-48B0-B712-93CC89D20643}`.
  Resolution: After `dotnet sln add`, always verify the `NestedProjects` section and
  remove any auto-generated solution folder entries that duplicate existing folders.

- Observation: `FppClientFactory` (Milestone 3) references `FppClient` (Milestone 4), so the
  project would not compile at the end of Milestone 3 without a stub.
  Resolution: Created a stub `Client/FppClient.cs` with `NotImplementedException` bodies during
  Milestone 3. Milestone 4 replaces those bodies with the real implementation.

- Observation: `FPPClient.csproj` was missing `<Nullable>enable</Nullable>` and
  `<ImplicitUsings>enable</ImplicitUsings>`, causing CS8632 warnings on nullable annotations.
  Resolution: Added both properties to the `<PropertyGroup>` in the csproj.

- Observation: `FppPlaylistInfo` fields `total_duration` and `total_items` are snake_case in
  the FPP API response. `PropertyNameCaseInsensitive = true` handles case differences only;
  it does not bridge underscores (so `total_duration` would not match `TotalDuration`).
  Resolution: Added `[JsonPropertyName("total_duration")]` and `[JsonPropertyName("total_items")]`
  attributes directly on the `FppPlaylistInfo` record properties.

## Decision Log

- Decision: Use `System.Text.Json` (built into .NET 10) rather than `Newtonsoft.Json`.
  Rationale: No additional NuGet entry is needed; it is the modern standard for new code.
  The existing project uses `Newtonsoft.Json` only in legacy paths. New code should not
  grow that dependency further.
  Date/Author: 2026-05-26 / Planning

- Decision: Accept `HttpClient` directly in `FppClient`'s constructor rather than using
  `IHttpClientFactory`.
  Rationale: Vixen's plugin system does not wire up `Microsoft.Extensions.DependencyInjection`.
  Injecting `HttpClient` directly is simpler, keeps the module self-contained, and is fully
  testable via a mock `HttpMessageHandler`.
  Date/Author: 2026-05-26 / Planning

- Decision: Keep upload via `POST file/:DirName/:Filename` (direct streaming) for MVP; do not
  implement the PATCH chunked upload in the first version.
  Rationale: `StreamContent` streams directly from the caller's `Stream` without buffering in
  memory, so large files are handled safely. The PATCH chunked path is more complex and is
  not required by the stated use cases. A risk note calls this out in case FSEQ files are so
  large that the server rejects a single-request body.
  Date/Author: 2026-05-26 / Planning

- Decision: `GetPlaylistSequencesAsync` extracts sequence filenames by walking every entry in
  `leadIn`, `mainPlaylist`, and `leadOut` and returning `sequenceName` from entries whose
  `type` is `"sequence"` or `"both"`.
  Rationale: The requirement says "get the list of sequences in a playlist." A playlist entry
  can reference a sequence as its sole content (`sequence`) or paired with media (`both`).
  Both must be included.
  Date/Author: 2026-05-26 / Planning

- Decision: JIRA issue creation is Milestone 1, before any implementation work.
  Rationale: All implementation plans must start with a JIRA issue so every commit is
  traceable and the work is visible in the project tracker from day one.
  Date/Author: 2026-05-26 / Jeff Uchitjil
  JIRA issue key: VIX-3921 (https://vixenlights.atlassian.net/browse/VIX-3921)

- Decision: Place the module under `src/Vixen.Modules/App/FPPClient/` with namespace
  `VixenModules.App.FPPClient`.
  Rationale: Matches the pattern of every other App module in the solution (WebServer,
  ColorGradients, Shows, etc.) and satisfies the requirements.
  Date/Author: 2026-05-26 / Planning

- Decision: Tests live in the existing `src/Vixen.Tests` project under a new `FPPClient/`
  subfolder rather than in a new project.
  Rationale: The existing `Vixen.Tests` project is already wired to the solution and CI.
  Creating a second test project would require additional solution plumbing with no benefit.
  Date/Author: 2026-05-26 / Planning

## Outcomes & Retrospective

All five milestones completed 2026-05-26 / 2026-05-27.

The module builds cleanly and 31 unit tests pass (16 FPPClient, 15 pre-existing).
The client has been manually verified against a live FPP instance via the factory.

Key lessons:
- `dotnet sln add` generates spurious solution folder hierarchy — always fix `NestedProjects` manually.
- `PropertyNameCaseInsensitive` does not bridge underscore differences; `[JsonPropertyName]` is needed for snake_case fields.
- xUnit v3 `Assert.ThrowsAsync<T>` is an exact-type match; `ArgumentException.ThrowIfNullOrWhiteSpace` throws `ArgumentNullException` for null inputs, so assert `ArgumentNullException` specifically.
- `FppClientFactory` depended on `FppClient` (Milestone 4) but was defined in Milestone 3; a stub class was needed to keep the project compiling between milestones.

## Risks and Concerns

**Large file uploads.** FSEQ sequence files can exceed 100 MB. The chosen `POST
file/:DirName/:Filename` endpoint documentation notes it "may not support chunked
Transfer-Encoding." Using `StreamContent` avoids client-side memory pressure, but the FPP
HTTP server itself may impose a body size limit. If uploads of very large files fail with
HTTP 413 or a connection reset, the implementer should add the PATCH chunked upload
(`PATCH api/file/:DirName`) as an alternative code path. That endpoint uses `Upload-Length`,
`Upload-Name`, and `Upload-Offset` headers and requires a follow-up `GET file/move/:Filename`
call to move the file to its final directory.

**Mixed-case JSON field names.** FPP's API inconsistently uses `"Status"` (PascalCase) in
some endpoints and `"status"` (camelCase) in others. Deserialisation must set
`PropertyNameCaseInsensitive = true` on `JsonSerializerOptions`. Without this, fields will
silently deserialise as null.

**No authentication by default.** FPP ships with authentication disabled. The client is
designed so future authentication can be added by setting an `Authorization` header on the
underlying `HttpClient` before passing it to `FppClient`. The current design must not close
off that path (e.g., by creating `HttpClient` internally and hiding it).

**Upload timeout.** The default `HttpClient.Timeout` is 100 seconds, which is insufficient
for large file uploads. The `FppClientOptions` record exposes a separate `UploadTimeout`
that is applied only to upload calls by creating a new `CancellationTokenSource` that links
to the caller's token with the upload deadline. The factory sets a sensible default of 10
minutes.

**Platform.** Vixen targets `net10.0-windows`. The FPP client code must not reference any
Windows-only types so it can in principle be reused cross-platform later, but the csproj
may target `net10.0-windows` for consistency with the solution.

## Context and Orientation

Vixen is a WPF desktop application (.NET 10, x64/x86) for designing and running animated
light shows. Its plugin system loads modules from subdirectories of the output folder. Each
module is a `.dll` whose assembly name begins with `Module.` — this prefix is how Vixen
identifies plugin assemblies.

An **App module** (the type used here) is the simplest module kind. It requires three
boilerplate files:

- `Descriptor.cs` — a class inheriting `AppModuleDescriptorBase` that declares a stable
  GUID TypeId, a human-readable name, author, version, and the two runtime types
  (`ModuleClass` and `ModuleStaticDataClass`).
- `Data.cs` — a class inheriting `ModuleDataModelBase` decorated with `[DataContract]`;
  holds serialised settings for the module.
- `Module.cs` — a class inheriting `AppModuleInstanceBase`; called by Vixen on `Loading()`
  and `Unloading()`.

For this module the `Module.cs` does nothing on load because the client has no background
service. Consumers obtain an `IFppClient` by calling the static `FppClientFactory.Create`
method directly; no Vixen service locator is involved.

The FPP REST API base URL is `http://<host>/api/`. All endpoints return JSON. The common
success envelope is `{ "Status": "OK", "Message": "", "respCode": 200, ... }`. Some simpler
endpoints use lowercase `{ "status": "OK" }`. The full API reference is at
`docs/references/fpp-rest-api.md`.

The existing test project lives at `src/Vixen.Tests/Vixen.Tests.csproj`. It uses
`xunit.v3`, `Moq`, and `Microsoft.NET.Test.Sdk` — all already present in
`Directory.Packages.props`. It currently references only `Vixen.Core`; a project reference
to the new FPPClient project must be added.

## Plan of Work

### Milestone 1 — Create JIRA Issue

Before any code is written, create a JIRA issue in the `VIX` project to track all work in
this plan. Using the Atlassian MCP tools available in the session
(`mcp__atlassian__createJiraIssue`), create one `New Feature` issue with:

- **Project key:** `VIX`
- **Issue type:** `New Feature`
- **Summary:** `Add FPP REST API client module`
- **Description:** the full scope, milestone breakdown, acceptance criteria, and risks from
  this plan written in JIRA-friendly Markdown

Record the assigned issue key (e.g. `VIX-XXXX`) in the Decision Log. Prefix every git
commit message made during this work with that key so changes are traceable.

**Acceptance:** A JIRA issue exists in the `VIX` project with the key recorded in the
Decision Log before any implementation begins.

### Milestone 2 — Project Scaffolding

Create the directory `src/Vixen.Modules/App/FPPClient/` and within it:

1. `FPPClient.csproj` — mirrors `WebServer.csproj` in structure but with no `AspNetCore`
   framework reference, no `wwwroot`, and minimal dependencies (only `NLog` and
   `Vixen.Core`). Set `AssemblyName` to `Module.App.FPPClient` and `RootNamespace` to
   `VixenModules.App.FPPClient`. Target `net10.0-windows`. Enable `Nullable` and
   `ImplicitUsings`.

2. `Descriptor.cs` — implements `AppModuleDescriptorBase`. Choose a new GUID for TypeId
   (generate one; do not reuse any existing GUID from the repository). Set `TypeName` to
   `"FPP Client"`, `Author` to `"Vixen Team"`, `Version` to `"1.0"`, `ModuleClass` to
   `typeof(Module)`, `ModuleStaticDataClass` to `typeof(Data)`.

3. `Data.cs` — minimal `[DataContract]` class with no fields for now. The `Clone()`
   implementation uses `MemberwiseClone()`.

4. `Module.cs` — minimal `AppModuleInstanceBase` with empty `Loading()` and `Unloading()`
   overrides and a `StaticModuleData` property backed by a `Data` field.

Add the new project to `Vixen.sln` using `dotnet sln add`, then open the solution file and
correct the `Any CPU` platform entries to point to `x64` targets, as described in
`CLAUDE.md` under "Adding Projects to the Solution."

**Acceptance:** `msbuild Vixen.sln -m -t:Restore,Rebuild -p:Configuration=Debug` completes
with no errors and produces `Debug\Output\Module.App.FPPClient.dll`.

### Milestone 3 — Interfaces and Models

Create the following files within `src/Vixen.Modules/App/FPPClient/`:

**`Client/IFppClient.cs`** — the public contract that consumers depend on. Declare it
`public interface IFppClient : IAsyncDisposable`. Required members:

    Task<FppSystemInfo> GetSystemInfoAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetSequencesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetPlaylistsAsync(CancellationToken cancellationToken = default);
    Task<FppPlaylist> GetPlaylistAsync(string playlistName, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<string>> GetPlaylistSequencesAsync(string playlistName, CancellationToken cancellationToken = default);
    Task UploadFileAsync(string dirName, string filename, Stream content, CancellationToken cancellationToken = default);
    Task UploadSequenceAsync(string filename, Stream content, CancellationToken cancellationToken = default);
    Task UploadMusicAsync(string filename, Stream content, CancellationToken cancellationToken = default);
    Task UploadVideoAsync(string filename, Stream content, CancellationToken cancellationToken = default);

**`Client/FppClientOptions.cs`** — a `public sealed record`:

    public sealed record FppClientOptions
    {
        public required string BaseUrl { get; init; }
        public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);
        public TimeSpan UploadTimeout { get; init; } = TimeSpan.FromMinutes(10);
    }

`BaseUrl` must end with a trailing slash when passed to `HttpClient.BaseAddress` so that
relative paths resolve correctly. Validate and normalise this in the `FppClient` constructor.

**`Client/IFppClientFactory.cs`** — a simple factory interface:

    public interface IFppClientFactory
    {
        IFppClient Create(FppClientOptions options);
    }

**`Client/FppClientFactory.cs`** — sealed implementation:

    public sealed class FppClientFactory : IFppClientFactory
    {
        public IFppClient Create(FppClientOptions options) =>
            new FppClient(new HttpClient(), options);
    }

**Models** — one file per record in `Models/`:

- `Models/FppSystemInfo.cs` — maps `system/info` response:

      public sealed record FppSystemInfo
      {
          public string HostName { get; init; } = string.Empty;
          public string HostDescription { get; init; } = string.Empty;
          public string Platform { get; init; } = string.Empty;
          public string Variant { get; init; } = string.Empty;
          public string Mode { get; init; } = string.Empty;
          public string Version { get; init; } = string.Empty;
          public string Branch { get; init; } = string.Empty;
          public string OSVersion { get; init; } = string.Empty;
          public string OSRelease { get; init; } = string.Empty;
          public string ChannelRanges { get; init; } = string.Empty;
          public int MajorVersion { get; init; }
          public int MinorVersion { get; init; }
          public int TypeId { get; init; }
          public string Uuid { get; init; } = string.Empty;
          public FppUtilization? Utilization { get; init; }
          public string[] IPs { get; init; } = [];
      }

- `Models/FppUtilization.cs`:

      public sealed record FppUtilization
      {
          public double CPU { get; init; }
          public double Memory { get; init; }
          public string Uptime { get; init; } = string.Empty;
      }

- `Models/FppPlaylist.cs`:

      public sealed record FppPlaylist
      {
          public string Name { get; init; } = string.Empty;
          public FppPlaylistEntry[] LeadIn { get; init; } = [];
          public FppPlaylistEntry[] MainPlaylist { get; init; } = [];
          public FppPlaylistEntry[] LeadOut { get; init; } = [];
          public FppPlaylistInfo? PlaylistInfo { get; init; }
      }

- `Models/FppPlaylistEntry.cs`:

      public sealed record FppPlaylistEntry
      {
          public string Type { get; init; } = string.Empty;
          public int Enabled { get; init; }
          public int PlayOnce { get; init; }
          public string? SequenceName { get; init; }
          public string? MediaName { get; init; }
          public string? Note { get; init; }
      }

- `Models/FppPlaylistInfo.cs`:

      public sealed record FppPlaylistInfo
      {
          public double TotalDuration { get; init; }
          public int TotalItems { get; init; }
      }

- `Models/FppUploadResult.cs` — maps the upload response envelope:

      internal sealed record FppUploadResult
      {
          public string Status { get; init; } = string.Empty;
          public string? File { get; init; }
          public string? Dir { get; init; }
      }

**`Exceptions/FppClientException.cs`** — thrown when FPP returns a non-success status or
the HTTP response is not 2xx:

    public sealed class FppClientException : Exception
    {
        public int? HttpStatusCode { get; }
        public FppClientException(string message) : base(message) { }
        public FppClientException(string message, int httpStatusCode)
            : base(message) { HttpStatusCode = httpStatusCode; }
        public FppClientException(string message, Exception inner)
            : base(message, inner) { }
    }

**Acceptance:** The project compiles cleanly. All types are present. No implementation is
required yet.

### Milestone 4 — Client Implementation

Create `Client/FppClient.cs` — `internal sealed class FppClient : IFppClient`.

**Constructor.** Accept `HttpClient httpClient` and `FppClientOptions options`. Store both.
Normalise `options.BaseUrl`: if it does not end with `/`, append one. Set
`httpClient.BaseAddress = new Uri(options.BaseUrl)`. Set
`httpClient.Timeout = options.Timeout`. Configure a static `JsonSerializerOptions` field:

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

**`GetSystemInfoAsync`.** Issue `GET system/info`. Deserialise the response body as
`FppSystemInfo`. If the HTTP response is not 2xx, throw `FppClientException` with the
status code. Use `ConfigureAwait(false)` on every `await`. Use structured NLog logging
(no string interpolation) at `Debug` level for the request and at `Error` level for failures.

**`GetSequencesAsync`.** Issue `GET sequence`. The response is a JSON array of strings.
Deserialise as `string[]` and return as `IReadOnlyList<string>`. If the array is null (FPP
returned an unexpected shape), return an empty list.

**`GetPlaylistsAsync`.** Issue `GET playlists`. Response is a JSON array of strings.
Same pattern as `GetSequencesAsync`.

**`GetPlaylistAsync`.** Issue `GET playlist/{Uri.EscapeDataString(playlistName)}`.
Deserialise as `FppPlaylist`. Throw `FppClientException` if the response is not 2xx.

**`GetPlaylistSequencesAsync`.** Call `GetPlaylistAsync` internally. Walk the `LeadIn`,
`MainPlaylist`, and `LeadOut` arrays. Collect `SequenceName` from any entry whose `Type`
is `"sequence"` or `"both"` and whose `SequenceName` is not null or whitespace. Return the
collected names as `IReadOnlyList<string>`.

**`UploadFileAsync`.** Validate `dirName` and `filename` are not null or whitespace; throw
`ArgumentException` otherwise. Build the relative URI
`file/{Uri.EscapeDataString(dirName)}/{Uri.EscapeDataString(filename)}`. Create a
`StreamContent` from `content`. Set `Content-Type` to
`application/octet-stream`. Use a `CancellationTokenSource` linked to the caller's
`cancellationToken` with a deadline of `options.UploadTimeout` to avoid hanging on large
uploads. Issue the POST and check for 2xx. If the response body can be parsed as
`FppUploadResult` and `Status` is not `"OK"`, log a warning but do not throw (FPP sometimes
returns 200 with a descriptive body on partial failures). If the HTTP status is not 2xx,
throw `FppClientException`.

**`UploadSequenceAsync`.** Calls `UploadFileAsync("sequences", filename, content,
cancellationToken)`.

**`UploadMusicAsync`.** Calls `UploadFileAsync("music", filename, content,
cancellationToken)`.

**`UploadVideoAsync`.** Calls `UploadFileAsync("videos", filename, content,
cancellationToken)`.

**`DisposeAsync`.** Dispose the `HttpClient`.

**`SendGetAsync<T>` helper.** Extract the repeated GET + deserialise + error-check pattern
into a private `Task<T> SendGetAsync<T>(string relativeUrl, CancellationToken ct)` method.

**Acceptance:** The project compiles. The NLog logger is initialised with
`LogManager.GetCurrentClassLogger()` and all public methods are covered by the interface.

### Milestone 5 — Test Suite

Add a project reference to the new `FPPClient` project in
`src/Vixen.Tests/Vixen.Tests.csproj`:

    <ProjectReference Include="..\Vixen.Modules\App\FPPClient\FPPClient.csproj" />

Create `src/Vixen.Tests/FPPClient/` and add the following test files. Each uses
the AAA (Arrange / Act / Assert) pattern from xUnit v3.

**`FPPClient/Helpers/MockHttpMessageHandler.cs`** — a helper that captures outgoing
requests and returns pre-configured responses. This avoids taking a live network dependency
in tests:

    internal sealed class MockHttpMessageHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, HttpResponseMessage> _handler;
        public MockHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> handler)
            => _handler = handler;
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(_handler(request));
    }

A factory helper `CreateClient(handler)` wraps the handler in an `HttpClient`, passes it
to `FppClient` with `new FppClientOptions { BaseUrl = "http://fpp.test/" }`, and returns
the client.

**`FPPClient/FppClientGetSystemInfoTests.cs`** — tests for `GetSystemInfoAsync`:
- `GetSystemInfoAsync_Success_ReturnsSystemInfo` — mock returns HTTP 200 with a valid JSON
  `system/info` body; assert `HostName`, `Platform`, `Version`, `IPs` are mapped correctly.
- `GetSystemInfoAsync_HttpError_ThrowsFppClientException` — mock returns HTTP 503; assert
  `FppClientException` is thrown with the status code populated.
- `GetSystemInfoAsync_MalformedJson_ThrowsException` — mock returns HTTP 200 with
  `"not json"` body; assert an exception is thrown (either `JsonException` or
  `FppClientException`).

**`FPPClient/FppClientGetSequencesTests.cs`** — tests for `GetSequencesAsync`:
- `GetSequencesAsync_Success_ReturnsNames` — mock returns `["Seq1","Seq2"]`; assert two
  items are returned with correct values.
- `GetSequencesAsync_EmptyArray_ReturnsEmpty` — mock returns `[]`.
- `GetSequencesAsync_HttpError_ThrowsFppClientException`.

**`FPPClient/FppClientGetPlaylistsTests.cs`** — mirrors `GetSequencesAsync` tests.

**`FPPClient/FppClientGetPlaylistTests.cs`** — tests for `GetPlaylistAsync` and
`GetPlaylistSequencesAsync`:
- `GetPlaylistAsync_Success_MapsAllSections` — mock returns a playlist with one entry in
  each section; assert `Name`, `MainPlaylist.Length`, entry `Type` and `SequenceName`.
- `GetPlaylistSequencesAsync_ReturnsOnlySequenceEntries` — playlist has a `sequence` entry,
  a `both` entry, and a `pause` entry; assert only the two sequence names are returned.
- `GetPlaylistSequencesAsync_EmptyPlaylist_ReturnsEmpty`.
- `GetPlaylistAsync_NotFound_ThrowsFppClientException` — mock returns HTTP 404.

**`FPPClient/FppClientUploadTests.cs`** — tests for upload methods:
- `UploadSequenceAsync_Success_PostsToCorrectUrl` — assert the request URI is
  `file/sequences/test.fseq` and method is POST.
- `UploadMusicAsync_Success_PostsToCorrectUrl` — assert URI is `file/music/song.mp3`.
- `UploadVideoAsync_Success_PostsToCorrectUrl` — assert URI is `file/videos/show.mp4`.
- `UploadFileAsync_NullDirName_ThrowsArgumentException`.
- `UploadFileAsync_NullFilename_ThrowsArgumentException`.
- `UploadFileAsync_HttpError_ThrowsFppClientException` — mock returns HTTP 500.

**Acceptance:** Run:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj --logger "console;verbosity=normal"

All tests pass. The new FPPClient tests appear in the output. No skipped or failed tests
are acceptable before declaring this milestone done.

## Concrete Steps

Run all commands from the repository root `C:\Dev\Vixen` unless stated otherwise.

**Step 1 — Create JIRA issue** (Milestone 1)

Use `mcp__atlassian__createJiraIssue` as described in Milestone 1. Record the issue key
before proceeding.

**Step 2 — Create the project directory and files** (Milestone 2)

    mkdir src\Vixen.Modules\App\FPPClient

Create the files as described in Milestones 2 and 3 (see file list in Interfaces and
Dependencies section).

**Step 3 — Add to solution and fix platform entries** (Milestone 2)

    dotnet sln Vixen.sln add src\Vixen.Modules\App\FPPClient\FPPClient.csproj

Open `Vixen.sln` in a text editor, find the newly-added project GUID in the
`GlobalSection(ProjectConfigurationPlatforms)` block, and replace all three `Any CPU`
entries with `x64` as described in `CLAUDE.md`.

**Step 4 — Build verification** (Milestone 2)

    msbuild Vixen.sln -m -t:Restore,Rebuild -p:Configuration=Debug

Expected: `Build succeeded.` with `Module.App.FPPClient.dll` in `Debug\Output\`.

**Step 5 — Implement client** (Milestone 4)

Create `Client/FppClient.cs` as described.

**Step 6 — Run tests** (Milestone 5)

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --logger "console;verbosity=normal"

Expected output example (count will vary):

    Passed! - Failed: 0, Passed: N, Skipped: 0, Total: N

Use `mcp__atlassian__createJiraIssue` as described.

## Validation and Acceptance

The feature is complete when:

1. `msbuild Vixen.sln -m -t:Restore,Rebuild -p:Configuration=Debug` succeeds with no errors.
2. `Debug\Output\Module.App.FPPClient.dll` exists and its assembly name begins with
   `Module.App.`.
3. `dotnet test src\Vixen.Tests\Vixen.Tests.csproj` reports zero failures and includes at
   minimum the 15 FPPClient test methods listed in Milestone 4.
4. A JIRA issue exists in the `VIX` project documenting the work.

To manually verify the client against a live FPP instance, write a small console snippet:

    var factory = new FppClientFactory();
    await using var client = factory.Create(new FppClientOptions { BaseUrl = "http://fpp2.home.kb9kld.org/" });
    var info = await client.GetSystemInfoAsync();
    Console.WriteLine($"{info.HostName} running FPP {info.Version} on {info.Platform}");

Expected output: something like `fpp2 running FPP 7.5 on Raspberry Pi`.

## Idempotence and Recovery

The `dotnet sln add` command is idempotent if the project is already listed. The `msbuild`
rebuild command can be run any number of times. If the Any CPU → x64 fix is applied twice,
the result is identical. If a test run fails halfway, fix the failing test and re-run;
no cleanup is needed.

If the project is partially created and a build fails, delete `src\Vixen.Modules\App\FPPClient\`
and start Milestone 1 fresh.

## Interfaces and Dependencies

### New files (all under `src/Vixen.Modules/App/FPPClient/`)

    FPPClient.csproj
    Descriptor.cs
    Module.cs
    Data.cs
    Client/IFppClient.cs
    Client/IFppClientFactory.cs
    Client/FppClientFactory.cs
    Client/FppClientOptions.cs
    Client/FppClient.cs
    Models/FppSystemInfo.cs
    Models/FppUtilization.cs
    Models/FppPlaylist.cs
    Models/FppPlaylistEntry.cs
    Models/FppPlaylistInfo.cs
    Models/FppUploadResult.cs
    Exceptions/FppClientException.cs

### Modified files

    Vixen.sln                            — add FPPClient project reference
    src/Vixen.Tests/Vixen.Tests.csproj   — add ProjectReference to FPPClient

### New test files (under `src/Vixen.Tests/FPPClient/`)

    Helpers/MockHttpMessageHandler.cs
    FppClientGetSystemInfoTests.cs
    FppClientGetSequencesTests.cs
    FppClientGetPlaylistsTests.cs
    FppClientGetPlaylistTests.cs
    FppClientUploadTests.cs

### NuGet dependencies

No new NuGet packages are required. `System.Text.Json` and `System.Net.Http` are included
in the .NET 10 SDK. `NLog` is already in `Directory.Packages.props`.

### Key types and their public signatures

    namespace VixenModules.App.FPPClient.Client;

    public interface IFppClient : IAsyncDisposable
    {
        Task<FppSystemInfo> GetSystemInfoAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetSequencesAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetPlaylistsAsync(CancellationToken cancellationToken = default);
        Task<FppPlaylist> GetPlaylistAsync(string playlistName, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<string>> GetPlaylistSequencesAsync(string playlistName, CancellationToken cancellationToken = default);
        Task UploadFileAsync(string dirName, string filename, Stream content, CancellationToken cancellationToken = default);
        Task UploadSequenceAsync(string filename, Stream content, CancellationToken cancellationToken = default);
        Task UploadMusicAsync(string filename, Stream content, CancellationToken cancellationToken = default);
        Task UploadVideoAsync(string filename, Stream content, CancellationToken cancellationToken = default);
    }

    public interface IFppClientFactory
    {
        IFppClient Create(FppClientOptions options);
    }

    public sealed record FppClientOptions
    {
        public required string BaseUrl { get; init; }
        public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(30);
        public TimeSpan UploadTimeout { get; init; } = TimeSpan.FromMinutes(10);
    }
