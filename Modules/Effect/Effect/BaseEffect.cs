using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vixen.Attributes;
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

		protected HashSet<Color> GetValidColors()
		{
			HashSet<Color> validColors = new HashSet<Color>();
			validColors.AddRange(TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
			return validColors;
		}

		protected static IIntent CreateIntent(Color color, double intensity, TimeSpan duration)
		{
			return IntentBuilder.CreateIntent(color, intensity, duration);
		}

		protected static IIntent CreateIntent(Color startColor, Color endColor, double startIntensity, double endIntensity, TimeSpan duration)
		{
			return IntentBuilder.CreateIntent(startColor, endColor, startIntensity, endIntensity, duration);
		}

		protected static IIntent CreateDiscreteIntent(Color color, double intensity, TimeSpan duration)
		{
			return IntentBuilder.CreateDiscreteIntent(color, intensity, duration);
		}

		protected static IIntent CreateDiscreteIntent(Color color, double startIntensity, double endIntensity, TimeSpan duration)
		{
			return IntentBuilder.CreateDiscreteIntent(color, startIntensity, endIntensity, duration);
		}

	}
}
