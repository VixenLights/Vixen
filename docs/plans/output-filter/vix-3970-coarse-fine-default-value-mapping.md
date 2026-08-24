# VIX-3970: Add opt-in default-value mapping to Coarse Fine Breakdown

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds. Maintain this document in accordance with `.agents/PLANS.md`.

## Purpose / Big Picture

The Coarse Fine Breakdown output filter converts one 16-bit lighting value into two 8-bit output channels: a coarse (high-byte) channel and a fine (low-byte) channel. After this work, a user can optionally configure one exact input value, normally the fixture's default or off value, to emit a distinct resting coarse/fine byte pair. The option is persisted with the filter, can be configured in its setup dialog, and can also be set by later code through documented module properties. With the option disabled—the persisted default—existing shows continue to split values exactly as they did before.

The feature is observable by configuring default input `0` and resting bytes `12` / `34`, passing a zero input through the filter, and observing coarse command `12` and fine command `34`; a nonzero input remains conventionally split. Disabling the checkbox makes zero again produce `0` / `0`.

## Progress

- [x] (2026-08-24 00:00 -05:00) Read `.agents/PLANS.md`, the entire current Coarse Fine Breakdown module, descriptor, output implementation, project file, and the current test project configuration.
- [x] (2026-08-24 00:00 -05:00) Identified the legacy source/file-name mismatch and the absence of both setup UI and focused output-filter tests.
- [x] (2026-08-24 00:00 -05:00) Wrote this implementation plan only; no production source, test source, or tracker record was changed.
- [ ] Update VIX-3970 with the final requirements, acceptance criteria, and test plan before modifying production code.
- [x] (2026-08-24 00:00 -05:00) Added persisted configuration, documented module configuration properties, immutable runtime snapshots, and the renamed source files; the affected Debug module build succeeded.
- [x] (2026-08-24 00:00 -05:00) Added the Catel WPF setup dialog with draft-only editing, inclusive decimal-range validation, and a single accepted-configuration rebuild; the affected Debug module build succeeded.
- [x] (2026-08-24 00:00 -05:00) Added focused behavior, persistence, programmatic-setter, and setup-view-model tests. The full Vixen test target built successfully and `dotnet test --no-build` reported 822 passed, 0 failed, 0 skipped.
- [ ] Build, run the full test sequence, perform manual setup verification, and reconcile VIX-3970 with exact validation results.

## Surprises & Discoveries

- Observation: the type names are already `CoarseFineBreakdownData` and `CoarseFineBreakdownOutput`, but their source files are respectively named `CoarseFineBreakdown.cs` and `CoarseFineByteBreakDownOutput.cs`.
  Evidence: the module project currently compiles those two files from `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/`.

- Observation: current command handling normalizes a `_16BitCommand` to a `double` and then converts it back to `ushort` before splitting. That extra round trip is unnecessary for command input and is not the requested behavior.
  Evidence: `CoarseFineBreakdownOutput.ProcessInputData(CommandDataFlowData)` obtains `command.CommandValue`, divides by `ushort.MaxValue`, and calls `Handle(double)`.

- Observation: the filter project is WinForms-only today but the repository has a current Catel WPF dialog convention with `ViewModelBase`, `TaskCommand`, `SaveAndCloseViewModelAsync`, and `CancelAndCloseViewModelAsync`.
  Evidence: `src/Vixen.Modules/Property/State/Setup/ViewModels/StateDefinitionNameDialogViewModel.cs` and its `StateDefinitionNameDialogView.xaml` use that pattern; the State project enables WPF, references `Catel.MVVM`, and references `WPFCommon`.

- Observation: `src/Vixen.Tests/Vixen.Tests.csproj` does not currently reference CoarseFineBreakdown, while it already contains `Xunit.StaFact` and references WPFCommon through other projects.
  Evidence: the test-project reference list has no CoarseFineBreakdown project entry, and central package management lists `Xunit.StaFact` 3.0.13.

