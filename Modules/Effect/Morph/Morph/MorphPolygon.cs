using System.Collections.Generic;
using System.ComponentModel;
using Vixen.Attributes;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.App.Polygon;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using ZedGraph;
using Line = VixenModules.App.Polygon.Line;

namespace VixenModules.Effect.Morph
{	
	/// <summary>
	/// Maintains morph polygon settings for the morph effect.
	/// </summary>
	[ExpandableObject]
	public class MorphPolygon : ExpandoObjectBase, IMorphPolygon
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public MorphPolygon()
		{
			HeadLength = 4;
			HeadDuration = 20;
			Acceleration = 0;
			HeadColor = new ColorGradient(System.Drawing.Color.White);
			TailColor = new ColorGradient(System.Drawing.Color.Red);
			FillColor = new ColorGradient(System.Drawing.Color.Red);

			int width = 10;
			int height = 10;

			// If the display element size is available then...
			if (BufferWidth != 0 && BufferHeight != 0)
			{
				// Default to the polygon being 1/4 the display element
				width = BufferWidth / 4;
				height = BufferHeight / 4;
			}

			// Create the default polygon
			Polygon = new Polygon();
			PolygonPoint ptTopLeft = new PolygonPoint();
			ptTopLeft.X = 0;
			ptTopLeft.Y = 0;
			PolygonPoint ptTopRight = new PolygonPoint();
			ptTopRight.X = width - 1;
			ptTopRight.Y = 0;
			PolygonPoint ptBottomRight = new PolygonPoint();
			ptBottomRight.X = width - 1;
			ptBottomRight.Y = height - 1;
			PolygonPoint ptBottomLeft = new PolygonPoint();
			ptBottomLeft.X = 0;
			ptBottomLeft.Y = height - 1;

			// Add the points to the polygon
			Polygon.Points.Add(ptTopLeft);
			Polygon.Points.Add(ptTopRight);
			Polygon.Points.Add(ptBottomRight);
			Polygon.Points.Add(ptBottomLeft);

			FillType = PolygonFillType.Wipe;

			// Default the brightness to 100%
			TailBrightness = new Curve(CurveType.Flat100);
			HeadBrightness = new Curve(CurveType.Flat100);
			FillBrightness = new Curve(CurveType.Flat100);
		}

		#endregion

		#region IMorphPolygon

