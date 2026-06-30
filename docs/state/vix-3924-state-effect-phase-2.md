# State Effect Phase 2: Custom Render Source

## Purpose

The first State effect milestone lets a sequence author render either a single State item selection or State item names driven by a Mark Collection. Phase 2 adds a third render source, `Custom`, for the workflow where the author wants an ordered list of State item activations inside the effect itself and wants to override the configured State item color per activation.

After this change, a user can add a State effect, choose a State definition, set `Render Source` to `Custom`, add rows such as `Open`, `Closed`, `Open`, assign a color to each row, and render those rows across the effect duration using the same playback and timing semantics already used for Mark Collection item sequences. In `Iterate` playback mode, the user can also add `<None>` rows to create intentional blank timing slots. This removes the need to create a Mark Collection solely to sequence a subset of State items, and it provides per-row color override capability that Mark Collections do not provide.

## Relationship to VIX-3924 Phase 1

This specification extends `docs/state/vix-3924-state-effect.md`. All existing State effect behavior remains in force unless this document explicitly changes it.

The existing render sources are:

- `State Item`, which uses the selected State item or `<All>`.
- `Mark Collection`, which uses mark text to activate State item names over time.

Phase 2 adds:

- `Custom`, which uses an ordered, user-editable collection of State item rows stored on the effect.

The existing `State`, `Playback Mode`, `Iterations`, rendering, assignment lookup, discrete-color fallback, render-pass reset, visual representation, and Mark Collection listener behavior continue to apply.

## Terminology

A **custom State row** is one entry in the new effect-owned collection. It contains one State item selection and one color value.

A **State item selection** in a custom row is a selected State item from the current State definition. In `Iterate` playback mode only, the selection may also be the special value `<None>`.

`<None>` is an Iterate-only custom-row placeholder that consumes timing but renders no intents. It is equivalent to an empty or unknown segment in Mark Collection Iterate mode. `<None>` is not offered in `Default` playback mode because Default mode enables every row in the collection simultaneously.

A **row color** is the color stored on a custom State row. When the row activates a State item, this row color replaces the State item's configured default color for that row only.

## Scope

In scope:

- Add `Custom` as a third `StateRenderSource` value.
- Add a persisted, ordered custom State row collection to `StateData`.
- Expose the custom row collection in the Effect Editor only when `Render Source` is `Custom`.
- Allow adding, removing, and editing custom rows through the same expandable object collection pattern used by the Wave effect's `Waves` collection and the Liquid effect's `Emitters` collection. Reordering is out of scope for this enhancement.
- Let each custom row choose one State item from the selected State definition. In `Iterate` playback mode only, also allow `<None>`.
- Default each new row to the selected State item's configured color.
- Allow the row color to be changed independently of the State item default.
- In `Default` playback mode, allow each State item to appear in the collection only once.
- In `Iterate` playback mode, allow the same State item to appear in the collection multiple times.
- Render the custom rows using the selected State definition's assignments and the row color override.
- Preserve the existing State effect render semantics for assignment lookup, leaf expansion, discrete-color fallback, overlapping items, coalescing, dirty-state invalidation, and visual representation.
- Add focused tests for persistence, option generation, timing, color override, `<None>`, duplicate State item rows, invalid/missing selections, and existing mode regression coverage.

Out of scope:

- Adding per-row intensity, curves, gradients, easing, or overlap strategy.
- Adding a custom Mark Collection type.
- Changing State property setup, assignment editing, xModel import, or State definition discovery.
- Changing the existing `State Item` or `Mark Collection` render sources except where shared helpers need to accept the new custom source.
- Creating a custom WPF editor outside the standard expandable object collection/property grid pattern unless the existing pattern cannot satisfy the row-editing requirements.

## Editor Requirements

The Effect Editor must present controls in this order:

1. `State`
2. `Render Source`
3. `State Item`, `Mark Collection`, or `Custom State Items`, depending on `Render Source`
4. `Playback Mode`
5. `Iterations`, only when `Playback Mode` is `Iterate`
6. Existing visual settings, such as `ShowEffectVisual`

`Render Source` must offer `State Item`, `Mark Collection`, and `Custom`.

When `Render Source` is `State Item`, the existing `State Item` selector is visible and the Mark Collection selector and custom row collection are hidden.

When `Render Source` is `Mark Collection`, the existing Mark Collection selector is visible and the State Item selector and custom row collection are hidden.

