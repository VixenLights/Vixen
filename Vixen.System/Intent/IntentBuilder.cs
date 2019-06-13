using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using Vixen.Data.Value;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;

namespace Vixen.Intent
{
	public class IntentBuilder
	{
		private static int EmptyColor = Color.FromArgb(0, 0, 0).ToArgb();
		
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

		public static EffectIntents ConvertToStaticArrayIntents(EffectIntents intents, TimeSpan duration, bool discrete = false)
		{
			if (discrete)
			{
				return ConvertToDiscreteStaticArrayIntents(intents, duration);
			}

			var interval = VixenSystem.DefaultUpdateTimeSpan;
			var intervals = (int)(duration.TotalMilliseconds / interval.TotalMilliseconds);
			EffectIntents effectIntents = new EffectIntents(intents.Count);
	
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

		private static EffectIntents ConvertToDiscreteStaticArrayIntents(EffectIntents intents, TimeSpan duration)
		{
			var interval = VixenSystem.DefaultUpdateTimeSpan;
			var intervals = (int)(duration.TotalMilliseconds / interval.TotalMilliseconds);
			EffectIntents effectIntents = new EffectIntents();

			foreach (var effectIntent in intents)
			{
				Dictionary<Color, DiscreteValue[]> colorValues = new Dictionary<Color, DiscreteValue[]>();
				
				for (int i = 0; i < intervals + 1; i++)
				{
					var currentTime = TimeSpan.FromMilliseconds(interval.TotalMilliseconds * i);
					var colors = ProcessDiscreteIntentNodes(effectIntent, currentTime);
					DiscreteValue[] values;
					foreach (KeyValuePair<Color, DiscreteValue> color in colors)
					{
						if(color.Key.ToArgb() == EmptyColor) { continue;}
						colorValues.TryGetValue(color.Key, out values);
						if (values == null)
						{
							values = InitializeDiscreteValues(color.Key, intervals + 1);
							colorValues[color.Key] = values;
						}
						values[i] = color.Value;
					}
				}

				foreach (var discreteValues in colorValues)
				{
					effectIntents.AddIntentForElement(effectIntent.Key, new StaticArrayIntent<DiscreteValue>(interval, discreteValues.Value, duration), TimeSpan.Zero);
				}
				
			}

			return effectIntents;
		}

		private static DiscreteValue[] InitializeDiscreteValues(Color c, int number)
		{
			var discreteValues = new DiscreteValue[number];
			for (int i = 0; i < discreteValues.Length; i++)
			{
				discreteValues[i] = new DiscreteValue(c, 0);
			}

			return discreteValues;
		}

		private static Dictionary<Color, DiscreteValue> ProcessDiscreteIntentNodes(KeyValuePair<Guid, IntentNodeCollection> effectIntent, TimeSpan effectRelativeTime)
		{
			IntentStateList states = new IntentStateList();
			foreach (IIntentNode intentNode in effectIntent.Value)
			{
				if (TimeNode.IntersectsInclusively(intentNode, effectRelativeTime))
				{
					IIntentState intentState = intentNode.CreateIntentState(effectRelativeTime - intentNode.StartTime, SequenceLayers.GetDefaultLayer());
					states.Add(intentState);
				}
			}

			return GetAlphaDiscreteColorsForIntents(states);
		}

		private static Color ProcessIntentNodes(KeyValuePair<Guid, IntentNodeCollection> effectIntent, TimeSpan effectRelativeTime)
		{
			IntentStateList states = new IntentStateList();
			foreach (IIntentNode intentNode in effectIntent.Value)
			{
				if (intentNode.StartTime > effectRelativeTime) break;
				if (TimeNode.IntersectsInclusively(intentNode, effectRelativeTime))
				{
					IIntentState intentState = intentNode.CreateIntentState(effectRelativeTime - intentNode.StartTime, null);
					states.Add(intentState);
				}
			}

			return IntentHelpers.GetOpaqueRGBMaxColorForIntents(states);
		}


		public static IIntent CreateIntent(Color color, double intensity, TimeSpan duration)
		{
			LightingValue lightingValue = new LightingValue(color, intensity);
			IIntent intent = new StaticLightingIntent(lightingValue, duration);
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
			IIntent intent = new StaticDiscreteLightingIntent(discreteValue, duration);
			return intent;
		}

		public static IIntent CreateDiscreteIntent(Color color, double startIntensity, double endIntensity, TimeSpan duration)
		{
			var startingValue = new DiscreteValue(color, startIntensity);
			var endValue = new DiscreteValue(color, endIntensity);
			IIntent intent = new DiscreteLightingIntent(startingValue, endValue, duration);
			return intent;
		}

		public static Dictionary<Color, DiscreteValue> GetAlphaDiscreteColorsForIntents(IIntentStates states)
		{
			Dictionary<Color, DiscreteValue> colors = new Dictionary<Color, DiscreteValue>();

			IEnumerable<IGrouping<Color, IIntentState>> colorStates = states.GroupBy(
				(x =>
				{
					if (x is IIntentState<DiscreteValue> state)
					{
						return state.GetValue().Color;
					}
					
					return Color.Empty;
				}
				));

			foreach (IGrouping<Color, IIntentState> grouping in colorStates)
			{

				double intensity = grouping.Max(x =>
				{
					if (x is IIntentState<DiscreteValue> state)
					{
						return state.GetValue().Intensity;
					}
					
					return 0;
				});

				Color brightest = Color.FromArgb(grouping.Key.R, grouping.Key.G, grouping.Key.B);
				colors.Add(brightest, new DiscreteValue(brightest, intensity));
			}

			return colors;
		}
	}
}
