using System;
using Vixen.Module;
using Vixen.Module.SmartController;
using Vixen.Sys;
using Vixen.Sys.Output;

namespace Vixen.Factory {
	public class SmartControllerFactory {
		public IOutputDevice CreateDevice(Guid moduleId, string name) {
			return CreateDevice(Guid.NewGuid(), moduleId, Guid.NewGuid(), name);
		}

		public IOutputDevice CreateDevice(Guid deviceId, Guid moduleId, Guid moduleInstanceId, string name) {
			IHasOutputs<IntentOutput> outputs = new OutputCollection<IntentOutput>();
			IModuleDataRetriever dataRetriever = new ModuleInstanceDataRetriever(VixenSystem.ModuleStore.InstanceData);
			IOutputModuleConsumer<ISmartControllerModuleInstance> outputModuleConsumer = new OutputModuleConsumer<ISmartControllerModuleInstance>(moduleId, moduleInstanceId, dataRetriever);
			IOutputMediator<IntentOutput> outputMediator = new OutputMediator<IntentOutput>(outputs, outputModuleConsumer.Module);
			IHardware executionControl = new BasicOutputModuleExecutionControl(outputModuleConsumer.Module);
			return new SmartOutputController(deviceId, name, outputMediator, executionControl, outputModuleConsumer);
		}
	}
}
