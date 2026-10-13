# VIX-4020: Remove network waits from Export Wizard controller selection

This ExecPlan is a living document. Maintain Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective in accordance with .agents/PLANS.md. The user explicitly invoked .agents/skills/analyze-and-plan-issue/SKILL.md; its manual milestone review boundaries apply. Do not create commits without an explicit user request.

## Purpose / Big Picture


A user with a large display should reach the Export Wizard controller list promptly, even when controller hostnames cannot be resolved on the current computer. The list should display actual configured controllers rather than a temporary Test row. Controller selection and ordering should work without contacting the network. When an export actually needs network configuration, show progress and keep the wizard responsive while preparing it.

The original issue is https://vixenlights.atlassian.net/browse/VIX-4020. Reproduce by opening a profile with many controllers, selecting Tools → Export Wizard, choosing or creating an export profile, adding a sequence, and clicking Next to enter the controllers stage. The reported delay is close to 14 seconds. The supplied reproduction snapshot is C:\Dev\Snapshots\MarkBar\VIX-4020-initial-capture.dtp.

## Progress


- [x] (2026-10-12) Read the issue and the analyze-and-plan-issue and dottrace-analyze skills; read .agents/PLANS.md and the relevant project skills.
- [x] (2026-10-12) Inspect the supplied trace, including Main-thread waiting and running trees; locate the eager DNS path and the designer placeholder.
- [x] (2026-10-12) Review existing controller-selection documentation and the relevant export source; record a scoped architectural review in docs/reviews/vix-4020-export-controller-loading.md.
- [x] (2026-10-12 21:21Z) Milestone 1: Updated JIRA requirements and acceptance criteria, preserved New Ticket status, saved the plan and design review, and reached the required manual review boundary. No application code changed.
- [x] (2026-10-12) Milestone 2: Implement metadata-only wizard synchronization, explicit asynchronous network preparation, placeholder removal, regression tests, and validation. Paused at the required manual review boundary.
- [x] (2026-10-13) Milestone 3: User repeatedly exercised cold and warm runs with the original profile (>20 controllers, several requiring network lookups), confirmed immediate controller-list population, conditional summary progress, correct universe outputs, immediate cancellation during preparation, and successful wizard re-entry. Per the user's direction, no new trace or numerical timing was required; JIRA validation comments were added. No quantitative speedup or post-change profiler result is claimed.

## Surprises & Discoveries


The captured delay is network waiting, not list rendering or CPU saturation. The unfiltered tree attributes 16,204.1 ms of own elapsed time to System.Net.NameResolution.dll beneath E131OutputPlugin.DetermineIp. Six SocketException events report No such host is known at 5,165, 7,865, 10,564, 13,262, 15,968, and 18,674 ms. Each unsuccessful lookup costs about 2.7 seconds. One ArgumentNullException occurs at 2,464 ms. The trace cannot establish the actual hostname strings or whether the six names are distinct.

Main thread 40924, filtered to waiting between 2.4 and 18.8 seconds, attributes 16,197 ms to the same DNS path. The running tree attributes only 36.5 ms to UpdateNetworkList. The profiler reports overlapping UI-freeze intervals; do not add their durations and describe that sum as wall-clock freeze time.

Controller metadata and network details are currently coupled in the public Controller constructor in src/Vixen.Core/Export/Export.cs. BulkExportControllersStage.UpdateNetworkList uses only Name, Channels, IsActive, and Index. The fake row is explicitly inserted in BulkExportControllersStage.Designer.cs, rather than being a real controller or loading state.

BulkExportSummaryStage.StageStart is already asynchronous and calls ConfigureSummary, which inspects Export.AllSelectedControllersSupportUniverses. That property accesses HasNetworkSupport. Simply making network properties lazy would risk moving the same freeze to summary initialization. The chosen design therefore uses explicit preparation and readiness, not implicit work in getters.

The existing docs/plans/export/vix-2823-export-wizard-controller-toggle.md describes native multi-selection and checkbox toggles, coalesced channel reindexing, Escape interception, and bulk buttons. Preserve these behaviors. Its historical dotnet-test-only commands are superseded by the current AGENTS.md full-MSBuild test workflow.

Implementation exposed another eager lookup: `BulkExportWizardData.Export` created `Export`, whose constructor loaded all network configurations before the wizard assigned its profile snapshots. The wizard now uses an overload that skips only this initial network load; the existing parameterless `Export` constructor retains its eager behavior for the legacy editor dialog.

