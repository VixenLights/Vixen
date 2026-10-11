# Improve wire diagram exports (VIX-4017)


This ExecPlan is a living document maintained according to `.agents/PLANS.md`. Keep Progress, Surprises & Discoveries, Decision Log, and Outcomes & Retrospective current. The Jira issue is https://vixenlights.atlassian.net/browse/VIX-4017, “Export Wire Diagram improvements”. This document defines a design; its creation does not implement the feature, update Jira, or authorize commits.

## Purpose / Big Picture


A user exporting a crowded prop can show fewer node numbers, adjust marker size, and immediately recognize the beginning and end of the wiring. All three export locations will offer the same options for both front and back diagrams. Every export will use a letter-sized canvas with small margins, preserve the prop's proportions, and fit the largest possible view into that canvas. An enlargement setting will enlarge the entire result, including its canvas, numbers, markers, and lines.

The existing output is SVG, a vector image whose shapes remain sharp when enlarged. Keep that format and its existing temporary-file/open-in-viewer workflow. There is no bitmap resolution or printer DPI setting to add.

## Progress


- [x] (2026-10-11) Read VIX-4017, the project planning skill, and `.agents/PLANS.md`; inspected the shared SVG exporter and identified all three callers.
- [x] (2026-10-11) User confirmed fixed letter canvas, small margins, maximal proportional fit, and enlargement of everything.
- [x] (2026-10-11) Designed the small milestones below; implementation began with milestone 2 after Jira alignment.
- [x] (2026-10-11) Milestone 1: Updated VIX-4017's description with the agreed user-facing scope and acceptance criteria; read back and verified the saved description.
- [x] (2026-10-11) Milestone 2: Confirmed available project/test/host conventions; added validated export options and boundary tests (15 passed).
- [x] (2026-10-11) Milestone 3: Extracted the shared SVG builder; automated checks passed and the user confirmed equivalent output at all three export locations.
- [ ] Milestone 4: Add number filtering.
- [ ] Milestone 5: Add configurable start, middle, and end markers.
- [ ] Milestone 6: Fit the diagram into a letter canvas.
- [ ] Milestone 7: Enlarge the whole SVG.
- [ ] Milestone 8: Add the options view model.
- [ ] Milestone 9: Add the shared WPF dialog and host bridge.
- [ ] Milestone 10: Connect the dialog to both shared export methods.
- [ ] Milestone 11: Verify all entry points and update repository evidence.
- [ ] Milestone 12: Reconcile Jira and record validation results.

Checkboxes describe implementation status. Do not mark a milestone complete merely because it appears in this plan.

## Surprises & Discoveries


The three entry points already converge in `src/Vixen.Modules/App/Modeling/ElementModeling.cs`. Custom Prop Editor calls `OrderedPointsToSvg`; Display Setup and Preview call `ElementsToSvg`. Therefore the dialog can be inserted at these two common export methods without copying it into three hosts.

The exporter currently draws red circles with radius 6, black labels with font size 16, and black connecting lines with stroke width 1. It enlarges props narrower than 800 units and uses prop-sized document dimensions rather than a paper-sized canvas. It writes a temporary `.svg` and opens it with `Process.Start` and `UseShellExecute = true`.

Milestone 3 moved that existing geometry, dimensions, mirroring, and SVG element ordering into the UI-independent `WireDiagramSvgBuilder`; `ElementModeling` retains both adapters, ordering, and file opening. The extracted builder tests verify the legacy dimensions and mirrored node coordinates for a normal three-node diagram. Manual review confirmed that all three export locations still produce an equivalent SVG diagram that looks like the pre-change output.

Numbering is not simply the loop counter. Custom Prop Editor supplies `(Point, int)` tuples. Element exports use `OrderModule.Order` when present and a counter otherwise. `OrderNodes` also handles duplicate order values by grouping leaves under parents. This improvement must not redefine that wiring order or overwrite stored order values.

Rider test discovery reported no existing tests for `VixenModules.App.Modeling.ElementModeling`. The new in-memory builder therefore needs focused behavioral coverage rather than relying on an existing export suite.

The existing `CalculateModelRectangle` substitutes 50 for a zero width or height. The new layout must handle genuinely vertical, horizontal, and single-point props directly instead of treating artificial dimensions as real geometry.

Bounded graph discovery surfaced related Custom Prop Editor plans, including `docs/plans/custom-prop-editor/VIX-2499-scale-custom-prop-background-image.md`, but did not return a wire-export specification or complete WPF hosting/project/test metadata. This is a limitation of the evidence collected, not proof that those documents or dependencies do not exist. Milestone 2 explicitly checks these conventions before any C# edits. The optional Gortex research operation was unavailable; the mounted graph read and impact tools worked.

Milestone 1 Jira verification: VIX-4017's description was read, replaced using the project Jira format, and read back. The saved description includes the all-numbers default, interval labels plus endpoints, marker choices and sizing, proportional letter-page fit with automatic orientation, full-image enlargement, all three entry points in both directions, unchanged wiring order/numbers, and no export on cancel/close. The initial read showed status `New Ticket`; after the description-only edit, Jira returned `In Progress`, and the read-back confirmed that status. No status field was sent, so this appears to have changed outside the requested field update; status was not edited separately.

