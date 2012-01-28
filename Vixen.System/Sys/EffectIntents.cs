using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys {
	public class EffectIntents : Dictionary<Guid, IntentNode[]> {
		public EffectIntents() {
		}

		public EffectIntents(IDictionary<Guid, IntentNode[]> data)
			: base(data) {
		}

		public void AddIntentForChannel(Guid channelId, params IntentNode[] data) {
			AddIntentsForChannel(channelId, data);
		}

		public void AddIntentsForChannel(Guid channelId, IEnumerable<IntentNode> data) {
			if(ContainsKey(channelId)) {
				this[channelId] = this[channelId].Concat(data).ToArray();
			} else {
				this[channelId] = data.ToArray();
			}
		}

		public void Add(EffectIntents other) {
			foreach(KeyValuePair<Guid, IntentNode[]> kvp in other) {
				Add(kvp.Key, kvp.Value);
			}
		}

		static public EffectIntents Restrict(EffectIntents effectIntents, TimeSpan startTime, TimeSpan endTime) {
			return new EffectIntents(effectIntents.ToDictionary(
				x => x.Key,
				x => x.Value.Where(y => !(y.StartTime >= endTime || y.EndTime < startTime)).ToArray()));
		}
	}
}
