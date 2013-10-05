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
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		public IntentNodeCollection(IEnumerable<IIntentNode> intentNodes)
		{
			//AddRange(intentNodes);
			AddRangeCombiner(intentNodes);
		}
		Dictionary<string, List<Tuple<int, double>>> intensityHistory = new Dictionary<string, List<Tuple<int, double>>>();

		private bool CreateNewIntent(LightingIntent oldIntent, LightingIntent newIntent)
		{
			if (!intensityHistory.ContainsKey(oldIntent.GenericID)) {
				intensityHistory.Add(oldIntent.GenericID, new List<Tuple<int, double>>() { 
							new Tuple<int,double>(0, oldIntent.StartValue.Intensity),
							new Tuple<int,double>(1, oldIntent.EndValue.Intensity)
						});
			}

			bool returnValue = true;

			try {


				var order = intensityHistory[oldIntent.GenericID].OrderBy(o => o.Item1);

				var item1= order.First().Item2;
				var item2 = order.Last().Item2;

				if (item1==item2)
					return true;
				else {
					if (oldIntent.EndValue.Intensity == newIntent.StartValue.Intensity && oldIntent.EndValue.Intensity != newIntent.EndValue.Intensity) {
						return true;
					} else {
						if (item1<item2) {
							returnValue= newIntent.EndValue.Intensity>item2;
						} else { //Ascending
							returnValue= newIntent.EndValue.Intensity<item2;
						}
					}
				}
				if (returnValue)
					intensityHistory.Remove(oldIntent.GenericID);
			} catch (Exception e) {
				Logging.ErrorException(e.Message, e);

			}
			return returnValue;
		}

	 	/// <summary>
		/// When adding IntentNodes, check the existing nodes for IntentNodes that are consecutive... if they are consecutive, and the colors match, combine the Intents
		/// </summary>
		/// <param name="intentNodes"></param>
		public void AddRangeCombiner(IEnumerable<IIntentNode> intentNodes)
		{
			//AddRange(intentNodes);
			//return;
			// TODO: it looks like these only support LightingIntents. It should either be made generic, or the class made specific.
			foreach (var node in intentNodes) {

				var newIntent = node.Intent as LightingIntent;

				if (newIntent != null) {
					var oldIntentNode = this.Where(nn => nn.EndTime.Equals(node.StartTime)).FirstOrDefault();
					if (oldIntentNode != null) {
						var oldIntent = oldIntentNode.Intent as LightingIntent;
						if (oldIntent != null && oldIntent.EndValue.FullColor.ToArgb()== newIntent.StartValue.FullColor.ToArgb() && !CreateNewIntent(oldIntent, newIntent)) {

							bool changeEndValue = false;

							//Create a new IntentNode to replace the old one with the new values.
							var lIntent = new LightingIntent(oldIntent.StartValue, changeEndValue?newIntent.EndValue: oldIntent.EndValue, oldIntent.TimeSpan.Add(newIntent.TimeSpan));
							lIntent.GenericID = ((LightingIntent)oldIntentNode.Intent).GenericID;
							var IntentNode = new IntentNode(lIntent, oldIntentNode.StartTime);

							this.Remove(oldIntentNode);
							Add(IntentNode);
							continue;
						}
					}  
				}
				
				Add(node);

			}
		}


	}
}