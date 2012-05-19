using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public class ChannelIntents : Dictionary<Guid,IIntentNode> {
		// For better storytelling.
		public IEnumerable<Guid> ChannelIds {
			get { return Keys; }
		}

		public void AddIntentNodeToChannel(Guid channelId, IIntentNode intentNode) {
			if(intentNode == null) return;

			if(!ContainsKey(channelId)) {
				_AddRootIntentNode(channelId, intentNode);
			}
		}

		public void AddIntentNodesToChannels(ChannelIntents channelIntents) {
			foreach(Guid channelId in channelIntents.ChannelIds) {
				IIntentNode intentNode = channelIntents[channelId];
				AddIntentNodeToChannel(channelId, intentNode);
			}
		}

		private void _AddRootIntentNode(Guid channelId, IIntentNode intentNode) {
			this[channelId] = intentNode;
		}
	}
}
