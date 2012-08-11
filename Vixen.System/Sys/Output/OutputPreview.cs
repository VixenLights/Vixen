using System;
using System.Linq;
using Vixen.Commands;
using Vixen.Data.Flow;
using Vixen.Module.Preview;

namespace Vixen.Sys.Output {
	public class OutputPreview : ModuleBasedOutputDevice<IPreviewModuleInstance> {
		public OutputPreview(string name, Guid moduleId)
			: this(Guid.NewGuid(), name, moduleId) {
		}

		public OutputPreview(Guid id, string name, Guid moduleId)
			: base(id, name, moduleId) {
		}

		protected override IPreviewModuleInstance GetModule(Guid moduleId) {
			IPreviewModuleInstance module = Modules.ModuleManagement.GetPreview(moduleId);
			ResetDataPolicy(module);
			return module;
		}

		protected override void UpdateState() {
			if(Module != null && DataPolicy != null) {
				ChannelCommands channelCommands = new ChannelCommands(VixenSystem.Channels.ToDictionary(x => x.Id, x => DataPolicy.GenerateCommand(new IntentsDataFlowData(x.State))));
				Module.UpdateState(channelCommands);
			}
		}

		public IDataPolicy DataPolicy { get; set; }

		public void ResetDataPolicy(IPreviewModuleInstance module) {
			if(module != null) {
				DataPolicy = module.DataPolicy;
			}
		}
	}
}