When `Render Source` is `Custom`, the custom row collection is visible and the State Item selector and Mark Collection selector are hidden.

Hidden selections and collections are retained. Switching away from `Custom` must not clear or mutate the custom rows. Switching back to `Custom` must restore the previous custom rows.

Switching `Render Source` to `Custom` must preserve the current `Playback Mode`. The effect must not automatically change `Playback Mode` to `Iterate`.

Custom mode requires at least one custom State item row when the selected State definition has at least one real State item. When `Render Source` changes to `Custom` and the custom row collection is empty, the effect must add one row using the same defaulting rules as a user-added row: select the first real State item in the selected State definition and copy that State item's configured color. If there are no real State items in the selected State definition, the collection may remain empty because no valid State item can be selected.

While `Render Source` is `Custom`, the collection editor should prevent removing the last remaining custom row when the selected State definition has at least one real State item. When `Render Source` is not `Custom`, the retained custom collection may be empty.

When `Playback Mode` changes from `Iterate` to `Default`, the custom row collection must be normalized for Default mode. Remove duplicate State item rows and keep the first row found for each State item ID in collection order. Remove `<None>` rows because `<None>` is not valid in Default mode. This cleanup should mark the effect dirty and refresh the row editors.

The custom row collection display name should be `Custom State Items` unless an implementation discovery shows an existing naming convention that strongly favors another label.

Each custom row appears as an expandable object in the property grid. The row exposes these editable properties in this order:

1. `State Item`
2. `Color`

In `Default` playback mode, the row `State Item` selector must list every State item row in the selected State definition in State definition order. `<None>` must not be listed because Default mode enables all rows simultaneously and a non-rendering row has no useful meaning.

In `Default` playback mode, the row `State Item` selector should not offer State items that are already used by another custom row in the same collection. Switching from `Iterate` to `Default` removes duplicate rows automatically, keeping the first occurrence of each State item ID in collection order.

In `Iterate` playback mode, the row `State Item` selector must list `<None>` first, followed by every State item row in the selected State definition in State definition order. Unlike the top-level `State Item` selector, this custom row selector must not collapse duplicate State item names into one option unless the implementation confirms that duplicate names cannot be uniquely represented in a row selector.

If the State definition contains duplicate State item names, the row selector must disambiguate options while preserving row identity. The label should use known State item context that helps the user identify the intended row. Preferred options, in order, are:

1. The State item name plus a concise assignment summary, such as `Open (Left Eye)` or `Open (Left Eye, Right Eye)`, when assigned element names are available and short enough to scan.
2. The State item name plus row ordinal in State definition order, such as `Open (2)` or `Open (3)`, when assignments are unavailable or too long.
3. The State item name plus a short stable ID suffix, such as `Open (a1b2c3)`, only if assignment and ordinal labels are still ambiguous.

Each duplicate option must map to one specific `StateItemData.Id`.

If the existing selection-editor infrastructure cannot support duplicate display labels, the implementation plan must call out the limitation and propose the least confusing fallback before coding.

When a user adds a new custom row:

- If the selected State definition has at least one State item, the new row defaults to the first State item in State definition order and copies that State item's current configured color.
- If the selected State definition has no State items and `Playback Mode` is `Iterate`, the new row defaults to `<None>` and uses a harmless color default such as white. The row renders nothing until changed to a valid State item.
- If the selected State definition has no State items and `Playback Mode` is `Default`, adding a custom row should be disabled if the collection editor supports disabling adds. If disabling adds is not practical with the existing collection editor, the new row may default to a missing/no-item placeholder and render nothing, but the implementation plan must call out that fallback before coding.

When a row's `State Item` value changes:

- Selecting `<None>` clears the row's State item ID and leaves the row color unchanged. `<None>` can only be selected in `Iterate` playback mode.
- Selecting a State item stores that State item's stable ID on the row.
- Selecting a State item also updates the row color to that State item's current configured color. This makes the common workflow fast: choose item, then optionally adjust color.

When a row's `Color` value changes, the selected color is used for only that row. It must not modify the State definition's default State item color.

The row `Color` editor must honor the color capabilities of the element set assigned to the selected State item. If the selected State item's assigned elements are full-color capable, the user sees the normal full-color picker. If the selected State item's assigned elements are discrete-color elements, the user sees the appropriate discrete-color picker constrained to the colors supported by that State item's assigned element set. The color picker context must be based on the selected row State item assignments, not the entire effect target and not unrelated State items in the same State definition.

