---
name: gortex-sys-dispatch-47-dirs
description: "Work in the Sys/Dispatch +47 dirs area — 904 symbols across 134 files (89% cohesion)"
---

# Sys/Dispatch +47 dirs

904 symbols | 134 files | 89% cohesion

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
- `src/Vixen.Core/Intent/RGBIntent.cs`
- `src/Vixen.Core/Intent/RangeIntent.cs`
- `src/Vixen.Core/Intent/StaticIntent.cs`
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
- `src/Vixen.Core/Sys/ICombinator.cs`
- `src/Vixen.Core/Sys/IEvaluator.cs`
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
- `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownFilter.cs`
- `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownOutput.cs`
- `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownOutputConfiguration.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownFilter.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownMixingFilterBase.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/IBreakdownFilter.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/Outputs/ColorBreakdownOutputBase.cs`
- `src/Vixen.Modules/OutputFilter/ColorBreakdown/Outputs/_8BitColorBreakdownOutput.cs`
- `src/Vixen.Modules/OutputFilter/ColorWheelFilter/Filters/ColorWheelFilter.cs`
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
- `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPixel.cs`
- `src/Vixen.Tests/Data/Value/RGBValueTests.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Core/Commands/16BitCommand.cs` | _16BitCommand.<init>, value, _16BitCommand, _16BitCommand.<init>, _16BitCommand.<init>, ... |
| `src/Vixen.Core/Commands/32BitCommand.cs` | value, value, CommandValue, _32BitCommand.<init>, value, ... |
| `src/Vixen.Core/Commands/64BitCommand.cs` | value, value, _64BitCommand.<init>, _64BitCommand.<init>, _64BitCommand, ... |
| `src/Vixen.Core/Commands/8BitCommand.cs` | value, value, _8BitCommand.<init>, value, _8BitCommand.<init>, ... |
| `src/Vixen.Core/Commands/ColorCommand.cs` | value, CommandValue, ColorCommand, ColorCommand.<init> |
| `src/Vixen.Core/Commands/CommandExtensions.cs` | command, commandOther, Max, CommandExtensions |
| `src/Vixen.Core/Commands/ICommand.cs` | CommandValue, ICommand |
| `src/Vixen.Core/Commands/Named8BitCommand.cs` | Named8BitCommand.<init>, T, RangeMaximum, value, value, ... |
| `src/Vixen.Core/Commands/StringCommand.cs` | value, StringCommand, CommandValue, StringCommand.<init> |
| `src/Vixen.Core/Commands/UnknownValueCommand.cs` | UnknownValueCommand, value, CommandValue, UnknownValueCommand.<init> |
| `src/Vixen.Core/Data/Combinator/Color/NaiveColorCombinator.cs` | _MergeColorNaively, NaiveColorCombinator, Handle, obj, value2, ... |
| `src/Vixen.Core/Data/Combinator/Combinator.cs` | T, obj, Combine, obj, obj, ... |
| `src/Vixen.Core/Data/Combinator/_16Bit/16BitAverageCombinator.cs` | _16BitAverageCombinator, Handle, obj |
| `src/Vixen.Core/Data/Combinator/_16Bit/16BitHighestWinsCombinator.cs` | obj, obj, Handle, obj, Handle, ... |
| `src/Vixen.Core/Data/Combinator/_32Bit/32BitHighestWinsCombinator.cs` | Handle, obj, obj, Handle, Handle, ... |
| `src/Vixen.Core/Data/Combinator/_64Bit/64BitHighestWinsCombinator.cs` | Handle, obj, obj, obj, Handle, ... |
| `src/Vixen.Core/Data/Combinator/_8Bit/8BitAverageCombinator.cs` | Handle, obj, _8BitAverageCombinator |
| `src/Vixen.Core/Data/Combinator/_8Bit/8BitHighestWinsCombinator.cs` | Handle, Handle, obj, Handle, _8BitHighestWinsCombinator, ... |
| `src/Vixen.Core/Data/Evaluator/16BitEvaluator.cs` | obj, Handle, Handle, obj, _16BitEvaluator, ... |
| `src/Vixen.Core/Data/Evaluator/32BitEvaluator.cs` | Handle, _32BitEvaluator, Handle, obj, obj, ... |
| `src/Vixen.Core/Data/Evaluator/64BitEvaluator.cs` | Handle, obj, Handle, _64BitEvaluator, obj, ... |
| `src/Vixen.Core/Data/Evaluator/8BitEvaluator.cs` | Handle, obj, Handle, obj, obj, ... |
| `src/Vixen.Core/Data/Evaluator/ColorEvaluator.cs` | Handle, obj, obj, Handle, Handle, ... |
| `src/Vixen.Core/Data/Evaluator/CommandLookup8BitEvaluator.cs` | CommandLookup8BitEvaluator, CommandLookup8BitEvaluator.<init>, CommandLookup |
| `src/Vixen.Core/Data/Evaluator/Evaluator.cs` | EvaluatorValue, Handle, obj, Handle, Handle, ... |
| `src/Vixen.Core/Data/Flow/CommandDataFlowData.cs` | CommandDataFlowData.<init>, CommandDataFlowData, command, Value |
| `src/Vixen.Core/Data/Flow/CommandsDataFlowData.cs` | CommandsDataFlowData.<init>, commands |
| `src/Vixen.Core/Data/Flow/ElementDataFlowAdapter.cs` | _outputs, Outputs |
| `src/Vixen.Core/Data/Flow/ElementDataFlowOutputAdapter.cs` | Name, ElementDataFlowOutputAdapter, ElementDataFlowOutputAdapter.<init>, _element, Data, ... |
| `src/Vixen.Core/Data/Flow/IDataFlowData.cs` | IDataFlowData, Value |
| `src/Vixen.Core/Data/Flow/IDataFlowOutput.cs` | IDataFlowOutput, Name, Data |
| `src/Vixen.Core/Data/Flow/IntentDataFlowData.cs` | IntentDataFlowData.<init>, IntentDataFlowData, Value, intentState |
| `src/Vixen.Core/Data/Flow/IntentsDataFlowData.cs` | intentStates, Value, IntentsDataFlowData, IntentsDataFlowData.<init> |
| `src/Vixen.Core/Data/Policy/ControllerDataPolicy.cs` | ControllerDataPolicy |
| `src/Vixen.Core/Data/Policy/DataFlowDataDispatchingDataPolicy.cs` | obj, Handle, _combinator, commands, _GetCombinator, ... |
| `src/Vixen.Core/Data/StateCombinator/LayeredStateCombinator.cs` | states, _combinedMixingColor, lowLayer, LayeredStateCombinator, obj, ... |
| `src/Vixen.Core/Data/StateCombinator/StateCombinator.cs` | obj, obj, Handle, obj, Combine, ... |
| `src/Vixen.Core/Data/Value/CommandValue.cs` | CommandValue, Command, command, CommandValue.<init> |
| `src/Vixen.Core/Data/Value/IIntentDataType.cs` | IIntentDataType |
| `src/Vixen.Core/Data/Value/IntensityValue.cs` | Intensity, IntensityValue.<init>, IntensityValue, intensity, _intensity |
| `src/Vixen.Core/Data/Value/LightingValue.cs` | LightingValue.<init>, Intensity, color, LightingValue.<init>, LightingValue.<init>, ... |
| `src/Vixen.Core/Data/Value/RGBValue.cs` | RGBValue.<init>, R, color, B, RGBValue, ... |
| `src/Vixen.Core/Data/Value/RangeValue.cs` | TagType, RangeValue.<init>, rangeValue, Tag, label, ... |
| `src/Vixen.Core/Export/Export.cs` | outputStates, UpdateState |
| `src/Vixen.Core/Export/ExportCommandHandler.cs` | obj, Value, Handle, ExportCommandHandler, Reset |
| `src/Vixen.Core/Instrumentation/IInstrumentation.cs` | ValueNames, Values, IInstrumentation |
| `src/Vixen.Core/Instrumentation/IInstrumentationValue.cs` | Maximum, Reset, Minimum, Value, IInstrumentationValue, ... |
| `src/Vixen.Core/Intent/LightingIntent.cs` | LightingIntent.<init>, endValue, startValue, timeSpan |
| `src/Vixen.Core/Intent/RGBIntent.cs` | startValue, RGBIntent.<init>, timeSpan, RGBIntent, endValue |
| `src/Vixen.Core/Intent/RangeIntent.cs` | startValue, timeSpan, endValue, RangeIntent.<init> |
| `src/Vixen.Core/Intent/StaticIntent.cs` | CreateIntentState, TypeOfValue, TimeSpan, _intentState, Value, ... |
| `src/Vixen.Core/Intent/StaticIntentState.cs` | StaticIntentState, GetValue, value, GetValue, StaticIntentState.<init>, ... |
| `src/Vixen.Core/Interpolator/LightingValueInterpolator.cs` | percent, InterpolateValue, startValue, endValue |
| `src/Vixen.Core/Interpolator/RGBValueInterpolator.cs` | percent, endValue, startValue, InterpolateValue |
| `src/Vixen.Core/Interpolator/RangeValueInterpolator.cs` | startValue, endValue, InterpolateValue, percent |
| `src/Vixen.Core/Interpolator/StaticLightingValueInterpolator.cs` | percent, InterpolateValue, startValue, endValue |
| `src/Vixen.Core/Sys/Dispatch/CommandDispatch.cs` | obj, Handle, Handle, Handle, Handle, ... |
| `src/Vixen.Core/Sys/Dispatch/DataFlowDataDispatch.cs` | Handle, obj, Handle, obj, obj, ... |
| `src/Vixen.Core/Sys/Dispatch/IAnyCombinatorHandler.cs` | IAnyCombinatorHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyCommandHandler.cs` | IAnyCommandHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyDataFlowDataHandler.cs` | IAnyDataFlowDataHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyEvaluatorHandler.cs` | IAnyEvaluatorHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyIntentHandler.cs` | IAnyIntentHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyIntentSegmentHandler.cs` | IAnyIntentSegmentHandler |
| `src/Vixen.Core/Sys/Dispatch/IAnyIntentStateHandler.cs` | IAnyIntentStateHandler |
| `src/Vixen.Core/Sys/Dispatch/IntentDispatch.cs` | obj, obj, Handle, obj, Handle, ... |
| `src/Vixen.Core/Sys/Dispatch/IntentSegmentDispatch.cs` | obj, IntentSegmentDispatch, Handle, obj, Handle, ... |
| `src/Vixen.Core/Sys/Dispatch/IntentStateDispatch.cs` | obj, obj, obj, Handle, Handle, ... |
| `src/Vixen.Core/Sys/Dispatchable.cs` | IHandler, Dispatch, Handle, Dispatch, Dispatchable, ... |
| `src/Vixen.Core/Sys/ICombinator.cs` | commands, Combine, ICombinator |
| `src/Vixen.Core/Sys/IEvaluator.cs` | intentState, Evaluate, IEvaluator |
| `src/Vixen.Core/Sys/IIntentState.cs` | GetValue, IIntentState, Layer |
| `src/Vixen.Core/Sys/IIntentStates.cs` | AsList |
| `src/Vixen.Core/Sys/IStateCombinator.cs` | states, IStateCombinator, Combine |
| `src/Vixen.Core/Sys/Instrumentation/Instrumentation.cs` | Instrumentation.<init>, GetValue, Values, name, Instrumentation, ... |
| `src/Vixen.Core/Sys/IntentNode.cs` | intentRelativeTime, CreateIntentState, layer |
| `src/Vixen.Core/Sys/IntentStateList.cs` | AsList |
| `src/Vixen.Core/Sys/Output/OutputController.cs` | commands |
| `src/Vixen.Core/Sys/OutputIntentStateList.cs` | OutputIntentStateList.<init>, OutputIntentStateList, intentStates |
| `src/Vixen.Core/Sys/OutputStateList.cs` | id, GetCommandForOutput |
| `src/Vixen.Core/Sys/OutputStateListAggregator.cs` | id, GetCommandsForOutput |
| `src/Vixen.Modules/Controller/DDP/DataPolicy.cs` | GetEvaluator, GetCombinator, DataPolicy |
| `src/Vixen.Modules/Controller/DMXUsbPro/DataPolicy.cs` | GetCombinator, DataPolicy, GetEvaluator |
| `src/Vixen.Modules/Controller/DMXUsbPro/DmxUsbProSender.cs` | SendDmxPacket, outputStates |
| `src/Vixen.Modules/Controller/DMXUsbPro/Module.cs` | chainIndex, outputStates, UpdateState |
| `src/Vixen.Modules/Controller/DebugController/DebugController.cs` | GetEvaluator, GetCombinator, DataPolicy |
| `src/Vixen.Modules/Controller/DebugController/DebugControllerOutputForm.cs` | outputStates, UpdateState |
| `src/Vixen.Modules/Controller/DummyLighting/CommandHandler.cs` | obj, Handle, obj, ColorValue, CommandHandler, ... |
| `src/Vixen.Modules/Controller/DummyLighting/DummyLightingOutputForm.cs` | UpdateState, fps, outputStates |
| `src/Vixen.Modules/Controller/DummyLighting/MonochromeDataPolicy.cs` | MonochromeDataPolicy, GetEvaluator, GetCombinator |
| `src/Vixen.Modules/Controller/DummyLighting/OneChannelColorDataPolicy.cs` | GetEvaluator, OneChannelColorDataPolicy, GetCombinator |
| `src/Vixen.Modules/Controller/DummyLighting/ThreeChannelColorDataPolicy.cs` | GetCombinator, GetEvaluator, ThreeChannelColorDataPolicy |
| `src/Vixen.Modules/Controller/E131/DataPolicy.cs` | GetCombinator, DataPolicy, GetEvaluator |
| `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIOCommandHandler.cs` | obj, Value, Handle, ElexolEtherIOCommandHandler |
| `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIODataPolicy.cs` | GetCombinator, ElexolEtherIODataPolicy, GetEvaluator |
| `src/Vixen.Modules/Controller/GenericSerial/CommandHandler.cs` | obj, Reset, Value, Handle, CommandHandler |
| `src/Vixen.Modules/Controller/GenericSerial/DataPolicy.cs` | GetCombinator, DataPolicy, GetEvaluator |
| `src/Vixen.Modules/Controller/GenericSerial/Module.cs` | chainIndex, outputStates, UpdateState |
| `src/Vixen.Modules/Controller/LauncherController/CommandHandler.cs` | Handle, Value, Reset, CommandHandler, obj |
| `src/Vixen.Modules/Controller/LauncherController/DataPolicy.cs` | GetCombinator, DataPolicy, GetEvaluator |
| `src/Vixen.Modules/Controller/OpenDMX/DataPolicy.cs` | DataPolicy, GetEvaluator, GetCombinator |
| `src/Vixen.Modules/Controller/OpenDMX/OpenDMX.cs` | outputStates, UpdateData |
| `src/Vixen.Modules/Controller/OpenDMX/OpenDMXInstance.cs` | UpdateState, chainInex, outputStates |
| `src/Vixen.Modules/Controller/RDSController/CommandHandler.cs` | Reset, CommandHandler, obj, Handle, Value |
| `src/Vixen.Modules/Controller/RDSController/DataPolicy.cs` | GetCombinator, DataPolicy, GetEvaluator |
| `src/Vixen.Modules/Controller/Renard/CommandHandler.cs` | Handle, Value, CommandHandler, obj |
| `src/Vixen.Modules/Controller/Renard/RenardDataPolicy.cs` | GetCombinator, GetEvaluator, RenardDataPolicy |
| `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownFilter.cs` | _intentValue, intentValue, Filter, intent, Handle, ... |
| `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownOutput.cs` | _configuration, Handle, highByte, _commandsData, intent, ... |
| `src/Vixen.Modules/OutputFilter/CoarseFineBreakdown/CoarseFineBreakdownOutputConfiguration.cs` | RestingValue, RestingCoarseValue, EnableDefaultValueMapping, RestingFineValue, CoarseFineBreakdownOutputConfiguration |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownFilter.cs` | ColorBreakdownFilter, Tolerance, intentValue, obj, ColorBreakdownFilter.<init>, ... |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/ColorBreakdownMixingFilterBase.cs` | intentValue, GetIntensityForState, GetMaxProportionFunc, obj, Handle, ... |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/IBreakdownFilter.cs` | intentValue, IBreakdownFilter, GetIntensityForState |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/Outputs/ColorBreakdownOutputBase.cs` | ColorBreakdownOutputBase.<init>, rgbToRGBWConverter, intensity, breakdownItem, Name, ... |
| `src/Vixen.Modules/OutputFilter/ColorBreakdown/Outputs/_8BitColorBreakdownOutput.cs` | mixColors, _8BitColorBreakdownOutput, rgbToRGBWConverter, intensity, _8BitColorBreakdownOutput.<init>, ... |
| `src/Vixen.Modules/OutputFilter/ColorWheelFilter/Filters/ColorWheelFilter.cs` | ConvertRGBToIndexCommand, Handle, ConvertColorIntentsIntoColorWheel, Handle, obj, ... |
| `src/Vixen.Modules/OutputFilter/DimmingCurve/DimmingCurveModule.cs` | DimmingCurveFilter.<init>, _curve, intentValue, obj, Handle, ... |
| `src/Vixen.Modules/OutputFilter/DimmingFilter/DimmingFilterModule.cs` | CreateOutputInternal |
| `src/Vixen.Modules/OutputFilter/DimmingFilter/Filters/DimmingFilter.cs` | DimmingFilter, ConvertRGBToDimIntent, ConvertIntensityToDimIntent, ConvertDiscreteIntentToDimIntent, obj, ... |
| `src/Vixen.Modules/OutputFilter/DimmingFilter/Outputs/DimmingFilterOutput.cs` | tag, _convertColorIntensityIntoDimIntents, ConfigureFilter, convertColorIntensityIntoDimIntents, DimmingFilterOutput, ... |
| `src/Vixen.Modules/OutputFilter/PrismFilter/Filter/PrismFilter.cs` | Handle, AssociatedFunctionName, OpenPrismIndexValue, intent, intent, ... |
| `src/Vixen.Modules/OutputFilter/PrismFilter/Output/PrismFilterOutput.cs` | PrismFilterOutput.<init>, CreateCloseShutterCommand, tag, convertPrismIntentsIntoOpenPrismIntents, IntentData, ... |
| `src/Vixen.Modules/OutputFilter/ShutterFilter/Filter/ShutterFilter.cs` | obj, Handle, ShutterFilter, Handle, Handle, ... |
| `src/Vixen.Modules/OutputFilter/ShutterFilter/Output/ShuttterFilterOutput.cs` | ShutterFilterOutput, _openShutterIndexValue, closeShutterIndexValue, convertColorIntoShutterIntents, intentData, ... |
| `src/Vixen.Modules/OutputFilter/ShutterFilter/ShutterFilterModule.cs` | CreateOutputInternal |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Filters/ITaggedFilter.cs` | Tag, Filter, ITaggedFilter, intentValue |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Filters/TaggedFilter.cs` | TaggedFilter, intent, intent, Handle, Handle |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Filters/TaggedFilterBase.cs` | TaggedFilterBase, Filter, IntentValue, intent, Tag |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Outputs/ITaggedFilterOutput.cs` | ITaggedFilterOutput, data, ProcessInputData |
| `src/Vixen.Modules/OutputFilter/TaggedFilter/Outputs/TaggedFilterOutputBase.cs` | Data, TFilter, data, handledIntent, IsIntentApplicable, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/DisplayItem.cs` | state, Handle, state, Handle |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/FullColorIntentHandler.cs` | Handle, Handle, _color, FullColorIntentHandler, obj, ... |
| `src/Vixen.Modules/Preview/VixenPreview/Shapes/PreviewPixel.cs` | states, GetFullColor |
| `src/Vixen.Tests/Data/Value/RGBValueTests.cs` | GetGrayscaleLevel_Red_Returns76, GetGrayscaleLevel_Black_Returns0, GetGrayscaleLevel_White_Returns255, RGBValueTests |

## Connected Communities

- **Vixen.Core · GetStateAt** (4 cross-edges)
- **Effect/Fireworks +11 dirs** (1 cross-edges)
- **Effect/Meteors +30 dirs** (1 cross-edges)
- **Editor/TimedSequenceEditor +68 dirs** (1 cross-edges)
- **Vixen.Core/Sys · AddDataPaths** (1 cross-edges)
- **VixenPreview/Shapes +22 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-644")
explore(operation:"context", task:"understand Sys/Dispatch +47 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
