using System;
using Vixen.Module;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Factory {
	public class ControllerFactory : IOutputDeviceFactory {
		public IOutputDevice CreateDevice(Guid moduleId, string name) {
			return CreateDevice(Guid.NewGuid(), moduleId, name);
		}

		public IOutputDevice CreateDevice(Guid id, Guid moduleId, string name) {
			IHasOutputs<CommandOutput> outputs = new OutputCollection<CommandOutput>();
			IModuleDataRetriever dataRetriever = new ModuleInstanceDataRetriever(VixenSystem.ModuleStore.InstanceData);
			IOutputModuleConsumer outputModuleConsumer = new OutputModuleConsumer(moduleId, dataRetriever);
			IOutputMediator<CommandOutput> outputMediator = new OutputMediator<CommandOutput>(outputs, (IUpdatableOutputCount)outputModuleConsumer.Module);
			IHardware executionControl = new BasicOutputModuleExecutionControl((IOutputModule)outputModuleConsumer.Module);
			return new OutputController(id, name, outputMediator, executionControl, outputModuleConsumer);
		}
	}
}
