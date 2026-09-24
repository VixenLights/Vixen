---
name: gortex-editor-timedsequenceeditor-46-dirs
description: "Work in the Editor/TimedSequenceEditor +46 dirs area — 2018 symbols across 132 files (80% cohesion)"
---

# Editor/TimedSequenceEditor +46 dirs

2018 symbols | 132 files | 80% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Application/ConfigPreviews.cs`
- `src/Vixen.Application/DataProfileForm.cs`
- `src/Vixen.Application/DataZipForm.cs`
- `src/Vixen.Common/AudioPlayer/FileReader/AudioFileReader.cs`
- `src/Vixen.Common/BaseSequence/Sequence.cs`
- `src/Vixen.Common/BaseSequence/SequenceData.cs`
- `src/Vixen.Common/Controls/MenuStripEx.cs`
- `src/Vixen.Common/Controls/MessageBoxForm.Designer.cs`
- `src/Vixen.Common/Controls/MessageBoxForm.cs`
- `src/Vixen.Common/Controls/TimeLineControl/ElementMoveType.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Grid.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarkTimeInfo.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksBulkChangeInfo.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksMoveResizeInfo.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksMovedEventArgs.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksSelectionManager.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/TimeLineGlobalEventManager.cs`
- `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Row.cs`
- `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs`
- `src/Vixen.Common/Controls/ToolStripEx.cs`
- `src/Vixen.Common/Utilities/DragDropUtils.cs`
- `src/Vixen.Core/Execution/Context/LiveContext.cs`
- `src/Vixen.Core/Execution/Context/PreCachingSequenceContext.cs`
- `src/Vixen.Core/Execution/Context/PreviewContext.cs`
- `src/Vixen.Core/Execution/Context/ProgramContext.cs`
- `src/Vixen.Core/Execution/Context/SequenceContext.cs`
- `src/Vixen.Core/Execution/ContextCurrentEffectsFull.cs`
- `src/Vixen.Core/Execution/DataSource/SequenceDataPump.cs`
- `src/Vixen.Core/Execution/EffectNodeQueue.cs`
- `src/Vixen.Core/Execution/IContextCurrentEffects.cs`
- `src/Vixen.Core/Execution/IProgramExecutor.cs`
- `src/Vixen.Core/Execution/MediaTimingProvider.cs`
- `src/Vixen.Core/Execution/ProgramExecutor.cs`
- `src/Vixen.Core/Intent/StaticIntentState.cs`
- `src/Vixen.Core/Marks/IMarkCollection.cs`
- `src/Vixen.Core/Module/App/AppModuleInstanceBase.cs`
- `src/Vixen.Core/Module/App/AppModuleRepository.cs`
- `src/Vixen.Core/Module/App/IApp.cs`
- `src/Vixen.Core/Module/Editor/IEditorUserInterface.cs`
- `src/Vixen.Core/Module/Effect/EffectModuleInstanceBase.cs`
- `src/Vixen.Core/Module/Effect/IEffect.cs`
- `src/Vixen.Core/Module/Effect/IEffectModuleInstance.cs`
- `src/Vixen.Core/Module/Input/InputEffectMap.cs`
- `src/Vixen.Core/Module/Media/IMediaModuleInstance.cs`
- `src/Vixen.Core/Module/MixingFilter/ILayerMixingFilterInstance.cs`
- `src/Vixen.Core/Module/MixingFilter/LayerMixingFilterModuleManagement.cs`
- `src/Vixen.Core/Module/ModuleImplementationMethod.cs`
- `src/Vixen.Core/Module/SequenceType/SequenceTypeDataModelBase.cs`
- `src/Vixen.Core/Module/SequenceType/Surrogate/EffectNodeSurrogate.cs`
- `src/Vixen.Core/Module/SequenceType/Surrogate/LayerMixingFilterSurrogate.cs`
- `src/Vixen.Core/Module/SequenceType/Surrogate/MediaSurrogate.cs`
- `src/Vixen.Core/Module/SequenceType/Surrogate/SelectedTimingProviderSurrogate.cs`
- `src/Vixen.Core/Services/LayerMixingFilterService.cs`
- `src/Vixen.Core/Sys/DataStream.cs`
- `src/Vixen.Core/Sys/EffectNode.cs`
- `src/Vixen.Core/Sys/ElementStateSourceCollection.cs`
- `src/Vixen.Core/Sys/IHasLayers.cs`
- `src/Vixen.Core/Sys/IHasMedia.cs`
- `src/Vixen.Core/Sys/ISequence.cs`
- `src/Vixen.Core/Sys/ISequenceTypeDataModel.cs`
- `src/Vixen.Core/Sys/InsertDataStack.cs`
- `src/Vixen.Core/Sys/LayerMixing/ILayer.cs`
- `src/Vixen.Core/Sys/LayerMixing/Layer.cs`
- `src/Vixen.Core/Sys/LayerMixing/SequenceLayers.cs`
- `src/Vixen.Core/Sys/MediaCollection.cs`
- `src/Vixen.Modules/Analysis/BeatsAndBars/PreviewWaveform.cs`
- `src/Vixen.Modules/App/ColorGradients/ColorGradientLibrary.cs`
- `src/Vixen.Modules/App/Curves/CurveEditor.cs`
- `src/Vixen.Modules/App/Curves/CurveLibrary.cs`
- `src/Vixen.Modules/App/Curves/ZedGraph/ZedGraphControl.ContextMenu.cs`
- `src/Vixen.Modules/App/Curves/ZedGraph/ZedGraphControl.Printing.cs`
- `src/Vixen.Modules/App/ExportWizard/BulkExportSummaryStage.cs`
- `src/Vixen.Modules/App/FPPClient/Module.cs`
- `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs`
- `src/Vixen.Modules/App/LivePreview/ILiveContext.cs`
- `src/Vixen.Modules/App/Shows/Actions/SequenceAction.cs`
- `src/Vixen.Modules/App/SuperScheduler/SetupScheduleForm.cs`
- `src/Vixen.Modules/App/SuperScheduler/SuperSchedulerModule.cs`
- `src/Vixen.Modules/App/WebServer/Service/SequenceHelper.cs`
- `src/Vixen.Modules/Controller/E131/SetupForm.cs`
- `src/Vixen.Modules/Editor/LayerEditor/Converters/DefaultLayerVisibilityConverter.cs`
- `src/Vixen.Modules/Editor/LayerEditor/Converters/HasSetupVisibilityConverter.cs`
- `src/Vixen.Modules/Editor/LayerEditor/Services/ILayerEditorLayerService.cs`
- `src/Vixen.Modules/Editor/LayerEditor/Services/LayerEditorLayerService.cs`
- `src/Vixen.Modules/Editor/LayerEditor/Services/LayerMixingFilterResolver.cs`
- `src/Vixen.Modules/Editor/LayerEditor/ViewModels/LayerEditorViewModel.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/AutomaticMusicDetection.Designer.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectModelCandidate.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/ExportDialog.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FindEffectForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FormEffectEditor.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_AddMultipleEffects.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_ColorLibrary.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_CurveLibrary.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Effects.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_GradientLibrary.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Grid.Designer.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Grid.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/InvalidAudioPathDialog.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/LayerEditor.Designer.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/LayerEditor.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/PapagayoDoc.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/PastingMode.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/PropertyDiscovery.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_ContextMenu.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Hotkeys.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceElement.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsAddedRemovedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsAddedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsCutUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsLayerChangedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsPastedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsRemovedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/ElementsTimeChangedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksAddedRemovedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksAddedUndoAction .cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksBulkChangeUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksRemovedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksTimeChangedUndoAction.cs`
- `src/Vixen.Modules/Media/Audio/Audio.cs`
- `src/Vixen.Modules/Media/Audio/SampleProviders/MonoSampleProvider.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomCreateForm.cs`
- `src/Vixen.Modules/Sequence/Timed/MarkCollection.cs`
- `src/Vixen.Modules/Sequence/Timed/RowSetting.cs`
- `src/Vixen.Modules/Sequence/Timed/TimedSequenceMigrator.cs`
- `src/Vixen.Tests/Sequencer/GridAlignmentNullReferenceTests.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Application/ConfigPreviews.cs` | e, ConfigPreviews_FormClosing, sender |
| `src/Vixen.Application/DataProfileForm.cs` | e, buttonOK_Click, sender |
| `src/Vixen.Application/DataZipForm.cs` | sender, buttonStartCancel_Click, e |
| `src/Vixen.Common/AudioPlayer/FileReader/AudioFileReader.cs` | disposing, Dispose |
| `src/Vixen.Common/BaseSequence/Sequence.cs` | GetSequenceLayerManager, effectNode, RemoveData, module, GetAllLayers, ... |
| `src/Vixen.Common/BaseSequence/SequenceData.cs` | SequenceData |
| `src/Vixen.Common/Controls/MenuStripEx.cs` | clickThrough, MenuStripEx |
| `src/Vixen.Common/Controls/MessageBoxForm.Designer.cs` | buttonOk, flowLayoutPanel1, buttonNo, tableLayoutPanel1, components, ... |
| `src/Vixen.Common/Controls/MessageBoxForm.cs` | sender, sender, sender, e, messageIcon_Paint, ... |
| `src/Vixen.Common/Controls/TimeLineControl/ElementMoveType.cs` | AlignStartToEnd, Move, Distribute, AlignDurations, AlignBoth, ... |
| `src/Vixen.Common/Controls/TimeLineControl/Grid.cs` | p, p, referenceElement, MoveResizeElements, elements, ... |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarkTimeInfo.cs` | mark, Duration, MarkCollection, SwapPlaces, lhs, ... |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksBulkChangeInfo.cs` | markKey, markValue, Add, MarksBulkChangeInfo, MarksBulkChangeInfo.<init>, ... |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksMoveResizeInfo.cs` | OriginalMarks, MarksMoveResizeInfo, modifyingMarks, MarksMoveResizeInfo.<init> |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksMovedEventArgs.cs` | MarksMovedEventArgs, type, MarksMovedEventArgs.<init>, MoveResizeInfo, MoveType, ... |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksSelectionManager.cs` | Select, mark |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/TimeLineGlobalEventManager.cs` | OnMarkMoved, e |
| `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs` | OnMouseUp, EndAllDrag, MouseUp_HResizing, e, CreatePhonemeMenuItem, ... |
| `src/Vixen.Common/Controls/TimeLineControl/Row.cs` | TreePathName, TreeId, input, CalculateMd5Hash |
| `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` | ResetRowHeight, ClearAllRows, marks, GridScrollHorizontalHandler, e, ... |
| `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs` | WaveformStyle, Full, WaveformStyle, Half |
| `src/Vixen.Common/Controls/ToolStripEx.cs` | ToolStripEx, clickThrough |
| `src/Vixen.Common/Utilities/DragDropUtils.cs` | dataObject, T, JsonOptions, value, TryGetDragDropData, ... |
| `src/Vixen.Core/Execution/Context/LiveContext.cs` | data, Execute, TerminateNode, targetNode, GetLayerForNode, ... |
| `src/Vixen.Core/Execution/Context/PreCachingSequenceContext.cs` | node, GetLayerForNode |
| `src/Vixen.Core/Execution/Context/PreviewContext.cs` | GetLayerForNode, node |
| `src/Vixen.Core/Execution/Context/ProgramContext.cs` | node, GetLayerForNode |
| `src/Vixen.Core/Execution/Context/SequenceContext.cs` | GetLayerForNode, node |
| `src/Vixen.Core/Execution/ContextCurrentEffectsFull.cs` | nodes, RemoveEffects |
| `src/Vixen.Core/Execution/DataSource/SequenceDataPump.cs` | GetDataAt, time |
| `src/Vixen.Core/Execution/EffectNodeQueue.cs` | time, Add, size, _queue, item, ... |
| `src/Vixen.Core/Execution/IContextCurrentEffects.cs` | RemoveEffects, nodes |
| `src/Vixen.Core/Execution/IProgramExecutor.cs` | sequence, Queue |
| `src/Vixen.Core/Execution/MediaTimingProvider.cs` | GetAvailableTimingSources, TimingProviderTypeName, sequence, MediaTimingProvider, sequence, ... |
| `src/Vixen.Core/Execution/ProgramExecutor.cs` | SequenceLayers |
| `src/Vixen.Core/Intent/StaticIntentState.cs` | value, StaticIntentState.<init> |
| `src/Vixen.Core/Marks/IMarkCollection.cs` | RemoveMark, mark |
| `src/Vixen.Core/Module/App/AppModuleInstanceBase.cs` | Loading |
| `src/Vixen.Core/Module/App/AppModuleRepository.cs` | Add, id |
| `src/Vixen.Core/Module/App/IApp.cs` | Loading |
| `src/Vixen.Core/Module/Editor/IEditorUserInterface.cs` | Save, filePath |
| `src/Vixen.Core/Module/Effect/EffectModuleInstanceBase.cs` | MarkDirty, Media |
| `src/Vixen.Core/Module/Effect/IEffect.cs` | Media |
| `src/Vixen.Core/Module/Effect/IEffectModuleInstance.cs` | IEffectModuleInstance, ForceGenerateVisualRepresentation, MarkDirty |
| `src/Vixen.Core/Module/Input/InputEffectMap.cs` | GenerateEffect, input, effectTimeSpan |
| `src/Vixen.Core/Module/Media/IMediaModuleInstance.cs` | IMediaModuleInstance |
| `src/Vixen.Core/Module/MixingFilter/ILayerMixingFilterInstance.cs` | ILayerMixingFilterInstance |
| `src/Vixen.Core/Module/MixingFilter/LayerMixingFilterModuleManagement.cs` | LayerMixingFilterModuleManagement |
| `src/Vixen.Core/Module/ModuleImplementationMethod.cs` | Invoke, parameters |
| `src/Vixen.Core/Module/SequenceType/SequenceTypeDataModelBase.cs` | context, Media, SurrogateRead |
| `src/Vixen.Core/Module/SequenceType/Surrogate/EffectNodeSurrogate.cs` | CreateEffectNode, EffectNodeSurrogate.<init>, EffectNodeSurrogate, effectNode, elementNodes |
| `src/Vixen.Core/Module/SequenceType/Surrogate/LayerMixingFilterSurrogate.cs` | CreateLayerMixingFilter |
| `src/Vixen.Core/Module/SequenceType/Surrogate/MediaSurrogate.cs` | CreateMedia |
| `src/Vixen.Core/Module/SequenceType/Surrogate/SelectedTimingProviderSurrogate.cs` | CreateSelectedTimingProvider |
| `src/Vixen.Core/Services/LayerMixingFilterService.cs` | LayerMixingFilterService.<init>, _instance, id, GetInstance, LayerMixingFilterService, ... |
| `src/Vixen.Core/Sys/DataStream.cs` | data, RemoveRangeData |
| `src/Vixen.Core/Sys/EffectNode.cs` | EffectNode.<init>, StartTime, TimeSpan, other, EffectNode, ... |
| `src/Vixen.Core/Sys/ElementStateSourceCollection.cs` | value, key, SetValue |
| `src/Vixen.Core/Sys/IHasLayers.cs` | IHasLayers, GetSequenceLayerManager, GetAllLayers |
| `src/Vixen.Core/Sys/IHasMedia.cs` | AddMedia, module, ClearMedia, GetAllMedia, module, ... |
| `src/Vixen.Core/Sys/ISequence.cs` | InsertData, effectNodes |
| `src/Vixen.Core/Sys/ISequenceTypeDataModel.cs` | Media |
| `src/Vixen.Core/Sys/InsertDataStack.cs` | InsertData, effectNode, InsertDataListenerStack, _listeners, effectNodes, ... |
| `src/Vixen.Core/Sys/LayerMixing/ILayer.cs` | Id, LayerMixingFilter, ILayer, Type, LayerName, ... |
| `src/Vixen.Core/Sys/LayerMixing/Layer.cs` | FilterTypeId |
| `src/Vixen.Core/Sys/LayerMixing/SequenceLayers.cs` | indexTo, GetDefaultLayer, EnsureUniqueName, ctx, AssignEffectNodeToLayer, ... |
| `src/Vixen.Core/Sys/MediaCollection.cs` | MediaCollection, MediaCollection.<init>, modules, MediaCollection.<init> |
| `src/Vixen.Modules/Analysis/BeatsAndBars/PreviewWaveform.cs` | audio, PreviewWaveform.<init> |
| `src/Vixen.Modules/App/ColorGradients/ColorGradientLibrary.cs` | Loading |
| `src/Vixen.Modules/App/Curves/CurveEditor.cs` | sender, CalculateFunction, btnFunctionCurve_Click, e, exp, ... |
| `src/Vixen.Modules/App/Curves/CurveLibrary.cs` | Loading |
| `src/Vixen.Modules/App/Curves/ZedGraph/ZedGraphControl.ContextMenu.cs` | MenuClick_Copy, sender, e, Copy, isShowMessage, ... |
| `src/Vixen.Modules/App/Curves/ZedGraph/ZedGraphControl.Printing.cs` | PrintDocument, DoPrintPreview |
| `src/Vixen.Modules/App/ExportWizard/BulkExportSummaryStage.cs` | sequence, LoadMedia |
| `src/Vixen.Modules/App/FPPClient/Module.cs` | Loading |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs` | sender, e, LipSyncNodeSelect_FormClosing |
| `src/Vixen.Modules/App/LivePreview/ILiveContext.cs` | Execute, data |
| `src/Vixen.Modules/App/Shows/Actions/SequenceAction.cs` | LoadMedia |
| `src/Vixen.Modules/App/SuperScheduler/SetupScheduleForm.cs` | e, sender, buttonOK_Click |
| `src/Vixen.Modules/App/SuperScheduler/SuperSchedulerModule.cs` | Loading |
| `src/Vixen.Modules/App/WebServer/Service/SequenceHelper.cs` | LoadMedia, sequence |
| `src/Vixen.Modules/Controller/E131/SetupForm.cs` | UnivDgvnCellValidating, sender, e, sender, UnivDgvnCellMouseClick, ... |
| `src/Vixen.Modules/Editor/LayerEditor/Converters/DefaultLayerVisibilityConverter.cs` | value, Convert, parameter, culture, targetType |
| `src/Vixen.Modules/Editor/LayerEditor/Converters/HasSetupVisibilityConverter.cs` | parameter, value, culture, Convert, targetType |
| `src/Vixen.Modules/Editor/LayerEditor/Services/ILayerEditorLayerService.cs` | layers, ILayerEditorLayerService, layerMixingFilter, layerMixingFilter, layer, ... |
| `src/Vixen.Modules/Editor/LayerEditor/Services/LayerEditorLayerService.cs` | AddLayer, layers, CreateUniqueLayerName, layers, desiredName, ... |
| `src/Vixen.Modules/Editor/LayerEditor/Services/LayerMixingFilterResolver.cs` | Resolve, LayerMixingFilterResolver, filterTypeId |
| `src/Vixen.Modules/Editor/LayerEditor/ViewModels/LayerEditorViewModel.cs` | sequenceLayers, _exportLayersCommand, ImportLayersCommand, LayersCollectionChanged, _importExportService, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/AutomaticMusicDetection.Designer.cs` | disposing, Dispose |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectModelCandidate.cs` | _effectData, Duration, StartTime, EffectModelCandidate.<init>, LayerId, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/ExportDialog.cs` | ShowDestinationMB |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FindEffectForm.cs` | comboBoxFind_SelectedIndexChanged, sender, comboBoxAvailableEffect_Click, listViewEffectStartTime_DoubleClick, e, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FormEffectEditor.cs` | PreviewPlay |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_AddMultipleEffects.cs` | TimeExistsForAddition, CheckMarkItem, btnOK_Click, sender, e, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_ColorLibrary.cs` | ExportColorLibrary, toolStripButtonExportColors_Click, sender, e |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_CurveLibrary.cs` | toolStripButtonExportCurves_Click, sender, listViewCurves_DragEnter, e, sender, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Effects.cs` | DeselectAllNodes |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_GradientLibrary.cs` | sender, sender, listViewGradients_DragEnter, toolStripButtonExportGradients_Click, ExportGradientLibrary, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Grid.Designer.cs` | InitializeComponent, components, Form_Grid, disposing, Dispose, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Grid.cs` | e, Form_Grid_KeyDown, InstanceId, sender, Form_Grid_DockStateChanged, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/InvalidAudioPathDialog.cs` | LocateAudio, RemoveAudio, KeepAudio, InvalidAudioDialogResult |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/LayerEditor.Designer.cs` | LayerEditor, components, InitializeComponent |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/LayerEditor.cs` | layers, Form_ColorEnter, host, sender, e, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/PapagayoDoc.cs` | fileName, Load |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/PastingMode.cs` | Default, Invert, VisibleMarks, PastingMode |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/PropertyDiscovery.cs` | ContainsTypes, target, types |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs` | toolStripAlignment, toolStripButton_IncreaseTimingSpeed, toolStripStatusLabel3, toolStripStatusLabel_RenderingElements, toolStripMenuItem_zoomRowsOut, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` | element, _showHiddenRows, markCollections, effectGuid, IsFormOnScreen, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_ContextMenu.cs` | sender, timelineControl_ContextSelected, e, AddContextCollectionsMenu |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Hotkeys.cs` | HandleQuickKeySWF, keyData, ProcessCmdKey, HandleSpacebarAction, keyData, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs` | e, e, HandleDockContentToolStripMenuClick, e, e, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs` | sender, toolStripButton_SelectionMode_Click, e, SpeedVisualisation, sender, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceElement.cs` | DrawCanvasContent, TimedSequenceElement, effectNode, TimedSequenceElement.<init>, ElementTimeHasChangedSinceDraw, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsAddedRemovedUndoAction.cs` | m_effectNodes, Count, form, addEffects, EffectsAddedRemovedUndoAction.<init>, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsAddedUndoAction.cs` | nodes, Redo, Description, form, Undo, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsCutUndoAction.cs` | nodes, Description, Undo, form, Redo, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsLayerChangedUndoAction.cs` | form, nodes, EffectsLayerChangedUndoAction.<init> |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsPastedUndoAction.cs` | Description, EffectsPastedUndoAction, nodes, form, EffectsPastedUndoAction.<init>, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsRemovedUndoAction.cs` | EffectsRemovedUndoAction.<init>, form, nodes, EffectsRemovedUndoAction, Undo, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/ElementsTimeChangedUndoAction.cs` | m_changedElements, changedElements, m_moveType, m_form, ElementsTimeChangedUndoAction.<init>, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksAddedRemovedUndoAction.cs` | markCollections, _markCollections, Count, MarksAddedRemovedUndoAction.<init>, MarksAddedRemovedUndoAction, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksAddedUndoAction .cs` | Undo, mc, markCollections, MarksAddedUndoAction.<init>, Redo, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksBulkChangeUndoAction.cs` | MarksBulkChangeUndoAction.<init>, _marksBulkChangeInfo, Description, marksBulkChangeInfo, form, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksRemovedUndoAction.cs` | MarksRemovedUndoAction.<init>, Undo, form, form, mark, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksTimeChangedUndoAction.cs` | _moveType, changedMarks, _form, MarksTimeChangedUndoAction, Description, ... |
| `src/Vixen.Modules/Media/Audio/Audio.cs` | BytesPerSample, SupportsVariableSpeeds, MediaExists, TimingSource, _DisposeAudio, ... |
| `src/Vixen.Modules/Media/Audio/SampleProviders/MonoSampleProvider.cs` | Dispose |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomCreateForm.cs` | e, buttonOK_Click, sender |
| `src/Vixen.Modules/Sequence/Timed/MarkCollection.cs` | IndexOf, time |
| `src/Vixen.Modules/Sequence/Timed/RowSetting.cs` | expanded, rowHeight, RowSetting, visible, RowSetting.<init>, ... |
| `src/Vixen.Modules/Sequence/Timed/TimedSequenceMigrator.cs` | s, IsNutcrackerResource, content, _Version_0_to_1 |
| `src/Vixen.Tests/Sequencer/GridAlignmentNullReferenceTests.cs` | AddElement, GridAlignmentNullReferenceTests, startTime, AlignElementMethods_WhenReferenceElementIsNull_DoNotChangeElementTiming, row, ... |

