using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Intent
{
	public class IntentBuilder
	{
		public static StaticArrayIntent<RGBValue> CreateStaticArrayIntent(LightingValue startValue, LightingValue endValue, TimeSpan duration)
		{
			var interval = VixenSystem.DefaultUpdateTimeSpan;
			var intervals = (int) (duration.TotalMilliseconds/interval.TotalMilliseconds);
			
			RGBValue[] values = new RGBValue[intervals+1];
			var interpolator = Interpolator.Interpolator.Create<LightingValue>();
			for (int i = 0; i < intervals+1; i++)
			{
				LightingValue sample;
				double percent = interval.TotalMilliseconds*i/duration.TotalMilliseconds;
				interpolator.Interpolate(percent, startValue, endValue, out sample);
				values[i] = new RGBValue(sample.FullColor);
			}

			return new StaticArrayIntent<RGBValue>(interval, values, duration);
		}

		public static EffectIntents ConvertToStaticArrayIntents(EffectIntents intents, TimeSpan duration)
		{
			var interval = VixenSystem.DefaultUpdateTimeSpan;
			var intervals = (int)(duration.TotalMilliseconds / interval.TotalMilliseconds);
			EffectIntents effectIntents = new EffectIntents();
	
			foreach (var effectIntent in intents)
			{
				RGBValue[] values = new RGBValue[intervals + 1];
				for (int i = 0; i < intervals + 1; i++)
				{
					var currentTime = TimeSpan.FromMilliseconds(interval.TotalMilliseconds*i);
					var color = ProcessIntentNodes(effectIntent, currentTime);
					values[i] = new RGBValue(color);
				}
				effectIntents.AddIntentForElement(effectIntent.Key, new StaticArrayIntent<RGBValue>(interval, values, duration), TimeSpan.Zero);
			}

			return effectIntents;

		}

		private static Color ProcessIntentNodes(KeyValuePair<Guid, IntentNodeCollection> effectIntent, TimeSpan effectRelativeTime)
		{
			List<IIntentState> states = new List<IIntentState>();
			foreach (IIntentNode intentNode in effectIntent.Value)
			{
				if (TimeNode.IntersectsInclusively(intentNode, effectRelativeTime))
				{
					IIntentState intentState = intentNode.CreateIntentState(effectRelativeTime - intentNode.StartTime, 0);
					states.Add(intentState);
				}
			}

			return IntentHelpers.GetOpaqueRGBMaxColorForIntents(states);
		}


		public static IIntent CreateIntent(Color color, double intensity, TimeSpan duration)
		{
			LightingValue lightingValue = new LightingValue(color, intensity);
			IIntent intent = new LightingIntent(lightingValue, lightingValue, duration);
			return intent;
		}

		public static IIntent CreateIntent(Color startColor, Color endColor, double startIntensity, double endIntensity, TimeSpan duration)
		{
			var startValue = new LightingValue(startColor, startIntensity);
			var endValue = new LightingValue(endColor, endIntensity);
			IIntent intent = new LightingIntent(startValue, endValue, duration);
			return intent;
		}

		public static IIntent CreateDiscreteIntent(Color color, double intensity, TimeSpan duration)
		{
			DiscreteValue discreteValue = new DiscreteValue(color, intensity);
			IIntent intent = new DiscreteLightingIntent(discreteValue, discreteValue, duration);
			return intent;
		}

		public static IIntent CreateDiscreteIntent(Color color, double startIntensity, double endIntensity, TimeSpan duration)
		{
			var startingValue = new DiscreteValue(color, startIntensity);
			var endValue = new DiscreteValue(color, endIntensity);
			IIntent intent = new DiscreteLightingIntent(startingValue, endValue, duration);
			return intent;
		}
	}
}
