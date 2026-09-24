# Upload FSEQ files directly to ESPixelStick devices (VIX-4003)

This ExecPlan is a living document. Maintain its Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective sections during implementation, following `.agents/PLANS.md` from the repository root. The working directory for every command below is `C:\Dev\Vixen` unless stated otherwise.

## Purpose / Big Picture

A Vixen user who chooses Direct Upload in the FPP export wizard can send generated `.fseq` sequences straight to an ESPixelStick running current firmware, without first saving the export to a folder and uploading it manually. The summary page identifies the device and makes clear that ESPixelStick receives FSEQ files only. A normal Falcon Player host continues to receive sequences, optional audio, and optional universe configuration as before. A successful ESPixelStick export can be seen in the device's sequence list, and a normal FPP export can be checked in its file manager.

## Progress

- [x] (2026-09-24 21:41Z) Read VIX-4003, the existing export documentation and code, and the ESPixelStick firmware handlers; established the device marker and request format.
- [x] (2026-09-24 21:50Z) Milestone 1: Updated and re-read the existing VIX-4003 description with user requirements, acceptance criteria, and validation steps; status remains In Progress.
- [x] (2026-09-24 21:59Z) Milestone 2: Added the ESPixelStick FSEQ upload operation and HTTP tests; full-MSBuild test target succeeded, focused tests passed 19/19, and FPP client tests passed 38/38.
- [x] (2026-09-24 22:04Z) Corrected the Milestone 2 unreadable-stream test to use a live write-only stream; the Release test target rebuilt and the upload tests passed 15/15.
- [ ] Milestone 3: Detect the target once per export, route sequence uploads, suppress unsupported uploads, and make the summary accurate; add focused tests.
- [ ] Milestone 4: Build, run automated and manual checks, reconcile the issue description, and post validation results to VIX-4003.

## Surprises & Discoveries

- The existing FPP client builds HTTP requests under `/api/`. ESPixelStick's firmware instead registers its upload handler at `/fpp` and requires `path=uploadFile` and `filename` parameters; it returns FSEQ metadata JSON, including `Name`, on success. The issue's `/fpp/uploadFile` wording names the operation but is not the literal route registered by current firmware. Evidence: the upstream `WebMgr.cpp` and `FPPDiscovery.cpp` referenced by VIX-4003.
- The wizard already calls `GET /api/system/info` on its summary page, but `ExportDirect` currently constructs a new client for every sequence, and `CreateUniverseFileDirect` constructs another. Device identity must therefore be resolved for the actual export, rather than relying only on the earlier display call.
- Rider's `findTests` coverage hook failed to launch during planning. The existing test targets were located directly at `src/Vixen.Tests/FPPClient/FppClientUploadTests.cs` and `src/Vixen.Tests/ExportWizard/FppDirectUploadServiceTests.cs`. This does not affect the repository's MSBuild and xUnit test route.
- During Milestone 2, a pre-cancelled upload still reached the fake HTTP handler before cancellation surfaced. The first cancellation test incorrectly asserted that no handler call occurred (18 passed, 1 failed). The corrected test requires cancellation propagation without assuming zero handler calls; the focused suite then passed 19/19. The Release build also emitted warnings only in untouched files.
- The unreadable-stream argument test initially disposed a `MemoryStream` and then passed it to the client, a use-after-dispose pattern noted during review. A live `GZipStream` in compression mode has `CanRead == false`, so it exercises the same validation without using a disposed object. The corrected upload tests passed 15/15.

## Decision Log

