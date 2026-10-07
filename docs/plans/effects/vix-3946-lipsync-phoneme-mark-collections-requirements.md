# VIX-3946 LipSync Phoneme Mark Collection Filter Specification

## Purpose

VIX-3946 improves the LipSync effect setup workflow by filtering the Mark Collection selector to collections that are explicitly tagged as phoneme collections. Today the LipSync effect tries to auto-select the first `Phoneme` Mark Collection, but if the user changes the selection, the dropdown can contain many unrelated Mark Collections such as beat, phrase, word, or generic tracks. This makes the correct selection harder to find and increases the chance of choosing a collection that is not intended for LipSync.

After this change, the LipSync Mark Collection dropdown should show only Mark Collections whose `CollectionType` is `MarkCollectionType.Phoneme`, except when preserving an existing user selection that is not tagged as `Phoneme`. Existing sequences that rely on an untagged collection with valid phoneme mark labels must continue to render until the user can correctly tag the collection.

This document is a detailed specification. It is not an ExecPlan, but it is intended to contain enough detail for a self-contained ExecPlan under `docs/plans/effects/` to be created from it after approval.

## Current Implementation Context

The LipSync effect module lives under `src/Vixen.Modules/Effect/LipSync/`.

The main runtime class is `src/Vixen.Modules/Effect/LipSync/LipSync.cs`. The serialized data class is `src/Vixen.Modules/Effect/LipSync/LipSyncData.cs`.

LipSync stores the selected Mark Collection as a `Guid` in `LipSyncData.MarkCollectionId`. The public effect property exposed to the effect editor is named `MarkCollectionId`, but its getter and setter convert between the stored id and the collection display name:

- The getter returns the selected collection's `Name`.
- The setter finds a collection by `Name` and stores its `Id`.

The property currently uses `IMarkCollectionNameConverter` from `src/Vixen.Core/TypeConverters/IMarkCollectionNameConverter.cs`. That converter is shared by several effects and returns all collection names without filtering by collection type. It should not be changed globally for this issue because other effects need non-phoneme Mark Collection choices.

LipSync already contains phoneme-aware auto-selection in two places:

- `SetupMarks()` checks for available non-generic collections, then selects the first `MarkCollectionType.Phoneme` collection when no collection is selected.
- The `LipSyncMode` setter also selects the first `MarkCollectionType.Phoneme` collection when switching into `LipSyncMode.MarkCollection` and no collection is selected.

Rendering uses the stored `Guid` to locate the selected collection. This is important for compatibility: an existing LipSync effect can keep rendering from a non-phoneme collection as long as the stored collection id remains valid.

## User-Facing Requirements

The LipSync effect's Mark Collection selector must show Mark Collections that are explicitly tagged as `Phoneme`.

A Mark Collection qualifies for the normal LipSync dropdown list only when:

- `collection.CollectionType == MarkCollectionType.Phoneme`

No other collection type qualifies by default. Do not infer phoneme status from collection name, linked collection metadata, parent-child relationships, or the mark labels contained in the collection.

If a LipSync effect already has a selected Mark Collection and that collection still exists, the effect must retain that selection even when the collection is not tagged as `Phoneme`.

If the retained collection is not tagged as `Phoneme`, the dropdown should include it as a legacy/current selection so the setup UI can display the current value. The display text should be the collection name with this warning appended if the property editor can support it without major changes:

    Collection Name (Collection is not Phoneme)

If the existing selection editor cannot support separate display text and committed value without major changes, the implementation may show the plain collection name. In that fallback case, the behavior still must preserve the selected collection and continue excluding unrelated non-phoneme collections.

When no Mark Collection is selected and at least one `Phoneme` collection exists, LipSync should continue today's behavior of selecting the first `Phoneme` collection.

When no Mark Collection is selected and no `Phoneme` collections exist, LipSync should keep today's behavior. The current behavior in `SetupMarks()` is to fall back to `LipSyncMode.Phoneme` when all available collections are generic or no usable collection exists. The implementation should not introduce a new empty-selector state unless the later ExecPlan identifies that today's fallback has already changed in code.

