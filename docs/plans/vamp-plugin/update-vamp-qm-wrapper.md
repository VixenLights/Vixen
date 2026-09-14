# Update the embedded VAMP Bar/Beat analysis stack

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept current as work proceeds. Maintain it according to `.agents/PLANS.md` from the repository root.

## Purpose / Big Picture

Vixen's Beats and Bars analysis module will continue to create the same useful beat, bar, beat-count, and preview mark collections for a user's audio, while its embedded audio-analysis sources are updated from the old VAMP SDK 2.5 and QM VAMP Plugin 1.7-era sources. The final implementation will embed the Bar and Beat Tracker from QM VAMP Plugins 1.8.0 and VAMP Plugin SDK 2.10, retaining Vixen's existing managed C++/CLI API so the rest of the application neither discovers nor loads a separate VAMP plugin DLL.

An operator can see the result by running Beat and Bar detection on the baseline audio fixtures. The resulting marks must retain their feature meanings, labels, and times, except for a documented timing difference no larger than one audio sample where VAMP 2.10 corrects rounding behavior.

## Progress

- [x] (2026-09-14) Identified `QMLibrary` as a C++/CLI dynamic-library project that compiles Vixen-owned wrapper code plus selected native source files; it does not link or load a prebuilt QM VAMP plugin.
- [x] (2026-09-14) Confirmed that `src/Vixen.Modules/Analysis/QMLibrary/qm-vamp-plugins-1.7` had no repository references and was removed in a separate committed checkpoint.
- [x] (2026-09-14) Identified local VAMP SDK version 2.5 and current upstream release version 2.10; identified QM VAMP Plugin release 1.8.0 as the update target.
- [x] Milestone 1 completed (2026-09-14): captured a deterministic 44.1 kHz, 120 BPM, 32-bar click-track baseline, added direct QMLibrary regression coverage, and recorded a user-created real-audio Vixen export baseline for post-upgrade comparison.
- [x] Milestone 2 completed (2026-09-14): staged VAMP SDK 2.10, QM VAMP Plugins 1.8.0, and the exact QM DSP lock revision outside the working tree; recorded provenance and the Bar/Beat direct-source closure; proved the closure compiles and links natively without OpenBLAS.
- [x] Milestone 3 completed (2026-09-14): replaced the VAMP 2.5 source tree with VAMP 2.10, imported QM 1.8.0 Bar/Beat and its locked QM DSP source closure, added the required KissFFT sources, and built `QMLibrary` for Release x64 without an external VAMP, QM DSP, or OpenBLAS link input.
- [x] Milestone 4 completed (2026-09-14 10:41Z): reviewed the VAMP 2.10 managed façade, delegated input-domain reporting to the native plugin, added idempotent `ManagedPlugin` disposal with predictable post-disposal errors, and expanded focused wrapper coverage to 3 passing tests.
- [ ] Milestone 5 in progress (2026-09-14 11:32Z): moved the full-track analysis work off the WinForms UI thread, added an explicit finalization state, and passed the full 935-test suite; manual verification of responsive progress on the affected audio and final mark comparison remain.

## Surprises & Discoveries

- Observation: `QMFiles` is not a single VAMP source tree. Its `vamp-plugin-sdk-2.5` child is VAMP SDK code; `BarBeatTrack.*`, `base`, `dsp`, and `maths` are QM plugin and QM DSP algorithm code.
  Evidence: `QMLibrary.vcxproj` explicitly includes `QMFiles\\vamp-plugin-sdk-2.5\\src\\vamp-sdk\\RealTime.cpp`, `QMFiles\\BarBeatTrack.cpp`, and selected QM DSP files as separate compile items.

- Observation: Vixen uses one embedded plugin implementation rather than a generic plugin host.
  Evidence: `QMBarBeatTrack::QMBarBeatTrack` constructs `new BarBeatTracker(inputSampleRate)` directly, and `BeatsAndBars.cs` constructs `QMBarBeatTrack` directly.

- Observation: VAMP 2.10 includes fixes after VAMP 2.5, including an off-by-one frame/time conversion correction in 2.8.
  Evidence: the upstream VAMP 2.10 changelog documents the correction. Therefore exact timestamp equality is not an appropriate upgrade acceptance rule.