- Decision: Identify ESPixelStick by an ordinal comparison of `FppSystemInfo.Platform` with `"ESPixelStick"`; all other platform values follow the existing FPP route. Rationale: this is the explicit discriminator supplied for VIX-4003, and `FppSystemInfo` already deserializes `Platform`. Date/Author: 2026-09-24, Codex.
- Decision: For ESPixelStick, upload only FSEQ data; do not upload audio, rename or upload `co-universes.json`, or restart FPPD. Keep the user's saved profile flags intact and explain the effective behavior in the summary. Rationale: the issue says only FSEQ files are uploaded, and the firmware exposes none of those other FPP file/configuration operations. Date/Author: 2026-09-24, Codex.
- Decision: Resolve system info once at the start of each direct export and share one client/service across that export, including multiple sequences. Rationale: this gives a current device identity, avoids repeated calls, and keeps all operations on one target. Failure to identify the device must stop before writing remotely and show the existing upload error dialog. Date/Author: 2026-09-24, Codex.
- Decision: Add a distinct `IFppClient` operation for the ESPixelStick request and keep ordinary `UploadSequenceAsync` unchanged. Rationale: the URL, response shape, and supported operations differ; a separate operation preserves the existing FPP contract and is directly testable. Date/Author: 2026-09-24, Codex.
- Decision: The new upload operation disposes its supplied content stream when the request completes and documents that ownership in its public XML remarks. Rationale: disposing request content also disposes its underlying stream; the wizard's temporary-file stream already has an idempotent disposal scope. Reject path separators in the destination filename before sending. Date/Author: 2026-09-24, Codex.
- Decision: Test unreadable content with a live write-only stream, not a disposed stream. Rationale: the production guard checks `CanRead`, so a live stream with `CanRead == false` tests the intended contract without use-after-dispose. Date/Author: 2026-09-24, Codex.

## Outcomes & Retrospective

Milestone 1 aligned VIX-4003 with the user-visible FSEQ-only scope, acceptance checks, and validation expectations; the issue remains In Progress. Milestone 2 now provides the separately testable ESPixelStick request in `IFppClient` and `FppClient`, without changing ordinary FPP uploads. It streams an octet-stream body to the host-root handler, checks HTTP success and the returned `Name`, and rejects invalid response metadata. The Release test build succeeded; focused tests passed 19/19 and the FPP client suite passed 38/38. Rider reported no file problems on changed C# lines, and `git diff --check` passed. After review, the unreadable-stream test was corrected to use a live write-only stream; the Release test target rebuilt and the upload tests passed 15/15. The wizard does not yet call the new method; routing and user-visible behavior remain for Milestone 3, while complete and hardware validation remain for Milestone 4.

## Context and Orientation

