# State Effect Phase 3

## Overview

This document specifies the Phase 3 improvements for the State effect in `src/Vixen.Modules/Effect/State`.

The existing State effect supports `Render Source = Custom`, where the user creates an ordered `Custom State Items`
collection. In `Playback Mode = Iterate`, each row currently consumes one timing slot and renders independently. Phase 3
adds an option that lets the user keep that behavior or group consecutive custom rows with the same State item name so
they render together in one timing slot.

The user-visible goal is that a sequence author can build custom rows such as `Item 2`, `Item 2`, `Item 2`, `<None>`,
`Item 4`, `Item 4`, `Item 4`, then choose whether those seven rows cycle as seven individual slots or as three grouped
slots. Grouped rows still keep their row-specific custom colors.

## Terms

`Render Source` is the State effect editor option that chooses where active State items come from. This phase only applies
when `Render Source = Custom`.

`Playback Mode` is the State effect editor option that controls how multiple active items are scheduled. This phase only
applies when `Playback Mode = Iterate`.

`Custom State Items` is the existing ordered collection of custom State item rows. Each row has a selected State item and
a color override.

`Cycle Individually` is the new boolean editor option added by this phase. When checked, each custom row cycles in its own
timing slot. When unchecked, consecutive rows with the same resolved State item name are grouped into one timing slot.

`Grouped mode` means `Render Source = Custom`, `Playback Mode = Iterate`, and `Cycle Individually = false`.

## Requirements

Add a new boolean option named `Cycle Individually` to the State effect.

`Cycle Individually` must be checked by default. This default preserves the existing Custom Iterate behavior for new and
existing effects.

Persist `Cycle Individually` in `StateData` so saved effects reopen with the same setting. Deserialized data that does not
contain the field must behave as though `Cycle Individually` is `true`.

The option is visible only when `Render Source = Custom` and `Playback Mode = Iterate`.

The option appears in the Effect Editor immediately before `Custom State Items`. It should be the last scalar option shown
before the custom item collection.

Changing `Cycle Individually` must mark the effect dirty and invalidate/re-render the effect using the same update pattern
as the existing State effect properties.

When `Cycle Individually = true`, Custom Iterate behavior is unchanged. Each custom row consumes one timing slot for each
iteration. `<None>` rows and rows with missing/deleted State item IDs still consume timing but render no output.

When `Cycle Individually = false`, consecutive custom rows with the same grouping key collapse into one timing slot. Each
row in the group renders only the selected State item row that the user added to the custom collection, using that row's
custom color. Grouped mode must not expand a selected item to every State definition item with the same name.

Grouping uses the resolved `StateItemData.Name` for valid State item rows. Matching is exact, ordinal, and case-sensitive,
consistent with the existing State item and Mark Collection matching behavior.

`<None>` rows are grouped consistently with valid State item rows. Multiple consecutive `<None>` rows collapse into one
timing slot and render no output.

Rows with missing or deleted State item IDs consume timing consistently with other rows. Consecutive missing rows should
collapse when their grouping key matches. The grouping key should preserve the missing selection identity so unrelated
missing IDs do not accidentally merge merely because they are both missing.

Grouped mode works with `Iterations` by repeating the grouped sequence inside the same effect duration. For example, three
groups with `Iterations = 2` produce six equal timing slots.

`Playback Mode = Default` behavior is unchanged. In Custom Default mode, valid rows render for the full effect duration
with their row colors, while existing skip/deduplication behavior remains unchanged for `<None>`, missing, and duplicate
rows.

The generated visual representation should continue to identify the State effect and selected source. When the effect is
using Custom grouped mode, the visual representation text should add the word `Group` after `Custom`.

## Scheduling Details

In Custom Iterate mode, the scheduler should first build the sequence of timing entries.

When `Cycle Individually = true`, the timing entry sequence is the existing custom row list. The number of timing slots is:

    CustomStateItems.Count * Iterations

When `Cycle Individually = false`, the timing entry sequence is the consecutive groups produced from the custom row list.
The number of timing slots is:

    CustomGroups.Count * Iterations

Each timing slot duration is calculated with the existing Iterate duration behavior. All slots divide the total effect
duration equally, and any leftover ticks from integer division belong to the final slot.

Rows that consume timing but render no output still count toward slot duration. This includes `<None>` rows and missing
State item rows.

