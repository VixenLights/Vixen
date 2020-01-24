using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using Svg;
using Svg.Transforms;
using Vixen.Marks;
using Vixen.TypeConverters;

namespace VixenModules.Effect.Shapes
{
	public class Shapes : PixelEffectBase
	{
		private ShapesData _data;
		private List<ShapesClass> _shapes;
		private List<ShapesClass> _removeShapes;
		private int _shapesCount;
		private double _intervalPosFactor;
		private double _centerAngleSpeed;
		private double _angleSpeedVariation;
		private double _centerSizeSpeed;
		private double _sizeSpeedVariation;
		private int _minBuffer;
		private int _maxBuffer;
		private double _centerSpeed;
		private double _speedVariation;
		private double _minSpeed;
		private double _maxSpeed;
		private int _angle;
		private string _fileName;
		private readonly int _svgViewBoxSize = 200;
		private float _scaleShapeWidth;
		private float _scaleShapeHeight;
		private int _totalFrames;
		private IEnumerable<IMark> _marks = null;
		private List<int> _shapeFrame;

		public Shapes()
		{
			_data = new ShapesData();
			EnableTargetPositioning(true, true);
			UpdateAllAttributes();
		}

		public override bool IsDirty
		{
			get
			{
				if (FirstFillColors.Any(x => !x.CheckLibraryReference()))
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		#region String Setup properties

		[Value]
		public override StringOrientation StringOrientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Shape Effect properties

		[Value]
		[ProviderCategory("Config", 1)]
		[DisplayName(@"Shape Source")]
		[Description(@"Selects what source is used to determine Shapes.")]
		[PropertyOrder(0)]
		public ShapeMode ShapeMode
		{
			get
			{
				return _data.ShapeMode;
			}
			set
			{
				if (_data.ShapeMode != value)
				{
					_data.ShapeMode = value;
					UpdateShapeModeAttributes();
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Mark Collection")]
		[ProviderDescription(@"Mark Collection that has the phonemes to align to.")]
		[TypeConverter(typeof(IMarkCollectionNameConverter))]
		[PropertyEditor("SelectionEditor")]
		[PropertyOrder(1)]
		public string MarkCollectionId
		{
			get
			{
				return MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId)?.Name;
			}
			set
			{
				var newMarkCollection = MarkCollections.FirstOrDefault(x => x.Name.Equals(value));
				var id = newMarkCollection?.Id ?? Guid.Empty;
				if (!id.Equals(_data.MarkCollectionId))
				{
					var oldMarkCollection = MarkCollections.FirstOrDefault(x => x.Id.Equals(_data.MarkCollectionId));
					RemoveMarkCollectionListeners(oldMarkCollection);
					_data.MarkCollectionId = id;
					AddMarkCollectionListeners(newMarkCollection);
					IsDirty = true;
					OnPropertyChanged();
				}
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ShapeType")]
		[ProviderDescription(@"ShapeType")]
		[PropertyOrder(2)]
		public ShapeList ShapeList
		{
			get { return _data.ShapeList; }
			set
			{
				_data.ShapeList = value;
				UpdateShapeTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"GeometricShapes")]
		[ProviderDescription(@"GeometricShapes")]
		[PropertyOrder(3)]
		public GeometricShapesList GeometricShapesList
		{
			get { return _data.GeometricShapesList; }
			set
			{
				_data.GeometricShapesList = value;
				UpdateShapeTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ChristmasShapes")]
		[ProviderDescription(@"ChristmasShapes")]
		[PropertyOrder(4)]
		public ChristmasShapesList ChristmasShapesList
		{
			get { return _data.ChristmasShapesList; }
			set
			{
				_data.ChristmasShapesList = value;
				UpdateShapeTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"HalloweenShapes")]
		[ProviderDescription(@"HalloweenShapes")]
		[PropertyOrder(5)]
		public HalloweenShapesList HalloweenShapesList
		{
			get { return _data.HalloweenShapesList; }
			set
			{
				_data.HalloweenShapesList = value;
				UpdateShapeTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Filename")]
		[ProviderDescription(@"Filename")]
		[PropertyEditor("SvgPathEditor")]
		[PropertyOrder(7)]
		public string FileName
		{
			get { return _data.FileName; }
			set
			{
				_data.FileName = ConvertPath(value);
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Movement")]
		[ProviderDescription(@"Movement")]
		[PropertyOrder(8)]
		public ShapeType ShapeType
		{
			get { return _data.ShapeType; }
			set
			{
				_data.ShapeType = value;
				UpdatePositionAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Fade")]
		[ProviderDescription(@"Fade")]
		[PropertyOrder(9)]
		public FadeType FadeType
		{
			get { return _data.FadeType; }
			set
			{
				_data.FadeType = value;
				UpdateShapeTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"SizeMode")]
		[ProviderDescription(@"SizeMode")]
		[PropertyOrder(10)]
		public SizeMode SizeMode
		{
			get { return _data.SizeMode; }
			set
			{
				_data.SizeMode = value;
				UpdateShapeTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"VerticalOffset")]
		[ProviderDescription(@"VerticalOffset")]
		[PropertyOrder(11)]
		public Curve YOffsetCurve
		{
			get { return _data.YOffsetCurve; }
			set
			{
				_data.YOffsetCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"HorizontalOffset")]
		[ProviderDescription(@"HorizontalOffset")]
		[PropertyOrder(12)]
		public Curve XOffsetCurve
		{
			get { return _data.XOffsetCurve; }
			set
			{
				_data.XOffsetCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ShapeSize")]
		[ProviderDescription(@"ShapeSize")]
		[PropertyOrder(13)]
		public Curve SizeCurve
		{
			get { return _data.SizeCurve; }
			set
			{
				_data.SizeCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"CenterSize")]
		[ProviderDescription(@"CenterSize")]
		[PropertyOrder(14)]
		public Curve CenterSizeCurve
		{
			get { return _data.CenterSizeCurve; }
			set
			{
				_data.CenterSizeCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"SizeVariation")]
		[ProviderDescription(@"SizeVariation")]
		[PropertyOrder(15)]
		public Curve SizeVariationCurve
		{
			get { return _data.SizeVariationCurve; }
			set
			{
				_data.SizeVariationCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ShapeCount")]
		[ProviderDescription(@"ShapeCount")]
		[PropertyOrder(16)]
		public Curve ShapeCountCurve
		{
			get { return _data.ShapeCountCurve; }
			set
			{
				_data.ShapeCountCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ShapeCount")]
		[ProviderDescription(@"ShapeCount")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 50, 1)]
		[PropertyOrder(17)]
		public int ShapeCount
		{
			get { return _data.ShapeCount; }
			set
			{
				_data.ShapeCount = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"SizeChange")]
		[ProviderDescription(@"SizeChange")]
		[PropertyOrder(18)]
		public bool RemoveShape
		{
			get { return _data.RemoveShape; }
			set
			{
				_data.RemoveShape = value;
				UpdateShapeTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ScaleToGrid")]
		[ProviderDescription(@"ScaleToGrid")]
		[PropertyOrder(19)]
		public bool ScaleToGrid
		{
			get { return _data.ScaleToGrid; }
			set
			{
				_data.ScaleToGrid = value;
				UpdateShapeTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"RandomAngle")]
		[ProviderDescription(@"RandomAngle")]
		[PropertyOrder(20)]
		public bool RandomAngle
		{
			get { return _data.RandomAngle; }
			set
			{
				_data.RandomAngle = value;
				UpdateShapeTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"RandomSize")]
		[ProviderDescription(@"RandomSize")]
		[PropertyOrder(21)]
		public bool RandomSize
		{
			get { return _data.RandomSize; }
			set
			{
				_data.RandomSize = value;
				UpdateShapeTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ColorFill")]
		[ProviderDescription(@"ColorFill")]
		[PropertyOrder(22)]
		public bool Fill
		{
			get { return _data.Fill; }
			set
			{
				_data.Fill = value;
				UpdateShapeTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"ShapeSettings", 2)]
		[ProviderDisplayName(@"ShapeOutLine")]
		[ProviderDescription(@"ShapeOutLine")]
		[PropertyOrder(1)]
		public Curve ShapeOutLineCurve
		{
			get { return _data.ShapeOutLineCurve; }
			set
			{
				_data.ShapeOutLineCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"ShapeSettings", 2)]
		[ProviderDisplayName(@"ShapeOutLineSpace")]
		[ProviderDescription(@"ShapeOutLineSpace")]
		[PropertyOrder(2)]
		public Curve ShapeOutLineSpaceCurve
		{
			get { return _data.ShapeOutLineSpaceCurve; }
			set
			{
				_data.ShapeOutLineSpaceCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"ShapeSettings", 2)]
		[ProviderDisplayName(@"StarInsideSize")]
		[ProviderDescription(@"StarInsideSize")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(5, 200, 1)]
		[PropertyOrder(3)]
		public int StarInsideSize
		{
			get { return _data.StarInsideSize; }
			set
			{
				_data.StarInsideSize = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"ShapeSettings", 2)]
		[ProviderDisplayName(@"StarPoints")]
		[ProviderDescription(@"StarPoints")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(3, 20, 1)]
		[PropertyOrder(4)]
		public int StarPoints
		{
			get { return _data.StarPoints; }
			set
			{
				_data.StarPoints = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"ShapeSettings", 2)]
		[ProviderDisplayName(@"StarPoints")]
		[ProviderDescription(@"StarPoints")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(3, 20, 1)]
		[PropertyOrder(4)]
		public int NonIntersectingStarPoints
		{
			get { return _data.NonIntersectingStarPoints; }
			set
			{
				_data.NonIntersectingStarPoints = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"ShapeSettings", 2)]
		[ProviderDisplayName(@"PolygonSides")]
		[ProviderDescription(@"PolygonSides")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(3, 15, 1)]
		[PropertyOrder(5)]
		public int PolygonSides
		{
			get { return _data.PolygonSides; }
			set
			{
				_data.PolygonSides = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"ShapeSettings", 2)]
		[ProviderDisplayName(@"SkipPoints")]
		[ProviderDescription(@"SkipPoints")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(2, 5, 1)]
		[PropertyOrder(6)]
		public int SkipPoints
		{
			get { return _data.SkipPoints; }
			set
			{
				_data.SkipPoints = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"ShapeSettings", 2)]
		[ProviderDisplayName(@"ShapeSizeRatio")]
		[ProviderDescription(@"ShapeSizeRatio")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(7)]
		public int ShapeSizeRatio
		{
			get { return _data.ShapeSizeRatio; }
			set
			{
				_data.ShapeSizeRatio = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"ShapeSettings", 2)]
		[ProviderDisplayName(@"CrossSizeRatio")]
		[ProviderDescription(@"CrossSizeRatio")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(50, 100, 1)]
		[PropertyOrder(8)]
		public int CrossSizeRatio
		{
			get { return _data.CrossSizeRatio; }
			set
			{
				_data.CrossSizeRatio = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"ShapeSettings", 2)]
		[ProviderDisplayName(@"StrokeWidth")]
		[ProviderDescription(@"StrokeWidth")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 20, 1)]
		[PropertyOrder(9)]
		public int StrokeWidth
		{
			get { return _data.StrokeWidth; }
			set
			{
				_data.StrokeWidth = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"ShapeSettings", 2)]
		[ProviderDisplayName(@"RoundedCorners")]
		[ProviderDescription(@"RoundedCorners")]
		[PropertyOrder(10)]
		public bool RoundedCorner
		{
			get { return _data.RoundedCorner; }
			set
			{
				_data.RoundedCorner = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Speed Settings properties

		[Value]
		[ProviderCategory(@"SpeedSettings", 4)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyOrder(0)]
		public Curve CenterSpeedCurve
		{
			get { return _data.CenterSpeedCurve; }
			set
			{
				_data.CenterSpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"SpeedSettings", 4)]
		[ProviderDisplayName(@"SpeedVariation")]
		[ProviderDescription(@"SpeedVariation")]
		[PropertyOrder(1)]
		public Curve SpeedVariationCurve
		{
			get { return _data.SpeedVariationCurve; }
			set
			{
				_data.SpeedVariationCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"SpeedSettings", 4)]
		[ProviderDisplayName(@"SizeSpeed")]
		[ProviderDescription(@"SizeSpeed")]
		[PropertyOrder(2)]
		public Curve CenterSizeSpeedCurve
		{
			get { return _data.CenterSizeSpeedCurve; }
			set
			{
				_data.CenterSizeSpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

	   [Value]
		[ProviderCategory(@"SpeedSettings", 4)]
		[ProviderDisplayName(@"SizeSpeedVariation")]
		[ProviderDescription(@"SizeSpeedVariation")]
		[PropertyOrder(3)]
		public Curve SizeSpeedVariationCurve
		{
			get { return _data.SizeSpeedVariationCurve; }
			set
			{
				_data.SizeSpeedVariationCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"SpeedSettings", 4)]
		[ProviderDisplayName(@"AngleSpeed")]
		[ProviderDescription(@"AngleSpeed")]
		[PropertyOrder(4)]
		public Curve CenterAngleSpeedCurve
		{
			get { return _data.CenterAngleSpeedCurve; }
			set
			{
				_data.CenterAngleSpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"SpeedSettings", 4)]
		[ProviderDisplayName(@"AngleSpeedVariation")]
		[ProviderDescription(@"AngleSpeedVariation")]
		[PropertyOrder(5)]
		public Curve AngleSpeedVariationCurve
		{
			get { return _data.AngleSpeedVariationCurve; }
			set
			{
				_data.AngleSpeedVariationCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"SpeedSettings", 4)]
		[ProviderDisplayName(@"Angle")]
		[ProviderDescription(@"Angle")]
		[PropertyOrder(6)]
		public Curve AngleCurve
		{
			get { return _data.AngleCurve; }
			set
			{
				_data.AngleCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 5)]
		[ProviderDisplayName(@"StrokeFill")]
		[ProviderDescription(@"StrokeFill")]
		[PropertyOrder(1)]
		public bool StrokeFill
		{
			get { return _data.StrokeFill; }
			set
			{
				_data.StrokeFill = value;
				UpdateShapeTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 5)]
		[ProviderDisplayName(@"FirstFillColors")]
		[ProviderDescription(@"FirstFillColors")]
		[PropertyOrder(2)]
		public List<ColorGradient> FirstFillColors
		{
			get { return _data.FirstFillColors; }
			set
			{
				_data.FirstFillColors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 5)]
		[ProviderDisplayName(@"SecondFillColors")]
		[ProviderDescription(@"SecondFillColors")]
		[PropertyOrder(3)]
		public List<ColorGradient> SecondFillColors
		{
			get { return _data.SecondFillColors; }
			set
			{
				_data.SecondFillColors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 5)]
		[ProviderDisplayName(@"OutlineColors")]
		[ProviderDescription(@"OutlineColors")]
		[PropertyOrder(4)]
		public List<ColorGradient> OutlineColors
		{
			get { return _data.OutlineColors; }
			set
			{
				_data.OutlineColors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Brightness", 6)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
		[PropertyOrder(0)]
		public Curve LevelCurve
		{
			get { return _data.LevelCurve; }
			set
			{
				_data.LevelCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/shapes/"; }
		}

		#endregion

		#region Attributes

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as ShapesData;
				UpdateAllAttributes();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void UpdateAllAttributes()
		{
			UpdatePositionAttribute(false);
			UpdateShapeTypeAttribute(false);
			UpdateShapeModeAttributes(false);
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
		}
		private void UpdateShapeModeAttributes(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"MarkCollectionId", ShapeMode != ShapeMode.None},
				
				{"ShapeCount", ShapeMode == ShapeMode.RemoveShapesMarkCollection},

				{"ScaleToGrid", ShapeMode == ShapeMode.None},

				{"ShapeCountCurve", !ScaleToGrid && ShapeMode != ShapeMode.RemoveShapesMarkCollection},
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}

			if (ShapeMode != ShapeMode.None)
			{
				ScaleToGrid = false;
				UpdateShapeTypeAttribute();
			}
		}

		private void UpdatePositionAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(4)
			{
				{"CenterSpeedCurve", ShapeType != ShapeType.None},

				{"SpeedVariationCurve", ShapeType != ShapeType.None},

				{"YOffsetCurve", ShapeType == ShapeType.None},

				{"XOffsetCurve", ShapeType == ShapeType.None}
			};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}
		
		private void UpdateShapeTypeAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(42)
			{
				{"GeometricShapesList", ShapeList == ShapeList.GeometricShapes},

				{"ChristmasShapesList", ShapeList == ShapeList.ChristmasShapes},

				{"HalloweenShapesList", ShapeList == ShapeList.HalloweenShapes},

				{"FileName", ShapeList == ShapeList.File},

				{"PolygonSides", ShapeList == ShapeList.GeometricShapes && GeometricShapesList == GeometricShapesList.Polygon},

				{"SkipPoints", ShapeList == ShapeList.GeometricShapes && GeometricShapesList == GeometricShapesList.Star},

				{"StarInsideSize", ShapeList == ShapeList.GeometricShapes && GeometricShapesList == GeometricShapesList.NonIntersectingStar},

				{"StarPoints", ShapeList == ShapeList.GeometricShapes && GeometricShapesList == GeometricShapesList.Star},

				{"NonIntersectingStarPoints", ShapeList == ShapeList.GeometricShapes && GeometricShapesList == GeometricShapesList.NonIntersectingStar},

				{"ShapeSizeRatio", !ScaleToGrid && ShapeList == ShapeList.GeometricShapes && (GeometricShapesList == GeometricShapesList.Rectangle || GeometricShapesList == GeometricShapesList.Ellipse)},

				{"CrossSizeRatio", !ScaleToGrid && ShapeList == ShapeList.GeometricShapes && GeometricShapesList == GeometricShapesList.Cross},

				{"FirstFillColors", ShapeList != ShapeList.File && Fill},

				{"SecondFillColors", ShapeList != ShapeList.File && Fill && ShapeList != ShapeList.GeometricShapes && (ShapeList == ShapeList.ChristmasShapes && ChristmasShapesList != ChristmasShapesList.SnowFlake&& ChristmasShapesList != ChristmasShapesList.SnowFlake2) || (ShapeList == ShapeList.HalloweenShapes && HalloweenShapesList != HalloweenShapesList.Web)},

				{"RoundedCorner", ShapeList == ShapeList.GeometricShapes},

				{"OutlineColors", StrokeFill},

				{"Fill", ShapeList != ShapeList.File},

				{"StrokeFill", ShapeList == ShapeList.File},

				{"SizeCurve", !ScaleToGrid && !RemoveShape && !RandomSize},

				{"MaxSizeCurve", !ScaleToGrid && RemoveShape},

				{"CenterSpeedCurve", !ScaleToGrid && ShapeType != ShapeType.None},

				{"SpeedVariationCurve", !ScaleToGrid && ShapeType != ShapeType.None},

				{"YOffsetCurve", !ScaleToGrid && ShapeType == ShapeType.None},

				{"XOffsetCurve", !ScaleToGrid && ShapeType == ShapeType.None},

				{"ShapeCountCurve", !ScaleToGrid && ShapeMode != ShapeMode.RemoveShapesMarkCollection},

				{"CenterSizeSpeedCurve", !ScaleToGrid && (RemoveShape || RandomSize)},

				{"SizeSpeedVariationCurve", !ScaleToGrid && (RemoveShape || RandomSize)},

				{"CenterAngleSpeedCurve", !ScaleToGrid && RandomAngle},

				{"AngleSpeedVariationCurve", !ScaleToGrid && RandomAngle},

				{"AngleCurve", !ScaleToGrid && !RandomAngle},

				{"ShapeType", !ScaleToGrid},

				{"ShapeOutLineCurve", StrokeFill},

				{"ShapeOutLineSpaceCurve", StrokeFill},

				{"StrokeWidth", StrokeFill},

				{"RandomAngle", !ScaleToGrid},

				{"FadeType", ShapeList != ShapeList.File && !ScaleToGrid},

				{"SizeVariationCurve", !ScaleToGrid && (RemoveShape || RandomSize)},

				{"CenterSizeCurve", !ScaleToGrid &&  (RemoveShape || RandomSize)},

				{"SizeMode", !ScaleToGrid && RemoveShape},

				{"RemoveShape", !ScaleToGrid},

				{"RandomSize", !ScaleToGrid && !RemoveShape},

				{"ShapeCount", ShapeMode == ShapeMode.RemoveShapesMarkCollection}

			};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		#endregion

		#region Pre and Post processing

		private string ConvertPath(string path)
		{
			if(string.IsNullOrEmpty(path))
			{
				return path;
			}
			if (Path.IsPathRooted(path))
			{
				return CopyLocal(path);
			}

			return path;
		}

		private string CopyLocal(string path)
		{
			string name = Path.GetFileName(path);
			var destPath = Path.Combine(ShapesDescriptor.ModulePath, name);
			if (path != destPath)
			{
				File.Copy(path, destPath, true);
			}
			return name;
		}

		protected override void SetupRender()
		{
			_minBuffer = Math.Min(BufferHt, BufferWi);
			_maxBuffer = Math.Max(BufferHt, BufferWi);
			_shapes = new List<ShapesClass>();
			_removeShapes = new List<ShapesClass>();
			_shapeFrame = new List<int>();
			_shapesCount = 0;
			_totalFrames = GetNumberFrames();

			if (FileName != null && ShapeList == ShapeList.File)
			{
				var filePath = Path.Combine(ShapesDescriptor.ModulePath, FileName);
				if (File.Exists(filePath))
				{
					_fileName = filePath;
				}
				else
				{
					// Logging.Error("File is missing or invalid path. {0}", filePath);
					FileName = "";
				}
			}

			if (ShapeMode != ShapeMode.None)
			{
				SetupMarks();
				
				if (_marks != null)
				{
					foreach (var mark in _marks)
					{
						_shapeFrame.Add((int)((mark.StartTime.TotalMilliseconds - StartTime.TotalMilliseconds) / 50));
					}
				}
			}
		}

		protected override void CleanUpRender()
		{
			_shapes = null;
			_removeShapes = null;
			_shapeFrame = null;
		}

		#endregion

		#region Render Effect

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			var bufferHt = BufferHt;
			var bufferWi = BufferWi;
			using (var bitmap = new Bitmap(bufferWi, bufferHt))
			{
				InitialRender(frame, bitmap, bufferHt, bufferWi);
				double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				FastPixel.FastPixel fp = new FastPixel.FastPixel(bitmap);
				fp.Lock();
				for (int x = 0; x < bufferWi; x++)
				{
					for(int y = 0; y < bufferHt; y++)
					{
						CalculatePixel(x, y, ref bufferHt, fp, level, frameBuffer);
					}
				}
				fp.Unlock(false);
				fp.Dispose();
			}

		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var bufferHt = BufferHt;
			var bufferWi = BufferWi;
			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;
				double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				using (var bitmap = new Bitmap(bufferWi, bufferHt))
				{
					InitialRender(frame, bitmap, bufferHt, bufferWi);
					FastPixel.FastPixel fp = new FastPixel.FastPixel(bitmap);
					fp.Lock();
					foreach (var elementLocation in frameBuffer.ElementLocations)
					{
						CalculatePixel(elementLocation.X, elementLocation.Y, ref bufferHt, fp, level, frameBuffer);
					}
					fp.Unlock(false);
					fp.Dispose();
				}
			}
		}

		private void CalculatePixel(int x, int y, ref int bufferHt, FastPixel.FastPixel bitmap, double level, IPixelFrameBuffer frameBuffer)
		{
			int yCoord = y;
			int xCoord = x;
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				// Flip me over and offset my coordinates so I can act like the string version
				y = Math.Abs((BufferHtOffset - y) + (bufferHt - 1 + BufferHtOffset));
				y = y - BufferHtOffset;
				x = x - BufferWiOffset;
			}
			Color color = bitmap.GetPixel(x, bufferHt - y - 1);

			if (color.R != 0 || color.G != 0 || color.B != 0)
			{
				if (level < 1)
				{
					var hsv = HSV.FromRGB(color);
					hsv.V = hsv.V * level;
					color = hsv.ToRGB();
				}
				frameBuffer.SetPixel(xCoord, yCoord, color);
			}
		}

		#endregion

		#region Pre Render

		private void InitialRender(int frame, Bitmap bitmap, int bufferHt, int bufferWi)
		{
			_intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;
			_centerAngleSpeed = CalculateCenterAngleSpeed(_intervalPosFactor);
			_angleSpeedVariation = CalculateAngleSpeedVariation(_intervalPosFactor);
			_centerSizeSpeed = CalculateCenterSizeSpeed(_intervalPosFactor);
			_sizeSpeedVariation = CalculateSizeSpeedVariation(_intervalPosFactor);
			_centerSpeed = CalculateCenterSpeed(_intervalPosFactor);
			_speedVariation = CalculateSpeedVariation(_intervalPosFactor);
			_angle = CalculateAngle(_intervalPosFactor);
			double pos = GetEffectTimeIntervalPosition(frame);

			double minAngleSpeed = _centerAngleSpeed - (_angleSpeedVariation / 2);
			double maxAngleSpeed = _centerAngleSpeed + (_angleSpeedVariation / 2);
			double minSizeSpeed = _centerSizeSpeed - (_sizeSpeedVariation / 2);
			double maxSizeSpeed = _centerSizeSpeed + (_sizeSpeedVariation / 2);
			_minSpeed = _centerSpeed - (_speedVariation / 2);
			_maxSpeed = _centerSpeed + (_speedVariation / 2);

			// Create new shapes and maintain maximum number as per users selection.
			int adjustedPixelCount = 0;
			switch (ShapeMode)
			{
				case ShapeMode.None:
					if (ScaleToGrid)
					{
						// Scale to grid only requires a single shape so ensure there is only one created.
						adjustedPixelCount = _shapesCount = 1;
					}
					else
					{
						_shapesCount = CalculateShapeCount(_intervalPosFactor);
						if (frame / 3 >= _shapesCount || !RemoveShape)
						{
							adjustedPixelCount = _shapesCount;
						}
						else
						{
							if (frame % 3 == 0) adjustedPixelCount = 1;
						}
					}

					for (int i = 0; i < adjustedPixelCount; i++)
					{
						// Create new Shapes if shapes are below Shapecount.
						if (_shapes.Count < _shapesCount) CreateShapes(bufferHt, bufferWi);
						else
							break;
					}
					break;
				case ShapeMode.CreateShapesMarkCollection:
					// Create shapes based on selected Mark Collection and Marks
					foreach (var shapeFrame in _shapeFrame)
					{
						if (frame == shapeFrame)
						{
							CreateShapes(bufferHt, bufferWi);
							_shapesCount = _shapes.Count;
						}
					}
					break;
				case ShapeMode.RemoveShapesMarkCollection:
					if (frame == 0)
					{
						// Create marks at frame 0 based on Shape count.
						_shapesCount = ShapeCount;
						for (int i = 0; i < ShapeCount; i++)
							if (_shapes.Count < ShapeCount) CreateShapes(bufferHt, bufferWi);
							else
								break;
					}
					break;
			}

			// Update Shape properties like location, radius and speed.
			UpdateShapes(minAngleSpeed, maxAngleSpeed, minSizeSpeed, maxSizeSpeed, bufferHt, bufferWi);

			// Remove any shapes that have been added to the _removeShapes variable.
			RemoveShapes(frame, bufferHt, bufferWi);

			// Go through all used Shapes and adjust settings and then Draw SVG to Bitmap.
			foreach (var shape in _shapes)
			{
				_scaleShapeWidth = (float)(((_maxBuffer * 2) / shape.SvgImage.ViewBox.Width) * (shape.Size / (_maxBuffer * 2)));
				_scaleShapeHeight = (float)(((_minBuffer * 2) / shape.SvgImage.ViewBox.Height) * (shape.Size / (_minBuffer * 2)));

				if (ShapeList != ShapeList.File)
				{
					foreach (var child in shape.SvgImage.Children)
					{
						// Adjusts Shape properties based on effect settings
						if (Fill)
						{
							foreach (var descendant in shape.SvgImage.Descendants())
							{
								if (descendant.ID == null) continue;
								// Will add random color form first and second color fill to all SVG Paths that contain appropiate ID.
								if (descendant.ID.Contains("firstFill")) descendant.Fill = new SvgColourServer(FirstFillColors[shape.FirstFillColorIndex].GetColorAt(pos));
										
								if (descendant.ID.Contains("secondFill")) descendant.Fill = new SvgColourServer(SecondFillColors[shape.SecondFillColorIndex].GetColorAt(pos));
									
								// Used to Fade individual Shapes. This will adjust the assigned colors brightness.
								if (FadeType != FadeType.None && descendant.Fill != SvgPaintServer.None)
								{
									HSV fadeColor = HSV.FromRGB(ColorTranslator.FromHtml(descendant.Fill.ToString()));
									fadeColor.V *= shape.Fade;
									descendant.Fill = new SvgColourServer(fadeColor.ToRGB());
								}
							}
						}
						else
						{
							// When there is no Color Fill this allows the shape to show other shapes through.
							// If you don't what other shapes to show through then just select Color Fill and change Fill color to Black.
							shape.SvgImage.Children[0].FillOpacity = 0;
						}

						// Adjusts the stroke fill and width for all embedded SVG's.
						if (StrokeFill)
						{
							child.Stroke = new SvgColourServer(OutlineColors[shape.StrokeColorIndex]
								.GetColorAt(pos));
							child.StrokeWidth = TargetPositioning == TargetPositioningType.Locations
								? (SvgUnit) (new SvgUnit(StrokeWidth / _scaleShapeWidth) * _minBuffer / StringCount /
								             shape.LocationRatio1)
								: (SvgUnit) (new SvgUnit(StrokeWidth / _scaleShapeWidth));

							if (FadeType != FadeType.None)
							{
								HSV fadeColor = HSV.FromRGB(ColorTranslator.FromHtml(child.Stroke.ToString()));
								fadeColor.V *= shape.Fade;
								child.Stroke = new SvgColourServer(fadeColor.ToRGB());
							}

							if ((CalculateShapeOutLine(_intervalPosFactor) < 100) || (CalculateShapeOutLineSpace(_intervalPosFactor) > 0))
							{
								child.StrokeDashArray = new SvgUnitCollection
								{
									CalculateShapeOutLine(_intervalPosFactor),
									CalculateShapeOutLineSpace(_intervalPosFactor)
								};
							}
						}
					}
				}
				else
				{
					// Stroke fill and width for imported SVG Images from a file.
					if (StrokeFill)
					{
						shape.SvgImage.Stroke = new SvgColourServer(OutlineColors[shape.StrokeColorIndex]
							.GetColorAt(pos));
						shape.SvgImage.StrokeWidth = (SvgUnit) (new SvgUnit(StrokeWidth / _scaleShapeWidth) * _minBuffer / StringCount /
						                               shape.LocationRatio1);

						if ((CalculateShapeOutLine(_intervalPosFactor) < 100) || (CalculateShapeOutLineSpace(_intervalPosFactor) > 0))
						{
							shape.SvgImage.StrokeDashArray = new SvgUnitCollection
							{
								CalculateShapeOutLine(_intervalPosFactor),
								CalculateShapeOutLineSpace(_intervalPosFactor)
							};
						}
					}
				}
				
				if (!ScaleToGrid)
				{
					if (shape.SvgImage.Transforms == null)
					{
						shape.SvgImage.Transforms = new SvgTransformCollection();
					}
					// Rotates and scasles the shape, scaling is used as SVG images may have different sizes, especially for Imported files as they are not controlled.
					shape.SvgImage.Transforms[0] = new SvgRotate(shape.RotateAngle, shape.SvgImage.ViewBox.Width * shape.LocationRatio1 * _scaleShapeWidth / 2, shape.SvgImage.ViewBox.Height * shape.LocationRatio1 * _scaleShapeHeight / 2);
					shape.SvgImage.Transforms[1] = new SvgScale(_scaleShapeWidth, _scaleShapeHeight);
				}

				// Ensures a better image vs speed, although there wasnt any differance in render time between this
				// and optimized speed.
				shape.SvgImage.ShapeRendering = SvgShapeRendering.CrispEdges;

				double locationX;
				double locationY;
				if (_shapes.Count == 1 && ShapeType == ShapeType.None && !ScaleToGrid)
				{
					// Shape is added to center of grid.
					locationX = (double)bufferWi / 2;
					locationY = (double)bufferHt / 2; 
				}
				else
				{
					// Sets the shape location based off user or random location settings.
					locationX = shape.LocationX;
					locationY = shape.LocationY;
				}

				using (Graphics g = Graphics.FromImage(bitmap))
				{
					// Adjust position based on users x and y offset.
					int xOffset = 0;
					int yOffset = 0;
					if (ShapeType == ShapeType.None && !ScaleToGrid)
					{
						xOffset = CalculateXOffset(_intervalPosFactor, bitmap.Width);
						yOffset = CalculateYOffset(_intervalPosFactor, bitmap.Height);
					}
					
					// Draw SVG onto main bitmap
					g.DrawImage(shape.SvgImage.Draw(),
						ScaleToGrid
							? new Point(0, 0)
							: new Point(
								(int) (locationX + xOffset -
								       Math.Ceiling(shape.SvgImage.ViewBox.Width * shape.LocationRatio1 * _scaleShapeWidth / 2)),
								(int) (locationY + yOffset -
								       Math.Ceiling(shape.SvgImage.ViewBox.Height * shape.LocationRatio1 * _scaleShapeHeight / 2))));
				}
			}
		}

		#endregion

		#region Create Shapes

		private void CreateShapes(int bufferHt, int bufferWi)
		{
			ShapesClass m = new ShapesClass();

			// Sets starting location of svg image
			m.LocationX = Rand(0, bufferWi - 1);
			m.LocationY = Rand(0, bufferHt - 1);

			//Sets initial speed of svg image
			double speed = RandDouble() * (_maxSpeed - _minSpeed) + _minSpeed;
			double vx = RandDouble() * speed;
			double vy = RandDouble() * speed;
			if (Rand(0, 2) == 0) vx = -vx;
			if (Rand(0, 2) == 0) vy = -vy;
			if (ShapeType != ShapeType.None)
			{
				m.VelocityX = vx;
				m.VelocityY = vy;
			}

			// Starting angle based on position of angle curve
			_angle = CalculateAngle(_intervalPosFactor);
			m.Scale = 1;

			if (ShapeList == ShapeList.GeometricShapes)
			{
				// The embedded geometric Shapes require a SVG Viewbox created as they are designed around this size.
				m.SvgImage = new SvgDocument{ViewBox = new SvgViewBox(0, 0, _svgViewBoxSize, _svgViewBoxSize)};
			}

			// Get the shape from users selected source.
			Array enumValues;
			switch (ShapeList)
			{
				case ShapeList.GeometricShapes:
					enumValues = Enum.GetValues(typeof(GeometricShapesList));
					m.Shape = GeometricShapesList == GeometricShapesList.Random
						? ((GeometricShapesList) enumValues.GetValue(Rand(1, enumValues.Length))).ToString()
						: GeometricShapesList.ToString();
					int radius = _svgViewBoxSize / 2 - (StrokeWidth + 1);
					switch (m.Shape)
					{
						case "Square":
							m.SvgImage.Children.Add(GetGeometricShape.CreateSvgRectangle(radius, 1));
							break;
						case "Arrow":
							m.SvgImage.Children.Add(GetGeometricShape.CreateSvgArrow());
							break;
						case "Triangle":
							m.SvgImage.Children.Add(GetGeometricShape.CreateSvgTriangle(radius));
							break;
						case "Circle":
							m.SvgImage.Children.Add(GetGeometricShape.CreateSvgCircle(radius));
							break;
						case "Rectangle":
							m.SvgImage.Children.Add(GetGeometricShape.CreateSvgRectangle(radius, (float) ShapeSizeRatio / 100));
							break;
						case "Ellipse":
							m.SvgImage.Children.Add(GetGeometricShape.CreateSvgEllipse(radius, (float) ShapeSizeRatio / 100));
							break;
						case "Cross":
							m.SvgImage.Children.Add(GetGeometricShape.CreateSvgCross((float) CrossSizeRatio / 100));
							break;
						case "Star":
							m.SvgImage.Children.Add(GetGeometricShape.CreateSvgMultiStar(radius, StarPoints, SkipPoints));
							break;
						case "ConcaveStar":
							m.SvgImage = getXMLShape("Geometric." + m.Shape);
							break;
						case "NonIntersectingStar":
							m.SvgImage.Children.Add(
								GetGeometricShape.CreateSvgNonIntersectingStar(radius, NonIntersectingStarPoints, StarInsideSize, false));
							break;
						case "NorthStar":
							m.SvgImage.Children.Add(GetGeometricShape.CreateSvgNonIntersectingStar(radius, 8, 42, true));
							break;
						case "Polygon":
							m.SvgImage.Children.Add(GetGeometricShape.CreateSvgPolygon(radius, PolygonSides));
							break;
						case "Heart":
							m.SvgImage = getXMLShape("Geometric." + m.Shape);
							break;
					}
					m.SvgImage.Children[0].ID = "firstFillColor";
					break;

				case ShapeList.ChristmasShapes:
					enumValues = Enum.GetValues(typeof(ChristmasShapesList));
					m.Shape = ChristmasShapesList == ChristmasShapesList.Random
						? ((ChristmasShapesList) enumValues.GetValue(Rand(1, enumValues.Length))).ToString()
						: ChristmasShapesList.ToString();
					m.SvgImage = getXMLShape("Christmas." + m.Shape);
					break;

				case ShapeList.HalloweenShapes:
					enumValues = Enum.GetValues(typeof(HalloweenShapesList));
					m.Shape = HalloweenShapesList == HalloweenShapesList.Random
						? ((HalloweenShapesList) enumValues.GetValue(Rand(1, enumValues.Length))).ToString()
						: HalloweenShapesList.ToString();
					m.SvgImage = getXMLShape("Halloween." + m.Shape);
					break;

				case ShapeList.File:
					m.SvgImage = SvgDocument.Open(_fileName);
					break;
			}

			if ((int) m.SvgImage.ViewBox.Width == 0) m.SvgImage.ViewBox = new SvgViewBox(0, 0, m.SvgImage.Width, m.SvgImage.Height);
			
			// The following is used to setting size, scale, position settings depending on if the SVG image is scaled to grid or not.
			if (!ScaleToGrid)
			{
				// These adjustments ensure that any siazed SVG image can be used and adjusted to a standard size set by the user.
				// That way large images are displayed the same size as smaller images.
				m.SvgImage.ViewBox = new SvgViewBox(-((m.SvgImage.ViewBox.Width * 1.42f - m.SvgImage.ViewBox.Width) / 2),
					-((m.SvgImage.ViewBox.Height * 1.42f - m.SvgImage.ViewBox.Height) / 2), m.SvgImage.ViewBox.Width * 1.42f,
					m.SvgImage.ViewBox.Height * 1.42f);
				m.Scale = _svgViewBoxSize / m.SvgImage.ViewBox.Height;
				if (m.SvgImage.Transforms == null)
				{
					m.SvgImage.Transforms = new SvgTransformCollection();
				}
				m.SvgImage.Transforms.Add(new SvgRotate(0, (int) (m.SvgImage.Width / 2), (int) (m.SvgImage.Height / 2)));
				m.LocationRatio = StringCount / m.SvgImage.ViewBox.Height * 2 * ((float) _minBuffer / StringCount);
				m.LocationRatio1 = m.LocationRatio * m.LocationRatio;
				m.SvgImage.Height = new SvgUnit(m.LocationRatio).ToPercentage();
				m.SvgImage.Width = new SvgUnit(m.LocationRatio).ToPercentage();
				m.SvgImage.Transforms.Add(new SvgScale(m.Scale));
			}
			else
			{
				m.SvgImage.Height = new SvgUnit(bufferHt);
				m.SvgImage.Width = new SvgUnit(bufferWi);
				m.SvgImage.AspectRatio = new SvgAspectRatio(SvgPreserveAspectRatio.none);
				m.LocationRatio = m.LocationRatio1 = 1;
				m.LocationX = m.LocationY = 0;
			}

			// Adds rounded corners to Geometric Shapes only, no point doing it to the others.
			// Rounded corners are really only visible on sharpe corners that have think outline.
			if (RoundedCorner && ShapeList == ShapeList.GeometricShapes) m.SvgImage.StrokeLineJoin = SvgStrokeLineJoin.Round;

			var centerSize = CalculateCenterSize(_intervalPosFactor, m.LocationRatio1);
			var sizeVariation = CalculateSizeVariation(_intervalPosFactor, m.LocationRatio1);
			var minSize = centerSize - (sizeVariation / 2);
			var maxSize = centerSize + (sizeVariation / 2);
			if (minSize < 1) minSize = 1;

			switch (SizeMode)
			{
				case SizeMode.In:
					m.SizeMode = "In";
					break;
				case SizeMode.Out:
					m.SizeMode = "Out";
					break;
				case SizeMode.Random:
					m.SizeMode = Rand(0, 2) == 0 ? "In" : "Out";
					break;
			}

			// Sets starting size of shape
			if (RemoveShape && !ScaleToGrid)
			{
				m.Size = m.SizeMode == "In" ? maxSize : minSize;
				m.FadeStep = 0.9f / (maxSize - minSize);
			}
			else
			{
				m.FadeStep = 0.9f / _totalFrames;
				m.Size = RandomSize
					? Rand(minSize, maxSize)
					: CalculateSize(_intervalPosFactor,
						m.LocationRatio1);
			}

			if (ShapeType == ShapeType.Bounce)
			{
				// Ensures any new shape is added to the grid and not protuding off the grid where it could get stuck.
				var locationOffset = (m.SvgImage.ViewBox.Width / 1.42 * m.LocationRatio1 * _scaleShapeWidth / 2);
				bool notCreated;
				int i = 0;
				do
				{
					notCreated = false;
					m.LocationX = Rand(0, bufferWi - 1);
					m.LocationY = Rand(0, bufferHt - 1);
					if (m.LocationX - locationOffset < 0 || m.LocationY - locationOffset < 0 ||
					    m.LocationX + locationOffset >= BufferWi || m.LocationY + locationOffset >= bufferHt) notCreated = true;
					i++;
				} while (notCreated && i < 10000);
			}

			// Allocates Random Colors to the shape.
			m.FirstFillColorIndex = Rand(0, FirstFillColors.Count);
			m.SecondFillColorIndex = Rand(0, SecondFillColors.Count);
			m.StrokeColorIndex = Rand(0, OutlineColors.Count);

			// Sets random starting angle and rotation direction
			m.RotateAngle = RandomAngle ? Rand(0, 360) : 0;
			m.RotateCW = Rand(0, 2);
			if (ShapeList != ShapeList.File)
			{
				switch (FadeType)
				{
					case FadeType.In:
						m.Fade = 0.05f;
						m.FadeType = "In";
						break;
					case FadeType.Out:
						m.Fade = 1;
						m.FadeType = "Out";
						break;
					case FadeType.Random:
						if (Rand(0, 2) == 0)
						{
							m.FadeType = "In";
							m.Fade = 0.05f;
						}
						else
						{
							m.FadeType = "Out";
							m.Fade = 1;
						}
						break;
				}
			}

			// Finally adds a new Shape to _shapes class.
			_shapes.Add(m);
		}

		#endregion

		#region Update Shapes

		private void UpdateShapes(double minAngleSpeed, double maxAngleSpeed, double minSizeSpeed, double maxSizeSpeed, int bufferHt, int bufferWi)
		{
			// Remove shapes if the shape count has been reduced.
			while (_shapesCount < _shapes.Count - _removeShapes.Count)
			{
				// Removes random shape.
				_shapes.Remove(_shapes[Rand(0, _shapes.Count)]);
			}

			double angleStartSpeed = (maxAngleSpeed - minAngleSpeed) + minAngleSpeed;
			double sizeStartSpeed = ((maxSizeSpeed - minSizeSpeed) + minSizeSpeed) / 10;

			foreach (var shape in _shapes)
			{
				if (ShapeType != ShapeType.None)
				{
					// Adjust shape speeds when user adjust Speed curve
					if (_centerSpeed > CalculateCenterSpeed(_intervalPosFactor - 1) ||
					    _centerSpeed < CalculateCenterSpeed(_intervalPosFactor - 1))
					{
						double ratio = CalculateCenterSpeed(_intervalPosFactor) / CalculateCenterSpeed(_intervalPosFactor - 1);
						shape.VelocityX *= ratio;
						shape.VelocityY *= ratio;
					}

					if (_speedVariation > CalculateSpeedVariation(_intervalPosFactor - 1) ||
					    _speedVariation < CalculateSpeedVariation(_intervalPosFactor - 1))
					{
						double ratio = CalculateSpeedVariation(_intervalPosFactor) / CalculateSpeedVariation(_intervalPosFactor - 1);
						shape.VelocityX *= ratio;
						shape.VelocityY *= ratio;
					}
				}    

				double angleSpeed = RandDouble() * angleStartSpeed;
				double sizeSpeed = RandDouble() * sizeStartSpeed;
				
				var centerSize = CalculateCenterSize(_intervalPosFactor, shape.LocationRatio1);
				var sizeVariation = CalculateSizeVariation(_intervalPosFactor, shape.LocationRatio1);
				var minSize = centerSize - (sizeVariation / 2);
				var maxSize = centerSize + (sizeVariation / 2);
				
				if (RemoveShape && !ScaleToGrid)
				{
					switch (shape.SizeMode)
					{
						case "In":
							if (sizeSpeed > 1) shape.Size /= sizeSpeed;
							break;
						default:
							if (sizeSpeed > 1) shape.Size *= sizeSpeed;
							break;
					}

					// Remove shape if its bigger or smaller then the limits and then create new shape.
					if (shape.Size > maxSize || shape.Size < minSize) _removeShapes.Add(shape);
				}
				else
				{
					if (!RandomSize && !RemoveShape && !ScaleToGrid)
					{
						shape.Size = CalculateSize(_intervalPosFactor, shape.LocationRatio1);
					}
				}
				
				// Adjust shape angle.
				if (RandomAngle)
				{
					shape.RotateAngle = shape.RotateCW == 1
						? (shape.RotateAngle > 358 ? 0 : shape.RotateAngle + (int) angleSpeed)
						: (shape.RotateAngle < 2 ? 360 : shape.RotateAngle - (int) angleSpeed);
				}
				else
				{
					shape.RotateAngle = _angle;
				}

				if (ShapeList != ShapeList.File)
				{
					switch (shape.FadeType)
					{
						case "In":
							if (RemoveShape && !ScaleToGrid)
							{
								// Adjust fading exponentially so it looks better on live lights.
								shape.Fade = shape.SizeMode == "In"
									? (float) Math.Pow(1 - (1 / (float) (maxSize - minSize / 2) * (shape.Size - (float) minSize / 2)), 2)
									: (float)Math.Pow((1 / (float)(maxSize - minSize / 2) * (shape.Size - (float)minSize / 2)), 2);
							}
							else
							{
								shape.Fade += shape.FadeStep;
							}

							if (shape.Fade > .95) _removeShapes.Add(shape);
							break;
						case "Out":
							if (RemoveShape && !ScaleToGrid)
							{
								// Adjust fading exponentially so it looks better on live lights.
								shape.Fade = shape.SizeMode == "In"
									? (float) Math.Pow(1 / (float) (maxSize - minSize / 2) * (shape.Size - (float) minSize / 2), 2)
									: (float) Math.Pow(1 - (1 / (float) (maxSize - minSize / 2) * (shape.Size - (float) minSize / 2)), 2);
							}
							else
							{
								shape.Fade -= shape.FadeStep;
							}
							if (shape.Fade < 0.05) _removeShapes.Add(shape);
							break;
					}
				}

				// Move the shape.
				if (ShapeType != ShapeType.None)
				{
					shape.LocationX = shape.LocationX + shape.VelocityX;
					shape.LocationY = shape.LocationY + shape.VelocityY;
					var locationOffset = (shape.SvgImage.ViewBox.Width / 1.42 * shape.LocationRatio1 * _scaleShapeWidth / 2);

					switch (ShapeType)
					{
						case ShapeType.Bounce:
							if (shape.LocationX - locationOffset < 0)
							{
								shape.VelocityX = -shape.VelocityX;
							}
							else if (shape.LocationX + locationOffset >= bufferWi)
							{
								shape.VelocityX = -shape.VelocityX;
							}
							if (shape.LocationY - locationOffset < 0)
							{
								shape.VelocityY = -shape.VelocityY;
							}
							else if (shape.LocationY + locationOffset >= bufferHt)
							{
								shape.VelocityY = -shape.VelocityY;
							}
							break;

						case ShapeType.Wrap:

							if (shape.LocationX + locationOffset < 0)
							{
								shape.LocationX = bufferWi + locationOffset;
							}
							if (shape.LocationY + locationOffset < 0)
							{
								shape.LocationY = bufferHt + locationOffset;
							}
							if (shape.LocationX - locationOffset > bufferWi)
							{
								shape.LocationX = 0 - locationOffset;
							}
							if (shape.LocationY - locationOffset > bufferHt)
							{
								shape.LocationY = 0 - locationOffset;
							}
							break;
					}
				}
			}
		}

		#endregion

		#region Helpers

		private SvgDocument getXMLShape(string svgImage)
		{
			// Gets SVG data from embedded Shape xml files within the Shape Effect project and conerts to an SVG image. 
			Stream s = Assembly.GetExecutingAssembly()
				.GetManifestResourceStream("VixenModules.Effect.Shapes.SVGShapes." + svgImage + ".xml");
			XmlDocument xdoc = new XmlDocument();
			StreamReader reader = new StreamReader(s);
			xdoc.LoadXml(reader.ReadToEnd());
			reader.Close();
			return SvgDocument.Open(xdoc);
		}

		private void RemoveShapes(int frame, int bufferHt, int bufferWi)
		{
			if (ShapeMode == ShapeMode.RemoveShapesMarkCollection && _removeShapes.Count == 0)
			{
				// Remnoves oldest shape if mark position is the same as the frame.
				if (_shapeFrame.Any(shapeFrame => frame == shapeFrame && _shapes.Count > 0)) _shapes.RemoveAt(0);
			}
			else
			{
				if (_shapesCount >= _shapes.Count && _removeShapes.Count <= 0) return;
				foreach (var shape in _removeShapes)
				{
					_shapes.Remove(shape);
					if (_shapes.Count < _shapesCount) CreateShapes(bufferHt, bufferWi);
				}
				_removeShapes.Clear();
			}
		}

		private class ShapesClass
		{
			internal double LocationX;
			internal double LocationY;
			internal double VelocityX;
			internal double VelocityY;
			internal int FirstFillColorIndex;
			internal int SecondFillColorIndex;
			internal int StrokeColorIndex;
			internal double Size;
			internal float Scale;
			internal String Shape;
			internal int RotateAngle;
			internal int RotateCW;
			internal SvgDocument SvgImage;
			internal float LocationRatio;
			internal float LocationRatio1;
			internal float Fade;
			internal string FadeType;
			internal float FadeStep;
			internal string SizeMode;
		}
		
		private int CalculateShapeCount(double intervalPosFactor)
		{
			int value = (int)ScaleCurveToValue(ShapeCountCurve.GetValue(intervalPosFactor), 50, 1);
			if (value < 1) value = 1;
			return value;
		}

		private double CalculateCenterAngleSpeed(double intervalPosFactor)
		{
			return ScaleCurveToValue(CenterAngleSpeedCurve.GetValue(intervalPosFactor), 45, 0);
		}

		private double CalculateAngleSpeedVariation(double intervalPosFactor)
		{
			return ScaleCurveToValue(AngleSpeedVariationCurve.GetValue(intervalPosFactor), 45, 0);
		}

		private double CalculateCenterSizeSpeed(double intervalPosFactor)
		{
			return ScaleCurveToValue(CenterSizeSpeedCurve.GetValue(intervalPosFactor), 16, 10);
		}

		private double CalculateSizeSpeedVariation(double intervalPosFactor)
		{
			return ScaleCurveToValue(SizeSpeedVariationCurve.GetValue(intervalPosFactor), 4, 0);
		}

		private double CalculateCenterSpeed(double intervalPosFactor)
		{
			return ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPosFactor), (double)_maxBuffer / 10, 0) * FrameTime / 50d;
		}

		private double CalculateSpeedVariation(double intervalPosFactor)
		{
			return ScaleCurveToValue(SpeedVariationCurve.GetValue(intervalPosFactor), (double)_maxBuffer / 10, 0) * FrameTime / 50d;
		}

		private int CalculateSize(double intervalPosFactor, float locationRatio1)
		{
			int value = (int)ScaleCurveToValue(SizeCurve.GetValue(intervalPosFactor), (int)(_minBuffer * 2 / locationRatio1), 1);
			if (value < 1) value = 1;
			return value;
		}

		private int CalculateSizeVariation(double intervalPosFactor, float locationRatio1)
		{
			int value = (int)ScaleCurveToValue(SizeVariationCurve.GetValue(intervalPosFactor), (int)(_minBuffer * 2 / locationRatio1), 1);
			if (value < 1) value = 1;
			return value;
		}

		private int CalculateCenterSize(double intervalPosFactor, float locationRatio1)
		{
			int value = (int)ScaleCurveToValue(CenterSizeCurve.GetValue(intervalPosFactor), (int)(_minBuffer * 2 / locationRatio1), 4);
			if (value < 1) value = 1;
			return value;
		}

		private int CalculateXOffset(double intervalPos, int width)
		{
			return (int)ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), width, -width);
		}

		private int CalculateYOffset(double intervalPos, int height)
		{
			return (int)ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), -height, height);
		}

		private int CalculateAngle(double intervalPos)
		{
			return (int)ScaleCurveToValue(AngleCurve.GetValue(intervalPos), 360, 0);
		}

		private int CalculateShapeOutLine(double intervalPos)
		{
			return (int)ScaleCurveToValue(ShapeOutLineCurve.GetValue(intervalPos), 100, 0);
		}

		private int CalculateShapeOutLineSpace(double intervalPos)
		{
			return (int)ScaleCurveToValue(ShapeOutLineSpaceCurve.GetValue(intervalPos), 100, 0);
		}

		private void SetupMarks()
		{
			IMarkCollection mc = MarkCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			_marks = mc?.MarksInclusiveOfTime(StartTime, StartTime + TimeSpan);
		}

		/// <inheritdoc />
		protected override void MarkCollectionsChanged()
		{
			if (ShapeMode != ShapeMode.None)
			{
				var markCollection = MarkCollections.FirstOrDefault(x => x.Name.Equals(MarkCollectionId));
				InitializeMarkCollectionListeners(markCollection);
			}
		}

		/// <inheritdoc />
		protected override void MarkCollectionsRemoved(IList<IMarkCollection> addedCollections)
		{
			var mc = addedCollections.FirstOrDefault(x => x.Id == _data.MarkCollectionId);
			if (mc != null)
			{
				//Our collection is gone!!!!
				RemoveMarkCollectionListeners(mc);
				MarkCollectionId = String.Empty;
			}
		}

		#endregion
	}
}
