# VIX-3988: Import Pangolin Beyond marks into the Marks Docker

This ExecPlan is a living document. Maintain its `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` sections as work proceeds. Follow `.agents/PLANS.md` from the repository root when revising this document.

## Purpose / Big Picture

Timed Sequence Editor users can already export mark collections in Pangolin Beyond's CSV shape, but cannot bring that CSV back into the Marks Docker. After this work, a user can choose `Pangolin Beyond` from the existing import-type dialog, select a CSV exported by Vixen, and either retain the source colors as separate mark collections or replace every source color with one selected color. The user can see the imported marks immediately in the Marks Bar; invalid files and cancelled choices leave the current sequence unchanged.

## Progress

- [x] (2026-08-18 00:00Z) Inspected the existing import dialog, import/export service, Marks Docker dispatch, mark model, unique-name helper use, ColorPicker use, and test-project visibility.
- [x] (2026-08-18 15:46Z) Updated VIX-3988 with the user-facing Summary, Scope, Acceptance Criteria, and Validation Plan.
- [x] (2026-08-18 15:49Z) Added the Pangolin Beyond dialog selection, Marks Docker dispatch, and documented compile-safe service entry point; the affected Release build succeeded.
- [x] (2026-08-18 16:00Z) Added pure internal parser/materializer types and seven focused tests; the x64 test target build and `PangolinBeyondMarkImportTests` pass.
- [x] (2026-08-18 16:20Z) Implemented the file, decision, color-picker, atomic error, unique-name, and default-selection workflow; the focused suite now passes 13 tests.
- [x] (2026-08-18 16:34Z) User confirmed the full build and full unit suite pass; verified Vixen Beyond export/import round trip and a Beyond-exported test file; posted the results to VIX-3988 comment 40367.

## Surprises & Discoveries

- Observation: The existing import-type dialog is a WinForms `BaseForm`, even though the Marks Docker view model is a Catel/WPF type.
  Evidence: `src/Vixen.Modules/Editor/TimedSequenceEditor/MarkCollectionImportDialog.cs` exposes checked WinForms radio buttons, and `MarkDockerViewModel.ImportCollection()` creates it directly.

- Observation: The required test friend-assembly relationship already exists.
  Evidence: `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditor.csproj` declares `InternalsVisibleToAttribute` for `Vixen.Tests`, and `src/Vixen.Tests/Vixen.Tests.csproj` already references the Timed Sequence Editor project.

- Observation: Creating `new Mark(startTime)` deliberately gives the mark a 450 ms duration.
  Evidence: `src/Vixen.Modules/App/Marks/Mark.cs` assigns `TimeSpan.FromMilliseconds(450)` in its `Mark(TimeSpan)` constructor.

- Observation: `MarkCollection.AddMarks` enumerates its input once to assign parents and again to add it.
  Evidence: The initial focused factory test passed a lazy projection and observed two resulting marks with `Parent == null`; materializing the `Mark` list before the single `AddMarks` invocation made all seven focused tests pass.

- Observation: `MessageBoxForm` presents its `OK` button as “YES” but retains `DialogResult.OK`.
  Evidence: `MessageBoxForm.cs` changes `buttonOk.Text` for `YesNoCancel`, while its designer assigns `buttonOk.DialogResult = DialogResult.OK`; treating only `DialogResult.Yes` caused grouped imports to return without adding collections.

## Decision Log

- Decision: Parse and materialize through internal, UI-free helpers, while keeping open-file dialogs, prompts, messages, and mutations in `MarkImportExportService`.
  Rationale: The legacy service already owns all of those UI concerns. Separating only deterministic work lets unit tests cover success and failure behavior without WinForms automation or a new public service API.
  Date/Author: 2026-08-18 / Codex

- Decision: Treat the required `ImportPangolinBeyondMarks(ObservableCollection<IMarkCollection>)` service entry point as the one requested integration member; all new supporting records, enum values, parser, and materializer remain `internal`.
  Rationale: The handoff explicitly requires that service dispatch but prohibits broadening the public API. Existing test visibility makes internals directly testable.
  Date/Author: 2026-08-18 / Codex

- Decision: Preserve source-color group order by first row occurrence rather than sorting colors or names.
  Rationale: This makes grouped imports stable and preserves the order users see in the CSV. A dictionary for lookup plus a separate ordered color list avoids relying on dictionary enumeration order.
  Date/Author: 2026-08-18 / Codex