- Observation: the focused module build succeeds after the POC runtime change; warnings originate in existing Vixen.Core files rather than this module.
  Evidence: `msbuild src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdown.csproj -m -restore -t:Rebuild -p:Configuration=Debug -p:Platform=x64 -v:m` exited with code 0 and reported warnings only for `IElementTemplate.cs`, `HardwareUpdateThread.cs`, and `ProgramExecutor.cs` in Vixen.Core.

- Observation: enabling WPF and adding Catel.MVVM/WPFCommon is sufficient for a Catel setup window in this output-filter project; the project can retain its existing WinForms setting while hosting the new WPF dialog.
  Evidence: the Debug module rebuild after adding `<UseWPF>true</UseWPF>`, `Catel.MVVM`, WPFCommon, the XAML window, and the view model exited with code 0.

- Observation: the internal setup view model can be tested with normal xUnit facts; its Catel command/close helpers do not require a WPF dispatcher for these staging and validation cases.
  Evidence: `CoarseFineBreakdownSetupViewModelTests` ran as part of the complete `dotnet test` invocation, which reported 822 passed, 0 failed, and 0 skipped without an STA-specific attribute or another test package.

## Decision Log

- Decision: Persist the four requested scalar values as `[DataMember]` properties on `CoarseFineBreakdownData`, initialized to disabled/zero defaults, and explicitly copy each one in `Clone()`.
  Rationale: scalar DataMembers preserve the configuration in a saved module, while explicit copying makes the persistence contract clear and remains correct if cloning no longer uses a shallow memberwise implementation. The disabled/zero defaults make absent fields in existing serialized filters behave exactly as before.
  Date/Author: 2026-08-24 / Codex

- Decision: Snapshot the data into an immutable internal output configuration when outputs are created; do not let `CoarseFineBreakdownOutput` read mutable module data during `Handle`.
  Rationale: configuration is changed only through module setters or one successful setup commit, both of which replace the two output objects. This provides a consistent coarse/fine pair for each output without locks or races between rendering and setup.
  Date/Author: 2026-08-24 / Codex

- Decision: Treat only exact equality with the canonical `ushort` input as a default-value match. Do not introduce a tolerance, range clamp, modulo wrap, or conversion beyond the existing RangeValue truncation.
  Rationale: a default mapping is a precise fixture configuration. The issue explicitly excludes fuzzy matching and behavior-changing input repair.
  Date/Author: 2026-08-24 / Codex

- Decision: Keep this issue limited to the output-filter module, its project dependencies, and its tests. Do not edit `src/Vixen.Application/Setup/ElementTemplates/IntelligentFixtureTemplate.cs` or another fixture/setup pipeline, even though it instantiates this module.
  Rationale: VIX-3970 supplies programmatic properties so a later fixture-flow issue can opt in deliberately; wiring it now would change unrelated configuration behavior.
  Date/Author: 2026-08-24 / Codex

- Decision: Treat the runtime change delivered in Milestone 2 as an initial proof of concept for the configuration and exact-remapping path.
  Rationale: the POC validates persisted defaults, programmatic setup, immutable output snapshots, and byte mapping independently before the WPF configuration surface and complete test matrix are added in later milestones.
  Date/Author: 2026-08-24 / User direction recorded by Codex

## Outcomes & Retrospective

The proof of concept now has automated coverage. Persisted defaults and documented programmatic configuration properties create immutable coarse/fine output snapshots, exact default-value substitution occurs before byte splitting, and a Catel setup dialog stages decimal inputs until a valid OK result applies all settings in one rebuild. Focused tests cover both input paths, exact matching, boundaries, setters, cloning, and dialog staging/validation; the complete test run reports 822 passing tests. Manual dialog validation and the tracker update remain outstanding. Update this section after each milestone with actual test totals, manual observations, and any scope adjustment.

## Context and Orientation

`src/Vixen.Modules/OutputFilter/CoarseFineBreakdown` is one module in Vixen's output-filter pipeline. An output filter receives either intent data (high-level lighting values) or command data (already-encoded commands) and exposes one or more outputs. This module has two outputs: coarse emits bits 15 through 8 of a 16-bit value; fine emits bits 7 through 0.