Changing the selection away from a retained non-phoneme legacy collection should remove that legacy-only option from the dropdown as soon as practical. The least invasive acceptable behavior is:

- Include the current non-phoneme collection only while it is the currently selected collection.
- Once the user selects a `Phoneme` collection and the property grid refreshes, the old non-phoneme collection should disappear from the standard values list.

Do not add a broader "show all" toggle, modal warning, or automatic collection tagging as part of this issue.

## Compatibility Requirements

Existing sequences must not be broken just because a LipSync effect is using an untagged or non-phoneme Mark Collection.

The implementation must not clear `LipSyncData.MarkCollectionId` solely because the selected collection is not tagged as `Phoneme`.

The implementation must not replace an existing selected non-phoneme collection with the first phoneme collection during render, effect load, property refresh, or thumbnail generation.

The implementation must not mutate Mark Collections. In particular, it must not automatically change `CollectionType` to `Phoneme`.

The implementation must not validate the selected collection's mark text beyond the behavior LipSync already has today. If the marks are named `AI`, `E`, `REST`, or other values LipSync already recognizes, rendering should continue to work.

The implementation must preserve the shared Mark Collection behavior for other effects. Effects such as Text, Alternating, Dissolve, Fireworks, Shapes, Strobe, and State must continue using their current Mark Collection dropdown behavior unless a separate issue changes them.

## Proposed Implementation Direction

The least invasive implementation is to add a LipSync-specific Mark Collection name converter and apply it only to `LipSync.MarkCollectionId`.

Recommended target:

- Keep `IMarkCollectionNameConverter` unchanged.
- Add a LipSync-specific converter in the LipSync effect module, for example `LipSyncMarkCollectionNameConverter`.
- Replace `[TypeConverter(typeof(IMarkCollectionNameConverter))]` on `LipSync.MarkCollectionId` with the LipSync-specific converter.
- Have the converter return:
  - all Mark Collections with `CollectionType == MarkCollectionType.Phoneme`
  - plus the currently selected collection if it exists and is not already in the phoneme list

This keeps the behavior scoped to the LipSync effect and avoids changing the shared converter contract used by other effects.

The main implementation risk is the requested warning suffix. The existing property setter expects the selected value to match a Mark Collection `Name`. If the dropdown returns `Collection Name (Collection is not Phoneme)` as the actual standard value, the current setter will not find the collection by name.

The ExecPlan should research whether the existing property editor supports a distinct display label and committed value for standard values. If it does, use the warning suffix for display and commit the real collection name. If it does not, avoid invasive property editor work and show the plain collection name for the retained legacy selection.

If the warning suffix is not feasible without property editor changes, the implementation should document that limitation in the code change notes and still satisfy the core behavior:

- unrelated non-phoneme collections are hidden
- the current legacy non-phoneme selection is preserved and selectable/displayable
- selecting a phoneme collection removes the legacy exception on refresh

## Detailed Selection Rules

Given the Mark Collections:

- `Beat Marks`, `Generic`
- `Words`, `Word`
- `Phonemes`, `Phoneme`

The LipSync dropdown should show:

- `Phonemes`

Given the Mark Collections:

- `Phonemes A`, `Phoneme`
- `Phonemes B`, `Phoneme`

The LipSync dropdown should show:

- `Phonemes A`
- `Phonemes B`

Given an existing LipSync effect whose stored `MarkCollectionId` points to:

- `Old Manual Track`, `Generic`

And the sequence also contains:

- `Phonemes`, `Phoneme`
- `Words`, `Word`

The LipSync dropdown should show:

- `Old Manual Track (Collection is not Phoneme)`, if display aliases are feasible
- `Phonemes`

Or, if display aliases are not feasible:

- `Old Manual Track`
- `Phonemes`

It should not show `Words`.

After the user selects `Phonemes`, the next refreshed dropdown should show only:

- `Phonemes`

