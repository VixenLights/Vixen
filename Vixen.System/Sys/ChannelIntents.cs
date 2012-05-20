using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public class ChannelIntents : Dictionary<Guid,IIntentNode[]> {
		// For better storytelling.
		public IEnumerable<Guid> ChannelIds {
			get { return Keys; }
		}

		public void AddIntentNodeToChannel(Guid channelId, IIntentNode[] intentNodes) {
			if(intentNodes == null || intentNodes.Length == 0) return;

			if(!ContainsKey(channelId)) {
				_AddIntentNode(channelId, intentNodes);
			}
		}

		public void AddIntentNodesToChannels(ChannelIntents channelIntents) {
			foreach(Guid channelId in channelIntents.ChannelIds) {
				IIntentNode[] intentNodes = channelIntents[channelId];
				AddIntentNodeToChannel(channelId, intentNodes);
			}
		}

		private void _AddIntentNode(Guid channelId, IIntentNode[] intentNodes) {
			this[channelId] = intentNodes;
		}
	}
}