- Decision: Reject a malformed file before asking any grouping/color question or adding a collection.
  Rationale: This guarantees invalid-file atomicity and avoids prompting the user for an import that cannot succeed.
  Date/Author: 2026-08-18 / Codex

- Decision: Expose an internal append seam that accepts already parsed records and a nullable import mode.
  Rationale: The WinForms dialogs must stay in the legacy service, but the nullable mode makes cancellation/no-mutation, unique-name insertion, and default-selection behavior directly testable without automating dialogs.
  Date/Author: 2026-08-18 / Codex

- Decision: Map the legacy dialog's `DialogResult.OK` to the grouped (Yes) import choice.
  Rationale: The existing `MessageBoxForm` labels its OK button “YES” without changing its DialogResult. Accepting both `OK` and `Yes` keeps the user-visible choice correct and makes the mapping explicit in tests.
  Date/Author: 2026-08-18 / Codex

## Outcomes & Retrospective

All milestones are complete. Users can import valid Pangolin Beyond CSV mark files into the Marks Docker either as collections grouped by source color or as one replacement-color collection. The import guards against malformed input and cancellation without changing a sequence. Automated focused coverage passes 13 tests; the user also confirmed the full build and full unit suite pass. Manual validation confirmed a Vixen Beyond export can be imported back into Vixen and a test file exported by Beyond imports successfully. VIX-3988 comment 40367 records the validation results. No remaining gaps are known.

## Context and Orientation

`src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkDockerViewModel.cs` is the command owner for the Marks Docker toolbar. Its private `ImportCollection()` method displays `MarkCollectionImportDialog` and dispatches each selected legacy file format to `MarkImportExportService`.

`src/Vixen.Modules/Editor/TimedSequenceEditor/MarkCollectionImportDialog.cs` and its `.Designer.cs` file define the existing WinForms import format selector. It currently exposes a Boolean property for each radio button. Add one radio button and one read-only Boolean property; do not introduce a separate dialog or migrate this legacy dialog to WPF.

`src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Services/MarkImportExportService.cs` owns the existing import and export workflows. It retains the last selected folder, creates `OpenFileDialog` and `MessageBoxForm` instances, logs through NLog, has `CreateNewCollection`, `AddUniqueCollection`, and `SetDefaultCollection` helpers, and already writes the Beyond CSV on the `MarkExportType.PangolinBeyond` branch. Its exporter writes the exact header `#,Name,Start,Color`, then rows `M<number>,<text>,<time>,<six BGR hexadecimal digits>`; it uses `mm:ss.fff` below one hour and `hh:mm:ss.fff` at one hour or later.

Pangolin's CSV color field here is BGR: the first two hexadecimal digits are blue, the next two are green, and the final two are red. Thus `112233` represents RGB `#332211`, created as `Color.FromArgb(0x33, 0x22, 0x11)`. Parsing must use `CultureInfo.InvariantCulture`, never the workstation culture. It must accept only the two exact timestamp shapes written by Vixen: `mm:ss.fff` and `hh:mm:ss.fff`.

`src/Vixen.Modules/App/Marks/MarkCollection.cs` provides `AddMarks(IEnumerable<IMark>)`, which assigns parents, bulk-adds, orders marks, and raises one collection notification. Each newly imported collection must call it once, not use repeated `AddMark` calls. `AddUniqueCollection` in the import service must remain the only route used to append imported collections so existing names are not duplicated. `SetDefaultCollection` must run only if the target has no default collection after import.

`Common.Controls.ColorManagement.ColorPicker.ColorPicker` is the existing WinForms color picker. It accepts its color as `XYZ` and yields an RGB color via `picker.Color.ToRGB().ToArgb()`, as shown in `MarkCollectionViewModel.PickColor()`. Use this picker only for the one-collection (No) response.

## Plan of Work

### Milestone 1: Record the approved issue contract in Jira

Before implementation, update VIX-3988 through the repository Jira workflow. State that Vixen-exported Beyond CSV files have the fixed header and BGR colors; malformed headers, fields, timestamps, or colors show `Pangolin Beyond Import Error`, log an NLog error, and add nothing; Yes creates source-color collections, No opens the existing picker for one replacement-color collection, and Cancel changes nothing. Include automated parser/materializer coverage and the manual import scenarios from this plan. Use the project `jira` skill before modifying the tracker. If tracker access is unavailable, record the exact failure under `Surprises & Discoveries`, continue local work, and leave the final tracker update pending.

