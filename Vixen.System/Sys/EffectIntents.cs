using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Concurrent;

namespace Vixen.Sys
{
	public class EffectIntents : Dictionary<Guid, IntentNodeCollection>
	{
		public EffectIntents()
		{
		}

		public EffectIntents(IDictionary<Guid, IntentNodeCollection> data)
			: base(data)
		{
		}

		public IEnumerable<Guid> ElementIds
		{
			get { return Keys; }
		}

		public IntentNodeCollection GetIntentNodesForElement(Guid elementId)
		{
			IntentNodeCollection intentNodeCollection;
			TryGetValue(elementId, out intentNodeCollection);
			return intentNodeCollection;
		}

		public IIntentNode[] GetElementIntentsAtTime(Guid elementId, TimeSpan effectRelativeTime)
		{
			IntentNodeCollection elementIntents;
			if (TryGetValue(elementId, out elementIntents)) {
				return elementIntents.Where(x => TimeNode.IntersectsInclusively(x, effectRelativeTime)).ToArray();
			}
			return null;
		}

		public void AddIntentForElement(Guid elementId, IIntent intent, TimeSpan startTime)
		{
			_AddIntentForElement(elementId, new IntentNode(intent, startTime));
		}

		private void _AddIntentForElement(Guid elementId, IIntentNode intentNode)
		{
		   _AddIntentsForElement(elementId, new IIntentNode[] { intentNode });
		}
		
		private void _AddIntentsForElement(Guid elementId, IEnumerable<IIntentNode> intentNodes) {

			IntentNodeCollection nodes;
			if (TryGetValue(elementId, out nodes)) {
				nodes.AddRange(intentNodes);
			}
			else {
				this[elementId] = new IntentNodeCollection(intentNodes);
			}
			
		}

		public void Add(EffectIntents other)
		{
			foreach (Guid elementId in other.Keys) {
				_AddIntentsForElement(elementId, other[elementId]);
			}
		}

		public void OffsetAllCommandsByTime(TimeSpan offset)
		{
			foreach (IntentNodeCollection intentNodes in Values) {
				foreach (var intentNode in intentNodes)
				{
					intentNode.OffSetTime(offset);
				}
			}
		}

		public static EffectIntents Restrict(EffectIntents effectIntents, TimeSpan startTime, TimeSpan endTime)
		{
			return new EffectIntents(effectIntents.ToDictionary(
				x => x.Key,
				x => new IntentNodeCollection(x.Value.Where(y => !(y.StartTime >= endTime || y.EndTime < startTime)))));
		}
	}
}