- Observation: the full upstream QM 1.8 Windows project builds all QM plugins and external numerical libraries, which is broader than Vixen's current embedded implementation.
  Evidence: upstream `build/msvc/QMVampPlugins.vcxproj` includes every plugin and links `qm-dsp.lib` plus OpenBLAS-related libraries. Vixen's current `QMLibrary.vcxproj` compiles source directly and only requires Bar/Beat behavior.

- Observation: `ManagedPlugin::GetOutputDescriptors()` always threw before returning its descriptors.
  Evidence: the first regression run threw `ArgumentOutOfRangeException` at `ManagedPlugin.cpp:190`, where a newly allocated managed `List` was assigned by index instead of appended. The same capacity-versus-count error existed for non-empty bin-name lists.

- Observation: the direct C++/CLI reference can be built and exercised from `Vixen.Tests` using the repository's prescribed full-MSBuild test-target workflow.
  Evidence: `msbuild Vixen.sln -m -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:q` completed successfully on 2026-09-14; the following focused `dotnet test` command reported 2 passed and 0 failed tests.

- Observation: QM DSP 1.8.0 uses its own vendored KissFFT C sources for the Bar/Beat transform rather than OpenBLAS.
  Evidence: the isolated native x64 compile and link of the complete `BarBeatTrack` closure succeeded only after adding `ext/kissfft/kiss_fft.c`, `ext/kissfft/tools/kiss_fftr.c`, and the upstream `kiss_fft_scalar=double` definition; no BLAS library was linked.

- Observation: QM's `repoint.bat install` cannot run in the present staging environment because neither supported Standard ML runtime is installed.
  Evidence: `Get-Command sml,polyml` returned no commands. The Git-backed QM DSP dependency was cloned directly at the complete hash in `repoint-lock.json`, which is mechanically equivalent to Repoint's fetch for that dependency.

- Observation: C++/CLI cannot compile a `.c` translation unit with the project's `/clr` option.
  Evidence: the first integrated QMLibrary build failed with `D8045` for `kiss_fft.c`. Marking both unchanged KissFFT implementation files as `CompileAsCpp` allowed the Release x64 build to complete; the standalone native probe had already demonstrated their C++ compatibility.

- Observation: QM 1.8.0 exposes three additional optional Bar/Beat tuning parameters beyond `bpb`.
  Evidence: the wrapper test received `bpb`, `alpha`, `inputtempo`, and `constraintempo` from `BarBeatTracker::getParameterDescriptors()`. `BeatsAndBars.cs` still only reads and writes `bpb`, so its parameter interaction remains compatible.

- Observation: the existing modeless progress form was updated from the same UI thread that performed both PCM processing and QM's final beat calculation.
  Evidence: `GenerateFeatures` called `Show()`, then synchronously invoked `ManagedPlugin.Process` in a loop and `GetRemainingFeatures()` before returning. Windows marked the dialog as not responding when the final calculation occupied the UI thread; its last painted percentage did not describe that final phase.

## Decision Log

- Decision: Target VAMP Plugin SDK 2.10 and QM VAMP Plugins 1.8.0, using the QM 1.8.0-pinned `qm-dsp` revision for QM algorithm sources.
  Rationale: these are the latest official releases identified during discovery. Pairing QM plugin code with its locked dependency revision avoids mixing an algorithm implementation with an incompatible arbitrary dependency revision.
  Date/Author: 2026-09-14 / Codex.

- Decision: Preserve the existing `QMLibrary` C++/CLI assembly and its `QMBarBeatTrack`, `ManagedPlugin`, and `ManagedRealtime` managed surface.
  Rationale: `BeatsAndBars` consumes this managed surface. Upstream VAMP's `PluginWrapper` is a native host-side adapter and is not a replacement for Vixen's managed façade.
  Date/Author: 2026-09-14 / Codex.

- Decision: Keep the native dependency payload restricted to the transitive source closure for `BarBeatTrack`, rather than importing all QM plugins or adopting upstream's standalone plugin DLL project.
  Rationale: this preserves today's functional scope, deployment shape, and direct in-process instantiation behavior.
  Date/Author: 2026-09-14 / Codex.

