using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Garlands
{
	public class Garlands : PixelEffectBase
	{
		private GarlandsData _data;
		private int _speed;
		private int _frames;

		public Garlands()
		{
			_data = new GarlandsData();
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
		[ProviderDisplayName(@"Movement Type")]
		[ProviderDescription(@"Switches between speed or iterations")]
		[PropertyOrder(0)]
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
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Changes the direction that the garlands stack.")]
		[PropertyOrder(1)]
		public GarlandsDirection Direction
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
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
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
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(3)]
		public int Iterations
		{
			get { return _data.Iterations; }
			set
			{
				_data.Iterations = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Garland Type")]
		[ProviderDescription(@"Changes the garland type")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(0, 4, 1)]
		[PropertyOrder(4)]
		public int Type
		{
			get { return _data.Type; }
			set
			{
				_data.Type = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Spacing")]
		[ProviderDescription(@"Adjusts the space between garlands.")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(5)]
		public int Spacing
		{
			get { return _data.Spacing; }
			set
			{
				_data.Spacing = value;
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

		private void UpdateAttributes()
		{
			UpdateMovementTypeAttribute(false);
			TypeDescriptor.Refresh(this);
		}

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as GarlandsData;
				UpdateAttributes();
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		private void UpdateMovementTypeAttribute(bool refresh = true)
		{
			bool movementType = MovementType == MovementType.Speed;
			Dictionary<string, bool> propertyStates = new Dictionary<string, bool>(1);
			propertyStates.Add("Speed", movementType);
			propertyStates.Add("Iterations", !movementType);
			SetBrowsable(propertyStates);
			if (refresh)
			{
				TypeDescriptor.Refresh(this);
			}
		}
		protected override void SetupRender()
		{
			_frames = 0;
			_speed = 0;
		}

		protected override void CleanUpRender()
		{
			//Nothing to clean up
		}

		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
			int width, height;
			double totalFrames = (int)(TimeSpan.TotalMilliseconds / FrameTime) -1;
			
			int pixelSpacing = Spacing * BufferHt / 100 + 3;
			if (Direction == GarlandsDirection.Up || Direction == GarlandsDirection.Down)
			{
				width = BufferWi;
				height = BufferHt;
			}
			else
			{
				width = BufferHt;
				height = BufferWi;
			}
			int limit = height * pixelSpacing * 4;
			int garlandsState;
			if (MovementType == MovementType.Iterations)
			{
				garlandsState = (int) (limit - ((_frames)*(limit/totalFrames*Iterations)))/4; //Iterations
				if (garlandsState <= 0)
					_frames = 0;
			}
			else
			{
				_speed += Speed;
				garlandsState = (limit - _speed % limit) / 4; //Speed
			}
			_frames++;

			for (int ring = 0; ring < height; ring++)
			{
				var ratio = ring / (double)height;
				var color = GetMultiColorBlend(ratio, false, frame);
				var hsv = HSV.FromRGB(color);
				hsv.V = hsv.V*level;
				
				var y = garlandsState - ring * pixelSpacing;
				var ylimit = height - ring - 1;
				for (int x = 0; x < width; x++)
				{
					var yadj = y;
					switch (Type)
					{
						case 1:
							switch (x % 5)
							{
								case 2:
									yadj -= 2;
									break;
								case 1:
								case 3:
									yadj -= 1;
									break;
							}
							break;
						case 2:
							switch (x % 5)
							{
								case 2:
									yadj -= 4;
									break;
								case 1:
								case 3:
									yadj -= 2;
									break;
							}
							break;
						case 3:
							switch (x % 6)
							{
								case 3:
									yadj -= 6;
									break;
								case 2:
								case 4:
									yadj -= 4;
									break;
								case 1:
								case 5:
									yadj -= 2;
									break;
							}
							break;
						case 4:
							switch (x % 5)
							{
								case 1:
								case 3:
									yadj -= 2;
									break;
							}
							break;
					}
					switch (Direction)
					{
						case GarlandsDirection.Down:
							if (yadj < ylimit) yadj = ylimit;
							if (yadj < BufferHt) frameBuffer.SetPixel(x, BufferHt - 1 - yadj, hsv);
						break;
						case GarlandsDirection.Up:
							if (yadj < ylimit) yadj = ylimit;
							if (yadj < BufferHt) frameBuffer.SetPixel(x, yadj, hsv);
						break;
						case GarlandsDirection.Left:
							if (yadj < ylimit) yadj = ylimit;
							if (yadj < BufferWi) frameBuffer.SetPixel(BufferWi - 1 - yadj, x, hsv);
						break;
						case GarlandsDirection.Right:
							if (yadj < ylimit) yadj = ylimit;
							if (yadj < BufferWi) frameBuffer.SetPixel(yadj, x, hsv);
						break;
					}
				}
			}
		}

		// return a value between c1 and c2
		private int ChannelBlend(int c1, int c2, double ratio)
		{
			return c1 + (int)Math.Floor(ratio * (double)(c2 - c1) + 0.5);
		}

		public int GetColorCount()
		{
			return Colors.Count();
		}

		public Color Get2ColorBlend(int coloridx1, int coloridx2, double ratio, int frame)
		{
			Color c1, c2;
			c1 = Colors[coloridx1].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100);
			c2 = Colors[coloridx2].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100);

			return Color.FromArgb(ChannelBlend(c1.R, c2.R, ratio), ChannelBlend(c1.G, c2.G, ratio),
								  ChannelBlend(c1.B, c2.B, ratio));
			;
		}

		public Color GetMultiColorBlend(double n, bool circular, int frame)
		{
			int colorcnt = GetColorCount();
			if (colorcnt <= 1)
			{
				return Colors[0].GetColorAt((GetEffectTimeIntervalPosition(frame) * 100) / 100);
			}
			if (n >= 1.0) n = 0.99999;
			if (n < 0.0) n = 0.0;
			double realidx = circular ? n * colorcnt : n * (colorcnt - 1);
			int coloridx1 = (int)Math.Floor(realidx);
			int coloridx2 = (coloridx1 + 1) % colorcnt;
			double ratio = realidx - (double)coloridx1;
			return Get2ColorBlend(coloridx1, coloridx2, ratio, frame);
		}
	}
}
