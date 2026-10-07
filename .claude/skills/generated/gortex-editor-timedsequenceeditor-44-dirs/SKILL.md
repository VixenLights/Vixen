---
name: gortex-editor-timedsequenceeditor-44-dirs
description: "Work in the Editor/TimedSequenceEditor +44 dirs area — 1834 symbols across 116 files (80% cohesion)"
---

# Editor/TimedSequenceEditor +44 dirs

1834 symbols | 116 files | 80% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Application/ConfigPreviews.cs`
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
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksDeletedEventArgs.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksMoveResizeInfo.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksMovedEventArgs.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksMovingEventArgs.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksPastedEventArgs.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksSelectionManager.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/PhonemeBreakdownEventArgs.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/PlayRangeEventArgs.cs`
- `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/TimeLineGlobalEventManager.cs`
- `src/Vixen.Common/Controls/TimeLineControl/MarkRow.cs`
- `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Row.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Ruler.cs`
- `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`
- `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs`
- `src/Vixen.Common/Controls/ToolStripEx.cs`
- `src/Vixen.Core/Execution/Context/LiveContext.cs`
- `src/Vixen.Core/Execution/Context/PreCachingSequenceContext.cs`
- `src/Vixen.Core/Execution/Context/PreviewContext.cs`
- `src/Vixen.Core/Execution/Context/ProgramContext.cs`
- `src/Vixen.Core/Execution/Context/SequenceContext.cs`
- `src/Vixen.Core/Execution/MediaTimingProvider.cs`
- `src/Vixen.Core/Execution/ProgramExecutor.cs`
- `src/Vixen.Core/Marks/IMark.cs`
- `src/Vixen.Core/Marks/IMarkCollection.cs`
- `src/Vixen.Core/Module/App/AppModuleInstanceBase.cs`
- `src/Vixen.Core/Module/App/AppModuleRepository.cs`
- `src/Vixen.Core/Module/App/IApp.cs`
- `src/Vixen.Core/Module/Editor/IEditorUserInterface.cs`
- `src/Vixen.Core/Module/Effect/EffectModuleInstanceBase.cs`
- `src/Vixen.Core/Module/Effect/IEffectModuleInstance.cs`
- `src/Vixen.Core/Module/Input/InputEffectMap.cs`
- `src/Vixen.Core/Module/ModuleImplementationMethod.cs`
- `src/Vixen.Core/Sys/DataStream.cs`
- `src/Vixen.Core/Sys/EffectNode.cs`
- `src/Vixen.Core/Sys/IHasLayers.cs`
- `src/Vixen.Core/Sys/IHasMedia.cs`
- `src/Vixen.Core/Sys/LayerMixing/SequenceLayers.cs`
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
- `src/Vixen.Modules/App/LivePreview/Module.cs`
- `src/Vixen.Modules/App/Marks/MarkCollection.cs`
- `src/Vixen.Modules/App/Shows/Actions/SequenceAction.cs`
- `src/Vixen.Modules/App/SuperScheduler/SetupScheduleForm.cs`
- `src/Vixen.Modules/App/SuperScheduler/SuperSchedulerModule.cs`
- `src/Vixen.Modules/App/WebServer/Service/SequenceHelper.cs`
- `src/Vixen.Modules/Controller/E131/SetupForm.cs`
- `src/Vixen.Modules/Controller/ElexolEtherIO/SetupDialog.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectModelCandidate.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/ExportDialog.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/ColorCollectionLibrary_Form.Designer.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FindEffectForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FormEffectEditor.Designer.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FormEffectEditor.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_AddMultipleEffects.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_ColorLibrary.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_CurveLibrary.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Effects.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_GradientLibrary.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Grid.Designer.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Grid.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/InvalidAudioPathDialog.cs`
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
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsModifiedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsPastedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsRemovedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/ElementsTimeChangedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksAddedRemovedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksAddedUndoAction .cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksRemovedUndoAction.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksTimeChangedUndoAction.cs`
- `src/Vixen.Modules/Effect/Alternating/Alternating.cs`
- `src/Vixen.Modules/Effect/Dissolve/Dissolve.cs`
- `src/Vixen.Modules/Effect/Fireworks/Fireworks.cs`
- `src/Vixen.Modules/Effect/LipSync/LipSync.cs`
- `src/Vixen.Modules/Effect/Shapes/Shapes.cs`
- `src/Vixen.Modules/Effect/Strobe/Strobe.cs`
- `src/Vixen.Modules/Effect/Text/Text.cs`
- `src/Vixen.Modules/Media/Audio/Audio.cs`
- `src/Vixen.Modules/Media/Audio/SampleProviders/MonoSampleProvider.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomCreateForm.cs`
- `src/Vixen.Modules/Sequence/Timed/MarkCollection.cs`
- `src/Vixen.Modules/Sequence/Timed/RowSetting.cs`
- `src/Vixen.Tests/Sequencer/GridAlignmentNullReferenceTests.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Application/ConfigPreviews.cs` | e, sender, ConfigPreviews_FormClosing |
| `src/Vixen.Application/DataZipForm.cs` | sender, e, buttonStartCancel_Click |
| `src/Vixen.Common/AudioPlayer/FileReader/AudioFileReader.cs` | Dispose, disposing |
| `src/Vixen.Common/BaseSequence/Sequence.cs` | _DataListener, sequenceFilterNode, Dispose, module, AddSequenceFilter, ... |
| `src/Vixen.Common/BaseSequence/SequenceData.cs` | SequenceData |
| `src/Vixen.Common/Controls/MenuStripEx.cs` | MenuStripEx, clickThrough |
| `src/Vixen.Common/Controls/MessageBoxForm.Designer.cs` | txtMessage, messageIcon, MessageBoxForm, flowLayoutPanel1, buttonOk, ... |
| `src/Vixen.Common/Controls/MessageBoxForm.cs` | sender, msgIcon, messageIcon_Paint, e, MessageBoxForm_FormClosed, ... |
| `src/Vixen.Common/Controls/TimeLineControl/ElementMoveType.cs` | AlignEnd, Move, Distribute, AlignStart, Resize, ... |
| `src/Vixen.Common/Controls/TimeLineControl/Grid.cs` | referenceElement, referenceElement, holdDuration, holdEndTime, elements, ... |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarkTimeInfo.cs` | Duration, EndTime, mark, StartTime, MarkTimeInfo, ... |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksBulkChangeInfo.cs` | OriginalMarks, MarksBulkChangeInfo.<init>, Add, markValue, markKey, ... |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksDeletedEventArgs.cs` | MarksDeletedEventArgs, marks, Marks, MarksDeletedEventArgs.<init> |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksMoveResizeInfo.cs` | modifyingMarks, MarksMoveResizeInfo, OriginalMarks, MarksMoveResizeInfo.<init> |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksMovedEventArgs.cs` | MarksMovedEventArgs, MarksMovedEventArgs.<init>, marksMoveResizeInfo, MoveType, MoveResizeInfo, ... |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksMovingEventArgs.cs` | marks, MarksMovingEventArgs.<init>, Marks |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksPastedEventArgs.cs` | marks, MarksPastedEventArgs, MarksPastedEventArgs.<init>, Marks |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/MarksSelectionManager.cs` | MarksSelectionManager, SelectedMarks, ClearSelected, id, mark, ... |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/PhonemeBreakdownEventArgs.cs` | PhonemeBreakdownEventArgs.<init>, type, marks, Marks |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/PlayRangeEventArgs.cs` | PlayRangeEventArgs, PlayRangeEventArgs.<init>, endTime, StartTime, EndTime, ... |
| `src/Vixen.Common/Controls/TimeLineControl/LabeledMarks/TimeLineGlobalEventManager.cs` | e, OnMarksPasted, e, OnDeleteMark, e, ... |
| `src/Vixen.Common/Controls/TimeLineControl/MarkRow.cs` | GetMarkAtIndex, elementMaster, IndexOfMark, element, SetStackIndexes, ... |
| `src/Vixen.Common/Controls/TimeLineControl/MarksBar.cs` | e, CutMarksOnClick, WaitForDragMove, sender, sender, ... |
| `src/Vixen.Common/Controls/TimeLineControl/Row.cs` | input, TreeId, TreePathName, CalculateMd5Hash |
| `src/Vixen.Common/Controls/TimeLineControl/Ruler.cs` | DeleteMark_Click, e, DeleteSelectedMarks, ts, locked, ... |
| `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` | e, DetachRowToggledEvent, sender, LockRulerHeight, row, ... |
| `src/Vixen.Common/Controls/TimeLineControl/Waveform.cs` | Half, Full, WaveformStyle, WaveformStyle |
| `src/Vixen.Common/Controls/ToolStripEx.cs` | clickThrough, ToolStripEx |
| `src/Vixen.Core/Execution/Context/LiveContext.cs` | Execute, data |
| `src/Vixen.Core/Execution/Context/PreCachingSequenceContext.cs` | node, GetLayerForNode |
| `src/Vixen.Core/Execution/Context/PreviewContext.cs` | GetLayerForNode, node |
| `src/Vixen.Core/Execution/Context/ProgramContext.cs` | GetLayerForNode, node |
| `src/Vixen.Core/Execution/Context/SequenceContext.cs` | GetLayerForNode, node |
| `src/Vixen.Core/Execution/MediaTimingProvider.cs` | sourceName, sequence, TimingProviderTypeName, MediaTimingProvider, sequence, ... |
| `src/Vixen.Core/Execution/ProgramExecutor.cs` | SequenceLayers |
| `src/Vixen.Core/Marks/IMark.cs` | StartTime, Duration, Text, IMark, EndTime, ... |
| `src/Vixen.Core/Marks/IMarkCollection.cs` | SwapPlaces, lhs, rhs, mark, mark, ... |
| `src/Vixen.Core/Module/App/AppModuleInstanceBase.cs` | Loading |
| `src/Vixen.Core/Module/App/AppModuleRepository.cs` | id, Add |
| `src/Vixen.Core/Module/App/IApp.cs` | Loading |
| `src/Vixen.Core/Module/Editor/IEditorUserInterface.cs` | Save, filePath |
| `src/Vixen.Core/Module/Effect/EffectModuleInstanceBase.cs` | MarkDirty |
| `src/Vixen.Core/Module/Effect/IEffectModuleInstance.cs` | MarkDirty |
| `src/Vixen.Core/Module/Input/InputEffectMap.cs` | input, GenerateEffect, effectTimeSpan |
| `src/Vixen.Core/Module/ModuleImplementationMethod.cs` | parameters, Invoke |
| `src/Vixen.Core/Sys/DataStream.cs` | RemoveRangeData, data |
| `src/Vixen.Core/Sys/EffectNode.cs` | EffectNode, EndTime, other, EffectNode.<init>, CompareTo, ... |
| `src/Vixen.Core/Sys/IHasLayers.cs` | IHasLayers, GetSequenceLayerManager |
| `src/Vixen.Core/Sys/IHasMedia.cs` | module, RemoveMedia, ClearMedia, GetAllMedia |
| `src/Vixen.Core/Sys/LayerMixing/SequenceLayers.cs` | layerId, ContainsLayer, node, GetLayer |
| `src/Vixen.Modules/Analysis/BeatsAndBars/PreviewWaveform.cs` | PreviewWaveform.<init>, audio |
| `src/Vixen.Modules/App/ColorGradients/ColorGradientLibrary.cs` | Loading |
| `src/Vixen.Modules/App/Curves/CurveEditor.cs` | x, exp, btnFunctionCurve_Click, CalculateFunction, sender, ... |
| `src/Vixen.Modules/App/Curves/CurveLibrary.cs` | Loading |
| `src/Vixen.Modules/App/Curves/ZedGraph/ZedGraphControl.ContextMenu.cs` | CopyEmf, isShowMessage |
| `src/Vixen.Modules/App/Curves/ZedGraph/ZedGraphControl.Printing.cs` | DoPrintPreview, PrintDocument |
| `src/Vixen.Modules/App/ExportWizard/BulkExportSummaryStage.cs` | LoadMedia, sequence |
| `src/Vixen.Modules/App/FPPClient/Module.cs` | Loading |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs` | e, LipSyncNodeSelect_FormClosing, sender |
| `src/Vixen.Modules/App/LivePreview/ILiveContext.cs` | data, Execute |
| `src/Vixen.Modules/App/LivePreview/Module.cs` | Loading |
| `src/Vixen.Modules/App/Marks/MarkCollection.cs` | mark, MarkCollection.<init>, end, OffsetMarksByTime, start, ... |
| `src/Vixen.Modules/App/Shows/Actions/SequenceAction.cs` | LoadMedia |
| `src/Vixen.Modules/App/SuperScheduler/SetupScheduleForm.cs` | sender, buttonOK_Click, e |
| `src/Vixen.Modules/App/SuperScheduler/SuperSchedulerModule.cs` | Loading |
| `src/Vixen.Modules/App/WebServer/Service/SequenceHelper.cs` | sequence, LoadMedia |
| `src/Vixen.Modules/Controller/E131/SetupForm.cs` | e, UnivDgvnCellMouseClick, sender, sender, e, ... |
| `src/Vixen.Modules/Controller/ElexolEtherIO/SetupDialog.cs` | a, array, b, okButton_Click, sender, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectModelCandidate.cs` | Duration, EffectModelCandidate, _effectData, GetEffectData, LayerId, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/ExportDialog.cs` | ShowDestinationMB |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/ColorCollectionLibrary_Form.Designer.cs` | disposing, Dispose |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FindEffectForm.cs` | DisplaySelectedEffects, listViewEffectStartTime_DoubleClick, e, sender, sender, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FormEffectEditor.Designer.cs` | components, InitializeComponent, FormEffectEditor |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FormEffectEditor.cs` | Dispose, _selectionChangeBuffer_Tick, TogglePreviewState, CommitTextBoxValue, e, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_AddMultipleEffects.cs` | CheckMarkItem, sender, source, TimeExistsForAddition, e, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_ColorLibrary.cs` | e, sender, ExportColorLibrary, toolStripButtonExportColors_Click |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_CurveLibrary.cs` | ExportCurveLibrary, e, toolStripButtonExportCurves_Click, sender |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Effects.cs` | DeselectAllNodes |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_GradientLibrary.cs` | toolStripButtonExportGradients_Click, sender, e, ExportGradientLibrary |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Grid.Designer.cs` | Dispose, Form_Grid, InitializeComponent, disposing, timelineControl, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Grid.cs` | e, sender, InstanceId, TimelineControl, Form_Grid_KeyDown, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/InvalidAudioPathDialog.cs` | InvalidAudioDialogResult, RemoveAudio, KeepAudio, LocateAudio |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/PapagayoDoc.cs` | fileName, Load |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/PastingMode.cs` | Invert, Default, VisibleMarks, PastingMode |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/PropertyDiscovery.cs` | types, ContainsTypes, target |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs` | divideMarksEvenlyToolStripMenuItem, toolStripMenuItem_PasteInvert, pixelToolStripMenuItem, redoToolStripMenuItem, lyricConverterToolStripMenuItem, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` | PlayPauseToggle, _undoMgr, AlignTo_Threshold, name, timeSpan, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_ContextMenu.cs` | sender, timelineControl_ContextSelected, e, AddContextCollectionsMenu |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Hotkeys.cs` | keyData, msg, HandleQuickKeySWI, IsActiveRowNavigationKey, ProcessCmdKey, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs` | e, delay5SecondsToolStripMenuItem_Click, gridWindowToolStripMenuItem_Click, toolStripMenuItem_AutoSave_Click, toolStripMenuItem_zoomTimeOut_Click, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs` | sender, currentContextMenu, resetEffectsToolStripMenuItem_Click, sender, toolStripButton_Play_Click, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceElement.cs` | redBorder, g, overallWidth, OnTimeChanged, endTime, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsAddedRemovedUndoAction.cs` | m_count, removeEffects, nodes, Count, m_effectNodes, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsAddedUndoAction.cs` | Undo, form, nodes, EffectsAddedUndoAction, Redo, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsCutUndoAction.cs` | EffectsCutUndoAction, EffectsCutUndoAction.<init>, Redo, form, Undo, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsModifiedUndoAction.cs` | SwapEffectData |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsPastedUndoAction.cs` | Redo, Description, Undo, EffectsPastedUndoAction.<init>, form, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/EffectsRemovedUndoAction.cs` | Description, nodes, form, Redo, EffectsRemovedUndoAction.<init>, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/ElementsTimeChangedUndoAction.cs` | m_form, m_moveType, Description, m_changedElements, ElementsTimeChangedUndoAction.<init>, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksAddedRemovedUndoAction.cs` | MarksAddedRemovedUndoAction, markCollections, RemoveMark, MarksAddedRemovedUndoAction.<init>, form, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksAddedUndoAction .cs` | mark, form, Redo, Description, markCollections, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksRemovedUndoAction.cs` | MarksRemovedUndoAction.<init>, Description, form, markCollections, Undo, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Undo/MarksTimeChangedUndoAction.cs` | Description, form, _changedMarks, _moveType, _form, ... |
| `src/Vixen.Modules/Effect/Alternating/Alternating.cs` | _marks |
| `src/Vixen.Modules/Effect/Dissolve/Dissolve.cs` | _marks |
| `src/Vixen.Modules/Effect/Fireworks/Fireworks.cs` | _marks |
| `src/Vixen.Modules/Effect/LipSync/LipSync.cs` | _marks |
| `src/Vixen.Modules/Effect/Shapes/Shapes.cs` | _marks |
| `src/Vixen.Modules/Effect/Strobe/Strobe.cs` | _marks |
| `src/Vixen.Modules/Effect/Text/Text.cs` | _marks |
| `src/Vixen.Modules/Media/Audio/Audio.cs` | Logging, _DisposeAudio, Pause, disposing, BytesPerSample, ... |
| `src/Vixen.Modules/Media/Audio/SampleProviders/MonoSampleProvider.cs` | Dispose |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomCreateForm.cs` | e, sender, buttonOK_Click |
| `src/Vixen.Modules/Sequence/Timed/MarkCollection.cs` | time, IndexOf |
| `src/Vixen.Modules/Sequence/Timed/RowSetting.cs` | visible, RowHeight, RowSetting.<init>, RowSetting, rowHeight, ... |
| `src/Vixen.Tests/Sequencer/GridAlignmentNullReferenceTests.cs` | row, duration, CreateTimelineControl, AlignElementMethods_WhenReferenceElementIsNull_DoNotChangeElementTiming, AddElement, ... |

