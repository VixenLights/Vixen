using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Drawing;
using CommonElements.ColorManagement.ColorModels;
using Vixen.Module;
using Vixen.Commands.KnownDataTypes;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

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
		public Level DefaultLevel { get; set; }

		[DataMember]
		public Color StaticColor { get; set; }

		[DataMember]
		public ColorGradient ColorGradient { get; set; }

		[DataMember]
		public Curve PulseCurve { get; set; }

		[DataMember]
		public bool ReverseSpin { get; set; }


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
			ColorGradient = new ColorGradient();
			PulseCurve = new Curve();
			ReverseSpin = false;
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
			return result;
		}
	}

	public enum SpinSpeedFormat
	{
		RevolutionCount,
		RevolutionFrequency,
		FixedTime
	}

	public enum SpinPulseLengthFormat
	{
		FixedTime,
		PercentageOfRevolution,
		EvenlyDistributedAcrossSegments
	}

	public enum SpinColorHandling
	{
		StaticColor,
		GradientThroughWholeEffect,
		GradientForEachPulse,
		ColorAcrossItems
	}
}
