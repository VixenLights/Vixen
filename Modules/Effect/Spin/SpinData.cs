using System.Runtime.Serialization;
using System.Drawing;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.ComponentModel;
using Vixen.TypeConverters;

namespace VixenModules.Effect.Spin
{
	[DataContract]
	public class SpinData : ModuleDataModelBase
	{
		[DataMember]
		public SpinSpeedFormat SpeedFormat { get; set; }

		[DataMember]
		public SpinPulseLengthFormat PulseLengthFormat { get; set; }

		[DataMember]
		public SpinColorHandling ColorHandling { get; set; }

		[DataMember]
		public double RevolutionCount { get; set; }

		[DataMember]
		public double RevolutionFrequency { get; set; }

		[DataMember]
		public int RevolutionTime { get; set; }

		[DataMember]
		public int PulseTime { get; set; }

		[DataMember]
		public int PulsePercentage { get; set; }

		[DataMember]
		public double DefaultLevel { get; set; }

		private Color _staticColor;

		[DataMember]
		public Color StaticColor
		{
			get { return _staticColor; }
			set
			{
				_staticColor = value;
				StaticColorGradient = new ColorGradient(_staticColor);
			}
		}

		[DataMember]
		public ColorGradient ColorGradient { get; set; }

		public ColorGradient StaticColorGradient { get; set; }

		[DataMember]
		public Curve PulseCurve { get; set; }

		[DataMember]
		public bool ReverseSpin { get; set; }

		[DataMember]
		public int DepthOfEffect { get; set; }

		public SpinData()
		{
			SpeedFormat = SpinSpeedFormat.RevolutionCount;
			PulseLengthFormat = SpinPulseLengthFormat.EvenlyDistributedAcrossSegments;
			ColorHandling = SpinColorHandling.StaticColor;
			RevolutionCount = 3;
			RevolutionFrequency = 2;
			RevolutionTime = 500;
			PulseTime = 100;
			PulsePercentage = 10;
			DefaultLevel = 0;
			StaticColor = Color.Empty;
			ColorGradient = new ColorGradient(Color.White);
			PulseCurve = new Curve();
			ReverseSpin = false;
			DepthOfEffect = 0;
		}

		public override IModuleDataModel Clone()
		{
			SpinData result = new SpinData();
			result.SpeedFormat = SpeedFormat;
			result.PulseLengthFormat = PulseLengthFormat;
			result.ColorHandling = ColorHandling;
			result.RevolutionCount = RevolutionCount;
			result.RevolutionFrequency = RevolutionFrequency;
			result.RevolutionTime = RevolutionTime;
			result.PulseTime = PulseTime;
			result.PulsePercentage = PulsePercentage;
			result.DefaultLevel = DefaultLevel;
			result.StaticColor = StaticColor;
			result.ColorGradient = new ColorGradient(ColorGradient);
			result.PulseCurve = new Curve(PulseCurve);
			result.ReverseSpin = ReverseSpin;
			result.DepthOfEffect = DepthOfEffect;
			return result;
		}
	}

	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum SpinSpeedFormat
	{
		[Description("Revolution Count")]
		RevolutionCount,
		[Description("Revolution Freq")]
		RevolutionFrequency,
		[Description("Fixed Time")]
		FixedTime
	}

	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum SpinPulseLengthFormat
	{
		[Description("Fixed Time")]
		FixedTime,
		[Description("Percent Revolution")]
		PercentageOfRevolution,
		[Description("Distribute Evenly")]
		EvenlyDistributedAcrossSegments
	}

	[TypeConverter(typeof(EnumDescriptionTypeConverter))]
	public enum SpinColorHandling
	{
		[Description("Single Color")]
		StaticColor,
		[Description("Gradient Thru Effect")]
		GradientThroughWholeEffect,
		[Description("Gradient Per Pulse")]
		GradientForEachPulse,
		[Description("Gradient Across Items")]
		ColorAcrossItems
	}
}