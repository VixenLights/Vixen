using Vixen.Data.Flow;

namespace Vixen.Sys {
	class DataFlowInitializer {
		private SystemConfig _systemConfig;

		public DataFlowInitializer(SystemConfig systemConfig) {
			_systemConfig = systemConfig;
		}

		public void Run() {
			//_AddControllerOutputs();
			//_AddChannels();
			_AddOutputFilters();
			_BuildRelationships();
		}

		//private void _AddControllerOutputs() {
		//    foreach(OutputController controller in _systemConfig.Controllers.OfType<OutputController>()) {
		//        foreach(CommandOutput output in controller.Outputs) {
		//            VixenSystem.DataFlow.AddComponent(new CommandOutputDataFlowAdapter(output));
		//        }
		//    }
		//    foreach(SmartOutputController controller in _systemConfig.Controllers.OfType<SmartOutputController>()) {
		//        foreach(IntentOutput output in controller.Outputs) {
		//            VixenSystem.DataFlow.AddComponent(new IntentOutputDataFlowAdapter(output));
		//        }
		//    }
		//}

		//private void _AddChannels() {
		//    var channels = _systemConfig.Channels.Select(x => new ChannelDataFlowAdapter(x));
		//    foreach(var channelComponent in channels) {
		//        VixenSystem.DataFlow.AddComponent(channelComponent);
		//    }
		//}

		// Output filters are different because they are the only participants that *are*
		// data flow components.  That's their only participation in the system.  Instead of
		// filters being added to FilterManager, a filter component is added to DataFlowManager
		// and FilterManager responds to that.
		// The other participants -- channels and outputs -- have other primary roles and
		// participate only through adapters.  As a result, instead of their managers having to
		// play second fiddle to the data flow mechanism, the data flow mechanism plays second
		// fiddle for those participants.
		private void _AddOutputFilters() {
			var filters = _systemConfig.Filters;
			foreach(var filterComponent in filters) {
				VixenSystem.DataFlow.AddComponent(filterComponent);
			}
		}

		private void _BuildRelationships() {
			foreach(var dataFlowPatch in _systemConfig.DataFlow) {
				IDataFlowComponent childComponent = VixenSystem.DataFlow.GetComponent(dataFlowPatch.ComponentId);
				IDataFlowComponent sourceComponent = VixenSystem.DataFlow.GetComponent(dataFlowPatch.SourceComponentId);
				if(childComponent != null && dataFlowPatch.SourceComponentOutputIndex >= 0 && dataFlowPatch.SourceComponentOutputIndex < sourceComponent.Outputs.Length) {
					VixenSystem.DataFlow.SetComponentSource(childComponent, sourceComponent, dataFlowPatch.SourceComponentOutputIndex);
				}
			}
		}
	}
}
