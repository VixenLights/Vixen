using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Intent;

namespace Vixen.Sys
{
	public class IntentNodeCollection : List<IIntentNode>
	{
		public IntentNodeCollection()
		{
		}

		public IntentNodeCollection(IEnumerable<IIntentNode> intentNodes)
		{
			//AddRange(intentNodes);
			AddRangeCombiner(intentNodes);
		}
		/// <summary>
		/// When adding IntentNodes, check the existing nodes for IntentNodes that are consecutive... if they are consecutive, and the colors match, combine the Intents
		/// </summary>
		/// <param name="intentNodes"></param>
		public void AddRangeCombiner(IEnumerable<IIntentNode> intentNodes) {
			
			intentNodes.ToList().ForEach(node => {
				var newIntent = node.Intent as LightingIntent;
				if (newIntent != null) {
					var oldIntentNode = this.Where(nn => nn.EndTime.Equals(node.StartTime)).FirstOrDefault();
					if (oldIntentNode == null) {
						Add(node);
					} else {
						var oldIntent = oldIntentNode.Intent as LightingIntent;
						if (oldIntent != null && oldIntent.EndValue.Color.ToArgb() == newIntent.StartValue.Color.ToArgb()) {
							//Create a new IntentNode to replace the old one with the new values.
							var IntentNode = new IntentNode(new LightingIntent(oldIntent.StartValue, newIntent.EndValue, oldIntent.TimeSpan.Add(newIntent.TimeSpan)),oldIntentNode.StartTime);
							this.Remove(oldIntentNode);
							Add(IntentNode);
						} else
							Add(node);
					}
				} else {
					Add(node);
				}
			});
		}
	}
}