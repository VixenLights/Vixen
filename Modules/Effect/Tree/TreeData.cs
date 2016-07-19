using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Tree
{
	[DataContract]
	public class TreeData : EffectTypeModuleData
	{

		public TreeData()
		{
			Colors = new List<ColorGradient>{new ColorGradient(Color.Red), new ColorGradient(Color.Lime), new ColorGradient(Color.Blue)};
			BackgroundColor = new ColorGradient(Color.Red);
			ColorType = TreeColorType.Static;
			BranchDirection = TreeBranchDirection.Up;
			Speed = 10;
			ToggleBlend = true;
			Branches = 10;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			BlendCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 70.0, 70.0 }));
			BackgroundLevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 70.0, 70.0 }));
			Orientation=StringOrientation.Vertical;
		}

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public ColorGradient BackgroundColor { get; set; }

		[DataMember]
		public TreeColorType ColorType { get; set; }

		[DataMember]
		public int Speed { get; set; }

		[DataMember]
		public bool ToggleBlend { get; set; }

		[DataMember]
		public int Branches { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public Curve BlendCurve { get; set; }

		[DataMember]
		public Curve BackgroundLevelCurve { get; set; }

		[DataMember]
		public TreeBranchDirection BranchDirection { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			TreeData result = new TreeData
			{
				Colors = Colors.ToList(),
				BackgroundColor = BackgroundColor,
				ColorType = ColorType,
				Speed = Speed,
				BranchDirection = BranchDirection,
				Branches = Branches,
				ToggleBlend = ToggleBlend,
				Orientation = Orientation,
				BlendCurve = new Curve(BlendCurve),
				LevelCurve = new Curve(LevelCurve),
				BackgroundLevelCurve = new Curve(BackgroundLevelCurve)
			};
			return result;
		}
	}
}
