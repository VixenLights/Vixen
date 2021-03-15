using System.Collections.Generic;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Polygon;
using VixenModules.Effect.Effect;

namespace VixenModules.Effect.Morph
{
	/// <summary>
	/// Maintains and serializes the settings of the Morph effect.
	/// </summary>
	[DataContract]
	[KnownType(typeof(MorphData))]
	[KnownType(typeof(MorphPolygonData))]
	class MorphData : EffectTypeModuleData
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public MorphData()
		{
			Orientation = StringOrientation.Horizontal;
			PolygonType = PolygonType.Pattern;
			PolygonFillType = PolygonFillType.Wipe;
			RepeatCount = 0;
			RepeatDirection = WipeRepeatDirection.Up;
			RepeatSkip = 0;
			Stagger = 0;
			HeadDuration = 20; // Percent
			HeadLength = 4;
			HeadColor = new ColorGradient(System.Drawing.Color.White);
			TailColor = new ColorGradient(System.Drawing.Color.Red);
			Acceleration = 0;			
			FillColor = new ColorGradient(System.Drawing.Color.Red);
			FillPolygon = true;			
			MorphPolygonData = new List<MorphPolygonData>();
			DisplayElementWidth = 0;
			DisplayElementHeight = 0;

			// Default the brightness to 100%
			TailBrightness = new Curve(CurveType.Flat100);
			HeadBrightness = new Curve(CurveType.Flat100);
			FillBrightness = new Curve(CurveType.Flat100);
		}

		#endregion

		#region Public Properties

		[DataMember]
		public StringOrientation Orientation { get; set; }

		[DataMember]
		public PolygonType PolygonType { get; set; }

		[DataMember]
		public PolygonFillType PolygonFillType { get; set; }

		[DataMember]
		public int RepeatCount { get; set; }

		[DataMember]
		public WipeRepeatDirection RepeatDirection { get; set; }

		[DataMember]
		public int RepeatSkip { get; set; }

		[DataMember]
		public int Stagger { get; set; }

		[DataMember]
		public int HeadLength { get; set; }

		[DataMember]
		public int HeadDuration { get; set; }
						
		[DataMember]
		public ColorGradient HeadColor { get; set; }

		[DataMember]
		public ColorGradient TailColor { get; set; }
				
		[DataMember]
		public int Acceleration { get; set; }

		[DataMember]
		public ColorGradient FillColor { get; set; }

		[DataMember]
		public bool FillPolygon { get; set; }

		[DataMember]
		public List<MorphPolygonData> MorphPolygonData { get; set; }

		[DataMember]
		public Curve TailBrightness { get; set; }

		[DataMember]
		public Curve HeadBrightness { get; set; }

		[DataMember]
		public Curve FillBrightness { get; set; }

		[DataMember] 
		public int DisplayElementWidth { get; set; }

		[DataMember]
		public int DisplayElementHeight { get; set; }

		[DataMember]
		public int Margin { get; set; }

		#endregion

		#region Protected Methods

		protected override EffectTypeModuleData CreateInstanceForClone()
		{
			MorphData result = new MorphData
			{
				Orientation = Orientation,
				PolygonType = PolygonType,
				PolygonFillType = PolygonFillType,
				RepeatCount = RepeatCount,
				RepeatDirection = RepeatDirection,
				RepeatSkip = RepeatSkip,
				Stagger = Stagger,
				HeadDuration = HeadDuration,
				HeadLength = HeadLength,
				HeadColor = new ColorGradient(HeadColor),
				TailColor = new ColorGradient(TailColor),
				Acceleration = Acceleration,
				FillColor = new ColorGradient(FillColor),
				FillPolygon = FillPolygon,
				TailBrightness = new Curve(TailBrightness),
				HeadBrightness = new Curve(HeadBrightness),
				FillBrightness = new Curve(FillBrightness),
				DisplayElementWidth = DisplayElementWidth,
				DisplayElementHeight = DisplayElementHeight,
				Margin = Margin,
			};

			// Clone the Morph Polygons
			for (int index = 0; index < MorphPolygonData.Count; index++)
			{
				result.MorphPolygonData.Add((MorphPolygonData)(MorphPolygonData[index]).CreateInstanceForClone());
			}
					
			return result;
		}

		#endregion
	}	
}