- Decision: Capture behavioral output before source replacement and use that output as the primary compatibility oracle.
  Rationale: build success cannot demonstrate that beat labels, output indexes, or mark timing still mean the same thing to Vixen users.
  Date/Author: 2026-09-14 / Codex.

- Decision: Correct the existing managed-list population defect while adding the baseline test.
  Rationale: the public managed descriptor call must work to establish and preserve the wrapper contract. The correction changes no API and is limited to appending items to the lists that the method already creates.
  Date/Author: 2026-09-14 / Codex.

- Decision: Continue with the direct-source integration strategy and add QM DSP's KissFFT source closure; do not import OpenBLAS or the upstream all-plugin project.
  Rationale: the standalone native probe compiled, linked, and ran with the Bar/Beat source closure and no BLAS symbols. This preserves Vixen's existing one-plugin, no-extra-binary deployment model.
  Date/Author: 2026-09-14 / Codex.

- Decision: Clone QM DSP directly at its complete Repoint lock hash because the Repoint launcher needs an unavailable Standard ML runtime.
  Rationale: the lock file identifies a Git revision, so a detached checkout at that exact hash supplies the same immutable source material without choosing a branch or changing the dependency selection.
  Date/Author: 2026-09-14 / Codex.

- Decision: Compile the vendored KissFFT `.c` files as C++ within `QMLibrary.vcxproj`.
  Rationale: C++/CLI applies `/clr` to project compilation and rejects C translation units. The sources are C++-compatible, and this project-level mode change avoids modifying third-party source while retaining the direct-source architecture.
  Date/Author: 2026-09-14 / Codex.

- Decision: Add a `ManagedPlugin` destructor and finalizer that delete the owned `Vamp::Plugin` exactly once, and route all native calls through a null-checking accessor.
  Rationale: `QMBarBeatTrack` constructs and transfers ownership of one native `BarBeatTracker` to the managed wrapper. Releasing it in the wrapper prevents the native allocation from surviving managed object disposal, and throwing `ObjectDisposedException` after disposal avoids undefined native-pointer access.
  Date/Author: 2026-09-14 / Codex.

- Decision: Keep `ManagedRealtime` integer-width frame/time conversion unchanged.
  Rationale: VAMP 2.10 accepts the existing `long` frame and `unsigned int` sample-rate boundary values. The deterministic click-track baseline retained its timestamps exactly, so no compensating offset is warranted.
  Date/Author: 2026-09-14 / Codex.

- Decision: Preserve the synchronous `DoBeatBarDetection` call contract while running full-track feature generation on a worker thread behind a modal progress dialog.
  Rationale: the menu caller must not continue to modify the sequence until generated marks are available. A modal dialog keeps the UI message loop responsive, while `Progress<T>` marshals PCM progress and the finalization state back to the UI thread without unsafe `Application.DoEvents()` re-entrancy.
  Date/Author: 2026-09-14 / Codex.

## Outcomes & Retrospective

Milestones 1 through 4 are complete, and Milestone 5 has begun. `QmBarBeatTrackTests` directly exercises the C++/CLI wrapper with generated mono PCM, verifies the VAMP 2.10 descriptor set and input domain, retains the click-track feature baseline, and proves that disposing the managed wrapper releases its native plugin and prevents subsequent use. The deterministic 44.1 kHz click-track timestamps and feature counts remained unchanged. QM 1.8.0 adds optional `alpha`, `inputtempo`, and `constraintempo` descriptors, but `BeatsAndBars.cs` continues to require only `bpb`. Full-track generation now keeps the modal progress dialog responsive while native processing runs on a worker thread, and changes the dialog to a marquee finalization state before `GetRemainingFeatures()` performs the global tempo calculation. The full automated suite passed 935 tests. The operator has created and exported real-audio Beats and Bars mark collections for the final manual comparison; verifying the updated dialog with the affected audio remains required before Milestone 5 can complete.

## Context and Orientation

`src/Vixen.Modules/Analysis/QMLibrary/QMLibrary.vcxproj` builds `QMLibrary.dll`, a .NET 10 C++/CLI dynamic library. C++/CLI is C++ that exposes .NET-callable managed classes while also owning native C++ objects. The project uses the Visual C++ `v145` toolset and builds Win32 and x64 configurations, with Release output under `Release/Output` and Debug output under `Debug/Output`.