`CoarseFineBreakdownModule` owns the persisted module data and creates the outputs. Its `ModuleData` setter is the present configuration boundary: it casts the supplied model and calls `CreateOutputs()`. `CoarseFineBreakdownOutput` receives filtered `RangeValue<FunctionIdentity>` intents or `_16BitCommand` commands and currently appends `_8BitCommand` instances to a reusable commands collection. `CoarseFineBreakdownFilter` decides which intent states are range values; it is not part of the remapping rule and must remain unchanged except for references required by a file rename.

The current file `CoarseFineBreakdown.cs` declares `CoarseFineBreakdownData`; rename it to `CoarseFineBreakdownData.cs`. The current file `CoarseFineByteBreakDownOutput.cs` declares `CoarseFineBreakdownOutput`; rename it to `CoarseFineBreakdownOutput.cs`. These are source-name corrections only, with no namespace/type rename and no module descriptor TypeId change. `CoarseFineBreakdownDescriptor.TypeId` must remain `{7C1465DF-054C-4875-AB61-D7E7F5236897}`.

A *draft* is an independent in-memory copy of values shown in a dialog. Editing it must not alter the persisted `CoarseFineBreakdownData`. An *immutable output configuration* is a small object whose values cannot change after construction. Each rebuilt output receives its own immutable snapshot, so coarse and fine use the same settings even if a later setup action replaces the module's outputs.

## Plan of Work

### Milestone 1: Record the delivery contract in VIX-3970

Before source changes, update VIX-3970's description with the behavior in this plan: four persisted defaults, exact canonical-value matching, two accepted input paths, the Catel dialog bounds and cancel semantics, source-file corrections, and the explicit exclusion of `IntelligentFixtureTemplate` and other filters. Add the acceptance cases from the Validation section and name the focused test classes planned below. This makes the tracker independently actionable while retaining the original TypeId and scope boundary.

Do not transition the issue or claim completion at this point. If tracker wording exposes an ambiguity, resolve it in this plan's Decision Log before code is written.

### Milestone 2: Make configuration persistent, public, and atomic at the output boundary

Read `.agents/skills/csharp-docs/SKILL.md` before changing the public module/data APIs, then follow it for every public or protected member touched in this milestone. Edit and rename `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdown.cs` to `CoarseFineBreakdownData.cs`. Add `using System.Runtime.Serialization;` and define these `[DataMember]` public properties with initializers exactly as shown:

    [DataMember]
    public bool EnableDefaultValueMapping { get; set; } = false;

    [DataMember]
    public ushort DefaultInputValue { get; set; } = 0;

    [DataMember]
    public byte RestingCoarseValue { get; set; } = 0;

    [DataMember]
    public byte RestingFineValue { get; set; } = 0;

Give each public property XML documentation that explains its persisted role and units. Replace the `MemberwiseClone()` implementation with an explicit new `CoarseFineBreakdownData` initializer that copies all four fields. This change is intentionally explicit even though every member is scalar; future additions must not silently disappear from clones.

Edit `CoarseFineBreakdownModule.cs`. Change `HasSetup` to `true`. Add matching, fully XML-documented public properties named `EnableDefaultValueMapping`, `DefaultInputValue`, `RestingCoarseValue`, and `RestingFineValue`. Each getter reads `_data`; each setter updates its one persisted value and calls `CreateOutputs()` so programmatic callers immediately receive a new snapshot. Do not add validation, clamping, wrapping, or type conversion to the programmatic setters: their `bool`, `ushort`, and `byte` types already define the allowed domain. Preserve the `ModuleData` setter's rebuild behavior.

Add one private helper on the module, for example `ApplyConfiguration(CoarseFineBreakdownData configuration)`, which assigns all four values to `_data` and calls `CreateOutputs()` exactly once. The setup dialog must use this helper after acceptance instead of calling the four public setters, which would otherwise create four transient output arrays. It must never retain or replace `_data` with the draft object; only copy accepted scalar values into the existing persisted model.

