# VIX-2690 Element Tags Workflow Specification

## Status

Draft, refined from `docs\vix-2690-element-tags-initial-workfow.md` through a requirements
clarification pass. This document supersedes the initial workflow doc as the source of truth
for scoping an ExecPlan. It is a specification, not an ExecPlan — see `.agents\PLANS.md` for
the ExecPlan format this spec is meant to feed into.

## Overview

VIX-3933 introduced Element Tags as a core, headless capability: `IElementNode.Tags`
(`ElementTagCollection`), the `ElementTagDefinition` catalog model, the `ElementTagService`
catalog service, and full XML persistence. None of that capability is reachable by a user
today — there is no UI to assign a tag, no UI to see a tag, and none of the built-in tags
(`Deprecated`, `Hidden`, `Prop`) affect any workflow.

This ticket (VIX-2690) is the first user-facing workflow built on top of that foundation. It
answers: given the tag data model already exists, what can a user actually *do* with tags in
Display Setup and the Sequencer, and what behavior should the three built-in tags have?

## Prerequisite: default built-in tag colors (backported to VIX-3933)

The three built-in tags need a default `DisplayColor` (`Deprecated` = red, `Prop` = blue,
`Hidden` = black) for this ticket's dots (items 3/8) and color editor (item 6) to be meaningful
immediately, rather than colorless until a user manually sets one. This was originally drafted
as VIX-2690 scope (an "item 9"), but the user decided default tag colors belong in VIX-3933's
baseline instead — VIX-3933 already owns the built-in tag catalog and its seed values
(`src\Vixen.Core\Sys\BuiltInElementTags.cs`), so a tag's default appearance is core catalog
behavior, not a VIX-2690 UI concern. `docs\vix-3933-element-tags.md` has been updated with this
decision (see its "Risks And Open Questions (Resolved Decisions)" section), and its completed
ExecPlan, `docs\plans\vix-3933-element-tags-api.md`, has a new addendum milestone recording the
follow-up code change still needed there (`CreateDefaults()` currently leaves `DisplayColor`
unset for all three built-ins).

This ticket (VIX-2690) therefore treats default built-in tag colors as an external prerequisite:
its own ExecPlan should either confirm the VIX-3933 addendum has already landed before starting
the items that depend on it (3, 6, 8), or, if it has not landed yet, do that small VIX-3933-side
change first as an explicit early step attributed to VIX-3933 rather than folding it silently
into VIX-2690's own milestones.

## Problem

Vixen currently has no user-friendly way to label elements with simple organizational or
workflow metadata. A user who wants to stop using an element must remember that state
manually, rename the element, move it in the tree, or rely on external notes. This makes it
easy to accidentally sequence against deprecated elements or difficult to identify elements
reserved for a special purpose (e.g. future Prop migration).

## Terminology

Reuses terminology from `docs\vix-3933-element-tags.md` without redefinition: element tag,
tag definition, tag assignment, built-in tag, user-defined tag, direct tags, effective tags.
One term is new to this spec:

- **Tag picker**: the UI surface (a context menu submenu, in this spec) that lists the tags
  currently assignable to a node and lets the user toggle each on or off for that node.

## In Scope

1. Assign or remove a built-in tag (`Deprecated`, `Hidden`, `Prop`) on an `IElementNode` via a
   right-click context menu on the shared element tree control
   (`src\Vixen.Common\Controls\ElementTree.cs`), which automatically makes the capability
   available in both Display Setup and Preview Setup, since both already embed this one
   control.
2. Assign or remove a built-in tag on an `IElementNode` from the Sequencer's own element row
   list, via the same context-menu pattern, added to the Sequencer's separate WinForms row
   label surface (`TimedSequenceRowLabel` /
   `src\Vixen.Common\Controls\TimeLineControl\RowLabel.cs`), which is a distinct control from
   the shared `ElementTree` used by Display Setup/Preview Setup.
3. Show a small colored dot after each element's name in the Sequencer's row label list, one
   per assigned tag that has a `DisplayColor` (same dot treatment as item 8), instead of tinting
   the whole label — this includes `Deprecated` (and any other color-bearing tag visible on a
   row, e.g. `Hidden` on a row that is currently shown because the "Show Hidden" toggle from
   item 4 is on).
