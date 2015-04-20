using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using VixenModules.EffectEditor.TypeConverters;

namespace VixenModules.Effect.Chase
{
	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum ChaseColorHandling
	{
		[Description("Single Color")]
		StaticColor,
		[Description("Gradient Thru Effect")]
		GradientThroughWholeEffect,
		[Description("Gradient Per Pulse")]
		GradientForEachPulse,
		[Description("Gradient Accross Items")]
		ColorAcrossItems
	}
}