Create an internal immutable configuration type in the CoarseFineBreakdown module, preferably a `readonly record struct CoarseFineBreakdownOutputConfiguration` in a new clearly named `.cs` file or beside the output type. It contains exactly the four values needed by rendering and exposes an `EffectiveValue(ushort canonicalValue)` method. It returns `canonicalValue` when mapping is disabled or it does not exactly equal `DefaultInputValue`; otherwise it returns `(ushort)((RestingCoarseValue << 8) | RestingFineValue)`. No locking, mutable shared state, tolerance, clamping, or arithmetic wrapping is permitted.

Rename `CoarseFineByteBreakDownOutput.cs` to `CoarseFineBreakdownOutput.cs` and change its constructor to receive both `bool highByte` and the immutable configuration. Store both as readonly fields. Update `CreateOutputs()` to make one snapshot from `_data` and pass that same snapshot to both the high and low outputs.

Refactor output conversion around a single helper such as `Handle(ushort canonicalValue)`. The command path must call it directly with `_16BitCommand.CommandValue`; it must not normalize to `double` and convert back. The RangeValue path must retain the existing conversion exactly: `(ushort)(rangeValue * ushort.MaxValue)`, including C# truncation, and then call the helper. The helper obtains `effectiveValue = configuration.EffectiveValue(canonicalValue)`, emits `(byte)(effectiveValue >> 8)` for coarse, and emits `(byte)(effectiveValue & 0xFF)` for fine. Preserve output-command clearing behavior for intent input and all existing handling of unsupported/null input.

### Milestone 3: Provide a staged Catel WPF setup dialog

Edit `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdown.csproj`. Add `<UseWPF>true</UseWPF>` while retaining any required existing Windows Forms compatibility. Add a `Catel.MVVM` package reference (the centrally managed `6.2.0` version already exists) and a project reference to `src/Vixen.Common/WPFCommon/WPFCommon.csproj`, using the repository's normal `Copy Local = No` / `Include Assets = None` metadata if that metadata is present in comparable module references. Do not add a package version locally or modify `Directory.Packages.props`.

Create `Setup/ViewModels/CoarseFineBreakdownSetupViewModel.cs` as an internal `Catel.MVVM.ViewModelBase`. Its constructor accepts the four current values and copies them into draft properties. Use a Boolean `EnableDefaultValueMapping` and decimal draft properties for `DefaultInputValue`, `RestingCoarseValue`, and `RestingFineValue`, so typed values can be validated before conversion. Set `DeferValidationUntilFirstSaveCall = false`, validate at construction, and use Catel field validation to reject values outside inclusive ranges: default input `0` through `65535`, resting coarse `0` through `255`, and resting fine `0` through `255`. Validation errors must make the OK command unavailable; disabled mapping does not waive these numeric bounds.

Expose an accepted-result value object or individual read-only result properties only after OK succeeds. The OK command must revalidate, convert valid decimals to `ushort`/`byte`, place the accepted values in that result, and close using Catel's save-and-close helper. The Cancel command must close using Catel's cancel-and-close helper without changing the result. The view model must not receive `_data`, the module, or an output reference, and therefore cannot mutate production configuration while a dialog is open.

Create `Setup/Views/CoarseFineBreakdownSetupView.xaml` as a `catel:Window`, with code-behind only for normal view construction/DataContext assignment and optional initial focus. Merge the WPFCommon theme resource dictionary as the State dialog does. Bind a checkbox to the draft `EnableDefaultValueMapping`, and bind three decimal-capable input controls to the decimal draft properties with `UpdateSourceTrigger=PropertyChanged`, Catel data-error validation, and visible validation feedback. Label the fields clearly as Default input value (0–65535), Resting coarse value (0–255), and Resting fine value (0–255). Provide OK and Cancel buttons bound to the view-model commands; the OK command's CanExecute state and the dialog's validation presentation must make invalid values impossible to accept.

