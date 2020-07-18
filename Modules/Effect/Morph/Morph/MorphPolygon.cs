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
			FillType = PolygonFillType.Wipe;
			HeadLength = 4;
			HeadDuration = 20;
			Acceleration = 0;
			HeadColor = new ColorGradient(System.Drawing.Color.White);
			TailColor = new ColorGradient(System.Drawing.Color.Red);
			FillColor = new ColorGradient(System.Drawing.Color.Red);
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
		[ProviderCategory(@"Wipe", 1)]
		[ProviderDisplayName(@"HeadColor")]
		[ProviderDescription(@"HeadColor")]
		[PropertyOrder(5)]
		public ColorGradient HeadColor { get; set; }

		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"TailColor")]
		[ProviderDescription(@"TailColor")]
		[PropertyOrder(6)]
		public ColorGradient TailColor { get; set; }

		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"PolygonColor")]
		[ProviderDescription(@"PolygonColor")]
		[PropertyOrder(7)]
		public ColorGradient FillColor
		{
			get;
			set;			
		}

		[Browsable(false)]
		public Polygon Polygon { get; set; }

		[Browsable(false)]
		public App.Polygon.Line Line { get; set; }

		[Browsable(false)]
		public bool Removed { get; set; }
		
		[Browsable(false)]
		public double Time { get; set; }

		/// <summary>
		/// Refer to interface documentation.
		/// </summary>
		[Value]
		[ProviderCategory(@"Configuration", 1)]
		[ProviderDisplayName(@"Label")]
		[ProviderDescription(@"Label")]
		[PropertyOrder(8)]
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
			else
			{
				points = Line.Points;
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
			else
			{
				shape = Line;
			}

			return shape;
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
				Time = Time,
				Label = Label,
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
			};
			SetBrowsable(propertyStates);
		}

		#endregion
	}
}