`src/Vixen.Modules/Analysis/BeatsAndBars/BeatsAndBars.csproj` references `QMLibrary.vcxproj`. Its `BeatsAndBars.cs` class constructs `QMLibrary.QMBarBeatTrack`, configures its `bpb` (beats per bar) parameter, streams mono PCM sample blocks into it, and interprets output indexes 0 through 3 as all beat features, bars, beat counts, and beat spectral-difference values. The generated `MarkCollection` data is user-visible application behavior.

`src/Vixen.Modules/Analysis/QMLibrary/QMBarBeatTrack.cpp` is the native bridge endpoint. Its constructor creates `BarBeatTracker`, the QM VAMP plugin implementation. `WrapperFiles/ManagedPlugin.*` delegates managed method calls to a `Vamp::Plugin*`. `WrapperFiles/ManagedRealtime.*` converts between managed time values and `Vamp::RealTime` values. These are Vixen-owned wrapper files, not upstream VAMP wrapper code.

`src/Vixen.Modules/Analysis/QMLibrary/QMFiles` now contains VAMP SDK 2.10, QM 1.8.0-derived `BarBeatTrack` sources, and the minimal QM DSP and KissFFT source closure in `base`, `dsp`, `maths`, and `ext`. The project compiles only the explicitly named `ClCompile` items in `QMLibrary.vcxproj`; files merely present below `QMFiles` are not automatically built.

The old `qm-vamp-plugins-1.7` upstream distribution was removed before this plan and must not be restored merely as part of this upgrade. It was an unused source archive. Its deletion is intentionally a separate commit from the implementation work.

## Plan of Work

### Milestone 1: Define and capture existing behavior

Create a focused regression harness before modifying vendored sources. The harness must exercise `QMBarBeatTrack` directly: instantiate it with a fixed sample rate, set `bpb` to 4 and at least one non-default supported value, initialize it using its preferred step and block size, submit deterministic mono PCM blocks, and collect `GetRemainingFeatures()`.

Use three small, repository-permitted WAV or PCM fixtures that demonstrate stable tempo, a tempo transition, and a short/partial final block. If licensed fixture audio cannot be committed, generate deterministic PCM click tracks in test code and additionally record a manual comparison using a licensed real-song fixture that is not committed. Store the expected feature index, label, timestamp, and values in a compact text or JSON baseline that makes review practical. The test must explicitly assert the identifier `qm-barbeattracker`, parameter `bpb`, and the four output descriptor identifiers in their existing order. This detects accidental adoption of another plugin or a change in how `BeatsAndBars` indexes the result.

Update `src/Vixen.Tests/Vixen.Tests.csproj` only as needed to reference the existing C++/CLI project. If a direct project reference is not supported by the test build, add a small test-only managed adapter in the BeatsAndBars project rather than changing product behavior. Any public or protected C# API introduced for testability must have XML documentation and must be reviewed using the repository `csharp-docs` skill. Prefer an internal test seam plus `InternalsVisibleTo` when it supplies the same coverage without expanding the production public API.

Run the baseline test against the current retained `QMFiles` source and preserve the output in this plan's Artifacts section. Run manual detection through the application for the same audio samples and record the mark collections and preview beat period.

Acceptance for this milestone is a reproducible baseline test and a written list of expected manual results. Do not start source replacement until both exist.

### Milestone 2: Stage and inventory exact upstream dependencies

Create a temporary directory outside the repository, such as `C:\\Temp\\vamp-qm-upgrade`. Clone VAMP Plugin SDK tag `vamp-plugin-sdk-v2.10` and QM VAMP Plugins tag `qm-vamp-plugins-v1.8.0`. In the QM checkout, run its `repoint.bat install` command to fetch the dependency source revisions defined by its `repoint-lock.json`. This lock file pins the matching `qm-dsp` source at commit `4d2a4a4e0c2dd0ddf07bf9d2a224ded912712714`.

