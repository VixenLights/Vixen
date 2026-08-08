# Show a busy cursor and status message while the Custom Prop Editor vendor browser loads

This ExecPlan is a living document. The sections `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` must be kept up to date as work proceeds.

Maintain this document in accordance with `.agents/PLANS.md` from the repository root. It is self-contained so that an implementer can complete this work without relying on prior chat context. This work is tracked as [VIX-3968](https://vixenlights.atlassian.net/browse/VIX-3968).

## Purpose / Big Picture

The Custom Prop Editor is a window in Vixen (a Windows desktop application built with WPF, a Windows user-interface framework, using the Catel MVVM library for its view-model/command plumbing) that lets a user design a physical lighting "prop" made of individually addressable lights. One of its menu commands, `Tools > Vendor Browser`, downloads a list of prop vendors and then downloads each vendor's inventory file over the network before opening a dialog that lists the available models. Today, nothing on screen changes while those downloads are in flight: the mouse cursor stays as an ordinary pointer and there is no text telling the user that anything is happening. On a slow or flaky connection this makes the application look frozen or unresponsive between the moment the user clicks "Vendor Browser" and the moment the selection dialog appears.

After this change, clicking `Tools > Vendor Browser` immediately turns the mouse cursor into the standard Windows wait cursor, and a short status message appears in the Custom Prop Editor's status bar (for example "Loading vendor list..." followed by "Retrieving inventory from <Vendor Name>..." for each vendor in turn). Both the cursor and the message clear automatically once the vendor inventories have all been retrieved (successfully or not) and control returns to the user, whether that is because the selection dialog opened, the user had no vendors to browse, or every vendor download failed.

The status-bar message itself is added as a general-purpose, single-slot status line on the Custom Prop Editor's main view model, not as something private to the vendor browser. It is a single string property that any operation in the window can set while it runs and clear when it finishes, the same way `IBusyIndicatorService.Show()`/`Hide()` already work for the wait cursor across several unrelated commands in this class (`ImportProp`, `SaveModelAsync`, `LoadVendorModel`, and now `OpenVendorBrowserAsync`). This plan only wires the new property into `OpenVendorBrowserAsync`, but its name, placement, and shape must not assume vendor-browser wording or vendor-browser-only usage, so that a later change can reuse the same property for import, save, or export status without renaming or restructuring it.

A user can see this working by opening the Custom Prop Editor, choosing `Tools > Vendor Browser`, and watching the cursor change to the wait cursor and the status bar text update while the vendor list and inventories download, then watching both clear when the vendor selection dialog appears (or when the operation ends for any other reason).

## Progress

- [x] (2026-08-08) Milestone 1: Created [VIX-3968](https://vixenlights.atlassian.net/browse/VIX-3968) as an Improvement in project VIX, with the problem, proposed change, acceptance criteria, and test plan from this ExecPlan recorded in its description. Left in its default "New Ticket" status; no workflow transition made.
- [x] (2026-08-08) Milestone 2: Added `StatusMessage` (a `RegisterProperty`/`GetValue`/`SetValue`-backed, general-purpose property, matching the `FilePath`/`ElementTreeViewModel` pattern already used in this file) to `PropEditorViewModel.cs`, and a new `StatusMessageBarItem` `StatusBarItem` bound to it in `CustomPropEditorWindow.xaml`'s existing `StatusBar`, collapsed via a `DataTrigger` when the message is empty. Also fixed six pre-existing Rider warnings the post-edit check flagged in `PropEditorViewModel.cs` (two malformed `</summary` doc comments, a possible-multiple-enumeration on a `Where`/`Any`/`First` chain simplified to `FirstOrDefault`, an unused lambda parameter renamed to `_`, and `ImportProp`'s unused `bool` return type removed in favor of `Task`).
- [x] (2026-08-08) Milestone 3: Wired `IBusyIndicatorService.Show()`/`Hide()` and `StatusMessage` into `OpenVendorBrowserAsync`, wrapping the method body (after resolving `dependencyResolver` and `pleaseWaitService` and setting the initial "Loading vendor list..." message) in `try`/`finally`, with per-vendor `StatusMessage` updates in the download loop and both the cursor and message cleared unconditionally in `finally`. Left `GetVendorUrls`, `LoadVendorModel`, and the dialog/model-assignment logic unchanged. `dotnet build src\Vixen.Modules\App\CustomPropEditor\CustomPropEditor.csproj --no-restore` succeeded with 0 errors (6 pre-existing, unrelated warnings elsewhere in the solution).
- [ ] Milestone 4: Validate the change, update Jira, and close the loop.

## Surprises & Discoveries

- Observation: Editing `PropEditorViewModel.cs` for Milestone 2 triggered this repository's Rider post-edit check, which flagged six pre-existing warnings unrelated to the `StatusMessage` addition: two doc comments missing a closing `>` on `</summary`, a possible-multiple-enumeration warning on a `Where(...).Any()` / `Where(...).First()` pair, an unused lambda parameter, and an `async Task<bool> ImportProp(...)` whose `bool` return value was never used by either call site.
  Evidence: Post-edit check output listed `PropEditorViewModel.cs:725:15`, `:857:15`, `:1150:11`, `:1152:38`, `:406:14`, and `:1016:21`.
- Decision: Fixed all six as directed by the tooling (the two doc comments closed, the `Where`/`Any`/`First` chain simplified to a single `FirstOrDefault`, the unused lambda parameter renamed to `_`, and `ImportProp` changed from `Task<bool>` to `Task` since neither caller used the result). These are pre-existing, unrelated to this task's scope, but were required by the repository's mandatory post-edit tooling; behavior is unchanged in all cases.
  Date/Author: 2026-08-08 / Claude

(Fill in further as work proceeds. Seed observation carried in from prior, related work below.)

- Observation: Catel 6.2's WPF implementation of `IBusyIndicatorService` (the interface this repository already uses to request a busy cursor; see `Catel.Services.IBusyIndicatorService`, resolved via `this.GetDependencyResolver().Resolve<IBusyIndicatorService>()`) only overrides `Mouse.OverrideCursor`. It does not render a spinner window, and nothing in this repository currently renders the `status` text passed to `Show(string status)` or `UpdateStatus(string status)` — no `BusyIndicator`-style control exists anywhere under `src/`. This was confirmed for a related fix in `docs/plans/vix-3563-custom-prop-xmodel-import-busy-indicator.md`. Consequently, `IBusyIndicatorService` alone can give the spinning/wait cursor this task asks for, but the "status line message" part of this task requires adding real, visible UI — it cannot be achieved by passing a string into `IBusyIndicatorService`.

## Decision Log

- Decision: Use the existing `IBusyIndicatorService.Show()` / `Hide()` pair for the wait cursor, exactly as `PropEditorViewModel` already does around `LoadVendorModel`, `ImportProp`, and `SaveModelAsync`.
  Rationale: This is the established, working pattern in this exact class for "network or disk work is happening, show the wait cursor." Introducing a second mechanism for the cursor would be inconsistent with the rest of the file and with `docs/plans/vix-3563-custom-prop-xmodel-import-busy-indicator.md`.
  Date/Author: 2026-08-08 / Claude

- Decision: Implement the status message as a new plain view-model property (`StatusMessage`) bound to a new `StatusBarItem` in `CustomPropEditorWindow.xaml`, rather than trying to route it through `IBusyIndicatorService`.
  Rationale: As recorded above, `IBusyIndicatorService`'s status text is not rendered by anything in this codebase today. The window already has a `StatusBar` with one `StatusBarItem` (currently showing the drawing-canvas mouse coordinates only while the mouse is over the canvas), so adding a second, independently-visible `StatusBarItem` is a small, consistent extension rather than a new subsystem.
  Date/Author: 2026-08-08 / Claude

- Decision: Name the new property `StatusMessage` (not `VendorBrowserStatusMessage`) and treat it as a general-purpose, single-slot status line owned by `PropEditorViewModel`, so any current or future long-running operation in the Custom Prop Editor (import, save, vendor browsing, wire-diagram export, and so on) can set and clear it, not only `OpenVendorBrowserAsync`.
  Rationale: The user explicitly asked that the status bar not be constrained to vendor-browser messages. A single shared property on the view model, written to by whichever operation is currently running, is the simplest design that satisfies that: it needs no new collection, queueing, or priority logic, and every existing caller already runs its long operation on the UI thread inside a `try`/`finally` alongside `IBusyIndicatorService.Show()`/`Hide()`, so setting/clearing `StatusMessage` in that same `try`/`finally` is a natural fit anywhere, not just in the vendor browser. This plan still only wires it up in `OpenVendorBrowserAsync` (see Milestone 3); wiring it into other commands is out of scope here but is now unblocked for a future, separate change.
  Date/Author: 2026-08-08 / Claude

- Decision: Do not add an artificial `Task.Delay` before the first network call, unlike the CPU-bound fix in VIX-3563.
  Rationale: VIX-3563 needed a delay because `ImportProp` did synchronous, CPU-bound parsing immediately after `Show()`, with no `await` in between to hand control back to the WPF dispatcher (the queue that processes UI updates, including cursor changes) before that work began. Here, the first thing that happens after `Show()` is already `await GetVendorUrls()`, which itself begins with an asynchronous network call. That `await` yields control back to the dispatcher on its own, which lets the wait-cursor override reach the screen before any further work proceeds. This should be verified during Milestone 4's manual validation; if the cursor does not visibly appear in practice, adopt the same `Task.Delay(200)` yield used in VIX-3563 and record that reversal here.
  Date/Author: 2026-08-08 / Claude

## Outcomes & Retrospective

(Complete at the end of Milestone 4.)

## Context and Orientation

The Custom Prop Editor module lives at `src/Vixen.Modules/App/CustomPropEditor`. Its main view is `src/Vixen.Modules/App/CustomPropEditor/Views/CustomPropEditorWindow.xaml`, and its main view model is `src/Vixen.Modules/App/CustomPropEditor/ViewModels/PropEditorViewModel.cs`. Catel's MVVM pattern used throughout this repository binds XAML controls directly to public properties and `ICommand`-typed properties on a view model class; there is no code-behind logic beyond wiring.

The window's `Tools` menu contains a `MenuItem Header="Vendor Browser"` bound to `Command="{Binding OpenVendorBrowserCommand}"` (`CustomPropEditorWindow.xaml`, around line 106). `OpenVendorBrowserCommand` is a Catel `TaskCommand` in `PropEditorViewModel.cs` (around line 1026) that runs the private method `OpenVendorBrowserAsync` (around line 1034) when invoked.

`OpenVendorBrowserAsync` currently does the following, in order: it calls the private `GetVendorUrls()` method (around line 1489), which downloads and parses `https://app.vixenlights.com/vendor.json` into a list of `VendorLink` objects (each with a `Name` and a `Url`), and returns immediately if that list is empty. It then resolves `IDownloadService` (a Catel-provided service used elsewhere in this file to download files) from the dependency resolver, and for each `VendorLink` it downloads that vendor's inventory XML via `ds.GetFileAsStringAsync(new Uri(vendorLink.Url))` and imports it with `XModelInventoryImporter`, catching and reporting per-vendor download/parse failures with a message box rather than aborting the whole operation. If no inventories were successfully retrieved, the method returns. Otherwise it resolves `IUIVisualizerService` and shows the `VendorInventoryWindowViewModel` selection dialog, and if the user picks a model, calls the already-busy-indicator-wrapped `LoadVendorModel` (around line 1445, which itself calls `pleaseWaitService.Show()` at line 1454 and `pleaseWaitService.Hide()` at line 1485).

So the gap this task closes is specifically the window between the user choosing the `Vendor Browser` menu item and either the selection dialog appearing or the method returning early (no vendors configured, or every vendor download failed) — none of that window currently shows any busy indication.

`IBusyIndicatorService` is a Catel 6.2 (`Catel.Services` namespace) interface already referenced via `using Catel.Services;` at the top of `PropEditorViewModel.cs`. It is resolved the same way everywhere in this file: `var pleaseWaitService = dependencyResolver.Resolve<IBusyIndicatorService>();` followed by `pleaseWaitService.Show()` and, eventually, `pleaseWaitService.Hide()`. `Show()` has an overload `Show(string status)`, but as recorded in `Surprises & Discoveries` above, no visible control in this application actually displays that status text today — it is a no-op for user-visible purposes in this codebase's current WPF wiring. That is why this plan adds a second, independent status-bar text element rather than relying on that overload.

The window's existing `StatusBar` (`CustomPropEditorWindow.xaml`, lines 123-140) contains one `StatusBarItem` holding a `StackPanel` of two `TextBlock` elements bound to `Coordinates.X` and `Coordinates.Y` on the `PropDesigner` control, with a `DataTrigger` that hides that `StackPanel` unless the mouse is over `PropDesigner`. This plan adds a second `StatusBarItem` to the same `StatusBar`, to its left (i.e., inserted before the coordinates item, or in a new leftmost position using `StatusBar`'s default left-to-right layout), bound to the new view-model property and visible only when that property is a non-empty string.