Milestone 2 convention findings: the Modeling project is `src/Vixen.Modules/App/Modeling/Modeling.csproj`. It sets `UseWindowsForms=true`, does not set `UseWPF`, references the `Svg` package, and references Vixen.Core, Location, and Order projects. Rider's dependency view showed no direct Catel dependency. `Vixen.Tests` already references Modeling and has `xunit.v3`; the new tests use its existing xUnit `[Fact]`, `[Theory]`, and `[InlineData]` conventions. The Tag Manager exemplar's XAML derives from `<catel:Window>`, supports Escape, centers on screen, and contains no numeric spinner. Its view code constructs the view model directly, enables `ElementHost.EnableModelessKeyboardInterop`, then calls modal `ShowDialog()` without assigning an owner. Its view model derives from Catel `ViewModelBase` and uses Catel save/cancel lifecycle methods. Do not copy its manual DataContext construction into the planned options view.

The bounded graph searches did not surface relevant modeling/export documentation, a coordinate constraint in VIX-2499, or a reusable numeric up/down control. This is a limit of the evidence collected, not proof none exist. No current source evidence contradicts the plan's decision to leave stored prop coordinates unchanged. Milestone 9 must enable WPF in Modeling (`UseWPF=true` alongside WinForms), verify how Catel is provided and add only an existing centrally managed dependency if needed, and resolve the numeric input control from repository conventions before writing the view.

## Decision Log


Decision: Use a fixed letter canvas with proportional fitting and small margins; enlargement changes every visual component. Rationale: explicit user clarification on 2026-10-11. Date/Author: 2026-10-11, user.

Decision: Retain SVG and the current file-opening workflow. Rationale: the existing exporter already produces a scalable vector image; the issue does not request another format or save workflow. Date/Author: 2026-10-11, planning agent.

Decision: Automatically choose portrait or landscape by whichever permits the larger proportional fit; choose portrait on a tie. Use a quarter-inch margin. Rationale: this gives the largest view on a letter canvas without adding another dialog choice; it follows the recommendation presented before the user's clarification. Date/Author: 2026-10-11, planning agent.

Decision: Express the base SVG in 96 drawing units per inch, with physical document dimensions in inches. Portrait is 816 by 1056 drawing units and 8.5 by 11 inches; landscape reverses these. Rationale: physical SVG dimensions provide meaningful letter sizing, while a stable drawing coordinate system preserves the current radius 6 and font size 16. Date/Author: 2026-10-11, planning agent.

Decision: The first and last markers mean the first and last entries in the existing exported wiring sequence, not the minimum and maximum stored number. A single-node diagram uses the green start star and one label. Rationale: stored order values may repeat, and one point cannot show two distinguishable endpoint markers. Date/Author: 2026-10-11, planning agent.

Decision: Retain existing displayed number values. Always show the two endpoint labels; interval filtering applies to the existing displayed number for other nodes. Rationale: changing numbering is outside this improvement and could misrepresent existing wiring. The ticket's normal 1-based example remains exactly 1, 50, 100, 150, and the final number. Date/Author: 2026-10-11, planning agent.

Decision: Default to all numbers, radius 6, and enlargement 100 percent. Radius is selectable from 1 through 30 drawing units; number interval from 1 through 1000; enlargement from 100 through 400 percent. The interval input initially displays 50 while disabled in all-numbers mode. Rationale: these are bounded, understandable controls; defaults preserve existing marker size and full numbering, while the feature intentionally changes endpoint shapes and canvas sizing. The 1000 interval maximum reflects that higher values are not useful. Date/Author: 2026-10-11, planning agent; maximum clarified by user.

Decision: Use a WPF options dialog in the existing Modeling assembly with a Catel view model and a small view-side modal hosting bridge. Rationale: project skills prohibit new WinForms UI; the bridge permits existing WinForms hosts to use the same WPF dialog. Follow repository hosting conventions confirmed in Milestone 2 rather than introducing a new application-wide dialog framework. Date/Author: 2026-10-11, planning agent.

Decision: Keep each milestone to one responsibility and normally at most two production files, plus focused tests and this plan. Rationale: the user requested steps suitable for a lower reasoning model. Date/Author: 2026-10-11, planning agent.

## Outcomes & Retrospective


Planning outcome: requirements and the rendering algorithm are defined, and work is divided into twelve reviewable milestones. Planning validation: Gortex indexed the Markdown structure, reported no guard violations, and allowed its contract; `git status --short` showed only this new plan; `git diff --no-index --check -- /dev/null docs/plans/custom-prop-editor/VIX-4017-export-wire-diagram-improvements.md` emitted no whitespace diagnostics (exit 1 denotes the new-file difference). Rider found no existing ElementModeling tests. No application-level validation has occurred; milestone 2 implementation and test evidence is recorded below. Successful implementation means all six entry-point/direction combinations produce the same options and rendering behavior. Record actual commands, test counts, manual observations, and any changed assumptions here as implementation proceeds.

Milestone 1 outcome (2026-10-11): VIX-4017 was updated and read back. Its description now states the agreed user-facing requirements and Given/When/Then criteria across all three entry points and both directions. No comments were present before the update, and none were added. No source code or Jira fields other than the description were intentionally changed. The read-back showed Jira status `In Progress` although the pre-edit read showed `New Ticket`; see Surprises & Discoveries. No source validation or commit applies to that Jira-only milestone. Repository change validation consists of the Gortex-backed plan edit; no source tests were run.