Do not use an unpinned `master` branch. Record the tags, commit IDs, download date, source URLs, and licenses in a new provenance document under `src/Vixen.Modules/Analysis/QMLibrary/QMFiles` or a repository-standard third-party notices location selected after inspecting existing notices. Preserve the VAMP SDK license and QM GPL license with the vendored source as required by their terms.

Generate a dependency inventory beginning at QM 1.8.0 `plugins/BarBeatTrack.cpp`. Follow every local include and identify the minimal source/header closure needed by the Bar/Beat Tracker. Compare this list to the present `ClCompile` entries in `QMLibrary.vcxproj`. The inventory must distinguish source necessary to compile from headers necessary only for inclusion, and must identify any new third-party numerical dependency introduced by the matching QM DSP version.

Acceptance for this milestone is an auditable inventory and a successful isolated native compile experiment proving whether the minimal direct-source strategy remains viable. If upstream QM 1.8.0 requires linking OpenBLAS or another external library, do not silently import binaries. Record the evidence in this plan and create a small, separately reviewed decision point describing whether to retain the existing dependency style, build the library from source, or package a vetted binary.

### Milestone 3: Replace the narrow native source payload

In `src/Vixen.Modules/Analysis/QMLibrary/QMFiles`, replace `vamp-plugin-sdk-2.5` with VAMP SDK 2.10 source, named `vamp-plugin-sdk-2.10`. Replace `BarBeatTrack.cpp` and `BarBeatTrack.h` with their QM 1.8.0 versions. Replace only the QM DSP sources and headers identified by the milestone 2 inventory. Remove stale files from the old partial QM DSP copy only after confirming they are not headers included by the retained source and are not listed in the project.

Update `QMLibrary.vcxproj` and `QMLibrary.vcxproj.filters` together. Change all include paths and explicit source entries from `vamp-plugin-sdk-2.5` to `vamp-plugin-sdk-2.10`. Add, remove, or rename only the `ClCompile` and `ClInclude` items demanded by the inventory. Keep the build targets, output locations, target framework, and project GUID unchanged. Do not add the upstream `QMVampPlugins.vcxproj` to `Vixen.sln` and do not build the upstream `qm-vamp-plugins.dll`; that would change the current direct-in-process architecture.

Compile `QMLibrary` first for Release x64. Resolve compile errors by making source-compatible updates limited to the vendored source import, project settings, and Vixen wrapper. Do not change `BeatsAndBars.cs` during this milestone except for necessary testability changes already validated in milestone 1.

Acceptance for this milestone is a clean Release x64 build of QMLibrary using only the intended new source tree. Inspect the final linker inputs to prove no accidental dependency on a machine-installed VAMP or QM plugin library has been introduced.

### Milestone 4: Adapt and harden the managed wrapper

Review `QMBarBeatTrack.*`, `WrapperFiles/ManagedPlugin.*`, `WrapperFiles/ManagedRealtime.*`, `WrapperFiles/ManagedFeature.h`, `WrapperFiles/ManagedOutput.h`, and `WrapperFiles/ManagedParameter.h` against VAMP 2.10 headers. Preserve existing managed member names and semantics used by `BeatsAndBars.cs`: initialization, reset, parameter access, preferred block/step size, descriptor retrieval, processing, remaining features, and frame/time conversion.

Update include paths and native calls only where VAMP 2.10's API requires it. Do not replace `ManagedPlugin` with upstream `Vamp::PluginWrapper`; upstream's class serves native host-side plugin adapters and cannot provide the existing .NET-facing contract. Add deterministic native object disposal only if it can be done without breaking current caller ownership semantics. If a destructor/finalizer is added, ensure it deletes `m_plugin` exactly once, makes later native calls fail predictably, and has test coverage.

Pay special attention to `ManagedRealtime`. VAMP 2.8 corrected frame-to-time rounding, so preserve integer widths and conversions at the managed boundary and document any change that moves a timestamp by up to one sample. Do not compensate by introducing arbitrary timestamp offsets in `BeatsAndBars`.

Acceptance for this milestone is passing wrapper-level regression tests and a source review showing that every former `BeatsAndBars` call has an equivalent operation after the upgrade.

### Milestone 5: Validate end-to-end behavior and finalize vendoring records

