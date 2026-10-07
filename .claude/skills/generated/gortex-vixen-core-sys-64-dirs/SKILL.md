---
name: gortex-vixen-core-sys-64-dirs
description: "Work in the Vixen.Core/Sys +64 dirs area — 1759 symbols across 148 files (78% cohesion)"
---

# Vixen.Core/Sys +64 dirs

1759 symbols | 148 files | 78% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Application/ConfigPreviews.cs`
- `src/Vixen.Application/Setup/DisplaySetup.cs`
- `src/Vixen.Application/Setup/ElementTemplates/ElementTemplateBase.cs`
- `src/Vixen.Application/Setup/ElementTemplates/Icicles.cs`
- `src/Vixen.Application/Setup/ElementTemplates/IntelligentFixtureTemplate.cs`
- `src/Vixen.Application/Setup/ElementTemplates/LipSync.cs`
- `src/Vixen.Application/Setup/ElementTemplates/Megatree.cs`
- `src/Vixen.Application/Setup/ElementTemplates/NumberedGroup.cs`
- `src/Vixen.Application/Setup/ElementTemplates/PixelGrid.cs`
- `src/Vixen.Application/Setup/ElementTemplates/SingleItem.cs`
- `src/Vixen.Application/Setup/ElementTemplates/StarBurst.cs`
- `src/Vixen.Application/Setup/ElementTemplates/StartLocation.cs`
- `src/Vixen.Application/Setup/ElementsChangedEventArgs.cs`
- `src/Vixen.Application/Setup/ISetupElementsControl.cs`
- `src/Vixen.Application/Setup/ISetupPatchingControl.cs`
- `src/Vixen.Application/Setup/SetupControllersSimple.cs`
- `src/Vixen.Application/Setup/SetupElementsTree.Designer.cs`
- `src/Vixen.Application/Setup/SetupElementsTree.cs`
- `src/Vixen.Application/Setup/SetupPatchingGraphical.cs`
- `src/Vixen.Application/Setup/SetupPatchingSimple.Designer.cs`
- `src/Vixen.Application/Setup/SetupPatchingSimple.cs`
- `src/Vixen.Application/VixenApplication.cs`
- `src/Vixen.Common/Controls/ControlsEx/ValueControls/ValueUpDown.cs`
- `src/Vixen.Common/Controls/ElementTree.Designer.cs`
- `src/Vixen.Common/Controls/ElementTree.cs`
- `src/Vixen.Common/Controls/MultiSelectTreeview.cs`
- `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`
- `src/Vixen.Common/Controls/XMLProfileSettings.cs`
- `src/Vixen.Common/DiscreteColorPicker/Views/SingleDiscreteColorPickerView.xaml.cs`
- `src/Vixen.Common/ElementTagManager/ViewModels/ElementTagManagerWindowViewModel.cs`
- `src/Vixen.Common/ElementTagManager/ViewModels/TagColorItem.cs`
- `src/Vixen.Common/Utilities/ElementTemplateHelper.cs`
- `src/Vixen.Core/Execution/Context/LiveContext.cs`
- `src/Vixen.Core/Execution/ControllerUpdateAdjudicator.cs`
- `src/Vixen.Core/Extensions/Extensions.cs`
- `src/Vixen.Core/Intent/CommandIntent.cs`
- `src/Vixen.Core/Intent/RangeIntent.cs`
- `src/Vixen.Core/Module/Editor/IEditorUserInterface.cs`
- `src/Vixen.Core/Module/ModuleDataSet.cs`
- `src/Vixen.Core/Module/Property/IProperty.cs`
- `src/Vixen.Core/Module/Property/IPropertyModuleInstance.cs`
- `src/Vixen.Core/Module/Property/PropertyModuleInstanceBase.cs`
- `src/Vixen.Core/Rule/IElementSetupHelper.cs`
- `src/Vixen.Core/Rule/IElementTemplate.cs`
- `src/Vixen.Core/Services/ApplicationServices.cs`
- `src/Vixen.Core/Services/ElementNodeService.cs`
- `src/Vixen.Core/Sys/DataNodeCollection.cs`
- `src/Vixen.Core/Sys/DataStream.cs`
- `src/Vixen.Core/Sys/ElementNode.cs`
- `src/Vixen.Core/Sys/Execution.cs`
- `src/Vixen.Core/Sys/Extensions.cs`
- `src/Vixen.Core/Sys/GroupNode.cs`
- `src/Vixen.Core/Sys/IElementNode.cs`
- `src/Vixen.Core/Sys/IGroupNode.cs`
- `src/Vixen.Core/Sys/Managers/NodeManager.cs`
- `src/Vixen.Core/Sys/Modules.cs`
- `src/Vixen.Core/Sys/Output/OutputController.cs`
- `src/Vixen.Core/Sys/Program.cs`
- `src/Vixen.Core/Sys/PropertyManager.cs`
- `src/Vixen.Core/Sys/ProxyElementNode.cs`
- `src/Vixen.Core/Sys/State/Execution/Behavior/StandardOpeningBehavior.cs`
- `src/Vixen.Core/Sys/SystemConfig.cs`
- `src/Vixen.Core/Sys/TimeNode.cs`
- `src/Vixen.Core/Sys/VixenSystem.cs`
- `src/Vixen.Core/Utility/NamingUtilities.cs`
- `src/Vixen.Modules/App/ColorGradients/GradientEdit.cs`
- `src/Vixen.Modules/App/Fixture/FixtureFunction.cs`
- `src/Vixen.Modules/App/Fixture/FixtureIndexBase.cs`
- `src/Vixen.Modules/App/Fixture/FixtureSpecification.cs`
- `src/Vixen.Modules/App/LipSyncApp/LipSyncMapStaticData.cs`
- `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs`
- `src/Vixen.Modules/App/Modeling/ElementModeling.cs`
- `src/Vixen.Modules/App/TimedSequenceMapper/SequenceElementMapper/ViewModels/ElementMapperViewModel.cs`
- `src/Vixen.Modules/App/WebServer/Service/SystemHelper.cs`
- `src/Vixen.Modules/Editor/EffectEditor/Editors/BaseColorTypeEditor.cs`
- `src/Vixen.Modules/Editor/EffectEditor/Internal/Util.cs`
- `src/Vixen.Modules/Editor/LayerEditor/Services/ILayerMixingFilterResolver.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs`
- `src/Vixen.Modules/Effect/Alternating/Alternating.cs`
- `src/Vixen.Modules/Effect/AudioHelper/AudioPluginBase.cs`
- `src/Vixen.Modules/Effect/Dissolve/Dissolve.cs`
- `src/Vixen.Modules/Effect/Effect/BaseEffect.cs`
- `src/Vixen.Modules/Effect/Effect/FixtureEffectBase.cs`
- `src/Vixen.Modules/Effect/Effect/FixtureIndexEffectBase.cs`
- `src/Vixen.Modules/Effect/Effect/IDiscreteColorProvider.cs`
- `src/Vixen.Modules/Effect/Effect/PixelEffectBase.cs`
- `src/Vixen.Modules/Effect/Fire/Fire.cs`
- `src/Vixen.Modules/Effect/Fixture/FixtureEffect/FixtureModule.cs`
- `src/Vixen.Modules/Effect/FixtureStrobe/FixtureStrobeModule.cs`
- `src/Vixen.Modules/Effect/LineDance/FanCenterOptions.cs`
- `src/Vixen.Modules/Effect/LineDance/LineDanceModule.cs`
- `src/Vixen.Modules/Effect/Pulse/PulseRenderer.cs`
- `src/Vixen.Modules/Effect/SetPosition/SetPositionModule.cs`
- `src/Vixen.Modules/Effect/SpinColorWheel/SpinColorWheelModule.cs`
- `src/Vixen.Modules/Effect/State/State.cs`
- `src/Vixen.Modules/Effect/State/StateDefinitionDiscovery.cs`
- `src/Vixen.Modules/Effect/Twinkle/Twinkle.cs`
- `src/Vixen.Modules/Effect/Wipe/WipeDirection.cs`
- `src/Vixen.Modules/Effect/Wipe/WipeModule.cs`
- `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveHelper.Designer.cs`
- `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveHelper.cs`
- `src/Vixen.Modules/Preview/VixenPreview/PreviewCustomPropBuilder.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCane.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomProp.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewEllipse.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLightBaseShape.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewRectangle.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTools.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupElementsDocument.cs`
- `src/Vixen.Modules/Property/Color/ColorDescriptor.cs`
- `src/Vixen.Modules/Property/Color/ColorPanel.Designer.cs`
- `src/Vixen.Modules/Property/Color/ColorProperty.cs`
- `src/Vixen.Modules/Property/Color/ColorSetupForm.cs`
- `src/Vixen.Modules/Property/Color/ColorSetupHelper.Designer.cs`
- `src/Vixen.Modules/Property/Color/ColorSetupHelper.cs`
- `src/Vixen.Modules/Property/Face/FaceComponent.cs`
- `src/Vixen.Modules/Property/Face/FaceDescriptor.cs`
- `src/Vixen.Modules/Property/Face/FaceMapItem.cs`
- `src/Vixen.Modules/Property/Face/FaceModule.cs`
- `src/Vixen.Modules/Property/Face/FaceSetupHelper.Designer.cs`
- `src/Vixen.Modules/Property/Face/FaceSetupHelper.cs`
- `src/Vixen.Modules/Property/IntelligentFixture/IntelligentFixtureDescriptor.cs`
- `src/Vixen.Modules/Property/IntelligentFixture/IntelligentFixtureModule.cs`
- `src/Vixen.Modules/Property/Location/LocationModule.cs`
- `src/Vixen.Modules/Property/Order/OrderDescriptor.cs`
- `src/Vixen.Modules/Property/Order/OrderModule.cs`
- `src/Vixen.Modules/Property/Order/OrderSetupHelper.cs`
- `src/Vixen.Modules/Property/Orientation/Orientation.cs`
- `src/Vixen.Modules/Property/Orientation/OrientationDescriptor.cs`
- `src/Vixen.Modules/Property/Orientation/OrientationModule.cs`
- `src/Vixen.Modules/Property/Orientation/OrientationSetupHelper.cs`
- `src/Vixen.Modules/Property/Orientation/SetupForm.cs`
- `src/Vixen.Modules/Property/State/Setup/Models/StateDefinition.cs`
- `src/Vixen.Modules/Property/State/Setup/Services/IStateMapperDialogService.cs`
- `src/Vixen.Modules/Property/State/Setup/Services/StateColorPickerService.cs`
- `src/Vixen.Modules/Property/State/Setup/Services/StateMapperDialogService.cs`
- `src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs`
- `src/Vixen.Modules/Property/State/StateDescriptor.cs`
- `src/Vixen.Modules/Property/State/StateModule.cs`
- `src/Vixen.Modules/Property/State/StateSetupHelper.cs`
- `src/Vixen.Tests/Core/OutputControllerOutputIndexTests.cs`
- `src/Vixen.Tests/Preview/VixenPreview/PreviewCustomPropStateImportTests.cs`
- `src/Vixen.Tests/Property/State/StateMapperDefinitionTests.cs`
- `src/Vixen.Tests/Property/State/StateSetupHelperTests.cs`
- `src/Vixen.Tests/Setup/SetupPatchingSimpleOutputOrderTests.cs`
- `src/Vixen.Tests/Utility/NamingUtilitiesTests.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Application/ConfigPreviews.cs` | e, sender, listViewPreviews_ItemCheck |
| `src/Vixen.Application/Setup/DisplaySetup.cs` | ApplyConfirmedChanges, control_ElementsChanged, e, updateScrollPosition, elements, ... |
| `src/Vixen.Application/Setup/ElementTemplates/ElementTemplateBase.cs` | GetElementsToDelete, GetLeafNodes |
| `src/Vixen.Application/Setup/ElementTemplates/Icicles.cs` | SetupTemplate, selectedNodes, selectedNodes, GenerateElements |
| `src/Vixen.Application/Setup/ElementTemplates/IntelligentFixtureTemplate.cs` | automaticallyOpenAndClosePrism, node, GetAllColorWheelColors, fixture, GenerateElements, ... |
| `src/Vixen.Application/Setup/ElementTemplates/LipSync.cs` | selectedNodes, GenerateElements, SetupTemplate, selectedNodes |
| `src/Vixen.Application/Setup/ElementTemplates/Megatree.cs` | selectedNodes, SetupTemplate, GenerateElements, selectedNodes |
| `src/Vixen.Application/Setup/ElementTemplates/NumberedGroup.cs` | selectedNodes, GenerateElements, SetupTemplate, selectedNodes |
| `src/Vixen.Application/Setup/ElementTemplates/PixelGrid.cs` | e, sender, e, GenerateElements, selectedNodes, ... |
| `src/Vixen.Application/Setup/ElementTemplates/SingleItem.cs` | ConfigureDimming, Cancelled, selectedNodes, _itemName, selectedNodes, ... |
| `src/Vixen.Application/Setup/ElementTemplates/StarBurst.cs` | selectedNodes, selectedNodes, SetupTemplate, GenerateElements |
| `src/Vixen.Application/Setup/ElementTemplates/StartLocation.cs` | StartLocation, BottomRight, TopRight, TopLeft, BottomLeft |
| `src/Vixen.Application/Setup/ElementsChangedEventArgs.cs` | Action, action, Rename, ElementsChangedEventArgs, Add, ... |
| `src/Vixen.Application/Setup/ISetupElementsControl.cs` | nodes, ElementNodesEventArgs.<init>, ElementNodesEventArgs, ElementNodes, SelectedElements, ... |
| `src/Vixen.Application/Setup/ISetupPatchingControl.cs` | nodes, ISetupPatchingControl, UnpatchControllers, SetupPatchingControl, MasterForm, ... |
| `src/Vixen.Application/Setup/SetupControllersSimple.cs` | buttonSelectSourceElements_Click, sender, e |
| `src/Vixen.Application/Setup/SetupElementsTree.Designer.cs` | flowLayoutPanel2, panel1, elementTree, disposing, flowLayoutPanel1, ... |
| `src/Vixen.Application/Setup/SetupElementsTree.cs` | UpdateScrollPosition, e, e, buttonAddProperty_Click, buttonDeleteElements_Click, ... |
| `src/Vixen.Application/Setup/SetupPatchingGraphical.cs` | node, nodes, _allNodeParentsAreInSet |
| `src/Vixen.Application/Setup/SetupPatchingSimple.Designer.cs` | groupBoxControllers, groupBoxElementOptions, labelUnconnectedPatchPointCount, labelConnectedPatchPointCount, labelItemCount, ... |
| `src/Vixen.Application/Setup/SetupPatchingSimple.cs` | _cachedControllersAndOutputs, nodes, _countTypesDescendingFromElements, _UpdateEverything, groupCount, ... |
| `src/Vixen.Application/VixenApplication.cs` | e, sender, sender, optionsToolStripMenuItem_Click, VixenApp_FormClosing, ... |
| `src/Vixen.Common/Controls/ControlsEx/ValueControls/ValueUpDown.cs` | _trackerorientation, TrackerOrientation |
| `src/Vixen.Common/Controls/ElementTree.Designer.cs` | InitializeComponent, toolStripSeparator1, renameNodesToolStripMenuItem, reverseElementsToolStripMenuItem, treeIconsImageList, ... |
| `src/Vixen.Common/Controls/ElementTree.cs` | TreeviewOnAfterCollapse, newNode, treeview_DragStart, sender, e, ... |
| `src/Vixen.Common/Controls/MultiSelectTreeview.cs` | CanReverseElements |
| `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` | ToggleTagOnSelectedRows, tagMenuItem, SelectedRowElementNodes, cascadeToChildren, tagId |
| `src/Vixen.Common/Controls/XMLProfileSettings.cs` | type, RenameNode, newName, xPath |
| `src/Vixen.Common/DiscreteColorPicker/Views/SingleDiscreteColorPickerView.xaml.cs` | SingleDiscreteColorPickerView, GetSelectedColor |
| `src/Vixen.Common/ElementTagManager/ViewModels/ElementTagManagerWindowViewModel.cs` | SaveAsync |
| `src/Vixen.Common/ElementTagManager/ViewModels/TagColorItem.cs` | CommitColor |
| `src/Vixen.Common/Utilities/ElementTemplateHelper.cs` | template, treeElements, addToTree, template, treeElements, ... |
| `src/Vixen.Core/Execution/Context/LiveContext.cs` | TargetNodesComparer, GetHashCode, obj, Equals, y, ... |
| `src/Vixen.Core/Execution/ControllerUpdateAdjudicator.cs` | PetitionForUpdate |
| `src/Vixen.Core/Extensions/Extensions.cs` | GetEnumDescription, value |
| `src/Vixen.Core/Intent/CommandIntent.cs` | CommandIntent, command, CommandIntent.<init>, timeSpan |
| `src/Vixen.Core/Intent/RangeIntent.cs` | RangeIntent |
| `src/Vixen.Core/Module/Editor/IEditorUserInterface.cs` | CloseEditor |
| `src/Vixen.Core/Module/ModuleDataSet.cs` | RemoveDataModel, model |
| `src/Vixen.Core/Module/Property/IProperty.cs` | Owner, IProperty, CloneValues, sourceProperty |
| `src/Vixen.Core/Module/Property/IPropertyModuleInstance.cs` | nodes, SetupElements |
| `src/Vixen.Core/Module/Property/PropertyModuleInstanceBase.cs` | SetupElements, nodes |
| `src/Vixen.Core/Rule/IElementSetupHelper.cs` | Perform, HelperName, IElementSetupHelper, selectedNodes |
| `src/Vixen.Core/Rule/IElementTemplate.cs` | TemplateName, ConfigureDimming, selectedNodes, selectedNodes, SetupTemplate, ... |
| `src/Vixen.Core/Services/ApplicationServices.cs` | GetAllElementSetupHelpers |
| `src/Vixen.Core/Services/ElementNodeService.cs` | uniquifyName, count, name, Rename, createElement, ... |
| `src/Vixen.Core/Sys/DataNodeCollection.cs` | AddRange, values |
| `src/Vixen.Core/Sys/DataStream.cs` | data, AddData |
| `src/Vixen.Core/Sys/ElementNode.cs` | cleanup, content, value, Equals, obj, ... |
| `src/Vixen.Core/Sys/Execution.cs` | UpdateState, NodesChanged, allowed |
| `src/Vixen.Core/Sys/Extensions.cs` | GetElements, nodes, T, AddRange, hashSet, ... |
| `src/Vixen.Core/Sys/GroupNode.cs` | parent, RemoveFromParent, cleanupIfFloating |
| `src/Vixen.Core/Sys/IElementNode.cs` | IsProxy, GetLeafEnumerator, GetNonLeafEnumerator, IElementNode, GetMaxChildDepth, ... |
| `src/Vixen.Core/Sys/IGroupNode.cs` | IGroupNode |
| `src/Vixen.Core/Sys/Managers/NodeManager.cs` | NodeManager.<init>, AddChildToParent, GetEnumerator, node, GetRootNodes, ... |
| `src/Vixen.Core/Sys/Modules.cs` | ClearRepositories |
| `src/Vixen.Core/Sys/Output/OutputController.cs` | GetDataFlowComponentForOutput, output |
| `src/Vixen.Core/Sys/Program.cs` | Program.<init>, original |
| `src/Vixen.Core/Sys/PropertyManager.cs` | GetEnumerator, id, PropertyManager.<init>, owner, propertyTypeId, ... |
| `src/Vixen.Core/Sys/ProxyElementNode.cs` | GetNonLeafEnumerator, Parents, IsLeaf, IsProxy, GetLeafEnumerator, ... |
| `src/Vixen.Core/Sys/State/Execution/Behavior/StandardOpeningBehavior.cs` | Run |
| `src/Vixen.Core/Sys/SystemConfig.cs` | SystemConfig, ClearEffectCacheOnExit, _tags, FileName, _elements, ... |
| `src/Vixen.Core/Sys/TimeNode.cs` | Intersect, right, left |
| `src/Vixen.Core/Sys/VixenSystem.cs` | SaveDisabledDevices, Nodes, Instrumentation, UIContext, Started, ... |
| `src/Vixen.Core/Utility/NamingUtilities.cs` | NamingUtilities, Uniquify, name, names |
| `src/Vixen.Modules/App/ColorGradients/GradientEdit.cs` | _orientation |
| `src/Vixen.Modules/App/Fixture/FixtureFunction.cs` | GetIndexDataBase |
| `src/Vixen.Modules/App/Fixture/FixtureIndexBase.cs` | IndexType, FixtureIndexBase.<init>, FixtureIndexBase, EndValue, UseCurve, ... |
| `src/Vixen.Modules/App/Fixture/FixtureSpecification.cs` | functionName, functionIdentity, GetFunction, GetInUseFunction, functionName, ... |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncMapStaticData.cs` | MigrateMaps |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs` | SelectedElementNodes |
| `src/Vixen.Modules/App/Modeling/ElementModeling.cs` | OrderNodes, TopLevelNode, leafNodes |
| `src/Vixen.Modules/App/TimedSequenceMapper/SequenceElementMapper/ViewModels/ElementMapperViewModel.cs` | Elements |
| `src/Vixen.Modules/App/WebServer/Service/SystemHelper.cs` | SetControllerState, on, Save, id |
| `src/Vixen.Modules/Editor/EffectEditor/Editors/BaseColorTypeEditor.cs` | component, BaseColorTypeEditor.<init>, editedType, GetDiscreteColors, BaseColorTypeEditor, ... |
| `src/Vixen.Modules/Editor/EffectEditor/Internal/Util.cs` | Util, component, GetDiscreteColors |
| `src/Vixen.Modules/Editor/LayerEditor/Services/ILayerMixingFilterResolver.cs` | Resolve, filterTypeId |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` | CloseEditor, e, ElementChangedRowsHandler, sender |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs` | changeMapToolStripMenuItem_Click, sender, e, e, sender, ... |
| `src/Vixen.Modules/Effect/Alternating/Alternating.cs` | GetNodesToRenderOn |
| `src/Vixen.Modules/Effect/AudioHelper/AudioPluginBase.cs` | GetNodesToRenderOn |
| `src/Vixen.Modules/Effect/Dissolve/Dissolve.cs` | _PreRender, GetNodesToRenderOn, TempClass, ElementIndex, ColorIndex, ... |
| `src/Vixen.Modules/Effect/Effect/BaseEffect.cs` | depthOfEffect, GetNodesAtEffectDepth, node, duration, element, ... |
| `src/Vixen.Modules/Effect/Effect/FixtureEffectBase.cs` | indexCurve, node, type, tags, nodeToFunction, ... |
| `src/Vixen.Modules/Effect/Effect/FixtureIndexEffectBase.cs` | UpdateSupportsCurve, functionIdentity, UpdateFixtureCapabilities, GetFunctionIndex, indexValue, ... |
| `src/Vixen.Modules/Effect/Effect/IDiscreteColorProvider.cs` | IDiscreteColorProvider |
| `src/Vixen.Modules/Effect/Effect/PixelEffectBase.cs` | FindLeafParents, GetRenderGroups, FindLeafParents, targetNodes |
| `src/Vixen.Modules/Effect/Fire/Fire.cs` | GetRenderGroups |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/FixtureModule.cs` | GetRenderNodesForRangeFunctionType, colorGradient, RenderRGB, cancellationToken, GetRenderNodesForRGBFunctionType, ... |
| `src/Vixen.Modules/Effect/FixtureStrobe/FixtureStrobeModule.cs` | GetCompatibleIndexValues, function |
| `src/Vixen.Modules/Effect/LineDance/FanCenterOptions.cs` | Centered, Left, FanCenterOptions, Right |
| `src/Vixen.Modules/Effect/LineDance/LineDanceModule.cs` | renderNodes, leftMiddleIndex, leftMiddleIndex, centerIndex, maxIncrementPan, ... |
| `src/Vixen.Modules/Effect/Pulse/PulseRenderer.cs` | IsElementDiscrete, elementNode |
| `src/Vixen.Modules/Effect/SetPosition/SetPositionModule.cs` | SetPositionModule.<init> |
| `src/Vixen.Modules/Effect/SpinColorWheel/SpinColorWheelModule.cs` | function, GetCompatibleIndexValues |
| `src/Vixen.Modules/Effect/State/State.cs` | leafNode, stateItem, node, EnumerateTargetScope, GetAssignedTargetNodes, ... |
| `src/Vixen.Modules/Effect/State/StateDefinitionDiscovery.cs` | node, Traverse |
| `src/Vixen.Modules/Effect/Twinkle/Twinkle.cs` | GetNodesToRenderOn |
| `src/Vixen.Modules/Effect/Wipe/WipeDirection.cs` | Dimaond, Circle, Horizontal, DiagonalUp, Vertical, ... |
| `src/Vixen.Modules/Effect/Wipe/WipeModule.cs` | WipeClass, RenderMovement, GetRenderedLRUD, GetRenderedDiagonal, ReverseColorDirection, ... |
| `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveHelper.Designer.cs` | Dispose, disposing, radioButtonInsertAfter, radioButtonExistingDoNothing, DimmingCurveHelper, ... |
| `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveHelper.cs` | AddNew, component, SimpleMode, sender, Perform, ... |
| `src/Vixen.Modules/Preview/VixenPreview/PreviewCustomPropBuilder.cs` | _leafNodes, node, DefaultStateItemName, CreateElementsForChildren, AddImportedStateDefinitions, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCane.cs` | Reconfigure, node |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomProp.cs` | _zoomLevel, ResizeFromOriginal, PreviewCustomProp, Clone, aspect, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewEllipse.cs` | node, Reconfigure |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLightBaseShape.cs` | node, AddPixels, lightCount |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewRectangle.cs` | Reconfigure, node |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTools.cs` | GetParentNodes, node, node, GetLeafNodes |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs` | sender, e, saveToolStripMenuItem_Click |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupElementsDocument.cs` | SetupTemplate, template, sender, ButtonAddTemplate_Click, AddNodeToTree, ... |
| `src/Vixen.Modules/Property/Color/ColorDescriptor.cs` | ColorDescriptor, Author, ModuleDataClass, Version, TypeName, ... |
| `src/Vixen.Modules/Property/Color/ColorPanel.Designer.cs` | Dispose, disposing |
| `src/Vixen.Modules/Property/Color/ColorProperty.cs` | SingleColor, disposing, Logging, sender, isElementNodeTreeDiscreteColored, ... |
| `src/Vixen.Modules/Property/Color/ColorSetupForm.cs` | ColorSetupForm_Load, SelectRadioButton, sender, e, AnyRadioButtonCheckedChanged, ... |
| `src/Vixen.Modules/Property/Color/ColorSetupHelper.Designer.cs` | radioButtonOptionMultiple, buttonCancel, label3, buttonOk, Dispose, ... |
| `src/Vixen.Modules/Property/Color/ColorSetupHelper.cs` | sender, SetupColors, Green, Red, AnyRadioButtonCheckedChanged, ... |
| `src/Vixen.Modules/Property/Face/FaceComponent.cs` | EyesOpen, Outlines, EyesClosed, FaceComponent, Mouth |
| `src/Vixen.Modules/Property/Face/FaceDescriptor.cs` | Description, TypeId, Author, FaceDescriptor, ModuleStaticDataClass, ... |
| `src/Vixen.Modules/Property/Face/FaceMapItem.cs` | FaceMapItem.<init>, Clone, ElementColor, OnDeserialized, PhonemeList, ... |
| `src/Vixen.Modules/Property/Face/FaceModule.cs` | FaceModule, item, DefaultColor, Logging, HasElementSetupHelper, ... |
| `src/Vixen.Modules/Property/Face/FaceSetupHelper.Designer.cs` | dataGridViewMouth, components, tabMouth, buttonOK, buttonCancel, ... |
| `src/Vixen.Modules/Property/Face/FaceSetupHelper.cs` | HelperName, selectedNodes, width, _phonemeBitmaps, e, ... |
| `src/Vixen.Modules/Property/IntelligentFixture/IntelligentFixtureDescriptor.cs` | Description, _typeId, TypeId, ModuleDataClass, TypeName, ... |
| `src/Vixen.Modules/Property/IntelligentFixture/IntelligentFixtureModule.cs` | IntelligentFixtureModule, FixtureSpecification, HasSetup, _data |
| `src/Vixen.Modules/Property/Location/LocationModule.cs` | CloneValues, sourceProperty |
| `src/Vixen.Modules/Property/Order/OrderDescriptor.cs` | Version, Author, ModuleId, OrderDescriptor, TypeId, ... |
| `src/Vixen.Modules/Property/Order/OrderModule.cs` | HasSetup, AddPatchingOrder, Logging, CloneValues, _data, ... |
| `src/Vixen.Modules/Property/Order/OrderSetupHelper.cs` | selectedNodes, PopulateElementList, selectedNodes, Perform |
| `src/Vixen.Modules/Property/Orientation/Orientation.cs` | Orientation, Vertical, Horizontal |
| `src/Vixen.Modules/Property/Orientation/OrientationDescriptor.cs` | TypeId, ModuleClass, OrientationDescriptor, Author, Version, ... |
| `src/Vixen.Modules/Property/Orientation/OrientationModule.cs` | HasSetup, CloneValues, _data, sourceProperty, Logging, ... |
| `src/Vixen.Modules/Property/Orientation/OrientationSetupHelper.cs` | Perform, HelperName, OrientationSetupHelper, selectedNodes |
| `src/Vixen.Modules/Property/Orientation/SetupForm.cs` | Orientation, SetupForm.<init>, defaultOrientation |
| `src/Vixen.Modules/Property/State/Setup/Models/StateDefinition.cs` | Element |
| `src/Vixen.Modules/Property/State/Setup/Services/IStateMapperDialogService.cs` | data, IStateMapperDialogService, node, Show |
| `src/Vixen.Modules/Property/State/Setup/Services/StateColorPickerService.cs` | ChooseColor, nodes, initialColor, WaitForInitiatingMouseInputAsync, nodes, ... |
| `src/Vixen.Modules/Property/State/Setup/Services/StateMapperDialogService.cs` | StateMapperDialogService, data, node, Show |
| `src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs` | rootNode, TryGetFirstCommonDiscreteColor, color |
| `src/Vixen.Modules/Property/State/StateDescriptor.cs` | ModuleDataClass, ModuleClass, TypeId, Id, Description, ... |
| `src/Vixen.Modules/Property/State/StateModule.cs` | HasElementSetupHelper, element, ModuleData, Id, SetupElements, ... |
| `src/Vixen.Modules/Property/State/StateSetupHelper.cs` | StateSetupHelper.<init>, _dialogService, stateModuleFactory, HelperName, ShowMapper, ... |
| `src/Vixen.Tests/Core/OutputControllerOutputIndexTests.cs` | propertyName, OutputControllerOutputIndexTests.<init>, value, Dispose, SetVixenSystemProperty |
| `src/Vixen.Tests/Preview/VixenPreview/PreviewCustomPropStateImportTests.cs` | value, SetVixenSystemProperty, propertyName, PreviewCustomPropStateImportTests.<init> |
| `src/Vixen.Tests/Property/State/StateMapperDefinitionTests.cs` | id, owner, CreateNode, CreatePropertyManager, name, ... |
| `src/Vixen.Tests/Property/State/StateSetupHelperTests.cs` | Show, properties, Perform_ExistingPropertyCancelled_PreservesExistingData, returnsInvalidData, CreateInvalidDataModule, ... |
| `src/Vixen.Tests/Setup/SetupPatchingSimpleOutputOrderTests.cs` | value, SetVixenSystemProperty, propertyName, Dispose, SetupPatchingSimpleOutputOrderTests.<init> |
| `src/Vixen.Tests/Utility/NamingUtilitiesTests.cs` | Uniquify_NameAlreadyInSet_ReturnsNameWithSuffix2, Uniquify_NameNotInSet_ReturnsNameUnchanged, NamingUtilitiesTests, Uniquify_Name2AlsoInSet_ReturnsNameWithSuffix3 |