### Milestone 2: Expose and route the new import selection

Read all of `MarkCollectionImportDialog.cs`, `MarkCollectionImportDialog.Designer.cs`, `MarkDockerViewModel.cs`, and the relevant import-service methods before editing. Add a `radioPangolinBeyond` radio button labeled `Pangolin Beyond` to the existing mutually exclusive group. Place it after the current choices, increase the form client/minimum height enough that it and the existing OK/Cancel buttons do not overlap, and keep the existing default selection and tab order behavior coherent. Add `public bool IsPangolinBeyondSelection => radioPangolinBeyond.Checked;` to the dialog, with XML documentation consistent with the existing public dialog properties because this is a public API modification; use the project `csharp-docs` skill while implementing this public-member change.

In `MarkDockerViewModel.ImportCollection()`, add an independent selection branch matching the existing style:

    if (aDialog.IsPangolinBeyondSelection)
        MarkImportExportService.ImportPangolinBeyondMarks(MarkCollections);

Do not convert the existing independent `if` statements to `else if` or change another import format's behavior. At the end of this milestone, choosing the new radio button reaches the new service method (which may initially be a compile-safe stub only while the next milestone is in progress).

### Milestone 3: Add deterministic Beyond parsing and materialization seams

Create separate files under `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Services/`:

- `PangolinBeyondMarkRecord.cs`: an `internal` immutable record/record struct containing the parsed text, start time, and `System.Drawing.Color`.
- `PangolinBeyondImportMode.cs`: an `internal enum` with `GroupByColor` and `SingleCollection` values.
- `PangolinBeyondMarkParser.cs`: an `internal static` pure parser with a `TryParse`-style API accepting CSV text or lines and returning a complete record list plus an error description. It must not open dialogs, log, mutate collections, or create marks.
- `PangolinBeyondMarkCollectionFactory.cs`: an `internal static` pure materializer accepting parsed records, an import mode, and the selected replacement color for one-collection mode. It returns detached `MarkCollection` instances and must not mutate the target observable collection.

The parser must reject an empty file, a header that is not exactly `#,Name,Start,Color`, a row with anything other than four comma-separated columns, an invalid timestamp, and a color other than exactly six ASCII hexadecimal digits. Do not add CSV quoting/escaping behavior: Vixen's exporter removes commas from text, so precisely four direct comma fields is the supported contract. Preserve text verbatim from the second field, accept LF or CRLF input, and identify the failing line in the returned error description. Use `TimeSpan.TryParseExact` with invariant culture and exactly the format strings `mm\\:ss\\.fff` and `hh\\:mm\\:ss\\.fff`; do not accept locale separators, unpadded values, seconds-only values, or alternate fractions. Convert the color by parsing byte pairs in B, G, R order and return `Color.FromArgb(red, green, blue)`.

The materializer's grouped mode must scan records in input order, collect every record sharing each source color into one collection, and return collections in first-color-occurrence order. Each returned collection is named `Beyond Marks - #RRGGBB` using normal RGB hexadecimal display, has `Decorator.Color` equal to its source color, sets `ShowMarkBar = true`, and calls `AddMarks` exactly once. Its single mode returns one `Beyond Marks` collection, applies the caller's replacement color to the decorator, sets `ShowMarkBar = true`, and builds marks from all parsed records in input order using that collection color. For every mark, construct `new Mark(record.StartTime) { Text = record.Text }`; do not assign `Duration`, so it retains the model's 450 ms default.

Create `src/Vixen.Tests/Sequencer/PangolinBeyondMarkImportTests.cs`. Test these internals directly through the established friend-assembly access; no modal dialog test is required. Cover a valid two-row parse; the BGR `112233` to RGB `#332211` conversion; `mm:ss.fff` and `hh:mm:ss.fff` timestamps; header, column-count, time, and color failures; group mode's color grouping, input-first ordering, RGB names, source decorator colors, mark text/start time, 450 ms durations, and one `AddMarks`-equivalent result per collection; single mode's one name, one replacement color, and all marks; and materialization with an empty/invalid parse result only when parser contract permits it. Add a test around the service-level collection-append seam only if it can be exposed internally without UI, asserting unique suffixes and default behavior.