Run the full regression test suite using the repository's full-MSBuild workflow, then run the three baseline audio cases through the actual Beat and Bar UI. Compare the resulting mark collection names, counts, labels, and timestamps against the pre-upgrade records. Classify each difference as an expected VAMP rounding change, an expected documented QM 1.8 behavior change, or a regression. Do not accept an unexplained difference.

If the QM 1.8 Bar/Beat implementation itself changes the expected result, update the baseline only after recording the before/after output and explaining the upstream cause in this plan's Decision Log. Preserve Vixen's mapping of feature 0 to all beats, 1 to bars, 2 to beat counts, and 3 to beat spectral difference unless the application code is intentionally redesigned in a separately approved change.

Remove the external temporary staging directory after the imported sources and provenance records have been verified. It is not a repository artifact. Complete the Outcomes & Retrospective section with validation results and residual risks.

## Concrete Steps

Run all repository commands from `C:\\Dev\\Vixen`.

First, confirm that the earlier deletion checkpoint is clean and establish a feature branch if normal team workflow requires one:

    git status --short
    git log -1 --oneline

Build the pre-upgrade baseline:

    msbuild Vixen.sln -m -t:Rebuild -p:Configuration=Release -p:Platform=x64

Expected result: MSBuild completes without errors and produces `Release\\Output\\QMLibrary.dll` and the BeatsAndBars module output. If the Visual C++ toolset is unavailable, install the repository-required Visual Studio C++ components before proceeding; do not replace the C++/CLI project with a C# project.

After adding the regression test, use the documented test workflow because `Vixen.Tests` depends transitively on C++/CLI projects:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\\"

Expected result: the first command builds `Vixen_Tests` successfully, and the second reports all tests passing. The new QMLibrary regression test must fail if its expected baseline is deliberately changed.

Stage upstream sources outside the working tree:

    New-Item -ItemType Directory -Force C:\\Temp\\vamp-qm-upgrade | Out-Null
    git clone --depth 1 --branch vamp-plugin-sdk-v2.10 https://github.com/vamp-plugins/vamp-plugin-sdk.git C:\\Temp\\vamp-qm-upgrade\\vamp-plugin-sdk
    git clone --depth 1 --branch qm-vamp-plugins-v1.8.0 https://github.com/c4dm/qm-vamp-plugins.git C:\\Temp\\vamp-qm-upgrade\\qm-vamp-plugins
    Set-Location C:\\Temp\\vamp-qm-upgrade\\qm-vamp-plugins
    .\\repoint.bat install

Expected result: both clones resolve the stated tags, and the QM checkout receives the dependencies described by its lock file. If `repoint.bat` cannot fetch a dependency, stop and record the exact failure; do not substitute a newer, unpinned dependency.

After each implementation milestone, build the narrow native project before the full solution:

    Set-Location C:\\Dev\\Vixen
    msbuild src\\Vixen.Modules\\Analysis\\QMLibrary\\QMLibrary.vcxproj -m -t:Rebuild -p:Configuration=Release -p:Platform=x64

Then build the full solution and run the test commands above. Finally, use the Vixen UI with each baseline audio input and save an export or screenshot of resulting mark collections alongside the implementation evidence, without committing copyrighted audio content unless its license permits redistribution.

## Validation and Acceptance

The implementation is accepted only when all of the following are true:

- QMLibrary builds for Release x64 with VAMP SDK 2.10 and QM VAMP Plugin 1.8.0-derived Bar/Beat sources.
- The full solution builds and the full test workflow passes.
- Wrapper tests prove that the managed API still exposes `qm-barbeattracker`, the `bpb` parameter, preferred sizes, and the four expected output descriptors in the established order.
- For each baseline input, end-to-end Beats and Bars output retains the intended collection names and feature mapping; beat labels and mark counts match unless an upstream behavior change is explicitly documented.
- Timestamp differences are no greater than one sample interval unless a documented QM 1.8.0 algorithm change explains and justifies a larger difference.
- The final output has no runtime dependency on a separately installed VAMP or QM plugin DLL.
- VAMP and QM source provenance and license notices are preserved with the vendored source.

## Idempotence and Recovery