Implement `CoarseFineBreakdownModule.Setup()` to construct the view model from the current `_data` values, open the WPF view modally, and return `false` on cancel/close. On accepted result only, copy the result through `ApplyConfiguration` once and return `true`. Verify that pressing Cancel, closing the window, and entering an invalid decimal leave `_data` and current output instances untouched. Do not bind the view directly to the module data and do not rebuild outputs when checkbox/text fields change.

### Milestone 4: Add focused automated coverage

Add `CoarseFineBreakdown.csproj` to `src/Vixen.Tests/Vixen.Tests.csproj` with the solution's normal project-reference metadata. Add an `InternalsVisibleTo` assembly attribute for `Vixen.Tests` to the CoarseFineBreakdown project, matching `State.csproj`, so tests can construct the internal output configuration/setup view model without making implementation types public.

Create a focused test folder such as `src/Vixen.Tests/OutputFilter/CoarseFineBreakdown/`. Use the module's public `ModuleData`, `Handle`, and `Outputs` surface to verify emitted `_8BitCommand` values; unwrap each `IDataFlowOutput.Data` as the appropriate command data flow data. Create helpers that initialize a module with a fresh `CoarseFineBreakdownData`, submit a single `_16BitCommand` or one compatible `RangeValue<FunctionIdentity>` intent, and obtain the high/low command bytes. Keep test fixtures independent so reusable output command lists cannot leak an earlier assertion.

Add behavior tests covering all of the following:

- Mapping disabled is backward compatible: command `0x1234` emits `0x12` / `0x34`, and zero emits `0` / `0` despite nonzero configured resting bytes.
- Command input uses its `ushort` value directly: mapped canonical input exactly equal to `DefaultInputValue` maps to the configured resting bytes; one lower and one higher do not map.
- RangeValue input converts with the existing truncating expression `(ushort)(value * ushort.MaxValue)`, then matches that canonical value exactly. Include a value that demonstrates truncation rather than rounding, plus a canonical exact match and a nonmatch.
- Resting coarse and resting fine are independently reflected in the high and low outputs, including a pair such as `0xA5` / `0x3C`.
- Boundary inputs `0` and `ushort.MaxValue`, and boundary resting pairs `0` / `0` and `byte.MaxValue` / `byte.MaxValue`, emit the expected byte pairs.
- Every new public module setter updates its associated persisted value and causes following output processing to use the changed snapshot. Assert each property getter round-trips the assigned value.
- `CoarseFineBreakdownData.Clone()` returns an independent data object containing all four new values. Mutating the original after cloning must not change clone scalar values.

Add unit tests for `CoarseFineBreakdownSetupViewModel` without opening a WPF window. They must assert constructor staging, inclusive valid boundaries, invalid negative/above-maximum decimal validation, and that invalid drafts cannot execute OK. Exercise Cancel after changing every draft property and prove the original values supplied by the caller are unchanged and that no accepted result is produced. Exercise a valid OK path and assert its accepted result contains the staged values; the module-level setup test or a narrow extracted apply helper test must prove that this accepted result triggers one output rebuild, not one rebuild per field. If dispatching Catel close helpers requires an STA-aware test, use the already-installed `Xunit.StaFact` attribute for only that test class; do not add another test package.

### Milestone 5: Validate, document results, and close the tracker loop

Build the affected module first, then use the project's required full test build and no-build test invocation from `C:\Dev\Vixen`:

    msbuild src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdown.csproj -m -restore -t:Rebuild -p:Configuration=Debug -p:Platform=x64 -v:m
    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(Get-Location)\\"

Expect each command to exit with code 0. Record exact test totals and investigate every new warning/error attributable to this work; existing unrelated baseline warnings may be documented but not used to mask a regression.