Milestone 2 outcome (2026-10-11): Added `WireDiagramExportOptions` with defaults LabelInterval=1, NodeRadius=6f, ExportScale=1f and validation for all supported bounds and nonfinite float inputs. Added focused xUnit v3 tests covering defaults, lower/upper boundaries, zero, out-of-range, NaN, and positive infinity. Rider reported no file problems in either new C# file. The full-MSBuild test-target build completed with exit code 0; it emitted existing warnings in other files. The focused command `dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter FullyQualifiedName~WireDiagramExportOptionsTests` passed 15 tests (0 failed, 0 skipped). No test project reference or package changes were required.

Milestone 3 implementation outcome (2026-10-11): Added `WireDiagramSvgBuilder` and its focused tests, then routed `OrderedPointsToSvg` and `ElementsToSvg` through the shared builder while retaining node ordering and `OpenDoc` in `ElementModeling`. The builder materializes no files and does not launch a process; it accepts and validates options while preserving the prior radius 6, label size 16, width-based scaling, view box padding, horizontal flip, and element sequence. The full-MSBuild command `msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m` exited 0. The focused command `dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter FullyQualifiedName~WireDiagramSvgBuilderTests` passed 4 tests (0 failed, 0 skipped). Rider reported no file problems in the three changed C# files; `git diff --check` passed. Manual review (2026-10-11): the user confirmed that all three export locations still produce an equivalent SVG diagram that looks like the pre-change output. Milestone 3 is complete; milestone 4 remains open for separate confirmation.

## Context and Orientation


`src/Vixen.Modules/App/Modeling/ElementModeling.cs`, namespace `VixenModules.App.Modeling`, is the shared export boundary. `OrderedPointsToSvg(IEnumerable<Tuple<Point, int>> nodes, bool flip = false)` receives already ordered coordinates and labels. `ElementsToSvg(ElementNode elementNode, bool flip = false)` gathers distinct leaf nodes, checks Location properties, optionally calls `OrderNodes`, and obtains coordinates and labels. `flip` means a back diagram: reflect horizontal coordinates while retaining the wiring sequence and readable text.

The callers are `PropEditorViewModel.CreateWireDiagram` in `src/Vixen.Modules/App/CustomPropEditor/ViewModels/PropEditorViewModel.cs`, `SetupElementsTree.ExportWireDiagram` in `src/Vixen.Application/Setup/SetupElementsTree.cs`, and `VixenPreviewSetupElementsDocument.ExportWireDiagram` in `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupElementsDocument.cs`. The last two are reached through the common WinForms ElementTree menu in `src/Vixen.Common/Controls/ElementTree.cs`. Do not change those menu commands or the callers unless verification reveals an integration requirement.

Existing WPF hosting evidence includes `src/Vixen.Common/ElementTagManager/Views/ElementTagManagerWindow.xaml.cs`, whose `ShowAsDialog` is called from Preview. Use this as the bounded host-bridge exemplar in Milestone 2. Custom Prop Editor already uses WPF, but its window currently constructs its own view model; do not copy that pattern into the new dialog. The new dialog must use Catel's view-to-view-model mapping and constructor injection if a service is actually required.

Unit tests belong to `src/Vixen.Tests/Vixen.Tests.csproj`. Its native C++/CLI dependencies require full Visual Studio MSBuild before `dotnet test --no-build`. Do not assume a test framework from generic skill examples: confirm the existing project's attributes and assertions in Milestone 2 and use them consistently.

Proposed new files are `src/Vixen.Modules/App/Modeling/WireDiagramExportOptions.cs`, `src/Vixen.Modules/App/Modeling/WireDiagramSvgBuilder.cs`, `src/Vixen.Modules/App/Modeling/ViewModels/WireDiagramExportOptionsViewModel.cs`, `src/Vixen.Modules/App/Modeling/Views/WireDiagramExportOptionsWindow.xaml`, and its `.xaml.cs`. Proposed focused tests are `src/Vixen.Tests/App/Modeling/WireDiagramExportOptionsTests.cs` and `src/Vixen.Tests/App/Modeling/WireDiagramSvgBuilderTests.cs`. These are new paths, not claims that the files already exist.

The IDE is Rider with build, test discovery, and file-problem tools available. For implementation, provide small method-level snippets suitable for Rider's “Apply snippet from chat”. Apply actual writes through Gortex, as required by repository instructions. No snippets need to be generated during this planning-only task.

## Architecture Design: VIX-4017


### Core Strategy


Separate export input preparation, SVG construction, and user interaction. Keep input preparation and file opening in `ElementModeling`; put SVG construction in one UI-independent builder. Both entry methods gather their current ordered input, obtain options once, build, and open. Cancellation returns before building or creating temporary files.

The design uses the project `dotnet-best-practices`, `dotnet-design-pattern-review`, `catel-mvvm`, and `csharp-docs` guidance for resource disposal, focused responsibilities, WPF bindings, and documented APIs. It does not add unrelated command-handler infrastructure, libraries, persistence, or a new project. The test builder performs no dialog interaction or process launch.

### Data Model & Property Contracts