The baseline build and test commands can be repeated without changing repository content. The source-staging commands can be retried after deleting only `C:\\Temp\\vamp-qm-upgrade`; never delete `QMFiles` until the staged dependency inventory is complete and the pre-upgrade baseline has been captured.

Perform source replacement in a dedicated commit after the prior 1.7-removal checkpoint. If the new source fails to build or changes behavior unexpectedly, revert that dedicated upgrade commit or restore only the changed `QMFiles` and project-file paths from it. Do not revert the earlier, independent removal commit. Keep baseline fixture changes separate from the vendor-source replacement when practical, so output regressions can be bisected.

If source inspection establishes that QM 1.8.0 cannot be embedded without a new numerical library, stop before adding a binary dependency. Record the finding and make a new decision-log entry with the available approaches and their packaging/licensing implications.

## Artifacts and Notes

Current native source inputs compiled by `QMLibrary.vcxproj` are:

    QMFiles\\vamp-plugin-sdk-2.5\\src\\vamp-sdk\\RealTime.cpp
    QMFiles\\BarBeatTrack.cpp
    QMFiles\\dsp\\onsets\\DetectionFunction.cpp
    QMFiles\\dsp\\onsets\\PeakPicking.cpp
    QMFiles\\dsp\\phasevocoder\\PhaseVocoder.cpp
    QMFiles\\dsp\\rateconversion\\Decimator.cpp
    QMFiles\\dsp\\signalconditioning\\DFProcess.cpp
    QMFiles\\dsp\\signalconditioning\\Filter.cpp
    QMFiles\\dsp\\signalconditioning\\FiltFilt.cpp
    QMFiles\\dsp\\tempotracking\\DownBeat.cpp
    QMFiles\\dsp\\tempotracking\\TempoTrackV2.cpp
    QMFiles\\dsp\\transforms\\FFT.cpp
    QMFiles\\maths\\MathUtilities.cpp

After Milestone 3, the equivalent source list is VAMP SDK 2.10's `src\\vamp-sdk\\RealTime.cpp`, QM 1.8.0's `BarBeatTrack.cpp`, the same selected QM DSP C++ implementation files at the locked revision, and KissFFT's `ext\\kissfft\\kiss_fft.c` plus `ext\\kissfft\\tools\\kiss_fftr.c`. The two KissFFT files are compiled as C++ because QMLibrary is a `/clr` project. `QMLibrary.vcxproj` defines `kiss_fft_scalar=double`, matching the upstream QM DSP Windows project. The old `vamp-plugin-sdk-2.5` tree and all unused legacy QM DSP areas were removed after their absence from the include closure and project compile items was confirmed. The prior source payload is recoverable from Git history; an external staging copy is retained temporarily at `C:\\Temp\\vamp-qm-upgrade\\removed-vixen-qmfiles`.

Milestone 3 validation completed on 2026-09-14:

    msbuild src\\Vixen.Modules\\Analysis\\QMLibrary\\QMLibrary.vcxproj -m -t:Rebuild -p:Configuration=Release -p:Platform=x64 -v:m

Result: succeeded and produced `Release\\Output\\QMLibrary.dll`. The build has existing C4642 WinForms generic-constraint warnings from `VampOutputCtrl` and `VampParamCtrl`, but no errors. A diagnostic build showed only Windows system libraries in `AdditionalDependencies`; it had no `qm-dsp.lib`, `libopenblas.lib`, or VAMP plugin-library input.

The project additionally compiles Vixen code `QMBarBeatTrack.cpp`, `stdafx.cpp`, `VampOutputCtrl.cpp`, `VampParamCtrl.cpp`, `WrapperFiles\\ManagedPlugin.cpp`, and `WrapperFiles\\ManagedRealtime.cpp`. This list is an initial inventory, not permission to carry it forward unchanged: milestone 2 determines the matching QM 1.8.0 source closure.