Manually create or open a profile containing a Coarse Fine Breakdown filter, open its setup dialog, enter default input `0`, resting coarse `12`, resting fine `34`, enable mapping, and click OK. Reopen the dialog to prove persistence. Feed the filter a zero value and verify coarse `12`, fine `34`; feed `0x1234` and verify `0x12`, `0x34`. Reopen, disable mapping, keep resting bytes nonzero, accept, and verify zero returns `0` / `0`. Try `-1`, `65536`, and `256` in their respective fields and verify validation blocks OK. Edit every field and Cancel, reopen the setup dialog, and verify the last accepted configuration is unchanged.

After successful validation, update VIX-3970 with delivered behavior, exact build/test commands and totals, manual results, and the explicit confirmation that `IntelligentFixtureTemplate`, the descriptor TypeId, and unrelated output filters were untouched. Add a tracker comment containing the same concise validation evidence. Reconcile all living sections of this plan, add dated discovery/decision entries if reality differs, and append a dated revision note. Do not create a commit unless the user explicitly requests one. When a milestone that changes repository files completes, invoke the project `commit-msg` skill and report this candidate message:

    feat(output-filter): map coarse-fine default values

## Concrete Steps

Run all commands from `C:\Dev\Vixen` in PowerShell. The following read-only checks are safe before edits and should be rerun after rename/configuration changes:

    rg -n "CoarseFineBreakdown|CoarseFineByteBreakDown|CoarseFineBreakdownOutput|HasSetup" src/Vixen.Modules/OutputFilter/CoarseFineBreakdown src/Vixen.Tests
    rg -n "class CoarseFineBreakdownData|TypeId|IntelligentFixtureTemplate" src/Vixen.Modules/OutputFilter/CoarseFineBreakdown src/Vixen.Application/Setup/ElementTemplates/IntelligentFixtureTemplate.cs

After renaming, expect no match for the old filename in repository file lists and no match for the obsolete spelling in source references:

    rg --files src/Vixen.Modules/OutputFilter/CoarseFineBreakdown
    rg -n "CoarseFineByteBreakDownOutput|CoarseFineBreakdown.cs" src Vixen.sln

After implementation, build and test with the three Milestone 5 commands. The expected concise result is an MSBuild `Build succeeded` outcome for both builds and a `dotnet test` result with zero failed tests. The exact passed count is intentionally not fixed because the test suite changes as this issue's focused cases are added; record the observed total in VIX-3970 and the plan.

## Validation and Acceptance

Acceptance requires the following observable behavior and automated evidence:

- Existing filters with no new serialized fields, or filters with `EnableDefaultValueMapping == false`, keep legacy byte splitting for both command and RangeValue inputs.
- With mapping enabled, only the canonical `ushort` equal to `DefaultInputValue` is substituted; immediately neighboring values are split unmodified.
- A command uses its raw 16-bit value. A RangeValue is converted once using existing multiplication-and-truncation semantics before exact comparison. No rounding behavior is introduced.
- A matched input emits `(RestingCoarseValue, RestingFineValue)` in that order, and an unmatched input emits its own high and low bytes.
- Zero and maximum 16-bit inputs, and zero and maximum byte resting values, produce correct results without special-case wrapping or clamping.
- The four documented module properties can configure a newly created module programmatically and cause subsequent output processing to reflect their values.
- The setup dialog validates decimal ranges 0–65535, 0–255, and 0–255; it blocks OK for invalid values; accepted settings persist and rebuild outputs once; Cancel and window close mutate neither persisted data nor live outputs.
- Focused tests pass along with the complete Vixen test suite. Manual dialog and input checks in Milestone 5 confirm the UI behavior.
- The descriptor TypeId is byte-for-byte unchanged, `IntelligentFixtureTemplate.cs` is unmodified, and no unrelated output-filter directory is changed.

## Idempotence and Recovery

The changes are additive and safe to build repeatedly. The source-file renames should be performed with version-control-aware rename operations so history is preserved; if a rename is interrupted, restore the original file name before retrying so the project never contains two classes with the same type. The dialog creates a fresh draft each time, so canceling, closing, or reopening it is safe and does not contaminate module state. If a test reveals a changed legacy conversion result, revert only the new conversion refactor to the explicit required paths: command input direct to `ushort`, RangeValue input through the exact existing cast; do not mask the failure by adding tolerance or clamping.