Stage re-entry can overlap a synchronous provider call that cannot be cancelled in flight. The snapshot now serializes initialization attempts and marks itself ready only after successful completion, so overlapping/retried preparation cannot duplicate a provider lookup.

## Decision Log


Decision: Keep controller metadata synchronization independent of network configuration, and explicitly prepare network details only for exports requiring universe configuration. Rationale: The trace ties virtually the entire delay to eager DNS that the selection page does not use. Ordinary sequence export should not inherit a network dependency. Date/Author: 2026-10-12 / Codex.

Decision: Preserve the existing public constructor and CreateControllerInfo signatures and their eager defaults; add overloads for metadata-only construction. Rationale: The shared snapshot has callers in core export and the legacy editor dialog, and Gortex reports a lower-bound impact of 65 symbols. Limiting changed behavior to wizard profile synchronization reduces regressions. Date/Author: 2026-10-12 / Codex.

Decision: Leave E1.31 resolution semantics intact. Rationale: Replacing hostnames with loopback, inventing a short timeout, or caching failed DNS globally could silently change exported addresses. Preserve hostname support, multicast configuration, and existing failure fallback. Date/Author: 2026-10-12 / Codex.

Decision: Stop after the initial requirements milestone. Rationale: The explicitly invoked analyze-and-plan-issue skill requires manual review and confirmation before advancing to the next milestone. No application fix is claimed at this boundary. Date/Author: 2026-10-12 / Codex.

Decision: Keep the parameterless `Export` constructor eager and add an explicit overload used by the wizard. Rationale: the wizard replaces the initial controller snapshots with profile data, so it should avoid the unnecessary lookup while preserving the existing behavior for the legacy dialog. Date/Author: 2026-10-12 / Codex.

Decision: Invalidate pending summary preparation when the stage ends before readiness or is cancelled. Rationale: the controller module's synchronous lookup cannot be interrupted in flight, but cancellation and generation checks prevent its late completion from enabling export after navigation or closure. Date/Author: 2026-10-12 / Codex.

## Outcomes & Retrospective


Milestone 2 separates metadata snapshots from network configuration, starts required controller preparation on a worker, gates the summary stage until preparation and summary setup succeed, and invalidates stale work on cancellation/navigation. The temporary Test row is removed. Regression coverage verifies metadata construction, profile synchronization and cloning, saved identity/count/order/active state, eager versus explicit configuration equivalence with multicast and multiple universes, idempotent preparation, inactive-controller skipping, cancellation between controllers, provider retry after failure, missing source rejection, stale completion rejection after profile replacement/navigation/cancellation/disposal, and controllers without network or universe support. All provider tests use fakes and perform no real DNS lookups.

Validation completed: `msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m` succeeded; `dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~ExportWizard"` passed 41 tests; `msbuild Vixen.sln -m -t:restore -t:Build -p:Configuration=Release -p:Platform=x64` succeeded with 0 warnings and 0 errors. Rider file problem checks reported no diagnostics in changed C# files. `git diff --check` passed with no whitespace errors.

User-reported M3 verification (2026-10-13): The user repeatedly ran the original profile from cold starts and warm passes. It contains more than 20 controllers, several of which encounter network lookup issues; the controller list populated immediately on every run. On the final stage, the progress bar appears when included controllers require slow network configuration evaluation; for quick or unnecessary resolution it either does not appear or completes before becoming visible. Universe-file outputs were checked against several known configurations and appeared correct. The user tested cancellation during network configuration; Cancel closed the dialog immediately, and subsequent wizard visits worked as expected. The user explicitly determined that a new trace is unnecessary. No numerical timings or post-change trace were collected, so the observed responsiveness is qualitative and no quantitative speedup or profiler result is claimed.

## Context and Orientation


Detected IDE environment: Rider with automation hooks, dotTrace snapshot analysis, and get_file_problems. Use Gortex for indexed source inspection and mutation. The repository root is C:\Dev\Vixen. The existing wizard is WinForms; this work maintains it and does not introduce Catel views or new WPF architecture.

A controller snapshot is the Vixen.Export.Controller object describing an output controller for an export. Its saved DataMembers include Id, Index, and IsActive. Other fields are rebuilt from current display configuration by ExportProfile.SyncronizeControllerInfo. Network configuration supplies IP addresses and universe information used when producing controller configuration files. A universe is a numbered group of output channels.