If a selected State item assignment set contains a mix of color capabilities, the implementation plan must inspect the existing Vixen color-picker behavior and choose the closest established behavior. The preferred behavior is to offer only colors that can be rendered by the assigned elements without surprising fallback. If the existing editor infrastructure cannot support mixed assignment color contexts directly, the plan must call out the limitation and define the least misleading fallback before coding.

When the selected top-level `State` definition changes:

- The custom row collection is cleared.
- Clearing the collection prevents rows from silently pointing at unrelated State items in a different State definition.
- If `Render Source` is `Custom` and the newly selected State definition has at least one real State item, the effect immediately adds one row using the same defaulting rules as a user-added row.
- If `Render Source` is not `Custom`, the retained custom collection remains empty until the user switches back to `Custom` or adds rows.

## Data Model Requirements

Add a serializable custom row data type under `src/Vixen.Modules/Effect/State`, tentatively named `CustomStateItemData`.

`CustomStateItemData` must persist:

- `Guid StateItemId`, using `Guid.Empty` for `<None>`.
- `Color Color`, storing the row color override.

`CustomStateItemData` must support cloning. Clones must preserve the selected State item ID and color.

`StateData` must add a persisted ordered list:

- `List<CustomStateItemData> CustomStateItems`

The list must be initialized to an empty list for existing effects. Existing saved effects that do not contain the new data member must deserialize without throwing.

`StateData.CreateInstanceForClone()` must deep-clone the custom rows. Cloning an effect must not share row objects between the original and clone.

If any new public or protected C# type or member is added, it must include XML documentation per `.agents/skills/csharp-docs/SKILL.md`.

## Runtime Model Requirements

Expose an editor-facing expandable collection property on `State`, tentatively named `CustomStateItems`.

The implementation should follow the same broad pattern used by:

- `src/Vixen.Modules/Effect/Wave/Wave/WaveFormCollection.cs`
- `src/Vixen.Modules/Effect/Wave/Wave/Waveform.cs`
- `src/Vixen.Modules/Effect/Liquid/Liquid/EmitterCollection.cs`
- `src/Vixen.Modules/Effect/Liquid/Liquid/Emitter.cs`

The row view object should be a small expandable object, tentatively named `CustomStateItem`, backed by `CustomStateItemData`.

The collection should notify the parent State effect when rows are added, removed, or edited so `IsDirty` is set and the effect rerenders.

The row object needs access to the parent State effect, or to a small option provider, so it can list valid State item options for the currently selected State definition. Prefer a narrow provider interface or internal callback if that keeps the row object simpler and easier to test.

Avoid WPF-specific types inside the effect or row model. The Effect Editor integration should use existing attributes, type converters, and expandable object infrastructure.

## Custom Rendering Requirements

Custom rendering is active only when `Render Source` is `Custom`.

If no State definition is selected, rendering produces no intents.

If the selected State definition has no State items, rendering produces no intents.

If the custom row collection is empty, rendering produces no intents.

Each custom row contributes one timing slot in collection order when `Playback Mode` is `Iterate`. `<None>` rows and missing State item rows consume timing but render no intents.

When `Playback Mode` is `Default`:

- All valid custom rows are active simultaneously for the full effect duration.
- This behaves like selecting `<All>` in the top-level `State Item` render source, except the custom row list defines what counts as all.
- Each State item can be included only once in the Default custom row set.
- Switching from Iterate to Default removes duplicate rows for the same State item, keeping the first valid row in collection order.
- `<None>` is not offered in Default mode. Switching from Iterate to Default removes `<None>` rows.

When `Playback Mode` is `Iterate`:

- The full custom row sequence repeats `Iterations` times within the effect duration.
- Each row slot receives an equal share of the effect duration after applying `Iterations`.
- `<None>` rows and missing rows consume their equal time slot and render nothing.
- Duplicate custom rows are allowed and remain significant. They render in their listed order on each repetition.

For a valid custom row, the stored row `StateItemId` selects exactly one `StateItemData` row by stable ID. Unlike the top-level State Item render source, a custom row should not activate every same-named State item unless the user intentionally adds those rows to the custom collection.

The row color replaces the selected State item's configured color for that row's render intervals. All assignment lookup still comes from the selected State item. Row colors use simple `System.Drawing.Color` values because State items currently support simple colors. Color gradients are a possible future enhancement and are out of scope for phase 2.

Rendering must continue to:

