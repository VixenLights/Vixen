using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
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

namespace VixenModules.Effect.Bars
{
	public class Bars:PixelEffectBase
	{
		private BarsData _data;
		private double _position;
		private bool _negPosition;

		public Bars()
		{
			_data = new BarsData();
			EnableTargetPositioning(true, true);
			InitAllAttributes();
		}

		public override bool IsDirty
		{
			get
			{
				if(Colors.Any(x => !x.CheckLibraryReference()))
				{
					base.IsDirty = true;
				}

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		#region Setup

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

		#region Config properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		[PropertyOrder(0)]
		public BarDirection Direction
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"MovementType")]
		[ProviderDescription(@"MovementType")]
		[PropertyOrder(1)]
		public MovementType MovementType
		{
			get { return _data.MovementType; }
			set
			{
				_data.MovementType = value;
				IsDirty = true;
				UpdateMovementTypeAttribute();
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
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
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyOrder(3)]
		public Curve SpeedCurve
		{
			get { return _data.SpeedCurve; }
			set
			{
				_data.SpeedCurve = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Repeat")]
		[ProviderDescription(@"Repeat")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 10, 1)]
		[PropertyOrder(4)]
		public int Repeat
		{
			get { return _data.Repeat; }
			set
			{
				_data.Repeat = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Highlight")]
		[ProviderDescription(@"Highlight")]
		[PropertyOrder(5)]
		public bool Highlight
		{
			get { return _data.Highlight; }
			set
			{
				_data.Highlight = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Show3D")]
		[ProviderDescription(@"Show3D")]
		[PropertyOrder(6)]
		public bool Show3D
		{
			get { return _data.Show3D; }
			set
			{
				_data.Show3D = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties


		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"ColorGradients")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(1)]
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

		#endregion

		#region Level properties

		[Value]
		[ProviderCategory(@"Brightness", 3)]
		[ProviderDisplayName(@"Brightness")]
		[ProviderDescription(@"Brightness")]
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
			get { return "http://www.vixenlights.com/vixen-3-documentation/sequencer/effects/bars/"; }
		}

		#endregion

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as BarsData;
				InitAllAttributes();
				IsDirty = true;
			}
		}

		private void InitAllAttributes()
		{
			UpdateStringOrientationAttributes(true);
			UpdateMovementTypeAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void UpdateMovementTypeAttribute(bool refresh = true)
		{
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(2)
			{
				{ "SpeedCurve", MovementType == MovementType.Speed},
				
				{ "Speed", MovementType != MovementType.Speed}
			};
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}

		protected override void SetupRender()
		{
			//Nothing to setup
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			int x, y, n, colorIdx;
			int colorcnt = Colors.Count();
			int barCount = Repeat * colorcnt;
			double intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;
			_negPosition = false;

			if (MovementType == MovementType.Iterations)
			{
				_position = (GetEffectTimeIntervalPosition(frame) * Speed) % 1;
			}
			else
			{
				var s = CalculateSpeed(intervalPosFactor);
				if (frame == 0) _position = s;
				var adj = s / 100 * FrameTime / 50d;
				_position += adj; //Adjust the speed setting for different frame rates with FrameTime / 50d
				//Debug.WriteLine($"S:{s}, PosFactor:{intervalPosFactor}, Frame:{frame}, FrameTime:{FrameTime}, Position:{_position}, Adj:{adj}");
				_negPosition = _position < 0;
			}

			var workingPosition = Math.Abs(_position);

			if (barCount < 1) barCount = 1;
			double level = LevelCurve.GetValue(intervalPosFactor) / 100;
			var bufferHt = BufferHt;
			var bufferWi = BufferWi;

			if (Direction < BarDirection.Left || Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
			{
				int barHt = bufferHt / barCount+1;
				if (barHt < 1) barHt = 1;
				int halfHt = bufferHt / 2;
				int blockHt = colorcnt * barHt;
				if (blockHt < 1) blockHt = 1;
				int fOffset = (int) (workingPosition*blockHt*Repeat);// : Speed * frame / 4 % blockHt);
				if(Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
				{
					fOffset = (int)(Math.Floor(workingPosition*barCount)*barHt);
				}

				var indexAdjust = 1;
				
				for (y = 0; y < bufferHt; y++)
				{
					n = y + fOffset;
					colorIdx = ((n + indexAdjust) % blockHt) / barHt;
					//we need the integer division here to make things work
					double colorPosition = (n + indexAdjust) % barHt / (double)barHt;
					Color c = Colors[colorIdx].GetColorAt(colorPosition);
					
					if (Highlight || Show3D)
					{
						var hsv = HSV.FromRGB(c);
						if (Highlight && (n + indexAdjust) % barHt == 0) hsv.S = 0.0f;
						if (Show3D) hsv.V *= (float) (barHt - (n + indexAdjust) % barHt - 1) / barHt;
						hsv.V *= level;
						c = hsv.ToRGB();
					}
					else
					{
						if (level < 1)
						{
							HSV hsv = HSV.FromRGB(c);
							hsv.V *= level;
							c = hsv.ToRGB();
						}
					}

					switch (Direction)
					{
						case BarDirection.Down:
						case BarDirection.AlternateDown:
							// dow
							if (_negPosition)
							{
								for (x = 0; x < bufferWi; x++)
								{
									frameBuffer.SetPixel(x, bufferHt - y - 1, c);
								}
							}
							else
							{
								for (x = 0; x < bufferWi; x++)
								{
									frameBuffer.SetPixel(x, y, c);
								}
							}

							break;
						case BarDirection.Compress:
							// expand
							if (_negPosition)
							{
								if (y <= halfHt)
								{
									for (x = 0; x < bufferWi; x++)
									{
										frameBuffer.SetPixel(x, y, c);
										frameBuffer.SetPixel(x, bufferHt - y - 1, c);
									}
								}
							}
							else
							{
								if (y >= halfHt)
								{
									for (x = 0; x < bufferWi; x++)
									{
										frameBuffer.SetPixel(x, y, c);
										frameBuffer.SetPixel(x, bufferHt - y - 1, c);
									}
								}
							}
							break;
						case BarDirection.Expand:
							// compress
							if (!_negPosition)
							{
								if (y <= halfHt)
								{
									for (x = 0; x < bufferWi; x++)
									{
										frameBuffer.SetPixel(x, y, c);
										frameBuffer.SetPixel(x, bufferHt - y - 1, c);
									}
								}
							}
							else
							{
								if (y >= halfHt)
								{
									for (x = 0; x < bufferWi; x++)
									{
										frameBuffer.SetPixel(x, y, c);
										frameBuffer.SetPixel(x, bufferHt - y - 1, c);
									}
								}
							}
							break;
						default:
							// up & AlternateUp
							if (!_negPosition)
							{
								for (x = 0; x < bufferWi; x++)
								{
									frameBuffer.SetPixel(x, bufferHt - y - 1, c);
								}
							}
							else
							{
								for (x = 0; x < bufferWi; x++)
								{
									frameBuffer.SetPixel(x, y, c);
								}
							}
							break;
					}
				}
			}
			else
			{
				int barWi = bufferWi / barCount+1;
				if (barWi < 1) barWi = 1;
				int halfWi = bufferWi / 2;
				int blockWi = colorcnt * barWi;
				if (blockWi < 1) blockWi = 1;
				int fOffset = (int)(workingPosition * blockWi * Repeat);
				if (Direction > BarDirection.AlternateDown)
				{
					fOffset = (int)(Math.Floor(workingPosition * barCount) * barWi);
				} 
				
				for (x = 0; x < bufferWi; x++)
				{
					n = x + fOffset;
					//we need the integer division here to make things work
					colorIdx = ((n + 1) % blockWi) / barWi;
					var colorPosition = (n + 1) % barWi / (double)barWi;
					Color c = Colors[colorIdx].GetColorAt( colorPosition );
					
					if (Highlight || Show3D)
					{
						var hsv = HSV.FromRGB(c);
						if (Highlight && (n+1) % barWi == 0) hsv.S = 0.0f;
						if (Show3D) hsv.V *= (float)(barWi - n % barWi - 1) / barWi;
						hsv.V *= level;
						c = hsv.ToRGB();
					}
					else
					{
						if (level < 1)
						{
							HSV hsv = HSV.FromRGB(c);
							hsv.V *= level;
							c = hsv.ToRGB();
						}
					}

					switch (Direction)
					{
						case BarDirection.Right:
						case BarDirection.AlternateRight:
							// right
							for (y = 0; y < bufferHt; y++)
							{
								frameBuffer.SetPixel(_negPosition?x:bufferWi - x - 1, y, c);
							}
							break;
						case BarDirection.HExpand:
							// H-expand
							if (!_negPosition && x <= halfWi || _negPosition && x >= halfWi)
							{
								for (y = 0; y < bufferHt; y++)
								{
									frameBuffer.SetPixel(x, y, c);
									frameBuffer.SetPixel(bufferWi - x - 1, y, c);
								}
							}
							break;
						case BarDirection.HCompress:
							// H-compress
							if (!_negPosition && x >= halfWi || _negPosition && x <= halfWi)
							{
								for (y = 0; y < bufferHt; y++)
								{
									frameBuffer.SetPixel(x, y, c);
									frameBuffer.SetPixel(bufferWi - x - 1, y, c);
								}
							}
							break;
						default:
							// left & AlternateLeft
							for (y = 0; y < bufferHt; y++)
							{
								frameBuffer.SetPixel(_negPosition?bufferWi - x - 1:x, y, c);
							}
							break;
					}
				}
			}
		}

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
			int colorcnt = Colors.Count();
			int barCount = Repeat * colorcnt;
			if (barCount < 1) barCount = 1;
			
			int barHt = BufferHt / barCount + 1;
			if (barHt < 1) barHt = 1;
			int blockHt = colorcnt * barHt;
			if (blockHt < 1) blockHt = 1;

			int barWi = BufferWi / barCount + 1;
			if (barWi < 1) barWi = 1;
			int blockWi = colorcnt * barWi;
			if (blockWi < 1) blockWi = 1;

			var bufferHt = BufferHt;
			var bufferWi = BufferWi;
			var bufferHtOffset = BufferHtOffset;
			var bufferWiOffset = BufferWiOffset;

			IEnumerable<IGrouping<int, ElementLocation>> nodes;
			List<IGrouping<int, ElementLocation>> reversedNodes = new List<IGrouping<int, ElementLocation>>();
			
			switch (Direction)
			{
				case BarDirection.AlternateUp:
				case BarDirection.Up:
					nodes = frameBuffer.ElementLocations.OrderBy(x => x.Y).ThenBy(x => x.X).GroupBy(x => x.Y);
					break;
				case BarDirection.Left:
				case BarDirection.AlternateLeft:
					nodes = frameBuffer.ElementLocations.OrderByDescending(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
					break;
				case BarDirection.Right:
				case BarDirection.AlternateRight:
					nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
					break;
				case BarDirection.Compress:
				case BarDirection.Expand:
					nodes = frameBuffer.ElementLocations.OrderByDescending(x => x.Y).ThenBy(x => x.X).GroupBy(x => x.Y);
					reversedNodes = nodes.Reverse().ToList();
					break;
				case BarDirection.HCompress:
				case BarDirection.HExpand:
					nodes = frameBuffer.ElementLocations.OrderBy(x => x.X).ThenBy(x => x.Y).GroupBy(x => x.X);
					reversedNodes = nodes.Reverse().ToList();
					break;
				default:
					nodes = frameBuffer.ElementLocations.OrderByDescending(x => x.Y).ThenBy(x => x.X).GroupBy(x => x.Y);
					break;

			}
			var nodeCount = nodes.Count();
			var halfNodeCount = (nodeCount - 1) / 2;
			var evenHalfCount = nodeCount%2!=0;
			_negPosition = false;
			for (int frame = 0; frame < numFrames; frame++)
			{
				frameBuffer.CurrentFrame = frame;
				double intervalPosFactor = GetEffectTimeIntervalPosition(frame) * 100;
				double level = LevelCurve.GetValue(intervalPosFactor) / 100;
				
				if (MovementType == MovementType.Iterations)
				{
					_position = (GetEffectTimeIntervalPosition(frame) * Speed) % 1;
				}
				else
				{   
					var s = CalculateSpeed(intervalPosFactor);
					if (frame == 0) _position = s;
					_position += s / 100 * FrameTime / 50d; //Adjust the speed setting for different frame rates with FrameTime / 50d
					_negPosition = _position < 0;
				}

				
				var workingPosition = Math.Abs(_position);

				int n;
				int colorIdx;
				if (Direction < BarDirection.Left || Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
				{
					
					int fOffset = (int)(workingPosition * blockHt * Repeat);// : Speed * frame / 4 % blockHt);
					if (Direction == BarDirection.AlternateUp || Direction == BarDirection.AlternateDown)
					{
						fOffset = (int)(Math.Floor(workingPosition * barCount) * barHt);
					}
					
					int indexAdjust = 1;

					int i = 0;
					bool exitLoop = false;

					foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
					{
						int y = elementLocations.Key;

						switch (Direction)
						{
							case BarDirection.Down:
							case BarDirection.AlternateDown:
							case BarDirection.Expand:
								n = (bufferHt+bufferHtOffset) - (_negPosition?bufferHt-y-1:y) + fOffset;
								break;
							default:
								n =  (_negPosition?bufferHt-y-1:y) - bufferHtOffset + fOffset;
								break;
						}
						
						colorIdx = ((n + indexAdjust) % blockHt) / barHt;
						//we need the integer division here to make things work
						var colorPosition =(n + indexAdjust) % barHt / (double)barHt;
						Color c = Colors[colorIdx].GetColorAt(colorPosition); 
						
						if (Highlight || Show3D)
						{
							var hsv = HSV.FromRGB(c);
							if (Highlight && (n + indexAdjust) % barHt == 0 || colorPosition > .95) hsv.S = 0.0f;
							if (Show3D) hsv.V *= (float)(barHt - (n + indexAdjust) % barHt - 1) / barHt;
							hsv.V *= level;
							c = hsv.ToRGB();
						}
						else
						{
							if (level < 1)
							{
								HSV hsv = HSV.FromRGB(c);
								hsv.V *= level;
								c = hsv.ToRGB();
							}
						}

						switch (Direction)
						{
							case BarDirection.Expand:
							case BarDirection.Compress:
								// expand / compress
								if (i <= halfNodeCount)
								{
									foreach (var elementLocation in elementLocations)
									{
										frameBuffer.SetPixel(elementLocation.X, y, c);
									}
									if (i == halfNodeCount & evenHalfCount)
									{
										i++;
										continue;
									}
									foreach (var elementLocation in reversedNodes[i])
									{
										frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, c);
									}

									i++;
								}
								else
								{
									exitLoop = true;
								}
								break;
							default:
								foreach (var elementLocation in elementLocations)
								{
									frameBuffer.SetPixel(elementLocation.X, y, c);
								}
								break;
						}
						if (exitLoop) break;
					}
				}
				else
				{
					
					int fOffset = (int)(workingPosition * blockWi * Repeat);
					if (Direction > BarDirection.AlternateDown)
					{
						fOffset = (int)(Math.Floor(workingPosition * barCount) * barWi);
					}
					
					int i = 0;
					
					foreach (IGrouping<int, ElementLocation> elementLocations in nodes)
					{
						int x = elementLocations.Key;
						
						switch (Direction)
						{
							case BarDirection.Right:
							case BarDirection.AlternateRight:
								case BarDirection.HCompress:
								n = (bufferWi+bufferWiOffset) - (_negPosition?bufferWi-x-1:x) + fOffset;
								break;
							default:
								n = (_negPosition?bufferWi-x-1:x) - bufferWiOffset + fOffset;
								break;
						}
						
						//we need the integer division here to make things work
						colorIdx = (n + 1) % blockWi / barWi;
						double colorPosition = (n + 1) % barWi / (double)barWi;
						Color c = Colors[colorIdx].GetColorAt(colorPosition);
						
						if (Highlight || Show3D)
						{
							var hsv = HSV.FromRGB(c);
							if (Highlight && (n+1) % barWi == 0 || colorPosition > .95) hsv.S = 0.0f;
							if (Show3D) hsv.V *= (float)(barWi - n % barWi - 1) / barWi;
							hsv.V *= level;
							c = hsv.ToRGB();
						}
						else
						{
							if (level < 1)
							{
								HSV hsv = HSV.FromRGB(c);
								hsv.V *= level;
								c = hsv.ToRGB();
							}
						}

						switch (Direction)
						{
							case BarDirection.HExpand:
							case BarDirection.HCompress:
								if (i <= halfNodeCount)
								{
									foreach (var elementLocation in elementLocations)
									{
										frameBuffer.SetPixel(x, elementLocation.Y, c);
									}
									if (i == halfNodeCount & evenHalfCount)
									{
										i++;
										continue;
									}
									foreach (var elementLocation in reversedNodes[i])
									{
										frameBuffer.SetPixel(elementLocation.X, elementLocation.Y, c);
									}

									i++;
								}
								break;
							default:
								foreach (var elementLocation in elementLocations)
								{
									frameBuffer.SetPixel(x, elementLocation.Y, c);
								}
								break;
						}
					}

				}

			}

		}

		private double CalculateSpeed(double intervalPos)
		{
			return ScaleCurveToValue(SpeedCurve.GetValue(intervalPos), 15, -15);
		}

	}
}