If WPF test execution exposes a Catel dispatcher/lifecycle limitation, first keep the setup tests view-model-only and use `Xunit.StaFact` from the existing dependency. Do not substitute a real modal UI test with arbitrary sleeps. Record the limitation and use the required manual dialog validation before declaring the issue complete.

## Artifacts and Notes

The desired value transformation is:

    command input: canonical = command.CommandValue
    RangeValue input: canonical = (ushort)(rangeValue * ushort.MaxValue)

    effective = EnableDefaultValueMapping && canonical == DefaultInputValue
        ? (ushort)((RestingCoarseValue << 8) | RestingFineValue)
        : canonical

    coarse command = (byte)(effective >> 8)
    fine command   = (byte)(effective & 0xFF)

For example, enabling mapping with `DefaultInputValue = 0x0000`, `RestingCoarseValue = 0x0C`, and `RestingFineValue = 0x22` yields coarse `0x0C` and fine `0x22` only for canonical zero. A canonical `0x1234` always emits `0x12` and `0x34` unless it is explicitly selected as the default input. With mapping disabled, both values follow conventional byte splitting regardless of the resting byte settings.

The setup-state ownership must be:

    persisted CoarseFineBreakdownData -> copied to view-model draft when Setup opens
    user edits draft -> validation only; no module/output mutation
    accepted result -> module copies four scalars, calls CreateOutputs once
    cancel/close -> discard draft; module data and outputs remain unchanged

## Interfaces and Dependencies

At completion, `VixenModules.OutputFilter.CoarseFineBreakdown.CoarseFineBreakdownData` has the four public `[DataMember]` properties named `EnableDefaultValueMapping`, `DefaultInputValue`, `RestingCoarseValue`, and `RestingFineValue`; their types/defaults are respectively `bool = false`, `ushort = 0`, `byte = 0`, and `byte = 0`. `Clone()` explicitly preserves them.

`CoarseFineBreakdownModule` publicly exposes properties with those same names/types, documented with XML comments. It overrides `HasSetup` as `true` and implements the normal `Setup()` result contract: `true` only after a valid accepted configuration, `false` on Cancel/close. Its outputs are created from immutable configuration values and never read mutable data at render time.

The CoarseFineBreakdown project has WPF enabled and uses the centrally-versioned `Catel.MVVM` package plus the existing WPFCommon project for Catel setup-window behavior and shared theme resources. The internal setup view model and immutable output configuration remain internal; `InternalsVisibleTo("Vixen.Tests")` is only for focused test access and does not expand the module's public API. The project continues to use the same namespace and descriptor TypeId. No `IntelligentFixtureTemplate` interface or fixture-flow interface changes in this issue.

Revision note (2026-08-24): Created the plan from the VIX-3970 handoff after inspecting the existing output filter, current Catel WPF dialog conventions, test project, and repository plan requirements. No implementation was performed.

Revision note (2026-08-24): Implemented Milestone 2 as the initial POC. The module now persists default mapping configuration, rebuilds immutable output snapshots after programmatic changes, and applies exact mapping for command and RangeValue inputs. The module Debug build succeeds; setup UI and tests remain later milestones.

Revision note (2026-08-24): Implemented Milestone 3. The module now enables WPF, references Catel.MVVM and WPFCommon, and supplies an internal Catel setup window/view model that validates staged decimal values. Valid OK results copy all settings into the persisted model and rebuild outputs once; Cancel/close leaves the model untouched. The module Debug build succeeds.

Revision note (2026-08-24): Implemented Milestone 4. Added the CoarseFineBreakdown project reference and internal-test visibility to Vixen.Tests, then added focused module/output and setup-view-model tests. The full Vixen test build and no-build test execution complete with 822 passed, 0 failed, and 0 skipped.