## Plan of Work

### Milestone 1: Create the Jira issue and record acceptance criteria (complete)

[VIX-3968](https://vixenlights.atlassian.net/browse/VIX-3968) was created in the `VIX` Jira project with issue type Improvement ("An improvement or enhancement to an existing feature or task" — a better fit than Bug or New Feature, since the vendor browser already exists and this is UX feedback, not a defect). Its description restates the Purpose / Big Picture section above and lists the same acceptance criteria and test plan captured in this ExecPlan's Milestone 4. The issue was left in its default "New Ticket" status; no workflow transition was made.

This file was renamed from `vendor-browser-loading-indicator.md` to `VIX-3968-vendor-browser-loading-indicator.md` to match the convention of other files in `docs/plans/custom-prop-editor/`.

### Milestone 2: Add a general-purpose status-message property and status-bar UI

Edit `src/Vixen.Modules/App/CustomPropEditor/ViewModels/PropEditorViewModel.cs`. Add a new plain (non-`[Model]`, non-Catel-modeled) public string property named `StatusMessage` with a private setter, following the simplest existing property pattern in this file for a value that is not part of the serialized `Prop` model (a plain auto-implemented `{ get; private set; }` property is sufficient; this is UI-transient state, not part of `Prop`, so it must not be declared with the `[Model]` attribute or backed by `GetValue`/`SetValue`, which are reserved in this file for properties of the `Prop` model). Initialize it to `string.Empty` so the status bar starts empty. Name it `StatusMessage`, not `VendorBrowserStatusMessage` or any other operation-specific name: it is meant to be a single, shared status line that any command in this view model can set while it runs and clear when it finishes, and this plan is only the first caller of it. Document it with an XML `<summary>` that says so explicitly (for example, "A general-purpose, transient status message for the status bar; set by whichever long-running operation is currently in progress and cleared when it completes. Not tied to any specific command."), so a future implementer wiring in a second caller does not need to rename it.

Because Catel view models implement `INotifyPropertyChanged` through their base class, a plain property changed via a private setter inside the class will not automatically raise change notification — check how this file already exposes non-`[Model]`, view-only state that changes at runtime (for example, `ElementTreeViewModel`, which is a `[Model]`-less property set via `SetValue`/`GetValue` like the others). If every existing runtime-mutable property in this file goes through `GetValue`/`SetValue`, use that same mechanism for `StatusMessage` (declare a corresponding `PropertyData` via `RegisterProperty`, matching the exact pattern already used for `Prop` or `FilePath` earlier in the file) so the status bar updates live; do not introduce a different notification mechanism.

Edit `src/Vixen.Modules/App/CustomPropEditor/Views/CustomPropEditorWindow.xaml`. Inside the existing `<StatusBar DockPanel.Dock="Bottom">` element (lines 123-140), add a new `StatusBarItem` before the existing one, containing a `TextBlock` bound to `{Binding StatusMessage}`. Give that `StatusBarItem` a `Visibility` binding (using a `BooleanToVisibilityConverter`-style approach, or a `DataTrigger` in a `Style` on the `StatusBarItem` matching the style already used for the coordinates `StackPanel` at lines 126-134) so it collapses when `StatusMessage` is empty. Reuse `commonConverters:InverseBooleanConverter`-style existing converters if a suitable string-emptiness converter already exists under `Common.WPFCommon.Converters`; otherwise use a `DataTrigger` comparing the bound string to `""` (empty string), mirroring the `DataTrigger` idiom already present in this file rather than adding a new converter class for a single use. Do not name the `StatusBarItem`, any `x:Key`, or any style resource after "vendor" or "vendor browser" — name it generically (for example `StatusMessageBarItem`) so it visibly reads as shared infrastructure to a future reader of the XAML, not as vendor-browser-specific markup.

### Milestone 3: Wire the busy cursor and status message into `OpenVendorBrowserAsync`

Edit `OpenVendorBrowserAsync` in `PropEditorViewModel.cs` (around line 1034). Resolve `IBusyIndicatorService` from the dependency resolver at the top of the method, alongside the existing `dependencyResolver` variable (the method currently resolves `dependencyResolver` partway through, after the first `await`; move that resolution, or add a second one, so `IBusyIndicatorService` is available before any awaited work starts). Call `pleaseWaitService.Show()` and set `StatusMessage = "Loading vendor list..."` before calling `GetVendorUrls()`. Wrap the remainder of the method's body — everything from that point through the end, including the early `return` when `vendorLinks` is empty — in a `try`/`finally` block whose `finally` clause sets `StatusMessage = string.Empty` and calls `pleaseWaitService.Hide()`. This guarantees the cursor and message clear whether the method returns early (no vendor links, no inventories retrieved) or falls through to opening the selection dialog, and also clears them if an unexpected exception propagates. This is the same `try`/`finally` shape any other command would use to drive `StatusMessage`, which is why the property itself carries no vendor-specific naming or typing — only the literal strings assigned inside this particular method mention vendors.

Inside the existing `foreach (var vendorLink in vendorLinks)` loop, set `StatusMessage = $"Retrieving inventory from {vendorLink.Name}..."` at the start of each iteration, before the `try` that calls `ds.GetFileAsStringAsync`. Leave the existing per-vendor `catch` block and its `mbs.ShowError(...)` call unchanged; do not suppress or alter existing error reporting.

Do not change `GetVendorUrls`, `LoadVendorModel`, `XModelInventoryImporter`, `VendorInventoryWindowViewModel`, or any vendor-model-selection or download logic beyond adding the cursor/message wiring described here. `LoadVendorModel` already manages its own `IBusyIndicatorService.Show()`/`Hide()` pair independently (lines 1454 and 1485); leave that as is — the new `finally` in `OpenVendorBrowserAsync` runs before `LoadVendorModel` is ever called (the dialog and model loading happen after the `try`/`finally` region ends, once vendor inventories have already been retrieved), so the two do not overlap or double-hide the cursor.

The intended shape of the edited method's start and end is:

    private async Task OpenVendorBrowserAsync()
    {
        XModelInventoryImporter mi = new XModelInventoryImporter();
        List<ModelInventory> vendorInventories = new List<ModelInventory>();

        var dependencyResolver = this.GetDependencyResolver();
        var pleaseWaitService = dependencyResolver.Resolve<IBusyIndicatorService>();
        pleaseWaitService.Show();
        StatusMessage = "Loading vendor list...";
        try
        {
            var vendorLinks = await GetVendorUrls();
            if (!vendorLinks.Any()) { return; }

            var ds = dependencyResolver.Resolve<IDownloadService>();

            foreach (var vendorLink in vendorLinks)
            {
                StatusMessage = $"Retrieving inventory from {vendorLink.Name}...";
                try
                {
                    var xml = await ds.GetFileAsStringAsync(new Uri(vendorLink.Url));
                    vendorInventories.Add(await mi.Import(xml));
                }
                catch (Exception e)
                {
                    Logging.Error(e, $"An error occurred retrieving the inventory from: {vendorLink.Name}, {vendorLink.Url}");
                    var mbs = dependencyResolver.Resolve<IMessageBoxService>();
                    mbs.ShowError($"Unable to retrieve inventory from {vendorLink.Name}\nEnsure you have an active internet connection.", "Error Retrieving Inventory");
                }
            }

            if (!vendorInventories.Any()) { return; }
            // ...existing dialog / LoadVendorModel logic, unchanged...
        }
        finally
        {
            StatusMessage = string.Empty;
            pleaseWaitService.Hide();
        }
    }

Preserve every other line of the existing method body (the dialog-showing and `LoadVendorModel`-calling logic after the loop) exactly as it is today, just nested inside the `try` block.

### Milestone 4: Validate the change, update Jira, and close the loop

Build the Custom Prop Editor project first to catch compile errors quickly, then run the full repository build, then run the Custom Prop Editor test filter, all from `C:\Dev\Vixen`:

    dotnet build src\Vixen.Modules\App\CustomPropEditor\CustomPropEditor.csproj --no-restore

    msbuild Vixen.sln -m -restore -t:Vixen_Tests -p:Configuration=Release -p:Platform=x64 -p:PlatformTarget=x64 -v:m

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj -c Release --no-build --no-restore -p:Platform=x64 -p:SolutionDir="$(pwd)/" --filter "FullyQualifiedName~App.CustomPropEditor"

Expected result for each: a successful build with 0 errors, and a test run with 0 failed tests (this area of the codebase has no dedicated tests for `PropEditorViewModel` today, so this filter mainly guards against accidental breakage of importer/model tests under the same namespace; note in Progress if the filter matches zero tests, and fall back to the full `Vixen.Tests` suite in that case).

Manually run the Debug build of the application (or use this repository's `run` skill if available) and perform these scenarios: (1) with a working internet connection, choose `Tools > Vendor Browser` and confirm the wait cursor appears immediately and the status bar shows "Loading vendor list...", then updates to name each vendor as its inventory downloads, then both clear when the selection dialog appears; select a model and confirm it still imports correctly (unchanged `LoadVendorModel` behavior). (2) Disconnect from the network (or otherwise force `GetVendorUrls` or the per-vendor download to fail) and confirm the existing error message box(es) still appear, and that the wait cursor and status message are both cleared afterward rather than left stuck. (3) Repeat the successful scenario once more immediately afterward to confirm the cursor/message reliably reset between runs (no leaked state from the `finally` block). If the wait cursor does not visibly appear before the first network call completes during scenario (1), revisit the "no artificial delay" decision above: add `await Task.Delay(200);` immediately after `pleaseWaitService.Show()`, matching the mitigation already validated in `docs/plans/vix-3563-custom-prop-xmodel-import-busy-indicator.md`, and update the Decision Log to record that reversal with the evidence observed.

Update the Jira issue created in Milestone 1 with a comment summarizing the build/test commands run, their results, and the manual validation outcomes, including commit references. Do not transition the issue's workflow status. Update this plan's `Progress`, `Surprises & Discoveries`, `Decision Log`, and `Outcomes & Retrospective` sections with what actually happened.

## Concrete Steps

All commands below run from `C:\Dev\Vixen` in PowerShell unless noted otherwise.

Inspect starting state before editing:

    git status --short
    Get-Content -Path src\Vixen.Modules\App\CustomPropEditor\ViewModels\PropEditorViewModel.cs -TotalCount 40
    Get-Content -Path src\Vixen.Modules\App\CustomPropEditor\Views\CustomPropEditorWindow.xaml -TotalCount 30

Edit only `src\Vixen.Modules\App\CustomPropEditor\ViewModels\PropEditorViewModel.cs` and `src\Vixen.Modules\App\CustomPropEditor\Views\CustomPropEditorWindow.xaml` for the changes described in Milestones 2 and 3. Preserve tab indentation and LF line endings (this file currently mixes tabs and a handful of stray spaces in nearby regions; match the prevailing tab style for any new lines). Do not reformat unrelated code.

Build and test using the commands listed in Milestone 4. Then check the final patch scope:

    git diff --check
    git diff -- src/Vixen.Modules/App/CustomPropEditor/ViewModels/PropEditorViewModel.cs src/Vixen.Modules/App/CustomPropEditor/Views/CustomPropEditorWindow.xaml
    git status --short

## Validation and Acceptance

The change is accepted only when all of the following are true.

Given the Custom Prop Editor is open and the network is reachable, when the user chooses `Tools > Vendor Browser`, then the mouse cursor immediately becomes the wait cursor and the status bar shows a non-empty loading message, which updates to name each vendor being retrieved.

Given the vendor and inventory downloads finish (successfully or with per-vendor failures already reported via message box), when the method returns or the selection dialog opens, then the wait cursor and the status-bar message are both cleared.

Given no vendor links are configured, or every vendor inventory download fails, when `OpenVendorBrowserAsync` returns early, then the wait cursor and status-bar message are still cleared (not left stuck) and no selection dialog appears.

Given a vendor model is subsequently selected and downloaded, when `LoadVendorModel` runs, then its existing independent busy-indicator behavior and import behavior are unchanged.

Given the new `StatusMessage` property and its status-bar binding, when read as code by someone unfamiliar with this task, then nothing in the property's name, its XML documentation, or the XAML `StatusBarItem`'s name suggests it is limited to the vendor browser — it must read as shared, general-purpose status-bar infrastructure that `OpenVendorBrowserAsync` happens to be the first user of.

Given the Custom Prop Editor build, the full `Vixen_Tests` build, and the Custom Prop Editor test filter are run per Milestone 4, when they finish, then the build has 0 errors and the test run has 0 failed tests. Given `git diff --check` is run, then it produces no output and exits successfully.

## Idempotence and Recovery

The edits are additive and localized to two files; they can be safely reapplied if an attempt is interrupted partway. Do not delete generated build output folders or reset unrelated working-tree changes. If the new `StatusBarItem`'s visibility trigger does not compile as sketched (for example, because no existing empty-string converter exists and a `DataTrigger` string comparison behaves unexpectedly with `Binding` type coercion), fall back to adding a small, single-purpose `BooleanToVisibilityConverter`-backed boolean property (for example `IsVendorBrowserStatusVisible`) computed alongside `VendorBrowserStatusMessage` rather than trying to trigger directly off the string; record that fallback in the Decision Log.

If the property-notification approach chosen in Milestone 2 (either a plain property or a Catel `GetValue`/`SetValue`-backed one) turns out not to update the status bar live while `OpenVendorBrowserAsync` runs (visible as the message only appearing after the whole operation completes), switch to the `RegisterProperty`/`GetValue`/`SetValue` pattern used elsewhere in this file if not already used, since that is the pattern proven to raise live-updating change notification in this exact class. Whichever pattern is used, keep the property named `StatusMessage` and keep it generic — do not rename it to something vendor-specific as part of a fallback fix.

## Artifacts and Notes

The pre-change method has this effective shape (no busy indication at all before the selection dialog):

    var vendorLinks = await GetVendorUrls();
    if (!vendorLinks.Any()) { return; }
    var dependencyResolver = this.GetDependencyResolver();
    var ds = dependencyResolver.Resolve<IDownloadService>();
    foreach (var vendorLink in vendorLinks) { /* download, no status */ }
    if (!vendorInventories.Any()) { return; }
    /* show selection dialog */

The post-change method wraps that in `Show()`/`try`/`finally`/`Hide()` plus `StatusMessage` updates, as fully spelled out in Milestone 3 above.

## Interfaces and Dependencies

Use the existing Catel `IBusyIndicatorService` from `Catel.Services` (already imported in `PropEditorViewModel.cs`); no new package or project reference is needed for the cursor.

Add one new public string property, `StatusMessage`, to `VixenModules.App.CustomPropEditor.ViewModels.PropEditorViewModel`. It is a general-purpose, view-only status line (not part of the serialized `Prop` model, and not scoped to any single command) that any current or future long-running operation in this view model may set and clear; `OpenVendorBrowserAsync` is only its first caller. It does not change any existing public API's signature or contract, so no XML documentation update is required beyond a one-line `<summary>` on the new property itself, which must describe it as general-purpose rather than vendor-browser-specific, consistent with this file's existing documented properties (see the `csharp-docs` skill at `.agents/skills/csharp-docs/SKILL.md` for the expected shape of that summary).

Add one new `StatusBarItem` (and, if needed per the Idempotence and Recovery fallback, one new converter or computed boolean property) to `VixenModules.App.CustomPropEditor.Views.CustomPropEditorWindow`. No new XAML namespaces should be required beyond what the file already imports, unless the fallback converter is added, in which case add it under the existing `Common.WPFCommon.Converters` namespace already referenced via `xmlns:commonConverters`.

## Revision Notes

2026-08-08 / Claude: Created this ExecPlan from a direct request to improve the Custom Prop Editor vendor browser's loading feedback. Investigated `OpenVendorBrowserAsync`, `IBusyIndicatorService`'s existing use in this exact class, and confirmed via `docs/plans/vix-3563-custom-prop-xmodel-import-busy-indicator.md` that Catel 6.2's WPF busy indicator only affects the mouse cursor and renders no status text in this codebase, which is why the status-line requirement needs new, real status-bar UI rather than reuse of `IBusyIndicatorService`'s `status` parameter. No Jira issue exists yet; Milestone 1 creates one and this file must be renamed once its key is known.

2026-08-08 / Claude: Revised the plan after explicit feedback that the new status-bar property must not be constrained to vendor-browser messages. Renamed the planned property from `VendorBrowserStatusMessage` to `StatusMessage` throughout, reframed it in the Purpose, Decision Log, Milestone 2, Milestone 3, Validation and Acceptance, and Interfaces and Dependencies sections as general-purpose status-bar infrastructure that any current or future long-running operation in `PropEditorViewModel` may use, and required its XML documentation and XAML element naming to read as generic rather than vendor-specific. The wiring still only touches `OpenVendorBrowserAsync` in this plan; extending it to other commands remains explicitly out of scope here but is now unblocked.

2026-08-08 / Claude: Completed Milestone 1. Created [VIX-3968](https://vixenlights.atlassian.net/browse/VIX-3968) in the `VIX` Jira project as an Improvement, with this plan's Purpose, acceptance criteria, and test plan carried into the issue description. Renamed this file to `VIX-3968-vendor-browser-loading-indicator.md` and updated the Progress and Milestone 1 sections accordingly. No code was changed and no workflow transition was made.

2026-08-08 / Claude: Completed Milestone 3. Wired `IBusyIndicatorService` and `StatusMessage` into `OpenVendorBrowserAsync` exactly as specified: `Show()`/`try`/`finally`/`Hide()` around the whole method body, an initial "Loading vendor list..." message, and a per-vendor "Retrieving inventory from {Name}..." message inside the download loop. `GetVendorUrls`, `LoadVendorModel`, and the post-download dialog/model-assignment logic were left untouched other than being nested one level deeper inside the new `try` block. The Custom Prop Editor project builds with 0 errors. Manual runtime validation and the "no artificial delay" decision's verification remain Milestone 4 work.
