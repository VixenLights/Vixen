---
name: gortex-vixen-common-controls-50-dirs
description: "Work in the Vixen.Common/Controls +50 dirs area — 531 symbols across 128 files (70% cohesion)"
---

# Vixen.Common/Controls +50 dirs

531 symbols | 128 files | 70% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Application/AboutVixen.Designer.cs`
- `src/Vixen.Application/AboutVixen.cs`
- `src/Vixen.Application/CheckForUpdates.cs`
- `src/Vixen.Application/ConfigPreviews.cs`
- `src/Vixen.Application/DataZipForm.cs`
- `src/Vixen.Application/InstalledModules.cs`
- `src/Vixen.Application/OptionsDialog.cs`
- `src/Vixen.Application/ReleaseNotes.cs`
- `src/Vixen.Application/SelectProfile.cs`
- `src/Vixen.Application/Setup/DisplaySetup.cs`
- `src/Vixen.Application/Setup/ElementTemplates/Icicles.cs`
- `src/Vixen.Application/Setup/ElementTemplates/LipSync.cs`
- `src/Vixen.Application/Setup/ElementTemplates/Megatree.cs`
- `src/Vixen.Application/Setup/ElementTemplates/NumberedGroup.cs`
- `src/Vixen.Application/Setup/ElementTemplates/PixelGrid.cs`
- `src/Vixen.Application/Setup/ElementTemplates/StarBurst.cs`
- `src/Vixen.Application/Setup/SetupPatchingSimple.cs`
- `src/Vixen.Application/Updates/IUpdateService.cs`
- `src/Vixen.Common/Controls/BaseForm.Designer.cs`
- `src/Vixen.Common/Controls/BaseUserControl.cs`
- `src/Vixen.Common/Controls/ComboBoxItem.cs`
- `src/Vixen.Common/Controls/ControllerTree.cs`
- `src/Vixen.Common/Controls/DragAndDropListView.cs`
- `src/Vixen.Common/Controls/DragDropListView/DragDropListView.cs`
- `src/Vixen.Common/Controls/ListSelectDialog.cs`
- `src/Vixen.Common/Controls/MessageBoxForm.cs`
- `src/Vixen.Common/Controls/NameGeneration/LetterCounterEditor.Designer.cs`
- `src/Vixen.Common/Controls/NameGeneration/LetterCounterEditor.cs`
- `src/Vixen.Common/Controls/NameGeneration/LetterIteratorEditor.cs`
- `src/Vixen.Common/Controls/NameGeneration/NumericCounterEditor.cs`
- `src/Vixen.Common/Controls/NameGeneration/WordIteratorEditor.cs`
- `src/Vixen.Common/Controls/NumberDialog.cs`
- `src/Vixen.Common/Controls/SerialPortConfig.cs`
- `src/Vixen.Common/Controls/TextDialog.cs`
- `src/Vixen.Common/Controls/Theme/ThemeColorTable.cs`
- `src/Vixen.Common/Controls/Theme/ThemePropertyGridRenderer.cs`
- `src/Vixen.Common/Controls/Theme/ThemeToolStripRenderer.cs`
- `src/Vixen.Common/Controls/Theme/ThemeUpdateControls.cs`
- `src/Vixen.Common/Controls/Wizard/WizardForm.cs`
- `src/Vixen.Common/Controls/Wizard/WizardStage.cs`
- `src/Vixen.Modules/Analysis/BeatsAndBars/BeatsAndBarsSettings.cs`
- `src/Vixen.Modules/Analysis/BeatsAndBars/MusicStaff.cs`
- `src/Vixen.Modules/App/ColorGradients/ColorGradientLibrarySelector.cs`
- `src/Vixen.Modules/App/ColorGradients/GradientEditPanel.cs`
- `src/Vixen.Modules/App/Curves/CurveEditor.cs`
- `src/Vixen.Modules/App/Curves/CurveLibrarySelector.cs`
- `src/Vixen.Modules/App/Curves/FunctionGenerator.cs`
- `src/Vixen.Modules/App/Curves/ZedGraph/GraphPane.cs`
- `src/Vixen.Modules/App/ExportWizard/BulkExportControllersStage.cs`
- `src/Vixen.Modules/App/Instrumentation/InstrumentationForm.cs`
- `src/Vixen.Modules/App/LipSyncApp/LipSyncMapMatrixEditor.cs`
- `src/Vixen.Modules/App/LipSyncApp/LipSyncMapSelector.cs`
- `src/Vixen.Modules/App/LipSyncApp/LipSyncMultiPicSelect.cs`
- `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs`
- `src/Vixen.Modules/App/LipSyncApp/LipSyncTextConvertFailForm.cs`
- `src/Vixen.Modules/App/LipSyncApp/LipSyncTextConvertForm.cs`
- `src/Vixen.Modules/App/Shows/Editors/PauseTypeEditor.cs`
- `src/Vixen.Modules/App/SuperScheduler/SetupScheduleForm.cs`
- `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageExport/SequencePackageExportSummaryStage.cs`
- `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageImport/ImportConfig.cs`
- `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageImport/SequencePackageImportFinishedStage.Designer.cs`
- `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageImport/SequencePackageImportFinishedStage.cs`
- `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageImport/SequencePackageImportSequencesStage.cs`
- `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageImport/SequencePackageImportSummaryStage.cs`
- `src/Vixen.Modules/App/WebServer/Settings.cs`
- `src/Vixen.Modules/Controller/DDP/DDPSetup.cs`
- `src/Vixen.Modules/Controller/DummyLighting/DummyLightingSetup.cs`
- `src/Vixen.Modules/Controller/E131/UnicastForm.cs`
- `src/Vixen.Modules/Controller/ElexolEtherIO/SetupDialog.cs`
- `src/Vixen.Modules/Controller/GenericSerial/SetupDialog.cs`
- `src/Vixen.Modules/Controller/LauncherController/SetupForm.Designer.cs`
- `src/Vixen.Modules/Controller/LauncherController/SetupForm.cs`
- `src/Vixen.Modules/Controller/OpenDMX/SetupDialog.cs`
- `src/Vixen.Modules/Controller/RDSController/SetupForm.cs`
- `src/Vixen.Modules/Controller/Renard/SetupDialog.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/AutomaticMusicDetection.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/BulkEffectMoveForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectDistributionDialog.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectTimeEditor.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/ExportDialog.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/EffectDefaultsExportSelectionForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FormParameterPicker.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_AddMultipleEffects.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Effects.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Marks.Designer.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Marks.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/InvalidAudioPathDialog.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/MarkCollectionImportDialog.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/SetSequenceLength.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs`
- `src/Vixen.Modules/LayerMixingFilter/ChromaKey/ChromaKeySetup.cs`
- `src/Vixen.Modules/LayerMixingFilter/LumaKey/LumaKeySetup.cs`
- `src/Vixen.Modules/LayerMixingFilter/MaskFill/MaskAndFillSetup.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownItemControl.Designer.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownItemControl.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownSetup.cs`
- `src/Vixen.Modules/OutputFilter/ColorWheelFilter/ColorWheelFilterSetup.cs`
- `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveHelper.cs`
- `src/Vixen.Modules/OutputFilter/DimmingFilter/DimmingFilterSetup.cs`
- `src/Vixen.Modules/OutputFilter/PrismFilter/PrismFilterSetup.cs`
- `src/Vixen.Modules/OutputFilter/ShutterFilter/ShutterFilterSetup.cs`
- `src/Vixen.Modules/OutputFilter/TaggedFilter/TaggedFilterSetup.cs`
- `src/Vixen.Modules/Preview/VixenPreview/LocationOffsetForm.Designer.cs`
- `src/Vixen.Modules/Preview/VixenPreview/LocationOffsetForm.cs`
- `src/Vixen.Modules/Preview/VixenPreview/PreviewPixelSetupForm.cs`
- `src/Vixen.Modules/Preview/VixenPreview/ResizePreviewForm.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomCreateForm.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomSetupControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewFlood.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLightBaseShape.cs`
- `src/Vixen.Modules/Preview/VixenPreview/TemplateDialog.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupDocument.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.cs`
- `src/Vixen.Modules/Property/Color/ColorPanel.Designer.cs`
- `src/Vixen.Modules/Property/Color/ColorPanel.cs`
- `src/Vixen.Modules/Property/Color/ColorSetupForm.cs`
- `src/Vixen.Modules/Property/Color/ColorSetupHelper.cs`
- `src/Vixen.Modules/Property/Face/FaceSetupHelper.cs`
- `src/Vixen.Modules/Property/Grid/SetupForm.cs`
- `src/Vixen.Modules/Property/Location/SetupForm.cs`
- `src/Vixen.Modules/Property/Order/OrderSetupHelper.Designer.cs`
- `src/Vixen.Modules/Property/Order/OrderSetupHelper.cs`
- `src/Vixen.Modules/Property/Order/SetupForm.cs`
- `src/Vixen.Modules/Property/Orientation/SetupForm.cs`
- `src/Vixen.Modules/Sequence/Vixen2x/ConversionProgressForm.Designer.cs`
- `src/Vixen.Modules/Sequence/Vixen2x/ConversionProgressForm.cs`
- `src/Vixen.Modules/Sequence/Vixen2x/Vixen2xSequenceImporterForm.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Application/AboutVixen.Designer.cs` | disposing, pictureBoxIcon, AboutVixen, textBoxLicense, labelHeading, ... |
| `src/Vixen.Application/AboutVixen.cs` | AboutVixen_Load, AboutVixen.<init>, e, sender |
| `src/Vixen.Application/CheckForUpdates.cs` | githubDownloadClient, updateService, CheckForUpdates.<init> |
| `src/Vixen.Application/ConfigPreviews.cs` | ConfigPreviews.<init> |
| `src/Vixen.Application/DataZipForm.cs` | DataZipForm.<init> |
| `src/Vixen.Application/InstalledModules.cs` | InstalledModules.<init> |
| `src/Vixen.Application/OptionsDialog.cs` | OptionsDialog.<init> |
| `src/Vixen.Application/ReleaseNotes.cs` | ReleaseNotes.<init>, releaseTag, updateService |
| `src/Vixen.Application/SelectProfile.cs` | ListBoxProfilesOnDrawItem, e, sender, SelectProfile.<init> |
| `src/Vixen.Application/Setup/DisplaySetup.cs` | DisplaySetup.<init> |
| `src/Vixen.Application/Setup/ElementTemplates/Icicles.cs` | Icicles.<init> |
| `src/Vixen.Application/Setup/ElementTemplates/LipSync.cs` | LipSync.<init> |
| `src/Vixen.Application/Setup/ElementTemplates/Megatree.cs` | Megatree.<init> |
| `src/Vixen.Application/Setup/ElementTemplates/NumberedGroup.cs` | groupName, prefix, NumberedGroup.<init>, count |
| `src/Vixen.Application/Setup/ElementTemplates/PixelGrid.cs` | PixelGrid.<init> |
| `src/Vixen.Application/Setup/ElementTemplates/StarBurst.cs` | Starburst.<init> |
| `src/Vixen.Application/Setup/SetupPatchingSimple.cs` | SetupPatchingSimple.<init> |
| `src/Vixen.Application/Updates/IUpdateService.cs` | IUpdateService |
| `src/Vixen.Common/Controls/BaseForm.Designer.cs` | InitializeComponent |
| `src/Vixen.Common/Controls/BaseUserControl.cs` | BaseUserControl, BaseUserControl.<init> |
| `src/Vixen.Common/Controls/ComboBoxItem.cs` | ToString |
| `src/Vixen.Common/Controls/ControllerTree.cs` | ControllerTree.<init> |
| `src/Vixen.Common/Controls/DragAndDropListView.cs` | ColumnAutoSize, sender, e, List_DrawItem, List_DrawSubItem, ... |
| `src/Vixen.Common/Controls/DragDropListView/DragDropListView.cs` | List_DrawItem, e, sender, List_DrawSubItem, List_DrawColumnHeader, ... |
| `src/Vixen.Common/Controls/ListSelectDialog.cs` | formTitle, items, ListSelectDialog.<init> |
| `src/Vixen.Common/Controls/MessageBoxForm.cs` | MessageBoxForm.<init>, messageBoxTitle, buttonCancelVisible, messageBoxData, icon, ... |
| `src/Vixen.Common/Controls/NameGeneration/LetterCounterEditor.Designer.cs` | InitializeComponent |
| `src/Vixen.Common/Controls/NameGeneration/LetterCounterEditor.cs` | counter, LetterCounterEditor.<init> |
| `src/Vixen.Common/Controls/NameGeneration/LetterIteratorEditor.cs` | LetterIteratorEditor.<init>, counter |
| `src/Vixen.Common/Controls/NameGeneration/NumericCounterEditor.cs` | NumericCounterEditor.<init>, counter |
| `src/Vixen.Common/Controls/NameGeneration/WordIteratorEditor.cs` | counter, WordIteratorEditor.<init> |
| `src/Vixen.Common/Controls/NumberDialog.cs` | minimum, value, maximum, NumberDialog.<init>, prompt, ... |
| `src/Vixen.Common/Controls/SerialPortConfig.cs` | allowDataEdit, _BaudRate, allowParityEdit, allowBaudEdit, _DataBits, ... |
| `src/Vixen.Common/Controls/TextDialog.cs` | prompt, TextDialog.<init> |
| `src/Vixen.Common/Controls/Theme/ThemeColorTable.cs` | ThemeColorTable, Locked, ElementSelected, Linked, _selected, ... |
| `src/Vixen.Common/Controls/Theme/ThemePropertyGridRenderer.cs` | PropertyGridRender, propertyGrid, ThemePropertyGridRenderer |
| `src/Vixen.Common/Controls/Theme/ThemeToolStripRenderer.cs` | item, RenderCheckedButtonFill, image, OnRenderDropDownButtonBackground, e, ... |
| `src/Vixen.Common/Controls/Theme/ThemeUpdateControls.cs` | sender, e, excludes, control, ThemeUpdateControls, ... |
| `src/Vixen.Common/Controls/Wizard/WizardForm.cs` | wizard, WizardForm.<init> |
| `src/Vixen.Common/Controls/Wizard/WizardStage.cs` | WizardStage.<init> |
| `src/Vixen.Modules/Analysis/BeatsAndBars/BeatsAndBarsSettings.cs` | BeatsAndBarsDialog.<init>, audio |
| `src/Vixen.Modules/Analysis/BeatsAndBars/MusicStaff.cs` | MusicStaff.<init> |
| `src/Vixen.Modules/App/ColorGradients/ColorGradientLibrarySelector.cs` | e, ColorGradientLibrarySelector.<init>, listViewColorGradients_SelectedIndexChanged, sender |
| `src/Vixen.Modules/App/ColorGradients/GradientEditPanel.cs` | GradientEditPanel.<init> |
| `src/Vixen.Modules/App/Curves/CurveEditor.cs` | Curve, CurveEditor.<init>, PopulateFormWithCurve, LibraryCurveName, curve |
| `src/Vixen.Modules/App/Curves/CurveLibrarySelector.cs` | listViewCurves_SelectedIndexChanged, e, sender, CurveLibrarySelector.<init> |
| `src/Vixen.Modules/App/Curves/FunctionGenerator.cs` | function, FunctionGenerator.<init> |
| `src/Vixen.Modules/App/Curves/ZedGraph/GraphPane.cs` | points, label, color, AddCurve |
| `src/Vixen.Modules/App/ExportWizard/BulkExportControllersStage.cs` | BulkExportControllersStage.<init>, data |
| `src/Vixen.Modules/App/Instrumentation/InstrumentationForm.cs` | InstrumentationForm.<init> |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncMapMatrixEditor.cs` | LipSyncMapMatrixEditor.<init>, mapData |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncMapSelector.cs` | LipSyncMapSelector.<init> |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncMultiPicSelect.cs` | LipSyncMultiPicSelect.<init> |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs` | LipSyncNodeSelect.<init> |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncTextConvertFailForm.cs` | LipSyncTextConvertFailForm.<init> |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncTextConvertForm.cs` | sender, LipSyncTextConvertForm.<init>, markCollectionCombo_EnabledChanged, e |
| `src/Vixen.Modules/App/Shows/Editors/PauseTypeEditor.cs` | showItem, PauseTypeEditor.<init> |
| `src/Vixen.Modules/App/SuperScheduler/SetupScheduleForm.cs` | scheduleItem, SetupScheduleForm.<init> |
| `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageExport/SequencePackageExportSummaryStage.cs` | data, SequencePackageExportSummaryStage.<init> |
| `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageImport/ImportConfig.cs` | MapFile, ImportConfig.<init>, InputFile, ImportConfig, Sequences |
| `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageImport/SequencePackageImportFinishedStage.Designer.cs` | SequencePackageImportFinishedStage, InitializeComponent, lblFinished, components, disposing, ... |
| `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageImport/SequencePackageImportFinishedStage.cs` | CanMovePrevious, SequencePackageImportFinishedStage.<init>, StageStart, IsPreviousVisible, IsCancelVisible |
| `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageImport/SequencePackageImportSequencesStage.cs` | SequencePackageImportSequencesStage.<init>, data |
| `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageImport/SequencePackageImportSummaryStage.cs` | SequencePackageImportSummaryStage.<init>, data |
| `src/Vixen.Modules/App/WebServer/Settings.cs` | Settings.<init> |
| `src/Vixen.Modules/Controller/DDP/DDPSetup.cs` | DDPSetup.<init>, data |
| `src/Vixen.Modules/Controller/DummyLighting/DummyLightingSetup.cs` | formTitle, DummyLightingSetup.<init>, renderStyle |
| `src/Vixen.Modules/Controller/E131/UnicastForm.cs` | UnicastForm.<init> |
| `src/Vixen.Modules/Controller/ElexolEtherIO/SetupDialog.cs` | e, sliderMinIntensityTrackBar_ValueChanged, sender, SetupDialog.<init>, data |
| `src/Vixen.Modules/Controller/GenericSerial/SetupDialog.cs` | SetupDialog.<init>, data |
| `src/Vixen.Modules/Controller/LauncherController/SetupForm.Designer.cs` | Dispose, disposing, SetupForm, chkHideLaunchedWindows, components, ... |
| `src/Vixen.Modules/Controller/LauncherController/SetupForm.cs` | SetupForm.<init>, chkHideLaunchedWindows_CheckedChanged, data, sender, LauncherData, ... |
| `src/Vixen.Modules/Controller/OpenDMX/SetupDialog.cs` | SetupDialog.<init>, data |
| `src/Vixen.Modules/Controller/RDSController/SetupForm.cs` | cboPortName_SelectedIndexChanged, sender, e |
| `src/Vixen.Modules/Controller/Renard/SetupDialog.cs` | data, SetupDialog.<init> |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/AutomaticMusicDetection.cs` | audio, AutomaticMusicDetection.<init> |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/BulkEffectMoveForm.cs` | BulkEffectMoveForm.<init>, sequencelength, startTime |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectDistributionDialog.cs` | sequenceLength, EffectDistributionDialog.<init> |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectTimeEditor.cs` | EffectTimeEditor.<init>, sequenceLength, minimumDuration, start, duration |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/ExportDialog.cs` | sender, UpdateNetworkList, e, networkListView_ColumnWidthChanged, ExportDialog.<init>, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/EffectDefaultsExportSelectionForm.cs` | EffectDefaultsExportSelectionForm.<init>, summaries |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/FormParameterPicker.cs` | closeInterval, FormParameterPicker.<init>, controls |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_AddMultipleEffects.cs` | Form_AddMultipleEffects.<init>, sequenceLength, SetCheckboxStates, EffectCount |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Effects.cs` | Form_Effects.<init>, timelineControl |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Marks.Designer.cs` | Dispose, Form_Marks, InitializeComponent, components, disposing |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/Form_Marks.cs` | sequence, _markDockerView, e, Form_MarksKeyDown, Form_Marks.<init>, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/InvalidAudioPathDialog.cs` | InvalidAudioPathDialog.<init>, audioPath |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/MarkCollectionImportDialog.cs` | MarkCollectionImportDialog.<init> |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/SetSequenceLength.cs` | SetSequenceLength.<init>, sequenceLength |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` | TimedSequenceEditorForm.<init>, MarksForm |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Toolstrip.cs` | sender, e, toolStripDropDownButtonAlignToStrength_MenuItem_Click |
| `src/Vixen.Modules/LayerMixingFilter/ChromaKey/ChromaKeySetup.cs` | sender, trkSaturationTolerance_Scroll, trkHueTolerance_Scroll, data, e, ... |
| `src/Vixen.Modules/LayerMixingFilter/LumaKey/LumaKeySetup.cs` | lowerLimit, upperLimit, UpdateLimitControls, LumaKeySetup.<init> |
| `src/Vixen.Modules/LayerMixingFilter/MaskFill/MaskAndFillSetup.cs` | requireMixingPartner, excludeZeroValues, MaskAndFillSetup.<init> |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownItemControl.Designer.cs` | InitializeComponent |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownItemControl.cs` | ColorBreakdownItemControl.<init>, color, ColorBreakdownItemControl.<init>, name, breakdownItem, ... |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownSetup.cs` | ColorBreakdownSetup.<init>, breakdownData |
| `src/Vixen.Modules/OutputFilter/ColorWheelFilter/ColorWheelFilterSetup.cs` | ColorWheelFilterSetup.<init>, data |
| `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveHelper.cs` | DimmingCurveHelper.<init>, simpleMode |
| `src/Vixen.Modules/OutputFilter/DimmingFilter/DimmingFilterSetup.cs` | DimmingFilterSetup.<init>, data |
| `src/Vixen.Modules/OutputFilter/PrismFilter/PrismFilterSetup.cs` | PrismFilterSetup.<init>, data |
| `src/Vixen.Modules/OutputFilter/ShutterFilter/ShutterFilterSetup.cs` | ShutterFilterSetup.<init>, data |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/TaggedFilterSetup.cs` | data, TaggedFilterSetup.<init> |
| `src/Vixen.Modules/Preview/VixenPreview/LocationOffsetForm.Designer.cs` | label4, label3, txtX, txtY, components, ... |
| `src/Vixen.Modules/Preview/VixenPreview/LocationOffsetForm.cs` | Offset, LocationOffsetForm.<init>, offset, sender, e, ... |
| `src/Vixen.Modules/Preview/VixenPreview/PreviewPixelSetupForm.cs` | lightSize, startingIndex, prefixName, PreviewPixelSetupForm.<init> |
| `src/Vixen.Modules/Preview/VixenPreview/ResizePreviewForm.cs` | width, scaleShapes, ResizePreviewForm.<init>, height |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomCreateForm.cs` | PreviewCustomCreateForm.<init> |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomSetupControl.cs` | PreviewCustomSetupControl.<init>, shape |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewFlood.cs` | locked, Draw, selected, fp, zoomLevel, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLightBaseShape.cs` | editMode, highlightedElements, selected, forceDraw, locked, ... |
| `src/Vixen.Modules/Preview/VixenPreview/TemplateDialog.cs` | TemplateDialog.<init> |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupDocument.cs` | VixenPreviewSetupDocument.<init> |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupPropertiesDocument.cs` | VixenPreviewSetupPropertiesDocument.<init>, previewControl |
| `src/Vixen.Modules/Property/Color/ColorPanel.Designer.cs` | InitializeComponent |
| `src/Vixen.Modules/Property/Color/ColorPanel.cs` | ColorPanel.<init> |
| `src/Vixen.Modules/Property/Color/ColorSetupForm.cs` | comboBoxColorSet_SelectedIndexChanged, e, sender, ColorSetupForm.<init> |
| `src/Vixen.Modules/Property/Color/ColorSetupHelper.cs` | ColorSetupHelper.<init> |
| `src/Vixen.Modules/Property/Face/FaceSetupHelper.cs` | sender, e, SetGridDefaults, PaintRowCell, dgv, ... |
| `src/Vixen.Modules/Property/Grid/SetupForm.cs` | SetupForm.<init>, height, productRequired, width |
| `src/Vixen.Modules/Property/Location/SetupForm.cs` | data, SetupForm.<init> |
| `src/Vixen.Modules/Property/Order/OrderSetupHelper.Designer.cs` | InitializeComponent |
| `src/Vixen.Modules/Property/Order/OrderSetupHelper.cs` | OrderSetupHelper.<init> |
| `src/Vixen.Modules/Property/Order/SetupForm.cs` | data, SetupForm.<init> |
| `src/Vixen.Modules/Property/Orientation/SetupForm.cs` | SetupForm.<init>, defaultOrientation |
| `src/Vixen.Modules/Sequence/Vixen2x/ConversionProgressForm.Designer.cs` | InitializeComponent, ConversionProgressForm, lblStatusLine, disposing, components, ... |
| `src/Vixen.Modules/Sequence/Vixen2x/ConversionProgressForm.cs` | StatusLineLabel, ConversionProgressForm.<init> |
| `src/Vixen.Modules/Sequence/Vixen2x/Vixen2xSequenceImporterForm.cs` | Vixen2xSequenceImporterForm.<init>, staticModuleData, ParseV2SequenceData, Vixen2File |

## Connected Communities

- **Editor/TimedSequenceEditor +68 dirs** (56 cross-edges)
- **Vixen.Modules · ColorCollectionLibrary_Form** (5 cross-edges)
- **Vixen.Application/Setup +25 dirs** (5 cross-edges)
- **VixenPreview/Shapes +22 dirs** (3 cross-edges)
- **Sys/Output +32 dirs** (1 cross-edges)
- **Vixen.Core/Sys +77 dirs** (1 cross-edges)
- **Shows/Editors · LaunchTypeEditor** (1 cross-edges)
- **Controller/DDP** (1 cross-edges)
- **Analysis/BeatsAndBars · BeatsAndBarsDialog** (1 cross-edges)
- **App/LipSyncApp +3 dirs** (1 cross-edges)
- **Sequence/Vixen2x · Vixen2xSequenceImporterForm** (1 cross-edges)
- **Controller/DummyLighting · DummyLightingOutputForm** (1 cross-edges)
- **Editor/TimedSequenceEditor +3 dirs · PropertyMetaData** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-95")
explore(operation:"context", task:"understand Vixen.Common/Controls +50 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