- Affect only element assignments defined by the selected State item.
- Expand assigned groups to current descendant leaves during each render pass.
- Skip deleted or out-of-target-scope assignments.
- Resolve discrete-color fallback per leaf. If the row color is not supported by a discrete leaf, use the first supported color. If the leaf has no supported colors, skip it.
- Add overlapping intents in row order so the rendering pipeline can mix them later.
- Start each render pass from an empty intent collection.
- Coalesce only adjacent intervals when coalescing does not change ordering or output.

## Mark Collection and State Item Regression Requirements

Existing `State Item` and `Mark Collection` behavior from phase 1 must remain unchanged.

Adding `Custom` must not change:

- `<All>` behavior in `State Item`.
- Unique-name grouping in top-level `State Item`.
- Mark text parsing, including case sensitivity, comma trimming, unknown segments, empty segments, gaps, overlaps, clipping, and `Iterations`.
- Mark Collection listener registration, removal handling, and invalidation.
- State definition discovery, missing-State behavior, and selected State definition ID retention.

## Test Requirements

Add focused unit tests under `src/Vixen.Tests/Effect/State`.

Data and cloning tests:

- New `StateData` instances initialize `CustomStateItems` to an empty list.
- Deserialized or manually constructed data with `CustomStateItems == null`, if possible, is normalized before use.
- Cloning deep-copies custom rows and preserves row State item IDs and colors.

Editor option tests:

- `RenderSource` includes `Custom`.
- Switching to `Custom` preserves the current `Playback Mode`.
- Switching to `Custom` adds one custom row when the collection is empty and the selected State definition has at least one real State item.
- Switching to `Custom` keeps existing custom rows unchanged when the collection already has rows.
- The custom row collection requires at least one row while `Render Source` is `Custom`.
- `Custom State Items` is visible only for `Custom`.
- `State Item` and `Mark Collection` remain visible only for their matching render sources.
- Custom row State item options include State item rows from the selected State definition.
- `<None>` appears in custom row State item options only when `Playback Mode` is `Iterate`.
- In `Default`, already-selected State items are not offered for additional rows.
- In `Iterate`, already-selected State items remain available for additional rows.
- Changing `Playback Mode` from `Iterate` to `Default` removes duplicate State item rows, keeps the first row for each State item, removes `<None>` rows, marks the effect dirty, and refreshes row editors.
- Duplicate State item names are represented in a way that maps to stable State item IDs.
- Missing row State item IDs display a missing placeholder and do not silently clear.
- Changing the top-level `State` definition clears existing custom rows and, when `Render Source` is `Custom`, seeds the first real State item from the newly selected State definition.
- Custom row color editors use the selected State item's assigned element set to choose full-color versus discrete-color picker behavior.

Rendering planner tests:

- Custom Default mode renders every valid row for the full effect duration.
- Custom Default mode permits only one row per State item.
- Custom Default mode does not offer `<None>`.
- Custom Iterate mode divides the effect duration equally across row count times `Iterations`.
- Custom Iterate mode permits duplicate State item rows, preserves duplicate rows, and consumes time for `<None>` and missing rows.
- Custom Iterate mode repeats the sequence when `Iterations` is greater than `1`.
- Empty custom rows render no intervals.

Integration-style rendering tests, where existing test infrastructure makes them practical:

- Custom rows render the selected State item's assignments using the row color, not the State item default color.
- Custom row color editing honors the selected State item's assigned element color capabilities.
- Row color uses the existing discrete-color fallback behavior.
- Deleted or out-of-scope assignments are skipped.
- Iterate duplicate rows add intents in row order.
- Coalescing does not merge across a `<None>` timing gap or across rows with different colors.

Regression tests:

- Existing `Effect.State` tests for State Item and Mark Collection still pass.
- Existing `Property.State` tests still pass.

Recommended focused validation commands for the eventual implementation plan:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Effect.State" --no-restore
    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Property.State" --no-restore
    dotnet build src\Vixen.Modules\Effect\State\State.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore
    git diff --check

## Manual Acceptance Scenario