4. Toggle-hide `Hidden`-tagged nodes (and their descendants) from the Sequencer element tree
   for the current editing session.
5. Block adding new effects to a `Deprecated`-tagged node in the Sequencer, with visible
   feedback explaining why the action was rejected.
6. Let the user set the `DisplayColor` of each of the three built-in tags.
7. Show a modal dialog when a sequence is opened in the Sequencer if any element in that
   sequence has effects on a `Deprecated`-tagged node, listing the affected elements.
8. In Display Setup (and, by shared-control effect, Preview Setup — see item 1), never filter
   elements out of the tree based on tags; instead, show a small colored dot after each
   element's name for every assigned tag that has a `DisplayColor`, ordered left-to-right by
   ascending `SortOrder`. The Sequencer (item 3) uses this same dot treatment instead of a
   whole-label tint, for visual consistency and to avoid a tag color clashing with the row
   label's own text color.
9. ~~Set a default `DisplayColor` for each of the three built-in tags~~ — moved out of this
   ticket's scope. Default `DisplayColor` values (`Deprecated` = red, `Prop` = blue, `Hidden` =
   black) are now a VIX-3933 baseline prerequisite instead of VIX-2690 scope; see "Prerequisite:
   default built-in tag colors (backported to VIX-3933)" below. This ticket's items 3/8/6 depend
   on that prerequisite being in place but do not implement it themselves.

## Deferred / Explicitly Out of Scope

These are named so a future contributor does not assume they were forgotten:

- **User-defined tag catalog management** (create, rename, delete custom tags) has no UI in
  this ticket. `ElementTagService` already supports it programmatically (from VIX-3933), but
  exposing catalog CRUD in Display Setup/Sequencer is a follow-on ticket. This ticket's tag
  picker only lists tags that already exist in the catalog (the three built-ins, seeded
  automatically by `VixenSystem.EnsureBuiltInElementTags()`).
- **Generic tag-based filtering, sorting, or grouping** of element lists/trees (e.g. "show
  only nodes tagged X", "sort by tag") is not built here. The only filtering behavior in scope
  is the single-purpose Hidden-tag toggle (item 4 above) and the Deprecated color/notification
  behavior (items 3 and 7). Broader filter/sort UI is deferred to a later ticket per the wider
  `docs\vix-3933-element-tags.md` "Filtering, Sorting, And Display" section, which is
  aspirational beyond VIX-2690's literal in-scope list.
- **Element node deletion wizard** that checks sequences before allowing deletion — tracked as
  a separate related issue per the original JIRA description (VIX-2690) and the initial
  workflow doc.
- **Migration wizard** to remap effects from a node marked for deletion to another node when a
  sequence loads with an unknown node reference — noted as the preferred migration path and already exists.
- **Coupling `Deprecated` to the existing `Masked` flag on `Element`** (which actually affects
  rendering/output) is explicitly not done. Tags remain UI-only, per VIX-3933's resolved
  decision. Hiding/blocking in this ticket only affects Sequencer UI and editing; it does not
  stop already-sequenced effects on a deprecated node from rendering during playback or export.
- **Tag inheritance / effective tags** (a tag on a group implicitly applying to children) is
  not implemented. All behavior in this ticket operates on direct tag assignment only,
  consistent with VIX-3933's decision to keep `GetTags`/`HasTag` direct-only.

## Requirement Details

### 1–2. Tag assignment context menu

Both the shared `ElementTree` control and the Sequencer's row label surface are WinForms, not
WPF — this corrects an earlier draft of this spec, which incorrectly assumed Display Setup was
a WPF/Catel surface. `src\Vixen.Application\Setup\SetupElementsTree.cs` (a `UserControl`) hosts
`Vixen.Common.Controls.ElementTree` for Display Setup, and
`src\Vixen.Modules\Preview\VixenPreview\VixenPreviewSetupElementsDocument.cs` hosts the same
`ElementTree` control for Preview Setup. Because both editors embed the one control, adding the
Tags submenu once to `ElementTree`'s existing context menu (see its `ToolStripMenuItem`
enable/disable pattern around `Vixen.Common\Controls\ElementTree.cs:900-920`, e.g.
`renameNodesToolStripMenuItem.Enabled = (SelectedTreeNodes.Count > 0)`) makes tagging available,
with identical behavior, in Display Setup and Preview Setup simultaneously — exactly like the
existing rename/delete/copy/paste/group functions already are, since those are also implemented
once on the shared control rather than duplicated per host.