The relevant files are src/Vixen.Core/Export/Export.cs for CreateControllerInfo, Controller, universe writers, and capability checks; src/Vixen.Modules/App/ExportWizard/ExportProfile.cs for synchronization and cloning; src/Vixen.Modules/App/ExportWizard/BulkExportControllersStage.cs and its Designer.cs for list population and the Test placeholder; and src/Vixen.Modules/App/ExportWizard/BulkExportSummaryStage.cs for asynchronous preparation and universe output. src/Vixen.Modules/App/ExportWizard/BulkExportWizardData.cs assigns profile snapshots to the export engine. src/Vixen.Modules/Controller/E131/E131OutputPlugin.cs implements the measured DNS path and is evidence, not a planned modification target.

Existing regression coverage includes src/Vixen.Tests/ExportWizard/ExportProfileCloneTests.cs. Follow actual project test conventions rather than imposing the generic skill's example framework. Add meaningful tests under src/Vixen.Tests/ExportWizard/ControllerNetworkConfigurationTests.cs, subject to confirming the existing fake module and OutputController construction conventions before writing them.

## Architecture Design: VIX-4020


The core strategy is an explicit two-phase snapshot: populate metadata for controller selection, then prepare network configuration when the requested export needs it. Apply .agents/skills/dotnet-best-practices, csharp-async, and csharp-docs during implementation. The design-pattern review is recorded in docs/reviews/vix-4020-export-controller-loading.md. No controller-module interface, project dependency, serialized profile schema, or global DNS cache changes are required.

Retain the existing eager constructor Controller(OutputController controller, int index), delegating to a new overload with initializeNetworkConfiguration true. Add an overload Controller(OutputController controller, int index, bool initializeNetworkConfiguration). Capture the source controller in a private runtime-only field; populate names, identity, counts, order, and active state immediately. Add an explicit InitializeNetworkConfiguration method that obtains the module configuration and sets HasNetworkSupport and ControllerNetworkConfiguration using the existing SupportsNetwork and SupportsUniverses rules. Guard successful completion so repeated preparation of the same snapshot does not repeat a lookup. Do not mark runtime source references or readiness as DataMembers. A metadata-only snapshot must not cause network work through property access, cloning, list painting, or reindexing.

Retain Export.CreateControllerInfo(bool index) and add CreateControllerInfo(bool index, bool initializeNetworkConfiguration), preserving stable enumeration order. Have both branches of ExportProfile.SyncronizeControllerInfo use metadata-only construction. Preserve the existing reconciliation by Id, IsActive, and Index and the final reindex operation. A rebuilt snapshot permits new configuration to be loaded after display edits; do not reuse a stale global snapshot.

Add an internal Task-returning preparation helper on ExportProfile. Capture the intended profile and active controller snapshots on the UI thread before scheduling work. Prepare selected network controllers on a worker with cancellation checks between controllers. The current synchronous module API cannot cancel an in-flight DNS call; cancellation must nevertheless prevent stale results from updating the wizard. Do not start unbounded concurrent plugin calls. Public/protected APIs added or changed need XML summary, parameter, return, and behavioral documentation in the same change.

On the summary stage, prepare network details only when the profile is a Falcon export requesting CreateUniverseFile. Display a clear preparation message and busy indication, disable export completion until preparation succeeds, await the helper, then publish readiness and call ConfigureSummary on the UI thread. Guard a closed stage, cancellation, navigation away, and a replacement profile so old work cannot re-enable a different export. Reuse the existing StageEnd and cancellation lifecycle after inspecting their full source. Keep exceptions observed and report an actionable error while allowing retry or cancellation. The void StageStart signature belongs to the existing wizard contract; do not redesign that shared interface. Put new work in Task-returning helpers and contain exceptions at the existing lifecycle boundary.

Change ConfigureSummary's universe-warning condition so CreateUniverseFile and the applicable format are checked before inspecting completed network capability. Ordinary exports must not interpret unloaded snapshots as unsupported controllers. Both CreateUniverseFileDirect and CreateUniverseFileToPath must receive fully prepared snapshots before writing a file. Existing eager callers continue to receive configuration immediately.

Remove the local Test ListViewItem declaration and its Items.AddRange and StateImageIndex lines from BulkExportControllersStage.Designer.cs. Keep list layout, native selection, bulk toggles, and channel reindexing intact. A real configured controller named Test must still appear normally.

