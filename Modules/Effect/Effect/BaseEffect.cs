using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Vixen.Intent;
using Vixen.Module.Effect;
using Vixen.Sys;
using VixenModules.Property.Color;

namespace VixenModules.Effect.Effect
{
	/// <summary>
	/// Base effect implementation to be used by all basic type effects. This provides 
	/// some utility methods to create intents and determine valid discrete colors
	/// </summary>
	public abstract class BaseEffect : EffectModuleInstanceBase
	{
		
		protected abstract EffectTypeModuleData EffectModuleData { get; }

		/// <summary>
		/// Indicates if there is any discrete elements underlying this effect. It does not mean all of them are discrete if true.
		/// </summary>
		protected bool IsDiscrete { get; set; }

		protected HashSet<Color> GetValidColors()
		{
			HashSet<Color> validColors = new HashSet<Color>();
			validColors.AddRange(TargetNodes.SelectMany(x => ColorModule.getValidColorsForElementNode(x, true)));
			return validColors;
		}

		protected bool IsElementDiscrete(ElementNode elementNode)
		{
			return ColorModule.isElementNodeDiscreteColored(elementNode);
		}

		protected IIntent CreateIntent(ElementNode node, Color color, double intensity, TimeSpan duration)
		{
			if (IsDiscrete && IsElementDiscrete(node))
			{
				return CreateDiscreteIntent(color, intensity, duration);
			}

			return CreateIntent(color, intensity, duration);
		}

		protected IIntent CreateIntent(ElementNode node, Color startColor, Color endColor, double startIntensity, double endIntensity, TimeSpan duration)
		{
			if (IsDiscrete && IsElementDiscrete(node))
			{
				return CreateDiscreteIntent(startColor, startIntensity, endIntensity, duration);
			}

			return CreateIntent(startColor, endColor, startIntensity, endIntensity, duration);
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
