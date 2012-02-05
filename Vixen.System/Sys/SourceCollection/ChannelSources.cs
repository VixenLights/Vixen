using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Commands;
using Vixen.Sys.Output;

namespace Vixen.Sys.SourceCollection {
	// Pulling everything dynamically so there isn't any maintenance when channels or 
	// patching changes.
	class ChannelPatch : IOutputSourceCollection {
		public OutputSources GetOutputSources(Guid controllerId, int outputIndex) {
			OutputSources outputSources = new OutputSources(outputIndex);

			// Get patches with references to the output.
			IEnumerable<IStateSource<Command>> patchReferences = _GetOutputSources(controllerId, outputIndex);
			// Get a collection of { Patch, OutputIndex } objects to iterate of references
			// to this controller.
			outputSources.Sources.AddRange(patchReferences);

			return outputSources;
		}


		public IEnumerable<Guid> Controllers {
			get { return VixenSystem.Controllers.Select(x => x.Id); }
		}

		public IEnumerable<OutputSources> GetControllerSources(Guid controllerId) {
			OutputController controller = (OutputController)VixenSystem.Controllers.Get(controllerId);
			if(controller != null) {
				for(int i = 0; i < controller.OutputCount; i++) {
					yield return GetOutputSources(controllerId, i);
					//OutputSources sourceCollection = new OutputSources(i);
					//IEnumerable<IOutputStateSource> sources = _GetOutputSources(controller.Id, i);
					//sourceCollection.Sources.AddRange(sources);
					//yield return sourceCollection;
				}
			}
		}

		private IEnumerable<IStateSource<Command>> _GetOutputSources(Guid controllerId, int outputIndex) {
			//return VixenSystem.Channels.Select(x => x.Patch).Where(x => x.ControllerReferences.Any(y => y.ControllerId == controllerId && y.OutputIndex == outputIndex));
			IEnumerable<Guid> channelIds = VixenSystem.ChannelPatching.Where(x => x.ControllerReferences.Any(y => y.ControllerId == controllerId && y.OutputIndex == outputIndex)).Select(x => x.ChannelId);
			return channelIds.Select(VixenSystem.Channels.GetChannel);
		}
	}
}
