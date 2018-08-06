using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using NLog;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using ZedGraph;

namespace VixenModules.Effect.Picture
{

	public class Picture : PixelEffectBase
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private PictureData _data;

		private double _currentGifImageNum;
		private Image _image;
		private Bitmap _scaledImage;
		private int _frameCount;
		private FrameDimension _dimension;
		private FastPixel.FastPixel _fp;
		private bool _enableColorEffect;
		private double _movementX = 0.0;
		private double _movementY = 0.0;
		private double _position = 0.0;
		private bool _gifSpeed;
		private bool _movementRate;
		private double _level;
		private double _adjustedBrightness;
		private int _xOffsetAdj;
		private int _yOffsetAdj;
		private int _imageWidth;
		private int _imageHeight;
		private int _imageHt;
		private int _imageWi;
		private int _xoffset;
		private int _yoffset;
		private List<PictureClass> _pictures;

		public Picture()
		{
			_data = new PictureData();
			EnableTargetPositioning(true, true);
			UpdateAttributes();
		}

		#region Movement

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Type")]
		[ProviderDescription(@"EffectType")]
		[PropertyOrder(0)]
		public EffectType Type
		{
			get { return _data.EffectType; }
			set
			{
				_data.EffectType = value;
				IsDirty = true;
				UpdateDirectionAttribute();
				_movementRate = false;
				if (Type == EffectType.RenderPictureDownleft || Type == EffectType.RenderPictureDownright ||
				    Type == EffectType.RenderPictureUpleft || Type == EffectType.RenderPictureUpright)
				{
					_movementRate = true;
				}
				UpdateMovementRateAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction based on angle")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 360, 1)]
		[PropertyOrder(1)]
		public int Direction
		{
			get { return _data.Direction; }
			set
			{
				_data.Direction = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 2)]
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 20, 1)]
		[PropertyOrder(2)]
		public int Speed
		{
			get { return _data.Speed; }
			set
			{
				_data.Speed = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"GIF Interations")]
		[ProviderDescription(@"Number of Iterations for the Gif file")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(3)]
		public int GifSpeed
		{
			get { return _data.GifSpeed; }
			set
			{
				_data.GifSpeed = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"XOffset")]
		[ProviderDescription(@"XOffset")]
		//[NumberRange(-100, 100, 1)]
		[PropertyOrder(4)]
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
		[ProviderCategory(@"Movement", 1)]
		[ProviderDisplayName(@"YOffset")]
		[ProviderDescription(@"YOffset")]
		//[NumberRange(-100, 100, 1)]
		[PropertyOrder(5)]
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

		#endregion

