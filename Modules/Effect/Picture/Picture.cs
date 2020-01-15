using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Picture
{

	public class Picture : PixelEffectBase
	{
		private PictureData _data;

		private double _currentGifImageNum;
		private int _frameCount;
		private FastPixel.FastPixel _fp;
		private double _movementX;
		private double _movementY;
		private double _position;
		private bool _gifSpeed;
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
		[NumberRange(1, 100, 1)]
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
			var enableColorEffect = ColorEffect == ColorEffect.CustomColor;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"Colors", enableColorEffect}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		private void UpdateMovementRateAttribute(bool refresh = true)
		{
			bool movementRate = Type == EffectType.RenderPictureDownleft || Type == EffectType.RenderPictureDownright ||
			                    Type == EffectType.RenderPictureUpleft || Type == EffectType.RenderPictureUpright;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1)
			{
				{"MovementRate", movementRate}
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
			Image image = null;
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
						image = Image.FromStream(ms);
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
					image = Image.FromStream(fs);
				}
			}

			if (image != null)
			{
				var dimension = new FrameDimension(image.FrameDimensionsList[0]);
				// Number of frames
				_frameCount = image.GetFrameCount(dimension);
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
						image.SelectActiveFrame(dimension, (int) _currentGifImageNum);

						if (StretchToGrid)
						{
							pic.bitmap = new Bitmap(BufferWi, BufferHt);
							Graphics.FromImage(pic.bitmap).DrawImage(image, 0, 0, BufferWi, BufferHt);
						}
						else
						{
							pic.bitmap = ScaleToGrid
							? ScalePictureImage(image, BufferWi, BufferHt)
							: ScaleImage(image, (double)ScalePercent / 100);
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

				image.Dispose();
			}
			_movementX = 0.0;
			_movementY = 0.0;
		}

		protected override void CleanUpRender()
		{
			_fp?.Dispose();
			_fp = null;
			_pictures = null;
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			InitFrameData(frame, out double intervalPosFactor, out double level, out double adjustedBrightness);
			InitialRender(frame, intervalPosFactor);
			if (_fp != null)
			{
				_fp.Lock();
				var bufferWi = BufferWi;
				var bufferHt = BufferHt;
				for (int x = 0; x < _imageWi; x++)
				{
					for (int y = 0; y < _imageHt; y++)
					{
						CalculatePixel(x, y, frameBuffer, frame, level, adjustedBrightness, ref bufferHt, ref bufferWi);
					}
				}
				_fp.Unlock(false);
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			var bufferWi = BufferWi;
			var bufferHt = BufferHt;

			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;

				InitFrameData(frame, out double intervalPosFactor, out double level, out double adjustedBrightness);
				InitialRender(frame, intervalPosFactor);
				if (_fp != null)
				{
					_fp.Lock();
					
					foreach (var elementLocation in frameBuffer.ElementLocations)
					{
						CalculatePixel(elementLocation.X, elementLocation.Y, frameBuffer, frame, level, adjustedBrightness, ref bufferHt, ref bufferWi);
					}
					
					_fp.Unlock(false);
				}
			}
		}

		private void InitFrameData(int frame, out double intervalPosFactor, out double level, out double adjustedBrightness)
		{
			var intervalPos = GetEffectTimeIntervalPosition(frame);
			intervalPosFactor = intervalPos * 100;
			level = LevelCurve.GetValue(intervalPosFactor) / 100;
			adjustedBrightness = CalculateIncreaseBrightness(intervalPosFactor) / 10;
		}

		private void InitialRender(int frame, double intervalPosFactor)
		{
			if (_fp != null || _pictures.Count > 0)
			{
				var dir = 360 - Direction;
				
				if (Type != EffectType.RenderPictureTiles && Type != EffectType.RenderPictureNone)
				{
					_position = ((GetEffectTimeIntervalPosition(frame) * Speed) % 1);
				}
				else if (frame == 0)
				{
					_position = ((GetEffectTimeIntervalPosition(frame + 1) * Speed) % 1);
				}

				if (_pictures.Count > 0)
				{
					for (int i = 0; i < _pictures.Count; i++)
					{
						if (frame >= _pictures[i].frame)
						{
							_fp = new FastPixel.FastPixel(_pictures[i].bitmap);
							_pictures.RemoveAt(i);
							break;
						}
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

				_yoffset = ((BufferHt + _imageHeight) / 2) - 1; //subtract 1 because we are zero based and inverting the y 
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

		private void CalculatePixel(int x, int y, IPixelFrameBuffer frameBuffer, int frame, double level, double adjustedBrightness, ref int bufferHt, ref int bufferWi)
		{
			int yCoord = y;
			int xCoord = x;
			int speedFactor = 4;
			int state = MovementRate * frame;
			int locationY = 0;
			int locationX = 0;
			
			Color fpColor = Color.Empty;

			if (TargetPositioning == TargetPositioningType.Locations)
			{
				var bufferHtOffset = BufferHtOffset;
				//Flip me over and offset my coordinates so I can act like the string version
				y = Math.Abs((bufferHtOffset - y) + (bufferHt - 1 + bufferHtOffset));
				y = y - bufferHtOffset;
				x = x - BufferWiOffset;
			}
			else
			{
				if (Type != EffectType.RenderPictureTiles) // change this so only when tiles are disabled
				{
					fpColor = CustomColor(frame, level, _fp.GetPixel(x, y), adjustedBrightness);
				}
			}
			switch (Type)
			{
				case EffectType.RenderPicturePeekaboo0:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = bufferHt + _yoffset - y + _yOffsetAdj;
						locationX = x + _xoffset - _xOffsetAdj;
						break;
					}
					frameBuffer.SetPixel(xCoord - _xoffset + _xOffsetAdj, bufferHt + _yoffset - yCoord + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureWiggle:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _yoffset - y + _yOffsetAdj;
						locationX = x - _xoffset - _xOffsetAdj - bufferWi + _imageWi;
						break;
					}
					frameBuffer.SetPixel(xCoord + _xoffset + _xOffsetAdj + bufferWi - _imageWi, _yoffset - yCoord + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPicturePeekaboo90:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _xoffset + bufferWi - x - _xOffsetAdj;
						locationX = bufferHt - y - _yOffsetAdj - _yoffset;
						break;
					}
					frameBuffer.SetPixel(bufferWi + _xoffset - yCoord + _xOffsetAdj, bufferHt - xCoord - _yoffset + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPicturePeekaboo180:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = y - _yOffsetAdj + bufferHt - _imageHt + _yoffset;
						locationX = (bufferWi - x) + _xoffset + _xOffsetAdj;
						break;
					}
					if (Orientation == StringOrientation.Horizontal)
					{
						frameBuffer.SetPixel((_imageWi - xCoord) - _xoffset + _xOffsetAdj - bufferWi + bufferWi, yCoord - _yoffset + _yOffsetAdj, fpColor);
					}
					else
					{
						frameBuffer.SetPixel((bufferWi - xCoord) - _xoffset + _xOffsetAdj - _imageWi + bufferWi, yCoord - _yoffset + _yOffsetAdj, fpColor);
					}
					return;
				case EffectType.RenderPicturePeekaboo270:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _xoffset + x - _xOffsetAdj;
						locationX = y - _yOffsetAdj - _yoffset;
						break;
					}
					frameBuffer.SetPixel(yCoord - _xoffset + _xOffsetAdj, bufferHt + _yoffset - (bufferHt - xCoord) + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureLeft:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _yoffset - y + _yOffsetAdj;
						locationX = ((x - bufferWi) + (int)(_position * (_imageWi + bufferWi))) + _xOffsetAdj;
						break;
					}
					int leftX = xCoord + (bufferWi - (int)(_position * (_imageWi + bufferWi)));
					frameBuffer.SetPixel(leftX + _xOffsetAdj, _yoffset - y + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureRight:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = _yoffset - y + _yOffsetAdj;
						locationX = ((x + _imageWi) - (int)(_position * (_imageWi + bufferWi))) + _xOffsetAdj;
						break;
					}
					int rightX = xCoord + -_imageWi + (int)(_position * (_imageWi + bufferWi));
					frameBuffer.SetPixel(rightX + _xOffsetAdj, _yoffset - yCoord + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureUp:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((int)((_imageHt + bufferHt) * _position) - y) + _yOffsetAdj;
						locationX = x + _xoffset - _xOffsetAdj;
						break;
					}
					int upY = (int)((_imageHt + bufferHt) * _position) - y;
					frameBuffer.SetPixel(xCoord - _xoffset + _xOffsetAdj, upY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureDown:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((bufferHt + _imageHt - 1) - (int)((_imageHt + bufferHt) * _position) - y) + _yOffsetAdj;
						locationX = x + _xoffset - _xOffsetAdj;
						break;
					}
					int downY = (bufferHt + _imageHt - 1) - (int)((_imageHt + bufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(x - _xoffset + _xOffsetAdj, downY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureUpleft:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((int)((_imageHt + bufferHt) * _position) - y) + _yOffsetAdj;
						locationX = (x - bufferWi + (state % ((_imageWi + bufferWi) * speedFactor)) / speedFactor) + _xOffsetAdj;
						break;
					}
					int upLeftY = (int)((_imageHt + bufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(
						Convert.ToInt32(xCoord + bufferWi - (state % ((_imageWi + bufferWi) * speedFactor)) / speedFactor) + _xOffsetAdj,
						upLeftY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureDownleft:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((bufferHt + _imageHt - 1) - (int)((_imageHt + bufferHt) * _position) - y) + _yOffsetAdj;
						locationX = (x - bufferWi + (state % ((_imageWi + bufferWi) * speedFactor)) / speedFactor) + _xOffsetAdj;
						break;
					}
					int downLeftY = bufferHt + _imageHt - (int)((_imageHt + bufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(
						Convert.ToInt32(xCoord + bufferWi - (state % ((_imageWi + bufferWi) * speedFactor)) / speedFactor) + _xOffsetAdj,
						downLeftY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureUpright:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((int)((_imageHt + bufferHt) * _position) - y) + _yOffsetAdj;
						locationX = (x + _imageWi - (state % ((_imageWi + bufferWi) * speedFactor)) / speedFactor) +_xOffsetAdj;
						break;
					}
					int upRightY = (int)((_imageHt + bufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(
						Convert.ToInt32(xCoord + (state % ((_imageWi + bufferWi) * speedFactor)) / speedFactor - _imageWi) + _xOffsetAdj,
						upRightY + _yOffsetAdj, fpColor);
					return;
				case EffectType.RenderPictureDownright:
					if (TargetPositioning == TargetPositioningType.Locations)
					{
						locationY = ((bufferHt + _imageHt - 1) - (int)((_imageHt + bufferHt) * _position) - y) + _yOffsetAdj;
						locationX = (x + _imageWi - (state % ((_imageWi + bufferWi) * speedFactor)) / speedFactor) + _xOffsetAdj;
						break;
					}
					int downRightY = bufferHt + _imageHt - (int)((_imageHt + bufferHt) * _position) - yCoord;
					frameBuffer.SetPixel(
						Convert.ToInt32(xCoord + (state % ((_imageWi + bufferWi) * speedFactor)) / speedFactor - _imageWi) + _xOffsetAdj,
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
					locationX = x + Convert.ToInt32(_movementX) - (_xOffsetAdj * bufferWi / 100);
					locationY = (bufferHt - y) + Convert.ToInt32(_movementY) + (_yOffsetAdj * bufferHt / 100);

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
				fpColor = CustomColor(frame, level, _fp.GetPixel(locationX, locationY), adjustedBrightness);
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
