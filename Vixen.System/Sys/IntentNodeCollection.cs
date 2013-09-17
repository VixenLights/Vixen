using System;
using System.Collections.Concurrent;
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

		 Dictionary<string, List<Tuple<int, float>>> intentIntensityHistory = new  Dictionary<string, List<Tuple<int, float>>>();
		private bool DoesIntentIntensityMatchPattern(LightingIntent intent )
		{
			return DoesIntentIntensityMatchPattern(intent, intent.StartValue.Intensity);
		}
	 
		private bool DoesIntentIntensityMatchPattern(LightingIntent intent, float intensity)
		{
			if (!intentIntensityHistory.ContainsKey(intent.GenericID)) {
				intentIntensityHistory.Add(intent.GenericID, new List<Tuple<int, float>>() { new Tuple<int, float>(1, intent.EndValue.Intensity) });
			} else {
				var t =new Tuple<int, float>(intentIntensityHistory[intent.GenericID].Max(m => m.Item1)+1, intent.EndValue.Intensity);
				intentIntensityHistory[intent.GenericID].Add(t);
			}

			var order = intentIntensityHistory[intent.GenericID].OrderBy(o => o.Item1);
			var item1= order.First().Item2;
			var item2 = order.Last().Item2;

			if (item1==item2)
				return true;
			else {
				if (item1>item2) {  
					return intensity>item2;
				} else { //Ascending
					return intensity<item2;
				}
			}
		}
		public IntentNodeCollection(IEnumerable<IIntentNode> intentNodes)
		{
			 AddRange(intentNodes);
			 //AddRangeCombiner(intentNodes);
		}
		/// <summary>
		/// When adding IntentNodes, check the existing nodes for IntentNodes that are consecutive... if they are consecutive, and the colors match, combine the Intents
		/// </summary>
		/// <param name="intentNodes"></param>
		public void AddRangeCombiner(IEnumerable<IIntentNode> intentNodes)
		{
			AddRange(intentNodes);
			return;
			intentNodes.ToList().ForEach(node => {
				lock (lockObject) {
					var newIntent = node.Intent as LightingIntent;

					if (newIntent != null) {
						var oldIntentNode = this.Where(nn => nn.EndTime.Equals(node.StartTime)).FirstOrDefault();
						if (oldIntentNode == null) {
							DoesIntentIntensityMatchPattern(node.Intent as LightingIntent);
							Add(node);
						} else {
							var oldIntent = oldIntentNode.Intent as LightingIntent;
							if (oldIntent != null 
								&& (oldIntent.EndValue.Color.R== newIntent.StartValue.Color.R
								&& oldIntent.EndValue.Color.G== newIntent.StartValue.Color.G
								&& oldIntent.EndValue.Color.B== newIntent.StartValue.Color.B 
								&& DoesIntentIntensityMatchPattern(oldIntent,newIntent.EndValue.Intensity))) {
								
								//Check to see if the Intensity matches the pattern
								
								//Create a new IntentNode to replace the old one with the new values.
								var lIntent = new LightingIntent(oldIntent.StartValue, newIntent.EndValue, oldIntent.TimeSpan.Add(newIntent.TimeSpan));
								lIntent.GenericID = ((LightingIntent)oldIntentNode.Intent).GenericID;
								var IntentNode = new IntentNode(lIntent, oldIntentNode.StartTime);
								this.Remove(oldIntentNode);

								Add(IntentNode);
							} else {
							 
								Add(node);
							}
						}
						 
					} else {
						 Add(node);
					}
				}
			});
		}
		private object lockObject = new object();
	}
}