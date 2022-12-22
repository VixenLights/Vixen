﻿using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Alternating {
	[DataContract]
	public class AlternatingData : EffectTypeModuleData {

		[DataMember]
		public List<GradientLevelPair> Colors { get; set; }

		[DataMember]
		public int Interval { get; set; }

		[DataMember]
		public bool EnableStatic { get; set; }

		[DataMember]
		public int GroupLevel { get; set; }

		[DataMember]
		public int IntervalSkipCount { get; set; }
		
		[DataMember]
		public int DepthOfEffect { get; set; }
		
		[DataMember]
		public bool EnableDepth { get; set; }

		[DataMember]
		public AlternatingMode AlternatingMode { get; set; }

		[DataMember]
		public Guid MarkCollectionId { get; set; }

		public AlternatingData()
		{
			Colors = new List<GradientLevelPair> {new GradientLevelPair(Color.Red, CurveType.Flat100), new GradientLevelPair(Color.Lime, CurveType.Flat100)};

			EnableStatic = false;
			Interval = 500;
			GroupLevel = 1;
			IntervalSkipCount = 1;
			DepthOfEffect = 0;
			EnableDepth = false;
			AlternatingMode = AlternatingMode.TimeInterval;
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			var gradientLevelList = Colors.Select(glp => new GradientLevelPair(new ColorGradient(glp.ColorGradient), new Curve(glp.Curve))).ToList();
			var result = new AlternatingData
			{
				Colors = gradientLevelList,
				EnableStatic = EnableStatic,
				Interval = Interval,
				GroupLevel = GroupLevel,
				IntervalSkipCount = IntervalSkipCount,
				DepthOfEffect = DepthOfEffect,
				EnableDepth = EnableDepth,
				AlternatingMode = AlternatingMode,
				MarkCollectionId = MarkCollectionId
			};
			return result;
		}
	}
}