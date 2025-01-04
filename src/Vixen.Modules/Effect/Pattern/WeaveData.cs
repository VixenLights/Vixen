using System.Drawing;
using System.Runtime.Serialization;

using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Pattern;

using ZedGraph;

namespace VixenModules.Effect.Weave
{
	/// <summary>	
	/// Maintains the Pattern (Weave & Brick) effect data.
	/// </summary>	
	/// <remarks>
	/// This effect was originally released as the Weave effect.  When addtional patterns were added
	/// the effect was renamed but this class kept its original name and namespace to ensure existing sequences
	/// would continue to deserialize correctly.
	/// </remarks>
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

			MortarHeight = 5;
			MortarColor = new ColorGradient(Color.Black);
			BrickHeight = 5;
			BrickWidth = 15;
		}

		#endregion

		#region Public Properties

		[DataMember]
		public PatternTypes PatternType { get; set; }

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

		// Brick Specific Data Members
		[DataMember]
		public ColorGradient MortarColor { get; set; }

		[DataMember]
		public int MortarHeight { get; set; }

		[DataMember]
		public int BrickWidth { get; set; }

		[DataMember]
		public int BrickHeight { get; set; }

		[DataMember]
		public bool TransposeTile {  get; set; }	

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
				MortarColor = MortarColor,
				MortarHeight = MortarHeight,
				BrickWidth = BrickWidth,
				BrickHeight = BrickHeight,
				TransposeTile = TransposeTile,
			};
			return result;
		}

		#endregion

		#region Private Methods

		[OnDeserializing]
		private void OnDeserializing(StreamingContext streamingContext)
		{
			if (MortarColor == null)
			{
				MortarHeight = 5;
				MortarColor = new ColorGradient(Color.Black);
				BrickHeight = 5;
				BrickWidth = 15;
			}
		}

		#endregion
	}
}