The submenu lists the three built-in tags as checkable items (checked = currently assigned).
Clicking an unchecked item assigns that tag to the node via `node.Tags`; clicking a checked item
removes it. Multi-select behavior follows the same pattern already used by the existing
`ElementTree` context menu items (see the `SelectedTreeNodes.Count` / `SelectedElementNodes`
checks referenced above): if multiple nodes are selected when the user right-clicks, an
unchecked tag click assigns that tag to every selected node, and a checked tag click removes it
from every selected node that has it. This was confirmed as the intended behavior, matching how
existing bulk actions (e.g. delete, rename, copy/paste) already operate on a multi-selection in
this same control.

The Sequencer's row label context menu is a second, separate WinForms surface
(`TimedSequenceRowLabel` in `src\Vixen.Modules\Editor\TimedSequenceEditor\TimedSequenceRowLabel.cs`,
deriving from `src\Vixen.Common\Controls\TimeLineControl\RowLabel.cs`) — it does not share code
with `ElementTree`, so the Tags submenu must be implemented there independently, following the
same interaction design (checkable submenu items, multi-select applies to all selected rows) for
consistency even though the underlying control is different. Per `CLAUDE.md`/
`dotnet-best-practices`, new UI must not be built in WinForms — but this ticket treats adding a
submenu to each of these two *existing* WinForms context menus as maintaining/extending an
already-WinForms surface, not introducing a new WinForms UI stack. This was a deliberate scoping
decision (see Decision Log) made to avoid a parallel WPF-hosted-in-WinForms control for what is,
in both cases, a small context menu addition.

### 3. Tag dots in the Sequencer row labels

Superseded from an earlier draft of this spec, which called for tinting the whole row label
when a node carries `Deprecated`. The Sequencer's row label surface (`TimedSequenceRowLabel` /
`src\Vixen.Common\Controls\TimeLineControl\RowLabel.cs`) instead uses the same per-tag colored
dot treatment as item 8: after the element name, draw one small filled circle per assigned tag
that has a non-empty `DisplayColor`, ordered left-to-right by ascending `SortOrder`. This
applies to any color-bearing tag on the row, not just `Deprecated` — in practice, today that
means `Deprecated` always, and `Hidden` only on the rows that are visible while the item 4
"Show Hidden" toggle is on (rows hidden by that toggle are not in the list to draw at all).

This was changed from whole-label tinting to per-tag dots for two reasons: consistency with
Display Setup/Preview Setup's item 8 treatment, and to avoid a tag's `DisplayColor` clashing
with or overriding the row label's own text color, which whole-label tinting would do. The
`RowLabel` base class does not currently share any owner-draw code with `ElementTree`, so this
dot-drawing must be implemented independently on `RowLabel`/`TimedSequenceRowLabel`, following
the same minimal-owner-draw technique described in item 8 (draw the label as usual, then paint
extra ellipses after it) rather than copying `ElementTree`'s owner-draw code verbatim, since the
two controls have different base rendering paths.

### 4. Hidden nodes toggle

Confirmed. The Sequencer element row list gets a toolbar toggle button (e.g. "Show Hidden")
that is off by default each time a sequence is opened. When off, nodes carrying the `Hidden`
built-in tag, and all of their descendants, are filtered out of the displayed rows. Toggling
it on redisplays them. This state is session-only (per open sequence editor instance); it is
not persisted to the profile or the sequence file.

### 5. Blocking effects on Deprecated nodes

In the Sequencer, when the user attempts to add an effect to a row whose node is
`Deprecated`-tagged — via drag-and-drop from the effects palette onto the timeline row, or via
paste of copied effects onto that row — the action must be rejected rather than silently
allowed or allowed-with-a-warning. The rejection must be visible: during drag, the drop cursor/
target row should indicate the drop is not allowed (standard WinForms drag-drop
`DragDropEffects.None` feedback); on paste, a status-bar message or similar inline message
should explain the row was skipped because the node is deprecated, without silently discarding
the user's clipboard content for other, non-deprecated rows in a multi-row paste.

