---
name: gortex-sys-output-28-dirs
description: "Work in the Sys/Output +28 dirs area — 595 symbols across 66 files (91% cohesion)"
---

# Sys/Output +28 dirs

595 symbols | 66 files | 91% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Common/BaseSequence/SequenceExecutor.cs`
- `src/Vixen.Core/Cache/Sequence/FixedIntervalManualTiming.cs`
- `src/Vixen.Core/Execution/Context/ContextBase.cs`
- `src/Vixen.Core/Execution/IExecutionControl.cs`
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
- `src/Vixen.Core/Sys/Output/BasicOutputModuleExecutionControl.cs`
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
- `src/Vixen.Modules/Controller/E131/E131OutputPlugin.cs`
- `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIOCommandHandler.cs`
- `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIOModule.cs`
- `src/Vixen.Modules/Controller/GenericSerial/Module.cs`
- `src/Vixen.Modules/Controller/LauncherController/Module.cs`
- `src/Vixen.Modules/Controller/OpenDMX/OpenDMX.cs`
- `src/Vixen.Modules/Controller/OpenDMX/OpenDMXInstance.cs`
- `src/Vixen.Modules/Controller/Renard/CommandHandler.cs`
- `src/Vixen.Modules/Controller/Renard/Module.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectTimeEditor.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkCollectionViewModel.cs`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs`
- `src/Vixen.Modules/Media/Audio/Audio.cs`
- `src/Vixen.Modules/Media/Audio/AudioUtilities.cs`
- `src/Vixen.Modules/Timing/Generic/Module.cs`
- `src/Vixen.Tests/Sequencer/SequenceExecutorLifecycleTests.cs`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Common/BaseSequence/SequenceExecutor.cs` | Start, e, _loopPlay, Stop, OnSequenceReStarted, ... |
| `src/Vixen.Core/Cache/Sequence/FixedIntervalManualTiming.cs` | FixedIntervalManualTiming.<init>, Resume, Interval, Position, Start, ... |
| `src/Vixen.Core/Execution/Context/ContextBase.cs` | _currentTime, ResetElementStates, _UpdateCurrentEffectList, _SequenceTiming, IsRunning, ... |
| `src/Vixen.Core/Execution/IExecutionControl.cs` | Start, IExecutionControl, Stop, Resume, Pause |
| `src/Vixen.Core/Execution/ProgramExecutor.cs` | Resume, OnProgramStarted, Dispose, Stop, disposing, ... |
| `src/Vixen.Core/Module/Controller/ControllerModuleInstanceBase.cs` | _dataPolicyFactory, GetHashCode, x, Equals, other, ... |
| `src/Vixen.Core/Module/Controller/IController.cs` | outputStates, SupportsNetwork, IController, UpdateState, GetNetworkConfiguration, ... |
| `src/Vixen.Core/Module/Controller/IControllerModuleInstance.cs` | IControllerModuleInstance |
| `src/Vixen.Core/Module/Input/IInput.cs` | IInput, DeviceName, Inputs |
| `src/Vixen.Core/Module/Input/IInputInput.cs` | IInputInput, Name, Value |
| `src/Vixen.Core/Module/Input/IInputModuleInstance.cs` | IInputModuleInstance |
| `src/Vixen.Core/Module/Input/InputEffectMap.cs` | inputModule, nodes, effectModule, inputModule, IsMappedTo, ... |
| `src/Vixen.Core/Module/Input/InputModuleInstanceBase.cs` | obj, DoPause, DeviceName, other, Pause, ... |
| `src/Vixen.Core/Module/Input/InputValueChangedEventArgs.cs` | InputValueChangedEventArgs, InputValueChangedEventArgs.<init>, inputModule, Input, input, ... |
| `src/Vixen.Core/Module/Media/IMedia.cs` | TimingSource, IMedia, MediaFilePath |
| `src/Vixen.Core/Module/Media/MediaModuleInstanceBase.cs` | Resume, Stop, Start |
| `src/Vixen.Core/Module/Preview/IPreview.cs` | UpdateState, PlayerEnded, IPreview, PlayerStarted |
| `src/Vixen.Core/Module/Preview/IPreviewModuleInstance.cs` | Name, IPreviewModuleInstance |
| `src/Vixen.Core/Module/Preview/PreviewModuleInstanceBase.cs` | PlayerActivatedImpl, Equals, Equals, x, y, ... |
| `src/Vixen.Core/Module/Service/IService.cs` | IService |
| `src/Vixen.Core/Module/SmartController/ISmartController.cs` | UpdateState, ISmartController, outputStates |
| `src/Vixen.Core/Module/Timing/TimingModuleInstanceBase.cs` | Start, Resume, Stop |
| `src/Vixen.Core/Module/Trigger/ITrigger.cs` | TriggerInputs, ITrigger, UpdateState |
| `src/Vixen.Core/Module/Trigger/ITriggerInput.cs` | Type, ITriggerInput, Value, Name, Id |
| `src/Vixen.Core/Module/Trigger/ITriggerModuleInstance.cs` | ITriggerModuleInstance |
| `src/Vixen.Core/Module/Trigger/TriggerModuleInstanceBase.cs` | obj, _TriggerSet, _pause, Pause, _StateUpdate, ... |
| `src/Vixen.Core/Module/Trigger/TriggerSetEventArgs.cs` | TriggerSetEventArgs, Trigger, trigger, TriggerSetEventArgs.<init> |
| `src/Vixen.Core/Sys/Execution.cs` | SystemTime, contextName, Shutdown, sequence, QueueSequence |
| `src/Vixen.Core/Sys/IHardware.cs` | IHardware |
| `src/Vixen.Core/Sys/IHardwareModule.cs` | IHardwareModule |
| `src/Vixen.Core/Sys/IHasSetup.cs` | HasSetup, IHasSetup |
| `src/Vixen.Core/Sys/Output/BasicOutputModuleExecutionControl.cs` | _Start, IsRunning, Start, BasicOutputModuleExecutionControl.<init>, Stop, ... |
| `src/Vixen.Core/Sys/Output/IOutputModule.cs` | IOutputModule |
| `src/Vixen.Core/Sys/Output/IOutputModuleConsumer.cs` | IOutputModuleConsumer, UpdateSignaler, NameOutputs, SupportsNamedOutputs, UpdateInterval, ... |
| `src/Vixen.Core/Sys/Output/IOutputter.cs` | UpdateInterval, IOutputter, UpdateSignaler, SupportsNamedOutputs, NameOutputs |
| `src/Vixen.Core/Sys/Output/IUpdatableOutputCount.cs` | OutputCount, IUpdatableOutputCount, OutputLimit |
| `src/Vixen.Core/Sys/Output/OutputController.cs` | Start, Resume, eventArgs, outputMediator, name, ... |
| `src/Vixen.Core/Sys/Output/OutputModuleConsumer.cs` | IsPaused, UpdateInterval, UpdateSignaler, Pause, _outputModule, ... |
| `src/Vixen.Core/Sys/Output/OutputModuleInstanceBase.cs` | OutputModuleInstanceBase, IsPaused, HasSetup, Pause, Stop, ... |
| `src/Vixen.Core/Sys/Output/OutputPreview.cs` | OutputPreview.<init>, id, Stop, Start, executionControl, ... |
| `src/Vixen.Core/Sys/Output/SmartOutputController.cs` | Stop, Resume, Start |
| `src/Vixen.Core/Sys/SystemClock.cs` | Pause, Start, Speed, _time, SupportsVariableSpeeds, ... |
| `src/Vixen.Core/Sys/UIThread.cs` | Start |
| `src/Vixen.Modules/Controller/DDP/DDP.cs` | DDP_FLAGS1_REPLY, DDP_CHANNELS_PER_PACKET, _udpClient, HostInfo, OutputCount, ... |
| `src/Vixen.Modules/Controller/DMXUsbPro/DmxUsbProSender.cs` | Dispose, Stop, _serialPort, Start, DmxUsbProSender, ... |
| `src/Vixen.Modules/Controller/DMXUsbPro/Module.cs` | InitializePort, disposing, Dispose, Module, GetModuleData, ... |
| `src/Vixen.Modules/Controller/DebugController/DebugController.cs` | Start, DebugControllerModule, disposing, chainIndex, DebugControllerModule.<init>, ... |
| `src/Vixen.Modules/Controller/DebugController/DebugControllerOutputForm.Designer.cs` | DebugControllerOutputForm, chkVerbose, components, Dispose, textBoxOutput, ... |
| `src/Vixen.Modules/Controller/DebugController/DebugControllerOutputForm.cs` | chkVerbose_CheckedChanged, e, _timer, UpdateTextBox, sender, ... |
| `src/Vixen.Modules/Controller/DummyLighting/DummyLighting.cs` | outputStates, _updateCount, ModuleData, DummyLighting.<init>, _data, ... |
| `src/Vixen.Modules/Controller/E131/E131OutputPlugin.cs` | Setup, Start, Stop |
| `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIOCommandHandler.cs` | Reset |
| `src/Vixen.Modules/Controller/ElexolEtherIO/ElexolEtherIOModule.cs` | initModule, _remotePort, Stop, ElexolEtherIOModule.<init>, chainIndex, ... |
| `src/Vixen.Modules/Controller/GenericSerial/Module.cs` | e, _headerLen, _commandHandler, RetryTimer_Elapsed, CreateSerialPortFromData, ... |
| `src/Vixen.Modules/Controller/LauncherController/Module.cs` | _lastCommandValues, _Data, Logging, Module, HasSetup, ... |
| `src/Vixen.Modules/Controller/OpenDMX/OpenDMX.cs` | device, _status, Logging, FindDeviceIndex, channel, ... |
| `src/Vixen.Modules/Controller/OpenDMX/OpenDMXInstance.cs` | Stop, VixenOpenDMXInstance, HasSetup, VixenOpenDMXInstance.<init>, _dmxPort, ... |
| `src/Vixen.Modules/Controller/Renard/CommandHandler.cs` | Reset |
| `src/Vixen.Modules/Controller/Renard/Module.cs` | bytesToWrite, _moduleData, _GetPacket, DEFAULT_WRITE_TIMEOUT, _port, ... |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/EffectTimeEditor.cs` | sender, Time_Changed, e |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkCollectionViewModel.cs` | e, nameClickTimer_Elapsed, e, sender, BeginEdit |
| `src/Vixen.Modules/Editor/TimedSequenceEditor/TimedSequenceEditorForm.cs` | Stop, Start, Resume |
| `src/Vixen.Modules/Media/Audio/Audio.cs` | Resume, Start, Stop |
| `src/Vixen.Modules/Media/Audio/AudioUtilities.cs` | StartPlayback, StopPlayback |
| `src/Vixen.Modules/Timing/Generic/Module.cs` | _stopwatch, Pause, _offset, Resume, Module, ... |
| `src/Vixen.Tests/Sequencer/SequenceExecutorLifecycleTests.cs` | Stop, Resume, Start |

## Entry Points

- `src/Vixen.Modules/Media/Audio/AudioUtilities.cs::AudioUtilities.StartPlayback`
- `src/Vixen.Modules/Editor/TimedSequenceEditor/Forms/WPF/MarksDocker/ViewModels/MarkCollectionViewModel.cs::MarkCollectionViewModel.BeginEdit`

## Connected Communities

- **Controller/E131 · SetupForm** (5 cross-edges)
- **Vixen.Common/BaseSequence +1 dirs · SequenceExecutor** (4 cross-edges)
- **Module/Preview · PreviewModuleDescriptorBase** (3 cross-edges)
- **Module/Controller +9 dirs** (3 cross-edges)
- **Controller/DDP +2 dirs** (3 cross-edges)
- **VixenPreview/Shapes +22 dirs** (3 cross-edges)
- **Module/Media +3 dirs** (3 cross-edges)
- **Module/Trigger · TriggerModuleDescriptorBase** (3 cross-edges)
- **Module/Input · InputModuleDescriptorBase** (3 cross-edges)
- **Editor/TimedSequenceEditor +46 dirs** (2 cross-edges)
- **Vixen.Core/Execution · ProgramExecutor** (2 cross-edges)
- **Controller/E131 · J1MsgBox** (2 cross-edges)
- **Vixen.Common/NShape +64 dirs** (2 cross-edges)
- **Controller/OpenDMX** (2 cross-edges)
- **Vixen.Application +48 dirs** (1 cross-edges)
- **Controller/Renard · ProtocolFormatterService** (1 cross-edges)
- **Vixen.Application/Setup +24 dirs** (1 cross-edges)
- **MarksDocker/ViewModels · MarkCollectionViewModel** (1 cross-edges)
- **Module/Service +10 dirs** (1 cross-edges)
- **Vixen.Core/Module +61 dirs** (1 cross-edges)
- **Vixen.Core · _RemovePerformanceValues** (1 cross-edges)
- **Sys/Dispatch +47 dirs** (1 cross-edges)
- **Vixen.Core/Sys +79 dirs** (1 cross-edges)
- **Vixen.Core · IContext** (1 cross-edges)
- **Vixen.Core · ContextCaching** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-1327")
explore(operation:"context", task:"understand Sys/Output +28 dirs", format:"gcx")
relations(operation:"usages", target:{symbol:"src/Vixen.Modules/Media/Audio/AudioUtilities.cs::AudioUtilities.StartPlayback"}, format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
