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

		public IIntentNode GetChannelIntentAtTime(Guid channelId, TimeSpan effectRelativeTime) {
			IntentNodeCollection channelIntents;
			if(TryGetValue(channelId, out channelIntents)) {
				//return channelIntents.FirstOrDefault(x => effectRelativeTime >= x.StartTime && effectRelativeTime <= x.EndTime);
				return channelIntents.FirstOrDefault(x => TimeNode.IntersectsInclusively(x, effectRelativeTime));
			}
			return null;
		}

		public void AddIntentForChannel(Guid channelId, IIntent intent, TimeSpan startTime) {
			_AddIntentForChannel(channelId, new IntentNode(intent, startTime));
		}

		private void _AddIntentForChannel(Guid channelId, IIntentNode intentNode) {
			if(ContainsKey(channelId)) {
				this[channelId].Add(intentNode);
			} else {
				this[channelId] = IntentNodeCollection.Create(new[] { intentNode });
			}
		}

		private void _AddIntentsForChannel(Guid channelId, IEnumerable<IIntentNode> intentNodes) {
			if(ContainsKey(channelId)) {
				this[channelId].AddRange(intentNodes);
			} else {
				this[channelId] = IntentNodeCollection.Create(intentNodes);
			}
		}

		public void Add(EffectIntents other) {
			foreach(Guid channelId in other.Keys) {
				_AddIntentsForChannel(channelId, other[channelId]);
			}
		}

		public void OffsetAllCommandsByTime(TimeSpan offset) {
			foreach(IntentNodeCollection intentNodes in Values) {
				IntentNode[] newIntentNodes = intentNodes.Select(x => new IntentNode(x.Intent, x.StartTime + offset)).ToArray();
				intentNodes.Clear();
				intentNodes.AddRange(newIntentNodes);
			}
			//foreach(KeyValuePair<Guid, CommandNode[]> kvp in this.ToArray()) {
			//    List<CommandNode> newCommands = new List<CommandNode>();
			//    foreach(CommandNode cn in kvp.Value)
			//        newCommands.Add(new CommandNode(cn.Command, cn.StartTime + offset, cn.TimeSpan));
			//    this[kvp.Key] = newCommands.ToArray();
			//}
		}

		static public EffectIntents Restrict(EffectIntents effectIntents, TimeSpan startTime, TimeSpan endTime) {
			return new EffectIntents(effectIntents.ToDictionary(
				x => x.Key,
				x => IntentNodeCollection.Create(x.Value.Where(y => !(y.StartTime >= endTime || y.EndTime < startTime)))));
		}
	}
}
