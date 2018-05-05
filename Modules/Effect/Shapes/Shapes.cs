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

namespace VixenModules.Effect.Shapes
{
	public class Shapes : PixelEffectBase
	{
		private ShapesData _data;
		private readonly Random _random = new Random();
		private readonly List<ShapesClass> _shapes = new List<ShapesClass>();
		private readonly List<ShapesClass> _removeShapes = new List<ShapesClass>();
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

		#region Shape properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ShapeType")]
		[ProviderDescription(@"ShapeType")]
		[PropertyOrder(1)]
		public ShapeList ShapeList
		{
			get { return _data.ShapeList; }
			set
			{
				_data.ShapeList = value;
				if (ShapeList != ShapeList.BorderShapes) StrokeFill = true;
				UpdateShapeTypeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"GeometricShapes")]
		[ProviderDescription(@"GeometricShapes")]
		[PropertyOrder(2)]
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
		[PropertyOrder(3)]
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
		[PropertyOrder(4)]
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
		[ProviderDisplayName(@"BorderShapes")]
		[ProviderDescription(@"BorderShapes")]
		[PropertyOrder(5)]
		public BorderShapesList BorderShapesList
		{
			get { return _data.BorderShapesList; }
			set
			{
				_data.BorderShapesList = value;
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
		[PropertyOrder(6)]
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
		[ProviderDisplayName(@"Mode")]
		[ProviderDescription(@"Mode")]
		[PropertyOrder(7)]
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
		[ProviderDisplayName(@"VerticalOffset")]
		[ProviderDescription(@"VerticalOffset")]
		[PropertyOrder(8)]
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
		[PropertyOrder(9)]
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
		[PropertyOrder(10)]
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
		[ProviderDisplayName(@"MaxShapeSize")]
		[ProviderDescription(@"MaxShapeSize")]
		[PropertyOrder(11)]
		public Curve MaxSizeCurve
		{
			get { return _data.MaxSizeCurve; }
			set
			{
				_data.MaxSizeCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ShapeCount")]
		[ProviderDescription(@"ShapeCount")]
		[PropertyOrder(12)]
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
		[ProviderDisplayName(@"RandomShapeSize")]
		[ProviderDescription(@"RandomShapeSize")]
		[PropertyOrder(13)]
		public bool RandomShapeSize
		{
			get { return _data.RandomShapeSize; }
			set
			{
				_data.RandomShapeSize = value;
				UpdateShapeSizeAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ScaleToGrid")]
		[ProviderDescription(@"ScaleToGrid")]
		[PropertyOrder(14)]
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
		[PropertyOrder(15)]
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
		[ProviderDisplayName(@"ColorFill")]
		[ProviderDescription(@"ColorFill")]
		[PropertyOrder(16)]
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

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void UpdateAllAttributes()
		{
			UpdateShapeSizeAttribute(false);
			UpdatePositionAttribute(false);
			UpdateShapeTypeAttribute(false);
			UpdateStringOrientationAttributes();
			TypeDescriptor.Refresh(this);
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

		private void UpdateShapeSizeAttribute (bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{"SizeCurve", !RandomShapeSize},

				{"MaxSizeCurve", RandomShapeSize}
			};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}
		
		private void UpdateShapeTypeAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(34)
			{
				{"GeometricShapesList", ShapeList == ShapeList.GeometricShapes},

				{"ChristmasShapesList", ShapeList == ShapeList.ChristmasShapes},

				{"HalloweenShapesList", ShapeList == ShapeList.HalloweenShapes},

				{"BorderShapesList", ShapeList == ShapeList.BorderShapes},

				{"FileName", ShapeList == ShapeList.File},

				{"PolygonSides", ShapeList == ShapeList.GeometricShapes && GeometricShapesList == GeometricShapesList.Polygon},

				{"SkipPoints", ShapeList == ShapeList.GeometricShapes && GeometricShapesList == GeometricShapesList.Star},

				{"StarInsideSize", ShapeList == ShapeList.GeometricShapes && GeometricShapesList == GeometricShapesList.NonIntersectingStar},

				{"StarPoints", ShapeList == ShapeList.GeometricShapes && (GeometricShapesList == GeometricShapesList.Star || GeometricShapesList == GeometricShapesList.NonIntersectingStar)},

				{"ShapeSizeRatio", !ScaleToGrid && ShapeList == ShapeList.GeometricShapes && (GeometricShapesList == GeometricShapesList.Rectangle || GeometricShapesList == GeometricShapesList.Ellipse)},

				{"CrossSizeRatio", !ScaleToGrid && ShapeList == ShapeList.GeometricShapes && GeometricShapesList == GeometricShapesList.Cross},

				{"FirstFillColors", ShapeList != ShapeList.File && Fill && ShapeList != ShapeList.BorderShapes},

				{"SecondFillColors", ShapeList != ShapeList.File && Fill && ShapeList != ShapeList.BorderShapes && ShapeList != ShapeList.GeometricShapes && (ShapeList == ShapeList.ChristmasShapes && ChristmasShapesList != ChristmasShapesList.SnowFlake&& ChristmasShapesList != ChristmasShapesList.SnowFlake2) || (ShapeList == ShapeList.HalloweenShapes && HalloweenShapesList != HalloweenShapesList.Web)},

				{"RoundedCorner", ShapeList == ShapeList.GeometricShapes},

				{"OutlineColors", StrokeFill},

				{"Fill", ShapeList != ShapeList.File && ShapeList != ShapeList.BorderShapes},

				{"StrokeFill", ShapeList == ShapeList.BorderShapes || ShapeList == ShapeList.File},

				{"SizeCurve", !ScaleToGrid && !RandomShapeSize},

				{"MaxSizeCurve", !ScaleToGrid && RandomShapeSize},

				{"CenterSpeedCurve", !ScaleToGrid && ShapeType != ShapeType.None},

				{"SpeedVariationCurve", !ScaleToGrid && ShapeType != ShapeType.None},

				{"YOffsetCurve", !ScaleToGrid && ShapeType == ShapeType.None},

				{"XOffsetCurve", !ScaleToGrid && ShapeType == ShapeType.None},

				{"ShapeCountCurve", !ScaleToGrid},

				{"RandomShapeSize", !ScaleToGrid},

				{"CenterSizeSpeedCurve", !ScaleToGrid},

				{"SizeSpeedVariationCurve", !ScaleToGrid},

				{"CenterAngleSpeedCurve", !ScaleToGrid && RandomAngle},

				{"AngleSpeedVariationCurve", !ScaleToGrid && RandomAngle},

				{"AngleCurve", !ScaleToGrid && !RandomAngle},

				{"ShapeType", !ScaleToGrid},

				{"ShapeOutLineCurve", StrokeFill},

				{"ShapeOutLineSpaceCurve", StrokeFill},

				{"StrokeWidth", StrokeFill},

				{"RandomAngle", !ScaleToGrid}

			};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

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
			_shapes.Clear();
			_shapesCount = 0;

			if (FileName != null && ShapeList == ShapeList.File)
			{
				var filePath = Path.Combine(ShapesDescriptor.ModulePath, FileName);
				if (File.Exists(filePath))
				{
					_fileName = filePath;
				}
				else
				{
					//Logging.Error("File is missing or invalid path. {0}", filePath);
					FileName = "";
				}

			}
		}

		protected override void CleanUpRender()
		{
			_shapes.Clear();
			_removeShapes.Clear();
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			using (var bitmap = new Bitmap(BufferWi, BufferHt))
			{
				InitialRender(frame, bitmap);
				double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				// copy to frameBuffer
				for(int x = 0; x < BufferWi; x++)
				{
					for(int y = 0; y < BufferHt; y++)
					{
						CalculatePixel(x, y, bitmap, level, frameBuffer);
					}
				}
			}

		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;
				double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
				using (var bitmap = new Bitmap(BufferWi, BufferHt))
				{
					InitialRender(frame, bitmap);
					foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
					{
						foreach (var elementLocation in elementLocations)
						{
							CalculatePixel(elementLocation.X, elementLocation.Y, bitmap, level, frameBuffer);
						}
					}
				}

			}
		}

		private void InitialRender(int frame, Bitmap bitmap)
		{
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			_intervalPosFactor = intervalPos * 100;
			//_radius = CalculateSize(_intervalPosFactor);

			_centerAngleSpeed = CalculateCenterAngleSpeed(_intervalPosFactor);
			_angleSpeedVariation = CalculateAngleSpeedVariation(_intervalPosFactor);
			_centerSizeSpeed = CalculateCenterSizeSpeed(_intervalPosFactor);
			_sizeSpeedVariation = CalculateSizeSpeedVariation(_intervalPosFactor);
			_centerSpeed = CalculateCenterSpeed(_intervalPosFactor);
			_speedVariation = CalculateSpeedVariation(_intervalPosFactor);
			_angle = CalculateAngle(_intervalPosFactor);

			double minAngleSpeed = _centerAngleSpeed - (_angleSpeedVariation / 2);
			double maxAngleSpeed = _centerAngleSpeed + (_angleSpeedVariation / 2);
			double minSizeSpeed = _centerSizeSpeed - (_sizeSpeedVariation / 2);
			double maxSizeSpeed = _centerSizeSpeed + (_sizeSpeedVariation / 2);
			_minSpeed = _centerSpeed - (_speedVariation / 2);
			_maxSpeed = _centerSpeed + (_speedVariation / 2);

			// create new shapes and maintain maximum number as per users selection.
			int adjustedPixelCount;
			if (ScaleToGrid)
			{
				adjustedPixelCount = _shapesCount = 1;
			}
			else
			{
				_shapesCount = CalculateShapeCount(_intervalPosFactor);
				adjustedPixelCount = frame >= _shapesCount ? _shapesCount : 2;
			}
				
			for (int i = 0; i < adjustedPixelCount; i++)
			{
				//Create new Shapes and add shapes due to increase in shape count curve.
				if (_shapes.Count < _shapesCount) CreateShapes();
				else
					break;
			}

			//Update Shape location, radius and speed.
			UpdateShapes(minAngleSpeed, maxAngleSpeed, minSizeSpeed, maxSizeSpeed);

			//Remove Excess Shapes due to ShapeCount Curve.
			RemoveShapes();
			
			foreach (var shape in _shapes)
			{
				_scaleShapeWidth = (float)(((_maxBuffer * 2) / shape.SvgImage.ViewBox.Width) * (shape.Size / (_maxBuffer * 2)));
				_scaleShapeHeight = (float)(((_minBuffer * 2) / shape.SvgImage.ViewBox.Height) * (shape.Size / (_minBuffer * 2)));
				if (ShapeList != ShapeList.File)
				{
					bool borderFill = false;
					foreach (var child in shape.SvgImage.Children)
					{
						//Adjusts Shape properties based on effect settings
						if (Fill && ShapeList != ShapeList.BorderShapes)
						{
							foreach (var descendant in shape.SvgImage.Descendants())
							{
								if (descendant.ID != null)
								{
									if (descendant.ID.Contains("firstFill"))
									{
										descendant.Fill = new SvgColourServer(FirstFillColors[shape.FirstFillColorIndex]
											.GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100));
									}
									if (descendant.ID.Contains("secondFill"))
									{
										descendant.Fill = new SvgColourServer(SecondFillColors[shape.SecondFillColorIndex]
											.GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100));
									}
									//if (shape.SvgImage.ID == "borderFillColor" || borderFill)
									//{
									//	descendant.Fill = new SvgColourServer(SecondFillColors[shape.SecondFillColorIndex]
									//		.GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100));
									//	borderFill = true;
									//}
								}
							}
						}
						else
						{
							shape.SvgImage.Children[0].FillOpacity = 0;
						}

						if (StrokeFill)
						{
							child.Stroke = new SvgColourServer(OutlineColors[shape.StrokeColorIndex]
								.GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100));
							child.StrokeWidth = TargetPositioning == TargetPositioningType.Locations
								? (SvgUnit) (new SvgUnit(StrokeWidth / _scaleShapeWidth) * _minBuffer / StringCount /
								             shape.LocationRatio1)
								: (SvgUnit) (new SvgUnit(StrokeWidth / _scaleShapeWidth));
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
					if (StrokeFill)
					{
						shape.SvgImage.Stroke = new SvgColourServer(OutlineColors[shape.StrokeColorIndex]
							.GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100));
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
					shape.SvgImage.Transforms[0] = new SvgRotate(shape.RotateAngle, shape.SvgImage.ViewBox.Width * shape.LocationRatio1 * _scaleShapeWidth / 2, shape.SvgImage.ViewBox.Height * shape.LocationRatio1 * _scaleShapeHeight / 2);
					shape.SvgImage.Transforms[1] = new SvgScale(_scaleShapeWidth, _scaleShapeHeight);
				}

				shape.SvgImage.ShapeRendering = SvgShapeRendering.Auto;
				double locationX;
				double locationY;
				if (_shapes.Count == 1 && ShapeType == ShapeType.None && !ScaleToGrid)
				{
					locationX = (double)BufferWi / 2;
					locationY = (double)BufferHt / 2; 
				}
				else
				{
					locationX = shape.LocationX;
					locationY = shape.LocationY;
				}

				using (Graphics g = Graphics.FromImage(bitmap))
				{
					//Adjust position based on x and y offset.
					int xOffset = 0;
					int yOffset = 0;
					if (ShapeType == ShapeType.None && !ScaleToGrid)
					{
						xOffset = CalculateXOffset(_intervalPosFactor, bitmap.Width);
						yOffset = CalculateYOffset(_intervalPosFactor, bitmap.Height);
					}

					//Draw svg onto bitmap
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

		private void CalculatePixel(int x, int y, Bitmap bitmap, double level, IPixelFrameBuffer frameBuffer)
		{
			int yCoord = y;
			int xCoord = x;
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				//Flip me over and offset my coordinates so I can act like the string version
				y = Math.Abs((BufferHtOffset - y) + (BufferHt - 1 + BufferHtOffset));
				y = y - BufferHtOffset;
				x = x - BufferWiOffset;
			}
			Color color = bitmap.GetPixel(x, BufferHt - y - 1);

			if (color.R != 0 || color.G != 0 || color.B != 0)
			{
				var hsv = HSV.FromRGB(color);
				hsv.V = hsv.V * level;
				frameBuffer.SetPixel(xCoord, yCoord, hsv);
			}
			else if (TargetPositioning == TargetPositioningType.Locations)
			{
				frameBuffer.SetPixel(xCoord, yCoord, Color.Transparent);
			}
		}

		private void CreateShapes()
		{
			while (_shapesCount > _shapes.Count)
			{
				ShapesClass m = new ShapesClass();
				
				//Sets starting location of svg image
				m.LocationX = _random.Next(0, BufferWi - 1);
				m.LocationY = _random.Next(0, BufferHt - 1);
				
				//Sets initial speed of svg image
				double speed = _random.NextDouble() * (_maxSpeed - _minSpeed) + _minSpeed;
				double vx = _random.NextDouble() * speed;
				double vy = _random.NextDouble() * speed;
				if (_random.Next(0, 2) == 0) vx = -vx;
				if (_random.Next(0, 2) == 0) vy = -vy;
				if (ShapeType != ShapeType.None)
				{
					m.VelocityX = vx;
					m.VelocityY = vy;
				}

				//Starting angle based on position of angle curve
				_angle = CalculateAngle(_intervalPosFactor);
				m.Scale = 1;

				if (ShapeList == ShapeList.GeometricShapes)
				{
					//Creates svg Viewbox used geometric Shapes as they are designed around this size.
					m.SvgImage = new SvgDocument
					{
						ViewBox = new SvgViewBox(0, 0, _svgViewBoxSize, _svgViewBoxSize)
					};
				}
				
				//Get the shape from users selected source.
				Array enumValues;
				switch (ShapeList)
				{
					case ShapeList.GeometricShapes:
						enumValues = Enum.GetValues(typeof(GeometricShapesList));
						m.Shape = GeometricShapesList == GeometricShapesList.Random
							? ((GeometricShapesList)enumValues.GetValue(_random.Next(1, enumValues.Length))).ToString()
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
								m.SvgImage.Children.Add(GetGeometricShape.CreateSvgRectangle(radius, (float)ShapeSizeRatio / 100));
								break;
							case "Ellipse":
								m.SvgImage.Children.Add(GetGeometricShape.CreateSvgEllipse(radius, (float)ShapeSizeRatio / 100));
								break;
							case "Cross":
								m.SvgImage.Children.Add(GetGeometricShape.CreateSvgCross((float)CrossSizeRatio / 100));
								break;
							case "Star":
								m.SvgImage.Children.Add(GetGeometricShape.CreateSvgMultiStar(radius, StarPoints, SkipPoints));
								break;
							case "ConcaveStar":
								m.SvgImage = getXMLShape("Geometric." + m.Shape);
								break;
							case "NonIntersectingStar":
								m.SvgImage.Children.Add(GetGeometricShape.CreateSvgNonIntersectingStar(radius, StarPoints, StarInsideSize, false));
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
							? ((ChristmasShapesList)enumValues.GetValue(_random.Next(1, enumValues.Length))).ToString()
							: ChristmasShapesList.ToString();

						m.SvgImage = getXMLShape("Christmas." + m.Shape);
						break;

					case ShapeList.HalloweenShapes:
						enumValues = Enum.GetValues(typeof(HalloweenShapesList));
						m.Shape = HalloweenShapesList == HalloweenShapesList.Random
							? ((HalloweenShapesList)enumValues.GetValue(_random.Next(1, enumValues.Length))).ToString()
							: HalloweenShapesList.ToString();

						m.SvgImage = getXMLShape("Halloween." + m.Shape);
						break;

					case ShapeList.BorderShapes:
						enumValues = Enum.GetValues(typeof(BorderShapesList));
						m.Shape = BorderShapesList == BorderShapesList.Random
							? ((BorderShapesList)enumValues.GetValue(_random.Next(1, enumValues.Length))).ToString()
							: BorderShapesList.ToString();

						m.SvgImage = getXMLShape("Borders." + m.Shape);
						break;

					case ShapeList.File:
						m.SvgImage = SvgDocument.Open(_fileName);
						break;
				}

				if ((int)m.SvgImage.ViewBox.Width == 0)
				{
					m.SvgImage.ViewBox = new SvgViewBox(0, 0, m.SvgImage.Width, m.SvgImage.Height);
				}

				if (!ScaleToGrid)
				{
					m.SvgImage.ViewBox = new SvgViewBox(-((m.SvgImage.ViewBox.Width * 1.42f - m.SvgImage.ViewBox.Width) / 2),
						-((m.SvgImage.ViewBox.Height * 1.42f - m.SvgImage.ViewBox.Height) / 2), m.SvgImage.ViewBox.Width * 1.42f,
						m.SvgImage.ViewBox.Height * 1.42f);
					m.Scale = _svgViewBoxSize / m.SvgImage.ViewBox.Height;
					m.SvgImage.Transforms.Add(new SvgRotate(0, (int)(m.SvgImage.Width / 2), (int)(m.SvgImage.Height / 2)));
					m.LocationRatio = StringCount / m.SvgImage.ViewBox.Height * 2 * ((float)_minBuffer / StringCount);
					m.LocationRatio1 = m.LocationRatio * m.LocationRatio;
					m.SvgImage.Height = new SvgUnit(m.LocationRatio).ToPercentage();
					m.SvgImage.Width = new SvgUnit(m.LocationRatio).ToPercentage();
					m.SvgImage.Transforms.Add(new SvgScale(m.Scale));
				}
				else
				{
					// StringOrientation == StringOrientation.Vertical && TargetPositioning != TargetPositioningType.Locations && MaxPixelsPerString > StringCount)
					// 
					//if (StringOrientation != StringOrientation.Horizontal)
					//{
					//	var temp = _maxBuffer;
					//	_maxBuffer = _minBuffer;
					//	_minBuffer = temp;
					//}
					m.SvgImage.Height = new SvgUnit(BufferHt);
					m.SvgImage.Width = new SvgUnit(BufferWi);
					m.SvgImage.AspectRatio = new SvgAspectRatio(SvgPreserveAspectRatio.none);
					m.LocationRatio = m.LocationRatio1 = 1;
					m.LocationX = m.LocationY = 0;
				}
				
				//Adds rounded corners to Geometric Shapes only, no point doing it to the others.
				if (RoundedCorner && ShapeList == ShapeList.GeometricShapes)
				{
					foreach (var child in m.SvgImage.Children)
					{
						child.StrokeLineJoin = SvgStrokeLineJoin.Round;
					}
				}

				//Sets starting size of svg image
				if (!ScaleToGrid)
				{
					m.Size = RandomShapeSize
						? _random.Next(5, 60)
						: CalculateSize(_intervalPosFactor,
							m.LocationRatio1);
				}

				//_random.Next(3, CalculateSize(_intervalPosFactor));
				switch (ShapeType)
				{
					case ShapeType.Bounce:

						var locationOffset = (m.SvgImage.ViewBox.Width / 1.42 * m.LocationRatio1 * _scaleShapeWidth / 2);
						bool notCreated;
						int i = 0;
						do
						{
							notCreated = false;
							m.LocationX = _random.Next(0, BufferWi - 1);
							m.LocationY = _random.Next(0, BufferHt - 1);
							if (m.LocationX - locationOffset < 0 || m.LocationY - locationOffset < 0 || m.LocationX + locationOffset >= BufferWi || m.LocationY + locationOffset >= BufferHt) notCreated = true;
							i++;
						} while (notCreated && i < 10000);

						break;
				}

				//Sets Random Colors
				m.FirstFillColorIndex = _random.Next(0, FirstFillColors.Count);
				m.SecondFillColorIndex = _random.Next(0, SecondFillColors.Count);
				m.StrokeColorIndex = _random.Next(0, OutlineColors.Count);

				//Sets random starting angle and rotation direction
				m.RotateAngle = _random.Next(0, 360);
				m.RotateCW = _random.Next(0, 2);

				//Adds new Shape to _shapes
				_shapes.Add(m);
				
				//int num = _random.Next(0, 26); // Zero to 25
			}
		}

		private SvgDocument getXMLShape(string svgImage)
		{
			Stream s = Assembly.GetExecutingAssembly()
				.GetManifestResourceStream("VixenModules.Effect.Shapes.SVGShapes." + svgImage + ".xml");
			XmlDocument xdoc = new XmlDocument();
			StreamReader reader = new StreamReader(s);
			xdoc.LoadXml(reader.ReadToEnd());
			reader.Close();

			//Used to get points
			//var tempImage = SvgDocument.Open(xdoc);
			//var svgPoints = tempImage.Path.PathPoints.ToArray();
			////m.svgImage.Children.Add(GetGeometricShape.CreateSvgImage(svgPoints));
			//string test = null;
			//foreach (var point in svgPoints)
			//{
			//	test = test + point.X + ", " + point.Y + ", ";
			//}
			return SvgDocument.Open(xdoc);
		}
		
		private void UpdateShapes(double minAngleSpeed, double maxAngleSpeed, double minSizeSpeed, double maxSizeSpeed)
		{
			foreach (var shape in _shapes)
			{
				//if (_shapesCount < _shapes.Count - _removeShapes.Count)
				//{
				//	//Removes shapes if shape count curve position is below the current number of shapes. Will only remove ones that hit the edge of the grid.
				//	if (shape.LocationX + _radius >= BufferWi - 1 || shape.LocationY + _radius >= BufferHt - 1 ||
				//	    shape.LocationX - _radius <= 1 || shape.LocationY + _radius <= 1)
				//	{
				//		_removeShapes.Add(shape);
				//	}
				//}

				if (ShapeType != ShapeType.None)
				{
					//Adjust shape speeds when user adjust Speed curve
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

				double angleSpeed = _random.NextDouble() * (maxAngleSpeed - minAngleSpeed) + minAngleSpeed;
				double sizeSpeed = (_random.NextDouble() * (maxSizeSpeed - minSizeSpeed) + minSizeSpeed) / 10;

				if (RandomShapeSize && !ScaleToGrid)
				{
					if (sizeSpeed > 1) shape.Size *= sizeSpeed; //ratio;

					if (shape.Size > CalculateMaxSize(_intervalPosFactor, shape.LocationRatio1)) //(int)(Math.Min(BufferWi, BufferHt) * 0.75))
					{
						_removeShapes.Add(shape);
					}
				}
				else
				{
					shape.Size = CalculateSize(_intervalPosFactor, shape.LocationRatio1);
				}

				if (RandomAngle)
				{
					//shape.RotateAngle = CalculateAngle(_intervalPosFactor);
					if (shape.RotateCW == 1)
					{
						if (shape.RotateAngle > 358)
						{
							shape.RotateAngle = 0;
						}
						else
						{
							shape.RotateAngle += (int) angleSpeed;
						}
					}
					else
					{
						if (shape.RotateAngle < 2)
						{
							shape.RotateAngle = 360;
						}
						else
						{
							shape.RotateAngle -= (int) angleSpeed;
						}
					}
				}
				else
				{
					shape.RotateAngle = _angle;
				}

				// Move the shape.
				if (ShapeType != ShapeType.None)
				{
					shape.LocationX = shape.LocationX + shape.VelocityX;
					shape.LocationY = shape.LocationY + shape.VelocityY;
					var locationOffset = (shape.SvgImage.ViewBox.Width / 1.42 * shape.LocationRatio1 * _scaleShapeWidth / 2);

					switch (ShapeType)
					{
						case ShapeType.None:
							if (shape.LocationX + shape.Size < 0 || shape.LocationY + shape.Size < 0 ||
							    shape.LocationX - shape.Size > BufferWi || shape.LocationY - shape.Size > BufferHt)
							{
								_removeShapes.Add(shape);
							}
							break;

						case ShapeType.Bounce:
							if (shape.LocationX - locationOffset < 0)
							{
								shape.VelocityX = -shape.VelocityX;
							}
							else if (shape.LocationX + locationOffset >= BufferWi)
							{
								shape.VelocityX = -shape.VelocityX;
							}
							if (shape.LocationY - locationOffset < 0)
							{
								shape.VelocityY = -shape.VelocityY;
							}
							else if (shape.LocationY + locationOffset >= BufferHt)
							{
								shape.VelocityY = -shape.VelocityY;
							}
							break;

						case ShapeType.Wrap:

							if (shape.LocationX + locationOffset < 0)
							{
								shape.LocationX = BufferWi + locationOffset;
							}
							if (shape.LocationY + locationOffset < 0)
							{
								shape.LocationY = BufferHt + locationOffset;
							}
							if (shape.LocationX - locationOffset > BufferWi)
							{
								shape.LocationX = 0 - locationOffset;
							}
							if (shape.LocationY - locationOffset > BufferHt)
							{
								shape.LocationY = 0 - locationOffset;
							}
							break;
					}
				}
			}
		}

		private void RemoveShapes()
		{
			if (_shapesCount < _shapes.Count || _removeShapes.Count > 0)
			{
				foreach (var shape in _removeShapes)
				{
					_shapes.Remove(shape);
					CreateShapes();
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
			return ScaleCurveToValue(CenterSizeSpeedCurve.GetValue(intervalPosFactor), 12, 10);
		}

		private double CalculateSizeSpeedVariation(double intervalPosFactor)
		{
			return ScaleCurveToValue(SizeSpeedVariationCurve.GetValue(intervalPosFactor), 4, 0);
		}

		private double CalculateCenterSpeed(double intervalPosFactor)
		{
			return ScaleCurveToValue(CenterSpeedCurve.GetValue(intervalPosFactor), (double)_maxBuffer / 10, 0);
		}

		private double CalculateSpeedVariation(double intervalPosFactor)
		{
			return ScaleCurveToValue(SpeedVariationCurve.GetValue(intervalPosFactor), (double)_maxBuffer / 10, 0);
		}

		private int CalculateSize(double intervalPosFactor, float locationRatio1)
		{
			int value = (int)ScaleCurveToValue(SizeCurve.GetValue(intervalPosFactor), (int)(_minBuffer * 2 / locationRatio1), 4);
			if (value < 1) value = 1;
			return value;
		}
		
		private int CalculateMaxSize(double intervalPosFactor, float locationRatio1)
		{
			int value = (int)ScaleCurveToValue(MaxSizeCurve.GetValue(intervalPosFactor), (int)(_minBuffer * 2 / locationRatio1), 4);
			if (value < 1) value = 1;
			return value;
		}

		private int CalculateXOffset(double intervalPos, int width)
		{
			return (int)ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), width, -width);
		}

		private int CalculateYOffset(double intervalPos, int height)
		{
			return (int)ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), height, -height);
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

	}
}
