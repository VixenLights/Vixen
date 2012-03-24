using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys {
	public class EffectIntents : Dictionary<Guid, List<IntentNode>> {
		public EffectIntents() {
		}

		public EffectIntents(IDictionary<Guid, List<IntentNode>> data)
			: base(data) {
		}

		public IEnumerable<Guid> ChannelIds {
			get { return Keys; }
		}

		//public void AddIntentForChannel(Guid channelId, params IntentNode[] data) {
		//    AddIntentsForChannel(channelId, data);
		//}
		public void AddIntentForChannel(Guid channelId, IIntent intent, TimeSpan startTime) {
			_AddIntentForChannel(channelId, new IntentNode(intent, startTime));
		}

		//public void AddIntentsForChannel(Guid channelId, IEnumerable<IntentNode> data) {
		//    if(ContainsKey(channelId)) {
		//        this[channelId] = this[channelId].Concat(data).ToArray();
		//    } else {
		//        this[channelId] = data.ToArray();
		//    }
		//}

		private void _AddIntentForChannel(Guid channelId, IntentNode intentNode) {
			if(ContainsKey(channelId)) {
				this[channelId].Add(intentNode);
			} else {
				this[channelId] = new List<IntentNode> { intentNode };
			}
		}

		public void _AddIntentsForChannel(Guid channelId, IEnumerable<IntentNode> intentNodes) {
			if(ContainsKey(channelId)) {
				this[channelId].AddRange(intentNodes);
			} else {
				this[channelId] = new List<IntentNode>(intentNodes);
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
				x => x.Value.Where(y => !(y.StartTime >= endTime || y.EndTime < startTime)).ToList()));
		}
	}
}
