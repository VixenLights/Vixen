using Vixen.Data.Value;
using Vixen.Sys;

namespace Vixen.Intent
{
	public class IntentBuilder
	{
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
	}
}