Add a public sealed immutable record `WireDiagramExportOptions` with init-only `int LabelInterval`, `float NodeRadius`, and `float ExportScale`, defaulting to 1, 6f, and 1f. Interval 1 means show all numbers. Add a public `void Validate()` that throws `ArgumentOutOfRangeException` for an interval outside 1..1000, a nonfinite radius or radius outside 1..30, or a nonfinite scale or scale outside 1..4. Validate at the builder boundary as well as in the dialog. This object is per export; do not serialize it or change prop data models.

The Catel view model exposes `bool ShowAllNumbers` (true), `int NumberInterval` (50), `int NodeRadius` (6), and `int ExportPercent` (100). Use Catel property registration and field/business-rule validation. Its accepted `WireDiagramExportOptions` maps the interval to 1 in all-numbers mode and maps percent to `ExportPercent / 100f`. Cancel never returns accepted options. Do not put SVG, WinForms, WPF window, or process types in this view model.

### Mathematical / Boundary Logic


Materialize the ordered input once. Preserve tuple labels and sequence. Empty input produces no dialog or output. Element exports with any missing Location property continue to produce no output. Keep null ElementNode handling. Do not change `OrderNodes` or introduce new sorting.

For node index i in a list of count n, show its label when i is 0, i is n-1, the interval is 1, or its supplied number modulo the interval is zero. This filters text only: every marker and every link between consecutive nodes remains. For n = 1, emit one star and one label.

Use red circles of radius r for middle nodes. The first marker is a filled green five-point star: ten alternating vertices around the center, starting upward at angle -pi/2, stepping by pi/5, with outer radius 1.25r and inner radius half the outer radius. The last marker is a red square of side 2r centered on the final point. Determine roles before applying the mirror. Draw wires behind markers and labels. Keep label text black, font size 16, and line stroke width 1 at base scale.

Fit all geometry AND visible marker/label bounds inside the margins. Use base dimensions (816,1056) for portrait and (1056,816) for landscape, and margin M = 24 units. Obtain the real input extents dx = xmax - xmin and dy = ymax - ymin, using floating-point subtraction to avoid integer overflow. Measure visible labels with a single disposed System.Drawing Font/Graphics measurement context using Arial, 16 pixels, and set the SVG font family to Arial too. Use conservative ascent/descent bounds, including measurement padding, so labels do not cross the margin. Do not add a font-measurement package or allocate a measurement context per node.

Let R = 1.25r, W be the maximum measured visible-label width, A be a conservative maximum height above the text baseline, and D be the maximum descent below it. Use global guards L = R, T = max(R, r + A), B = max(R, D - r), and Q = max(R, r + W). These guards reserve enough room for a marker and its label around every node; the baseline stays at centerY - r and the text starts at centerX + r. When there are no labels, the label terms are zero, although ordinary nonempty exports always have at least one endpoint label.

For each orientation let usableWidth = canvasWidth - 2M - L - Q and usableHeight = canvasHeight - 2M - T - B. The uniform coordinate-fit factor k is the smaller of usableWidth/dx and usableHeight/dy, ignoring a ratio whose extent is zero. When both extents are zero use k = 1. Select the orientation with larger k; tie means portrait. This is the largest proportional fit under the explicit conservative bounds, not independent X/Y stretching. Assert usable dimensions remain positive at all allowed option values.

Center the resulting content envelope: originX = M + ((canvasWidth - 2M) - (dx*k + L + Q))/2 + L; originY uses dy*k, T, and B analogously. Front coordinates are (originX + (x-xmin)*k, originY + (y-ymin)*k). For back coordinates replace the X term by originX + dx*k - (x-xmin)*k. The origin and guards are the same for both directions, and labels stay readable. A horizontal or vertical line fits along its nonzero axis; coincident points center safely. Coincident markers may overlap; this change does not invent artificial spacing or remove input points.

At enlargement factor s, keep the ViewBox and all drawing coordinates at base size. Set physical SVG width to pageWidthInches*s and height to pageHeightInches*s. A fixed ViewBox mapped to larger physical dimensions enlarges the canvas, margin, lines, labels, and markers together without separately scaling them or multiplying twice. At 100 percent portrait is 8.5in by 11in; at 200 percent it is 17in by 22in. Enlarge only after choosing the base orientation and fit. Preserve the SVG namespace and write valid unit-bearing dimensions supported by the existing Svg package.

### Subsystem Component Matrix


`ElementModeling.cs` becomes two existing public export adapters plus unchanged ordering and file-opening helpers. New `WireDiagramSvgBuilder.cs` owns label filtering, markers, page fitting, mirroring, and SVG dimensions. New `WireDiagramExportOptions.cs` owns defaults and validation. The new WPF view and Catel view model own user choices and cancellation. The three host files consume the behavior through their existing calls. Focused Modeling tests inspect in-memory SVGs, without opening a desktop viewer.

## ACTIVE EXECUTION PLAN (Derived from .agents/PLANS.md)


### Shared execution and stop rules


Read relevant project skills before applying them: `dotnet-best-practices` for every C# milestone, `csharp-docs` for public/protected API changes, and `catel-mvvm` for the dialog milestones. Apply tabs and LF, document added/changed public APIs, and preserve unrelated work. Do not make commits without an explicit request.

