using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
using Vixen.Attributes;
using Vixen.Data.Value;
using Vixen.Intent;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Property.Color;

namespace VixenModules.Effect.Effect
{
	public abstract class BaseEffect : EffectModuleInstanceBase
	{

		[ProviderCategory(@"Layer", 0)]
		[ProviderDisplayName(@"Layer")]
		[ProviderDescription(@"Layer")]
		[PropertyOrder(3)]
		public override byte Layer { get; set; }

		protected static IIntent CreateIntent(Color color, double intensity, TimeSpan duration)
		{
			LightingValue lightingValue = new LightingValue(color, intensity);
			IIntent intent = new LightingIntent(lightingValue, lightingValue, duration);
			return intent;
		}

		protected static IIntent CreateIntent(Color startColor, Color endColor, double startIntensity, double endIntensity, TimeSpan duration)
		{
			var startValue = new LightingValue(startColor, startIntensity);
			var endValue = new LightingValue(endColor, endIntensity);
			IIntent intent = new LightingIntent(startValue, endValue, duration);
			return intent;
		}

		protected static IIntent CreateDiscreteIntent(Color color, double intensity, TimeSpan duration)
		{
			DiscreteValue discreteValue = new DiscreteValue(color, intensity);
			IIntent intent = new DiscreteLightingIntent(discreteValue, discreteValue, duration);
			return intent;
		}

		protected static IIntent CreateDiscreteIntent(Color color, double startIntensity, double endIntensity, TimeSpan duration)
		{
			var startingValue = new DiscreteValue(color, startIntensity);
			var endValue = new DiscreteValue(color, endIntensity);
			IIntent intent = new DiscreteLightingIntent(startingValue, endValue, duration);
			return intent;
		}

		protected bool IsDiscrete()
		{
			return TargetNodes.Any(x => ColorModule.isElementNodeDiscreteColored(x));
		}
		
	}
}
