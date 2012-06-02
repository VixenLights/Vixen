using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public class ChannelOutputPatch : HashSet<ControllerReference> {
		public ChannelOutputPatch(Guid channelId) {
			ChannelId = channelId;
		}

		public ChannelOutputPatch(Guid channelId, IEnumerable<ControllerReference> controllerReferences)
			: this(channelId) {
			this.AddRange(controllerReferences);
		}

		public Guid ChannelId { get; private set; }

		public void Remove(Guid controllerId) {
			RemoveWhere(x => x.ControllerId == controllerId);
		}
	}
}