For every coding milestone, use Gortex `explore(operation:"task")`, at most one targeted follow-up, and `change(operation:"impact")` before editing. Use `change(operation:"verify")` for a proposed signature change. Write only through `edit` or `refactor`. After edits use `change(operation:"detect")`, then its returned IDs with `tests`, `guards`, and `contract`. Run Rider `get_file_problems` for each changed C# or XAML file; correct only issues in the lines changed for this milestone. Do not use shell searches to reopen indexed source.

Each milestone ends with the exact stopping marker below. At that boundary halt implementation, run `git status --short` and the file-scoped `git diff`, update this plan's living sections, invoke `.agents/skills/commit-msg/SKILL.md` with VIX-4017 as the subject prefix when repository files changed, and emit its paste-ready Commit message block. Pause for explicit confirmation before moving on; do not create the commit yourself. For a Jira-only milestone report that there is no source commit and still pause. These manual boundaries follow the explicitly invoked planning skill.

### Milestone 1: Align the Jira requirements


Context and work: this milestone changes only VIX-4017 and the plan's progress. Read the current issue again through Atlassian MCP. Update its description using the project Jira skill's Summary, Scope, and Acceptance Criteria format. Include all ticket behaviors, the user's canvas/scaling clarifications, automatic orientation, all-numbers default, and cancel-without-export. Keep technical file paths and formulas in this plan. State verification as user behavior, including all three entry points in both directions. The update belongs to future execution; it has not been performed by authoring this plan.

Concrete steps and acceptance: use Atlassian get/edit/get issue tools for VIX-4017, then compare the returned description to Purpose and Validation below. Expect the issue to describe a fixed letter canvas, preserved proportions, interval labels plus endpoints, adjustable markers, green start star, final square, and full-image enlargement. Preserve unrelated issue fields and comments.

STOP HERE for manual review and commit execution before proceeding.

### Milestone 2: Confirm conventions and add validated options


Context and work: before code, use a bounded Gortex task to inspect relevant `docs/` modeling/export guidance, the Modeling project file, the existing test project conventions, and `ElementTagManagerWindow.ShowAsDialog` plus its view model. Read the related background-image plan only for requirements that might constrain coordinate handling; this feature does not rescale stored coordinates. Record the actual Modeling project path, WPF/Catel references, dialog base class, and test framework in Artifacts. If a repository document demonstrably conflicts with the runtime behavior described above, record and reconcile that specific difference before editing; do not silently change this design.

Create only `WireDiagramExportOptions.cs` and `WireDiagramExportOptionsTests.cs` using the record and Validate contract above. Use the existing test framework. Ensure the test project references Modeling; if absent, add its existing project reference with repository asset/copy conventions. Do not add a project or test package. If Modeling lacks required WPF support, record the minimal project-property/reference addition here for Milestone 9, using central packages already in the solution.

Concrete steps: perform the test build and focused test command in Concrete Steps with the options-test class filter. Acceptance: defaults are exactly 1, 6f, 1f; lower and upper allowed bounds validate; zero, out-of-range, NaN, and infinity inputs throw the documented exception. Test boundaries, not every possible value.

STOP HERE for manual review and commit execution before proceeding.

### Milestone 3: Extract the shared builder without changing export behavior


Context and work: edit `ElementModeling.cs`, create `WireDiagramSvgBuilder.cs`, and create `WireDiagramSvgBuilderTests.cs`. Add the public static Build signature specified under Interfaces. Move the existing document assembly, circle, label, line, and coordinate calculation into the builder, preserving the current width-based sizing and visual output for this milestone. Accept and validate options, but defer their rendering effects to later milestones. Keep `OpenDoc` in ElementModeling. Do not open files or show UI from Build.

Make `OrderedPointsToSvg` materialize tuples once; make `ElementsToSvg` materialize leaves, retain its Location guard and current OrderNodes behavior, and adapt the ordered nodes to coordinate/number tuples. Both call Build with default options and then their existing OpenDoc. Preserve original public signatures. A null element or empty list returns without output; do not call Max/Min on an empty sequence.

Concrete steps: build tests and run the builder test class. Acceptance: both adapters reach one document-building path; tests observe supplied labels, all markers, n-1 lines, input order, and reversed horizontal positions for back diagrams. Build itself does not create files or launch processes. Manually compare a normal pre-change and post-extraction SVG before proceeding. This is a refactor checkpoint, not completion of letter sizing.

STOP HERE for manual review and commit execution before proceeding.

### Milestone 4: Filter only node numbers


Context and work: change only the builder and its test file. Apply the label predicate from Mathematical / Boundary Logic. Keep markers, lines, order, and displayed number values intact. Do not identify endpoints by comparing their labels to 1 or to the largest value.

Concrete steps: run the builder test class. Acceptance: a sequence labelled 1 through 123 with interval 50 produces labels 1, 50, 100, 123, 123 markers, and 122 lines. At count 100 it produces 1, 50, 100 without a duplicate end label. Interval 1 shows all labels. An interval larger than the sequence leaves the two endpoint labels. Add a nonsequential/repeated-number case to prove endpoint selection uses sequence position. A single point has one label.

STOP HERE for manual review and commit execution before proceeding.

### Milestone 5: Render the three marker roles


Context and work: change only the builder and its tests. Replace the first circle with the green star, the last circle with the centered square, and retain red circles in the middle. Use the selected radius and the exact star/square geometry above. Put wire elements before markers and labels so a connecting line cannot conceal the marker center. Use first-node precedence for a singleton.

