---
name: gortex-vixen-core-sys-77-dirs
description: "Work in the Vixen.Core/Sys +77 dirs area — 2075 symbols across 190 files (79% cohesion)"
---

# Vixen.Core/Sys +77 dirs

2075 symbols | 190 files | 79% cohesion

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
- `src/Vixen.Common/Controls/ElementTree.Designer.cs`
- `src/Vixen.Common/Controls/ElementTree.cs`
- `src/Vixen.Common/Controls/MultiSelectTreeview.cs`
- `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs`
- `src/Vixen.Common/Controls/XMLProfileSettings.cs`
- `src/Vixen.Common/DiscreteColorPicker/Views/SingleDiscreteColorPickerView.xaml.cs`
- `src/Vixen.Common/ElementTagManager/ViewModels/ElementTagManagerWindowViewModel.cs`
- `src/Vixen.Common/ElementTagManager/ViewModels/TagColorItem.cs`
- `src/Vixen.Common/Utilities/ElementTemplateHelper.cs`
- `src/Vixen.Core/Data/Value/FunctionIdentity.cs`
- `src/Vixen.Core/Execution/Context/LiveContext.cs`
- `src/Vixen.Core/Execution/ControllerUpdateAdjudicator.cs`
- `src/Vixen.Core/Extensions/Extensions.cs`
- `src/Vixen.Core/Intent/CommandIntent.cs`
- `src/Vixen.Core/Intent/RangeIntent.cs`
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
- `src/Vixen.Core/Sys/Managers/PreviewManager.cs`
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
- `src/Vixen.Modules/App/Fixture/FixtureChannel.cs`
- `src/Vixen.Modules/App/Fixture/FixtureFunction.cs`
- `src/Vixen.Modules/App/Fixture/FixtureIndexBase.cs`
- `src/Vixen.Modules/App/Fixture/FixtureItem.cs`
- `src/Vixen.Modules/App/Fixture/FixtureSpecification.cs`
- `src/Vixen.Modules/App/FixtureSpecificationManager/FixtureSpecificationManager.cs`
- `src/Vixen.Modules/App/FixtureSpecificationManager/IFixtureSpecificationManager.cs`
- `src/Vixen.Modules/App/LipSyncApp/LipSyncMapStaticData.cs`
- `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs`
- `src/Vixen.Modules/App/Modeling/ElementModeling.cs`
- `src/Vixen.Modules/App/TimedSequenceMapper/SequenceElementMapper/ViewModels/ElementMapperViewModel.cs`
- `src/Vixen.Modules/App/WebServer/Service/SystemHelper.cs`
- `src/Vixen.Modules/Editor/EffectEditor/Editors/BaseColorTypeEditor.cs`
- `src/Vixen.Modules/Editor/EffectEditor/Internal/Util.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/ChannelItemViewModel.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FixturePropertyEditorViewModel.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FixturePropertyEditorWindowViewModel.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionItemViewModel.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionTypeViewModel.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionTypeWindowViewModel.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/Views/FunctionTypeWindowView.xaml.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/Views/SelectFixtureSpecificationView.xaml.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/AutomationWizardPage.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/ColorSupportWizardPage.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/DimmingCurveWizardPage.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/EditProfileFunctionsWizardPage.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/EditProfileWizardPage.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/FixtureWizardPageBase.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/GroupingWizardPage.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/IntelligentFixtureWizard.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/SelectProfileWizardPage.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/SummaryWizardPageBase.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/AutomationWizardPageViewModel.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/ColorSupportWizardPageViewModel.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/DimmingCurveWizardPageViewModel.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/EditProfileFunctionsWizardPageViewModel.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/EditProfileWizardPageViewModel.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/SelectProfileWizardPageViewModel.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs`
- `src/Vixen.Modules/Effect/Alternating/Alternating.cs`
- `src/Vixen.Modules/Effect/Alternating/AlternatingMode.cs`
- `src/Vixen.Modules/Effect/AudioHelper/AudioPluginBase.cs`
- `src/Vixen.Modules/Effect/Dissolve/Dissolve.cs`
- `src/Vixen.Modules/Effect/Effect/BaseEffect.cs`
- `src/Vixen.Modules/Effect/Effect/FixtureEffectBase.cs`
- `src/Vixen.Modules/Effect/Effect/FixtureIndexEffectBase.cs`
- `src/Vixen.Modules/Effect/Effect/IDiscreteColorProvider.cs`
- `src/Vixen.Modules/Effect/Effect/PixelEffectBase.cs`
- `src/Vixen.Modules/Effect/Fire/Fire.cs`
- `src/Vixen.Modules/Effect/Fixture/FixtureEffect/Converters/FunctionCollectionNameConverter.cs`
- `src/Vixen.Modules/Effect/Fixture/FixtureEffect/Data/FixtureData.cs`
- `src/Vixen.Modules/Effect/Fixture/FixtureEffect/Data/FixtureFunctionData.cs`
- `src/Vixen.Modules/Effect/Fixture/FixtureEffect/FixtureFunctionExpando.cs`
- `src/Vixen.Modules/Effect/Fixture/FixtureEffect/FixtureFunctionExpandoCollection.cs`
- `src/Vixen.Modules/Effect/Fixture/FixtureEffect/FixtureModule.cs`
- `src/Vixen.Modules/Effect/Fixture/IFixtureFunctionExpando/IFixtureFunctionExpando.cs`
- `src/Vixen.Modules/Effect/FixtureStrobe/Converters/FixtureStrobeFunctionNameConverter.cs`
- `src/Vixen.Modules/Effect/FixtureStrobe/FixtureStrobeModule.cs`
- `src/Vixen.Modules/Effect/Frost/Converters/FrostFunctionNameConverter.cs`
- `src/Vixen.Modules/Effect/Frost/FrostModule.cs`
- `src/Vixen.Modules/Effect/Gobo/Converters/GoboFunctionNameConverter.cs`
- `src/Vixen.Modules/Effect/Gobo/GoboModule.cs`
- `src/Vixen.Modules/Effect/LineDance/FanCenterOptions.cs`
- `src/Vixen.Modules/Effect/LineDance/LineDanceModule.cs`
- `src/Vixen.Modules/Effect/Prism/Converters/PrismFunctionNameConverter.cs`
- `src/Vixen.Modules/Effect/Prism/PrismModule.cs`
- `src/Vixen.Modules/Effect/SetPosition/SetPositionModule.cs`
- `src/Vixen.Modules/Effect/SetZoom/SetZoomModule.cs`
- `src/Vixen.Modules/Effect/SpinColorWheel/Converters/SpinColorWheelFunctionNameConverter.cs`
- `src/Vixen.Modules/Effect/SpinColorWheel/SpinColorWheelModule.cs`
- `src/Vixen.Modules/Effect/State/State.cs`
- `src/Vixen.Modules/Effect/State/StateDefinitionDiscovery.cs`
- `src/Vixen.Modules/Effect/Twinkle/Twinkle.cs`
- `src/Vixen.Modules/Effect/Wipe/WipeDirection.cs`
- `src/Vixen.Modules/Effect/Wipe/WipeModule.cs`
- `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveHelper.Designer.cs`
- `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveHelper.cs`
- `src/Vixen.Modules/Preview/VixenPreview/PreviewCustomPropBuilder.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/MovingHeadIntentHandler.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCane.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomProp.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewEllipse.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLightBaseShape.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHead.cs`
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
- `src/Vixen.Modules/Property/IntelligentFixture/IntelligentFixtureData.cs`
- `src/Vixen.Modules/Property/IntelligentFixture/IntelligentFixtureDescriptor.cs`
- `src/Vixen.Modules/Property/IntelligentFixture/IntelligentFixtureModule.cs`
- `src/Vixen.Modules/Property/Location/LocationModule.cs`
- `src/Vixen.Modules/Property/Order/OrderDescriptor.cs`
- `src/Vixen.Modules/Property/Order/OrderModule.cs`
- `src/Vixen.Modules/Property/Order/OrderSetupHelper.cs`
- `src/Vixen.Modules/Property/Orientation/OrientationDescriptor.cs`
- `src/Vixen.Modules/Property/Orientation/OrientationModule.cs`
- `src/Vixen.Modules/Property/Orientation/OrientationSetupHelper.cs`
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
| `src/Vixen.Application/ConfigPreviews.cs` | sender, e, listViewPreviews_ItemCheck |
| `src/Vixen.Application/Setup/DisplaySetup.cs` | SelectElements, e, elements, ApplyConfirmedChanges, sender, ... |
| `src/Vixen.Application/Setup/ElementTemplates/ElementTemplateBase.cs` | GetElementsToDelete, GetLeafNodes |
| `src/Vixen.Application/Setup/ElementTemplates/Icicles.cs` | selectedNodes, selectedNodes, GenerateElements, SetupTemplate |
| `src/Vixen.Application/Setup/ElementTemplates/IntelligentFixtureTemplate.cs` | automaticallyOpenAndCloseShutter, fixtureSpecification, automaticallyOpenAndClosePrism, CreateColorProperty, dimmingCurveSelection, ... |
| `src/Vixen.Application/Setup/ElementTemplates/LipSync.cs` | GenerateElements, SetupTemplate, selectedNodes, selectedNodes |
| `src/Vixen.Application/Setup/ElementTemplates/Megatree.cs` | selectedNodes, SetupTemplate, GenerateElements, selectedNodes |
| `src/Vixen.Application/Setup/ElementTemplates/NumberedGroup.cs` | selectedNodes, GenerateElements, SetupTemplate, selectedNodes |
| `src/Vixen.Application/Setup/ElementTemplates/PixelGrid.cs` | PixelGrid_Load, PixelGrid_FormClosed, sender, GenerateElements, e, ... |
| `src/Vixen.Application/Setup/ElementTemplates/SingleItem.cs` | GetLeafNodes, GenerateElements, selectedNodes, SetupTemplate, SingleItem.<init>, ... |
| `src/Vixen.Application/Setup/ElementTemplates/StarBurst.cs` | selectedNodes, selectedNodes, GenerateElements, SetupTemplate |
| `src/Vixen.Application/Setup/ElementTemplates/StartLocation.cs` | BottomLeft, TopRight, BottomRight, TopLeft, StartLocation |
| `src/Vixen.Application/Setup/ElementsChangedEventArgs.cs` | ElementsChangedAction, action, ElementsChangedEventArgs.<init>, ElementsChangedEventArgs, Rename, ... |
| `src/Vixen.Application/Setup/ISetupElementsControl.cs` | nodes, ElementNodesEventArgs, UpdateScrollPosition, ElementNodesEventArgs.<init>, SelectedElements, ... |
| `src/Vixen.Application/Setup/ISetupPatchingControl.cs` | SetupPatchingControl, MasterForm, nodes, UpdateElementSelection, ISetupPatchingControl, ... |
| `src/Vixen.Application/Setup/SetupControllersSimple.cs` | sender, buttonSelectSourceElements_Click, e |
| `src/Vixen.Application/Setup/SetupElementsTree.Designer.cs` | components, buttonDeleteElements, toolTip1, buttonRemoveProperty, label3, ... |
| `src/Vixen.Application/Setup/SetupElementsTree.cs` | e, SelectedElements, SetupElementsControl, ConfigureSelectedProperties, ColumnAutoSize, ... |
| `src/Vixen.Application/Setup/SetupPatchingGraphical.cs` | nodes, node, _allNodeParentsAreInSet |
| `src/Vixen.Application/Setup/SetupPatchingSimple.Designer.cs` | labelUnconnectedPatchPointCount, radioButtonUnconnectedPatchPointsOnly, radioButtonUnpatchedOutputsOnly, label20, label3, ... |
| `src/Vixen.Application/Setup/SetupPatchingSimple.cs` | _updateControllerDetails, outputs, Item, buttonUnpatchControllers_Click, args, ... |
| `src/Vixen.Application/VixenApplication.cs` | sender, optionsToolStripMenuItem_Click, e |
| `src/Vixen.Common/Controls/ElementTree.Designer.cs` | pastePropertiesToolStripMenuItem, exportElementTreeToolStripMenuItem, deleteNodesToolStripMenuItem, toolStripSeparator3, ElementTree, ... |
| `src/Vixen.Common/Controls/ElementTree.cs` | sender, OnDragFinished, _topDisplayedNodes, tagMenuItem, RenameSelectedElements, ... |
| `src/Vixen.Common/Controls/MultiSelectTreeview.cs` | CanReverseElements |
| `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` | SelectedRowElementNodes, ToggleTagOnSelectedRows, tagMenuItem, cascadeToChildren, tagId |
| `src/Vixen.Common/Controls/XMLProfileSettings.cs` | xPath, newName, RenameNode, type |
| `src/Vixen.Common/DiscreteColorPicker/Views/SingleDiscreteColorPickerView.xaml.cs` | SingleDiscreteColorPickerView, GetSelectedColor |
| `src/Vixen.Common/ElementTagManager/ViewModels/ElementTagManagerWindowViewModel.cs` | SaveAsync |
| `src/Vixen.Common/ElementTagManager/ViewModels/TagColorItem.cs` | CommitColor |
| `src/Vixen.Common/Utilities/ElementTemplateHelper.cs` | CleanupTree, ProcessElementTemplate, template, addToTree, template, ... |
| `src/Vixen.Core/Data/Value/FunctionIdentity.cs` | Prism, Custom, OpenClosePrism, Gobo, SpinColorWheel, ... |
| `src/Vixen.Core/Execution/Context/LiveContext.cs` | TargetNodesComparer, y, x, TerminateNodes, obj, ... |
| `src/Vixen.Core/Execution/ControllerUpdateAdjudicator.cs` | PetitionForUpdate |
| `src/Vixen.Core/Extensions/Extensions.cs` | value, GetEnumDescription |
| `src/Vixen.Core/Intent/CommandIntent.cs` | CommandIntent, timeSpan, CommandIntent.<init>, command |
| `src/Vixen.Core/Intent/RangeIntent.cs` | RangeIntent |
| `src/Vixen.Core/Module/ModuleDataSet.cs` | RemoveDataModel, model |
| `src/Vixen.Core/Module/Property/IProperty.cs` | sourceProperty, Owner, CloneValues, IProperty |
| `src/Vixen.Core/Module/Property/IPropertyModuleInstance.cs` | nodes, SetupElements |
| `src/Vixen.Core/Module/Property/PropertyModuleInstanceBase.cs` | nodes, SetupElements |
| `src/Vixen.Core/Rule/IElementSetupHelper.cs` | Perform, IElementSetupHelper, selectedNodes, HelperName |
| `src/Vixen.Core/Rule/IElementTemplate.cs` | selectedNodes, TemplateName, GetLeafNodes, selectedNodes, IElementTemplate, ... |
| `src/Vixen.Core/Services/ApplicationServices.cs` | GetAllElementSetupHelpers |
| `src/Vixen.Core/Services/ElementNodeService.cs` | uniquifyName, ElementNodeService, templateFileName, elementNode, _CreateElement, ... |
| `src/Vixen.Core/Sys/DataNodeCollection.cs` | AddRange, values |
| `src/Vixen.Core/Sys/DataStream.cs` | AddData, data |
| `src/Vixen.Core/Sys/ElementNode.cs` | node, GetNodeEnumerator, name, OnChanged, name, ... |
| `src/Vixen.Core/Sys/Execution.cs` | UpdateState, NodesChanged, allowed |
| `src/Vixen.Core/Sys/Extensions.cs` | nodes, GetElements, AddRange, hashSet, values, ... |
| `src/Vixen.Core/Sys/GroupNode.cs` | cleanupIfFloating, parent, RemoveFromParent |
| `src/Vixen.Core/Sys/IElementNode.cs` | Element, Id, Tags, IsLeaf, GetNonLeafEnumerator, ... |
| `src/Vixen.Core/Sys/IGroupNode.cs` | IGroupNode |
| `src/Vixen.Core/Sys/Managers/NodeManager.cs` | GetElementNode, sender, id, node, ClearElementNode, ... |
| `src/Vixen.Core/Sys/Managers/PreviewManager.cs` | outputPreview, outputDevice, RemoveDataModel, Remove |
| `src/Vixen.Core/Sys/Modules.cs` | ClearRepositories |
| `src/Vixen.Core/Sys/Output/OutputController.cs` | GetDataFlowComponentForOutput, output |
| `src/Vixen.Core/Sys/Program.cs` | Program.<init>, original |
| `src/Vixen.Core/Sys/PropertyManager.cs` | PropertyManager, owner, GetEnumerator, _items, AddWithoutDefaults, ... |
| `src/Vixen.Core/Sys/ProxyElementNode.cs` | ProxyElementNode.<init>, Properties, GetNonLeafEnumerator, ProxyElementNode, IsLeaf, ... |
| `src/Vixen.Core/Sys/State/Execution/Behavior/StandardOpeningBehavior.cs` | Run |
| `src/Vixen.Core/Sys/SystemConfig.cs` | _filters, FileName, _elements, _disabledDevicesIds, IsPreviewThreaded, ... |
| `src/Vixen.Core/Sys/TimeNode.cs` | right, left, Intersect |
| `src/Vixen.Core/Sys/VixenSystem.cs` | UIThread, _LoadSystemConfig, Stop, State, LoadSystemConfig, ... |
| `src/Vixen.Core/Utility/NamingUtilities.cs` | NamingUtilities, name, names, Uniquify |
| `src/Vixen.Modules/App/Fixture/FixtureChannel.cs` | ChannelNumber, FixtureChannel, CreateInstanceForClone, Function |
| `src/Vixen.Modules/App/Fixture/FixtureFunction.cs` | RotationLimits, ZoomType, NarrowToWide, None, GetIndexDataBase, ... |
| `src/Vixen.Modules/App/Fixture/FixtureIndexBase.cs` | UseCurve, FixtureIndexBase.<init>, IndexType, EndValue, StartValue, ... |
| `src/Vixen.Modules/App/Fixture/FixtureItem.cs` | Name, FixtureItem |
| `src/Vixen.Modules/App/Fixture/FixtureSpecification.cs` | CreateInstanceForClone, FixtureSpecification.<init>, IsFunctionUsed, GetInUseFunction, IsRGBW, ... |
| `src/Vixen.Modules/App/FixtureSpecificationManager/FixtureSpecificationManager.cs` | FixtureSpecifications |
| `src/Vixen.Modules/App/FixtureSpecificationManager/IFixtureSpecificationManager.cs` | FixtureSpecifications |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncMapStaticData.cs` | MigrateMaps |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs` | SelectedElementNodes |
| `src/Vixen.Modules/App/Modeling/ElementModeling.cs` | leafNodes, TopLevelNode, OrderNodes |
| `src/Vixen.Modules/App/TimedSequenceMapper/SequenceElementMapper/ViewModels/ElementMapperViewModel.cs` | Elements |
| `src/Vixen.Modules/App/WebServer/Service/SystemHelper.cs` | id, Save, on, SetControllerState |
| `src/Vixen.Modules/Editor/EffectEditor/Editors/BaseColorTypeEditor.cs` | editedType, inlineTemplate, component, GetDiscreteColors, BaseColorTypeEditor.<init>, ... |
| `src/Vixen.Modules/Editor/EffectEditor/Internal/Util.cs` | component, GetDiscreteColors, Util |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/ChannelItemViewModel.cs` | FunctionObjects, ChannelItemViewModel.<init>, functionObjects, functions |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FixturePropertyEditorViewModel.cs` | channelItem, EditFunctions, UpdateFunctionNames, functionData |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FixturePropertyEditorWindowViewModel.cs` | FixtureSpecification |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionItemViewModel.cs` | FunctionIdentities, FunctionItemViewModel.<init>, RGBColor, ConvertFixtureFunctionType, FunctionTypeEnum, ... |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionTypeViewModel.cs` | name, CreateFunctionType, functionType, functionTypeVM, GetFunctionData, ... |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionTypeWindowViewModel.cs` | Functions, FunctionsProperty, FunctionTypeWindowViewModel, GetChildViewModel, UpdatedFunctions, ... |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/Views/FunctionTypeWindowView.xaml.cs` | Initialize, FunctionTypeWindowView, sender, _functions, Hyperlink_RequestNavigate, ... |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/Views/SelectFixtureSpecificationView.xaml.cs` | Initialize, SelectFixtureSpecificationView.<init>, fixtures |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/AutomationWizardPage.cs` | AutomaticallyOpenAndClosePrism, AutomationWizardPage, AutomationWizardPage.<init>, AutomaticallyControlDimmer, GetSummaryString, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/ColorSupportWizardPage.cs` | ColorSupportWizardPage, ColorSupportWizardPage.<init>, ColorMixing, ColorWheel, NoColorSupport, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/DimmingCurveWizardPage.cs` | DimmingCurveWizardPage, OneDimmingCurvePerFixture, NoDimmingCurve, DimmingCurveWizardPage.<init>, OneDimmingCurvePerColor, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/EditProfileFunctionsWizardPage.cs` | EditProfileFunctionsWizardPage, EditProfileFunctionsWizardPage.<init> |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/EditProfileWizardPage.cs` | EditProfileWizardPage.<init>, EditProfileWizardPage |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/FixtureWizardPageBase.cs` | HelpURL, helpURL, FixtureWizardPageBase.<init>, FixtureWizardPageBase |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/GroupingWizardPage.cs` | GroupName, ElementPrefix, GetSummaryString, NumberOfFixtures, GroupingWizardPage.<init>, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/IntelligentFixtureWizard.cs` | IntelligentFixtureWizard.<init>, messageService, typeFactory |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/SelectProfileWizardPage.cs` | CreatedBy, CreateNewProfile, GetSummaryString, SelectProfileWizardPage.<init>, SelectedFixture, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/SummaryWizardPageBase.cs` | helpURL, GetSummaryString, GetSummary, SummaryWizardPageBase.<init>, SummaryWizardPageBase |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/AutomationWizardPageViewModel.cs` | EnableShutter, AutomaticallyOpenAndCloseShutter, EnableDimmer, AutomationWizardPageViewModel, AutomaticallyControlColorWheel, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/ColorSupportWizardPageViewModel.cs` | ColorMixing, ColorSupportWizardPageViewModel.<init>, NoColorSupport, ColorSupportWizardPageViewModel, ColorWheel, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/DimmingCurveWizardPageViewModel.cs` | EnableDimmingCurvePerColor, DimmingCurveWizardPageViewModel, DimmingCurveWizardPageViewModel.<init>, OneDimmingCurvePerFixture, EditDimmingCurveCommand, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/EditProfileFunctionsWizardPageViewModel.cs` | FunctionsProperty, EditProfileFunctionsWizardPageViewModel.<init>, EditProfileFunctionsWizardPageViewModel, wizardPage, Functions |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/EditProfileWizardPageViewModel.cs` | wizardPage, FixtureSpecification, EditProfileWizardPageViewModel.<init> |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/SelectProfileWizardPageViewModel.cs` | Fixture |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` | e, sender, ElementChangedRowsHandler |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs` | editMapsToolStripMenuItem_Click, e, sender, e, changeMapToolStripMenuItem_Click, ... |
| `src/Vixen.Modules/Effect/Alternating/Alternating.cs` | RenderNode, _PreRender, cancellationToken, GetNodesToRenderOn, nodes |
| `src/Vixen.Modules/Effect/Alternating/AlternatingMode.cs` | MarkCollection, TimeInterval, AlternatingMode |
| `src/Vixen.Modules/Effect/AudioHelper/AudioPluginBase.cs` | GetNodesToRenderOn |
| `src/Vixen.Modules/Effect/Dissolve/Dissolve.cs` | cancellationToken, GetNodesToRenderOn, ElementIndex, ColorIndex, TempClass, ... |
| `src/Vixen.Modules/Effect/Effect/BaseEffect.cs` | DistanceFromPoint, origin, point |
| `src/Vixen.Modules/Effect/Effect/FixtureEffectBase.cs` | drawStringYOffset, nodes, InformationLink, tags, cancellationToken, ... |
| `src/Vixen.Modules/Effect/Effect/FixtureIndexEffectBase.cs` | functionName, functionIdentity, indexFunctionName, indexvalue, cancellationToken, ... |
| `src/Vixen.Modules/Effect/Effect/IDiscreteColorProvider.cs` | IDiscreteColorProvider, GetDiscreteColors |
| `src/Vixen.Modules/Effect/Effect/PixelEffectBase.cs` | FindLeafParents, FindLeafParents, GetRenderGroups, targetNodes |
| `src/Vixen.Modules/Effect/Fire/Fire.cs` | GetRenderGroups |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/Converters/FunctionCollectionNameConverter.cs` | GetStandardValuesInternal, FunctionCollectionNameConverter, fixtureEffect |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/Data/FixtureData.cs` | FixtureData.<init>, FixtureData, FunctionData, CreateInstanceForClone, FixtureFunctions |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/Data/FixtureFunctionData.cs` | Range, c, ColorIndexValue, CreateInstanceForClone, FixtureFunctionData.<init>, ... |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/FixtureFunctionExpando.cs` | FunctionName, FunctionType, FixtureFunctionExpando, _colorIndexValue, IndexData, ... |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/FixtureFunctionExpandoCollection.cs` | FixtureFunctionExpandoCollection, FixtureFunctionExpandoCollection.<init> |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/FixtureModule.cs` | FixtureFunctions, RenderCurve, UpdateFixtureCapabilities, intensity, GetRenderNodesForRGBFunctionType, ... |
| `src/Vixen.Modules/Effect/Fixture/IFixtureFunctionExpando/IFixtureFunctionExpando.cs` | TimelineColor, FunctionName, FixtureFunctions, Intensity, ColorIndexValue, ... |
| `src/Vixen.Modules/Effect/FixtureStrobe/Converters/FixtureStrobeFunctionNameConverter.cs` | effect, GetStandardValuesInternal |
| `src/Vixen.Modules/Effect/FixtureStrobe/FixtureStrobeModule.cs` | PreRenderInternal, UpdateFixtureCapabilities, GetCompatibleIndexValues, function, cancellationToken |
| `src/Vixen.Modules/Effect/Frost/Converters/FrostFunctionNameConverter.cs` | effect, GetStandardValuesInternal |
| `src/Vixen.Modules/Effect/Frost/FrostModule.cs` | cancellationToken, PreRenderInternal, UpdateAttributes, refresh, FrostModule, ... |
| `src/Vixen.Modules/Effect/Gobo/Converters/GoboFunctionNameConverter.cs` | GetStandardValuesInternal, effect |
| `src/Vixen.Modules/Effect/Gobo/GoboModule.cs` | cancellationToken, UpdateFixtureCapabilities, GetGoboFunctionNames, PreRenderInternal |
| `src/Vixen.Modules/Effect/LineDance/FanCenterOptions.cs` | Left, FanCenterOptions, Right, Centered |
| `src/Vixen.Modules/Effect/LineDance/LineDanceModule.cs` | CalculatePanIncrement, leftMiddleIndex, rightMiddleIndex, renderNodes, displayCenter, ... |
| `src/Vixen.Modules/Effect/Prism/Converters/PrismFunctionNameConverter.cs` | effect, GetStandardValuesInternal |
| `src/Vixen.Modules/Effect/Prism/PrismModule.cs` | cancellationToken, PreRenderInternal, UpdateFixtureCapabilities |
| `src/Vixen.Modules/Effect/SetPosition/SetPositionModule.cs` | SetPositionModule.<init>, PreRenderInternal, UpdateFixtureCapabilities, cancellationToken |
| `src/Vixen.Modules/Effect/SetZoom/SetZoomModule.cs` | SetZoomModule, UpdateAttributes, refresh, _zoomTags, UpdateFixtureCapabilities, ... |
| `src/Vixen.Modules/Effect/SpinColorWheel/Converters/SpinColorWheelFunctionNameConverter.cs` | effect, GetStandardValuesInternal |
| `src/Vixen.Modules/Effect/SpinColorWheel/SpinColorWheelModule.cs` | graphics, cancellationToken, GenerateVisualRepresentation, GetCompatibleIndexValues, PreRenderInternal, ... |
| `src/Vixen.Modules/Effect/State/State.cs` | node, stateItemColor, ResolveStateItemColor, GetTargetScopeNodes, leafNode, ... |
| `src/Vixen.Modules/Effect/State/StateDefinitionDiscovery.cs` | node, Traverse |
| `src/Vixen.Modules/Effect/Twinkle/Twinkle.cs` | GetNodesToRenderOn |
| `src/Vixen.Modules/Effect/Wipe/WipeDirection.cs` | DiagonalUp, DiagonalDown, Horizontal, Dimaond, WipeDirection, ... |
| `src/Vixen.Modules/Effect/Wipe/WipeModule.cs` | RenderWipeForTargets, tokenSource, renderedNodes, GetTargetRenderGroups, tokenSource, ... |
| `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveHelper.Designer.cs` | Dispose, label4, lblQuestion, label1, buttonCancel, ... |
| `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveHelper.cs` | buttonSetupCurve_Click, DoNothing, DimmingCurveHelper.<init>, ExistingBehaviour, AddInsertAfterElement, ... |
| `src/Vixen.Modules/Preview/VixenPreview/PreviewCustomPropBuilder.cs` | GetStatePropertyId, _tokenLookup, elementModel, ShowDimmingCurveMessage, node, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/MovingHeadIntentHandler.cs` | rangeIntent, rangeIntent, Handle, UpdateLegend |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCane.cs` | node, Reconfigure |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomProp.cs` | AddLightNodes, _rotationCenter, matchShape, aspect, PreviewCustomProp, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewEllipse.cs` | node, Reconfigure |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLightBaseShape.cs` | AddPixels, node, lightCount |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHead.cs` | selectedNode, InitializeMovingHeadMovementConstraints, node, Reconfigure |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewRectangle.cs` | Reconfigure, node |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTools.cs` | node, GetParentNodes, node, GetLeafNodes |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs` | e, sender, saveToolStripMenuItem_Click |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupElementsDocument.cs` | node, AddNodeToTree, SetupTemplate, sender, ButtonAddTemplate_Click, ... |
| `src/Vixen.Modules/Property/Color/ColorDescriptor.cs` | _typeId, ModuleStaticDataClass, TypeId, Description, ModuleDataClass, ... |
| `src/Vixen.Modules/Property/Color/ColorPanel.Designer.cs` | Dispose, disposing |
| `src/Vixen.Modules/Property/Color/ColorProperty.cs` | Logging, ColorType, FullColor, SingleColor, ColorSetChangedHandler, ... |
| `src/Vixen.Modules/Property/Color/ColorSetupForm.cs` | sender, sender, ColorSetupForm_Load, AnyRadioButtonCheckedChanged, e, ... |
| `src/Vixen.Modules/Property/Color/ColorSetupHelper.Designer.cs` | buttonColorSetsSetup, Dispose, label3, radioButtonOptionFullColor, comboBoxColorOrder, ... |
| `src/Vixen.Modules/Property/Color/ColorSetupHelper.cs` | _colorSetNameSelectedItem, colorType, Green, Perform, Red, ... |
| `src/Vixen.Modules/Property/Face/FaceComponent.cs` | EyesClosed, EyesOpen, Mouth, Outlines, FaceComponent |
| `src/Vixen.Modules/Property/Face/FaceDescriptor.cs` | Id, ModuleId, Author, Version, ModuleClass, ... |
| `src/Vixen.Modules/Property/Face/FaceMapItem.cs` | ElementGuid, FaceMapItem, FaceComponents, OnDeserialized, PhonemeList, ... |
| `src/Vixen.Modules/Property/Face/FaceModule.cs` | FaceModule.<init>, CloneValues, ConfiguredIntensity, SetupElements, GetFaceModuleForElement, ... |
| `src/Vixen.Modules/Property/Face/FaceSetupHelper.Designer.cs` | dataGridViewOther, tabMouth, disposing, components, buttonOK, ... |
| `src/Vixen.Modules/Property/Face/FaceSetupHelper.cs` | Perform, BuildMouthDialogFromMap, elementName, _mouthDataTable, dataGridView_CellDoubleClick, ... |
| `src/Vixen.Modules/Property/IntelligentFixture/IntelligentFixtureData.cs` | IntelligentFixtureData.<init> |
| `src/Vixen.Modules/Property/IntelligentFixture/IntelligentFixtureDescriptor.cs` | ModuleClass, Description, ModuleDataClass, _typeId, Version, ... |
| `src/Vixen.Modules/Property/IntelligentFixture/IntelligentFixtureModule.cs` | HasSetup, FixtureSpecification, _data, IntelligentFixtureModule |
| `src/Vixen.Modules/Property/Location/LocationModule.cs` | CloneValues, sourceProperty |
| `src/Vixen.Modules/Property/Order/OrderDescriptor.cs` | Id, Author, Description, TypeId, ModuleClass, ... |
| `src/Vixen.Modules/Property/Order/OrderModule.cs` | AddPatchingOrder, sourceProperty, _data, nodes, AddPatchingOrder, ... |
| `src/Vixen.Modules/Property/Order/OrderSetupHelper.cs` | Perform, PopulateElementList, selectedNodes, selectedNodes |
| `src/Vixen.Modules/Property/Orientation/OrientationDescriptor.cs` | _typeId, ModuleId, TypeName, Version, Author, ... |
| `src/Vixen.Modules/Property/Orientation/OrientationModule.cs` | OrientationModule, OrientationModule.<init>, sourceProperty, GetOrientationForElement, HasSetup, ... |
| `src/Vixen.Modules/Property/Orientation/OrientationSetupHelper.cs` | Perform, OrientationSetupHelper, HelperName, selectedNodes |
| `src/Vixen.Modules/Property/State/Setup/Models/StateDefinition.cs` | Element |
| `src/Vixen.Modules/Property/State/Setup/Services/IStateMapperDialogService.cs` | IStateMapperDialogService |
| `src/Vixen.Modules/Property/State/Setup/Services/StateColorPickerService.cs` | nodes, WaitForInitiatingMouseInputAsync, initialColor, initialColor, StateColorPickerService, ... |
| `src/Vixen.Modules/Property/State/Setup/Services/StateMapperDialogService.cs` | StateMapperDialogService |
| `src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs` | color, rootNode, TryGetFirstCommonDiscreteColor |
| `src/Vixen.Modules/Property/State/StateDescriptor.cs` | Author, TypeName, Description, StateDescriptor, TypeId, ... |
| `src/Vixen.Modules/Property/State/StateModule.cs` | SetupElements, HasElementSetupHelper, Logging, element, GetStateModuleForElement, ... |
| `src/Vixen.Modules/Property/State/StateSetupHelper.cs` | data, StateSetupHelper.<init>, _stateModuleFactory, ShowMapper, HelperName, ... |
| `src/Vixen.Tests/Core/OutputControllerOutputIndexTests.cs` | Dispose, propertyName, value, SetVixenSystemProperty, OutputControllerOutputIndexTests.<init> |
| `src/Vixen.Tests/Preview/VixenPreview/PreviewCustomPropStateImportTests.cs` | PreviewCustomPropStateImportTests.<init>, propertyName, SetVixenSystemProperty, value |
| `src/Vixen.Tests/Property/State/StateMapperDefinitionTests.cs` | name, colors, children, CreateNode, owner, ... |
| `src/Vixen.Tests/Property/State/StateSetupHelperTests.cs` | property, Data, CreateStateModule, Perform_ExistingPropertyAccepted_MutatesExistingDataInstance, Perform_ExistingPropertyCancelled_PreservesExistingData, ... |
| `src/Vixen.Tests/Setup/SetupPatchingSimpleOutputOrderTests.cs` | SetVixenSystemProperty, propertyName, value, SetupPatchingSimpleOutputOrderTests.<init>, Dispose |
| `src/Vixen.Tests/Utility/NamingUtilitiesTests.cs` | Uniquify_Name2AlsoInSet_ReturnsNameWithSuffix3, Uniquify_NameAlreadyInSet_ReturnsNameWithSuffix2, Uniquify_NameNotInSet_ReturnsNameUnchanged, NamingUtilitiesTests |

