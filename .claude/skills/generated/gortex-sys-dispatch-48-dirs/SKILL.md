---
name: gortex-sys-dispatch-48-dirs
description: "Work in the Sys/Dispatch +48 dirs area — 790 symbols across 111 files (83% cohesion)"
---

# Sys/Dispatch +48 dirs

790 symbols | 111 files | 83% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Core/Commands/ColorCommand.cs`
- `src/Vixen.Core/Commands/FixtureIndexType.cs`
- `src/Vixen.Core/Commands/Named8BitCommand.cs`
- `src/Vixen.Core/Commands/StringCommand.cs`
- `src/Vixen.Core/Data/Combinator/_16Bit/16BitHighestWinsCombinator.cs`
- `src/Vixen.Core/Data/Combinator/_32Bit/32BitHighestWinsCombinator.cs`
- `src/Vixen.Core/Data/Combinator/_64Bit/64BitHighestWinsCombinator.cs`
- `src/Vixen.Core/Data/Combinator/_8Bit/8BitHighestWinsCombinator.cs`
- `src/Vixen.Core/Data/Evaluator/16BitEvaluator.cs`
- `src/Vixen.Core/Data/Evaluator/32BitEvaluator.cs`
- `src/Vixen.Core/Data/Evaluator/64BitEvaluator.cs`
- `src/Vixen.Core/Data/Evaluator/8BitEvaluator.cs`
- `src/Vixen.Core/Data/Evaluator/ColorEvaluator.cs`
- `src/Vixen.Core/Data/Evaluator/CommandLookup8BitEvaluator.cs`
- `src/Vixen.Core/Data/Evaluator/Evaluator.cs`
- `src/Vixen.Core/Data/Flow/CommandDataFlowData.cs`
- `src/Vixen.Core/Data/Flow/ElementDataFlowAdapter.cs`
- `src/Vixen.Core/Data/Flow/ElementDataFlowOutputAdapter.cs`
- `src/Vixen.Core/Data/Flow/IDataFlowData.cs`
- `src/Vixen.Core/Data/Flow/IDataFlowOutput.cs`
- `src/Vixen.Core/Data/Flow/IntentDataFlowData.cs`
- `src/Vixen.Core/Data/Flow/IntentsDataFlowData.cs`
- `src/Vixen.Core/Data/StateCombinator/LayeredStateCombinator.cs`
- `src/Vixen.Core/Data/StateCombinator/StateCombinator.cs`
- `src/Vixen.Core/Data/Value/CommandValue.cs`
- `src/Vixen.Core/Data/Value/FunctionIdentity.cs`
- `src/Vixen.Core/Data/Value/IIntentDataType.cs`
- `src/Vixen.Core/Data/Value/IntensityValue.cs`
- `src/Vixen.Core/Data/Value/LightingValue.cs`
- `src/Vixen.Core/Data/Value/RGBValue.cs`
- `src/Vixen.Core/Data/Value/RangeValue.cs`
- `src/Vixen.Core/Export/ExportCommandHandler.cs`
- `src/Vixen.Core/Instrumentation/IInstrumentation.cs`
- `src/Vixen.Core/Instrumentation/IInstrumentationValue.cs`
- `src/Vixen.Core/Intent/LightingIntent.cs`
- `src/Vixen.Core/Intent/RGBIntent.cs`
- `src/Vixen.Core/Intent/RangeIntent.cs`
- `src/Vixen.Core/Intent/StaticIntentState.cs`
- `src/Vixen.Core/Interpolator/LightingValueInterpolator.cs`
- `src/Vixen.Core/Interpolator/RGBValueInterpolator.cs`
- `src/Vixen.Core/Interpolator/RangeValueInterpolator.cs`
- `src/Vixen.Core/Interpolator/StaticLightingValueInterpolator.cs`
- `src/Vixen.Core/Sys/Dispatch/CommandDispatch.cs`
- `src/Vixen.Core/Sys/Dispatch/DataFlowDataDispatch.cs`
- `src/Vixen.Core/Sys/Dispatch/IAnyCombinatorHandler.cs`
- `src/Vixen.Core/Sys/Dispatch/IAnyCommandHandler.cs`
- `src/Vixen.Core/Sys/Dispatch/IAnyDataFlowDataHandler.cs`
- `src/Vixen.Core/Sys/Dispatch/IAnyEvaluatorHandler.cs`
- `src/Vixen.Core/Sys/Dispatch/IAnyIntentHandler.cs`
- `src/Vixen.Core/Sys/Dispatch/IAnyIntentSegmentHandler.cs`
- `src/Vixen.Core/Sys/Dispatch/IAnyIntentStateHandler.cs`
- `src/Vixen.Core/Sys/Dispatch/IntentDispatch.cs`
- `src/Vixen.Core/Sys/Dispatch/IntentSegmentDispatch.cs`
- `src/Vixen.Core/Sys/Dispatch/IntentStateDispatch.cs`
- `src/Vixen.Core/Sys/Dispatchable.cs`
- `src/Vixen.Core/Sys/IIntentState.cs`
- `src/Vixen.Core/Sys/IIntentStates.cs`
- `src/Vixen.Core/Sys/IStateCombinator.cs`
- `src/Vixen.Core/Sys/Instrumentation/Instrumentation.cs`
- `src/Vixen.Core/Sys/IntentNode.cs`
- `src/Vixen.Core/Sys/IntentStateList.cs`
- `src/Vixen.Core/Sys/OutputIntentStateList.cs`
- `src/Vixen.Modules/App/Fixture/FixtureColorWheel.cs`
- `src/Vixen.Modules/App/Fixture/FixtureFunction.cs`
- `src/Vixen.Modules/Controller/DebugController/DebugControllerOutputForm.cs`
- `src/Vixen.Modules/Controller/DummyLighting/CommandHandler.cs`
- `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIOCommandHandler.cs`
- `src/Vixen.Modules/Controller/GenericSerial/CommandHandler.cs`
- `src/Vixen.Modules/Controller/LauncherController/CommandHandler.cs`
- `src/Vixen.Modules/Controller/RDSController/CommandHandler.cs`
- `src/Vixen.Modules/Controller/Renard/CommandHandler.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/ColorWheelViewModel.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionItemViewModel.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionTypeViewModel.cs`
- `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/IndexedItemViewModel.cs`
- `src/Vixen.Modules/Effect/Fixture/FixtureEffect/FixtureFunctionExpando.cs`
- `src/Vixen.Modules/Effect/Frost/FrostModule.cs`
- `src/Vixen.Modules/Effect/SetPosition/SetPositionModule.cs`
- `src/Vixen.Modules/Effect/SetZoom/SetZoomModule.cs`
- `src/Vixen.Modules/Effect/SpinColorWheel/SpinColorWheelModule.cs`
- `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownFilter.cs`
- `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownOutput.cs`
- `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownOutputConfiguration.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownFilter.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownMixingFilterBase.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/IBreakdownFilter.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/Outputs/ColorBreakdownOutputBase.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/Outputs/_8BitColorBreakdownOutput.cs`
- `src/Vixen.Modules/OutputFilter/ColorWheelFilter/ColorWheelFilterData.cs`
- `src/Vixen.Modules/OutputFilter/ColorWheelFilter/ColorWheelFilterModule.cs`
- `src/Vixen.Modules/OutputFilter/ColorWheelFilter/Filters/ColorWheelFilter.cs`
- `src/Vixen.Modules/OutputFilter/ColorWheelFilter/Outputs/ColorWheelFilterOutput.cs`
- `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveModule.cs`
- `src/Vixen.Modules/OutputFilter/DimmingFilter/DimmingFilterModule.cs`
- `src/Vixen.Modules/OutputFilter/DimmingFilter/Filters/DimmingFilter.cs`
- `src/Vixen.Modules/OutputFilter/DimmingFilter/Outputs/DimmingFilterOutput.cs`
- `src/Vixen.Modules/OutputFilter/PrismFilter/Filter/PrismFilter.cs`
- `src/Vixen.Modules/OutputFilter/PrismFilter/Output/PrismFilterOutput.cs`
- `src/Vixen.Modules/OutputFilter/ShutterFilter/Filter/ShutterFilter.cs`
- `src/Vixen.Modules/OutputFilter/ShutterFilter/Output/ShuttterFilterOutput.cs`
- `src/Vixen.Modules/OutputFilter/ShutterFilter/ShutterFilterModule.cs`
- `src/Vixen.Modules/OutputFilter/TaggedFilter/Filters/ITaggedFilter.cs`
- `src/Vixen.Modules/OutputFilter/TaggedFilter/Filters/TaggedFilter.cs`
- `src/Vixen.Modules/OutputFilter/TaggedFilter/Filters/TaggedFilterBase.cs`
- `src/Vixen.Modules/OutputFilter/TaggedFilter/Outputs/ITaggedFilterOutput.cs`
- `src/Vixen.Modules/OutputFilter/TaggedFilter/Outputs/TaggedFilterOutputBase.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/FullColorIntentHandler.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/MovingHeadIntentHandler.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPixel.cs`
- `src/Vixen.Tests/Data/Value/RGBValueTests.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Core/Commands/ColorCommand.cs` | CommandValue, ColorCommand.<init>, value, ColorCommand |
| `src/Vixen.Core/Commands/FixtureIndexType.cs` | PrismOpen, ShutterOpen, LampOff, Custom, Prism, ... |
| `src/Vixen.Core/Commands/Named8BitCommand.cs` | Named8BitCommand.<init>, value, Tag, RangeMinimum, value, ... |
| `src/Vixen.Core/Commands/StringCommand.cs` | StringCommand, value, CommandValue, StringCommand.<init> |
| `src/Vixen.Core/Data/Combinator/_16Bit/16BitHighestWinsCombinator.cs` | Handle, obj |
| `src/Vixen.Core/Data/Combinator/_32Bit/32BitHighestWinsCombinator.cs` | obj, Handle |
| `src/Vixen.Core/Data/Combinator/_64Bit/64BitHighestWinsCombinator.cs` | obj, Handle |
| `src/Vixen.Core/Data/Combinator/_8Bit/8BitHighestWinsCombinator.cs` | Handle, obj |
| `src/Vixen.Core/Data/Evaluator/16BitEvaluator.cs` | Handle, obj, Handle, obj, Handle, ... |
| `src/Vixen.Core/Data/Evaluator/32BitEvaluator.cs` | obj, obj, obj, Handle, obj, ... |
| `src/Vixen.Core/Data/Evaluator/64BitEvaluator.cs` | Handle, Handle, obj, _64BitEvaluator, Handle, ... |
| `src/Vixen.Core/Data/Evaluator/8BitEvaluator.cs` | Handle, obj, Handle, _8BitEvaluator, obj, ... |
| `src/Vixen.Core/Data/Evaluator/ColorEvaluator.cs` | obj, Handle, obj, obj, Handle, ... |
| `src/Vixen.Core/Data/Evaluator/CommandLookup8BitEvaluator.cs` | CommandLookup8BitEvaluator.<init>, CommandLookup8BitEvaluator, CommandLookup |
| `src/Vixen.Core/Data/Evaluator/Evaluator.cs` | obj, obj, Handle, obj, obj, ... |
| `src/Vixen.Core/Data/Flow/CommandDataFlowData.cs` | CommandDataFlowData.<init>, command, Value, CommandDataFlowData |
| `src/Vixen.Core/Data/Flow/ElementDataFlowAdapter.cs` | _outputs, Outputs |
| `src/Vixen.Core/Data/Flow/ElementDataFlowOutputAdapter.cs` | _element, ElementDataFlowOutputAdapter, Data, element, ElementDataFlowOutputAdapter.<init>, ... |
| `src/Vixen.Core/Data/Flow/IDataFlowData.cs` | IDataFlowData, Value |
| `src/Vixen.Core/Data/Flow/IDataFlowOutput.cs` | Data, IDataFlowOutput, Name |
| `src/Vixen.Core/Data/Flow/IntentDataFlowData.cs` | intentState, IntentDataFlowData, Value, IntentDataFlowData.<init> |
| `src/Vixen.Core/Data/Flow/IntentsDataFlowData.cs` | IntentsDataFlowData, IntentsDataFlowData.<init>, intentStates, Value |
| `src/Vixen.Core/Data/StateCombinator/LayeredStateCombinator.cs` | filter, MixLayerColors, x, _tempMixingColor, LayeredStateCombinator, ... |
| `src/Vixen.Core/Data/StateCombinator/StateCombinator.cs` | Handle, Handle, Handle, Handle, obj, ... |
| `src/Vixen.Core/Data/Value/CommandValue.cs` | Command, CommandValue, command, CommandValue.<init> |
| `src/Vixen.Core/Data/Value/FunctionIdentity.cs` | OpenClosePrism, Zoom, Prism, Pan, Gobo, ... |
| `src/Vixen.Core/Data/Value/IIntentDataType.cs` | IIntentDataType |
| `src/Vixen.Core/Data/Value/IntensityValue.cs` | IntensityValue, intensity, _intensity, Intensity, IntensityValue.<init> |
| `src/Vixen.Core/Data/Value/LightingValue.cs` | lv, LightingValue.<init>, intensity, LightingValue.<init>, Intensity, ... |
| `src/Vixen.Core/Data/Value/RGBValue.cs` | Intensity, G, color, GetGrayscaleLevel, _BasicGrayscaleLuma, ... |
| `src/Vixen.Core/Data/Value/RangeValue.cs` | Tag, TagType, rangeValue, label, Label, ... |
| `src/Vixen.Core/Export/ExportCommandHandler.cs` | Value, Reset, obj, Handle, ExportCommandHandler |
| `src/Vixen.Core/Instrumentation/IInstrumentation.cs` | ValueNames, Values, IInstrumentation |
| `src/Vixen.Core/Instrumentation/IInstrumentationValue.cs` | Value, Name, Maximum, Minimum, Reset, ... |
| `src/Vixen.Core/Intent/LightingIntent.cs` | timeSpan, startValue, LightingIntent.<init>, endValue |
| `src/Vixen.Core/Intent/RGBIntent.cs` | timeSpan, RGBIntent.<init>, RGBIntent, endValue, startValue |
| `src/Vixen.Core/Intent/RangeIntent.cs` | RangeIntent.<init>, timeSpan, endValue, startValue |
| `src/Vixen.Core/Intent/StaticIntentState.cs` | ResultType, _value, value, GetValue, StaticIntentState, ... |
| `src/Vixen.Core/Interpolator/LightingValueInterpolator.cs` | startValue, percent, endValue, InterpolateValue |
| `src/Vixen.Core/Interpolator/RGBValueInterpolator.cs` | InterpolateValue, percent, startValue, endValue |
| `src/Vixen.Core/Interpolator/RangeValueInterpolator.cs` | endValue, startValue, percent, InterpolateValue |
| `src/Vixen.Core/Interpolator/StaticLightingValueInterpolator.cs` | percent, startValue, endValue, InterpolateValue |
| `src/Vixen.Core/Sys/Dispatch/CommandDispatch.cs` | Handle, obj, Handle, Handle, obj, ... |
| `src/Vixen.Core/Sys/Dispatch/DataFlowDataDispatch.cs` | DataFlowDataDispatch, obj, obj, Handle, obj, ... |
| `src/Vixen.Core/Sys/Dispatch/IAnyCombinatorHandler.cs` | IAnyCombinatorHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyCommandHandler.cs` | IAnyCommandHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyDataFlowDataHandler.cs` | IAnyDataFlowDataHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyEvaluatorHandler.cs` | IAnyEvaluatorHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyIntentHandler.cs` | IAnyIntentHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyIntentSegmentHandler.cs` | IAnyIntentSegmentHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyIntentStateHandler.cs` | IAnyIntentStateHandler |
| `src/Vixen.Core/Sys/Dispatch/IntentDispatch.cs` | obj, obj, IntentDispatch, obj, Handle, ... |
| `src/Vixen.Core/Sys/Dispatch/IntentSegmentDispatch.cs` | IntentSegmentDispatch, Handle, Handle, Handle, obj, ... |
| `src/Vixen.Core/Sys/Dispatch/IntentStateDispatch.cs` | obj, Handle, obj, obj, Handle, ... |
| `src/Vixen.Core/Sys/Dispatchable.cs` | Dispatchable, IHandler, Dispatched, handler, HandlerType, ... |
| `src/Vixen.Core/Sys/IIntentState.cs` | GetValue, Layer, IIntentState |
| `src/Vixen.Core/Sys/IIntentStates.cs` | AsList |
| `src/Vixen.Core/Sys/IStateCombinator.cs` | Combine, states |
| `src/Vixen.Core/Sys/Instrumentation/Instrumentation.cs` | GetValue, Instrumentation.<init>, ValueNames, Values, _values, ... |
| `src/Vixen.Core/Sys/IntentNode.cs` | layer, intentRelativeTime, CreateIntentState |
| `src/Vixen.Core/Sys/IntentStateList.cs` | AsList |
| `src/Vixen.Core/Sys/OutputIntentStateList.cs` | OutputIntentStateList, intentStates, AddIntentState, OutputIntentStateList.<init>, intentState |
| `src/Vixen.Modules/App/Fixture/FixtureColorWheel.cs` | Color1, FixtureColorWheel.<init>, Color2, FixtureColorWheel, CreateInstanceForClone, ... |
| `src/Vixen.Modules/App/Fixture/FixtureFunction.cs` | ColorWheelData |
| `src/Vixen.Modules/Controller/DebugController/DebugControllerOutputForm.cs` | UpdateState, outputStates |
| `src/Vixen.Modules/Controller/DummyLighting/CommandHandler.cs` | CommandHandler, obj, obj, ColorValue, ByteValue, ... |
| `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIOCommandHandler.cs` | Handle, ElexolEtherIOCommandHandler, Value, obj |
| `src/Vixen.Modules/Controller/GenericSerial/CommandHandler.cs` | Handle, Value, CommandHandler, obj |
| `src/Vixen.Modules/Controller/LauncherController/CommandHandler.cs` | CommandHandler, Value, obj, Handle, Reset |
| `src/Vixen.Modules/Controller/RDSController/CommandHandler.cs` | CommandHandler, Handle, Reset, Value, obj |
| `src/Vixen.Modules/Controller/Renard/CommandHandler.cs` | obj, CommandHandler, Handle, Value |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/ColorWheelViewModel.cs` | GetColorWheelData |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionItemViewModel.cs` | FunctionItemViewModel.<init>, ColorWheelData, FunctionIdentities |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/FunctionTypeViewModel.cs` | SaveColorWheelData |
| `src/Vixen.Modules/Editor/FixturePropertyEditor/ViewModels/IndexedItemViewModel.cs` | IndexTypes |
| `src/Vixen.Modules/Effect/Fixture/FixtureEffect/FixtureFunctionExpando.cs` | ColorWheelIndexData |
| `src/Vixen.Modules/Effect/Frost/FrostModule.cs` | UpdateFixtureCapabilities |
| `src/Vixen.Modules/Effect/SetPosition/SetPositionModule.cs` | PreRenderInternal, cancellationToken, UpdateFixtureCapabilities |
| `src/Vixen.Modules/Effect/SetZoom/SetZoomModule.cs` | SetZoomModule.<init>, SetZoomModule, PreRenderInternal, _canZoom, UpdateFixtureCapabilities, ... |
| `src/Vixen.Modules/Effect/SpinColorWheel/SpinColorWheelModule.cs` | PreRenderInternal, cancellationToken, UpdateFixtureCapabilities |
| `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownFilter.cs` | CoarseFineBreakdownFilter, Filter, intentValue, _intentValue, intent, ... |
| `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownOutput.cs` | Name, CoarseFineBreakdownOutput.<init>, _commandsData, configuration, _configuration, ... |
| `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownOutputConfiguration.cs` | CoarseFineBreakdownOutputConfiguration, RestingCoarseValue, RestingValue, EnableDefaultValueMapping, RestingFineValue |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownFilter.cs` | obj, ColorBreakdownFilter.<init>, _breakdownItem, ColorBreakdownFilter, obj, ... |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownMixingFilterBase.cs` | obj, _intensityValue, obj, GetIntensityForState, intentValue, ... |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/IBreakdownFilter.cs` | IBreakdownFilter, GetIntensityForState, intentValue |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/Outputs/ColorBreakdownOutputBase.cs` | BreakdownItem, mixColors, ColorBreakdownOutputBase.<init>, ProcessInputData, rgbToRGBWConverter, ... |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/Outputs/_8BitColorBreakdownOutput.cs` | _8BitColorBreakdownOutput, ProcessInputDataInternal, rgbToRGBWConverter, _8BitColorBreakdownOutput.<init>, mixColors, ... |
| `src/Vixen.Modules/OutputFilter/ColorWheelFilter/ColorWheelFilterData.cs` | ColorWheelData |
| `src/Vixen.Modules/OutputFilter/ColorWheelFilter/ColorWheelFilterModule.cs` | ColorWheelData, ColorWheelFilterModule, CreateOutputInternal, HasSetup |
| `src/Vixen.Modules/OutputFilter/ColorWheelFilter/Filters/ColorWheelFilter.cs` | obj, rgbIntent, color, discreteIntent, obj, ... |
| `src/Vixen.Modules/OutputFilter/ColorWheelFilter/Outputs/ColorWheelFilterOutput.cs` | colorWheelData, convertRBGIntoColorWheel, tag, ConfigureFilter, _convertColorIntentsIntoColorWheel, ... |
| `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveModule.cs` | DimmingCurveOutput.<init>, _states, curve, data, data, ... |
| `src/Vixen.Modules/OutputFilter/DimmingFilter/DimmingFilterModule.cs` | CreateOutputInternal |
| `src/Vixen.Modules/OutputFilter/DimmingFilter/Filters/DimmingFilter.cs` | Handle, rgbIntent, discreteIntent, intensity, DimmingFilter, ... |
| `src/Vixen.Modules/OutputFilter/DimmingFilter/Outputs/DimmingFilterOutput.cs` | tag, DimmingFilterOutput.<init>, _convertColorIntensityIntoDimIntents, ConfigureFilter, convertColorIntensityIntoDimIntents, ... |
| `src/Vixen.Modules/OutputFilter/PrismFilter/Filter/PrismFilter.cs` | OpenPrismIndexValue, intent, PrismFilter, HandleIntent, intent, ... |
| `src/Vixen.Modules/OutputFilter/PrismFilter/Output/PrismFilterOutput.cs` | _openPrismIndexValue, intentData, _associatedFunctionName, _closePrismIndexValue, PrismFilterOutput.<init>, ... |
| `src/Vixen.Modules/OutputFilter/ShutterFilter/Filter/ShutterFilter.cs` | ConvertColorIntoShutterIntents, OpenShutterIndexValue, HandleIntent, Handle, Handle, ... |
| `src/Vixen.Modules/OutputFilter/ShutterFilter/Output/ShuttterFilterOutput.cs` | ShutterFilterOutput, openShutterIndexValue, IntentData, intentData, _convertColorIntoShutterIntents, ... |
| `src/Vixen.Modules/OutputFilter/ShutterFilter/ShutterFilterModule.cs` | CreateOutputInternal |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Filters/ITaggedFilter.cs` | Tag, Filter, intentValue, ITaggedFilter |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Filters/TaggedFilter.cs` | intent, TaggedFilter, Handle, intent, Handle |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Filters/TaggedFilterBase.cs` | TaggedFilterBase, intent, IntentValue, Filter, Tag |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Outputs/ITaggedFilterOutput.cs` | ITaggedFilterOutput, data, ProcessInputData |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Outputs/TaggedFilterOutputBase.cs` | intent, tag, Filter, IsIntentApplicable, TFilter, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs` | state, state, Handle, Handle |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/FullColorIntentHandler.cs` | Handle, GetFullColor, obj, FullColorIntentHandler, Handle, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/MovingHeadIntentHandler.cs` | OpenShutter, Handle, label, lightingIntent, taggedCommand, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPixel.cs` | states, GetFullColor |
| `src/Vixen.Tests/Data/Value/RGBValueTests.cs` | GetGrayscaleLevel_Red_Returns76, RGBValueTests, GetGrayscaleLevel_White_Returns255, GetGrayscaleLevel_Black_Returns0 |

## Connected Communities

- **Vixen.Core · GetStateAt** (4 cross-edges)
- **VixenPreview/Shapes +20 dirs** (3 cross-edges)
- **Vixen.Common/NShape +69 dirs** (2 cross-edges)
- **Editor/TimedSequenceEditor +70 dirs** (1 cross-edges)
- **Vixen.Core/Sys · AddDataPaths** (1 cross-edges)
- **TimedSequenceEditor/Forms +44 dirs** (1 cross-edges)
- **OpenGL/Volumes +24 dirs** (1 cross-edges)
- **App/Curves +3 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-669")
explore(operation:"context", task:"understand Sys/Dispatch +48 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
