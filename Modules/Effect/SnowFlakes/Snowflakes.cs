using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Common.Controls.ColorManagement.ColorModels;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.Curves;
using VixenModules.Effect.Effect;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Snowflakes
{
	public class Snowflakes:PixelEffectBase
	{
		private SnowflakesData _data;
		private PixelFrameBuffer tempBuffer;
		private readonly Random _Random = new Random();
		
		public Snowflakes()
		{
			_data = new SnowflakesData();
		}

		[Browsable(false)]
		public override StringOrientation StringOrientation
		{
			get { return StringOrientation.Vertical; }
			set
			{
			}
		}

		#region Config properties

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"SnowflakeType")]
		[ProviderDescription(@"SnowflakeType")]
		[PropertyOrder(0)]
		public SnowflakeType SnowflakeType
		{
			get { return _data.SnowflakeType; }
			set
			{
				_data.SnowflakeType = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Speed")]
		[ProviderDescription(@"Speed")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 20, 1)]
		[PropertyOrder(1)]
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
		[ProviderDisplayName(@"FlakeCount")]
		[ProviderDescription(@"FlakeCount")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 40, 1)]
		[PropertyOrder(2)]
		public int FlakeCount
		{
			get { return _data.FlakeCount; }
			set
			{
				_data.FlakeCount = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		#endregion

		#region Color properties

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"CenterColor")]
		[ProviderDescription(@"Color")]
		[Browsable(false)]
		[PropertyOrder(1)]
		public Color CenterColor
		{
			get { return _data.CenterColor; }
			set
			{
				_data.CenterColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"CenterColor")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(2)]
		public List<Color> InnerColor
		{
			get { return _data.InnerColor; }
			set
			{
				_data.InnerColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"OuterColor")]
		[ProviderDescription(@"Color")]
		[Browsable(false)]
		[PropertyOrder(3)]
		public Color OuterColor
		{
			get { return _data.OuterColor; }
			set
			{
				_data.OuterColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Color", 2)]
		[ProviderDisplayName(@"OuterColor")]
		[ProviderDescription(@"Color")]
		[PropertyOrder(4)]
		public List<Color> OutSideColor
		{
			get { return _data.OutSideColor; }
			set
			{
				_data.OutSideColor = value;
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

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as SnowflakesData;
				IsDirty = true;
			}
		}

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		protected override void SetupRender()
		{
			if (CenterColor != Color.Empty)
			{
				//Converts the Old Effect Color already on the timeline to the new List Colors.
				InnerColor = new List<Color> { CenterColor };
				OutSideColor = new List<Color> { OuterColor };
				CenterColor = Color.Empty;
				OuterColor = Color.Empty;
			}
			if (BufferHt <= 1) return; //Invalid configuration
			int x = 0;
			int y = 0;

			// initialize
			tempBuffer = new PixelFrameBuffer(BufferWi, BufferHt);

			// place Count snowflakes
			for (int n = 0; n < FlakeCount; n++)
			{
				var deltaY = BufferHt / 4;
				var y0 = (n % 4) * deltaY;
				if (y0 + deltaY > BufferHt) deltaY = BufferHt - y0;
				// find unused space
				int check;
				for (check = 0; check < 20; check++)
				{
					x = Rand() % BufferWi;
					y = y0 + (Rand() % deltaY);
					if (tempBuffer.GetColorAt(x, y) == Color.Black) break;
				}
				Color outerColor = OutSideColor[_Random.Next(0, OutSideColor.Count)];
				Color innerColor = InnerColor[_Random.Next(0, InnerColor.Count)];

				SnowflakeType type = SnowflakeType == SnowflakeType.Random ? RandomFlakeType<SnowflakeType>() : SnowflakeType;
				// draw flake
				switch (type)
				{
					case SnowflakeType.Single:
						// single node
						tempBuffer.SetPixel(x, y, innerColor);
						break;
					case SnowflakeType.Five:
						// 5 nodes
						if (x < 1) x += 1;
						if (y < 1) y += 1;
						if (x > BufferWi - 2) x -= 1;
						if (y > BufferHt - 2) y -= 1;
						tempBuffer.SetPixel(x, y, innerColor);
						tempBuffer.SetPixel(x - 1, y, outerColor);
						tempBuffer.SetPixel(x + 1, y, outerColor);
						tempBuffer.SetPixel(x, y - 1, outerColor);
						tempBuffer.SetPixel(x, y + 1, outerColor);
						break;
					case SnowflakeType.Three:
						// 3 nodes
						if (x < 1) x += 1;
						if (y < 1) y += 1;
						if (x > BufferWi - 2) x -= 1;
						if (y > BufferHt - 2) y -= 1;
						tempBuffer.SetPixel(x, y, innerColor);
						if (Rand() % 100 > 50) // % 2 was not so Random
						{
							tempBuffer.SetPixel(x - 1, y, outerColor);
							tempBuffer.SetPixel(x + 1, y, outerColor);
						}
						else
						{
							tempBuffer.SetPixel(x, y - 1, outerColor);
							tempBuffer.SetPixel(x, y + 1, outerColor);
						}
						break;
					case SnowflakeType.Nine:
						// 9 nodes
						if (x < 2) x +=2;
						if (y < 2) y += 2;
						if (x > BufferWi - 3) x -= 2;
						if (y > BufferHt - 3) y -= 2;
						tempBuffer.SetPixel(x, y, innerColor);
						int i;
						for (i = 1; i <= 2; i++)
						{
							tempBuffer.SetPixel(x - i, y, outerColor);
							tempBuffer.SetPixel(x + i, y, outerColor);
							tempBuffer.SetPixel(x, y - i, outerColor);
							tempBuffer.SetPixel(x, y + i, outerColor);
						}
						break;
					case SnowflakeType.Thirteen:
						// 13 nodes
						if (x < 2) x += 2;
						if (y < 2) y += 2;
						if (x > BufferWi - 3) x -= 2;
						if (y > BufferHt - 3) y -= 2;
						tempBuffer.SetPixel(x, y, innerColor);
						tempBuffer.SetPixel(x - 1, y, outerColor);
						tempBuffer.SetPixel(x + 1, y, outerColor);
						tempBuffer.SetPixel(x, y - 1, outerColor);
						tempBuffer.SetPixel(x, y + 1, outerColor);

						tempBuffer.SetPixel(x - 1, y + 2, outerColor);
						tempBuffer.SetPixel(x + 1, y + 2, outerColor);
						tempBuffer.SetPixel(x - 1, y - 2, outerColor);
						tempBuffer.SetPixel(x + 1, y - 2, outerColor);
						tempBuffer.SetPixel(x + 2, y - 1, outerColor);
						tempBuffer.SetPixel(x + 2, y + 1, outerColor);
						tempBuffer.SetPixel(x - 2, y - 1, outerColor);
						tempBuffer.SetPixel(x - 2, y + 1, outerColor);
						break;
					case SnowflakeType.FortyFive:
						// 45 nodes
						if (x < 4) x += 4;
						if (y < 4) y += 4;
						if (x > BufferWi - 5) x -= 4;
						if (y > BufferHt - 5) y -= 4;

						int ii = 4;
						for (int j = -4; j < 5; j++)
						{
							tempBuffer.SetPixel(x + j, y + ii, outerColor);
							ii --;
						}
						for (int j = -4; j < 5; j++)
						{
							tempBuffer.SetPixel(x + j, y + j, outerColor);
						}
						tempBuffer.SetPixel(x - 2, y + 3, outerColor);
						tempBuffer.SetPixel(x - 3, y + 2, outerColor);
						tempBuffer.SetPixel(x - 3, y - 2, outerColor);
						tempBuffer.SetPixel(x - 2, y - 3, outerColor);
						tempBuffer.SetPixel(x + 2, y + 3, outerColor);
						tempBuffer.SetPixel(x + 2, y - 3, outerColor);
						tempBuffer.SetPixel(x + 3, y + 2, outerColor);
						tempBuffer.SetPixel(x + 3, y - 2, outerColor);
						for (int j = -5; j < 6; j++)
						{
							tempBuffer.SetPixel(x, y + j, outerColor);
						}for (int j = -5; j < 6; j++)
						{
							tempBuffer.SetPixel(x + j, y, outerColor);
						}
						tempBuffer.SetPixel(x, y, innerColor);
						break;
				}
			}
		}

		protected override void CleanUpRender()
		{
			tempBuffer = null;
		}

		protected override void RenderEffect(int frame, ref PixelFrameBuffer frameBuffer)
		{
			if (BufferHt <= 1) return; //Invalid configuration
			int stateInt = Speed*frame;
			double position = GetEffectTimeIntervalPosition(frame);
			double level = LevelCurve.GetValue(position * 100) / 100;
			// move snowflakes
			for (int x = 0; x < BufferWi; x++)
			{
				var newX = (x + stateInt / 20) % BufferWi;
				var newX2 = (x - stateInt / 20) % BufferWi;
				if (newX2 < 0) newX2 += BufferWi;
				for (int y = 0; y < BufferHt; y++)
				{
					var newY = (y + stateInt / 10) % BufferHt;
					var newY2 = (newY + BufferHt / 2) % BufferHt;
					var color1 = tempBuffer.GetColorAt(newX, newY);
					if (color1 == Color.Empty)
					{
						color1 = tempBuffer.GetColorAt(newX2, newY2);
					}
					var hsv = HSV.FromRGB(color1);
					hsv.V = hsv.V * level;
					frameBuffer.SetPixel(x, y, hsv);
				}
			}
		}

		private int Rand()
		{
			return _Random.Next();
		}

		private int Rand(int seed)
		{
			return _Random.Next(seed);
		}

		private T RandomFlakeType<T>()
		{
			Array values = Enum.GetValues(typeof(T));
			T randomEnum = (T)values.GetValue(Rand(values.Length));
			return randomEnum;
		}
	}
}