Boundary logic remains unchanged: export indices follow saved order; active channel ranges start at 1 and accumulate each controller's channel count; inactive rows have blank ranges. Zero controllers leave the existing navigation and bulk-button rules intact. Multicast, literal IPv4, resolvable hostnames, unresolved hostnames, missing destinations, and controllers without universe support must retain the existing prepared-configuration semantics.

## Plan of Work


### Milestone 1: Record requirements and profiling evidence


Update VIX-4020's description with the original reproduction and concise user-facing scope and acceptance criteria. Preserve the reported timing and issue status. Save this plan and the architectural review, inspect the scoped changes, and report that the root cause is known but implementation is pending. Validate the baseline with the Main-thread DNS waiting tree and exception timestamps. No build is needed for this documentation-only milestone.

STOP HERE for manual review and commit execution before proceeding. Halt code execution, run git status --short and scoped git diff, invoke the project commit-msg skill, and output its complete paste-ready message. Do not create a commit. Pause and wait for explicit user confirmation before advancing.

### Milestone 2: Implement and verify the separated snapshot


Read the complete remaining callers, summary lifecycle, universe writers, and relevant test setup using Gortex before modifying them. Apply the constructor/factory overloads and explicit preparation contract described above. Modify Export.cs, ExportProfile.cs, BulkExportSummaryStage.cs, BulkExportWizardData.cs, the controller-stage Designer.cs, the necessary regression tests, and this plan. The BulkExportWizardData change is required because its lazy Export property constructs Export before replacing the initial controller list; preserve Export's existing eager default for non-wizard callers. Change BulkExportControllersStage.cs only if a brief local metadata-loading indication is still needed after measurement. Keep existing eager callers compatible. Use Gortex impact before mutation and verify every added or changed signature. Mutate through Gortex edit/refactor; afterwards detect changes and run tests, guards, and contract on the returned symbol IDs.

Tests must demonstrate zero configuration-provider calls during metadata construction, profile synchronization, and cloning; preservation of controller identity, counts, active flags, and order; equivalence of explicitly prepared and eagerly constructed configuration; no repeat work after successful preparation of the same snapshot; and correct behavior for non-network or non-universe controllers. Cover inactive controllers, multiple universes, hostname/multicast data, provider failure, cancellation, and stale completion without real DNS. Confirm the existing profile-clone suite remains green. For a missing or removed source controller, report the preparation error and prevent exporting a partial universe file.

Run Rider get_file_problems for every changed C# file and resolve only diagnostics within changed lines. Build the tests with full MSBuild, run the focused export tests, and build the affected solution configuration because core behavior crosses project boundaries. Acceptance is a populated controller list with no DNS on entry, intact selection/reorder/bulk behavior, and correctly prepared universe configuration with visible responsive loading when actually required. Record exact commands and results in this document.

STOP HERE for manual review and commit execution before proceeding. Halt code execution, run git status --short and scoped git diff, invoke commit-msg with VIX-4020 as the subject prefix, output the complete message, and wait for explicit confirmation. Do not create a commit.

### Milestone 3: Confirm the reproduction and report results


Exercise the user's original profile and sequence across cold and repeated warm runs, including ordinary and universe-file exports, then verify Back/Next and cancellation during preparation. Confirm controller-list responsiveness with the original large profile and that wizard re-entry works after cancellation. The user may explicitly waive a new trace and quantitative timing; if so, record that decision and report observed behavior without claiming measured latency or profiler evidence. If a trace is requested, compare it with the supplied initial capture using dottrace-analyze.

Update this plan and the design review with results and limitations. Adjust VIX-4020's description to match final behavior and add a concise validation comment using the jira skill. Do not close or transition the issue unless explicitly requested.

STOP HERE for manual review and commit execution before proceeding. Inspect scoped git status/diff, invoke commit-msg if repository documents changed, output the complete message, and pause for final review. Do not create a commit.

## Concrete Steps


Run all shell commands from C:\Dev\Vixen in PowerShell. Start long-running commands with exec_command and yield_time_ms 10000; wait with write_stdin and yield_time_ms at least 30000. If msbuild is not on PATH, locate the installed Visual Studio MSBuild; do not substitute dotnet's MSBuild for C++/CLI projects.

    git status --short
    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter "FullyQualifiedName~ExportWizard"
    msbuild Vixen.sln -m -t:restore -t:Build -p:Configuration=Release -p:Platform=x64
    git diff --check

