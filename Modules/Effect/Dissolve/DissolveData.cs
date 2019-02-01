using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Dissolve
{
	[DataContract]
	public class DissolveData : EffectTypeModuleData {
		
		[DataMember]
		public List<GradientLevelPair> Colors { get; set; }

		[DataMember]
		public bool GroupColors { get; set; }

		[DataMember]
		public DissolveMode DissolveMode { get; set; }

		[DataMember]
		public DissolveMarkType DissolveMarkType { get; set; }

		[DataMember]
		public Curve DissolveCurve { get; set; }
		
		[DataMember]
		public bool DissolveMethod { get; set; }

		[DataMember]
		public Guid MarkCollectionId { get; set; }
		
		[DataMember]
		public bool RandomDissolve { get; set; }

		[DataMember]
		public bool DissolveFlip { get; set; }
		
		[DataMember]
		public int GroupLevel { get; set; }

		[DataMember]
		public int StartingNode { get; set; }

		[DataMember]
		public bool RandomColor { get; set; }
		
		[DataMember]
		public int DepthOfEffect { get; set; }
		
		[DataMember]
		public bool EnableDepth { get; set; }

		[DataMember]
		public bool BothDirections { get; set; }

		[DataMember]
		public bool DirectionsTogether { get; set; }

		[DataMember]
		public bool ColorPerStep { get; set; }

		public DissolveData()
		{
			Colors = new List<GradientLevelPair> {new GradientLevelPair(Color.White,CurveType.Flat100) };
			DissolveMode = DissolveMode.TimeInterval;
			DissolveCurve = new Curve(new PointPairList(new[] { 100.0, 0.0 }, new[] { 0.0, 100.0 }));
			DissolveMarkType = DissolveMarkType.PerMark;
			RandomDissolve = true;
			DissolveFlip = true;
			GroupLevel = 1;
			StartingNode = 1;
			RandomColor = true;
			DepthOfEffect = 1;
			EnableDepth = false;
			BothDirections = false;
			DirectionsTogether = false;
			ColorPerStep = true;
		}

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			var gradientList = Colors.ToList();
			var result = new DissolveData
            {
				Colors = gradientList,
                DissolveMode = DissolveMode,
				MarkCollectionId = MarkCollectionId,
	            DissolveCurve = new Curve(DissolveCurve),
	            DissolveMethod = DissolveMethod,
	            RandomDissolve = RandomDissolve,
	            DissolveFlip = DissolveFlip,
	            GroupLevel = GroupLevel,
	            DissolveMarkType = DissolveMarkType,
	            StartingNode = StartingNode,
	            RandomColor = RandomColor,
	            DepthOfEffect = DepthOfEffect,
	            EnableDepth = EnableDepth,
	            BothDirections = BothDirections,
	            DirectionsTogether = DirectionsTogether,
	            ColorPerStep = ColorPerStep
			};
			return result;
		}
	}
}