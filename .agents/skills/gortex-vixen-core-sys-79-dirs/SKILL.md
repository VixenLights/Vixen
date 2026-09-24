---
name: gortex-vixen-core-sys-79-dirs
description: "Work in the Vixen.Core/Sys +79 dirs area — 2287 symbols across 207 files (81% cohesion)"
---

# Vixen.Core/Sys +79 dirs

2287 symbols | 207 files | 81% cohesion

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
- `src/Vixen.Application/Setup/ISetupControllersControl.cs`
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
- `src/Vixen.Core/Instrumentation/IInstrumentation.cs`
- `src/Vixen.Core/Instrumentation/MillisecondsValue.cs`
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
- `src/Vixen.Core/Services/FileService.cs`
- `src/Vixen.Core/Services/IFileService.cs`
- `src/Vixen.Core/Sys/ElementNode.cs`
- `src/Vixen.Core/Sys/Execution.cs`
- `src/Vixen.Core/Sys/Extensions.cs`
- `src/Vixen.Core/Sys/GroupNode.cs`
- `src/Vixen.Core/Sys/IElementNode.cs`
- `src/Vixen.Core/Sys/IGroupNode.cs`
- `src/Vixen.Core/Sys/Instrumentation/Instrumentation.cs`
- `src/Vixen.Core/Sys/Managers/ContextManager.cs`
- `src/Vixen.Core/Sys/Managers/ElementManager.cs`
- `src/Vixen.Core/Sys/Managers/FilterManager.cs`
- `src/Vixen.Core/Sys/Managers/HardwareUpdateThread.cs`
- `src/Vixen.Core/Sys/Managers/NodeManager.cs`
- `src/Vixen.Core/Sys/Managers/PreviewManager.cs`
- `src/Vixen.Core/Sys/Modules.cs`
- `src/Vixen.Core/Sys/Output/OutputController.cs`
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
- `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/ItemsViewModel.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/SelectFixtureSpecificationViewModel.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/Views/FixturePropertyEditorView.xaml.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/Views/FixturePropertyEditorWindowView.xaml.cs`
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
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/EditWizardPageViewModelBase.cs`
- `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/SelectProfileWizardPageViewModel.cs`
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
- `src/Vixen.Modules/Preview/VixenPreview/OpenGL/OpenGLPreviewForm.cs`
- `src/Vixen.Modules/Preview/VixenPreview/PreviewCustomPropBuilder.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/MovingHeadIntentHandler.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCane.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomProp.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewEllipse.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLightBaseShape.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHead.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewRectangle.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTools.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewModuleInstance.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs`
- `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupElementsDocument.cs`
- `src/Vixen.Modules/Property/Color/ColorData.cs`
- `src/Vixen.Modules/Property/Color/ColorDescriptor.cs`
- `src/Vixen.Modules/Property/Color/ColorPanel.Designer.cs`
- `src/Vixen.Modules/Property/Color/ColorProperty.cs`
- `src/Vixen.Modules/Property/Color/ColorSetsSetupForm.cs`
- `src/Vixen.Modules/Property/Color/ColorSetupForm.Designer.cs`
- `src/Vixen.Modules/Property/Color/ColorSetupForm.cs`
- `src/Vixen.Modules/Property/Color/ColorSetupHelper.Designer.cs`
- `src/Vixen.Modules/Property/Color/ColorSetupHelper.cs`
- `src/Vixen.Modules/Property/Color/ColorStaticData.cs`
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
- `src/Vixen.Modules/Property/Order/OrderSetupHelper.Designer.cs`
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
- `src/Vixen.Tests/Preview/VixenPreview/PreviewCustomPropStateImportTests.cs`
- `src/Vixen.Tests/Property/State/StateMapperDefinitionTests.cs`
- `src/Vixen.Tests/Property/State/StateSetupHelperTests.cs`
- `src/Vixen.Tests/Utility/NamingUtilitiesTests.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Application/ConfigPreviews.cs` | e, sender, listViewPreviews_ItemCheck |
| `src/Vixen.Application/Setup/DisplaySetup.cs` | control_ElementsChanged, controllersAndOutputs, sender, ApplyConfirmedChanges, updateScrollPosition, ... |
| `src/Vixen.Application/Setup/ElementTemplates/ElementTemplateBase.cs` | GetElementsToDelete, GetLeafNodes |
| `src/Vixen.Application/Setup/ElementTemplates/Icicles.cs` | GenerateElements, selectedNodes, selectedNodes, SetupTemplate |
| `src/Vixen.Application/Setup/ElementTemplates/IntelligentFixtureTemplate.cs` | dimmingCurveSelection, selectedNodes, node, GetLeafNodes, CreateFixtureNode, ... |
| `src/Vixen.Application/Setup/ElementTemplates/LipSync.cs` | selectedNodes, SetupTemplate, selectedNodes, GenerateElements |
| `src/Vixen.Application/Setup/ElementTemplates/Megatree.cs` | GenerateElements, SetupTemplate, selectedNodes, selectedNodes |
| `src/Vixen.Application/Setup/ElementTemplates/NumberedGroup.cs` | selectedNodes, GenerateElements, SetupTemplate, selectedNodes |
| `src/Vixen.Application/Setup/ElementTemplates/PixelGrid.cs` | SetupTemplate, sender, GenerateElements, e, PixelGrid_Load, ... |
| `src/Vixen.Application/Setup/ElementTemplates/SingleItem.cs` | selectedNodes, TemplateName, selectedNodes, SingleItem, SingleItem.<init>, ... |
| `src/Vixen.Application/Setup/ElementTemplates/StarBurst.cs` | SetupTemplate, GenerateElements, selectedNodes, selectedNodes |
| `src/Vixen.Application/Setup/ElementTemplates/StartLocation.cs` | BottomRight, BottomLeft, TopLeft, StartLocation, TopRight |
| `src/Vixen.Application/Setup/ElementsChangedEventArgs.cs` | Edit, Action, ElementsChangedEventArgs, ElementsChangedAction, Remove, ... |
| `src/Vixen.Application/Setup/ISetupControllersControl.cs` | ControllersAndOutputsSet |
| `src/Vixen.Application/Setup/ISetupElementsControl.cs` | SelectedElements, ElementNodesEventArgs.<init>, UpdateScrollPosition, nodes, ElementNodes, ... |
| `src/Vixen.Application/Setup/ISetupPatchingControl.cs` | ISetupPatchingControl, UpdateControllerSelection, MasterForm, UpdateElementSelection, controllersAndOutputs, ... |
| `src/Vixen.Application/Setup/SetupControllersSimple.cs` | e, sender, buttonSelectSourceElements_Click, ReorderControllers |
| `src/Vixen.Application/Setup/SetupElementsTree.Designer.cs` | label3, buttonSelectDestinationOutputs, buttonAddProperty, columnHeader2, buttonRunHelperSetup, ... |
| `src/Vixen.Application/Setup/SetupElementsTree.cs` | sender, sender, buttonRemoveProperty_Click, buttonRunSetupHelper_Click, sender, ... |
| `src/Vixen.Application/Setup/SetupPatchingGraphical.cs` | controllersAndOutputs, controllersAndOutputs, _allNodeParentsAreInSet, node, nodes |
| `src/Vixen.Application/Setup/SetupPatchingSimple.Designer.cs` | labelFirstOutput, label15, labelPatchedOutputCount, checkBoxReverseOutputOrder, label9, ... |
| `src/Vixen.Application/Setup/SetupPatchingSimple.cs` | selectedNodes, _cachedControllersAndOutputs, OnFiltersAdded, filterCount, T, ... |
| `src/Vixen.Application/VixenApplication.cs` | sender, optionsToolStripMenuItem_Click, e |
| `src/Vixen.Common/Controls/ElementTree.Designer.cs` | cutNodesToolStripMenuItem, exportWireDiagramToolStripMenuItem, frontWireDiagramToolStripMenuItem, treeview, collapseAllToolStripMenuItem, ... |
| `src/Vixen.Common/Controls/ElementTree.cs` | sender, CanPaste, SaveTreeNodeTopVisible, addChildren, GenerateEquivalentTreeNodeFullPathFromElement, ... |
| `src/Vixen.Common/Controls/MultiSelectTreeview.cs` | CanReverseElements |
| `src/Vixen.Common/Controls/TimeLineControl/TimelineControl.cs` | tagMenuItem, cascadeToChildren, SelectedRowElementNodes, tagId, ToggleTagOnSelectedRows |
| `src/Vixen.Common/Controls/XMLProfileSettings.cs` | newName, xPath, type, RenameNode |
| `src/Vixen.Common/DiscreteColorPicker/Views/SingleDiscreteColorPickerView.xaml.cs` | SingleDiscreteColorPickerView, GetSelectedColor |
| `src/Vixen.Common/ElementTagManager/ViewModels/ElementTagManagerWindowViewModel.cs` | SaveAsync |
| `src/Vixen.Common/ElementTagManager/ViewModels/TagColorItem.cs` | CommitColor |
| `src/Vixen.Common/Utilities/ElementTemplateHelper.cs` | owner, createdElements, treeElements, ProcessElementTemplate, template, ... |
| `src/Vixen.Core/Data/Value/FunctionIdentity.cs` | Gobo, Shutter, FunctionIdentity, OpenClosePrism, SpinColorWheel, ... |
| `src/Vixen.Core/Execution/Context/LiveContext.cs` | targetNodes, x, obj, TerminateNodes, GetHashCode, ... |
| `src/Vixen.Core/Execution/ControllerUpdateAdjudicator.cs` | PetitionForUpdate |
| `src/Vixen.Core/Extensions/Extensions.cs` | GetEnumDescription, value |
| `src/Vixen.Core/Instrumentation/IInstrumentation.cs` | value, AddValue |
| `src/Vixen.Core/Instrumentation/MillisecondsValue.cs` | tot, cnt, MillisecondsValue, _GetFormattedValue, name, ... |
| `src/Vixen.Core/Intent/CommandIntent.cs` | CommandIntent, CommandIntent.<init>, timeSpan, command |
| `src/Vixen.Core/Intent/RangeIntent.cs` | RangeIntent |
| `src/Vixen.Core/Module/ModuleDataSet.cs` | model, RemoveDataModel |
| `src/Vixen.Core/Module/Property/IProperty.cs` | Owner, CloneValues, sourceProperty, IProperty |
| `src/Vixen.Core/Module/Property/IPropertyModuleInstance.cs` | SetupElements, nodes |
| `src/Vixen.Core/Module/Property/PropertyModuleInstanceBase.cs` | nodes, SetupElements |
| `src/Vixen.Core/Rule/IElementSetupHelper.cs` | selectedNodes, Perform, HelperName, IElementSetupHelper |
| `src/Vixen.Core/Rule/IElementTemplate.cs` | selectedNodes, ConfigureDimming, ConfigureColor, GetLeafNodes, GenerateElements, ... |
| `src/Vixen.Core/Services/ApplicationServices.cs` | GetAllElementSetupHelpers |
| `src/Vixen.Core/Services/ElementNodeService.cs` | ElementNodeService, templateFileName, parentNode, _CreateElement, name, ... |
| `src/Vixen.Core/Services/FileService.cs` | systemConfig, SaveSystemConfigFile |
| `src/Vixen.Core/Services/IFileService.cs` | systemConfig, SaveSystemConfigFile, filePath, SaveSystemConfigFile, systemConfig |
| `src/Vixen.Core/Sys/ElementNode.cs` | Logging, GetLeafEnumerator, childName, ElementNode, ElementNode.<init>, ... |
| `src/Vixen.Core/Sys/Execution.cs` | UpdateState, allowed, initInstrumentation, NodesChanged |
| `src/Vixen.Core/Sys/Extensions.cs` | values, T, hashSet, nodes, GetElements, ... |
| `src/Vixen.Core/Sys/GroupNode.cs` | cleanupIfFloating, RemoveFromParent, parent |
| `src/Vixen.Core/Sys/IElementNode.cs` | Id, Parents, GetLeafEnumerator, Name, Element, ... |
| `src/Vixen.Core/Sys/IGroupNode.cs` | IGroupNode |
| `src/Vixen.Core/Sys/Instrumentation/Instrumentation.cs` | AddValue, value |
| `src/Vixen.Core/Sys/Managers/ContextManager.cs` | ContextManager.<init>, _contextUpdateTimeValue |
| `src/Vixen.Core/Sys/Managers/ElementManager.cs` | ElementManager.<init>, _elementUpdateTimeValue |
| `src/Vixen.Core/Sys/Managers/FilterManager.cs` | _filterUpdateTimeValue |
| `src/Vixen.Core/Sys/Managers/HardwareUpdateThread.cs` | _CreatePerformanceValues |
| `src/Vixen.Core/Sys/Managers/NodeManager.cs` | GetRootNodes, name, node, GetElementNode, OnNodesChanged, ... |
| `src/Vixen.Core/Sys/Managers/PreviewManager.cs` | RemoveDataModel, Remove, outputPreview, outputDevice |
| `src/Vixen.Core/Sys/Modules.cs` | ClearRepositories |
| `src/Vixen.Core/Sys/Output/OutputController.cs` | GetDataFlowComponentForOutput, output, CreatePerformanceValues |
| `src/Vixen.Core/Sys/PropertyManager.cs` | owner, PropertyManager, GetEnumerator, _typeIds, id, ... |
| `src/Vixen.Core/Sys/ProxyElementNode.cs` | Name, GetMaxChildDepth, GetNonLeafEnumerator, id, GetLeafEnumerator, ... |
| `src/Vixen.Core/Sys/State/Execution/Behavior/StandardOpeningBehavior.cs` | Run |
| `src/Vixen.Core/Sys/SystemConfig.cs` | _dataFlow, FileName, DisabledDeviceIds, _filters, SystemConfig, ... |
| `src/Vixen.Core/Sys/TimeNode.cs` | left, Intersect, right |
| `src/Vixen.Core/Sys/VixenSystem.cs` | AllowFilterEvaluation, systemDataPath, _state, ProfileName, Identity, ... |
| `src/Vixen.Core/Utility/NamingUtilities.cs` | NamingUtilities, name, names, Uniquify |
| `src/Vixen.Modules/App/Fixture/FixtureChannel.cs` | FixtureChannel, ChannelNumber, Function, CreateInstanceForClone |
| `src/Vixen.Modules/App/Fixture/FixtureFunction.cs` | GetIndexDataBase, NarrowToWide, Label, CreateInstanceForClone, FunctionIdentity, ... |
| `src/Vixen.Modules/App/Fixture/FixtureIndexBase.cs` | IndexType, UseCurve, FixtureIndexBase, EndValue, StartValue, ... |
| `src/Vixen.Modules/App/Fixture/FixtureItem.cs` | Name, FixtureItem |
| `src/Vixen.Modules/App/Fixture/FixtureSpecification.cs` | IsFunctionUsed, SupportsFunction, functionName, ContainsColorWheel, IsRGBW, ... |
| `src/Vixen.Modules/App/FixtureSpecificationManager/FixtureSpecificationManager.cs` | FixtureSpecifications |
| `src/Vixen.Modules/App/FixtureSpecificationManager/IFixtureSpecificationManager.cs` | FixtureSpecifications |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncMapStaticData.cs` | MigrateMaps |
| `src/Vixen.Modules/App/LipSyncApp/LipSyncNodeSelect.cs` | SelectedElementNodes |
| `src/Vixen.Modules/App/Modeling/ElementModeling.cs` | TopLevelNode, OrderNodes, leafNodes |
| `src/Vixen.Modules/App/TimedSequenceMapper/SequenceElementMapper/ViewModels/ElementMapperViewModel.cs` | Elements |
| `src/Vixen.Modules/App/WebServer/Service/SystemHelper.cs` | on, id, Save, SetControllerState |
| `src/Vixen.Modules/Editor/EffectEditor/Editors/BaseColorTypeEditor.cs` | GetDiscreteColors, inlineTemplate, BaseColorTypeEditor.<init>, BaseColorTypeEditor, editedType, ... |
| `src/Vixen.Modules/Editor/EffectEditor/Internal/Util.cs` | component, GetDiscreteColors, Util |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/ChannelItemViewModel.cs` | ChannelNumberProperty, IgnoreFunctionName, FunctionProperty, validationResults, functionObjects, ... |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FixturePropertyEditorViewModel.cs` | EditFunctions, Manufacturer, NameProperty, InitializeChildViewModels, UpdateFunctionNames, ... |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FixturePropertyEditorWindowViewModel.cs` | FixtureSpecificationProperty, OK, CanExecuteOK, FixturePropertyEditorWindowViewModel.<init>, FixtureSpecification, ... |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionItemViewModel.cs` | Range, FunctionIdentities, ConvertFixtureFunctionType, RGBWColor, ColorWheel, ... |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionTypeViewModel.cs` | CreateFunctionType, functionType, timelineColor, identity, GetFunctionData, ... |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionTypeWindowViewModel.cs` | functionToSelect, Functions, FunctionTypeWindowViewModel.<init>, UpdatedFunctions, FunctionsProperty, ... |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/ItemsViewModel.cs` | AddItem |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/SelectFixtureSpecificationViewModel.cs` | OkCommand, Specifications, SelectFixtureSpecificationViewModel, SelectedItem, Cancel, ... |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/Views/FixturePropertyEditorView.xaml.cs` | Refresh |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/Views/FixturePropertyEditorWindowView.xaml.cs` | e, Window_Loaded, sender |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/Views/FunctionTypeWindowView.xaml.cs` | e, Hyperlink_RequestNavigate, functionToSelect, Initialize, _functions, ... |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/Views/SelectFixtureSpecificationView.xaml.cs` | GetSelectedFixtureSpecification, SelectFixtureSpecificationView, SelectFixtureSpecificationView.<init>, Initialize, fixtures, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/AutomationWizardPage.cs` | AutomationWizardPage.<init>, AutomaticallyControlDimmer, AutomaticallyOpenAndCloseShutter, AutomaticallyControlColorWheel, GetSummaryString, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/ColorSupportWizardPage.cs` | ColorSupportWizardPage.<init>, ColorSupportWizardPage, NoColorSupport, GetSummaryString, ColorMixing, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/DimmingCurveWizardPage.cs` | NoDimmingCurve, DimmingCurveWizardPage.<init>, DimmingCurveWizardPage, OneDimmingCurvePerColor, DimmingCurve, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/EditProfileFunctionsWizardPage.cs` | EditProfileFunctionsWizardPage.<init>, EditProfileFunctionsWizardPage |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/EditProfileWizardPage.cs` | EditProfileWizardPage.<init>, EditProfileWizardPage |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/FixtureWizardPageBase.cs` | FixtureWizardPageBase.<init>, helpURL, FixtureWizardPageBase, HelpURL |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/GroupingWizardPage.cs` | NumberOfFixtures, GetSummaryString, GroupingWizardPage, ElementPrefix, CreateGroup, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/IntelligentFixtureWizard.cs` | messageService, typeFactory, IntelligentFixtureWizard.<init> |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/SelectProfileWizardPage.cs` | Fixture, CreatedBy, SelectProfileWizardPage.<init>, Revision, GetSummaryString, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/Models/SummaryWizardPageBase.cs` | SummaryWizardPageBase.<init>, GetSummaryString, helpURL, SummaryWizardPageBase, GetSummary |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/AutomationWizardPageViewModel.cs` | EnablePrism, EnableDimmer, EnableShutter, AutomaticallyOpenAndCloseShutter, AutomaticallyControlDimmer, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/ColorSupportWizardPageViewModel.cs` | NoColorSupport, wizardPage, ColorSupportWizardPageViewModel, ColorWheel, ColorSupportWizardPageViewModel.<init>, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/DimmingCurveWizardPageViewModel.cs` | OneDimmingCurvePerColor, DimmingCurveWizardPageViewModel.<init>, wizardPage, EnableDimmingCurvePerColor, NoDimmingCurve, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/EditProfileFunctionsWizardPageViewModel.cs` | EditProfileFunctionsWizardPageViewModel, EditProfileFunctionsWizardPageViewModel.<init>, FunctionsProperty, wizardPage, Functions, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/EditProfileWizardPageViewModel.cs` | CanMoveBack, CanMoveNext, EditProfileWizardPageViewModel.<init>, SaveAsync, IsPageValid, ... |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/EditWizardPageViewModelBase.cs` | EditWizardPageViewModelBase.<init>, TWizardPage, wizardPage, EditWizardPageViewModelBase |
| `src/Vixen.Modules/Editor/FixtureWizard/Wizard/ViewModels/SelectProfileWizardPageViewModel.cs` | Fixture |
| `src/Vixen.Modules/Editor/LayerEditor/Services/ILayerMixingFilterResolver.cs` | Resolve, filterTypeId |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` | ElementChangedRowsHandler, sender, e |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm_Menu.cs` | e, e, sender, sender, changeMapToolStripMenuItem_Click, ... |
| `src/Vixen.Modules/Effect/Alternating/Alternating.cs` | GetNodesToRenderOn |
| `src/Vixen.Modules/Effect/AudioHelper/AudioPluginBase.cs` | GetNodesToRenderOn |
| `src/Vixen.Modules/Effect/Dissolve/Dissolve.cs` | TempClass, ElementIndex, GetNodesToRenderOn, ColorIndex, cancellationToken, ... |
| `src/Vixen.Modules/Effect/Effect/BaseEffect.cs` | point, origin, DistanceFromPoint |
| `src/Vixen.Modules/Effect/Effect/FixtureEffectBase.cs` | _Render, T_Data, cancellationToken, TargetNodesChanged, curve, ... |
| `src/Vixen.Modules/Effect/Effect/FixtureIndexEffectBase.cs` | functionName, indexvalue, functionIdentity, function, DetermineFixtureCapabilities, ... |
| `src/Vixen.Modules/Effect/Effect/IDiscreteColorProvider.cs` | IDiscreteColorProvider, GetDiscreteColors |
| `src/Vixen.Modules/Effect/Effect/PixelEffectBase.cs` | targetNodes, FindLeafParents, FindLeafParents, GetRenderGroups |
| `src/Vixen.Modules/Effect/Fire/Fire.cs` | GetRenderGroups |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/Converters/FunctionCollectionNameConverter.cs` | FunctionCollectionNameConverter, fixtureEffect, GetStandardValuesInternal |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/Data/FixtureData.cs` | FixtureData.<init>, FunctionData, FixtureFunctions, FixtureData, CreateInstanceForClone |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/Data/FixtureFunctionData.cs` | OnDeserialized, IndexValue, ColorIndexValue, FixtureFunctionData, c, ... |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/FixtureFunctionExpando.cs` | _functionName, FixtureFunctions, _indexValue, _fixtureFunctions, _timelineColor, ... |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/FixtureFunctionExpandoCollection.cs` | FixtureFunctionExpandoCollection, FixtureFunctionExpandoCollection.<init> |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/FixtureModule.cs` | EffectModuleData, GetRenderNodesForRangeFunctionType, UpdateFixtureCapabilities, function, FixtureFunctions, ... |
| `src/Vixen.Modules/Effect/Fixture/IFixtureFunctionExpando/IFixtureFunctionExpando.cs` | TimelineColor, ColorIndexValue, IFixtureFunctionExpando, Range, Color, ... |
| `src/Vixen.Modules/Effect/FixtureStrobe/Converters/FixtureStrobeFunctionNameConverter.cs` | GetStandardValuesInternal, effect |
| `src/Vixen.Modules/Effect/FixtureStrobe/FixtureStrobeModule.cs` | PreRenderInternal, cancellationToken, GetCompatibleIndexValues, function, UpdateFixtureCapabilities |
| `src/Vixen.Modules/Effect/Frost/Converters/FrostFunctionNameConverter.cs` | GetStandardValuesInternal, effect |
| `src/Vixen.Modules/Effect/Frost/FrostModule.cs` | FrostModule, cancellationToken, UpdateFixtureCapabilities, PreRenderInternal, _canFrost, ... |
| `src/Vixen.Modules/Effect/Gobo/Converters/GoboFunctionNameConverter.cs` | GetStandardValuesInternal, effect |
| `src/Vixen.Modules/Effect/Gobo/GoboModule.cs` | PreRenderInternal, GetGoboFunctionNames, cancellationToken, UpdateFixtureCapabilities |
| `src/Vixen.Modules/Effect/LineDance/FanCenterOptions.cs` | Left, FanCenterOptions, Right, Centered |
| `src/Vixen.Modules/Effect/LineDance/LineDanceModule.cs` | rightMiddleIndex, renderNodes, ExtraNodeOnLeft, RenderOuterStaggerFan, PanIncrement, ... |
| `src/Vixen.Modules/Effect/Prism/Converters/PrismFunctionNameConverter.cs` | effect, GetStandardValuesInternal |
| `src/Vixen.Modules/Effect/Prism/PrismModule.cs` | cancellationToken, UpdateFixtureCapabilities, PreRenderInternal |
| `src/Vixen.Modules/Effect/SetPosition/SetPositionModule.cs` | cancellationToken, UpdateFixtureCapabilities, SetPositionModule.<init>, PreRenderInternal |
| `src/Vixen.Modules/Effect/SetZoom/SetZoomModule.cs` | SetZoomModule.<init>, UpdateFixtureCapabilities, SetZoomModule, UpdateAttributes, PreRenderInternal, ... |
| `src/Vixen.Modules/Effect/SpinColorWheel/Converters/SpinColorWheelFunctionNameConverter.cs` | effect, GetStandardValuesInternal |
| `src/Vixen.Modules/Effect/SpinColorWheel/SpinColorWheelModule.cs` | PreRenderInternal, UpdateFixtureCapabilities, graphics, GenerateVisualRepresentation, function, ... |
| `src/Vixen.Modules/Effect/State/State.cs` | stateItem, EnumerateTargetScope, node, leafNode, node, ... |
| `src/Vixen.Modules/Effect/State/StateDefinitionDiscovery.cs` | Traverse, node |
| `src/Vixen.Modules/Effect/Twinkle/Twinkle.cs` | GetNodesToRenderOn |
| `src/Vixen.Modules/Effect/Wipe/WipeDirection.cs` | Vertical, DiagonalDown, WipeDirection, Horizontal, Circle, ... |
| `src/Vixen.Modules/Effect/Wipe/WipeModule.cs` | renderedNodes, renderedNodes, Duration, tokenSource, StartTime, ... |
| `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveHelper.Designer.cs` | components, lblQuestion, radioButtonExistingDoNothing, label2, label4, ... |
| `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveHelper.cs` | ExistingBehaviour, DimmingCurveHelper.<init>, skipDimmingCurves, e, HelperName, ... |
| `src/Vixen.Modules/Preview/VixenPreview/OpenGL/OpenGLPreviewForm.cs` | data, OpenGlPreviewForm.<init>, instanceId |
| `src/Vixen.Modules/Preview/VixenPreview/PreviewCustomPropBuilder.cs` | CreateAsync, EnsureFaceMapColors, _tokenLookup, model, _parent, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/MovingHeadIntentHandler.cs` | Handle, rangeIntent, rangeIntent, UpdateLegend |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCane.cs` | node, Reconfigure |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewCustomProp.cs` | aspect, _zoomLevel, _p1Start, PixelSize, Match, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewEllipse.cs` | node, Reconfigure |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewLightBaseShape.cs` | AddPixels, node, lightCount |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewMovingHead.cs` | node, Reconfigure, InitializeMovingHeadMovementConstraints, selectedNode |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewRectangle.cs` | Reconfigure, node |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewTools.cs` | node, node, GetParentNodes, GetLeafNodes |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewModuleInstance.cs` | _updateTimeValue, VixenPreviewModuleInstance.<init> |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetup3.cs` | saveToolStripMenuItem_Click, e, sender |
| `src/Vixen.Modules/Preview/VixenPreview/VixenPreviewSetupElementsDocument.cs` | sender, ButtonAddTemplate_Click, TreeElements_ElementsChanged, AddNodeToTree, template, ... |
| `src/Vixen.Modules/Property/Color/ColorData.cs` | context, ColorData.<init>, ColorData, SingleColor, ColorSetName, ... |
| `src/Vixen.Modules/Property/Color/ColorDescriptor.cs` | ColorDescriptor, ModuleId, TypeId, _typeId, Description, ... |
| `src/Vixen.Modules/Property/Color/ColorPanel.Designer.cs` | disposing, Dispose |
| `src/Vixen.Modules/Property/Color/ColorProperty.cs` | ColorSetName, getValidColorsForElementNode, SingleColor, MultipleDiscreteColors, ModuleData, ... |
| `src/Vixen.Modules/Property/Color/ColorSetsSetupForm.cs` | sender, textBoxName_TextChanged, e |
| `src/Vixen.Modules/Property/Color/ColorSetupForm.Designer.cs` | radioButtonOptionMultiple, buttonOk, ColorSetupForm, groupBoxOptions, disposing, ... |
| `src/Vixen.Modules/Property/Color/ColorSetupForm.cs` | OnFormClosing, sender, sender, PopulateColorSetsComboBox, e, ... |
| `src/Vixen.Modules/Property/Color/ColorSetupHelper.Designer.cs` | label2, disposing, radioButtonOptionFullColor, label3, buttonCancel, ... |
| `src/Vixen.Modules/Property/Color/ColorSetupHelper.cs` | e, _colorSetNameSelectedItem, e, sender, GetColorType, ... |
| `src/Vixen.Modules/Property/Color/ColorStaticData.cs` | GetColorSetNames, name, ContainsColorSet, ColorSets, ColorStaticData |
| `src/Vixen.Modules/Property/Face/FaceComponent.cs` | EyesClosed, Outlines, FaceComponent, EyesOpen, Mouth |
| `src/Vixen.Modules/Property/Face/FaceDescriptor.cs` | Author, Id, Description, TypeId, TypeName, ... |
| `src/Vixen.Modules/Property/Face/FaceMapItem.cs` | OnDeserialized, ElementGuid, PhonemeList, FaceMapItem.<init>, ElementColor, ... |
| `src/Vixen.Modules/Property/Face/FaceModule.cs` | ConfiguredIntensity, Logging, ConfiguredColor, element, DefaultColor, ... |
| `src/Vixen.Modules/Property/Face/FaceSetupHelper.Designer.cs` | tabMouth, tableLayoutPanel1, disposing, dataGridViewOther, components, ... |
| `src/Vixen.Modules/Property/Face/FaceSetupHelper.cs` | e, HelperName, _mouthDataTable, color, ConfigureColumns, ... |
| `src/Vixen.Modules/Property/IntelligentFixture/IntelligentFixtureData.cs` | IntelligentFixtureData.<init> |
| `src/Vixen.Modules/Property/IntelligentFixture/IntelligentFixtureDescriptor.cs` | Version, ModuleClass, IntelligentFixtureDescriptor, Description, TypeId, ... |
| `src/Vixen.Modules/Property/IntelligentFixture/IntelligentFixtureModule.cs` | HasSetup, IntelligentFixtureModule, _data, FixtureSpecification |
| `src/Vixen.Modules/Property/Location/LocationModule.cs` | sourceProperty, CloneValues |
| `src/Vixen.Modules/Property/Order/OrderDescriptor.cs` | TypeId, TypeName, Id, ModuleId, Description, ... |
| `src/Vixen.Modules/Property/Order/OrderModule.cs` | AddPatchingOrder, element, Order, OrderModule.<init>, nodes, ... |
| `src/Vixen.Modules/Property/Order/OrderSetupHelper.Designer.cs` | OrderSetupHelper, btnCancel, columnOrder, elementList, columnName, ... |
| `src/Vixen.Modules/Property/Order/OrderSetupHelper.cs` | ElementList_ItemDragDropCompleted, every, e, ReverseItems_Click, OnKeyDown, ... |
| `src/Vixen.Modules/Property/Orientation/OrientationDescriptor.cs` | ModuleClass, _typeId, Author, TypeName, TypeId, ... |
| `src/Vixen.Modules/Property/Orientation/OrientationModule.cs` | HasSetup, OrientationModule, OrientationModule.<init>, element, CloneValues, ... |
| `src/Vixen.Modules/Property/Orientation/OrientationSetupHelper.cs` | selectedNodes, HelperName, Perform, OrientationSetupHelper |
| `src/Vixen.Modules/Property/State/Setup/Models/StateDefinition.cs` | Element |
| `src/Vixen.Modules/Property/State/Setup/Services/IStateMapperDialogService.cs` | data, node, Show, IStateMapperDialogService |
| `src/Vixen.Modules/Property/State/Setup/Services/StateColorPickerService.cs` | ChooseColor, ChooseColorAsync, WaitForInitiatingMouseInputAsync, nodes, initialColor, ... |
| `src/Vixen.Modules/Property/State/Setup/Services/StateMapperDialogService.cs` | Show, data, node, StateMapperDialogService |
| `src/Vixen.Modules/Property/State/Setup/ViewModels/StateMapperViewModel.cs` | color, rootNode, TryGetFirstCommonDiscreteColor |
| `src/Vixen.Modules/Property/State/StateDescriptor.cs` | TypeName, Author, Version, ModuleDataClass, TypeId, ... |
| `src/Vixen.Modules/Property/State/StateModule.cs` | HasElementSetupHelper, element, nodes, StateModule, _data, ... |
| `src/Vixen.Modules/Property/State/StateSetupHelper.cs` | ShowMapper, Perform, CreateStateModule, StateSetupHelper.<init>, nodes, ... |
| `src/Vixen.Tests/Preview/VixenPreview/PreviewCustomPropStateImportTests.cs` | propertyName, SetVixenSystemProperty, PreviewCustomPropStateImportTests.<init>, value |
| `src/Vixen.Tests/Property/State/StateMapperDefinitionTests.cs` | colors, colors, CreatePropertyManager, CreateNode, id, ... |
| `src/Vixen.Tests/Property/State/StateSetupHelperTests.cs` | StateSetupHelperTests, Perform_ExistingPropertyCancelled_PreservesExistingData, Perform_NewPropertyCancelled_AttachesNothing, Perform_InvalidNewModuleCreation_ReturnsFalse, RecordingStateMapperDialogService, ... |
| `src/Vixen.Tests/Utility/NamingUtilitiesTests.cs` | Uniquify_NameNotInSet_ReturnsNameUnchanged, Uniquify_Name2AlsoInSet_ReturnsNameWithSuffix3, Uniquify_NameAlreadyInSet_ReturnsNameWithSuffix2, NamingUtilitiesTests |

