# VIX-4005 Wipe clone design review

Reviewed 2026-10-02 using the project dotnet-design-pattern-review skill. Implementation details and acceptance live in [the ExecPlan](../plans/effects/vix-4005-wipe-clone-integrity.md).

## Finding

WipeData.CreateInstanceForClone uses MemberwiseClone, so TargetNodeSelection is already copied. The confirmed source-level reset occurs when a fresh WipeModule receives data before its target nodes: ModuleData invokes UpdateAttributes, and UpdateTargetingAttributes replaces Individual with Group for an empty target list. Both TimedSequenceEditorForm.CloneElements and the clipboard paste path assign data before CreateEffectNode attaches targets. Raw copying also shares both Curve objects and ColorGradient, contrary to the base data model's deep-clone contract.

## Pattern and ownership assessment

Retain the existing Prototype pattern, which copies an existing instance, and Template Method pattern, in which EffectTypeModuleData.Clone delegates effect-specific work to CreateInstanceForClone. Begin with a memberwise copy to preserve all scalar fields and inherited metadata, then detach Curve, MovementCurve, and ColorGradient with their established copy constructors. Preserve their library metadata and null state. ModuleDataSet remains the shared owner-service reference under existing conventions.

Delay target normalization when TargetNodes is empty and hide the two targeting controls. Apply existing shallow-target, group-mode, multiple-target, and useful-depth rules after populated targets are attached. This fixes Wipe's initialization behavior without changing shared editor ordering or other effects. No new factory, command handler, repository, dependency-injection service, or interface is needed for this synchronous in-memory copy.

## Testability and documentation

Add a complete serialized-member inventory and an explicit reference-ownership inventory in tests, so future fields require deliberate fixture coverage and mutable-reference handling. Test nested point mutations in both directions, both target modes, null raw data, and data-before-target assignment for cloned and serialized clipboard-style data. Keep reflection confined to tests and existing test seams. Update XML documentation only on changed APIs, and document the empty-target exception in the Wipe specification.

## Practical limits

Source inspection establishes the destructive assignment order, but planning did not execute runtime reproduction or tests. The ExecPlan requires focused failures before the fix, passing tests afterward, and manual editor clone/paste validation before claiming user-visible completion. The production scope remains WipeData and WipeModule.
