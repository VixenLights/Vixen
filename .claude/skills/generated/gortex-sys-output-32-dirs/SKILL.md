---
name: gortex-sys-output-32-dirs
description: "Work in the Sys/Output +32 dirs area — 681 symbols across 73 files (91% cohesion)"
---

# Sys/Output +32 dirs

681 symbols | 73 files | 91% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Common/AudioPlayer/PlayerFactory.cs`
- `src/Vixen.Common/BaseSequence/SequenceExecutor.cs`
- `src/Vixen.Core/Cache/Sequence/FixedIntervalManualTiming.cs`
- `src/Vixen.Core/Execution/Context/ContextBase.cs`
- `src/Vixen.Core/Execution/IExecutionControl.cs`
- `src/Vixen.Core/Execution/IExecutor.cs`
- `src/Vixen.Core/Execution/ProgramExecutor.cs`
- `src/Vixen.Core/Module/Controller/ControllerModuleInstanceBase.cs`
- `src/Vixen.Core/Module/Controller/IController.cs`
- `src/Vixen.Core/Module/Controller/IControllerModuleInstance.cs`
- `src/Vixen.Core/Module/Input/IInput.cs`
- `src/Vixen.Core/Module/Input/IInputInput.cs`
- `src/Vixen.Core/Module/Input/IInputModuleInstance.cs`
- `src/Vixen.Core/Module/Input/InputEffectMap.cs`
- `src/Vixen.Core/Module/Input/InputModuleInstanceBase.cs`
- `src/Vixen.Core/Module/Input/InputValueChangedEventArgs.cs`
- `src/Vixen.Core/Module/Media/IMedia.cs`
- `src/Vixen.Core/Module/Media/MediaModuleInstanceBase.cs`
- `src/Vixen.Core/Module/Preview/IPreview.cs`
- `src/Vixen.Core/Module/Preview/IPreviewModuleInstance.cs`
- `src/Vixen.Core/Module/Preview/PreviewModuleInstanceBase.cs`
- `src/Vixen.Core/Module/Service/IService.cs`
- `src/Vixen.Core/Module/SmartController/ISmartController.cs`
- `src/Vixen.Core/Module/Timing/TimingModuleInstanceBase.cs`
- `src/Vixen.Core/Module/Trigger/ITrigger.cs`
- `src/Vixen.Core/Module/Trigger/ITriggerInput.cs`
- `src/Vixen.Core/Module/Trigger/ITriggerModuleInstance.cs`
- `src/Vixen.Core/Module/Trigger/TriggerModuleInstanceBase.cs`
- `src/Vixen.Core/Module/Trigger/TriggerSetEventArgs.cs`
- `src/Vixen.Core/Sys/Execution.cs`
- `src/Vixen.Core/Sys/IHardware.cs`
- `src/Vixen.Core/Sys/IHardwareModule.cs`
- `src/Vixen.Core/Sys/IHasSetup.cs`
- `src/Vixen.Core/Sys/IRuns.cs`
- `src/Vixen.Core/Sys/Output/BasicOutputModuleExecutionControl.cs`
- `src/Vixen.Core/Sys/Output/IOutputDeviceUpdateSignaler.cs`
- `src/Vixen.Core/Sys/Output/IOutputModule.cs`
- `src/Vixen.Core/Sys/Output/IOutputModuleConsumer.cs`
- `src/Vixen.Core/Sys/Output/IOutputter.cs`
- `src/Vixen.Core/Sys/Output/IUpdatableOutputCount.cs`
- `src/Vixen.Core/Sys/Output/OutputController.cs`
- `src/Vixen.Core/Sys/Output/OutputModuleConsumer.cs`
- `src/Vixen.Core/Sys/Output/OutputModuleInstanceBase.cs`
- `src/Vixen.Core/Sys/Output/OutputPreview.cs`
- `src/Vixen.Core/Sys/Output/SmartOutputController.cs`
- `src/Vixen.Core/Sys/SystemClock.cs`
- `src/Vixen.Core/Sys/UIThread.cs`
- `src/Vixen.Modules/Controller/DDP/DDP.cs`
- `src/Vixen.Modules/Controller/DMXUsbPro/DmxUsbProSender.cs`
- `src/Vixen.Modules/Controller/DMXUsbPro/Module.cs`
- `src/Vixen.Modules/Controller/DebugController/DebugController.cs`
- `src/Vixen.Modules/Controller/DebugController/DebugControllerOutputForm.Designer.cs`
- `src/Vixen.Modules/Controller/DebugController/DebugControllerOutputForm.cs`
- `src/Vixen.Modules/Controller/DummyLighting/DummyLighting.cs`
- `src/Vixen.Modules/Controller/E131/E131ModuleDescriptor.cs`
- `src/Vixen.Modules/Controller/E131/E131OutputPlugin.cs`
- `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIOCommandHandler.cs`
- `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIOModule.cs`
- `src/Vixen.Modules/Controller/GenericSerial/Module.cs`
- `src/Vixen.Modules/Controller/LauncherController/Module.cs`
- `src/Vixen.Modules/Controller/OpenDMX/OpenDMX.cs`
- `src/Vixen.Modules/Controller/OpenDMX/OpenDMXInstance.cs`
- `src/Vixen.Modules/Controller/RDSController/Module.cs`
- `src/Vixen.Modules/Controller/Renard/CommandHandler.cs`
- `src/Vixen.Modules/Controller/Renard/Module.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkCollectionViewModel.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`
- `src/Vixen.Modules/Media/Audio/Audio.cs`
- `src/Vixen.Modules/Media/Audio/AudioUtilities.cs`
- `src/Vixen.Modules/Timing/Generic/Module.cs`
- `src/Vixen.Tests/Core/OutputControllerOutputIndexTests.cs`
- `src/Vixen.Tests/Sequencer/SequenceExecutorLifecycleTests.cs`
- `src/Vixen.Tests/Setup/SetupPatchingTestOutputModuleConsumer.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Common/AudioPlayer/PlayerFactory.cs` | PlayerFactory, CreateNew, fileName |
| `src/Vixen.Common/BaseSequence/SequenceExecutor.cs` | Stop, Resume, Start, _loopPlay, executionGeneration, ... |
| `src/Vixen.Core/Cache/Sequence/FixedIntervalManualTiming.cs` | Resume, Start, FixedIntervalManualTiming.<init>, SupportsVariableSpeeds, Position, ... |
| `src/Vixen.Core/Execution/Context/ContextBase.cs` | _UpdateCurrentEffectList, Start, _OnStart, _SequenceTiming, currentTime, ... |
| `src/Vixen.Core/Execution/IExecutionControl.cs` | Pause, IExecutionControl, Stop, Start, Resume |
| `src/Vixen.Core/Execution/IExecutor.cs` | Name, IExecutor, TimingSource, SequenceLayers |
| `src/Vixen.Core/Execution/ProgramExecutor.cs` | Dispose, OnProgramStarted, Start, Dispose, Stop, ... |
| `src/Vixen.Core/Module/Controller/ControllerModuleInstanceBase.cs` | SupportsNetwork, GetNetworkConfiguration, y, obj, OutputCount, ... |
| `src/Vixen.Core/Module/Controller/IController.cs` | DataPolicyFactory, UpdateState, SupportsNetwork, GetNetworkConfiguration, outputStates, ... |
| `src/Vixen.Core/Module/Controller/IControllerModuleInstance.cs` | IControllerModuleInstance |
| `src/Vixen.Core/Module/Input/IInput.cs` | IInput, Inputs, DeviceName |
| `src/Vixen.Core/Module/Input/IInputInput.cs` | Value, Name, IInputInput |
| `src/Vixen.Core/Module/Input/IInputModuleInstance.cs` | IInputModuleInstance |
| `src/Vixen.Core/Module/Input/InputEffectMap.cs` | parameterIndex, inputModule, inputModule, effectModule, InputEffectMap.<init>, ... |
| `src/Vixen.Core/Module/Input/InputModuleInstanceBase.cs` | DeviceName, HasSetup, x, DoShutdown, GetHashCode, ... |
| `src/Vixen.Core/Module/Input/InputValueChangedEventArgs.cs` | InputValueChangedEventArgs.<init>, input, Input, inputModule, InputValueChangedEventArgs, ... |
| `src/Vixen.Core/Module/Media/IMedia.cs` | MediaFilePath, startTime, LoadMedia, IMedia, TimingSource |
| `src/Vixen.Core/Module/Media/MediaModuleInstanceBase.cs` | startTime, Resume, Start, Stop, LoadMedia |
| `src/Vixen.Core/Module/Preview/IPreview.cs` | PlayerStarted, UpdateState, IPreview, PlayerEnded |
| `src/Vixen.Core/Module/Preview/IPreviewModuleInstance.cs` | Name, IPreviewModuleInstance |
| `src/Vixen.Core/Module/Preview/PreviewModuleInstanceBase.cs` | Equals, PlayerDeactivatedImpl, x, Update, PlayerStarted, ... |
| `src/Vixen.Core/Module/Service/IService.cs` | IService |
| `src/Vixen.Core/Module/SmartController/ISmartController.cs` | UpdateState, outputStates, ISmartController |
| `src/Vixen.Core/Module/Timing/TimingModuleInstanceBase.cs` | Start, Stop, Resume |
| `src/Vixen.Core/Module/Trigger/ITrigger.cs` | ITrigger, UpdateState, TriggerInputs |
| `src/Vixen.Core/Module/Trigger/ITriggerInput.cs` | Name, Value, ITriggerInput, Id, Type |
| `src/Vixen.Core/Module/Trigger/ITriggerModuleInstance.cs` | ITriggerModuleInstance |
| `src/Vixen.Core/Module/Trigger/TriggerModuleInstanceBase.cs` | Equals, Stop, TriggerModuleInstanceBase, GetHashCode, y, ... |
| `src/Vixen.Core/Module/Trigger/TriggerSetEventArgs.cs` | trigger, TriggerSetEventArgs.<init>, Trigger, TriggerSetEventArgs |
| `src/Vixen.Core/Sys/Execution.cs` | contextName, SystemTime, Shutdown, QueueSequence, sequence |
| `src/Vixen.Core/Sys/IHardware.cs` | IHardware |
| `src/Vixen.Core/Sys/IHardwareModule.cs` | IHardwareModule |
| `src/Vixen.Core/Sys/IHasSetup.cs` | HasSetup, IHasSetup |
| `src/Vixen.Core/Sys/IRuns.cs` | IsRunning, IsPaused, IRuns |
| `src/Vixen.Core/Sys/Output/BasicOutputModuleExecutionControl.cs` | Resume, Pause, _Start, BasicOutputModuleExecutionControl.<init>, IsRunning, ... |
| `src/Vixen.Core/Sys/Output/IOutputDeviceUpdateSignaler.cs` | RaiseSignal, IOutputDeviceUpdateSignaler, UpdateSignal, OutputDevice |
| `src/Vixen.Core/Sys/Output/IOutputModule.cs` | IOutputModule |
| `src/Vixen.Core/Sys/Output/IOutputModuleConsumer.cs` | IOutputModuleConsumer, SupportsNamedOutputs, T, UpdateInterval, UpdateSignaler, ... |
| `src/Vixen.Core/Sys/Output/IOutputter.cs` | UpdateSignaler, NameOutputs, UpdateInterval, IOutputter, SupportsNamedOutputs |
| `src/Vixen.Core/Sys/Output/IUpdatableOutputCount.cs` | IUpdatableOutputCount, OutputCount, OutputLimit |
| `src/Vixen.Core/Sys/Output/OutputController.cs` | eventArgs, sender, name, Stop, DataPolicyFactoryChanged, ... |
| `src/Vixen.Core/Sys/Output/OutputModuleConsumer.cs` | IsPaused, OutputModuleConsumer, Resume, UpdateSignaler, SupportsNamedOutputs, ... |
| `src/Vixen.Core/Sys/Output/OutputModuleInstanceBase.cs` | UpdateInterval, IsPaused, Start, Resume, Stop, ... |
| `src/Vixen.Core/Sys/Output/OutputPreview.cs` | Start, executionControl, Stop, name, Resume, ... |
| `src/Vixen.Core/Sys/Output/SmartOutputController.cs` | Stop, Resume, Start |
| `src/Vixen.Core/Sys/SystemClock.cs` | SystemClock, Position, Pause, Speed, IsRunning, ... |
| `src/Vixen.Core/Sys/UIThread.cs` | Start |
| `src/Vixen.Modules/Controller/DDP/DDP.cs` | DDP_ID_CONFIG, DDP.<init>, _ddpPacket, LogTag, Stop, ... |
| `src/Vixen.Modules/Controller/DMXUsbPro/DmxUsbProSender.cs` | Dispose, Logging, _dmxPacketMessage, Stop, _serialPort, ... |
| `src/Vixen.Modules/Controller/DMXUsbPro/Module.cs` | Module.<init>, _dmxUsbProSender, GetModuleData, Module, Stop, ... |
| `src/Vixen.Modules/Controller/DebugController/DebugController.cs` | disposing, outputStates, Stop, DebugControllerModule, _form, ... |
| `src/Vixen.Modules/Controller/DebugController/DebugControllerOutputForm.Designer.cs` | DebugControllerOutputForm, textBoxOutput, Dispose, chkVerbose, disposing, ... |
| `src/Vixen.Modules/Controller/DebugController/DebugControllerOutputForm.cs` | Verbose, DebugControllerOutputForm.<init>, chkVerbose_CheckedChanged, sender, e, ... |
| `src/Vixen.Modules/Controller/DummyLighting/DummyLighting.cs` | _updateCount, OutputCount, Stop, IsRunning, HasSetup, ... |
| `src/Vixen.Modules/Controller/E131/E131ModuleDescriptor.cs` | TypeId, TypeName, ModuleClass, E131ModuleDescriptor, Author, ... |
| `src/Vixen.Modules/Controller/E131/E131OutputPlugin.cs` | NameOutputs, PluginInstances, E131OutputPlugin.<init>, _data, Stop, ... |
| `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIOCommandHandler.cs` | Reset |
| `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIOModule.cs` | Stop, chainIndex, Logging, _data, ModuleData, ... |
| `src/Vixen.Modules/Controller/GenericSerial/Module.cs` | _serialPort, Stop, _retryTimer, DropExistingSerialPort, Start, ... |
| `src/Vixen.Modules/Controller/LauncherController/Module.cs` | _Data, Logging, HasSetup, Module, Module.<init>, ... |
| `src/Vixen.Modules/Controller/OpenDMX/OpenDMX.cs` | StopBits2, FindDeviceIndex, Bits8, _status, channel, ... |
| `src/Vixen.Modules/Controller/OpenDMX/OpenDMXInstance.cs` | _dmxPort, VixenOpenDMXInstance.<init>, _outputCount, OutputLimit, _moduleData, ... |
| `src/Vixen.Modules/Controller/RDSController/Module.cs` | outputStates, chainIndex, Module.<init>, Logging, UpdateState, ... |
| `src/Vixen.Modules/Controller/Renard/CommandHandler.cs` | Reset |
| `src/Vixen.Modules/Controller/Renard/Module.cs` | source, Module, _CreatePort, _GetPacket, _commandHandler, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkCollectionViewModel.cs` | e, nameClickTimer_Elapsed, e, sender, BeginEdit |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` | Stop, Start, Resume |
| `src/Vixen.Modules/Media/Audio/Audio.cs` | Stop, Resume, Start, startTime, MediaDuration, ... |
| `src/Vixen.Modules/Media/Audio/AudioUtilities.cs` | StopPlayback, StartPlayback |
| `src/Vixen.Modules/Timing/Generic/Module.cs` | _stopwatch, _offset, Stop, Pause, Position, ... |
| `src/Vixen.Tests/Core/OutputControllerOutputIndexTests.cs` | Module, UpdateSignaler, HasSetup, Start, SupportsNamedOutputs, ... |
| `src/Vixen.Tests/Sequencer/SequenceExecutorLifecycleTests.cs` | Resume, Stop, Start |
| `src/Vixen.Tests/Setup/SetupPatchingTestOutputModuleConsumer.cs` | Pause, Module, IsPaused, UpdateSignaler, SupportsNamedOutputs, ... |

## Entry Points

- `src/Vixen.Modules/Media/Audio/AudioUtilities.cs::AudioUtilities.StartPlayback`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkCollectionViewModel.cs::MarkCollectionViewModel.BeginEdit`