## Connected Communities

- **VixenPreview/Shapes +22 dirs** (58 cross-edges)
- **Effect/Fireworks +11 dirs** (31 cross-edges)
- **Effect/Effect +80 dirs** (11 cross-edges)
- **Editor/TimedSequenceEditor +4 dirs** (10 cross-edges)
- **Vixen.Application/Setup +5 dirs** (7 cross-edges)
- **Editor/TimedSequenceEditor +46 dirs** (6 cross-edges)
- **FixturePropertyEditor/ViewModels +7 dirs** (6 cross-edges)
- **Vixen.Common/NShape +64 dirs** (6 cross-edges)
- **Module/Effect +13 dirs** (5 cross-edges)
- **Vixen.Modules/Effect · ScaleCurveToValue** (5 cross-edges)
- **Vixen.Core/Sys · GroupNode** (5 cross-edges)
- **Data/Flow +8 dirs** (5 cross-edges)
- **Vixen.Application +7 dirs** (4 cross-edges)
- **Property/Color · ColorSetsSetupForm** (4 cross-edges)
- **Vixen.Modules · FixtureSpecificationManager** (3 cross-edges)
- **Vixen.Core/Sys +7 dirs** (3 cross-edges)
- **Vixen.Core/Sys +15 dirs** (3 cross-edges)
- **Vixen.Core · Modules** (2 cross-edges)
- **Sys/Managers +1 dirs** (2 cross-edges)
- **Curves/ZedGraph +7 dirs** (2 cross-edges)
- **Module/Property +9 dirs** (2 cross-edges)
- **Effect/Text +2 dirs** (2 cross-edges)
- **FixturePropertyEditor/ViewModels · FixturePropertyWindowViewModelB…** (2 cross-edges)
- **Vixen.Application/Setup +24 dirs** (2 cross-edges)
- **Effect/State +4 dirs** (2 cross-edges)
- **App/Curves +8 dirs** (2 cross-edges)
- **Vixen.Application/Setup +6 dirs** (2 cross-edges)
- **Preview/VixenPreview · OpenGlPreviewForm** (2 cross-edges)
- **Sys/Output +41 dirs** (2 cross-edges)
- **FixturePropertyEditor/ViewModels · CanExecuteOK** (2 cross-edges)
- **VixenPreview/Undo +10 dirs** (1 cross-edges)
- **Vixen.Common/Controls +1 dirs · MultiSelectTreeview** (1 cross-edges)
- **Sys/Managers · GetEnumerator** (1 cross-edges)
- **VixenPreview/Shapes · MouseMove** (1 cross-edges)
- **Vixen.Core/Sys +3 dirs · Execution** (1 cross-edges)
- **Vixen.Core · SaveToFile** (1 cross-edges)
- **Curves/ZedGraph +13 dirs** (1 cross-edges)
- **Sys/Managers · GetComponent** (1 cross-edges)
- **Vixen.Modules · CreateShapes** (1 cross-edges)
- **Effect/Effect +72 dirs** (1 cross-edges)
- **Vixen.Application +48 dirs** (1 cross-edges)
- **Vixen.Core · WriteContentToObject** (1 cross-edges)
- **WPFCommon/Services +6 dirs** (1 cross-edges)
- **Vixen.Common/NShape +5 dirs** (1 cross-edges)
- **Cache/Sequence +4 dirs** (1 cross-edges)
- **Vixen.Core/Sys · WindowsMultimedia** (1 cross-edges)
- **Editor/TimedSequenceEditor +5 dirs** (1 cross-edges)
- **Vixen.Core/Sys +1 dirs · BuiltInElementTags** (1 cross-edges)
- **CustomPropEditor/Model +8 dirs** (1 cross-edges)
- **App/Curves · GenerateGenericCurveImage** (1 cross-edges)
- **Effect/Dissolve** (1 cross-edges)
- **Editor/FixtureGraphics +3 dirs** (1 cross-edges)
- **TimeLineControl/LabeledMarks +12 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-650")
explore(operation:"context", task:"understand Vixen.Core/Sys +79 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