Concrete steps: run the builder test class and inspect one SVG in the existing viewer. Acceptance: a three-node export has one green star, one red circle, one red square, and two black wires; doubling the radius doubles each marker's dimensions and label offset. The star's outer radius is 1.25r and square side is 2r. Back export retains endpoint identity while mirroring centers. A one-node export has one green star and one label, with no square or wire.

STOP HERE for manual review and commit execution before proceeding.

### Milestone 6: Fit all visible content to a letter canvas


Context and work: change only the builder and its tests. Replace legacy width-based scaling with orientation selection, real extents, conservative label/marker bounds, and the proportional-fit calculation above. Add one disposed font-measurement context per build. Remove the artificial 50-unit zero-axis fallback from the builder's layout path. Do not alter stored locations or OrderNodes.

Concrete steps: run the builder test class. Acceptance: tall inputs choose 8.5in by 11in, wide inputs choose 11in by 8.5in, tie inputs choose portrait. Base ViewBox dimensions are exactly the matching 816/1056 units. Pairwise coordinate differences retain a single uniform factor. Visible content stays inside the quarter-inch margins, and the limiting axis consumes the usable envelope within floating-point tolerance. Test negative coordinates, a horizontal line, a vertical line, one point, all coincident points, large coordinate differences, and a long endpoint label. Mirror tests compare front/back center coordinates and unchanged label text. Inspect tall, wide, and dense examples for clipping in a viewer; do not assert font-dependent pixel widths exactly.

STOP HERE for manual review and commit execution before proceeding.

### Milestone 7: Enlarge the entire SVG through physical dimensions


Context and work: change only the builder and its tests. Apply ExportScale solely to the physical width and height in inches. Keep the base ViewBox, fitting, marker geometry, text geometry, and line widths unchanged. Add no separate transform or second scaling pass.

Concrete steps: run the builder test class. Acceptance: the same portrait input produces 8.5in by 11in at scale 1 and 17in by 22in at scale 2 with identical ViewBoxes and child coordinates. Serialize both documents in tests and verify unit-bearing dimensions survive serialization. In a viewer that honors SVG dimensions at actual size, the second output renders every component twice as large. Record if a browser's fit-to-window behavior hides that physical change; use serialized dimensions as the definitive automated proof.

STOP HERE for manual review and commit execution before proceeding.

### Milestone 8: Add the validated Catel view model


Context and work: create only `ViewModels/WireDiagramExportOptionsViewModel.cs` and a focused view-model test file under `src/Vixen.Tests/App/Modeling/`. Use Catel.MVVM.ViewModelBase, Catel property registration, and field/business-rule validation. Implement the four UI properties and accepted options mapping above. Use the confirmed Catel save/cancel lifecycle; do not introduce a parallel command framework or service locator.

Concrete steps: build and run the view-model test class using existing Catel test initialization if required. Acceptance: initial values match the design; all-numbers mode ignores the disabled interval; interval mode maps the chosen positive integer; 200 percent maps to 2f; invalid entries prevent acceptance; cancellation yields no accepted options. Tests exercise option conversion/validation, not XAML implementation details. No window or process is opened by these tests.

STOP HERE for manual review and commit execution before proceeding.

### Milestone 9: Add the one shared WPF dialog


Context and work: create `Views/WireDiagramExportOptionsWindow.xaml` and `.xaml.cs`. Use the Catel `<catel:Window>` base. The Milestone 2 Tag Manager exemplar has no numeric spinner; verify an existing control available through current dependencies and record its exact type before creating the view. If none exists, update this plan with a compatible control choice before writing XAML, and do not add a package without checking the central package catalog. Show an all-numbers checkbox, an interval spinner enabled only when that checkbox is clear, a numeric spinner labelled “Node marker size”, and an enlargement-percent spinner. Present a brief note that the result fits a letter page and that enlargement affects everything. Follow existing theme, spacing, and OK/Cancel conventions.

Use bindings and the Catel save/cancel lifecycle. Code-behind is limited to InitializeComponent, icon/view setup if conventional, and the host bridge. Add a public static `ShowAsDialog()` wrapper returning accepted options or null. It creates one modal view, lets Catel locate its view model, and enables `ElementHost.EnableModelessKeyboardInterop(window)` before calling `ShowDialog()`. The exemplar assigns no owner, so verify owner assignment for both WPF and WinForms hosts before implementation; do not assume that `CenterScreen` assigns an owner. Select the active WPF or WinForms owner using that convention; do not set DataContext manually or write rendering logic here. Cancel, Escape, and window close must all return null.

If necessary, make only the WPF project-property/reference changes recorded in Milestone 2. Use existing packages and project references; no new project or new WinForms controls. This milestone does not connect the dialog to export yet.

Concrete steps: build the affected project and run Rider get_file_problems on both view files and the view model. Open the view using Rider's existing WPF dialog/test conventions. Acceptance: numeric bounds and defaults are correct, invalid values cannot be accepted, tab navigation works, and it is modal and owned correctly from WPF and WinForms. Repeatedly open/cancel without an orphaned window. Capture a short manual result in Artifacts.

STOP HERE for manual review and commit execution before proceeding.

### Milestone 10: Connect options at the two common export boundaries