### 6. Setting built-in tag colors

Build a basic WPF window, `ElementTagColorEditorWindow` (name indicative; finalize during
implementation), listing the three built-in tags, each with a color picker bound to that tag
definition's `DisplayColor`. Saving updates the `ElementTagDefinition` in the catalog via
`ElementTagService` and persists through the existing `SystemConfig.Tags` XML persistence from
VIX-3933 — no new persistence work is needed, only a UI that calls existing service methods.

This window is WPF even though its two callers (the shared `ElementTree` control and the
Sequencer's row label context menu) are WinForms; a WPF `Window` can be shown modally from
WinForms code directly via `ShowDialog()` (setting the WinForms host as owner through
`System.Windows.Interop.WindowInteropHelper`, no `ElementHost`/`WindowsFormsHost` embedding
required, since the WPF window is a standalone top-level window rather than a control embedded
inside a WinForms form). Both the Display Setup/Preview Setup Tags submenu and the Sequencer
Tags submenu get a "Manage Tag Colors..." entry that opens this same window.

Build this window as a deliberately minimal baseline — a simple list of the three built-in
tags with one color picker each and Save/Cancel — but structure its view model and any
reusable pieces (e.g. a `TagColorPickerItem`-style row view model, the `ElementTagService`
integration) so a later "Tag Manager" ticket (which would add catalog CRUD: create/rename/
delete user-defined tags, per the "Deferred / Explicitly Out of Scope" section above) can
extend this same window rather than replacing it. Do not build speculative extensibility hooks
beyond straightforward, obvious seams (e.g. don't hardcode "exactly 3 rows" where a bound
collection is equally simple); do not build the Tag Manager's create/rename/delete UI itself in
this ticket.

### 7. Deprecated-effects-on-load notification

When a sequence is opened in the Sequencer, after the sequence's elements are resolved, check
whether any element referenced by an effect in that sequence carries the `Deprecated` tag. If
one or more do, show a modal dialog before the user can interact with the timeline, listing
the affected element names (deduplicated). The dialog is informational — it does not block
opening the sequence past dismissal, and does not itself offer remapping (that is the deferred
migration wizard). If no deprecated elements are referenced, no dialog appears and load
proceeds silently as it does today.

### 8. Display Setup / Preview Setup tag dots

Unlike the Sequencer (item 4), Display Setup and Preview Setup never remove elements from the
tree based on tags — every element is always visible in these two editors, regardless of which
tags it carries. Instead, the shared `ElementTree` control's tree-node rendering (the same
render path touched by item 1's context menu addition, in
`src\Vixen.Common\Controls\ElementTree.cs`) is extended so that after an element's name, one
small filled circle ("dot") is drawn for each of the node's assigned tags that has a non-empty
`DisplayColor`, in the tag's `DisplayColor`. Dots are ordered left-to-right by ascending
`ElementTagDefinition.SortOrder`. Tags without a `DisplayColor` contribute no dot. This is a
per-tag indicator, not a single winning color — unlike an earlier draft of this requirement
(now superseded), a node with both `Hidden` and `Deprecated` shows two dots, not one.

This is placed *after* the element name, mirroring — but functionally distinct from — the
existing icon already shown *before* the name: `ElementTree.cs` sets `TreeNode.ImageKey`/
`SelectedImageKey` to `"Group"` for group nodes (the "hamburger" icon) or one of
`"RedBall"`/`"GreenBall"`/`"WhiteBall"` for leaf nodes based on masked/output state (see
`ElementTree.cs:390-399`). That leading icon is the standard WinForms `TreeView` `ImageList`
mechanism: one image per node, always rendered before the label, with no support for arbitrary
post-text content.

A plain appended character (e.g. `●` U+25CF BLACK CIRCLE) was considered as a way to avoid
owner-draw entirely, but a `TreeNode`'s text has exactly one `ForeColor` for the whole label —
appended characters would all render in that single color, not each tag's own `DisplayColor`,
so plain-character dots cannot carry per-tag color without some form of owner-draw involvement
regardless. A second option — real colored circle emoji (🔴🟠🟡🟢🔵🟣⚫⚪) as plain text, genuinely
zero owner-draw — was also considered and rejected: it would force each tag's `DisplayColor`
color picker (item 6) to snap to the nearest of a small fixed emoji palette instead of a
free-form color, and color-emoji glyphs are not reliably rendered by the native, GDI-based
WinForms `TreeView` across Windows versions.