## Entry Points

- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs::TimedSequenceEditorForm.InitializeComponent`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs::TimedSequenceEditorForm.TimedSequenceEditorForm_Load`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_ContextMenu.cs::TimedSequenceEditorForm.timelineControl_ContextSelected`

## Connected Communities

- **Controls/TimeLineControl +4 dirs · Grid** (33 cross-edges)
- **VixenPreview/Undo +10 dirs** (20 cross-edges)
- **Editor/TimedSequenceEditor +4 dirs** (15 cross-edges)
- **App/Marks +7 dirs** (12 cross-edges)
- **Vixen.Application/Setup +24 dirs** (8 cross-edges)
- **Vixen.Common/NShape +64 dirs** (6 cross-edges)
- **Editor/TimedSequenceEditor +68 dirs** (5 cross-edges)
- **Vixen.Core/Sys +15 dirs** (5 cross-edges)
- **VixenPreview/Shapes +22 dirs** (5 cross-edges)
- **Xml/Serializer +8 dirs** (4 cross-edges)
- **Vixen.Core/Sys +79 dirs** (4 cross-edges)
- **Vixen.Core · Modules** (3 cross-edges)
- **App/Curves +8 dirs** (3 cross-edges)
- **App/ColorGradients +7 dirs** (3 cross-edges)
- **Controls/TimeLineControl · RowList** (3 cross-edges)
- **Controls/TimeLineControl · MarksBar** (2 cross-edges)
- **Vixen.Modules · Bars** (2 cross-edges)
- **App/LipSyncApp +3 dirs · LipSyncMapMatrixEditor** (2 cross-edges)
- **Editor/TimedSequenceEditor · FormEffectEditor** (2 cross-edges)
- **Effect/Effect +80 dirs** (2 cross-edges)
- **App/Curves · GenerateGenericCurveImage** (2 cross-edges)
- **Curves/ZedGraph +7 dirs** (2 cross-edges)
- **App/ColorGradients +8 dirs** (2 cross-edges)
- **Editor/TimedSequenceEditor +5 dirs** (2 cross-edges)
- **Editor/TimedSequenceEditor +3 dirs · PapagayoPhoneme** (2 cross-edges)
- **Vixen.Core/Sys +6 dirs** (2 cross-edges)
- **Vixen.Core/Execution +4 dirs** (2 cross-edges)
- **Editor/EffectEditor +7 dirs** (2 cross-edges)
- **WPFCommon/Services +6 dirs** (1 cross-edges)
- **Module/Service +10 dirs** (1 cross-edges)
- **TimeLineControl/LabeledMarks +2 dirs** (1 cross-edges)
- **Services/EffectDefaults +3 dirs** (1 cross-edges)
- **Vixen.Core · DataStreams** (1 cross-edges)
- **App/ColorGradients +3 dirs** (1 cross-edges)
- **TimeLineControl/LabeledMarks · MarksSelectionManager** (1 cross-edges)
- **Controls/TimeLineControl +1 dirs · TimeLineGlobalStateManager** (1 cross-edges)
- **Vixen.Core/Marks +3 dirs** (1 cross-edges)
- **TimedSequenceEditor/Forms +1 dirs** (1 cross-edges)
- **Sys/Managers +2 dirs** (1 cross-edges)
- **Module/SequenceFilter +5 dirs** (1 cross-edges)
- **Vixen.Modules · DeSerializer** (1 cross-edges)
- **Vixen.Application/Setup +3 dirs · ElementTreeManageTagsRequested** (1 cross-edges)
- **Vixen.Common/ffmpeg +3 dirs** (1 cross-edges)
- **AudioPlayer/SoundTouch +5 dirs** (1 cross-edges)
- **Controls/TimeLineControl · timeToPixels** (1 cross-edges)
- **Editor/TimedSequenceEditor · Form_ColorLibrary** (1 cross-edges)
- **Xml/Serializer +14 dirs** (1 cross-edges)
- **Editor/TimedSequenceEditor +3 dirs · Dispose** (1 cross-edges)
- **TimeLineControl/LabeledMarks +3 dirs** (1 cross-edges)
- **Messages/LivePreview +6 dirs** (1 cross-edges)
- **Effect/Effect +72 dirs** (1 cross-edges)
- **Vixen.Application · DataProfileForm** (1 cross-edges)
- **Controls/TimeLineControl · PaintTagColorDot** (1 cross-edges)
- **Controls/TimeLineControl +4 dirs · ToArray** (1 cross-edges)
- **CustomPropEditor/ViewModels +5 dirs** (1 cross-edges)
- **Vixen.Common/AudioPlayer +3 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-1145")
explore(operation:"context", task:"understand Editor/TimedSequenceEditor +46 dirs", format:"gcx")
relations(operation:"usages", target:{symbol:"src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs::TimedSequenceEditorForm.InitializeComponent"}, format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
