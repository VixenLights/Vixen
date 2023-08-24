using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using ZedGraph;

namespace VixenModules.Effect.Weave
{
	/// <summary>	
	/// Maintains the Weave effect data.
	/// </summary>	
	[DataContract]
	public class WeaveData: EffectTypeModuleData
	{
		#region Constructor

		public WeaveData()
		{
			HorizontalColors = new List<ColorGradient> { new ColorGradient(Color.Red) };
			VerticalColors = new List<ColorGradient> { new ColorGradient(Color.Green),  new ColorGradient(Color.White) };
			
			Direction = WeaveDirection.Up;
			LevelCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
			SpeedCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 10.0, 10.0 }));
			Orientation = StringOrientation.Vertical;

			WeaveThickness = 5;
			WeaveSpacing = 5;

			WeaveHorizontalThickness = 5;
			WeaveVerticalThickness = 5;

			WeaveVerticalSpacing = 5;
			WeaveHorizontalSpacing = 5;

			RotationAngle = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			HighlightPercentage = 5;
		}

		#endregion

		#region Public Properties

		[DataMember]
		public List<ColorGradient> HorizontalColors { get; set; }

		[DataMember]
		public List<ColorGradient> VerticalColors { get; set; }

		[DataMember]
		public WeaveDirection Direction { get; set; }

		[DataMember]
		public Curve SpeedCurve { get; set; }

		[DataMember]
		public bool Highlight { get; set; }

		[DataMember]
		public bool Show3D { get; set; }

		[DataMember]
		public Curve LevelCurve { get; set; }

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public bool AdvancedSizing { get; set; }

		[DataMember]
		public int WeaveThickness { get; set; }

		[DataMember]
		public int WeaveSpacing { get; set; }

		[DataMember]
		public int WeaveHorizontalThickness { get; set; }

		[DataMember]
		public int WeaveVerticalThickness { get; set; }

		[DataMember]
		public int WeaveHorizontalSpacing { get; set; }

		[DataMember]
		public int WeaveVerticalSpacing { get; set; }

		[DataMember]
		public Curve RotationAngle { get; set; }

		[DataMember]
		public int HighlightPercentage { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Creates a clone of the zoom data.
		/// </summary>
		/// <returns>Clone of the zoom data</returns>
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			WeaveData result = new WeaveData
			{
				HorizontalColors = HorizontalColors.ToList(),
				VerticalColors = VerticalColors.ToList(),
				Direction = Direction,
				Orientation = Orientation,
				Show3D = Show3D,
				Highlight = Highlight,
				LevelCurve = new Curve(LevelCurve),
				SpeedCurve = new Curve(SpeedCurve),
				AdvancedSizing = AdvancedSizing,
				WeaveSpacing = WeaveSpacing,
				WeaveThickness = WeaveThickness,
				WeaveHorizontalThickness = WeaveHorizontalThickness,
				WeaveVerticalThickness = WeaveVerticalThickness,
				WeaveVerticalSpacing = WeaveVerticalSpacing,
				WeaveHorizontalSpacing = WeaveHorizontalSpacing,
				RotationAngle = RotationAngle,
				HighlightPercentage = HighlightPercentage,
			};
			return result;
		}

		#endregion
	}
}
