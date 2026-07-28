# Technical Specification: VIX-3951 State Effect Index Offset

## 1. Refined Requirements

VIX-3951 adds an effect-wide initial phase shift to the State effect's existing `PlaybackMode.Iterate` scheduling. The editor calls the setting **Cycle Offset** and the persisted C# property is `CycleOffset`.

### Functional behavior

- Cycle Offset selects which already-calculated timing slot renders first. It is an index offset, not a stride, step size, timing multiplier, or a change to iteration count.
- For a base sequence containing `slotCount` chronological slots and a zero-based output slot index `i`, the source slot is:

  ```text
  sourceIndex = (i + normalizedCycleOffset) % slotCount
  normalizedCycleOffset = CycleOffset % slotCount
  ```

  Thus `[1, 2, 3, 4]` with an offset of `1` schedules `[2, 3, 4, 1]`; it does not schedule every other slot.
- Offset is meaningful only in `PlaybackMode.Iterate` (the editor's “Cycle” mode). In Default mode the existing simultaneous-render behavior is unchanged and the editor must hide the property.
- Offset applies to the completed base slot sequence, before that sequence is repeated for `Iterations`. For example, base `[A, B, C]`, `CycleOffset = 1`, and `Iterations = 2` produces `[B, C, A, B, C, A]`.
- The offset must not participate in the sequence editor's visual-representation text. Existing State/State Item, Mark Collection, and Custom visual text remains unchanged.
- `CycleOffset = 0` is the compatibility path: its generated intervals must remain interval-for-interval identical to current behavior, including order, start, duration, items, and color overrides.
- A base list with zero or one slot is a no-op. Rendering must not divide by zero, throw, or manufacture intervals.
- An offset equal to a list's slot count wraps to zero; larger values wrap with modulo. A value such as `100` must be evaluated against the current calculated slot count, not a fixed maximum or persisted normalized value.

### Persistence and property contract

- Add `[DataMember] public int CycleOffset { get; set; }` to `StateData`, initialized to `0`.
- `StateData.CycleOffset` stores the raw configured value; it is not normalized to a current slot count during persistence, cloning, loading, or editor assignment. Slot counts vary by render source and can change when the definition, mark text, or custom collection changes.
- Add `CycleOffset = CycleOffset` to `StateData.CreateInstanceForClone()`.
- Effects serialized before this field existed deserialize to the CLR/default value `0`; no migration, schema version, or deserialization callback change is required.
- Negative offsets are unsupported. The editor range must prevent them. The render helper should treat non-positive input as zero defensively so malformed legacy/manual data cannot create a negative array/list index; it must not rewrite the raw persisted value as part of rendering.
- `StateData` must retain the raw value even when it exceeds the current number of slots. Do not use an `Iterations`-style clamp for this setting and do not persist a modulo-normalized value.

### Effect-editor contract

- Add a documented public `State.CycleOffset` property that proxies `_data.CycleOffset`.
- Use the normal Effect Editor metadata pattern: `[Value]`, `ProviderCategory("Config", 2)`, resource-backed `ProviderDisplayName` and `ProviderDescription`, `SliderEditor`, and `[NumberRange(0, 100, 1)]`.
- Add localized display metadata with display text `Cycle Offset` and a concise description explaining that it starts Cycle playback at a later timing slot and wraps at the end.
- Place the property with the other Cycle settings, after `Iterations` and before the Custom-specific `CycleIndividually` / `CustomStateItems` controls. The exact `PropertyOrder` must preserve the existing ordering of unrelated controls.
- `SetRenderSourceBrowsables` must make `CycleOffset` browsable exactly when `PlaybackMode == PlaybackMode.Iterate`, independent of State Item, Mark Collection, or Custom render source. It is hidden in Default mode.
- Its setter must follow the existing State scalar-property pattern: return without a change when the stored value is equal; otherwise assign the raw value, set `IsDirty = true`, and call `OnPropertyChanged()`. It must not change `ForceGenerateVisualRepresentation`, selections, custom rows, or playback mode.
- Existing dirty/rerender behavior applies: a change queues the normal State rerender, and when multiple editor updates occur, the most recent dirty render wins. No new locks, collection snapshots, background work, or synchronization is part of this feature.

### Slot-definition rules

The implementation must finish the existing slot calculation first, then select slots by normalized index while assigning the same existing durations. The offset must not alter grouping, parsing, clipping, assignment expansion, color resolution, overlap behavior, or coalescing.

- **State Item source**
  - Selected State Item remains a full-duration exact-name group. It has one timing slot, so the offset is a no-op.
  - `<All>` in Iterate mode retains unique exact-name slots in State-definition order. All rows whose names match a slot name remain together atomically.
  - Default mode remains simultaneous full-duration rendering.
- **Mark Collection source**
  - Retain each mark's current clipping and parsing behavior. Do not introduce cross-mark anchoring or rotate marks as a whole.
  - For each Iterate mark, first parse the comma-delimited segments and form its existing per-mark base slot list; then use the offset when selecting segment slots for that mark's repetitions.
  - Recognized segments render their existing exact-name item group. Empty segments and unknown names continue to consume an equal timing share but produce no intervals, and consequently participate in the offset.
  - Mark gaps remain blank; overlapping marks still independently render and combine later. Default mark behavior remains unchanged.
- **Custom source**
  - In Iterate mode with `CycleIndividually = true`, the existing ordered custom row list is the base slot list. Duplicate rows, `<None>`, missing/deleted IDs, and per-row colors keep their current behavior. Blank or missing rows consume timing and participate in offset selection.
  - In Iterate mode with `CycleIndividually = false`, first construct the current consecutive resolved-name groups. Each completed group is one atomic slot; Cycle Offset indexes groups, never individual rows inside a group. A selected row in a group still produces only its own item/color interval, preserving group contents and internal row order.
  - Custom Default mode is unchanged, including its existing valid-row deduplication and full-duration behavior.

## 2. Technical Architecture & Impact

### Affected files and members

| File | Required change |
| --- | --- |
| `src/Vixen.Modules/Effect/State/StateData.cs` | Add persisted `CycleOffset` defaulting to zero, XML documentation, and clone propagation. Do not add a normalization setter. |
| `src/Vixen.Modules/Effect/State/State.cs` | Add the public documented editor proxy, range/resource metadata, dirty/notification setter, conditional browsability, and pass `CycleOffset` to each relevant planner call. |
| `src/Vixen.Modules/Effect/State/StateRenderPlanner.cs` | Extend Iterate planner entry points/helpers to receive the raw offset and select an already-built slot by normalized indexed lookup. Keep Default paths untouched. |
| `src/Vixen.Modules/EffectEditor/EffectDescriptorAttributes/EffectDisplayNameDescriptors.resx` | Add the State Cycle Offset display-name resource key. |
| `src/Vixen.Modules/EffectEditor/EffectDescriptorAttributes/EffectDescriptionDescriptors.resx` | Add the matching concise description resource key. |
| `src/Vixen.Tests/Effect/State/StateDataTests.cs` | Cover persisted defaults, cloning, public editor metadata, visibility, and dirty notification behavior. |
| `src/Vixen.Tests/Effect/State/StateRenderPlannerTests.cs` | Cover source-specific rotation, wrap-around, blank slots, grouped custom atomicity, repetitions, and timing parity. |

No change is required to `StateRenderSource`, `PlaybackMode`, State-property data, assignment expansion, interval coalescing, timeline representation, or any location/string/grid render source outside this effect.

### C# API shape

`StateData` should expose a new documented public auto-property:

```csharp
[DataMember]
public int CycleOffset { get; set; }
```

Its XML documentation must state that it stores the raw number of Cycle timing slots to skip before scheduling starts and that the default is `0`. The public `State.CycleOffset` property must likewise document that it controls the initial offset in Iterate playback and that `0` preserves the first slot. As required for modified public APIs, both properties need `summary` and `value` XML documentation consistent with the repository's documentation conventions.

Use constants for the editor bounds where this improves consistency (for example `MinCycleOffset = 0` and `MaxCycleOffset = 100` on `StateData`), but do not expose or use a `NormalizeCycleOffset` persistence method analogous to `NormalizeIterations`. The value is normalized only with the actual base slot count in the planner.

### Planner implementation model

The recommended implementation is a small private helper whose only responsibility is selecting a source index:

```csharp
private static int GetOffsetIndex(int outputIndex, int slotCount, int cycleOffset)
{
    if (slotCount <= 1 || cycleOffset <= 0)
    {
        return outputIndex % slotCount;
    }

    var normalizedOffset = cycleOffset % slotCount;
    return (outputIndex + normalizedOffset) % slotCount;
}
```

The production version must guard the `slotCount == 0` caller path before invoking modulo. It may normalize once per scheduling operation rather than once per loop. The crucial behavior is indexed lookup, not the helper's exact signature:

```text
baseSlotIndex = outputIndex % baseSlotCount
sourceSlotIndex = (baseSlotIndex + (rawCycleOffset % baseSlotCount)) % baseSlotCount
slot = baseSlots[sourceSlotIndex]
```

This is intentionally not `Skip(offset).Concat(Take(offset)).ToList()` or any other rotation/copy operation. It avoids an extra sequence-sized allocation and leaves the original groups, parser output, and custom row collection intact. The expected collection sizes are small, but indexed lookup is simpler, stable, and avoids accidental mutation.

Duration allocation stays based on the unrotated number of output slots:

```text
intervalCount = baseSlotCount * NormalizeIterations(iterations)
duration(slot i) = existing equal tick division
last duration = effectDuration - accumulated prior durations
```

Only the source selected for each output index changes. The output index continues to drive `GetIntervalDuration`, so the final-slot remainder-tick behavior is unchanged.

### Render-source integration

1. **State Item**: extend `CreateStateItemIntervals` with `cycleOffset`. For selected-item calls, preserve the existing single full-duration result. For `<All>` Iterate, use the unique-name list as the base slots; index the name list with the helper before expanding that name to all exact-name `StateItemData` rows.
2. **Mark Collection**: extend `CreateMarkCollectionIntervals` and the Iterate mark helper with `cycleOffset`. The input is still processed mark-by-mark. For an Iterate mark, `names` (including `string.Empty` for empty segments) is the base list. Compute segment count and durations exactly as today, then select `names[sourceIndex]`. Do not filter unknown/empty values before slot counting.
3. **Custom individual**: extend both custom planner overloads compatibly, keeping the existing convenience overload's documented/default behavior clear. In the individual Iterate loop, choose `customStateItems[sourceIndex]` only after calculating `intervalCount` from the complete row list.
4. **Custom grouped**: construct `groups` first using the existing exact-name / `<None>` / missing-ID grouping keys. Compute count and durations from `groups.Count`, then choose `groups[sourceIndex]`; enumerate that selected group in its existing order and retain its row colors.

Planner overload changes should be applied consistently at the single `State.CreateRenderIntervals` call site. Existing test calls must be updated deliberately; optional overload forwarding is acceptable only if it keeps old internal callers semantically zero-offset and does not obscure the new source contract.

## 3. Acceptance Criteria

1. **Persistence default** — Given State data serialized before VIX-3951 or a newly constructed `StateData`, when it is loaded or inspected, then `CycleOffset` is `0` and existing output is unchanged.
2. **Persistence and clone** — Given `CycleOffset` is set to `37`, when the State effect data is cloned or saved/loaded, then the clone/reloaded data contains raw value `37`, not a modulo-normalized value.
3. **Editor range and visibility** — Given the Effect Editor, when Playback Mode is Iterate, then Cycle Offset is visible with range `0` through `100`; when Playback Mode is Default, then it is not browsable for every render source.
4. **Editor invalidation** — Given an existing State effect, when Cycle Offset changes to a different editor value, then it updates stored data, raises its property notification, and sets `IsDirty`; when assigned the same value, then it does not create a redundant dirty/property update.
5. **Forward rotation** — Given an Iterate base sequence `[A, B, C, D]` and offset `1`, when rendering, then the chronological slots are `[B, C, D, A]` with their existing four equal-duration boundaries.
6. **Wrap-around** — Given four base slots, when offset is `4`, then the result is `[A, B, C, D]`; when offset is `5`, then the result is `[B, C, D, A]`; when offset is `100`, then the result is equivalent to offset `0`.
7. **Iterations** — Given base `[A, B, C]`, offset `1`, and iterations `2`, when rendering, then slots are `[B, C, A, B, C, A]`, not `[B, C, A, A, B, C]` or a single rotation of the six-slot output.
8. **Empty/singleton safety** — Given zero slots, when any Cycle Offset is supplied, then no exception and no intervals result. Given one slot, when any Cycle Offset is supplied, then the existing single-slot schedule results unchanged.
9. **State Item atomic names** — Given `<All>` Iterate with State items `Open`, `Open`, `Closed` and offset `1`, when rendering, then the `Closed` group occurs first and both `Open` rows occur together in their rotated slot. Given a specifically selected item, then its full-duration exact-name group is unchanged.
10. **Mark slots** — Given an Iterate mark with recognized, unknown, and empty parsed segments, when an offset moves an unknown or empty segment to the first position, then the first timing share is blank and all following shares retain their original duration behavior. Clipping, gaps, overlapping marks, and Default mark behavior remain unchanged.
11. **Custom individual slots** — Given Cycle Individually is true and Custom rows include duplicate selections, `<None>`, and missing IDs, when offset is applied, then every original row remains one timing slot, blank/missing rows consume timing, and valid rows retain their own color overrides.
12. **Custom grouped slots** — Given Cycle Individually is false and consecutive rows form groups, when offset is applied, then groups move as atomic slots; their rows remain in original internal order with original colors; non-consecutive matching names do not merge; and `<None>`/missing groups consume a blank slot.
13. **Timing compatibility** — Given a duration that is not evenly divisible by slot count, when Cycle Offset is nonzero, then only slot contents rotate; all non-final slots retain existing tick division and the final output slot receives the existing remainder. Given offset zero, interval sequences exactly match pre-change results.
14. **No unrelated output changes** — Given any Default-mode effect, visual-representation request, state assignment expansion, color fallback, or segment coalescing scenario, when Cycle Offset exists in data, then its current behavior remains unchanged.

## 4. Test Plan

### Unit Tests

- **State data contract**
  - Assert new `StateData` defaults `CycleOffset` to `0`.
  - Assert clone preserves representative raw values `0`, `1`, `slotCount`, `37`, and `100`.
  - Deserialize a payload omitting CycleOffset and assert zero; deserialize a large value and assert it is retained without clamping/modulo.
  - If defensive negative behavior is supported in the planner, assert a negative raw value does not throw and behaves as zero without mutating `StateData.CycleOffset`.
- **Editor metadata and mutation**
  - Use `TypeDescriptor` to assert display name, description, `SliderEditor`, and numeric range `0..100`.
  - Assert the property is browsable for State Item, Mark Collection, and Custom only with Iterate playback, and hidden with Default playback.
  - Assert its order is after Iterations and before Custom-only cycle/collection controls where those controls are visible.
  - Assert a changed value marks the effect dirty and raises `PropertyChanged(nameof(CycleOffset))`; assert same-value assignment is a no-op.
- **Index math / State Item planner**
  - Test zero-offset regression parity against current expected interval identities/times.
  - Test offsets `1`, slot count, slot count plus one, and `100` against three- and four-slot lists.
  - Test empty and singleton lists.
  - Test multiple iterations repeat the rotated base sequence.
  - Test duplicate exact-name State items remain in one atomic rotated slot; test a selected item ignores the offset.
- **Mark Collection planner**
  - Build an Iterate mark with names such as `Open,Unknown,,Closed`; verify forward rotation selects each original parsed segment, including unknown and empty blank slots.
  - Cover wrap-around, iteration repetition, an empty segment list, and a one-segment list.
  - Assert clipped mark starts/durations, a gap, and overlapping marks retain their existing behavior with an offset. Retain a Default-mode regression test proving no rotation is applied.
- **Custom planner**
  - With `CycleIndividually = true`, assert duplicate rows, `<None>`, and a missing ID each retain one slot and rotate by row; assert colors follow their selected rows.
  - With `CycleIndividually = false`, construct consecutive equal-name rows with distinct colors, `<None>` rows, and missing IDs. Assert only groups rotate, all group rows share the selected group's time interval, and colors/internal order are preserved.
  - Assert non-consecutive equal names remain distinct groups and distinct missing IDs do not merge.
  - For both modes, assert offset-before-iterations ordering and final-remainder tick allocation using a duration not divisible by total interval count.

### Integration/Manual Tests

1. Create a State definition with four uniquely named, visibly distinct State items. Add a State Item / `<All>` / Iterate effect with one iteration. Change Cycle Offset from `0` through `4` and verify the first visible state advances by one each time and returns to the original at `4`; confirm a specific State Item selection remains static.
2. Set Iterations to `2`, Cycle Offset to `1`, and verify that the rotated order repeats identically in the second cycle rather than rotating only once across the entire effect. Repeat with an effect duration that does not divide evenly by the number of slots and inspect the final boundary/frame for remainder preservation.
3. Use a Mark Collection containing recognized names, an unknown label, an empty comma-delimited segment, marks with gaps, and overlapping marks. In Iterate mode, apply offsets that move each blank segment to the beginning. Verify blank time moves but mark clipping, gaps, and concurrent overlap output do not otherwise change.
4. Use Custom / Iterate / Cycle Individually with rows containing repeated items, `<None>`, a deleted/missing State item, and distinct row colors. Verify offset moves rows (including blank rows) and colors remain attached to their rows.
5. Turn Cycle Individually off and use consecutive same-name rows with distinct colors separated by `<None>` and another name. Verify each group shifts as one visual time slice, the two colors within its group appear together, and a later non-consecutive matching row stays a separate group.
6. Switch each source to Default playback and verify Cycle Offset disappears and all prior Default behavior is unchanged. Confirm changing Cycle Offset does not alter the sequence-editor visual text.

Run focused automated validation from the repository root:

```powershell
dotnet test src\Vixen.Tests\Vixen.Tests.csproj --filter "FullyQualifiedName~Effect.State" --no-restore
dotnet build src\Vixen.Modules\Effect\State\State.csproj -p:Configuration=Debug -p:Platform=x64 --no-restore
```