		#region Config

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Orientation")]
		[ProviderDescription(@"Orientation")]
		[Browsable(false)]
		public StringOrientation Orientation
		{
			get { return _data.Orientation; }
			set
			{
				_data.Orientation = value;
				StringOrientation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Picture Source")]
		[ProviderDescription(@"Picture Source")]
		[PropertyOrder(0)]
		public PictureSource Source
		{
			get { return _data.Source; }
			set
			{
				_data.Source = value;
				IsDirty = true;
				UpdatePictureSourceAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Filename")]
		[ProviderDescription(@"Filename")]
		[PropertyEditor("ImagePathEditor")]
		[PropertyOrder(1)]
		public String FileName
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
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Embeded Pictures")]
		[ProviderDescription(@"Embeded Pictures")]
		[PropertyOrder(2)]
		public TilePictures TilePictures
		{
			get { return _data.TilePictures; }
			set
			{
				_data.TilePictures = value;
				IsDirty = true;

				if (TilePictures == TilePictures.Checkers)
				{
					ColorEffect = ColorEffect.CustomColor;
				}
				UpdateColorAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"StretchToGrid")]
		[ProviderDescription(@"StretchToGrid")]
		[PropertyOrder(3)]
		public bool StretchToGrid
		{
			get { return _data.StretchToGrid; }
			set
			{
				_data.StretchToGrid = value;
				IsDirty = true;
				UpdateScaleAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"ScaleToGrid")]
		[ProviderDescription(@"ScaleToGrid")]
		[PropertyOrder(4)]
		public bool ScaleToGrid
		{
			get { return _data.ScaleToGrid; }
			set
			{
				_data.ScaleToGrid = value;
				IsDirty = true;
				UpdateScaleAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"ScalePercent")]
		[ProviderDescription(@"ScalePercent")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(5)]
		public int ScalePercent
		{
			get { return _data.ScalePercent; }
			set
			{
				_data.ScalePercent = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 2)]
		[ProviderDisplayName(@"Movement Rate")]
		[ProviderDescription(@"MovementRate")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(6)]
		public int MovementRate
		{
			get { return _data.MovementRate; }
			set
			{
				_data.MovementRate = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

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

		#region Color

		[Value]
		[ProviderCategory(@"Effect Color", 3)]
		[ProviderDisplayName(@"Color Effect")]
		[ProviderDescription(@"ColorEffect")]
		[PropertyOrder(0)]
		public ColorEffect ColorEffect
		{
			get { return _data.ColorEffect; }
			set
			{
				_data.ColorEffect = value;
				IsDirty = true;
				UpdateColorAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Effect Color", 3)]
		[ProviderDisplayName(@"ColorGradient")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(1)]
		public ColorGradient Colors
		{
			get { return _data.Colors; }
			set
			{
				_data.Colors = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Level properties

		[Value]
		[ProviderCategory(@"Brightness", 4)]
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

		[Value]
		[ProviderCategory(@"Brightness", 4)]
		[ProviderDisplayName(@"Increase Brightness")]
		[ProviderDescription(@"Increase Brightness")]
		//[NumberRange(10, 100, 1)]
		[PropertyOrder(1)]
		public Curve IncreaseBrightnessCurve
		{
			get { return _data.IncreaseBrightnessCurve; }
			set
			{
				_data.IncreaseBrightnessCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		private void UpdateAttributes()
		{
			UpdateColorAttribute(false);
			UpdateDirectionAttribute(false);
			UpdateGifSpeedAttribute(false);
			UpdateScaleAttribute(false);
			UpdateMovementRateAttribute(false);
			UpdateStringOrientationAttributes();
			UpdatePictureSourceAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		private void UpdatePictureSourceAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"FileName", Source == PictureSource.File},
				{"TilePictures", Source != PictureSource.File}
			};

			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateColorAttribute(bool refresh = true)
		{
			_enableColorEffect = ColorEffect == ColorEffect.CustomColor;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"Colors", _enableColorEffect}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateMovementRateAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"MovementRate", _movementRate}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateDirectionAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{"Direction", Type == EffectType.RenderPictureTiles},

				{"Speed", Type != EffectType.RenderPictureNone}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateScaleAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{"ScalePercent", !ScaleToGrid && !StretchToGrid},

				{"ScaleToGrid", !StretchToGrid}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateGifSpeedAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"GifSpeed", !_gifSpeed}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		#region Information

		public override string Information
		{
			get { return "Visit the Vixen Lights website for more information on this effect."; }
		}

		public override string InformationLink
		{
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/picture/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as PictureData;
				UpdateAttributes();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private string ConvertPath(string path)
		{
			if (string.IsNullOrEmpty(path))
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
			var destPath = Path.Combine(PictureDescriptor.ModulePath, name);
			if (path != destPath)
			{
				File.Copy(path, destPath, true);
			}
			return name;
		}

		protected override void SetupRender()
		{
			_pictures = new List<PictureClass>(32);
			if (_scaledImage != null)
			{
				_scaledImage.Dispose();
			}
			if (!string.IsNullOrEmpty(FileName) && Source == PictureSource.File)
			{
				var filePath = Path.Combine(PictureDescriptor.ModulePath, FileName);
				if (File.Exists(filePath))
				{
					using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
					{
						var ms = new MemoryStream();
						fs.CopyTo(ms);
						ms.Position = 0;
						_image = Image.FromStream(ms);
					}

				}
				else
				{
					Logging.Error("File is missing or invalid path. {0}", filePath);
					FileName = "";
				}
			}
			else
			{
				if (Source == PictureSource.Embedded)
				{
					var fs =
						typeof(Picture).Assembly.GetManifestResourceStream("VixenModules.Effect.Picture.PictureTiles." +
						                                                   TilePictures + ".png");
					_image = Image.FromStream(fs);
				}
			}

			if (_image != null)
			{
				_dimension = new FrameDimension(_image.FrameDimensionsList[0]);
				// Number of frames
				_frameCount = _image.GetFrameCount(_dimension);
				_gifSpeed = true;
				if (_frameCount > 1)
				{
					_gifSpeed = false;
				}
				UpdateGifSpeedAttribute();
				if (Colors == null) //This will only be null for Picture effects that are already on the time line. This is due to the upgrade for the effect.
				{
					Colors = new ColorGradient(Color.DodgerBlue);
					Direction = 0;
					GifSpeed = 1;
					ColorEffect = ColorEffect.None;
					MovementRate = 4;
				}
				int currentImage = -1;
				for (int i = 0; i < GetNumberFrames(); i++)
				{
					PictureClass pic = new PictureClass();
					CalculateImageNumberByPosition((GetEffectTimeIntervalPosition(i) * GifSpeed) % 1);
					if (_currentGifImageNum > currentImage)
					{
						_image.SelectActiveFrame(_dimension, (int) _currentGifImageNum);

						if (StretchToGrid)
						{
							pic.bitmap = new Bitmap(BufferWi, BufferHt);
							Graphics.FromImage(pic.bitmap).DrawImage(_image, 0, 0, BufferWi, BufferHt);
						}
						else
						{
							pic.bitmap = ScaleToGrid
							? ScalePictureImage(_image, BufferWi, BufferHt)
							: ScaleImage(_image, (double)ScalePercent / 100);
						}

						if (ColorEffect == ColorEffect.GreyScale)
						{
							pic.bitmap = (Bitmap)ConvertToGrayScale(pic.bitmap);
						}
						pic.frame = i;
						_pictures.Add(pic);
					}
					currentImage = (int)_currentGifImageNum;
					if (_frameCount == 1) break;
				}
				if (_image != null)
				{
					_image.Dispose();
					_image = null;
				}
			}
			_movementX = 0.0;
			_movementY = 0.0;
		}

		protected override void CleanUpRender()
		{
			_dimension = null;
			if (_scaledImage != null)
			{
				_scaledImage.Dispose();
			}
			_pictures = null;
		}


		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			if (_fp != null || _pictures.Count > 0)
			{
				InitialRender(frame);

				for (int x = 0; x < _imageWi; x++)
				{
					for (int y = 0; y < _imageHt; y++)
					{
						CalculatePixel(x, y, frameBuffer, frame);
					}
				}
				_fp.Unlock(false);
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;

				if (_fp != null || _pictures.Count > 0)
				{
					InitialRender(frame);

					foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
					{
						foreach (var elementLocation in elementLocations)
						{
							CalculatePixel(elementLocation.X, elementLocation.Y, frameBuffer, frame);
						}
					}

				}
				_fp.Unlock(false);
			}
		}

		private void InitialRender(int frame)
		{
			if (_fp != null || _pictures.Count > 0)
			{
				var dir = 360 - Direction;
				var intervalPos = GetEffectTimeIntervalPosition(frame);
				var intervalPosFactor = intervalPos * 100;
				_level = LevelCurve.GetValue(intervalPosFactor) / 100;
				_adjustedBrightness = (double) (CalculateIncreaseBrightness(intervalPosFactor)) / 10;

				if (Type != EffectType.RenderPictureTiles && Type != EffectType.RenderPictureNone)
				{
					_position = ((GetEffectTimeIntervalPosition(frame) * Speed) % 1);
				}
				else if (frame == 0)
				{
					_position = ((GetEffectTimeIntervalPosition(frame + 1) * Speed) % 1);
				}

				for ( int i = 0; i < _pictures.Count; i++)
				{
					if ( frame >= _pictures[i].frame)
					{
						_fp = new FastPixel.FastPixel(_pictures[i].bitmap);
						_pictures.RemoveAt(i);
						break;
					}
				}

				_imageWidth = _fp.Width;
				_imageHeight = _fp.Height;
				double deltaX = 0;
				double deltaY = 0;
				double angleOffset;

				if (dir > 45 && dir <= 90)
					angleOffset = -(dir - 90);
				else if (dir > 90 && dir <= 135)
					angleOffset = dir - 90;
				else if (dir > 135 && dir <= 180)
					angleOffset = -(dir - 180);
				else if (dir > 180 && dir <= 225)
					angleOffset = dir - 180;
				else if (dir > 225 && dir <= 270)
					angleOffset = -(dir - 270);
				else if (dir > 270 && dir <= 315)
					angleOffset = dir - 270;
				else if (dir > 315 && dir <= 360)
					angleOffset = -(dir - 360);
				else
					angleOffset = dir;

				double imageSpeed = _position * (_imageWidth / (Math.Cos((Math.PI / 180) * angleOffset)));

				//Moving left and right
				if (dir > 0 && dir <= 90)
				{
					deltaX = ((double) dir / 90) * (imageSpeed);
				}
				else if (dir > 90 && dir <= 180)
				{
					deltaX = ((double) Math.Abs(dir - 180) / 90) * (imageSpeed);
				}
				else if (dir > 180 && dir <= 270)
				{
					deltaX = -1 * (((double) Math.Abs(dir - 180) / 90) * (imageSpeed));
				}
				else if (dir > 270 && dir <= 360)
				{
					deltaX = -1 * (((double) Math.Abs(dir - 360) / 90) * (imageSpeed));
				}

				//Moving up and down
				if (dir >= 0 && dir <= 90)
				{
					deltaY = (((double) Math.Abs(dir - 90) / 90)) * (imageSpeed);
				}
				else if (dir > 90 && dir <= 180)
				{
					deltaY = -1 * (((double) Math.Abs(dir - 90) / 90) * (imageSpeed));
				}
				else if (dir > 180 && dir <= 270)
				{
					deltaY = -1 * (((double) Math.Abs(dir - 270) / 90) * (imageSpeed));
				}
				else if (dir > 270 && dir <= 360)
				{
					deltaY = ((double) Math.Abs(270 - dir) / 90) * (imageSpeed);
				}

				_movementX += deltaX;
				_movementY += deltaY;

				_fp.Lock();

				_yoffset = (BufferHt + _imageHeight) / 2;
				_xoffset = (_imageWidth - BufferWi) / 2;
				_xOffsetAdj = CalculateXOffset(intervalPosFactor) * BufferWi / 100;
				_yOffsetAdj = CalculateYOffset(intervalPosFactor) * BufferHt / 100;
				_imageHt = _imageHeight;
				_imageWi = _imageWidth;

				switch (Type)
				{
					case EffectType.RenderPicturePeekaboo0:
					case EffectType.RenderPicturePeekaboo180:
						//Peek a boo
						_yoffset = -(BufferHt) + (int) ((BufferHt + 5) * _position * 2);
						if (_yoffset > 10)
						{
							_yoffset = -_yoffset + 10; //reverse direction
						}
						else if (_yoffset >= -1)
						{
							_yoffset = -1; //pause in middle
						}
						break;
					case EffectType.RenderPictureWiggle:
						if (_position >= 0.5)
						{
							_xoffset += (int) (_imageWi * ((1.0 - _position) * 2.0 - 0.5));
						}
						else
						{
							_xoffset += (int) (_imageWi * (_position * 2.0 - 0.5));
						}
						break;
					case EffectType.RenderPicturePeekaboo90: //peekaboo 90
					case EffectType.RenderPicturePeekaboo270: //peekaboo 270
						if (Orientation == StringOrientation.Horizontal || TargetPositioning == TargetPositioningType.Locations)
						{
							_yoffset = (BufferHt - _imageWi) / 2; //adjust offsets for other axis	
							_xoffset = -(BufferWi) + (int)((BufferWi + 5) * _position * 2);
						}
						else
						{
							_yoffset = (BufferWi) / 2; //adjust offsets for other axis
							_xoffset = -(BufferHt) + (int)((BufferHt + 5) * _position * 2);
						}

						if (_xoffset > 10)
						{
							_xoffset = -_xoffset + 10; //reverse direction
						}
						else if (_xoffset >= -1)
						{
							_xoffset = -1; //pause in middle
						}

						break;
					case EffectType.RenderPictureTiles:
						_imageHt = BufferHt;
						_imageWi = BufferWi;
						break;

				}
			}
		}

		private void CalculatePixel(int x, int y, IPixelFrameBuffer frameBuffer, int frame)
		{
			int yCoord = y;
			int xCoord = x;
			int speedFactor = 4;
			int state = MovementRate * frame;
			int locationY = 0;
			int locationX = 0;

			Color fpColor = new Color();

			if (TargetPositioning == TargetPositioningType.Locations)
			{
				//Flip me over and offset my coordinates so I can act like the string version
				y = Math.Abs((BufferHtOffset - y) + (BufferHt - 1 + BufferHtOffset));
				y = y - BufferHtOffset;
				x = x - BufferWiOffset;
			}
			else
			{
				if (Type != EffectType.RenderPictureTiles) // change this so only when tiles are disabled
				{
					fpColor = CustomColor(frame, _level, _fp.GetPixel(x, y), _adjustedBrightness);
				}
			}
			switch (Type)
			{
				case EffectType.RenderPicturePeekaboo0:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = BufferHt + _yoffset - y + _yOffsetAdj;
						locationX = x + _xoffset - _xOffsetAdj;
						break;
					}
					frameBuffer.SetPixel(xCoord - _xoffset + _xOffsetAdj, BufferHt + _yoffset - yCoord + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureWiggle:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _yoffset - y + _yOffsetAdj;
						locationX = x - _xoffset - _xOffsetAdj - BufferWi + _imageWi;
						break;
					}
					frameBuffer.SetPixel(xCoord + _xoffset + _xOffsetAdj + BufferWi - _imageWi, _yoffset - yCoord + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPicturePeekaboo90:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _xoffset + BufferWi - x - _xOffsetAdj;
						locationX = BufferHt - y - _yOffsetAdj - _yoffset;
						break;
					}
					frameBuffer.SetPixel(BufferWi + _xoffset - yCoord + _xOffsetAdj, BufferHt - xCoord - _yoffset + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPicturePeekaboo180:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = y - _yOffsetAdj + BufferHt - _imageHt + _yoffset;
						locationX = (BufferWi - x) + _xoffset + _xOffsetAdj;
						break;
					}
					if (Orientation == StringOrientation.Horizontal)
					{
						frameBuffer.SetPixel((_imageWi - xCoord) - _xoffset + _xOffsetAdj - BufferWi + BufferWi, yCoord - _yoffset + _yOffsetAdj, fpColor);
					}
					else
					{
						frameBuffer.SetPixel((BufferWi - xCoord) - _xoffset + _xOffsetAdj - _imageWi + BufferWi, yCoord - _yoffset + _yOffsetAdj, fpColor);
					}
					return;
				case EffectType.RenderPicturePeekaboo270:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _xoffset + x - _xOffsetAdj;
						locationX = y - _yOffsetAdj - _yoffset;
						break;
					}
					frameBuffer.SetPixel(yCoord - _xoffset + _xOffsetAdj, BufferHt + _yoffset - (BufferHt - xCoord) + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureLeft:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _yoffset - y + _yOffsetAdj;
						locationX = ((x - BufferWi) + (int)(_position * (_imageWi + BufferWi))) + _xOffsetAdj;
						break;
					}
					int leftX = xCoord + (BufferWi - (int)(_position * (_imageWi + BufferWi)));
					frameBuffer.SetPixel(leftX + _xOffsetAdj, _yoffset - y + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureRight:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _yoffset - y + _yOffsetAdj;
						locationX = ((x + _imageWi) - (int)(_position * (_imageWi + BufferWi))) + _xOffsetAdj;
						break;
					}
					int rightX = xCoord + -_imageWi + (int)(_position * (_imageWi + BufferWi));
					frameBuffer.SetPixel(rightX + _xOffsetAdj, _yoffset - yCoord + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureUp:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((int)((_imageHt + BufferHt) * _position) - y) + _yOffsetAdj;
						locationX = x + _xoffset - _xOffsetAdj;
						break;
					}
					int upY = (int)((_imageHt + BufferHt) * _position) - y;
					frameBuffer.SetPixel(xCoord - _xoffset + _xOffsetAdj, upY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureDown:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((BufferHt + _imageHt - 1) - (int)((_imageHt + BufferHt) * _position) - y) + _yOffsetAdj;
						locationX = x + _xoffset - _xOffsetAdj;
						break;
					}
					int downY = (BufferHt + _imageHt - 1) - (int)((_imageHt + BufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(x - _xoffset + _xOffsetAdj, downY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureUpleft:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((int)((_imageHt + BufferHt) * _position) - y) + _yOffsetAdj;
						locationX = ((x - BufferWi) + (int)(_position * (state % ((_imageWi + BufferWi) * speedFactor)) / speedFactor)) + _xOffsetAdj;
						break;
					}
					int upLeftY = (int)((_imageHt + BufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(
						Convert.ToInt32(xCoord + BufferWi - (state % ((_imageWi + BufferWi) * speedFactor)) / speedFactor) + _xOffsetAdj,
						upLeftY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureDownleft:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((BufferHt + _imageHt - 1) - (int)((_imageHt + BufferHt) * _position) - y) + _yOffsetAdj;
						locationX = ((x - BufferWi) + (int)(_position * (state % ((_imageWi + BufferWi) * speedFactor)) / speedFactor)) + _xOffsetAdj;
						break;
					}
					int downLeftY = BufferHt + _imageHt - (int)((_imageHt + BufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(
						Convert.ToInt32(xCoord + BufferWi - (state % ((_imageWi + BufferWi) * speedFactor)) / speedFactor) + _xOffsetAdj,
						downLeftY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureUpright:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((int)((_imageHt + BufferHt) * _position) - y) + _yOffsetAdj;
						locationX = ((x + _imageWi) - (int)(_position * (state % ((_imageWi + BufferWi) * speedFactor)) / speedFactor)) + _xOffsetAdj;
						break;
					}
					int upRightY = (int)((_imageHt + BufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(
						Convert.ToInt32(xCoord + (state % ((_imageWi + BufferWi) * speedFactor)) / speedFactor - _imageWi) + _xOffsetAdj,
						upRightY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureDownright:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((BufferHt + _imageHt - 1) - (int)((_imageHt + BufferHt) * _position) - y) + _yOffsetAdj;
						locationX = ((x + _imageWi) - (int)(_position * (state % ((_imageWi + BufferWi) * speedFactor)) / speedFactor)) + _xOffsetAdj;
						break;
					}
					int downRightY = BufferHt + _imageHt - (int)((_imageHt + BufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(
						Convert.ToInt32(xCoord + (state % ((_imageWi + BufferWi) * speedFactor)) / speedFactor - _imageWi) + _xOffsetAdj,
						downRightY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureNone:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _yoffset - y + _yOffsetAdj;
						locationX = x + _xoffset - _xOffsetAdj;
						break;
					}
					frameBuffer.SetPixel(xCoord - _xoffset + _xOffsetAdj, _yoffset - yCoord + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureTiles:
					locationX = x + Convert.ToInt32(_movementX) - (_xOffsetAdj * BufferWi / 100);
					locationY = (BufferHt - y) + Convert.ToInt32(_movementY) + (_yOffsetAdj * BufferHt / 100);

					if (locationX >= 0)
					{
						locationX = locationX % _imageWidth;
					}
					else if (locationX < 0)
					{
						locationX = Convert.ToInt32(locationX % _imageWidth) + _imageWidth - 1;
					}

					if (locationY >= 0)
					{
						locationY = Convert.ToInt32((locationY % _imageHeight));
					}
					else if (locationY < 0)
					{
						locationY = Convert.ToInt32(locationY % _imageHeight) + _imageHeight - 1;
					}
					break;
			}

			//will only get to here if Location and/or Tiles is selected.
			if (locationX < _fp.Width && locationY < _fp.Height && locationX >= 0 && locationY >= 0)
			{
				fpColor = CustomColor(frame, _level, _fp.GetPixel(locationX, locationY), _adjustedBrightness);
				frameBuffer.SetPixel(xCoord, yCoord, fpColor);
			}
		}

		private int CalculateXOffset(double intervalPos)
		{
			return (int) Math.Round(ScaleCurveToValue(XOffsetCurve.GetValue(intervalPos), 100, -100));
		}

		private int CalculateYOffset(double intervalPos)
		{
			return (int) Math.Round(ScaleCurveToValue(YOffsetCurve.GetValue(intervalPos), 100, -100));
		}

		private double CalculateIncreaseBrightness(double intervalPos)
		{
			return ScaleCurveToValue(IncreaseBrightnessCurve.GetValue(intervalPos), 100, 10);
		}

		private Color CustomColor(int frame, double level, Color fpColor, double adjustedBrightness)
		{
			if (ColorEffect == ColorEffect.CustomColor)
			{
				Color newColor = new Color();
				newColor = Colors.GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100);
				double hsvLevel = Convert.ToInt32(fpColor.GetBrightness() * 255);
				HSV hsv = HSV.FromRGB(newColor);
				hsv.V = hsvLevel / 100;
				fpColor = hsv.ToRGB();
			}

			if (level < 1 || adjustedBrightness > 1)
			{
				HSV hsv = HSV.FromRGB(fpColor);
				double tempV = hsv.V * level * adjustedBrightness;
				if (tempV > 1) tempV = 1;
				hsv.V = tempV;
				fpColor = hsv.ToRGB();
			}

			return fpColor;
		}

		private void CalculateImageNumberByPosition(double position)
		{
			_currentGifImageNum = Math.Round(position * (_frameCount - 1));
		}

		public static Bitmap ScalePictureImage(Image image, int maxWidth, int maxHeight)
		{
			lock (image)
			{
				var ratioX = (double) maxWidth / image.Width;
				var ratioY = (double) maxHeight / image.Height;
				var ratio = Math.Min(ratioX, ratioY);
				var newWidth = (int) (image.Width * ratio);
				var newHeight = (int) (image.Height * ratio);
				if (newHeight <= 0) newHeight = 1;
				if (newWidth <= 0) newWidth = 1;
				var newImage = new Bitmap(newWidth, newHeight);
				Graphics.FromImage(newImage).DrawImage(image, 0, 0, newWidth, newHeight);
				return newImage;
			}

		}

		public static Bitmap ScaleImage(Image image, double scale)
		{
			lock (image)
			{
				int maxWidth = Convert.ToInt32((double) image.Width * scale);
				int maxHeight = Convert.ToInt32((double) image.Height * scale);
				var ratioX = (double) maxWidth / image.Width;
				var ratioY = (double) maxHeight / image.Height;
				var ratio = Math.Min(ratioX, ratioY);

				var newWidth = (int) (image.Width * ratio);
				var newHeight = (int) (image.Height * ratio);

				var newImage = new Bitmap(newWidth, newHeight);
				using (var g = Graphics.FromImage(newImage))
				{
					g.DrawImage(image, 0, 0, newWidth, newHeight);
					return newImage;
				}
			}
		}

		public static Image ConvertToGrayScale(Image srce)
		{
			Bitmap bmp = new Bitmap(srce.Width, srce.Height);
			using (Graphics gr = Graphics.FromImage(bmp))
			{
				var matrix = new float[][]
				{
					new float[] {0.299f, 0.299f, 0.299f, 0, 0},
					new float[] {0.587f, 0.587f, 0.587f, 0, 0},
					new float[] {0.114f, 0.114f, 0.114f, 0, 0},
					new float[] {0, 0, 0, 1, 0},
					new float[] {0, 0, 0, 0, 1}
				};
				var ia = new System.Drawing.Imaging.ImageAttributes();
				ia.SetColorMatrix(new System.Drawing.Imaging.ColorMatrix(matrix));
				var rc = new Rectangle(0, 0, srce.Width, srce.Height);
				gr.DrawImage(srce, rc, 0, 0, srce.Width, srce.Height, GraphicsUnit.Pixel, ia);
				return bmp;
			}
		}

		public static bool IsPictureTileResource(string s)
		{
			return s.Contains("VixenModules.Effect.Picture.PictureTiles");
		}
		public class PictureClass
		{
			public int frame;
			public Bitmap bitmap;
		}
	}
}
