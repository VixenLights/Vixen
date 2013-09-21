//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Vixen.Intent;
//using Vixen.Sys;

 
//	public static class IntentNodeCollectionExtensions
//	{
//		/// <summary>
//		/// When adding IntentNodes, check the existing nodes for IntentNodes that are consecutive... if they are consecutive, and the colors match, combine the Intents
//		/// </summary>
//		/// <param name="intentNodes"></param>
//		public static void AddRangeCombiner(this IntentNodeCollection collection, IEnumerable<IIntentNode> intentNodes,  object lockObject)
//		{

//			intentNodes.ToList().ForEach(node => {
//				lock (lockObject) {
//					var newIntent = node.Intent as LightingIntent;
//					if (newIntent != null) {
//						var oldIntentNode = collection.Where(nn => nn.EndTime.Equals(node.StartTime)).FirstOrDefault();
//						if (oldIntentNode == null) {
//							collection.Add(node);
//						} else {
//							var oldIntent = oldIntentNode.Intent as LightingIntent;
//							if (oldIntent != null && oldIntent.EndValue.Color.ToArgb() == newIntent.StartValue.Color.ToArgb()) {
//								//Create a new IntentNode to replace the old one with the new values.
//								var IntentNode = new IntentNode(new LightingIntent(oldIntent.StartValue, newIntent.EndValue, oldIntent.TimeSpan.Add(newIntent.TimeSpan)), oldIntentNode.StartTime);
//								collection.Remove(oldIntentNode);

//								collection.Add(IntentNode);
//							} else
//								collection.Add(node);
//						}
//					} else {
//						collection.Add(node);
//					}
//				}
//			});
//		}
		
//	}
 