## Entry Points

- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs::TimedSequenceEditorForm.InitializeComponent`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_ContextMenu.cs::TimedSequenceEditorForm.timelineControl_ContextSelected`

## Connected Communities

- **Controls/TimeLineControl +7 dirs** (35 cross-edges)
- **VixenPreview/Undo +11 dirs** (18 cross-edges)
- **App/Marks +7 dirs** (13 cross-edges)
- **Editor/TimedSequenceEditor +3 dirs · PropertyMetaData** (10 cross-edges)
- **Vixen.Core/Sys +77 dirs** (9 cross-edges)
- **Vixen.Application/Setup +25 dirs** (8 cross-edges)
- **Vixen.Common/Controls +5 dirs** (5 cross-edges)
- **Controls/TimeLineControl · MarksBar** (4 cross-edges)
- **LayerEditor/ImportExport +16 dirs** (4 cross-edges)
- **VixenPreview/Shapes +22 dirs** (4 cross-edges)
- **Xml/Serializer +8 dirs** (3 cross-edges)
- **Controls/TimeLineControl · RowList** (3 cross-edges)
- **Editor/TimedSequenceEditor +68 dirs** (3 cross-edges)
- **Editor/TimedSequenceEditor +3 dirs · TryGetDragDropData** (2 cross-edges)
- **Editor/EffectEditor +7 dirs** (2 cross-edges)
- **Vixen.Core/Execution +4 dirs · GetTimingSource** (2 cross-edges)
- **Vixen.Application +5 dirs** (2 cross-edges)
- **Services/EffectDefaults +3 dirs** (2 cross-edges)
- **Vixen.Core/Sys +6 dirs** (2 cross-edges)
- **Vixen.Modules · LipSyncMapMatrixEditor** (2 cross-edges)
- **Editor/TimedSequenceEditor +3 dirs · PapagayoPhoneme** (2 cross-edges)
- **Controls/TimeLineControl · timeToPixels** (2 cross-edges)
- **Curves/ZedGraph +7 dirs** (2 cross-edges)
- **Vixen.Core/Marks +3 dirs** (1 cross-edges)
- **App/ExportWizard · ExportWizardModule** (1 cross-edges)
- **App/Curves · GenerateGenericCurveImage** (1 cross-edges)
- **TimeLineControl/LabeledMarks +3 dirs** (1 cross-edges)
- **App/Curves +10 dirs** (1 cross-edges)
- **Vixen.Common/Controls +1 dirs · MessageType** (1 cross-edges)
- **Property/Color +17 dirs** (1 cross-edges)
- **Xml/Serializer +14 dirs** (1 cross-edges)
- **Controls/TimeLineControl +1 dirs · TimeLineGlobalStateManager** (1 cross-edges)
- **Vixen.Core/Sys +14 dirs** (1 cross-edges)
- **App/ColorGradients +6 dirs** (1 cross-edges)
- **WPFCommon/Services +6 dirs** (1 cross-edges)
- **App/ColorGradients +7 dirs** (1 cross-edges)
- **Messages/LivePreview +6 dirs** (1 cross-edges)
- **Vixen.Common/AudioPlayer +3 dirs** (1 cross-edges)
- **TimedSequenceEditor/Forms +1 dirs** (1 cross-edges)
- **Effect/Effect +104 dirs** (1 cross-edges)
- **App/CustomPropEditor · Configuration** (1 cross-edges)
- **Vixen.Application/Setup +3 dirs · ElementTreeManageTagsRequested** (1 cross-edges)
- **Vixen.Common/NShape +70 dirs** (1 cross-edges)
- **Controls/TimeLineControl · PaintTagColorDot** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-1162")
explore(operation:"context", task:"understand Editor/TimedSequenceEditor +44 dirs", format:"gcx")
relations(operation:"usages", target:{symbol:"src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.Designer.cs::TimedSequenceEditorForm.InitializeComponent"}, format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