		private PolygonFillType _fillType;

		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"FillType")]
		[ProviderDescription(@"FillType")]
		[PropertyOrder(1)]
		public PolygonFillType FillType 
		{ 
			get
			{
				return _fillType;
			}
			set
			{
				_fillType = value;

				// Forward the fill type to the associated shape
				GetPointBasedShape().FillType = value;
			
				UpdatePolygonFillTypeAttributes();
			}
		}

		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"HeadLength")]
		[ProviderDescription(@"HeadLength")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(2)]
		public int HeadLength { get; set; }

		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"HeadDuration")]
		[ProviderDescription(@"HeadDuration")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(3)]
		
		public int HeadDuration { get; set; }

		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"Acceleration")]
		[ProviderDescription(@"Acceleration")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-10, 10, 1)]
		[PropertyOrder(4)]

		public int Acceleration { get; set; }

		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"HeadColor")]
		[ProviderDescription(@"HeadColor")]
		[PropertyOrder(5)]
		public ColorGradient HeadColor { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"HeadBrightness")]
		[ProviderDescription(@"HeadBrightness")]
		[PropertyOrder(6)]
		public Curve HeadBrightness { get; set; }

		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"TailColor")]
		[ProviderDescription(@"TailColor")]
		[PropertyOrder(7)]
		public ColorGradient TailColor { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"TailBrightness")]
		[ProviderDescription(@"TailBrightness")]
		[PropertyOrder(8)]
		public Curve TailBrightness { get; set; }

		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"PolygonColor")]
		[ProviderDescription(@"PolygonColor")]
		[PropertyOrder(9)]
		public ColorGradient FillColor
		{
			get;
			set;			
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"FillBrightness")]
		[ProviderDescription(@"FillBrightness")]
		[PropertyOrder(10)]
		public Curve FillBrightness { get; set; }

		private Polygon _polygon;

		[Browsable(false)]
		public Polygon Polygon 
		{
			get
			{
				return _polygon;
			}
			set
			{
				_polygon = value;
				if (value != null)
				{
					// Clear out the other shape types
					Line = null;
					Ellipse = null;
				}
			}
		}

		private Line _line;

		[Browsable(false)]
		public App.Polygon.Line Line
		{
			get
			{
				return _line;
			}
			set
			{
				_line = value;
				if (value != null)
				{
					// Clear out the other shape types
					Polygon = null;
					Ellipse = null;
				}
			}
		}

		private Ellipse _ellipse;

		[Browsable(false)]
		public Ellipse Ellipse
		{
			get
			{
				return _ellipse;
			}
			set
			{
				_ellipse = value;
				if (value != null)
				{
					// Clear out the other shape types
					Polygon = null;
					Line = null;
				}
			}
		}

		[Browsable(false)]
		public bool Removed { get; set; }
		
		[Browsable(false)]
		public double Time { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"StartOffset")]
		[ProviderDescription(@"StartOffset")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 100, 1)]
		[PropertyOrder(11)]
		public int StartOffset { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"Label")]
		[ProviderDescription(@"Label")]
		[PropertyOrder(12)]
		public string Label
		{
			get
			{
				string label = string.Empty;
				if (Polygon != null)
				{
					label = Polygon.Label;
				}
				else if (Line != null)
				{
					label = Line.Label;
				}
				else if (Ellipse != null)
				{
					label = Ellipse.Label;
				}

				return label;
			}
			set
			{
				if (Polygon != null)
				{
					Polygon.Label = value;
				}
				else if (Line != null)
				{
					Line.Label = value;
				}
				else if (Ellipse != null)
				{
					Ellipse.Label = value;
				}
			}
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public void LimitPoints(int width, int height)
		{
			if (Polygon != null)
			{
				Polygon.LimitPoints(width, height);
			}
			if (Line != null)
			{
				Line.LimitPoints(width, height);
			}
			if (Ellipse != null)
			{
				Ellipse.LimitPoints(width, height);
			}
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public IList<PolygonPoint> GetPolygonPoints()
		{
			IList<PolygonPoint> points;

			if (Polygon != null)
			{
				points = Polygon.Points;
			}
			else if (Line != null)
			{
				points = Line.Points;
			}
			else /*if (Ellipse != null)*/
			{
				points = Ellipse.Points;
			}

			return points;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>		
		public PointBasedShape GetPointBasedShape()
		{
			PointBasedShape shape;

			if (Polygon != null)
			{
				shape = Polygon;
			}
			else if (Line != null)
			{
				shape = Line;
			}
			else /*if (Ellipse != null)*/
			{
				shape = Ellipse;
			}

			return shape;
		}

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		public void Scale(double xScaleFactor, double yScaleFactor, int width, int height)
		{
			// Scale the associated shape using the specified scale factors
			GetPointBasedShape().Scale(xScaleFactor, yScaleFactor, width, height);
		}

		#endregion

		#region ICloneable

		/// <summary>
		/// Refer to MSDN documentation.
		/// </summary>		
		public object Clone()
		{
			IMorphPolygon clone = new MorphPolygon
			{
				FillType = FillType,
				HeadLength = HeadLength,
				HeadDuration = HeadDuration,
				Acceleration = Acceleration,
				HeadColor = new ColorGradient(HeadColor),
				TailColor = new ColorGradient(TailColor),
				FillColor = new ColorGradient(FillColor),
				Polygon = Polygon == null ? null : Polygon.Clone(),
				Line = Line == null ? null : Line.Clone(),
				Ellipse = Ellipse == null ? null : Ellipse.Clone(),
				Time = Time,
				Label = Label,
				StartOffset = StartOffset,
				TailBrightness = new Curve(TailBrightness),
				HeadBrightness = new Curve(HeadBrightness),
				FillBrightness = new Curve(FillBrightness)
			};

			return clone;
		}
		#endregion
				
		#region Private Methods

		/// <summary>
		/// Updates the browseable state of properties related to polygon fill type.
		/// </summary>
		private void UpdatePolygonFillTypeAttributes()
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(7)
			{
				{nameof(HeadLength), FillType == PolygonFillType.Wipe },
				{nameof(HeadDuration), FillType == PolygonFillType.Wipe },
				{nameof(Acceleration), FillType == PolygonFillType.Wipe },
				{nameof(HeadColor), FillType == PolygonFillType.Wipe },
				{nameof(TailColor), FillType == PolygonFillType.Wipe },
				{nameof(FillColor), (FillType == PolygonFillType.Solid || FillType == PolygonFillType.Outline) },

				{nameof(HeadBrightness), FillType == PolygonFillType.Wipe },
				{nameof(StartOffset), FillType == PolygonFillType.Wipe },
				{nameof(TailBrightness), FillType == PolygonFillType.Wipe },
				{nameof(FillBrightness), (FillType == PolygonFillType.Solid || FillType == PolygonFillType.Outline) },
			};
			SetBrowsable(propertyStates);
		}

		#endregion

		#region Public Static Properties

		/// <summary>
		/// Width of the display element associated with the effect.
		/// </summary>
		public static int BufferWidth { get; set; }

		/// <summary>
		/// Height of the display element associated with the effect.
		/// </summary>
		public static int BufferHeight { get; set; }
		
		#endregion
	}
}
