# VIX-2775 Mark Collection selector notification contract review

## Finding

`BaseEffect` currently raises `PropertyChanged("MarkCollectionId")` to refresh the selected effect's Mark Collection dropdown. This works for editor-effect selectors because Alternating, Dissolve, Fireworks, Shapes, State, Strobe, Text, and LipSync each expose a public `string MarkCollectionId` property. However, that common property name is not currently represented by a shared type contract, so a future effect can participate in BaseEffect mark lifecycle behavior without providing the editor property that receives the notification.

`Vixen.Marks.IMarkCollectionSelection` must remain separate. It models an effect-local persisted `Guid` selection and normalization policy. Its implementations include child objects such as Waveforms and Liquid Emitters, which do not necessarily expose the effect-editor's public string proxy property. Combining the two contracts would either force irrelevant UI members onto child models or falsely imply that every persisted selection is an editor property.

## Recommendation

Add a small public Core interface named `IMarkCollectionSelector` in `src/Vixen.Core/Marks/IMarkCollectionSelector.cs`:

    public interface IMarkCollectionSelector
    {
        string MarkCollectionId { get; set; }
    }

Its XML documentation should state that this is the editor-facing selection property: it displays a Mark Collection name but maps that displayed name to the owning effect's stable persisted `Guid`. It should also document that BaseEffect raises `PropertyChanged` for this member when the available collection names change. This interface has no dependency on WPF, a converter, or the concrete effect modules.

Declare the eight affected editor-effect classes as implementing `IMarkCollectionSelector`: Alternating, Dissolve, Fireworks, Shapes, State, Strobe, Text, and LipSync. Their existing public `string MarkCollectionId` properties satisfy the new interface unchanged; no setter, data model, converter, or selection policy rewrite is needed. LipSync retains its special converter and requires its own filtered-list regression. Do not add the interface to Wave/Liquid child selectors.

Replace every literal notification in `BaseEffect` with a private helper such as:

    private void NotifyMarkCollectionSelectorChanged()
    {
        if (this is IMarkCollectionSelector)
        {
            OnPropertyChanged(nameof(IMarkCollectionSelector.MarkCollectionId));
        }
    }

Call that helper at the same existing lifecycle positions and from the subscribed Mark Collection property handler. This preserves notification ordering and the no-dirty rename behavior while making the property name compile-time checked. Effects that do not expose the editor-facing property receive no irrelevant event.

Update the `TestEffect` in `BaseEffectMarkCollectionSelectionTests` to implement `IMarkCollectionSelector`, with a simple test-owned `string MarkCollectionId` property. Retain the existing separate `Guid` test selection field, because the test continues to verify that a renamed collection does not mutate persisted identity. The existing eight focused tests should remain the regression suite, and add one assertion or test that a BaseEffect-derived non-selector does not receive the selector property event if preserving that boundary is important.

Because this adds a public C# interface, implementation must use the repository `csharp-docs` skill and provide complete XML documentation. The affected public effect class declarations gain an interface but no new member implementation, so their existing public property documentation should be reviewed for accuracy but need not be rewritten solely for the declaration change.

## Rationale

This is a narrow Interface Segregation Principle application: the persisted-selection policy and the editor-notification target have different consumers and different types. The compiler verifies that an effect opting into the UI selector contract has the required `string MarkCollectionId` member, and `nameof(IMarkCollectionSelector.MarkCollectionId)` prevents a future rename from leaving BaseEffect with a stale string. It avoids reflection, runtime property lookup, UI changes, and a per-effect callback list.

## Scope and validation

Expected changed files are the new Core interface, `BaseEffect.cs`, the eight editor-effect class declarations, `BaseEffectMarkCollectionSelectionTests.cs`, and `LipSyncMarkCollectionNameConverterTests.cs`. No converter, property-grid, serialized data, or individual selector implementation needs logic changes.

Run the prescribed Release/x64 `Vixen_Tests` MSBuild target, then the focused `BaseEffectMarkCollectionSelectionTests` and `LipSyncMarkCollectionNameConverterTests` filters and the full `dotnet test --no-build` suite. In the editor, verify that a selected converter-backed effect updates its dropdown after a collection add or rename, while its persisted selected collection remains unchanged after rename. For LipSync, also verify that its special dropdown continues to show Phoneme collections and a selected legacy collection only.