Context and work: edit only `ElementModeling.cs` plus focused adapter tests if existing test infrastructure permits. After input validation/preparation in each public exporter, call the new ShowAsDialog once. Return immediately on null; otherwise pass accepted options to Build and call OpenDoc. Preserve both existing method signatures and the callers' flip arguments. Update XML documentation for the changed public behavior, including no output on cancellation and unchanged numbering/order semantics.

Do not move the dialog into the three callers, change the ElementTree export delegate, or add a second confirmation dialog. If adapter unit tests would require launching UI, use the manual entry-point check instead of an artificial global callback or reflection-heavy test seam. The builder already tests the rendering contract.

Concrete steps: build the affected solution configuration, run focused Modeling tests, and exercise one WPF and one WinForms export. Acceptance: exactly one options popup appears before a valid export; OK opens one SVG with the chosen options; Cancel/Escape/close opens none and creates no export file. Empty input or missing Location data follows existing no-output behavior. Both directions pass the same options through the same builder.

STOP HERE for manual review and commit execution before proceeding.

### Milestone 11: Verify the six user workflows


Context and work: no feature expansion. Exercise Custom Prop Editor, Display Setup Element Tree, and Preview Element Tree, each front and back. Use the same recognizable prop with at least 123 nodes, a known starting point, and asymmetric geometry. Export defaults, interval 50, smaller/larger radius, and 200 percent enlargement. Check the behavior in Validation and Acceptance, including cancellation from each host and dense-label margin containment.

Concrete steps: run the full affected Release/x64 build, the focused Modeling tests, and the existing full unit suite once using the commands below. Run Rider get_file_problems on changed production/test files. Record actual test counts and manual observations, not predicted transcripts. Fix only defects in this feature and rerun affected checks. Update all living plan sections and repository guidance only if a discovered export contract needs documenting.

Acceptance: all six combinations show the same dialog and visual rules; front/back differ only by mirrored coordinates, with readable numbers and correct wiring order. Existing unrelated tests remain green or pre-existing failures are explicitly identified. No prop coordinates, element Order properties, or saved prop configuration changes during export.

STOP HERE for manual review and commit execution before proceeding.

### Milestone 12: Close the documentation and Jira loop


Context and work: review VIX-4017 against delivered behavior. Make only necessary description adjustments and add a concise comment reporting user-visible completion, actual validation results, and any remaining limitation. Do not claim success for an unperformed manual check or change issue status without authorization. Keep implementation paths and algorithms in this repository document. Update Progress and Outcomes with what was delivered and what remains.

Concrete steps and acceptance: read/edit/comment/read through Atlassian MCP, following the Jira skill. Confirm its acceptance criteria match all six workflows and the recorded test results. The plan and issue must agree on canvas sizing, enlargement, numbering, marker behavior, and cancellation.

STOP HERE for manual review and commit execution before proceeding.

## Concrete Steps


All shell commands below run in PowerShell with working directory `C:\Dev\Vixen`. Use full Visual Studio MSBuild from a Developer PowerShell or its installed executable, not `dotnet msbuild`, because tests depend on C++/CLI projects. Start long commands with native exec_command and yield_time_ms 10000; poll running commands through write_stdin with yield_time_ms at least 30000.

Before every coding milestone:

    git status --short

After a milestone, substitute its actual paths in a file-scoped diff; do not stage unrelated work:

    git diff --check
    git diff -- src/Vixen.Modules/App/Modeling/ElementModeling.cs

Build the test target before running built tests:

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m

Run the initial options tests, then change the FullyQualifiedName substring to WireDiagramSvgBuilderTests or WireDiagramExportOptionsViewModelTests as the corresponding milestone requires:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter FullyQualifiedName~WireDiagramExportOptionsTests

After several test classes exist, run the focused set:

    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/ --filter FullyQualifiedName~WireDiagram

At the integration/final milestones build the affected solution and run the full already-built unit suite:

    msbuild Vixen.sln -m -t:restore -t:Rebuild -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m
    dotnet test src/Vixen.Tests/Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir=C:/Dev/Vixen/

Expected evidence is exit code 0, Build succeeded with zero errors, and test output reporting zero failed tests and a nonzero number of executed tests. A filter matching zero tests is a failure of validation, not a pass. Record actual totals rather than prescribing an invented test count. If msbuild is not on PATH, use a Visual Studio Developer PowerShell or locate the full installation through vswhere; do not replace it with dotnet's bundled MSBuild.

Launch the built Vixen application from Rider's existing application run configuration or `Release/Output/`, using the actual executable configured by the solution. Open Custom Prop Editor, Display Setup, and Preview Setup to perform the six manual exports. Avoid changing the user's real show: use a disposable test profile and prop.

## Validation and Acceptance


At defaults every valid export shows all existing number values, uses radius 6, marks the first point with a green star and the last with a red square, and fits a letter canvas without distorted proportions or clipped visible content. The document dimensions, not the viewer's arbitrary zoom level, prove physical page size.

For a normal 123-node sequence numbered 1..123, interval 50 shows exactly 1, 50, 100, 123. All nodes and 122 wires remain. Switching front to back mirrors positions but does not reverse input order, replace labels, or mirror text glyphs. One node has one star, one label, and no wire. Zero nodes produce no export.