### Milestone 4: Implement the legacy service workflow atomically

Add `ImportPangolinBeyondMarks(ObservableCollection<IMarkCollection> collections)` to `MarkImportExportService`. Follow existing open-dialog conventions: initialize from `_lastFolder`, filter `.csv` files plus all files, return on cancellation, and update `_lastFolder` only after a file is selected. Read the entire selected file using the default text reader behavior used by nearby imports, then call the pure parser before displaying any choice dialog or changing `collections`.

If parsing fails, construct a specific NLog error containing the parser error and file path (include an exception only if one occurred), show `MessageBoxForm` with the title exactly `Pangolin Beyond Import Error`, and return. Do not add a collection, change an existing collection, set a default, or show the grouping question on this path. Also catch read/unexpected errors around this workflow, log them, show that same title, and preserve the same no-mutation guarantee.

After a successful parse, ask with Yes/No/Cancel using the existing `MessageBoxForm`/WinForms dialog conventions and exact prompt text `Create a Mark Collection for each Beyond color?`. Map Yes to `PangolinBeyondImportMode.GroupByColor`. Map No to `SingleCollection`, then show the existing `ColorPicker`; if it returns anything other than `DialogResult.OK`, return without mutation. Convert its selected XYZ color with `picker.Color.ToRGB().ToArgb()`. Map Cancel, form-close, or any other result to an immediate no-mutation return. Dispose the ColorPicker with `using` so its message filter is removed.

Only after a successful mode and, where required, color selection, call the materializer. Then append every fully constructed collection with `AddUniqueCollection(collections, collection)`, retaining the materializer order. Once all additions are complete, call `SetDefaultCollection(collections)` only if `collections.Any(x => x.IsDefault)` is false. This sequencing ensures failed parse, prompt cancellation, and picker cancellation cannot produce partial imports. Do not add a new dialog, do not move UI behavior out of the import service, and do not alter the existing Beyond export branch.

Add or extend a narrow internal orchestration/helper seam if necessary to test append/default/cancellation state deterministically, but keep it `internal` and UI-free. Tests must demonstrate that mode cancellation leaves a pre-populated collection collection reference-identical and unchanged, that picker cancellation has the same result, that invalid input leaves it unchanged, and that successful additions use unique names if `Beyond Marks` or a grouped RGB name already exists. If keeping cancellation purely in the UI method makes a direct unit test impossible, test the no-mutation property by verifying parser failure and materializer non-execution, then manually verify the dialogs in Milestone 5; record that boundary in the Decision Log.

### Milestone 5: Validate the user flow and close the tracker loop

Use a small Vixen-exported CSV such as the following manual fixture (the first field is an identifier and is validated only by its column position; the export currently writes `M1`, `M2`, and so on):

    #,Name,Start,Color
    M1,Intro,00:01.250,112233
    M2,Chorus,01:02:03.004,A0B0C0
    M3,Again,00:03.500,112233

Open a timed sequence, open the Marks Docker import action, choose `Pangolin Beyond`, and select the fixture. For Yes, observe two visible Marks Bar collections ordered `Beyond Marks - #332211` then `Beyond Marks - #C0B0A0`, with the first containing Intro and Again and colors matching their names. For No, select a visibly distinctive replacement color in the existing picker and observe one visible `Beyond Marks` collection with all three marks and that replacement color. In both cases, check that a mark duration remains 450 ms and an existing default collection stays default; when importing into a sequence with no default, exactly one default is assigned.

Repeat with an invalid header, wrong column count, invalid timestamp, and invalid color. Each must log an error and show `Pangolin Beyond Import Error`; record the collection count before opening the dialog and confirm it is unchanged afterwards. Repeat the valid flow but choose Cancel at the Yes/No/Cancel prompt and Cancel in ColorPicker after choosing No; neither route may add anything. Finally, import a fixture into a sequence already holding `Beyond Marks` and a matching grouped name and confirm `AddUniqueCollection` supplies its standard unique suffixes.

Build tests using full Visual Studio MSBuild, because the test graph contains C++/CLI dependencies. Then run the focused class and the whole test project without rebuilding, from `C:\Dev\Vixen`:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:\Dev\Vixen\\" --filter FullyQualifiedName~PangolinBeyondMarkImportTests
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:\Dev\Vixen\\"