The chosen approach is a **minimal** owner-draw hook, not a full reimplementation of node
rendering: set `TreeView.DrawMode = DrawMode.OwnerDrawText`, handle `DrawNode`, set
`e.DrawDefault = true` so Windows draws the node's icon and label exactly as it does today
(including selection highlighting), and then, after the default draw call, use `e.Graphics` to
paint one small filled ellipse per color-bearing tag, positioned immediately after the default
text's measured bounds (e.g. via `TextRenderer.MeasureText` on the node's label to find where
the dots should start), in that tag's `DisplayColor`, ordered by ascending `SortOrder`. Because
`DrawDefault = true` delegates the label/icon/highlight rendering back to the platform, this is
a small, contained addition — not a rewrite of how nodes are drawn — while still letting each
dot carry the tag's actual, freely-chosen `DisplayColor`. Implementation should still verify
dot legibility against the selected-row highlight background and confirm no perceptible
performance regression on large trees, since `DrawNode` still runs once per visible node per
paint.

Confirmed:

- Hovering a dot (or the row) shows a tooltip naming the tag(s) it represents, so the dot
  color isn't the only way to identify which tag it is. This applies equally to item 3's
  Sequencer dots.
- No maximum dot count / overflow handling (e.g. a "+N" collapse) is designed in this ticket;
  only the three built-in tags exist today, so at most three dots can appear on any node. This
  should be revisited once a future ticket adds user-defined tag color assignment at scale.
