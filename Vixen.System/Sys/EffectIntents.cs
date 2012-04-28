using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys {
	public class EffectIntents : Dictionary<Guid, IntentNodeCollection> {
		public EffectIntents() {
		}

		public EffectIntents(IDictionary<Guid, IntentNodeCollection> data)
			: base(data) {
		}

		public IEnumerable<Guid> ChannelIds {
			get { return Keys; }
		}

		public IntentNodeCollection GetIntentNodesForChannel(Guid channelId) {
			IntentNodeCollection intentNodeCollection;
			TryGetValue(channelId, out intentNodeCollection);
			return intentNodeCollection;
		}

		public void AddIntentForChannel(Guid channelId, IIntent intent, TimeSpan startTime) {
			_AddIntentForChannel(channelId, new IntentNode(intent, startTime));
		}

		//public void ApplyFilter(IPreFilterNode preFilterNode, TimeSpan contextAbsoluteEffectStartTime) {
		//    foreach(IntentNodeCollection intentNodes in Values) {
		//        foreach(IntentNode intentNode in intentNodes) {
		//            intentNode.ApplyFilter(preFilterNode, contextAbsoluteEffectStartTime);
		//        }
		//    }
		//}

		private void _AddIntentForChannel(Guid channelId, IntentNode intentNode) {
			if(ContainsKey(channelId)) {
				this[channelId].Add(intentNode);
			} else {
				this[channelId] = new IntentNodeCollection { intentNode };
			}
		}

		private void _AddIntentsForChannel(Guid channelId, IEnumerable<IntentNode> intentNodes) {
			if(ContainsKey(channelId)) {
				this[channelId].AddRange(intentNodes);
			} else {
				this[channelId] = new IntentNodeCollection(intentNodes);
			}
		}

		public void Add(EffectIntents other) {
			foreach(Guid channelId in other.Keys) {
				_AddIntentsForChannel(channelId, other[channelId]);
			}
		}

		static public EffectIntents Restrict(EffectIntents effectIntents, TimeSpan startTime, TimeSpan endTime) {
			return new EffectIntents(effectIntents.ToDictionary(
				x => x.Key,
				x => new IntentNodeCollection(x.Value.Where(y => !(y.StartTime >= endTime || y.EndTime < startTime)))));
		}
	}
}