## Connected Communities

- **Vixen.Common/NShape +69 dirs** (5 cross-edges)
- **Vixen.Common/BaseSequence +1 dirs · SequenceExecutor** (4 cross-edges)
- **Module/Controller +10 dirs** (3 cross-edges)
- **Module/Input · InputModuleDescriptorBase** (3 cross-edges)
- **Module/Trigger · TriggerModuleDescriptorBase** (3 cross-edges)
- **TimedSequenceEditor/Forms +37 dirs** (3 cross-edges)
- **Module/Preview · PreviewModuleDescriptorBase** (3 cross-edges)
- **Vixen.Common/Controls +51 dirs** (2 cross-edges)
- **Controller/E131 · Make** (2 cross-edges)
- **CustomPropEditor/Model +13 dirs** (2 cross-edges)
- **Controller/E131 · TryParseInt32** (2 cross-edges)
- **Vixen.Core/Execution · ProgramExecutor** (2 cross-edges)
- **Controller/OpenDMX · GetDeviceList** (2 cross-edges)
- **Controller/RDSController · SetupForm** (1 cross-edges)
- **VixenPreview/Shapes +20 dirs** (1 cross-edges)
- **Sys/Managers +6 dirs** (1 cross-edges)
- **Vixen.Core/Module +55 dirs** (1 cross-edges)
- **Controller/E131 · UniverseEntry** (1 cross-edges)
- **CustomPropEditor/ViewModels +9 dirs** (1 cross-edges)
- **Vixen.Core · ContextManager** (1 cross-edges)
- **Vixen.Core · _RemovePerformanceValues** (1 cross-edges)
- **Media/Audio** (1 cross-edges)
- **Controller/Renard · ProtocolFormatterService** (1 cross-edges)
- **Vixen.Core · ContextCaching** (1 cross-edges)
- **Vixen.Core/Sys +17 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-1378")
explore(operation:"context", task:"understand Sys/Output +32 dirs", format:"gcx")
relations(operation:"usages", target:{symbol:"src/Vixen.Modules/Media/Audio/AudioUtilities.cs::AudioUtilities.StartPlayback"}, format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