## Connected Communities

- **VixenPreview/Shapes +20 dirs** (55 cross-edges)
- **TimedSequenceEditor/Forms +44 dirs** (31 cross-edges)
- **Vixen.Common/Controls +51 dirs** (18 cross-edges)
- **App/ColorGradients +92 dirs** (10 cross-edges)
- **TimedSequenceEditor/Forms +37 dirs** (6 cross-edges)
- **Vixen.Application +6 dirs** (6 cross-edges)
- **Vixen.Core/Sys · GroupNode** (5 cross-edges)
- **Vixen.Common/Controls +5 dirs** (5 cross-edges)
- **Data/Flow +8 dirs** (5 cross-edges)
- **Vixen.Core/Intent +11 dirs** (4 cross-edges)
- **Vixen.Core/Sys +7 dirs** (3 cross-edges)
- **Vixen.Common/NShape +69 dirs** (3 cross-edges)
- **Data/Policy +4 dirs** (3 cross-edges)
- **Vixen.Core/Sys +14 dirs** (3 cross-edges)
- **Vixen.Application/Setup +6 dirs** (2 cross-edges)
- **Vixen.Core/Services +26 dirs** (2 cross-edges)
- **Curves/ZedGraph +10 dirs** (2 cross-edges)
- **Sys/Managers +6 dirs** (2 cross-edges)
- **Sys/Managers +1 dirs** (2 cross-edges)
- **Vixen.Common/Controls · FindControllerNode** (2 cross-edges)
- **Sys/Output +43 dirs** (2 cross-edges)
- **Vixen.Modules · StateRenderInterval** (2 cross-edges)
- **Module/Property +9 dirs** (2 cross-edges)
- **App/ColorGradients +8 dirs** (1 cross-edges)
- **Sys/Managers · GetEnumerator** (1 cross-edges)
- **Vixen.Application +4 dirs** (1 cross-edges)
- **Effect/Effect · ConvertRange** (1 cross-edges)
- **Vixen.Application +5 dirs** (1 cross-edges)
- **VixenPreview/Shapes · MouseMove** (1 cross-edges)
- **Vixen.Core/Sys +1 dirs · BuiltInElementTags** (1 cross-edges)
- **Vixen.Common/NShape +4 dirs** (1 cross-edges)
- **WPFCommon/Services +6 dirs** (1 cross-edges)
- **Sys/Dispatch +48 dirs** (1 cross-edges)
- **Vixen.Core/Sys · WindowsMultimedia** (1 cross-edges)
- **Vixen.Modules · FixtureSpecification** (1 cross-edges)
- **Vixen.Core/Sys +3 dirs · Execution** (1 cross-edges)
- **Cache/Sequence +4 dirs** (1 cross-edges)
- **Vixen.Common/Controls +2 dirs** (1 cross-edges)
- **CustomPropEditor/Model +11 dirs** (1 cross-edges)
- **Sys/Managers · GetComponent** (1 cross-edges)
- **Editor/TimedSequenceEditor +70 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-676")
explore(operation:"context", task:"understand Vixen.Core/Sys +64 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