Expect MSBuild exit code 0 and no new compiler errors. Expect the focused export test run to pass every selected test; record the actual count rather than inventing one. The provider-call regression must fail against the eager baseline and pass after the change. The final build covers shared core and wizard callers. Launch the built Vixen executable under Release/Output and follow the original reproduction; do not stop an unrelated running instance or alter the original profile without a copy.

The useful baseline profiler commands are invoked through Rider execute_tool, not a shell process:

    dotTraceGetCallTree --snapshotPath "C:\Dev\Snapshots\MarkBar\VIX-4020-initial-capture.dtp" --filterThreads 40924 --filterThreadState waiting --timeRangeStart 2.4s --timeRangeEnd 18.8s --minOwnTimePercent 0.1 --maxNodes 35 --format compact
    dotTraceGetTimelineEvents --snapshotPath "C:\Dev\Snapshots\MarkBar\VIX-4020-initial-capture.dtp" --eventTypes exception --maxEvents 15 --format compact

## Validation and Acceptance


Against the original profile, controller-stage entry must no longer wait for the six hostname failures. Every real configured controller appears with the correct count and saved order, without a temporary Test row. A configured controller whose actual name is Test is not filtered out. Checkboxes, Ctrl+A, Escape, Space, Enable All, Disable All, and multi-row dragging keep the VIX-2823 behavior and correct contiguous channel ranges.

Ordinary sequence exports proceed without loading controller network configuration. Universe-file export prepares network data with visible loading feedback while the window remains responsive, then generates the same IP/universe/channel configuration as before for equivalent resolved inputs. Preserve E1.31's existing failure fallback; do not promise a new hostname-error policy in this issue. Actual module preparation failures must prevent a partial configuration export and be reported. Test zero controllers, one controller, inactive controllers, mixed universe support, returning to earlier stages, switching export profiles, and closing during preparation.

Capture before/after evidence on the same profile. The structural performance acceptance is zero controller-address lookups during controller-stage initialization and no main-thread DNS wait during later preparation. Record measured latency after implementation; do not assume a numerical speedup from the estimated 16.2 seconds of removable waiting.

## Idempotence and Recovery


Changes are local source-controlled edits plus the documented JIRA requirements update. No profile migration, new package, or solution project is required. Re-run focused builds/tests after correcting a failure. Preserve unrelated user edits; never reset the whole worktree. Restore only the task's edits by an explicit reviewed patch if recovery is required. Keep the baseline snapshot untouched. Avoid indefinite DNS caches; a new synchronization must rebuild fresh runtime snapshots after display changes.

## Artifacts and Notes


The design review is docs/reviews/vix-4020-export-controller-loading.md. Gortex impact reports 14 direct and 65 transitively affected symbols as a lower bound, including the legacy export dialog and ExportProfileCloneTests; this does not imply all those files need edits.

Baseline evidence:

    DNS own elapsed time: 16204.1 ms
    Main-thread scoped DNS waiting: 16197.0 ms
    UpdateNetworkList running time: 36.5 ms
    Failed hostname lookups: 6, approximately 2.7 s each

These figures are measurements from the initial snapshot, not a measured improvement.

## Interfaces and Dependencies


Preserve the existing core APIs and add overloads rather than removing them:

    public Export()
    public Export(bool initializeControllerNetworkConfiguration)
    public Controller(OutputController controller, int index)
    public Controller(OutputController controller, int index, bool initializeNetworkConfiguration)
    public void InitializeNetworkConfiguration()
    public static List<Controller> CreateControllerInfo(bool index)
    public static List<Controller> CreateControllerInfo(bool index, bool initializeNetworkConfiguration)

In ExportProfile, add an internal Task-returning helper for explicit worker preparation, with an Async suffix and a CancellationToken. The exact helper name may be InitializeControllerNetworkConfigurationAsync. Existing serialized profile members remain unchanged. Document readiness and clone behavior in XML documentation for modified APIs. Before adding these signatures, use Gortex verify against their actual declarations and consumers.

No Catel service, controller-module interface change, NuGet package, or solution entry is proposed. Use the existing module GetNetworkConfiguration contract and existing universe writers. Inspect their full source before execution; adapt lifecycle guards to the existing summary-stage cancellation and control-enabling code instead of inventing a parallel wizard lifecycle.

Analysis complete and plan integrated with plans.md.

Initial plan written 2026-10-12 from VIX-4020 and the supplied trace. It records a verified cause, a proposed explicit preparation design, and required manual review boundaries; implementation and post-change profiling remain pending.