A radius change affects every marker and label offset; the star remains slightly larger than a circle, and the final square has the circle's diameter as its side length. At 200 percent every visual component doubles physically, with no double scaling and no change to the prop's stored coordinates.

Every entry point shows one shared popup for both directions. Cancel, Escape, and close leave no exported SVG or viewer launch. Missing Location data retains existing behavior. Tests prove the in-memory builder contract; manual checks prove ownership, modal behavior, viewer compatibility, and integration.

## Idempotence and Recovery


The feature changes only transient export options and generated images; it does not migrate or resave props. Rerunning builds, tests, and exports is safe. Never overwrite unrelated source changes or alter existing wiring order to make a test pass. If a milestone fails, keep its Progress item open, record the failure, and fix only its scoped changes before review. Remove only explicitly identified test-generated temporary artifacts; do not clean a user's general temporary folder. Preserve the existing OpenDoc workflow, including unrelated temporary-file quirks, unless a demonstrated failure requires a separately recorded scope decision.

If a project reference or WPF setting already exists, do not add it twice. If required framework/hosting metadata differs from this design, resolve the exact integration detail in Milestone 2 and update the relevant milestones before editing; do not let a lower reasoning model invent a second UI architecture.

## Artifacts and Notes


Planning evidence from the existing source:

    ElementModeling.OrderedPointsToSvg: radius = 6; CreateLabel uses FontSize = 16.
    ElementModeling.ElementsToSvg: preserve OrderNodes and OrderModule.Order.
    ElementModeling.OpenDoc: write .svg; Process.Start with UseShellExecute = true.
    Direct export consumers: Custom Prop Editor, Display Setup, Vixen Preview.

Milestone 2 evidence: the Modeling project is `src/Vixen.Modules/App/Modeling/Modeling.csproj`. It sets `UseWindowsForms=true` but has no `UseWPF` property; it references `Svg`, Vixen.Core, Location, and Order. The Rider dependency view showed no direct Catel dependency. `Vixen.Tests` already references Modeling and uses xUnit v3 (`[Fact]`, `[Theory]`, `[InlineData]`). The Tag Manager view derives from Catel's `<catel:Window>`, supports Escape, and has no numeric spinner. `ShowAsDialog` enables `ElementHost.EnableModelessKeyboardInterop(window)` and invokes `window.ShowDialog()`; the exemplar does not assign an owner. Its view model derives from Catel `ViewModelBase`, but the view manually constructs it, which this dialog must not copy. Bounded graph discovery did not surface relevant export docs, VIX-2499 coordinate constraints, or a reusable numeric up/down control; do not interpret that as proof those do not exist. Milestone 9 must add `UseWPF=true` alongside WinForms if still required, verify/add Catel only through existing centrally managed dependencies, and settle the numeric control after checking repository conventions. The new options type needed no project changes.

Append short actual build/test transcripts and the six manual results as milestones complete. Example only, not an assertion of execution:

    Build succeeded. 0 Error(s).
    Test result: zero failed, nonzero executed; record the actual passed count.
    Custom Prop Editor front/back: one dialog, expected SVG, cancel creates no output.

## Interfaces and Dependencies


Retain these existing ElementModeling public signatures:

    public static void OrderedPointsToSvg(IEnumerable<Tuple<Point, int>> nodes, bool flip = false)
    public static void ElementsToSvg(ElementNode elementNode, bool flip = false)

In namespace `VixenModules.App.Modeling`, add one type per file:

    public sealed record WireDiagramExportOptions
    {
        public int LabelInterval { get; init; } = 1;
        public float NodeRadius { get; init; } = 6f;
        public float ExportScale { get; init; } = 1f;
        public void Validate();
    }

    public static class WireDiagramSvgBuilder
    {
        public static SvgDocument? Build(
            IReadOnlyList<Tuple<Point, int>> nodes,
            bool flip,
            WireDiagramExportOptions options);
    }

The declaration blocks above specify contracts, not complete paste-ready class definitions: implement method bodies in their named milestones. Build throws ArgumentNullException for null nodes/options, validates options, and returns null for empty input. It uses the existing Svg package and System.Drawing Point/PointF types, does not reorder input, and creates no files. Document all new public members and exceptions with the project csharp-docs skill.

In `VixenModules.App.Modeling.ViewModels`, add public sealed `WireDiagramExportOptionsViewModel : Catel.MVVM.ViewModelBase` with the four properties and accepted-options lifecycle described above. In the new view add:

    public static WireDiagramExportOptions? ShowAsDialog();

The view's exact base class and numeric-control namespace come from the established repository exemplar confirmed in Milestone 2. The bridge owns window creation, modal display, and host ownership; the view model owns option values and validation; ElementModeling owns whether to launch an export. Keep that division and do not add service-locator calls or a new global mutable export setting.

No new runtime or test dependency is planned. If project metadata requires a reference to an existing solution package/project, follow central package and Copy Local/asset conventions. Follow existing test-framework conventions even where a generic skill names a different framework.

Analysis complete and plan integrated with plans.md.

Revision note (2026-10-11): Created this plan after the user resolved paper sizing and enlargement semantics. Split rendering, UI, and integration into twelve small milestones to reduce the reasoning needed per implementation turn. Initial source evidence confirms the common SVG boundary; remaining repository integration metadata has a bounded prerequisite in Milestone 2.
