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

		private bool CreateNewIntent(LightingIntent oldIntent, LightingIntent newIntent)
		{
			//We can only combine intents if they are truly linear. i.e. start and end values of both are the same. 
			//Anything else we cannot tell because it can vary based on timespan and start and end values what linear is especially in ramps.
			bool returnValue = false;
			try {


				if (oldIntent.StartValue.Intensity.Equals(oldIntent.EndValue.Intensity) && oldIntent.EndValue.Intensity.Equals(newIntent.StartValue.Intensity) 
					&& oldIntent.EndValue.Intensity.Equals(newIntent.EndValue.Intensity))
				{
					returnValue = true;
				} 
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
					var oldIntentNode = this.FirstOrDefault(nn => nn.EndTime.Equals(node.StartTime));
					if (oldIntentNode != null) {
						var oldIntent = oldIntentNode.Intent as LightingIntent;
						if (oldIntent != null && oldIntent.EndValue.FullColor.ToArgb()== newIntent.StartValue.FullColor.ToArgb() && CreateNewIntent(oldIntent, newIntent)) {

							//Create a new IntentNode to replace the old one with the new values.
							var lIntent = new LightingIntent(oldIntent.StartValue,  newIntent.EndValue , oldIntent.TimeSpan.Add(newIntent.TimeSpan));
							
							lIntent.GenericID = ((LightingIntent)oldIntentNode.Intent).GenericID;
							
							var intentNode = new IntentNode(lIntent, oldIntentNode.StartTime);

							this.Remove(oldIntentNode);
							Add(intentNode);
							continue;
						}
					}  
				}
				
				Add(node);

			}
		}


	}
}