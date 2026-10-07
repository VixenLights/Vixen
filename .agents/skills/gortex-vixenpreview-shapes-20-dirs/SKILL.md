---
name: gortex-vixenpreview-shapes-20-dirs
description: "Work in the VixenPreview/Shapes +20 dirs area — 1413 symbols across 79 files (84% cohesion)"
---

# VixenPreview/Shapes +20 dirs

1413 symbols | 79 files | 84% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Application/Setup/DisplaySetup.cs`
- `src/Vixen.Application/Setup/SetupElementsTree.cs`
- `src/Vixen.Common/Controls/RoundButton.cs`
- `src/Vixen.Common/Controls/TimeLineControl/BlockingCollectionExtensions.cs`
- `src/Vixen.Common/Controls/TimeLineControl/ExtensionMethods.cs`
- `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`
- `src/Vixen.Common/WPFCommon/Converters/AdditionConverter.cs`
- `src/Vixen.Core/Common/ColorSpaces.cs`
- `src/Vixen.Core/IO/Xml/SystemConfig/SystemConfigXElementMigrator.cs`
- `src/Vixen.Core/Module/SequenceType/SequenceTypeDataModelBase.cs`
- `src/Vixen.Core/Sys/AppCommand.cs`
- `src/Vixen.Core/Sys/DataStream.cs`
- `src/Vixen.Core/Sys/ElementTagCollection.cs`
- `src/Vixen.Core/Sys/IntentStateList.cs`
- `src/Vixen.Core/Sys/PropertyManager.cs`
- `src/Vixen.Core/Sys/VixenSystem.cs`
- `src/Vixen.Modules/App/CustomPropEditor/ViewModels/ElementModelViewModel.cs`
- `src/Vixen.Modules/App/LipSyncApp/LipSyncPhonemeUtils.cs`
- `src/Vixen.Modules/App/SuperScheduler/SuperSchedulerModule.cs`
- `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageImport/SequencePackageImportSummaryStage.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Services/ExportableMarkCollection.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Services/MarkImportExportService.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkDockerViewModel.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkExportWindowViewModel.cs`
- `src/Vixen.Modules/Effect/Pulse/PulseRenderer.cs`
- `src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIPreviewForm.cs`
- `src/Vixen.Modules/Preview/VixenPreview/OpenGL/Constructs/Shaders/ProgramParam.cs`
- `src/Vixen.Modules/Preview/VixenPreview/OpenGL/OpenGLPreviewForm.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/DiscreteIntentHandler.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItemBaseControl.Designer.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItemBaseControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/IDrawStaticPreviewShape.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewArch.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewArchSetupControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewBaseShape.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCane.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCaneSetupControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustom.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomDefineDisplayItems.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomProp.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomSetupControl.Designer.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomSetupControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewDoublePoint.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewEllipse.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewFlood.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewIcicle.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewIcicleSetupControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLightBaseShape.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLine.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMegaTree.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMegaTreeSetupControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHead.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHeadPartial.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHeadSetupControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMultiString.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewNet.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewNetSetupControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPixel.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPixelGrid.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPoint.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPolyLine.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewRectangle.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewSetElementString.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewSetElements.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewSetElementsUIEditor.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewShapeBaseSetupControl.Designer.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewShapeBaseSetupControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewSingle.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewStar.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewStarBurst.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewStarSetupControl.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTools.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTriangle.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs`
- `src/Vixen.Modules/Sequence/Vixen2x/ConversionProgressForm.cs`
- `src/Vixen.Modules/Sequence/Vixen2x/Vixen2xSequenceImportSM.cs`
- `src/Vixen.Modules/Sequence/Vixen2x/Vixen2xSequenceImporterChannelMapper.cs`
- `src/Vixen.Modules/Sequence/Vixen2x/Vixen3SequenceCreator.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Application/Setup/DisplaySetup.cs` | updateScrollPosition, SelectControllersAndOutputs, controllersAndOutputs |
| `src/Vixen.Application/Setup/SetupElementsTree.cs` | sender, e, buttonSelectDestinationOutputs_Click |
| `src/Vixen.Common/Controls/RoundButton.cs` | RoundButton |
| `src/Vixen.Common/Controls/TimeLineControl/BlockingCollectionExtensions.cs` | TryAdd, item |
| `src/Vixen.Common/Controls/TimeLineControl/ExtensionMethods.cs` | BottomRight, r |
| `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` | SelectedElementTypes |
| `src/Vixen.Common/WPFCommon/Converters/AdditionConverter.cs` | culture, targetType, value, parameter, Convert |
| `src/Vixen.Core/Common/ColorSpaces.cs` | w, r, b, color, g, ... |
| `src/Vixen.Core/IO/Xml/SystemConfig/SystemConfigXElementMigrator.cs` | _Version_13_to_14, content, content, _Version_14_to_15 |
| `src/Vixen.Core/Module/SequenceType/SequenceTypeDataModelBase.cs` | OnSerializing |
| `src/Vixen.Core/Sys/AppCommand.cs` | appCommand, Add |
| `src/Vixen.Core/Sys/DataStream.cs` | data, RemoveRangeData, Clear |
| `src/Vixen.Core/Sys/ElementTagCollection.cs` | Contains, tagId |
| `src/Vixen.Core/Sys/IntentStateList.cs` | AddRangeIntentState, intentStates |
| `src/Vixen.Core/Sys/PropertyManager.cs` | Contains, propertyTypeId |
| `src/Vixen.Core/Sys/VixenSystem.cs` | CleanUpOrphanedElementTagAssignments |
| `src/Vixen.Modules/App/CustomPropEditor/ViewModels/ElementModelViewModel.cs` | DisplayName |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncPhonemeUtils.cs` | VoiceList |
| `src/Vixen.Modules/App/SuperScheduler/SuperSchedulerModule.cs` | CreateMenu |
| `src/Vixen.Modules/App/TimedSequenceMapper/SequencePackageImport/SequencePackageImportSummaryStage.cs` | MapSequence, elementMap, sequence |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Services/ExportableMarkCollection.cs` | MarkCollection, ExportableMarkCollection, IsTextIncluded |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/Services/MarkImportExportService.cs` | color, Color, Vixen, exportType, PangolinBeyond, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkDockerViewModel.cs` | ExportCollection |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkExportWindowViewModel.cs` | MarkExportType |
| `src/Vixen.Modules/Effect/Pulse/PulseRenderer.cs` | levelCurve, colorGradient, color, _GetAllSignificantDataPoints |
| `src/Vixen.Modules/Preview/VixenPreview/GDIPreview/GDIPreviewForm.cs` | UpdateElementPixels, element, Reload, LayoutProps, Setup |
| `src/Vixen.Modules/Preview/VixenPreview/OpenGL/Constructs/Shaders/ProgramParam.cs` | SetValue, param |
| `src/Vixen.Modules/Preview/VixenPreview/OpenGL/OpenGLPreviewForm.cs` | disposing, Dispose, UpdateShapePoints, Setup |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/DiscreteIntentHandler.cs` | states, GetAlphaAffectedColor |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs` | LightShape, IsLightShape |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItemBaseControl.Designer.cs` | DisplayItemBaseControl, disposing, Dispose, InitializeComponent, components |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItemBaseControl.cs` | DisplayItemBaseControl.<init>, _title, shape, DisplayItemBaseControl.<init>, Title, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/IDrawStaticPreviewShape.cs` | InitializeGDI |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewArch.cs` | matchShape, OnDeserialized, aspect, Width, Match, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewArchSetupControl.cs` | shape, PreviewArchSetupControl.<init>, PreviewArchSetupControl |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewBaseShape.cs` | InitializeNonDataMembers, GetSetupControl, point, ZoomLevel, PointToZoomPoint, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCane.cs` | _bottomRightPoint, Right, TopLeft, PreviewCane.<init>, zoomLevel, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCaneSetupControl.cs` | PreviewCaneSetupControl.<init>, PreviewCaneSetupControl, shape |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustom.cs` | forceDraw, point, locked, zoomLevel, Clone, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomDefineDisplayItems.cs` | PreviewCustomDefineDisplayItems.<init> |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomProp.cs` | point, fp, SetSelectPoint, DrawSelectPoints, PointInSelectPoint, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomSetupControl.Designer.cs` | PreviewCustomSetupControl, components, Dispose, lblSync, tableLayoutPanel1, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomSetupControl.cs` | shape, comboBoxStringToEdit_SelectedIndexChanged, OnPropertiesChanged, sender, sender, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewDoublePoint.cs` | _y, y, PreviewDoublePoint.<init>, _pointType, PreviewDoublePoint.<init>, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewEllipse.cs` | aspect, topRight, _topLeft, ResizeFromOriginal, p2Start, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewFlood.cs` | PreviewFlood.<init>, StringType, point, SelectDragPoints, selectedNode, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewIcicle.cs` | InitialStringSpacing, x, changeX, TypeName, _points, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewIcicleSetupControl.cs` | PreviewIcicleSetupControl.<init>, shape, PreviewIcicleSetupControl |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLightBaseShape.cs` | LightStrings, _pixelCache, allIn, referenceHeight, Standard, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLine.cs` | AddEndPadding, x, Right, _lightCount, PreviewLine, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMegaTree.cs` | OnDeserialized, selectedNode, IsPixelTreeSelected, StringCount, x, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMegaTreeSetupControl.cs` | PreviewMegaTreeSetupControl |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHead.cs` | InitializeGDI, PreviewMovingHead.<init> |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHeadPartial.cs` | changeX, point1, selectedNode, MouseMove, y, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHeadSetupControl.cs` | PreviewMovingHeadSetupControl, shape, PreviewMovingHeadSetupControl.<init> |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMultiString.cs` | _points, sender, PointInShape, MouseMove, MoveTo, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewNet.cs` | PointInShape, PreviewNet, Resize, initiallyAssignedNode, Right, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewNetSetupControl.cs` | shape, PreviewNetSetupControl, PreviewNetSetupControl.<init> |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPixel.cs` | TestForDiscrete, _maxAlpha, pixelSize, X, pixelSize, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPixelGrid.cs` | StringOrientation, Y, MouseMove, PointInShape, PixelCount, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPoint.cs` | pointToClone, X, y, type, PreviewPoint, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPolyLine.cs` | MoveTo, PreviewPolyLine, TypeName, CreateDefaultPixels, point1, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewRectangle.cs` | TopLeftPoint, selectedNode, ResizeFromOriginal, BottomRightPoint, aspect, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewSetElementString.cs` | _pixels, Pixels |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewSetElements.cs` | e, PreviewSetElements.<init>, sender, stringName, sender, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewSetElementsUIEditor.cs` | context, GetEditStyle, value, PreviewSetElementsUIEditor, EditValue, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewShapeBaseSetupControl.Designer.cs` | disposing, PreviewShapeBaseSetupControl, Dispose, buttonHelp, propertyGrid, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewShapeBaseSetupControl.cs` | shape, e, sender, sender, buttonHelp_Click, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewSingle.cs` | point, Top, Clone, Layout, PointInShape, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewStar.cs` | Clone, Right, Left, Match, SelectDefaultSelectPoint, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewStarBurst.cs` | Top, TypeName, topLeftStart, Pixels, changeX, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewStarSetupControl.cs` | PreviewStarSetupControl.<init>, shape, PreviewStarSetupControl |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTools.cs` | distance, TransformPreviewPoint, T, CalculateRotation, strings, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTriangle.cs` | TypeName, x, SelectDefaultSelectPoint, MouseMove, changeX, ... |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewControl.cs` | item, AddNodeToPixelMapping, Reload, NodeToPixel, RemoveNodeToPixelMapping, ... |
| `src/Vixen.Modules/Sequence/Vixen2x/ConversionProgressForm.cs` | UpdateProgressBar, value |
| `src/Vixen.Modules/Sequence/Vixen2x/Vixen2xSequenceImportSM.cs` | TargetChannelId, OpenChannel, closeChannel, eventDuration |
| `src/Vixen.Modules/Sequence/Vixen2x/Vixen2xSequenceImporterChannelMapper.cs` | Vixen2xSequenceImporterChannelMapper.<init>, mappingName, mappings, mapExists |
| `src/Vixen.Modules/Sequence/Vixen2x/Vixen3SequenceCreator.cs` | importSequenceData |

## Connected Communities

- **Vixen.Core/Sys +64 dirs** (10 cross-edges)
- **VixenPreview/Shapes · MouseMove** (4 cross-edges)
- **Vixen.Common/Controls +51 dirs** (4 cross-edges)
- **Editor/TimedSequenceEditor +70 dirs** (4 cross-edges)
- **Vixen.Core/Sys +6 dirs** (3 cross-edges)
- **VixenPreview/Shapes · PreviewFlood** (2 cross-edges)
- **VixenPreview/Undo +10 dirs** (2 cross-edges)
- **Vixen.Modules · Draw** (1 cross-edges)
- **Curves/ZedGraph +10 dirs** (1 cross-edges)
- **App/ColorGradients +8 dirs** (1 cross-edges)
- **Vixen.Modules · FixtureFunction** (1 cross-edges)
- **Vixen.Modules/App** (1 cross-edges)
- **Sequence/Vixen2x · Vixen2xSequenceImportSM** (1 cross-edges)
- **Vixen.Common/Controls +5 dirs** (1 cross-edges)
- **VixenPreview/Shapes · Select** (1 cross-edges)
- **Vixen.Core/Sys +14 dirs** (1 cross-edges)
- **Vixen.Modules · Bars** (1 cross-edges)
- **App/ColorGradients +7 dirs** (1 cross-edges)
- **MarksDocker/Services +4 dirs** (1 cross-edges)
- **Preview/VixenPreview · GDIControl** (1 cross-edges)
- **Sequence/Vixen2x · ChannelMapping** (1 cross-edges)
- **Vixen.Application +6 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-1348")
explore(operation:"context", task:"understand VixenPreview/Shapes +20 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