1. Create or open a profile with a prop that has a State property, one State definition, and at least three State item rows with assignments and distinct default colors.
2. Add a State effect to the prop.
3. Select the State definition.
4. Set `Render Source` to `Custom`.
5. Confirm the editor shows the custom row collection, hides the top-level `State Item` and `Mark Collection` selectors, and automatically adds one row for the first real State item when the collection was empty.
6. Add a second row while still in `Default`. Confirm `<None>` is not offered and confirm the already-added State items are not offered for additional rows.
7. Change the color of the first row to a color different from the State item default.
8. Set `Playback Mode` to `Iterate` and `Iterations` to `2`.
9. Confirm `<None>` is now offered, then add a `<None>` row between the first and second State item rows and add the first State item again.
10. Render or play the effect.
11. Confirm the sequence plays row 1, blank, row 2, row 1, row 1, blank, row 2, row 1 across the effect duration, and that the overridden row color is used wherever that row activates.
12. Switch `Render Source` to `State Item`, then back to `Custom`, and confirm the custom rows and colors are still present.
13. Change the selected State definition and confirm stale custom rows are cleared and replaced with the first real State item from the newly selected State definition while `Render Source` remains `Custom`.
14. Confirm the timeline visual representation includes a `Custom` hint when `Render Source` is `Custom`.

## Jira Update Requirement for the Future ExecPlan

The first implementation milestone of the ExecPlan created from this specification must update Jira issue VIX-3924 in the VIX project.
[State Effect JIRA Issue](https://vixenlights.atlassian.net/browse/VIX-3924)

The Jira update should describe this work as an additional enhancement discovered during validation and user testing of the State effect. The update should include:

- Requirements summary for the `Custom` render source.
- High-level design: persisted custom row list, expandable object editor pattern, per-row State item ID and color, render planner extension.
- Acceptance criteria from this specification.
- Test plan and manual validation steps.
- Risks and open questions.

The ExecPlan should read the current Jira issue before composing the final paste text, because the issue may already contain phase 1 implementation details.

## Risks and Concerns

Duplicate State item names are the main editor risk. The existing top-level State Item selector intentionally collapses duplicate names into groups, but custom rows need stable row identity so one duplicate row can be selected independently from another. The implementation plan must validate whether the existing selection editor can display duplicate labels or whether labels need disambiguation.

The expandable object collection pattern may require a runtime object layer separate from serialized data. The plan should keep serialization simple and verify that edits flow both directions without sharing mutable row objects between cloned effects.

Changing a row's State item resets the row color to that State item's default color. This is intentional because choosing a new item should also start from that item's configured default.

Changing the top-level State definition clears the custom row collection. This avoids stale rows that refer to item IDs from a different State definition, but users must recreate custom rows after changing State definitions.

Default playback mode for custom rows treats all valid rows as simultaneous, matching the top-level `State Item` `<All>` behavior with the custom row list treated as the all-set. `<None>` is not offered in Default mode, and each State item can appear only once. Switching from Iterate to Default removes `<None>` rows and duplicate State item rows, keeping the first row found for each State item.

Coalescing must account for row color and row identity. It must not merge across `<None>` gaps in Iterate mode, because that would remove an intentional blank interval.

## Resolved Design Decisions

Custom preserves the current `Playback Mode` when selected. It does not force or default the effect to `Iterate`.

Custom requires at least one row while active when the selected State definition has at least one real State item. Switching to Custom automatically seeds an empty custom collection with the first real State item and that State item's configured color.

In `Default` playback mode, custom rows render like top-level `State Item` `<All>`, with the custom row list treated as the all-set.

Custom rows select one specific State item by stable ID. Duplicate State item names must be disambiguated in the row selector using useful State item context, preferably assignment names, then row ordinal, then short ID suffix if needed.

Default mode allows each State item to be added only once. Iterate mode allows the same State item to be added multiple times. When switching from Iterate to Default, duplicate rows are removed and the first row found for each State item is kept.

Changing a custom row's `State Item` resets the row color to the newly selected State item's default color.

Changing the top-level `State` definition clears the custom row collection.

New custom rows default to the first actual State item in the selected State definition. `<None>` is first in the row selector list only when `Playback Mode` is `Iterate`.

Custom row reordering is out of scope for this enhancement. Use the existing add, remove, and edit collection behavior.

The timeline visual representation should include a `Custom` hint when `Render Source` is `Custom`.

Row colors use simple `System.Drawing.Color` values, matching current State item color support. Color gradients are a possible future enhancement.

The custom row color picker must use the selected State item's assigned element set as its color context. Users should see the correct full-color or discrete-color picker for the elements that the selected State item actually controls.

## Open Questions

No open product questions remain for the ExecPlan draft. The implementation plan should still validate technical feasibility of duplicate-label generation and expandable collection editor behavior before coding.