The issue is [VIX-4003](https://vixenlights.atlassian.net/browse/VIX-4003), an improvement to FPP export. `docs/export-wizard-fpp-upload.md` defines the current Direct Upload behavior, while `docs/plans/fpp-direct-upload-export-wizard.md` records its implementation. `src/Vixen.Modules/App/ExportWizard/BulkExportOutputFormatStage.cs` collects the host and options. `BulkExportSummaryStage.cs` shows device info and runs the export. `FppDirectUploadService.cs` opens the temporary FSEQ file and delegates uploads. `src/Vixen.Modules/App/FPPClient/Client/FppClient.cs` implements HTTP calls through `IFppClient.cs`; `Models/FppSystemInfo.cs` already contains the `Platform` property. `FppClientOptions.cs` stores the original host URL, but `FppClient` sets `HttpClient.BaseAddress` to the host's `/api/` directory.

An FSEQ file is the binary sequence produced by the export engine. A universe file is the optional FPP output configuration named `co-universes.json`. The existing direct path writes the FSEQ to a temporary local file, uploads it, then deletes it. Preserve that bounded-memory behavior. `FppClientUploadTests.cs` uses a fake HTTP handler to inspect requests without a device; `FppDirectUploadServiceTests.cs` uses Moq to verify client calls. The wizard is legacy WinForms, so maintain its existing controls and event-driven pattern rather than introducing a new view framework.

The firmware sources are `https://github.com/forkineye/ESPixelStick/blob/main/src/WebMgr.cpp` and `https://github.com/forkineye/ESPixelStick/blob/main/src/service/FPPDiscovery.cpp`. `GetSysInfoJSON` sets `Platform` to `ESPixelStick` and `Mode` to `remote` when an SD card is installed. The web server registers POST/PUT at `/fpp`; `ProcessBody` reads `path=uploadFile` and `filename`, writes the request body as file bytes, and `ProcessPOST` returns FSEQ metadata after the file exists. An unavailable SD card causes an HTTP error. Test the wire shape against current firmware before declaring the hardware acceptance complete.

## Architecture Design: VIX-4003

Detected IDE environment: JetBrains Rider, with the repository's command-line MSBuild route available when a Rider automation hook cannot run. Use Rider's Apply snippet from chat for the small, file-scoped C# updates and its Test Runner for focused tests where available; use the exact commands below as the reliable fallback. The design follows the project `dotnet-best-practices`, `csharp-async`, and `csharp-docs` skills: keep asynchronous HTTP I/O cancellable, dispose streams/responses, use structured NLog calls, and document the new public interface method. The existing `IFppClient` and `FppClientFactory` remain the provider boundary; no new persisted data model, dependency injection registration, or project is required.

The routing predicate is `string.Equals(info.Platform, "ESPixelStick", StringComparison.Ordinal)`. A missing or different value uses FPP behavior only after a successful system-info response; a failed or malformed response aborts direct export rather than guessing. For ESPixelStick, send `POST` to the host-root URL `/fpp?path=uploadFile&filename=<URI-escaped-fseq-name>` with the FSEQ bytes as `application/octet-stream`. The leading slash is essential because the current `HttpClient.BaseAddress` ends in `/api/`. Use the configured upload timeout and caller cancellation token. Treat a non-2xx response as `FppClientException` with the status and filename. On HTTP success, read the firmware metadata response and verify its `Name` is the requested filename; an empty, invalid, or mismatched response is a failed upload, not a reported success. The firmware currently returns this metadata from `BuildFseqResponse`; confirm the exact body in the hardware acceptance check.

The component boundary is: `BulkExportSummaryStage` fetches current system info before starting the sequence loop and shows the effective ESPixelStick limitations; `FppDirectUploadService` routes a generated FSEQ according to the resolved target type; `FppClient` performs the device-specific HTTP request. `BulkExportOutputFormatStage` continues collecting the same saved options. `ExportProfile` serialization is unchanged. The normal file-path export never queries a device.

## Plan of Work

### Milestone 1 — Align the issue before implementation

Update the existing VIX-4003 description, rather than creating a second issue. Keep the issue user-facing: state that a user can select an ESPixelStick host in Direct Upload, that Vixen detects it from system info, that the resulting FSEQ appears on the device, and that audio and universe configuration are not uploaded to ESPixelStick. State that ordinary FPP and file-path exports retain their current behavior. Include acceptance checks for one sequence, multiple sequences, an ESPixelStick with audio/universe options selected, an unreachable or storage-less target, a normal FPP host, and file-path export. Include automated test and hardware validation expectations in plain language. Re-read VIX-4003 to verify the update; do not transition its status merely to satisfy this plan.

### Milestone 2 — Implement the device-specific client request

In `src/Vixen.Modules/App/FPPClient/Client/IFppClient.cs`, declare `Task UploadEspPixelStickSequenceAsync(string filename, Stream content, CancellationToken cancellationToken = default)` with XML summary, parameter, exception, and return documentation. In `src/Vixen.Modules/App/FPPClient/Client/FppClient.cs`, implement it using the existing `_httpClient`, `_options.UploadTimeout`, and structured NLog pattern. Validate the filename and stream, escape the filename as a query value, use a host-root request URI, and send a streaming octet-stream body without buffering the entire file. Dispose linked cancellation, request content, and response correctly; the service closes its temporary file stream after the call. Check HTTP status and the returned `Name`; surface a clear `FppClientException` for an unsuccessful response. Leave `UploadSequenceAsync`, `UploadFileAsync`, and other FPP endpoints unchanged.

Extend `src/Vixen.Tests/FPPClient/FppClientUploadTests.cs` using its existing fake HTTP handler. The handler in `src/Vixen.Tests/FPPClient/Helpers/MockHttpMessageHandler.cs` accepts an asynchronous callback so a test can inspect request bytes without blocking. Assert the full host-root path and query, POST method, content type, exact body bytes, successful metadata handling, escaped filenames, cancellation/error behavior, and rejection of a mismatched or invalid metadata response. Keep normal-FPP upload tests green. A focused Rider Test Runner execution or the test command below proves this milestone independently.

### Milestone 3 — Route the wizard export and explain effective options

In `src/Vixen.Modules/App/ExportWizard/BulkExportSummaryStage.cs`, fetch `GetSystemInfoAsync` once for the actual direct-export run before rendering/uploading any sequence. Share the resulting client and `FppDirectUploadService` through the sequence loop and the optional universe step, instead of creating a client for each item. Dispose the client after the run. If detection fails, log and display the existing direct-upload error and avoid remote writes. Handle the user's cancellation consistently with the current wizard. Pass the resolved ESPixelStick predicate to the service; in `FppDirectUploadService.UploadSequenceFileAsync`, call the new client operation for ESPixelStick and the existing `UploadSequenceAsync` for FPP. Do not change sequence rendering or local file-path export.

For an ESPixelStick run, bypass the audio branch in `ExportDirect` and the universe backup/upload/restart branch in `CreateUniverseFile`. Ensure the per-sequence progress still reaches completion and temporary FSEQ files are deleted on success, error, or cancellation. Do not overwrite saved `IncludeAudio`, `CreateUniverseFile`, or `BackupUniverseFile` flags: a later export to ordinary FPP should still honor them. In `ConfigureSummary`/`PopulateFppInfoAsync`, show `Platform: ESPixelStick` and a clear FSEQ-only notice, and ensure audio/universe summary labels do not claim those files will be uploaded. Restore the ordinary summary text on each page entry so switching hosts does not leave a stale notice. Keep any changed public/protected API XML comments accurate.

Extend `src/Vixen.Tests/ExportWizard/FppDirectUploadServiceTests.cs` to verify both routing branches and progress/error behavior. Extract only the small decision logic needed for unit coverage from the WinForms stage; test that ESPixelStick suppresses audio and universe actions while a normal FPP target retains them. Verify a system-info failure prevents all upload calls. Keep the diff limited to this behavior and run Rider `get_file_problems` on each changed C# file; fix only findings on changed lines.

### Milestone 4 — Validate and close out the plan

Build the affected solution and run the test project with the full MSBuild C++ toolchain, then run the focused and full xUnit suites without rebuilding. In Rider, the Test Runner can repeat the focused tests; the `findTests` dotCover hook failed during planning, so it is optional. Manually export at least one FSEQ and then multiple FSEQs to an SD-equipped ESPixelStick on current firmware; verify their names and contents in the device sequence list and confirm no audio or `co-universes.json` action was attempted. Repeat with an ordinary FPP host, with file-path output, and with an inaccessible or storage-less device to verify the visible error. Record firmware version, commands, pass counts, and any missing hardware check in this plan. Reconcile VIX-4003's description if the final behavior differs from the initial requirements, then add a concise Jira comment with validation results. Do not claim hardware verification unless a device was actually exercised.

## Concrete Steps

From `C:\Dev\Vixen`, inspect `git status --short` before editing and preserve unrelated changes. For Milestone 2, run the full-MSBuild test target first because two test dependencies are C++/CLI projects. The commands are:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:/Dev/Vixen/" --filter "FullyQualifiedName~FppClientUploadTests|FullyQualifiedName~FppDirectUploadServiceTests"
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:/Dev/Vixen/"

When production changes span both modules, also run:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release

Record the actual passed/failed totals rather than assuming a fixed future count. The focused tests should fail before the client/routing change and pass after it; the existing upload and export tests must remain passing. Launch the built application via Rider's Vixen run configuration for manual wizard checks, or use the application's executable from `Release/Output/` if that configuration is unavailable. In the wizard select Falcon Pi Player 2.x format, Direct Upload, and the target host; finish the export and inspect the device. Use a disposable test sequence and host so repeated uploads do not overwrite show data.

## Validation and Acceptance

Given an ESPixelStick whose `GET /api/system/info` returns `Platform: "ESPixelStick"`, a Direct Upload of `Test.fseq` sends one streamed FSEQ request to the firmware upload handler, reports completion only after a successful response, and leaves `Test.fseq` on the device. Multiple selected sequences produce one upload per sequence while system info is resolved once for the export run. With Include Audio and universe options selected, the summary explicitly states that these files are not uploaded and no music, configuration, rename, or FPPD restart HTTP request occurs. If system info or file storage is unavailable, the user sees an actionable error and the wizard does not report a successful FSEQ upload. The temporary FSEQ is removed in every outcome.

Given a normal FPP system-info response, the sequence still uses `/api/file/sequences/<name>`, and selected audio and universe options still use their existing FPP operations. Given File Path mode, exports still write to the selected folder without contacting the host. The Release solution build and focused/full xUnit commands above must pass; capture their actual counts. A real ESPixelStick check is required to establish protocol compatibility because a fake HTTP handler cannot prove firmware routing or storage behavior.

## Idempotence and Recovery

Re-running the tests and builds is safe. Re-running a manual direct export may replace an identically named sequence on the target; use a throwaway sequence or back up device content first. A failed upload must leave the original local sequence untouched and clean its temporary export file. If a network/device check fails, fix connectivity or storage and retry the wizard; do not fall back silently to the normal FPP endpoint. Restore the user's existing profile selections when switching to another host. Do not create a Git commit unless explicitly requested.

## Artifacts and Notes

Expected ESPixelStick wire shape, with the filename escaped as a URI query component:

    GET  http://<host>/api/system/info  -> {"Platform":"ESPixelStick", ...}
    POST http://<host>/fpp?path=uploadFile&filename=Test.fseq
    Content-Type: application/octet-stream
    Body: the generated FSEQ bytes
    Response: HTTP 200, JSON with "Name":"Test.fseq"

The ticket retains links to the firmware sources above. The route and parameter interpretation comes from those sources; the final hardware check validates the complete request against an actual firmware build. The existing FPP route is `POST http://<host>/api/file/sequences/Test.fseq` and must remain unchanged.

## Interfaces and Dependencies

`IFppClient` gains one public asynchronous method, `UploadEspPixelStickSequenceAsync(string filename, Stream content, CancellationToken cancellationToken = default)`. `FppClient` implements it using `HttpClient`, `StreamContent`, `System.Text.Json` for the metadata response, and the existing `FppClientException`. `FppDirectUploadService` retains the existing sequence/audio/universe methods but takes the resolved target kind for its sequence upload decision. `BulkExportSummaryStage` owns target detection for each export run and passes that decision down; no serialized `ExportProfile` change is needed. Tests use xUnit, Moq, and the existing `MockHttpMessageHandler`; no new NuGet package or solution project is required.

Plan revision (2026-09-24, Codex): Created this plan after reading VIX-4003, the user's explicit `Platform` clarification, current Vixen docs/source/tests, and the referenced firmware handlers. The initial decisions are recorded above; revise every affected section if implementation findings change them.

Plan revision (2026-09-24 21:50Z, Codex): Completed Milestone 1 by updating the existing Jira description, then re-reading it to confirm all planned user-facing requirements, acceptance checks, and validation expectations. Updated Progress and Outcomes accordingly; no implementation design changed.

Plan revision (2026-09-24 21:59Z, Codex): Completed Milestone 2's client API and HTTP tests. Recorded the stream-ownership decision, the cancellation-test correction, actual build and test results, and the remaining wizard routing and hardware checks so the plan reflects observed behavior.

Plan revision (2026-09-24 22:04Z, Codex): Corrected the unreadable-stream test after review identified a disposed `MemoryStream` being passed to the client. A live write-only stream preserves the intended `CanRead` coverage without use-after-dispose; updated Progress, Surprises, Decision Log, and Outcomes with the correction and 15/15 upload-test result.
