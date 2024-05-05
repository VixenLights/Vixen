using System.Drawing;
using System.Runtime.Serialization;

using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;

using ZedGraph;

namespace VixenModules.Effect.Whirlpool
{
	/// <summary>
	/// Maintains data for the whirlpool effect.
	/// </summary>
	[DataContract]
	public class WhirlpoolData : EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public WhirlpoolData()
		{
			// String Setup
			Orientation = StringOrientation.Vertical;
			// Initialize the scale factor to max fidelity
			RenderScaleFactor = 1;

			// Grid Panels
			Columns = 1;
			Rows = 1;
			PanelSpacing = 0;
			IndividualConfiguration = false;

			// Begin Whirl Properties
			WhirlMode = WhirlpoolMode.RecurrentWhirls;
			TailLength = 10;
			StartLocation = WhirlpoolStartLocation.TopLeft;
			WhirlDirection = WhirlpoolDirection.In;
			Rotation = WhirlpoolRotation.Clockwise;
			ReverseDraw = false;
			Spacing = 2;
			Thickness = 2;
			Show3D = false;
			ColorMode = WhirlpoolColorMode.GradientOverTime;
			BandLength = 10;
			Colors = new List<ColorGradient> { new ColorGradient(Color.Red), new ColorGradient(Color.White) };
			LeftColor = new ColorGradient(Color.Blue);
			BottomColor = new ColorGradient(Color.Green);
			RightColor = new ColorGradient(Color.Yellow);
			TopColor = new ColorGradient(Color.Red);
			SingleColor = new ColorGradient(Color.Red);
			// End Whirl Properties

			// Configuration
			Iterations = 1;
			PauseAtEnd = 5;

			// Movement
			XOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			YOffsetCurve = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 50.0, 50.0 }));
			XScale = 100;
			YScale = 100;
			
			// Whirl Data
			WhirlData = new List<WhirlData>();
			WhirlData.Add(new WhirlData());

			// Brightness
			Intensity = new Curve(new PointPairList(new[] { 0.0, 100.0 }, new[] { 100.0, 100.0 }));
		}

		#endregion

		#region String Properties

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public int RenderScaleFactor { get; set; }

		#endregion

		#region Grid Panel Properties

		[DataMember]
		public int Columns { get; set; }

		[DataMember]
		public int Rows { get; set; }

		[DataMember]
		public int PanelSpacing { get; set; }

		[DataMember]
		public bool IndividualConfiguration { get; set; }

		#endregion

		#region Configuration Properties

		[DataMember]
		public int Iterations { get; set; }

		[DataMember]
		public int PauseAtEnd { get; set; }

		#endregion

		#region Movement Properties

		[DataMember]
		public int XScale { get; set; }

		[DataMember]
		public int YScale { get; set; }

		[DataMember]
		public Curve YOffsetCurve { get; set; }

		[DataMember]
		public Curve XOffsetCurve { get; set; }

		#endregion

		#region Whirl Properties

		[DataMember]
		public WhirlpoolMode WhirlMode { get; set; }

		[DataMember]
		public int TailLength { get; set; }

		[DataMember]
		public WhirlpoolStartLocation StartLocation { get; set; }

		[DataMember]
		public WhirlpoolDirection WhirlDirection { get; set; }

		[DataMember]
		public WhirlpoolRotation Rotation { get; set; }

		[DataMember]
		public int Spacing { get; set; }

		[DataMember]
		public int Thickness { get; set; }

		[DataMember]
		public bool ReverseDraw { get; set; }

		[DataMember]
		public bool Show3D { get; set; }

		[DataMember]
		public WhirlpoolColorMode ColorMode { get; set; }

		[DataMember]
		public int BandLength { get; set; }

		[DataMember]
		public ColorGradient SingleColor { get; set; }

		[DataMember]
		public List<ColorGradient> Colors { get; set; }

		[DataMember]
		public ColorGradient LeftColor { get; set; }

		[DataMember]
		public ColorGradient RightColor { get; set; }

		[DataMember]
		public ColorGradient TopColor { get; set; }

		[DataMember]
		public ColorGradient BottomColor { get; set; }

		[DataMember]
		public List<WhirlData> WhirlData { get; set; }


		#endregion

		#region Brightness Properties

		[DataMember]
		public Curve Intensity { get; set; }

		#endregion

		#region Protected Methods

		/// </inheritdoc>
		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			WhirlpoolData result = new WhirlpoolData
			{
				// String Properties
				Orientation = Orientation,
				RenderScaleFactor = RenderScaleFactor,

				// Grid Panel Properties
				Columns = Columns,
				Rows = Rows,
				PanelSpacing = PanelSpacing,
				IndividualConfiguration = IndividualConfiguration,

				// Configuration Properties
				Iterations = Iterations,
				PauseAtEnd = PauseAtEnd,

				// Movement Properties
				XScale = XScale,
				YScale = YScale,
				YOffsetCurve = new Curve(YOffsetCurve),
				XOffsetCurve = new Curve(XOffsetCurve),

				// Whirl Properties
				WhirlMode = WhirlMode,
				TailLength = TailLength,
				StartLocation = StartLocation,
				WhirlDirection = WhirlDirection,
				Rotation = Rotation,
				ReverseDraw = ReverseDraw,
				Spacing = Spacing,
				Thickness = Thickness,
				Show3D = Show3D,
				ColorMode = ColorMode,
				BandLength = BandLength,
				Colors = Colors.ToList(),
				LeftColor = LeftColor,
				BottomColor = BottomColor,
				RightColor = RightColor,
				TopColor = TopColor,
				SingleColor = SingleColor,

				// Brightness Properties
				Intensity = new Curve(Intensity),
			};
	
			// Loop over the Whirl Data
			foreach (WhirlData whirl in WhirlData)
			{
				result.WhirlData.Add(whirl.CreateInstanceForClone());
			}

			return result;
		}

		#endregion
	}
}