Expect each completed test command to report zero failed tests. Run `git diff --check` before handoff. Update VIX-3988's wording if implementation discoveries changed its requirements, then add a Jira comment containing actual build, focused-test, full-suite, and manual-validation results. Update this plan's Progress, Outcomes & Retrospective, Artifacts, and revision note with real evidence.

## Concrete Steps

All commands run from `C:\Dev\Vixen`.

1. Before editing, read the project-specific documentation skill because this work changes the public `MarkCollectionImportDialog` API:

       Get-Content -Raw .agents\skills\csharp-docs\SKILL.md

2. Locate the current boundaries and ensure no unrelated work is overwritten:

       git status --short
       rg -n -C 4 "ImportCollection\(|ImportVixen3Beats|ExportMarkCollections|PangolinBeyond|AddUniqueCollection|SetDefaultCollection" src\Vixen.Modules\Editor\TimedSequenceEditor -g "*.cs"
       rg -n "MarkCollectionNameService|InternalsVisibleTo" src\Vixen.Modules\Editor\TimedSequenceEditor src\Vixen.Tests -g "*.cs" -g "*.csproj"

3. Complete Milestones 2 through 4 with tabs and LF line endings. Review only expected files:

       git diff --check
       git diff -- src/Vixen.Modules/Editor/TimedSequenceEditor/MarkCollectionImportDialog.cs src/Vixen.Modules/Editor/TimedSequenceEditor/MarkCollectionImportDialog.Designer.cs src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkDockerViewModel.cs src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Services src/Vixen.Tests/Sequencer/PangolinBeyondMarkImportTests.cs

4. Run the Milestone 5 commands. A successful test summary contains:

       Passed!  - Failed:     0, Passed:     <count>, Skipped:     0

5. Record actual commands, counts, manual results, and Jira result in this plan rather than copying example results.

## Validation and Acceptance

The feature is accepted when the import dialog has a selectable Pangolin Beyond format and the user can import a Vixen-exported Beyond CSV through it. A file with the exact header and valid four-field rows must parse invariantly, preserve mark text and start time, translate BGR colors correctly, preserve the 450 ms default duration, and create visible collections.

Yes must create one `Beyond Marks - #RRGGBB` collection for each source RGB color in first-occurrence order. No must show the existing ColorPicker and, after confirmation, create one `Beyond Marks` collection whose decorator is the selected replacement color. The collections must get unique names, marks must be bulk-added once per collection, and default selection must only be supplied when absent. Cancel at either decision point must leave collections untouched.

For malformed headers, non-four-column rows, invalid timestamps, or invalid colors, the application must log an NLog error, display `Pangolin Beyond Import Error`, and make no collection/default changes. Automated tests must cover parsing, BGR conversion, both time forms, both materialization modes, grouping order, uniqueness/default append behavior where exposed, cancellation/no-mutation seams, and invalid-input atomicity. The focused and full `Vixen.Tests` commands must have zero failures, and the manual scenarios in Milestone 5 must pass.

## Idempotence and Recovery

Parser and materializer helpers are pure: rerunning them with the same input produces detached equivalent results and does not affect a sequence. The service mutates only after all parse and user decisions have succeeded. Retrying after a failed parse or a cancellation is safe because nothing was added. Retrying a successful import intentionally adds another import, but `AddUniqueCollection` prevents duplicate collection names.

If implementation must be backed out, remove only the new dialog option, dispatch branch, service import entry point, Beyond helper files, and focused tests after confirming their exact paths with `git status --short`. Do not change or delete existing exports, other import formats, or user collections. If test build prerequisites are missing, leave source unchanged, capture the exact MSBuild failure, and retry on a machine with the Visual Studio C++ toolset rather than modifying project references.

## Artifacts and Notes

The fixed round-trip fixture is:

    #,Name,Start,Color
    M1,Intro,00:01.250,112233
    M2,Chorus,01:02:03.004,A0B0C0
    M3,Again,00:03.500,112233

The expected grouped result is:

    Beyond Marks - #332211, decorator RGB #332211, marks Intro at 00:01.250 and Again at 00:03.500
    Beyond Marks - #C0B0A0, decorator RGB #C0B0A0, mark Chorus at 01:02:03.004

Both imported collections set `ShowMarkBar = true`. Every listed mark has a 450 ms duration unless a future product requirement explicitly adds Beyond duration serialization.