For a grouped timing slot, every renderable custom row in that group produces an interval with the group's start time and
duration, using that row's selected State item and color override. Rows that are `<None>` or missing produce no interval,
but the group still consumes its slot.

## Examples

### Consecutive Matching Names Collapse

The list has seven rows:

- Rows 1 through 3 are named `Item 2`.
- Row 4 is `<None>`.
- Rows 5 through 7 are named `Item 4`.

With `Cycle Individually = true`, there are seven timing slots.

With `Cycle Individually = false`, there are three timing slots:

- Slot 1 renders the three `Item 2` custom rows together, each with its own custom color.
- Slot 2 renders nothing because the group is `<None>`.
- Slot 3 renders the three `Item 4` custom rows together, each with its own custom color.

### Non-Consecutive Matching Names Do Not Merge

The list has five rows:

- Rows 1 and 2 are named `Item 2`.
- Row 3 is named `Item 5`.
- Row 4 is named `Item 2`.
- Row 5 is named `Item 5`.

With `Cycle Individually = false`, there are four groups and four timing slots:

- Slot 1 renders rows 1 and 2 together because they are consecutive and have the same name.
- Slot 2 renders row 3.
- Slot 3 renders row 4.
- Slot 4 renders row 5.

Rows 4 and 5 do not merge with earlier matching names because only consecutive rows are grouped.

### Iterations Repeat Groups

If the grouped sequence contains `Item 2`, `<None>`, and `Item 4`, and `Iterations = 2`, the effect produces six timing
slots:

- `Item 2`
- `<None>`
- `Item 4`
- `Item 2`
- `<None>`
- `Item 4`

The six slots divide the same total effect duration; the effect duration is not extended.

## Expected Implementation Shape

The implementation should stay localized to the State effect and its focused tests.

Expected State effect files:

- `src/Vixen.Modules/Effect/State/StateData.cs`
- `src/Vixen.Modules/Effect/State/State.cs`
- `src/Vixen.Modules/Effect/State/StateRenderPlanner.cs`

Expected test files:

- `src/Vixen.Tests/Effect/State/StateRenderPlannerTests.cs`
- Additional focused State effect tests only if needed to cover editor visibility, data cloning, or visual text behavior.

The render planner should keep Custom Default behavior unchanged and route only Custom Iterate scheduling through the
`Cycle Individually` option.

The data model clone path must copy `Cycle Individually`.

If adding or modifying any public or protected C# API, update XML documentation in the same change.

## Validation

Focused tests should cover at least these behaviors:

- New `StateData` defaults `Cycle Individually` to `true`.
- Cloning `StateData` preserves `Cycle Individually`.
- Custom Iterate with `Cycle Individually = true` produces the same intervals as the current behavior.
- Custom Iterate with `Cycle Individually = false` collapses consecutive valid rows with the same exact name into one slot.
- Grouped rows render only the custom rows the user added and keep each row's color override.
- Non-consecutive rows with the same name do not merge.
- Consecutive `<None>` rows collapse into one timing slot and render no output.
- Missing/deleted rows consume timing in grouped mode.
- `Iterations` repeats grouped timing entries inside the same total duration.
- Custom Default behavior is unchanged.
- The Effect Editor shows `Cycle Individually` only when `Render Source = Custom` and `Playback Mode = Iterate`.
- The editor orders `Cycle Individually` immediately before `Custom State Items`.
- The visual representation includes `Group` after `Custom` when grouped mode is active.

Run focused validation from the repository root:

    dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Effect.State"

The expected result is a passing Effect.State test run with the new Phase 3 tests included.

Run the State effect project build:

    dotnet build src\Vixen.Modules\Effect\State\State.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore

The expected result is a successful build with zero State effect errors.

## Risks and Concerns

The main behavioral risk is accidentally expanding grouped custom rows by State item name. Grouped Custom mode must render
only the rows the user added to `Custom State Items`.

Another risk is losing current timing compatibility. `Cycle Individually` defaults to `true`, and that path must preserve
the existing Custom Iterate interval behavior exactly.

Missing/deleted rows need careful grouping semantics. They must consume timing, but unrelated missing item IDs should not
collapse together simply because they no longer resolve to a name.

The editor visibility and ordering logic should be verified because the option is conditional and must appear directly
before the custom item collection.
