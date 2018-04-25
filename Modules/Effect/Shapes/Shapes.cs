using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Shapes
{
	public class Shapes : PixelEffectBase
	{
		private ShapesData _data;
		private static Color _emptyColor = Color.FromArgb(0, 0, 0, 0);
		private Font _font;
		private Font _newfont;
		private float _newFontSize;
		private int _maxTextSize;

		private SizeF _textsize = new SizeF(0, 0);
		private readonly Random _random = new Random();
		private readonly List<ShapesClass> _shapes = new List<ShapesClass>();
		private readonly List<ShapesClass> _removeShapes = new List<ShapesClass>();
		private int _shapesCount;
		private int _radius;
		private double _intervalPosFactor;
		private double _centerAngleSpeed;
		private double _angleSpeedVariation;
		private double _centerSizeSpeed;
		private double _sizeSpeedVariation;
		private int _minBuffer;
		private int _maxBuffer;
		private int _currentShapeColor;
		private double _centerSpeed;
		private double _speedVariation;
		private double _minSpeed;
		private double _maxSpeed;

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
				if (Colors.Any(x => !x.CheckLibraryReference()))
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
		[ProviderDisplayName(@"ShapeTypes")]
		[ProviderDescription(@"ShapeTypes")]
		[PropertyOrder(1)]
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
		[PropertyOrder(2)]
		public GeometricShapes GeometricShapes
		{
			get { return _data.GeometricShapes; }
			set
			{
				_data.GeometricShapes = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ChristmasShapes")]
		[ProviderDescription(@"ChristmasShapes")]
		[PropertyOrder(3)]
		public ChristmasShapes ChristmasShapes
		{
			get { return _data.ChristmasShapes; }
			set
			{
				_data.ChristmasShapes = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"FontShapes")]
		[ProviderDescription(@"FontShapes")]
		[PropertyOrder(3)]
		public FontShapes FontShapes
		{
			get { return _data.FontShapes; }
			set
			{
				_data.FontShapes = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Font")]
		[ProviderDescription(@"Font")]
		[PropertyOrder(4)]
		public Font Font
		{
			get { return _data.Font.FontValue; }
			set
			{
				_data.Font.FontValue = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ShapeType")]
		[ProviderDescription(@"ShapeType")]
		[PropertyOrder(5)]
		public ShapeType ShapeType
		{
			get { return _data.ShapeType; }
			set
			{
				_data.ShapeType = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"ShapeSize")]
		[ProviderDescription(@"ShapeSize")]
		[PropertyOrder(6)]
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
		[PropertyOrder(7)]
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
		[PropertyOrder(8)]
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
		[PropertyOrder(9)]
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

		#endregion


		[Value]
		[ProviderCategory(@"Position", 2)]
		[ProviderDisplayName(@"ManualPosition")]
		[ProviderDescription(@"ManualPosition")]
		[PropertyOrder(0)]
		public bool ManualPosition
		{
			get { return _data.ManualPosition; }
			set
			{
				_data.ManualPosition = value;
				UpdatePositionAttribute();
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Position", 2)]
		[ProviderDisplayName(@"Vertical Offset")]
		[ProviderDescription(@"Vertical Offset")]
		[PropertyOrder(1)]
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
		[ProviderCategory(@"Position", 2)]
		[ProviderDisplayName(@"Horizontal Offset")]
		[ProviderDescription(@"Horizontal Offset")]
		[PropertyOrder(2)]
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
		[ProviderCategory(@"Position", 2)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyOrder(3)]
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
		[ProviderCategory(@"Position", 2)]
		[ProviderDisplayName(@"SpeedVariation")]
		[ProviderDescription(@"SpeedVariation")]
		[PropertyOrder(4)]
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


		#region Speed Settings properties

		[Value]
		[ProviderCategory(@"SpeedSettings", 3)]
		[ProviderDisplayName(@"SizeSpeed")]
		[ProviderDescription(@"SizeSpeed")]
		[PropertyOrder(1)]
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
		[ProviderCategory(@"SpeedSettings", 3)]
		[ProviderDisplayName(@"SizeSpeedVariation")]
		[ProviderDescription(@"SizeSpeedVariation")]
		[PropertyOrder(2)]
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
		[ProviderCategory(@"SpeedSettings", 3)]
		[ProviderDisplayName(@"AngleSpeed")]
		[ProviderDescription(@"AngleSpeed")]
		[PropertyOrder(3)]
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
		[ProviderCategory(@"SpeedSettings", 3)]
		[ProviderDisplayName(@"AngleSpeedVariation")]
		[ProviderDescription(@"AngleSpeedVariation")]
		[PropertyOrder(4)]
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
		[ProviderCategory(@"SpeedSettings", 3)]
		[ProviderDisplayName(@"Angle")]
		[ProviderDescription(@"Angle")]
		[PropertyOrder(5)]
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
		[ProviderCategory(@"Color", 4)]
		[ProviderDisplayName(@"TextColors")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(0)]
		public List<ColorGradient> Colors
		{
			get { return _data.Colors; }
			set
			{
				_data.Colors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 4)]
		[ProviderDisplayName(@"GradientMode")]
		[ProviderDescription(@"GradientMode")]
		[PropertyOrder(1)]
		public GradientMode GradientMode
		{
			get { return _data.GradientMode; }
			set
			{
				_data.GradientMode = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Brightness", 5)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"TextBrightness")]
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
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3)
			{
				{"CenterSpeedCurve", !ManualPosition},

				{"SpeedVariationCurve", !ManualPosition},

				{"YOffsetCurve", ManualPosition},

				{"XOffsetCurve", ManualPosition}
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
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(3)
			{
				{"GeometricShapes", ShapeList == ShapeList.GeometricShapes},

				{"ChristmasShapes", ShapeList == ShapeList.ChristmasShapes},

				{"FontShapes", ShapeList == ShapeList.FontShapes},

				{"Font", ShapeList == ShapeList.FontShapes}

			};
			SetBrowsable(propertyStates);

			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		protected override void SetupRender()
		{
			_minBuffer = Math.Min(BufferHt, BufferWi);
			_maxBuffer = Math.Max(BufferHt, BufferWi);
			//if (TargetPositioning == TargetPositioningType.Locations)
			//{
			//	// Adjust the font size for Location support, default will ensure when swicthing between string and location that the Font will be the same visual size.
			//	// Font is further adjusted using the scale text slider.
			//	double newFontSize = ((StringCount - ((StringCount - Font.Size) / 100) * 100)) * ((double)BufferHt / StringCount);
			//	_font = new Font(Font.FontFamily.Name, (int)newFontSize, Font.Style);
			//	_newFontSize = _font.Size;
			//	return;
			//}
			switch (ShapeList)
			{
				case ShapeList.GeometricShapes:
					_newfont = new SerializableFont(new Font("Untitled2", 10, Font.Style));
					break;
				case ShapeList.ChristmasShapes:
					_newfont = new SerializableFont(new Font("Xmas tree", 10, Font.Style));
					break;
				case ShapeList.FontShapes:
					_newfont = new SerializableFont(new Font(Font.FontFamily.Name, 10, Font.Style));
					break;
			}
			//_font = Font;
			//_newFontSize = Font.Size;
			_shapes.Clear();
			_shapesCount = 0;

		}

		protected override void CleanUpRender()
		{
			_font = null;
			_newfont = null;
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
			var textAngle = CalculateAngle(_intervalPosFactor);
			//_radius = CalculateSize(_intervalPosFactor);
			_shapesCount = CalculateShapeCount(_intervalPosFactor);

			_centerAngleSpeed = CalculateCenterAngleSpeed(_intervalPosFactor);
			_angleSpeedVariation = CalculateAngleSpeedVariation(_intervalPosFactor);
			_centerSizeSpeed = CalculateCenterSizeSpeed(_intervalPosFactor);
			_sizeSpeedVariation = CalculateSizeSpeedVariation(_intervalPosFactor);
			_centerSpeed = CalculateCenterSpeed(_intervalPosFactor);
			_speedVariation = CalculateSpeedVariation(_intervalPosFactor);

			double minAngleSpeed = _centerAngleSpeed - (_angleSpeedVariation / 2);
			double maxAngleSpeed = _centerAngleSpeed + (_angleSpeedVariation / 2);
			double minSizeSpeed = _centerSizeSpeed - (_sizeSpeedVariation / 2);
			double maxSizeSpeed = _centerSizeSpeed + (_sizeSpeedVariation / 2);
			_minSpeed = _centerSpeed - (_speedVariation / 2);
			_maxSpeed = _centerSpeed + (_speedVariation / 2);

			// create new meteors and maintain maximum number as per users selection.
			int adjustedPixelCount = frame >= _shapesCount ? _shapesCount : 2;
			
			for (int i = 0; i < adjustedPixelCount; i++)
			{
				if (_shapes.Count < _shapesCount)
				{
				//Create new Shapes and add shapes due to increase in shape count curve.
				   CreateShapes();
				}
				else
				{
					break;
				}

			}

			// Update Ball location, radius and speed.
			UpdateShapes(minAngleSpeed, maxAngleSpeed, minSizeSpeed, maxSizeSpeed);

			//Remove Excess Shapes due to ShapeCount Curve.
			RemoveShapes();
			
			foreach (var shape in _shapes)
			{
				//text = shape.Shape; //ShapeType.ToString();
				_currentShapeColor = shape.ColorIndex;
				//Adjust Font Size based on the Font scaling factor
				//_newFontSize =
				//_font.SizeInPoints * ((float) shape.Size / 500 * _radius); //(CalculateFontScale(_intervalPosFactor) / 100);

				_newfont = new SerializableFont(new Font(_newfont.Name, (float)shape.Size, Font.Style));

				using (Graphics graphics1 = Graphics.FromImage(bitmap))
				{
					_textsize = graphics1.MeasureString(shape.Shape, _newfont);
				}
				int testSizeWi = (int)(_textsize.Width * 1.5);
				int testSizeHt = (int)(_textsize.Height * 1.5);

				Image img = new Bitmap(testSizeWi, testSizeHt);

				using (Graphics graphics = Graphics.FromImage(img))
				{
					_maxTextSize = Math.Max(testSizeWi, testSizeHt);

					//int offsetTop = (((BufferHt - maxht) / 2) * 2 + yOffset) / 2;
					//Rotate the text based off the angle setting
					//if (Direction == ShapesDirection.Rotate)
					//{
					//move rotation point to center of image
					graphics.TranslateTransform(img.Width / 2, img.Height / 2);
					//rotate
					graphics.SmoothingMode = SmoothingMode.HighSpeed;
					graphics.RotateTransform(shape.RotateAngle); //textAngle);
					//move image back
					graphics.TranslateTransform(-(img.Width / 2), -(img.Height / 2 ));
					//}

					//int offsetLeft = (((BufferWi - _maxTextSize) / 2) * 2 + xOffset) / 2;
					int offsetTop = (int) (((_textsize.Height * 1.5 - _textsize.Height)) / 2);
					//int offsetTop = ((BufferHt - maxht) + (int)shape.LocationY) / 2;
					//int offsetTop = (int) shape.LocationY - (maxht / 2);
					//double intervalPosition = (GetEffectTimeIntervalPosition(frame) * Speed) % 1;
					//Point point;
					Point point = new Point(((testSizeWi - _maxTextSize) / 2), offsetTop);
					//Point point = new Point((int) ((img.Width/2) - (_textsize.Width) / 2), (int)((img.Height / 2) - (_textsize.Height) / 2));
					DrawText(shape.Shape, graphics, point);
					//break;
					//}
				}
				using (Graphics g = Graphics.FromImage(bitmap))
				{
					int xOffset;
					int yOffset;
					if (ManualPosition)
					{
						xOffset = CalculateXOffset(_intervalPosFactor, bitmap.Width);
						yOffset = CalculateYOffset(_intervalPosFactor, bitmap.Height);
					}
					else
					{
						xOffset = 0;
						yOffset = 0;
					}
					
					g.DrawImage(img, new Point((int) (shape.LocationX + xOffset - testSizeWi / 2),  (int) (shape.LocationY + yOffset - testSizeHt / 2)));
				}
			}
		}

		private void CalculatePixel(int x, int y, Bitmap bitmap, double level, IPixelFrameBuffer frameBuffer)
		{
			int yCoord = y;
			int xCoord = x;
			if (TargetPositioning == TargetPositioningType.Locations)
			{
				//Flip me over so and offset my coordinates I can act like the string version
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

		private void DrawText(String text, Graphics g, Point p)
		{
			switch (GradientMode)
			{
				case GradientMode.AcrossElement:
					DrawTextAcrossGradient(text, g, p, LinearGradientMode.Horizontal);
					break;
				case GradientMode.AcrossText:
					DrawTextWithGradient(text, g, p, LinearGradientMode.Horizontal);
					break;
				case GradientMode.VerticalAcrossElement:
					DrawTextAcrossGradient(text, g, p, LinearGradientMode.Vertical);
					break;
				case GradientMode.VerticalAcrossText:
					DrawTextWithGradient(text, g, p, LinearGradientMode.Vertical);
					break;
				case GradientMode.DiagonalAcrossElement:
					DrawTextAcrossGradient(text, g, p, LinearGradientMode.ForwardDiagonal);
					break;
				case GradientMode.DiagonalAcrossText:
					DrawTextWithGradient(text, g, p, LinearGradientMode.ForwardDiagonal);
					break;
				case GradientMode.BackwardDiagonalAcrossElement:
					DrawTextAcrossGradient(text, g, p, LinearGradientMode.BackwardDiagonal);
					break;
				case GradientMode.BackwardDiagonalAcrossText:
					DrawTextWithGradient(text, g, p, LinearGradientMode.BackwardDiagonal);
					break;
			}
		}

		private void DrawTextWithGradient(String text, Graphics g, Point p, LinearGradientMode mode)
		{
			var offsetPoint = new Point(p.X + (_maxTextSize - (int)_textsize.Width) / 2, p.Y);
			ColorGradient cg = Colors[_currentShapeColor];
			var brush = new LinearGradientBrush(new Rectangle(offsetPoint.X, p.Y, _maxTextSize, (int)_textsize.Height), Color.Black,
					Color.Black, mode)
			{ InterpolationColors = cg.GetColorBlend() };
			DrawTextWithBrush(text, brush, g, offsetPoint);
			brush.Dispose();
		}

		private void DrawTextAcrossGradient(String text, Graphics g, Point p, LinearGradientMode mode)
		{
			var offsetPoint = new Point(p.X + (_maxTextSize - (int)_textsize.Width) / 2, p.Y);
			ColorGradient cg = Colors[_currentShapeColor];
			var brush = new LinearGradientBrush(new Rectangle(0, 0, _maxTextSize, (int)_textsize.Height),
					Color.Black,
					Color.Black, mode)
			{ InterpolationColors = cg.GetColorBlend() };
			DrawTextWithBrush(text, brush, g, offsetPoint);
			brush.Dispose();
		}

		private void DrawTextWithBrush(string text, Brush brush, Graphics g, Point p)
		{
			g.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
			g.DrawString(text, _newfont, brush, p);
		}

		private void CreateShapes()
		{
			while (_shapesCount > _shapes.Count)
			{
				ShapesClass m = new ShapesClass();

				//Sets Radius size and Shape location
				if (RandomShapeSize)
				{
					m.Size = _random.Next(2, 4);
				}
				else
				{
					m.Size = _random.Next(5, CalculateSize(_intervalPosFactor));
				}
				m.LocationX = _random.Next(0, BufferWi - 1);
				m.LocationY = _random.Next(0, BufferHt - 1);

				double speed = _random.NextDouble() * (_maxSpeed - _minSpeed) + _minSpeed;
				double vx = _random.NextDouble() * speed;
				double vy = _random.NextDouble() * speed;
				if (_random.Next(0, 2) == 0) vx = -vx;
				if (_random.Next(0, 2) == 0) vy = -vy;
				m.VelocityX = vx;
				m.VelocityY = vy;
				//m.Size = _random.Next(5, 8);//radius;
				m.ShapesGuid = Guid.NewGuid();
				m.ColorIndex = _random.Next(0, Colors.Count);
				//int num = _random.Next(0, 26); // Zero to 25

				switch (ShapeList)
				{
					case ShapeList.GeometricShapes:
						if (GeometricShapes == GeometricShapes.Random)
						{
							m.Shape = new string((char) ('A' + _random.Next(0, 26)), 1);
						}
						else
						{
							m.Shape = GeometricShapes.ToString();
						}
						
						break;
					case ShapeList.ChristmasShapes:
						if (ChristmasShapes == ChristmasShapes.Random)
						{
							m.Shape = new string((char)('A' + _random.Next(0, 26)), 1);
						}
						else
						{
							m.Shape = ChristmasShapes.ToString();
						}
						break;
					case ShapeList.FontShapes:
						if (FontShapes == FontShapes.Random)
						{
							m.Shape = new string((char)('A' + _random.Next(0, 26)), 1);
						}
						else
						{
							m.Shape = FontShapes.ToString();
						}
						break;
				}
				
				m.RotateAngle = _random.Next(0, 360);
				m.RotateCW = _random.Next(0, 2);
				_shapes.Add(m);
			}
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

				if (!ManualPosition)
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

				//int previousBallSize = CalculateSize(_intervalPosFactor - 1);
				//if (_radius > previousBallSize || _radius < previousBallSize)
				//{
				//	double ratio = (double) CalculateSize(_intervalPosFactor) / previousBallSize;

				double angleSpeed = _random.NextDouble() * (maxAngleSpeed - minAngleSpeed) + minAngleSpeed;
				double sizeSpeed = (_random.NextDouble() * (maxSizeSpeed - minSizeSpeed) + minSizeSpeed) / 10;

				if (RandomShapeSize)
				{
					if (sizeSpeed > 1) shape.Size *= sizeSpeed; //ratio;

					if (shape.Size > CalculateMaxSize(_intervalPosFactor)) //(int)(Math.Min(BufferWi, BufferHt) * 0.75))
					{
						_removeShapes.Add(shape);
					}
				}
				else
				{
					shape.Size = CalculateSize(_intervalPosFactor);
				}

				//shape.RotateAngle = CalculateAngle(_intervalPosFactor);
				if (shape.RotateCW == 1)
				{
					if (shape.RotateAngle > 358)
					{
						shape.RotateAngle = 0;
					}
					else
					{
						shape.RotateAngle += (int)angleSpeed;
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
						shape.RotateAngle -= (int)angleSpeed;
					}
				}

				// Move the shape.
				if (!ManualPosition)
				{
					shape.LocationX = shape.LocationX + shape.VelocityX;
					shape.LocationY = shape.LocationY + shape.VelocityY;

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
							if (shape.LocationX - shape.Size < 0)
							{
								shape.LocationX = shape.Size;
								shape.VelocityX = -shape.VelocityX;
							}
							else if (shape.LocationX + shape.Size >= BufferWi)
							{
								shape.LocationX = BufferWi - shape.Size - 1;
								shape.VelocityX = -shape.VelocityX;
							}
							if (shape.LocationY - shape.Size < 0)
							{
								shape.LocationY = shape.Size;
								shape.VelocityY = -shape.VelocityY;
							}
							else if (shape.LocationY + shape.Size >= BufferHt)
							{
								shape.LocationY = BufferHt - shape.Size - 1;
								shape.VelocityY = -shape.VelocityY;
							}
							break;

						case ShapeType.Wrap:

							if (shape.LocationX + shape.Size < 0)
							{
								shape.LocationX += BufferWi + (shape.Size * 2);
							}
							if (shape.LocationY + shape.Size < 0)
							{
								shape.LocationY += BufferHt + (shape.Size * 2);
							}
							if (shape.LocationX - shape.Size > BufferWi)
							{
								shape.LocationX -= BufferWi + (shape.Size * 2);
							}
							if (shape.LocationY - shape.Size > BufferHt)
							{
								shape.LocationY -= BufferHt + (shape.Size * 2);
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

		public class ShapesClass
		{
			public double LocationX;
			public double LocationY;
			public double VelocityX;
			public double VelocityY;
			public int ColorIndex;
			public double Size;
			public Guid ShapesGuid;
			public String Shape;
			public int RotateAngle;
			public int RotateCW;
		}
		
		private int CalculateShapeCount(double intervalPosFactor)
		{
			int value = (int)ScaleCurveToValue(ShapeCountCurve.GetValue(intervalPosFactor), 50, 1);
			if (value < 1) value = 1;
			return value;
		}

		private double CalculateCenterAngleSpeed(double intervalPosFactor)
		{
			return ScaleCurveToValue(CenterAngleSpeedCurve.GetValue(intervalPosFactor), (double)_maxBuffer / 5, 0);
		}

		private double CalculateAngleSpeedVariation(double intervalPosFactor)
		{
			return ScaleCurveToValue(AngleSpeedVariationCurve.GetValue(intervalPosFactor), (double)_maxBuffer / 5, 0);
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

		private int CalculateSize(double intervalPosFactor)
		{
			int value = (int)ScaleCurveToValue(SizeCurve.GetValue(intervalPosFactor), (int)(_maxBuffer), 4);
			if (value < 1) value = 1;
			return value;
		}

		private int CalculateMaxSize(double intervalPosFactor)
		{
			int value = (int)ScaleCurveToValue(MaxSizeCurve.GetValue(intervalPosFactor), (int)(_maxBuffer), 1);
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

	}
}