Given an existing LipSync effect whose stored `MarkCollectionId` points to a deleted collection, the dropdown should show only phoneme collections. The missing collection should not appear as a synthetic option.

Given no selected collection and no phoneme collections, LipSync should keep today's fallback behavior rather than listing generic collections.

## Data Model

No serialized data model change is expected.

`LipSyncData.MarkCollectionId` should remain a `Guid`. This is the correct persistence model for compatibility because it allows existing effects to retain a non-phoneme selected collection without relying on name matching during render.

The converter/filtering behavior should be UI-facing only. Rendering should continue to resolve the selected collection by `_data.MarkCollectionId`.

## Acceptance Criteria

Opening a LipSync effect with mixed Mark Collection types shows only collections tagged as `Phoneme`.

Creating or editing a LipSync effect with no current collection continues to auto-select the first available `Phoneme` collection.

Opening an existing LipSync effect that selected a non-phoneme collection keeps that collection selected and continues rendering from it.

The retained non-phoneme selected collection appears in the dropdown while it is selected. If the editor can support warning display text without major changes, the option is labeled with `(Collection is not Phoneme)`.

After changing from a retained non-phoneme collection to a phoneme collection, the old non-phoneme collection is no longer included in the dropdown after refresh.

No unrelated non-phoneme collections appear in the LipSync dropdown.

Other effects using the shared Mark Collection converter continue to show their existing unfiltered Mark Collection lists.

Existing LipSync rendering behavior for valid phoneme mark labels is unchanged.

## Automated Test Guidance

The ExecPlan should include focused tests for the filtering helper or converter behavior if the converter can be exercised without the full effect property grid.

Recommended test cases:

- Returns only `Phoneme` collections when no legacy selected collection exists.
- Includes the currently selected `Generic` collection plus phoneme collections.
- Includes the currently selected `Word` or `Phrase` collection plus phoneme collections.
- Does not include unrelated `Generic`, `Word`, or `Phrase` collections.
- Returns no generic fallback values when no phoneme collections exist and no selected collection exists.
- Does not include a deleted or missing selected collection.
- Removes the legacy exception after the selected collection changes to a phoneme collection.

If the warning suffix is implemented, tests should verify that selecting the warning-labeled item still commits the real collection name or id.

## Manual Validation

Create a timed sequence with Mark Collections of several types: at least one `Generic`, one `Word`, one `Phrase`, and one `Phoneme`. Add or edit a LipSync effect and confirm the Mark Collection dropdown only shows the `Phoneme` collection.

Create or load a LipSync effect whose stored collection id points to a `Generic` collection containing valid phoneme mark labels. Confirm the effect still renders from that collection and the dropdown still displays the selected collection.

If warning labels are implemented, confirm the retained generic collection appears as `Collection Name (Collection is not Phoneme)`.

Select a real `Phoneme` collection, close and reopen or refresh the editor, and confirm the old generic collection no longer appears in the LipSync dropdown.

Open at least one other mark-driven effect, such as Text, and confirm its Mark Collection dropdown still shows the normal unfiltered list.

Remove the selected collection from the sequence and confirm LipSync handles the missing selection without crashing.

## Out of Scope

This issue does not add automatic detection of phoneme-compatible mark labels.

This issue does not retag Mark Collections.

This issue does not change the Marks Docker tagging workflow.

This issue does not change the shared Mark Collection converter for other effects.

This issue does not redesign effect property selection to use Mark Collection ids instead of names.

This issue does not add warnings during render or sequence load for non-phoneme selected collections.

## Open Implementation Questions for ExecPlan

The requirements decisions are settled for this specification, but the ExecPlan should answer two implementation details after inspecting the property editor:

- Can the selection editor display `Collection Name (Collection is not Phoneme)` while committing the underlying collection name?
- What property refresh hook is already used after a LipSync property changes, and is `OnPropertyChanged()` plus the existing `TypeDescriptor.Refresh(this)` pattern enough to remove the legacy exception promptly after selection changes?