Milestone 3 validation evidence:

    msbuild Vixen.sln -m -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    Build succeeded with zero errors.

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="C:\Dev\Vixen\\" --filter FullyQualifiedName~PangolinBeyondMarkImportTests
    Passed! - Failed: 0, Passed: 13, Skipped: 0, Total: 13.

Milestone 5 validation evidence supplied by the user:

    Full build: passed.
    Full unit test suite: passed.
    Manual: exported marks in Beyond format and re-imported the same file successfully.
    Manual: imported a test file exported by Beyond successfully.
    Jira: validation recorded in VIX-3988 comment 40367.

## Interfaces and Dependencies

No package, solution, or project-reference change is expected. Use existing `System.Globalization.CultureInfo.InvariantCulture`, `System.Drawing.Color`, `System.Windows.Forms` dialogs, `NLog`, `VixenModules.App.Marks.Mark`, `MarkCollection`, `MarkCollectionNameService`, `MessageBoxForm`, and `Common.Controls.ColorManagement.ColorPicker.ColorPicker` dependencies.

At completion, the externally reachable integration additions are limited to:

    // MarkCollectionImportDialog.cs
    public bool IsPangolinBeyondSelection => radioPangolinBeyond.Checked;

    // MarkImportExportService.cs
    public static void ImportPangolinBeyondMarks(ObservableCollection<IMarkCollection> collections);

The following must remain internal to `Module.Editor.TimedSequenceEditor` and exist in separate source files so `Vixen.Tests` can test them via the existing friend-assembly declaration:

    internal record/record struct PangolinBeyondMarkRecord(string Text, TimeSpan StartTime, Color Color);
    internal enum PangolinBeyondImportMode { GroupByColor, SingleCollection }
    internal static class PangolinBeyondMarkParser { /* TryParse ... */ }
    internal static class PangolinBeyondMarkCollectionFactory { /* CreateCollections ... */ }

The exact helper signatures may be adjusted only to keep parsing and materialization pure and directly testable. They must not gain UI dependencies, mutate the destination collection, or become public. Preserve `MarkImportExportService.AddUniqueCollection` and `SetDefaultCollection` as the final append/default policy.

## Revision Note

2026-08-18: Initial ExecPlan created from the VIX-3988 handoff after source inspection of the existing import/export flow, WinForms selection dialog, Marks model, ColorPicker API, unique-name behavior, and test visibility. The plan resolves grouping order, parsing strictness, duration preservation, and no-mutation sequencing explicitly so implementation can proceed without relying on the handoff.

2026-08-18: Completed Milestone 1 by updating VIX-3988 with a concise user-facing Summary, Scope, Acceptance Criteria, and Validation Plan. Detailed parser, UI, and test design remains in this repository-local ExecPlan.

2026-08-18: Completed Milestone 2 by adding the legacy dialog radio button and `IsPangolinBeyondSelection` property, routing it from Marks Docker, and adding the documented `ImportPangolinBeyondMarks` entry point. Verified with `msbuild src\\Vixen.Modules\\Editor\\TimedSequenceEditor\\TimedSequenceEditor.csproj -m -t:Build -p:Configuration=Release -p:Platform=x64 -v:m`, which completed with zero errors.

2026-08-18: Completed Milestone 3 by adding separate internal record, mode, parser, and collection-factory files plus seven focused tests. The initial test exposed that `MarkCollection.AddMarks` re-enumerates its input, so the factory now materializes marks before its required single bulk-add call.

2026-08-18: Completed Milestone 4 by replacing the service placeholder with the legacy file/dialog workflow. It logs and displays the required error title for parse/read failures, maps Yes/No/other dialog results to grouping/single/cancellation behavior, uses the existing ColorPicker, and commits through an internal test seam after all decisions succeed. Added cancellation, unique-name/default-preservation, first-default, and legacy-dialog-result tests; the focused suite now has 13 passing tests.

2026-08-18: Corrected grouped import after manual testing found that the legacy Yes button returns `DialogResult.OK`, not `DialogResult.Yes`. The service now maps both results to grouped import and has direct regression coverage for OK, Yes, No, and Cancel mappings.

2026-08-18: Completed Milestone 5 with user-provided full-build, full-suite, and manual round-trip/import validation. Added the results to VIX-3988 comment 40367; no issue-description changes were needed.
