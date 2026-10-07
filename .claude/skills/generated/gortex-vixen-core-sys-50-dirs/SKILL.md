---
name: gortex-vixen-core-sys-50-dirs
description: "Work in the Vixen.Core/Sys +50 dirs area — 948 symbols across 140 files (89% cohesion)"
---

# Vixen.Core/Sys +50 dirs

948 symbols | 140 files | 89% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Core/Commands/16BitCommand.cs`
- `src/Vixen.Core/Commands/32BitCommand.cs`
- `src/Vixen.Core/Commands/64BitCommand.cs`
- `src/Vixen.Core/Commands/8BitCommand.cs`
- `src/Vixen.Core/Commands/ColorCommand.cs`
- `src/Vixen.Core/Commands/CommandExtensions.cs`
- `src/Vixen.Core/Commands/ICommand.cs`
- `src/Vixen.Core/Commands/Named8BitCommand.cs`
- `src/Vixen.Core/Commands/StringCommand.cs`
- `src/Vixen.Core/Commands/UnknownValueCommand.cs`
- `src/Vixen.Core/Data/Combinator/Color/NaiveColorCombinator.cs`
- `src/Vixen.Core/Data/Combinator/Combinator.cs`
- `src/Vixen.Core/Data/Combinator/_16Bit/16BitAverageCombinator.cs`
- `src/Vixen.Core/Data/Combinator/_16Bit/16BitHighestWinsCombinator.cs`
- `src/Vixen.Core/Data/Combinator/_32Bit/32BitHighestWinsCombinator.cs`
- `src/Vixen.Core/Data/Combinator/_64Bit/64BitHighestWinsCombinator.cs`
- `src/Vixen.Core/Data/Combinator/_8Bit/8BitAverageCombinator.cs`
- `src/Vixen.Core/Data/Combinator/_8Bit/8BitHighestWinsCombinator.cs`
- `src/Vixen.Core/Data/Evaluator/16BitEvaluator.cs`
- `src/Vixen.Core/Data/Evaluator/32BitEvaluator.cs`
- `src/Vixen.Core/Data/Evaluator/64BitEvaluator.cs`
- `src/Vixen.Core/Data/Evaluator/8BitEvaluator.cs`
- `src/Vixen.Core/Data/Evaluator/ColorEvaluator.cs`
- `src/Vixen.Core/Data/Evaluator/CommandLookup8BitEvaluator.cs`
- `src/Vixen.Core/Data/Evaluator/Evaluator.cs`
- `src/Vixen.Core/Data/Flow/CommandDataFlowData.cs`
- `src/Vixen.Core/Data/Flow/CommandsDataFlowData.cs`
- `src/Vixen.Core/Data/Flow/ElementDataFlowAdapter.cs`
- `src/Vixen.Core/Data/Flow/ElementDataFlowOutputAdapter.cs`
- `src/Vixen.Core/Data/Flow/IDataFlowData.cs`
- `src/Vixen.Core/Data/Flow/IDataFlowOutput.cs`
- `src/Vixen.Core/Data/Flow/IntentDataFlowData.cs`
- `src/Vixen.Core/Data/Flow/IntentsDataFlowData.cs`
- `src/Vixen.Core/Data/Policy/ControllerDataPolicy.cs`
- `src/Vixen.Core/Data/Policy/DataFlowDataDispatchingDataPolicy.cs`
- `src/Vixen.Core/Data/StateCombinator/LayeredStateCombinator.cs`
- `src/Vixen.Core/Data/StateCombinator/StateCombinator.cs`
- `src/Vixen.Core/Data/Value/CommandValue.cs`
- `src/Vixen.Core/Data/Value/IIntentDataType.cs`
- `src/Vixen.Core/Data/Value/IntensityValue.cs`
- `src/Vixen.Core/Data/Value/LightingValue.cs`
- `src/Vixen.Core/Data/Value/RGBValue.cs`
- `src/Vixen.Core/Data/Value/RangeValue.cs`
- `src/Vixen.Core/Export/Export.cs`
- `src/Vixen.Core/Export/ExportCommandHandler.cs`
- `src/Vixen.Core/Instrumentation/IInstrumentation.cs`
- `src/Vixen.Core/Instrumentation/IInstrumentationValue.cs`
- `src/Vixen.Core/Intent/LightingIntent.cs`
- `src/Vixen.Core/Intent/NonSegmentedLinearIntent.cs`
- `src/Vixen.Core/Intent/RGBIntent.cs`
- `src/Vixen.Core/Intent/RangeIntent.cs`
- `src/Vixen.Core/Intent/StaticArrayIntent.cs`
- `src/Vixen.Core/Intent/StaticIntent.cs`
- `src/Vixen.Core/Intent/StaticIntentState.cs`
- `src/Vixen.Core/Intent/StaticLightingIntent.cs`
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
- `src/Vixen.Core/Sys/ICombinator.cs`
- `src/Vixen.Core/Sys/IEvaluator.cs`
- `src/Vixen.Core/Sys/IIntent.cs`
- `src/Vixen.Core/Sys/IIntentState.cs`
- `src/Vixen.Core/Sys/IIntentStates.cs`
- `src/Vixen.Core/Sys/IStateCombinator.cs`
- `src/Vixen.Core/Sys/Instrumentation/Instrumentation.cs`
- `src/Vixen.Core/Sys/IntentNode.cs`
- `src/Vixen.Core/Sys/IntentStateList.cs`
- `src/Vixen.Core/Sys/Output/OutputController.cs`
- `src/Vixen.Core/Sys/OutputIntentStateList.cs`
- `src/Vixen.Core/Sys/OutputStateList.cs`
- `src/Vixen.Core/Sys/OutputStateListAggregator.cs`
- `src/Vixen.Modules/Controller/DDP/DataPolicy.cs`
- `src/Vixen.Modules/Controller/DMXUsbPro/DataPolicy.cs`
- `src/Vixen.Modules/Controller/DMXUsbPro/DmxUsbProSender.cs`
- `src/Vixen.Modules/Controller/DMXUsbPro/Module.cs`
- `src/Vixen.Modules/Controller/DebugController/DebugController.cs`
- `src/Vixen.Modules/Controller/DebugController/DebugControllerOutputForm.cs`
- `src/Vixen.Modules/Controller/DummyLighting/CommandHandler.cs`
- `src/Vixen.Modules/Controller/DummyLighting/DummyLightingOutputForm.cs`
- `src/Vixen.Modules/Controller/DummyLighting/MonochromeDataPolicy.cs`
- `src/Vixen.Modules/Controller/DummyLighting/OneChannelColorDataPolicy.cs`
- `src/Vixen.Modules/Controller/DummyLighting/ThreeChannelColorDataPolicy.cs`
- `src/Vixen.Modules/Controller/E131/DataPolicy.cs`
- `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIOCommandHandler.cs`
- `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIODataPolicy.cs`
- `src/Vixen.Modules/Controller/GenericSerial/CommandHandler.cs`
- `src/Vixen.Modules/Controller/GenericSerial/DataPolicy.cs`
- `src/Vixen.Modules/Controller/GenericSerial/Module.cs`
- `src/Vixen.Modules/Controller/LauncherController/CommandHandler.cs`
- `src/Vixen.Modules/Controller/LauncherController/DataPolicy.cs`
- `src/Vixen.Modules/Controller/OpenDMX/DataPolicy.cs`
- `src/Vixen.Modules/Controller/OpenDMX/OpenDMX.cs`
- `src/Vixen.Modules/Controller/OpenDMX/OpenDMXInstance.cs`
- `src/Vixen.Modules/Controller/RDSController/CommandHandler.cs`
- `src/Vixen.Modules/Controller/RDSController/DataPolicy.cs`
- `src/Vixen.Modules/Controller/Renard/CommandHandler.cs`
- `src/Vixen.Modules/Controller/Renard/RenardDataPolicy.cs`
- `src/Vixen.Modules/Effect/Twinkle/Twinkle.cs`
- `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownFilter.cs`
- `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownOutput.cs`
- `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownOutputConfiguration.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownFilter.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownMixingFilterBase.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/IBreakdownFilter.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/Outputs/ColorBreakdownOutputBase.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/Outputs/_8BitColorBreakdownOutput.cs`
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
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/FullColorIntentHandler.cs`
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPixel.cs`
- `src/Vixen.Tests/Data/Value/RGBValueTests.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Core/Commands/16BitCommand.cs` | value, value, value, _16BitCommand.<init>, _16BitCommand.<init>, ... |
| `src/Vixen.Core/Commands/32BitCommand.cs` | _32BitCommand.<init>, _32BitCommand.<init>, value, value, value, ... |
| `src/Vixen.Core/Commands/64BitCommand.cs` | _64BitCommand.<init>, _64BitCommand.<init>, value, _64BitCommand, _64BitCommand.<init>, ... |
| `src/Vixen.Core/Commands/8BitCommand.cs` | value, value, value, value, value, ... |
| `src/Vixen.Core/Commands/ColorCommand.cs` | ColorCommand.<init>, ColorCommand, value, CommandValue |
| `src/Vixen.Core/Commands/CommandExtensions.cs` | CommandExtensions, Max, commandOther, command |
| `src/Vixen.Core/Commands/ICommand.cs` | ICommand, CommandValue |
| `src/Vixen.Core/Commands/Named8BitCommand.cs` | RangeMaximum, Named8BitCommand.<init>, value, value, IndexType, ... |
| `src/Vixen.Core/Commands/StringCommand.cs` | value, CommandValue, StringCommand, StringCommand.<init> |
| `src/Vixen.Core/Commands/UnknownValueCommand.cs` | UnknownValueCommand, UnknownValueCommand.<init>, CommandValue, value |
| `src/Vixen.Core/Data/Combinator/Color/NaiveColorCombinator.cs` | value2, _MergeColorNaively, Handle, obj, value1, ... |
| `src/Vixen.Core/Data/Combinator/Combinator.cs` | obj, Combine, Handle, Handle, Combine, ... |
| `src/Vixen.Core/Data/Combinator/_16Bit/16BitAverageCombinator.cs` | _16BitAverageCombinator, Handle, obj |
| `src/Vixen.Core/Data/Combinator/_16Bit/16BitHighestWinsCombinator.cs` | Handle, Handle, obj, Handle, Handle, ... |
| `src/Vixen.Core/Data/Combinator/_32Bit/32BitHighestWinsCombinator.cs` | obj, obj, obj, Handle, Handle, ... |
| `src/Vixen.Core/Data/Combinator/_64Bit/64BitHighestWinsCombinator.cs` | Handle, Handle, obj, obj, obj, ... |
| `src/Vixen.Core/Data/Combinator/_8Bit/8BitAverageCombinator.cs` | _8BitAverageCombinator, obj, Handle |
| `src/Vixen.Core/Data/Combinator/_8Bit/8BitHighestWinsCombinator.cs` | Handle, _8BitHighestWinsCombinator, obj, obj, Handle, ... |
| `src/Vixen.Core/Data/Evaluator/16BitEvaluator.cs` | Handle, Handle, obj, Handle, obj, ... |
| `src/Vixen.Core/Data/Evaluator/32BitEvaluator.cs` | obj, Handle, Handle, Handle, obj, ... |
| `src/Vixen.Core/Data/Evaluator/64BitEvaluator.cs` | Handle, _64BitEvaluator, Handle, obj, obj, ... |
| `src/Vixen.Core/Data/Evaluator/8BitEvaluator.cs` | obj, obj, Handle, _8BitEvaluator, Handle, ... |
| `src/Vixen.Core/Data/Evaluator/ColorEvaluator.cs` | Handle, Handle, Handle, obj, obj, ... |
| `src/Vixen.Core/Data/Evaluator/CommandLookup8BitEvaluator.cs` | CommandLookup8BitEvaluator.<init>, CommandLookup8BitEvaluator, CommandLookup |
| `src/Vixen.Core/Data/Evaluator/Evaluator.cs` | obj, intentState, EvaluatorValue, Handle, Handle, ... |
| `src/Vixen.Core/Data/Flow/CommandDataFlowData.cs` | CommandDataFlowData.<init>, CommandDataFlowData, Value, command |
| `src/Vixen.Core/Data/Flow/CommandsDataFlowData.cs` | CommandsDataFlowData.<init>, commands |
| `src/Vixen.Core/Data/Flow/ElementDataFlowAdapter.cs` | _outputs, Outputs |
| `src/Vixen.Core/Data/Flow/ElementDataFlowOutputAdapter.cs` | _element, ElementDataFlowOutputAdapter, _data, element, ElementDataFlowOutputAdapter.<init>, ... |
| `src/Vixen.Core/Data/Flow/IDataFlowData.cs` | IDataFlowData, Value |
| `src/Vixen.Core/Data/Flow/IDataFlowOutput.cs` | IDataFlowOutput, Name, Data |
| `src/Vixen.Core/Data/Flow/IntentDataFlowData.cs` | IntentDataFlowData, Value, IntentDataFlowData.<init>, intentState |
| `src/Vixen.Core/Data/Flow/IntentsDataFlowData.cs` | intentStates, IntentsDataFlowData.<init>, Value, IntentsDataFlowData |
| `src/Vixen.Core/Data/Policy/ControllerDataPolicy.cs` | ControllerDataPolicy |
| `src/Vixen.Core/Data/Policy/DataFlowDataDispatchingDataPolicy.cs` | _evaluator, obj, Handle, intentState, obj, ... |
| `src/Vixen.Core/Data/StateCombinator/LayeredStateCombinator.cs` | LayerComparer, y, MixDiscreteLayerColors, highLayer, MixLayers, ... |
| `src/Vixen.Core/Data/StateCombinator/StateCombinator.cs` | obj, states, obj, Combine, obj, ... |
| `src/Vixen.Core/Data/Value/CommandValue.cs` | command, CommandValue.<init>, CommandValue, Command |
| `src/Vixen.Core/Data/Value/IIntentDataType.cs` | IIntentDataType |
| `src/Vixen.Core/Data/Value/IntensityValue.cs` | Intensity, _intensity, IntensityValue, IntensityValue.<init>, intensity |
| `src/Vixen.Core/Data/Value/LightingValue.cs` | LightingValue.<init>, LightingValue.<init>, lv, color, LightingValue.<init>, ... |
| `src/Vixen.Core/Data/Value/RGBValue.cs` | RGBValue.<init>, color, ConvertToGrayscale, color, RGBValue, ... |
| `src/Vixen.Core/Data/Value/RangeValue.cs` | Tag, Label, TagType, tag, tagType, ... |
| `src/Vixen.Core/Export/Export.cs` | UpdateState, outputStates |
| `src/Vixen.Core/Export/ExportCommandHandler.cs` | Value, Reset, obj, Handle, ExportCommandHandler |
| `src/Vixen.Core/Instrumentation/IInstrumentation.cs` | Values, ValueNames, IInstrumentation |
| `src/Vixen.Core/Instrumentation/IInstrumentationValue.cs` | Name, FormattedValue, Value, Reset, IInstrumentationValue, ... |
| `src/Vixen.Core/Intent/LightingIntent.cs` | LightingIntent.<init>, endValue, timeSpan, startValue |
| `src/Vixen.Core/Intent/NonSegmentedLinearIntent.cs` | TimeSpan, _interpolator, layer, NonSegmentedLinearIntent, EndValue, ... |
| `src/Vixen.Core/Intent/RGBIntent.cs` | startValue, timeSpan, RGBIntent, endValue, RGBIntent.<init> |
| `src/Vixen.Core/Intent/RangeIntent.cs` | RangeIntent.<init>, endValue, startValue, timeSpan |
| `src/Vixen.Core/Intent/StaticArrayIntent.cs` | StaticArrayIntent.<init>, intentRelativeTime, T, _frameTime, CreateIntentState, ... |
| `src/Vixen.Core/Intent/StaticIntent.cs` | StaticIntent, Value, _intentState, StaticIntent.<init>, CreateIntentState, ... |
| `src/Vixen.Core/Intent/StaticIntentState.cs` | _value, layer, SetValue, value, StaticIntentState, ... |
| `src/Vixen.Core/Intent/StaticLightingIntent.cs` | value, StaticLightingIntent, Interpolator, timeSpan, StaticLightingIntent.<init> |
| `src/Vixen.Core/Interpolator/LightingValueInterpolator.cs` | percent, endValue, startValue, InterpolateValue |
| `src/Vixen.Core/Interpolator/RGBValueInterpolator.cs` | endValue, InterpolateValue, startValue, percent |
| `src/Vixen.Core/Interpolator/RangeValueInterpolator.cs` | InterpolateValue, startValue, endValue, percent |
| `src/Vixen.Core/Interpolator/StaticLightingValueInterpolator.cs` | endValue, StaticLightingValueInterpolator, startValue, InterpolateValue, percent |
| `src/Vixen.Core/Sys/Dispatch/CommandDispatch.cs` | Handle, obj, obj, Handle, Handle, ... |
| `src/Vixen.Core/Sys/Dispatch/DataFlowDataDispatch.cs` | obj, obj, Handle, obj, DataFlowDataDispatch, ... |
| `src/Vixen.Core/Sys/Dispatch/IAnyCombinatorHandler.cs` | IAnyCombinatorHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyCommandHandler.cs` | IAnyCommandHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyDataFlowDataHandler.cs` | IAnyDataFlowDataHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyEvaluatorHandler.cs` | IAnyEvaluatorHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyIntentHandler.cs` | IAnyIntentHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyIntentSegmentHandler.cs` | IAnyIntentSegmentHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyIntentStateHandler.cs` | IAnyIntentStateHandler |
| `src/Vixen.Core/Sys/Dispatch/IntentDispatch.cs` | Handle, Handle, Handle, IntentDispatch, obj, ... |
| `src/Vixen.Core/Sys/Dispatch/IntentSegmentDispatch.cs` | Handle, obj, Handle, obj, obj, ... |
| `src/Vixen.Core/Sys/Dispatch/IntentStateDispatch.cs` | obj, IntentStateDispatch, Handle, obj, obj, ... |
| `src/Vixen.Core/Sys/Dispatchable.cs` | Dispatched, IHandler, handler, Dispatchable, HandledType, ... |
| `src/Vixen.Core/Sys/ICombinator.cs` | Combine, commands, ICombinator |
| `src/Vixen.Core/Sys/IEvaluator.cs` | IEvaluator, Evaluate, intentState |
| `src/Vixen.Core/Sys/IIntent.cs` | intentRelativeTime, intentRelativeTime, GetStateAt, GetStateAt |
| `src/Vixen.Core/Sys/IIntentState.cs` | GetValue, IIntentState, Layer |
| `src/Vixen.Core/Sys/IIntentStates.cs` | AsList |
| `src/Vixen.Core/Sys/IStateCombinator.cs` | states, IStateCombinator, Combine |
| `src/Vixen.Core/Sys/Instrumentation/Instrumentation.cs` | ValueNames, name, _values, GetValue, Instrumentation.<init>, ... |
| `src/Vixen.Core/Sys/IntentNode.cs` | intentRelativeTime, layer, CreateIntentState |
| `src/Vixen.Core/Sys/IntentStateList.cs` | AsList |
| `src/Vixen.Core/Sys/Output/OutputController.cs` | commands |
| `src/Vixen.Core/Sys/OutputIntentStateList.cs` | intentStates, OutputIntentStateList.<init>, OutputIntentStateList |
| `src/Vixen.Core/Sys/OutputStateList.cs` | GetCommandForOutput, id |
| `src/Vixen.Core/Sys/OutputStateListAggregator.cs` | GetCommandsForOutput, id |
| `src/Vixen.Modules/Controller/DDP/DataPolicy.cs` | GetEvaluator, GetCombinator, DataPolicy |
| `src/Vixen.Modules/Controller/DMXUsbPro/DataPolicy.cs` | GetEvaluator, GetCombinator, DataPolicy |
| `src/Vixen.Modules/Controller/DMXUsbPro/DmxUsbProSender.cs` | SendDmxPacket, outputStates |
| `src/Vixen.Modules/Controller/DMXUsbPro/Module.cs` | outputStates, UpdateState, chainIndex |
| `src/Vixen.Modules/Controller/DebugController/DebugController.cs` | DataPolicy, GetEvaluator, GetCombinator |
| `src/Vixen.Modules/Controller/DebugController/DebugControllerOutputForm.cs` | UpdateTextBox, UpdateState, text, outputStates |
| `src/Vixen.Modules/Controller/DummyLighting/CommandHandler.cs` | Handle, ByteValue, Reset, ColorValue, obj, ... |
| `src/Vixen.Modules/Controller/DummyLighting/DummyLightingOutputForm.cs` | UpdateState, fps, outputStates |
| `src/Vixen.Modules/Controller/DummyLighting/MonochromeDataPolicy.cs` | GetCombinator, GetEvaluator, MonochromeDataPolicy |
| `src/Vixen.Modules/Controller/DummyLighting/OneChannelColorDataPolicy.cs` | GetEvaluator, GetCombinator, OneChannelColorDataPolicy |
| `src/Vixen.Modules/Controller/DummyLighting/ThreeChannelColorDataPolicy.cs` | ThreeChannelColorDataPolicy, GetEvaluator, GetCombinator |
| `src/Vixen.Modules/Controller/E131/DataPolicy.cs` | GetEvaluator, GetCombinator, DataPolicy |
| `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIOCommandHandler.cs` | Value, obj, Handle, ElexolEtherIOCommandHandler |
| `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIODataPolicy.cs` | GetCombinator, GetEvaluator, ElexolEtherIODataPolicy |
| `src/Vixen.Modules/Controller/GenericSerial/CommandHandler.cs` | Value, obj, CommandHandler, Handle, Reset |
| `src/Vixen.Modules/Controller/GenericSerial/DataPolicy.cs` | GetEvaluator, GetCombinator, DataPolicy |
| `src/Vixen.Modules/Controller/GenericSerial/Module.cs` | outputStates, chainIndex, UpdateState |
| `src/Vixen.Modules/Controller/LauncherController/CommandHandler.cs` | Handle, obj, CommandHandler, Value, Reset |
| `src/Vixen.Modules/Controller/LauncherController/DataPolicy.cs` | DataPolicy, GetCombinator, GetEvaluator |
| `src/Vixen.Modules/Controller/OpenDMX/DataPolicy.cs` | DataPolicy, GetCombinator, GetEvaluator |
| `src/Vixen.Modules/Controller/OpenDMX/OpenDMX.cs` | outputStates, UpdateData |
| `src/Vixen.Modules/Controller/OpenDMX/OpenDMXInstance.cs` | chainInex, UpdateState, outputStates |
| `src/Vixen.Modules/Controller/RDSController/CommandHandler.cs` | Reset, obj, CommandHandler, Value, Handle |
| `src/Vixen.Modules/Controller/RDSController/DataPolicy.cs` | GetCombinator, GetEvaluator, DataPolicy |
| `src/Vixen.Modules/Controller/Renard/CommandHandler.cs` | CommandHandler, Handle, Value, obj |
| `src/Vixen.Modules/Controller/Renard/RenardDataPolicy.cs` | GetEvaluator, RenardDataPolicy, GetCombinator |
| `src/Vixen.Modules/Effect/Twinkle/Twinkle.cs` | values, CreateIntentForValues |
| `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownFilter.cs` | _intentValue, CoarseFineBreakdownFilter, Filter, intentValue, intent, ... |
| `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownOutput.cs` | canonicalValue, intents, Handle, ProcessInputData, HandleDefaultValue, ... |
| `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownOutputConfiguration.cs` | RestingFineValue, CoarseFineBreakdownOutputConfiguration, RestingValue, EnableDefaultValueMapping, RestingCoarseValue |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownFilter.cs` | Tolerance, Handle, breakdownItem, ColorBreakdownFilter, intentValue, ... |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownMixingFilterBase.cs` | GetMaxProportionFunc, _intensityValue, ColorBreakdownMixingFilterBase, Handle, obj, ... |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/IBreakdownFilter.cs` | intentValue, GetIntensityForState, IBreakdownFilter |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/Outputs/ColorBreakdownOutputBase.cs` | data, ColorBreakdownOutputBase, ProcessInputData, Name, intensity, ... |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/Outputs/_8BitColorBreakdownOutput.cs` | ProcessInputDataInternal, _8BitColorBreakdownOutput, _8BitColorBreakdownOutput.<init>, intensity, rgbToRGBWConverter, ... |
| `src/Vixen.Modules/OutputFilter/ColorWheelFilter/ColorWheelFilterModule.cs` | CreateOutputInternal, HasSetup, ColorWheelFilterModule |
| `src/Vixen.Modules/OutputFilter/ColorWheelFilter/Filters/ColorWheelFilter.cs` | rgbIntent, ConvertDiscreteColorToIndexCommand, Handle, Handle, obj, ... |
| `src/Vixen.Modules/OutputFilter/ColorWheelFilter/Outputs/ColorWheelFilterOutput.cs` | ConfigureFilter, _convertColorIntentsIntoColorWheel, ColorWheelFilterOutput, _colorWheelData |
| `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveModule.cs` | DimmingCurveOutput, Handle, Data, ProcessInputData, obj, ... |
| `src/Vixen.Modules/OutputFilter/DimmingFilter/DimmingFilterModule.cs` | CreateOutputInternal |
| `src/Vixen.Modules/OutputFilter/DimmingFilter/Filters/DimmingFilter.cs` | ConvertLightingValueToDimIntent, obj, ConvertIntensityToDimIntent, rgbIntent, obj, ... |
| `src/Vixen.Modules/OutputFilter/DimmingFilter/Outputs/DimmingFilterOutput.cs` | tag, _convertColorIntensityIntoDimIntents, DimmingFilterOutput.<init>, convertColorIntensityIntoDimIntents, ConfigureFilter, ... |
| `src/Vixen.Modules/OutputFilter/PrismFilter/Filter/PrismFilter.cs` | intent, intent, Handle, HandleIntent, OpenPrismIndexValue, ... |
| `src/Vixen.Modules/OutputFilter/PrismFilter/Output/PrismFilterOutput.cs` | _closePrismIndexValue, _convertPrismIntentsIntoOpenPrismIntents, intentData, openPrismIndexValue, CreateCloseShutterCommand, ... |
| `src/Vixen.Modules/OutputFilter/ShutterFilter/Filter/ShutterFilter.cs` | obj, Handle, ConvertColorIntoShutterIntents, intent, Handle, ... |
| `src/Vixen.Modules/OutputFilter/ShutterFilter/Output/ShuttterFilterOutput.cs` | _closeShutterIndexValue, CreateCloseShutterCommand, IntentData, intentData, ConfigureFilter, ... |
| `src/Vixen.Modules/OutputFilter/ShutterFilter/ShutterFilterModule.cs` | CreateOutputInternal |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Filters/ITaggedFilter.cs` | Tag, Filter, ITaggedFilter, intentValue |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Filters/TaggedFilter.cs` | TaggedFilter, Handle, Handle, intent, intent |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Filters/TaggedFilterBase.cs` | intent, Tag, IntentValue, TaggedFilterBase, Filter |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Outputs/ITaggedFilterOutput.cs` | ProcessInputData, ITaggedFilterOutput, data |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Outputs/TaggedFilterOutputBase.cs` | ConfigureFilter, IsIntentApplicable, TaggedFilterOutputBase, TFilter, intent, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/FullColorIntentHandler.cs` | obj, obj, state, Handle, FullColorIntentHandler, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPixel.cs` | states, GetFullColor |
| `src/Vixen.Tests/Data/Value/RGBValueTests.cs` | GetGrayscaleLevel_Black_Returns0, GetGrayscaleLevel_Red_Returns76, GetGrayscaleLevel_White_Returns255, RGBValueTests |

## Connected Communities

- **Vixen.Core · Interpolate** (1 cross-edges)
- **Editor/TimedSequenceEditor +68 dirs** (1 cross-edges)
- **Vixen.Common/NShape +70 dirs** (1 cross-edges)
- **Vixen.Core/Sys · AddDataPaths** (1 cross-edges)
- **Effect/Fireworks +9 dirs** (1 cross-edges)
- **VixenPreview/Shapes +22 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-638")
explore(operation:"context", task:"understand Vixen.Core/Sys +50 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
