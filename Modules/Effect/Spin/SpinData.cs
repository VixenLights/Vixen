using System.Runtime.Serialization;
using System.Drawing;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using System.ComponentModel;
using Vixen.TypeConverters;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Spin
{
	[DataContract]
	public class SpinData : EffectTypeModuleData
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

		[DataMember]
		public bool EnableDefaultLevel { get; set; }

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
		
		[DataMember]
		public TargetNodeSelection TargetNodeSelection { get; set; }

		[OnDeserialized]
		void OnDeserialized(StreamingContext c)
		{
			//Ensure defaults for new fields that might not be in older effects.
			if (StaticColor.IsEmpty)
			{
				StaticColor = Color.White;
			}
		}

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
			StaticColor = Color.White;
			ColorGradient = new ColorGradient(Color.White);
			PulseCurve = new Curve();
			ReverseSpin = false;
			DepthOfEffect = 0;
			TargetNodeSelection = TargetNodeSelection.Group;
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
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
			result.EnableDefaultLevel = EnableDefaultLevel;
			result.TargetNodeSelection = TargetNodeSelection;
			return result;
		}
	}

	public enum SpinSpeedFormat
	{
		[Description("Revolution Count")]
		RevolutionCount,
		[Description("Revolution Freq")]
		RevolutionFrequency,
		[Description("Fixed Time")]
		FixedTime
	}

	public enum SpinPulseLengthFormat
	{
		[Description("Fixed Time")]
		FixedTime,
		[Description("Percent Revolution")]
		PercentageOfRevolution,
		[Description("Distribute Evenly")]
		EvenlyDistributedAcrossSegments
	}

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