- Sequencer item 3 adopts the same per-tag dot treatment as Display Setup/Preview Setup (see
  item 3's Requirement Details above), rather than keeping a whole-label tint, for visual
  consistency across all three editors and to avoid a tag's color clashing with the row label's
  own text color.

### 9. Default built-in tag colors — moved to VIX-3933

Superseded: this requirement is no longer implemented as part of VIX-2690. See "Prerequisite:
default built-in tag colors (backported to VIX-3933)" near the top of this document and the
corresponding addendum in `docs\vix-3933-element-tags.md` and
`docs\plans\vix-3933-element-tags-api.md` for the full detail (which file changes, the hex-string
format question, and the "seed only, not retroactive" persistence note) — all of that content
now lives there rather than being duplicated in this VIX-2690 spec.

## Decisions (Resolved)

- Decision: User-defined tag catalog management (create/rename/delete) is out of scope for
  this ticket; the tag picker only assigns/removes tags that already exist in the catalog.
  Rationale: VIX-2690's own In-Scope list only mentions tagging/untagging "with the built-in
  tags," and keeping catalog CRUD out lets this ticket focus purely on the assignment workflow
  and the three built-in tags' behaviors, which is already substantial UI surface across two
  editors (WPF Display Setup, WinForms Sequencer).
  Date/Author: 2026-07-02, requirements clarification pass with Jeff Uchitjil.

- Decision: Sequencer-side tag UI extends the existing WinForms row label context menu rather
  than embedding a WPF control via `ElementHost`/`WindowsFormsHost`, or deferring Sequencer UI
  to a separate ticket.
  Rationale: The Sequencer's row label surface is entirely WinForms today; a context menu
  submenu is a small, low-risk addition consistent with maintaining existing WinForms UI
  (permitted by `CLAUDE.md`), whereas hosting a WPF control for just a context menu would add a
  second UI stack's complexity for no proportional benefit. This does not set precedent for
  larger new Sequencer UI, which should still avoid new WinForms surfaces where reasonably
  possible.
  Date/Author: 2026-07-02, requirements clarification pass with Jeff Uchitjil.

- Decision: Display Setup and Preview Setup are corrected, in this spec, from an earlier
  (incorrect) assumption of WPF/Catel to their actual architecture — both embed the same
  WinForms `Vixen.Common.Controls.ElementTree` control. The tag-assignment context menu is
  therefore implemented once, on that shared control, rather than twice per host editor.
  Rationale: The user corrected the initial draft's architecture assumption directly.
  Implementing the Tags submenu on the shared `ElementTree` control is both the accurate
  architecture and the lower-effort path, and guarantees Display Setup/Preview Setup stay
  behaviorally identical the same way every other existing `ElementTree` context menu action
  (rename, delete, copy/paste, group, sort, export) already does, by construction rather than
  by parallel maintenance.
  Date/Author: 2026-07-02, requirements clarification pass with Jeff Uchitjil.

- Decision: Built-in tag color editing is a basic, standalone WPF window (`ShowDialog()` from
  WinForms callers, no `ElementHost` embedding needed), shared by both the `ElementTree`
  context menu and the Sequencer row label context menu, and deliberately structured to be
  extended later into a full Tag Manager rather than replaced.
  Rationale: The user asked for a WPF UI usable from both the Sequencer and Display Setup as a
  baseline extendable to the eventual Tag Manager workflow (mentioned as future scope in
  `docs\vix-3933-element-tags.md` and this spec's Deferred section). A WPF `Window` shown
  modally does not require embedding (`ElementHost`/`WindowsFormsHost`) since it is a top-level
  window, not a control hosted inside a WinForms form, keeping the WinForms/WPF boundary simple.
  Date/Author: 2026-07-02, requirements clarification pass with Jeff Uchitjil.

- Decision: The `Prop` built-in tag gets no special UI-visible behavior in this ticket beyond
  being assignable/removable via the Tags submenu — no rendering, filtering, or blocking
  behavior tied to it, unlike `Deprecated` (color + block + notify) and `Hidden` (session hide
  toggle).
  Rationale: Confirmed directly by the user. `docs\vix-3933-element-tags.md` frames `Prop` as
  forward-looking metadata for a future Prop migration workflow, not a behavior to implement
  now.
  Date/Author: 2026-07-02, requirements clarification pass with Jeff Uchitjil.

- Decision: The Deprecated-effects-on-load notification is a blocking modal dialog listing
  affected elements, not a passive toast/banner.
  Rationale: The user should see this before continuing to work in the sequence, since it is
  the primary mechanism (short of the deferred migration wizard) for surfacing accidental use
  of deprecated elements — a passive notification is easy to miss, defeating the point of the
  requirement ("notify the user upon sequence load").
  Date/Author: 2026-07-02, requirements clarification pass with Jeff Uchitjil.

- Decision: Adding effects to a Deprecated-tagged node is hard-blocked (rejected with visible
  feedback), not merely warned-and-allowed.
  Rationale: Matches VIX-2690's literal requirement wording ("prevent new effects from being
  added"), which is stronger than "warn about." Warn-and-allow was considered but rejected as
  not actually preventing anything.
  Date/Author: 2026-07-02, requirements clarification pass with Jeff Uchitjil.

- Decision: Display Setup and Preview Setup never hide elements based on tags. An earlier
  version of this decision used a single "winning" label color chosen by lowest `SortOrder`;
  this was superseded by a per-tag colored dot after the element name, one dot per
  color-bearing assigned tag, ordered left-to-right by ascending `SortOrder` — so multiple tags
  remain simultaneously visible instead of only the highest-precedence one.
  Rationale: The user refined the requirement directly, preferring a dot-per-tag indicator
  (echoing the existing leading group/leaf icon convention in `ElementTree.cs`, but placed
  after the name since multiple dots must coexist) over collapsing multiple tags into one
  color. `SortOrder` is kept as the ordering key for the dots, reusing the same
  `ElementTagDefinition` field the earlier, superseded design also used.
  Date/Author: 2026-07-02, requirements clarification pass with Jeff Uchitjil.

- Decision: The tag dots are implemented via a minimal owner-draw hook on `ElementTree`'s
  `TreeView` (`DrawMode.OwnerDrawText`, `DrawNode` with `e.DrawDefault = true`, then painting
  extra ellipses after the default-drawn label), not full custom node rendering, and not plain
  appended text characters (colored or emoji).
  Rationale: The user asked whether a plain colored character could avoid owner-draw
  altogether. Investigation showed a `TreeNode` has only one `ForeColor` for its whole label, so
  per-tag custom colors cannot be achieved with plain characters without some owner-draw
  involvement regardless; a fixed color-emoji palette was considered and rejected because it
  would force `DisplayColor` to snap to a small fixed set of colors instead of the free-form
  picker built in item 6, and native WinForms `TreeView` (GDI-based) does not reliably render
  color-emoji glyphs. The user chose the minimal owner-draw hybrid (default draw plus painted
  dots) over the emoji-palette alternative once this tradeoff was made explicit.
  Date/Author: 2026-07-02, requirements clarification pass with Jeff Uchitjil.

- Decision: Tag dots get a hover tooltip naming the tag(s) they represent; no dot-count
  overflow handling is designed in this ticket; and the Sequencer (item 3) uses the same
  per-tag dot treatment as Display Setup/Preview Setup (item 8) instead of a whole-label tint.
  Rationale: Confirmed directly by the user. A tooltip keeps dots discoverable without relying
  on memorized color-to-tag mapping. Overflow handling is deferred because only the three
  built-in tags exist today (max 3 dots), so there's nothing to design against yet — revisit
  once user-defined tag colors exist. Sequencer dots (rather than a whole-label tint) keep all
  three editors visually consistent and avoid a tag's `DisplayColor` clashing with or
  overriding the row's own text color.
  Date/Author: 2026-07-02, requirements clarification pass with Jeff Uchitjil.

- Decision: Default `DisplayColor` values for the built-in tags (`Deprecated` = red, `Prop` =
  blue, `Hidden` = black) are implemented as a VIX-3933 baseline change
  (`BuiltInElementTags.CreateDefaults()`), not as VIX-2690 scope, even though the need for them
  was discovered while specifying VIX-2690's dots (items 3/8) and color editor (item 6).
  Rationale: The user decided a built-in tag's default appearance is core catalog behavior owned
  by VIX-3933 (which already owns `BuiltInElementTags.cs` and its seed values), not a VIX-2690
  UI concern. Full detail — the exact file change, the hex-string format question, and the
  "seed only, not retroactive" persistence behavior via `EnsureBuiltInElementTags()` — is
  recorded in `docs\vix-3933-element-tags.md` and its addendum milestone in
  `docs\plans\vix-3933-element-tags-api.md`, not duplicated here. VIX-2690's own items 3/6/8
  depend on this as an external prerequisite; see "Prerequisite: default built-in tag colors
  (backported to VIX-3933)" near the top of this document.
  Date/Author: 2026-07-02, requirements clarification pass with Jeff Uchitjil.

## Open Items Needing Confirmation

None. All questions raised during the clarification passes are resolved; see Decision Log and
the corresponding Requirement Details subsections.

## References

- `docs\vix-2690-element-tags-initial-workfow.md` — original brief this spec refines.
- `docs\vix-3933-element-tags.md` — core Element Tags data model/service spec (VIX-3933).
- `docs\plans\vix-3933-element-tags-api.md` — completed ExecPlan for VIX-3933; defines
  `IElementNode.Tags`, `ElementTagDefinition`, `ElementTagCollection`, `ElementTagService`, and
  `SystemConfig.Tags` persistence that this ticket's UI work builds on. Now also carries an
  addendum milestone for the default built-in tag colors prerequisite (see "Prerequisite:
  default built-in tag colors (backported to VIX-3933)" above).
- `src\Vixen.Common\Controls\ElementTree.cs` — shared WinForms tree control embedded by both
  `src\Vixen.Application\Setup\SetupElementsTree.cs` (Display Setup) and
  `src\Vixen.Modules\Preview\VixenPreview\VixenPreviewSetupElementsDocument.cs` (Preview
  Setup); the Tags context-menu addition (requirement 1) lives here.
- `src\Vixen.Modules\Editor\TimedSequenceEditor\TimedSequenceRowLabel.cs` and
  `src\Vixen.Common\Controls\TimeLineControl\RowLabel.cs` — the Sequencer's separate row label
  surface; the Tags context-menu addition (requirement 2) lives here.
- JIRA VIX-2690 (`https://vixenlights.atlassian.net/browse/VIX-2690`) — original issue,
  currently titled "Eliminated element setting," status Accepted. Its description is the
  informal origin of this feature; VIX-3933 and this spec have since formalized it into the
  Element Tags model. The JIRA issue itself should be updated with the finalized requirements,
  acceptance criteria, and test plan once this spec and its resulting ExecPlan are approved.
