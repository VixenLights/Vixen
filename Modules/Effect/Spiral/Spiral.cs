using System.Collections.Generic;
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

namespace VixenModules.Effect.Spiral
{
	public class Spiral:PixelEffectBase
	{
		private SpiralData _data;
		
		public Spiral()
		{
			_data = new SpiralData();
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
		public SpiralDirection Direction
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
		[ProviderDisplayName(@"Iterations")]
		[ProviderDescription(@"Iterations")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1,20,1)]
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
		[ProviderDisplayName(@"Repeat")]
		[ProviderDescription(@"Repeat")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 5, 1)]
		[PropertyOrder(2)]
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
		[ProviderDisplayName(@"Thickness")]
		[ProviderDescription(@"Thickness")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(4)]
		public int Thickness
		{
			get { return _data.Thickness; }
			set
			{
				_data.Thickness = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Rotation")]
		[ProviderDescription(@"Rotation")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-50, 50, 1)]
		[PropertyOrder(5)]
		public int Rotation
		{
			get { return _data.Rotation; }
			set
			{
				_data.Rotation = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"VerticalBlend")]
		[ProviderDescription(@"VerticalBlend")]
		[PropertyOrder(6)]
		public bool Blend
		{
			get { return _data.Blend; }
			set
			{
				_data.Blend = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Show3D")]
		[ProviderDescription(@"Show3D")]
		[PropertyOrder(7)]
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

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Grow")]
		[ProviderDescription(@"Grow")]
		[PropertyOrder(8)]
		public bool Grow
		{
			get { return _data.Grow; }
			set
			{
				_data.Grow = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Shrink")]
		[ProviderDescription(@"Shrink")]
		[PropertyOrder(9)]
		public bool Shrink
		{
			get { return _data.Shrink; }
			set
			{
				_data.Shrink = value;
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


		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as SpiralData;
				InitAllAttributes();
				IsDirty = true;
			}
		}

		private void InitAllAttributes()
		{
			UpdateStringOrientationAttributes(true);
		}


		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
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
			int colorcnt = Colors.Count();
			int spiralCount = colorcnt * Repeat;
			int deltaStrands = BufferWi / spiralCount;
			int spiralThickness = (deltaStrands * Thickness / 100)+1;
			int spiralGap = deltaStrands - spiralThickness;
			int thicknessState = 0;
			int spiralState = 0;
			double position = (GetEffectTimeIntervalPosition(frame) * Speed)%1;

			switch (Direction)
			{
				case SpiralDirection.Forward:
					spiralState = (int)(position*BufferWi*10);
					break;
				case SpiralDirection.Backwards:
					spiralState = (int)(position * BufferWi * -10);
					break;
			}

			if (Grow && Shrink)
			{
				thicknessState = (int)(position <= 0.5 ? spiralGap * (position * 2) : spiralGap * ((1 - position) * 2));
			}
			else if (Grow)
			{
				thicknessState = (int)(spiralGap * position);
			}
			else if (Shrink)
			{
				thicknessState = (int)(spiralGap * (1.0 - position));
			}

			spiralThickness += thicknessState;
			double level = LevelCurve.GetValue(GetEffectTimeIntervalPosition(frame) * 100) / 100;
			
			for (int ns = 0; ns < spiralCount; ns++)
			{
				var strandBase = ns * deltaStrands;
				int colorIdx = ns % colorcnt;
				
				int thick;
				for (thick = 0; thick < spiralThickness; thick++)
				{
					var strand = (strandBase + thick) % BufferWi;
					int y;
					for (y = 0; y < BufferHt; y++)
					{
						Color color;
						var x = (strand + (spiralState / 10) + (y * Rotation / BufferHt)) % BufferWi;
						if (x < 0) x += BufferWi;
						if (Blend)
						{
							color = Colors[colorIdx].GetColorAt((double)(BufferHt - y - 1) / BufferHt);
						}
						else
						{
							color = Colors[colorIdx].GetColorAt((double) thick/spiralThickness);
						}
						if (Show3D)
						{
							var hsv = HSV.FromRGB(color);
							
							if (Rotation < 0)
							{
								hsv.V = (float)((double)(thick + 1) / spiralThickness);
							}
							else
							{
								hsv.V = (float)((double)(spiralThickness - thick) / spiralThickness);
							}
							hsv.V = hsv.V * level;
							frameBuffer.SetPixel(x, y, hsv);
						}
						else
						{
							var hsv = HSV.FromRGB(color);
							hsv.V = hsv.V * level;
							frameBuffer.SetPixel(x, y, hsv);
						}
					}
				}
			}
		}

		
	}
}