The automated Milestone 1 baseline uses 44,100 Hz mono PCM generated in `QmBarBeatTrackTests`. It is 32 bars at 120 BPM, uses a 25 ms decaying 880 Hz downbeat click followed by 1,760 Hz beat clicks, uses Vixen's present 16-bit-scale amplitudes, and ends with 257 samples to exercise final-block zero padding. Current expected output is: output 0 has 127 features (first label `2`, first/last timestamp 476/63,494 ms); output 1 has 31 features (first label `1`, 1,973/61,974 ms); output 2 has 127 features (first label `2`, 476/63,494 ms); and output 3 has 125 features (first label empty, 975/62,972 ms). The test validates all output timestamps and labels where Vixen consumes them. The operator also has an exported real-audio Vixen mark-collection baseline; retain the exact audio, Beats and Bars settings, collection names, and export for the post-upgrade comparison.

Focused validation completed on 2026-09-14:

    msbuild Vixen.sln -m -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:q
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\\" --filter "FullyQualifiedName~QmBarBeatTrackTests"

Result: `Passed: 2, Failed: 0, Skipped: 0`.

Upstream release facts used by this plan:

- VAMP SDK 2.5 local `CHANGELOG` records its 2013-05-08 release.
- VAMP SDK 2.10 released 2020-05-18. Its changes after 2.5 include frame/time rounding corrections and plugin-adapter fixes.
- QM VAMP Plugins 1.8.0 released 2020-01-27. Its published changes concern chromagram/key and tonal-change behavior; the Bar/Beat Tracker must still be baseline-tested rather than assumed unchanged.

## Interfaces and Dependencies

At the end of the work, these existing interfaces remain available to the BeatsAndBars module without a calling-code rewrite:

    QMLibrary::QMBarBeatTrack(float inputSampleRate)
    ManagedPlugin::Initialise(size_t channels, size_t stepSize, size_t blockSize)
    ManagedPlugin::SetParameter(System::String^ parameter, float value)
    ManagedPlugin::GetPreferredStepSize()
    ManagedPlugin::GetPreferredBlockSize()
    ManagedPlugin::Process(array<float>^ inputBuffer, ManagedRealtime^ timestamp)
    ManagedPlugin::GetRemainingFeatures()
    ManagedRealtime::frame2RealTime(long frame, unsigned int sampleRate)

`QMBarBeatTrack` must continue to wrap native `BarBeatTracker`, and the resulting native type must continue to derive from VAMP's plugin-side `Vamp::Plugin`. The embedded SDK target is VAMP Plugin SDK 2.10. The embedded QM plugin target is the Bar and Beat Tracker from QM VAMP Plugins 1.8.0 with the `qm-dsp` dependency revision pinned by that release. No dynamic plugin loading, external plugin registration, or additional user configuration is part of this plan.

Plan created 2026-09-14 because the unused QM 1.7 source archive had been removed and the remaining embedded analysis stack needs a controlled, behavior-preserving upgrade path.

Plan revision note (2026-09-14): Completed Milestone 1 with a direct C++/CLI wrapper regression test, generated PCM baseline, and a user-created Vixen export of Beats and Bars collections from licensed real audio for post-upgrade comparison. Fixed the existing `GetOutputDescriptors` list-population exception because descriptor verification could not otherwise run. The full MSBuild test target and the finalized focused baseline test ran successfully with 2 passed tests and zero failures. Completed Milestone 2 by staging the pinned sources, documenting their provenance and native closure in `QMFiles/UPSTREAM-PROVENANCE.md`, and proving an x64 direct-source compile/link without OpenBLAS. The Repoint launcher itself was unavailable because no Standard ML runtime is installed, so the Git-backed QM DSP lock revision was cloned directly. Completed Milestone 3 by importing that narrow closure, replacing the VAMP 2.5 tree, adding the matching KissFFT implementation files, and successfully rebuilding QMLibrary in Release x64. The C++/CLI project compiles the unchanged KissFFT `.c` sources as C++ because `/clr` does not permit C translation units. Completed Milestone 4 by preserving VAMP 2.10-compatible frame/time conversion, delegating input-domain discovery to the native plugin, adding deterministic native plugin disposal, and expanding wrapper regression coverage. The full MSBuild test target built successfully and the focused test run reported 3 passed and 0 failed tests. Began Milestone 5 by moving full-track analysis off the UI thread, exposing a marquee finalizing state while QM calculates remaining features, and preserving the synchronous menu contract. The full test suite reported 935 passed and zero failures. Manual validation with the affected track is still required.