## Connected Communities

- **VixenPreview/Shapes +22 dirs** (56 cross-edges)
- **Effect/Fireworks +9 dirs** (31 cross-edges)
- **Effect/Effect +104 dirs** (20 cross-edges)
- **Property/Color +17 dirs** (13 cross-edges)
- **Vixen.Common/NShape +70 dirs** (6 cross-edges)
- **Vixen.Core/Sys · GroupNode** (5 cross-edges)
- **Module/Effect +13 dirs** (5 cross-edges)
- **Data/Flow +8 dirs** (5 cross-edges)
- **Vixen.Common/Controls +5 dirs** (5 cross-edges)
- **Editor/TimedSequenceEditor +44 dirs** (4 cross-edges)
- **Vixen.Application +6 dirs** (4 cross-edges)
- **Vixen.Core/Sys +14 dirs** (3 cross-edges)
- **Vixen.Common/Controls +50 dirs** (3 cross-edges)
- **Vixen.Core/Sys +7 dirs** (3 cross-edges)
- **Vixen.Application/Setup +25 dirs** (2 cross-edges)
- **Sys/Output +43 dirs** (2 cross-edges)
- **Vixen.Core/Sys +4 dirs · SetValue** (2 cross-edges)
- **Sys/Managers +6 dirs** (2 cross-edges)
- **Module/Property +9 dirs** (2 cross-edges)
- **Vixen.Common/Controls · FindControllerNode** (2 cross-edges)
- **Vixen.Modules · StateRenderInterval** (2 cross-edges)
- **Sys/Managers +1 dirs** (2 cross-edges)
- **Effect/Text +3 dirs** (2 cross-edges)
- **Curves/ZedGraph +7 dirs** (2 cross-edges)
- **Vixen.Application/Setup +6 dirs** (2 cross-edges)
- **App/Curves +10 dirs** (2 cross-edges)
- **FixturePropertyEditor/ViewModels +7 dirs** (2 cross-edges)
- **App/Curves · GenerateGenericCurveImage** (1 cross-edges)
- **Vixen.Core/Sys · WindowsMultimedia** (1 cross-edges)
- **Vixen.Common/NShape +4 dirs** (1 cross-edges)
- **Sys/Managers · OrderedDeviceDictionary** (1 cross-edges)
- **WPFCommon/Services +6 dirs** (1 cross-edges)
- **Vixen.Application +5 dirs** (1 cross-edges)
- **VixenPreview/Shapes · MouseMove** (1 cross-edges)
- **Cache/Sequence +4 dirs** (1 cross-edges)
- **Vixen.Modules · FixtureSpecificationManager** (1 cross-edges)
- **Vixen.Common/Controls +2 dirs** (1 cross-edges)
- **Sys/Managers · GetComponent** (1 cross-edges)
- **FixturePropertyEditor/ViewModels · FixturePropertyWindowViewModelB…** (1 cross-edges)
- **Vixen.Core · Modules** (1 cross-edges)
- **Vixen.Core/Sys +3 dirs · Execution** (1 cross-edges)
- **Vixen.Core · WriteContentToObject** (1 cross-edges)
- **CustomPropEditor/Model +7 dirs** (1 cross-edges)
- **Vixen.Modules · CreateShapes** (1 cross-edges)
- **Vixen.Core/Sys +1 dirs · BuiltInElementTags** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-645")
explore(operation:"context", task:"understand Vixen.Core/Sys +77 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
