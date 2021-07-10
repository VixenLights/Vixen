using Common.Controls.ColorManagement.ColorModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using VixenModules.Effect.DMXFixture;
using VixenModules.Effect.Effect;
using VixenModules.Effect.Effect.Location;
using VixenModules.EffectEditor.EffectDescriptorAttributes;
using VixenModules.Preview.VixenPreview.Fixtures.WPF;
using VixenModules.Preview.VixenPreview.Shapes;
using ZedGraph;

namespace VixenModules.Effect.Bars
{
	public class DMXFixture : PixelEffectBase
	{
		#region Private Fields

		private DMXFixtureData _data;

		#endregion

		#region Constructor

		public DMXFixture()
		{
			_data = new DMXFixtureData();
			EnableTargetPositioning(true, true);
			//InitAllAttributes();
		}

		#endregion

		#region Public (Override) Methods

		public override bool IsDirty
		{
			get
			{
				/*
				if (Colors.Any(x => !x.CheckLibraryReference()))
				{
					base.IsDirty = true;
				}
				*/

				return base.IsDirty;
			}
			protected set { base.IsDirty = value; }
		}

		#endregion

		#region Public Properties

		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as DMXFixtureData;
				//InitAllAttributes();
				IsDirty = true;
			}
		}

		#endregion

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

		#region Color properties

		[Value]
		[ProviderCategory(@"Colors", 2)]
		[ProviderDisplayName(@"Beam Color")]
		[ProviderDescription(@"Beam Color")]
		[PropertyOrder(1)]
		public ColorGradient BeamColor
		{
			get { return _data.BeamColor; }
			set
			{
				_data.BeamColor = value;
				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Colors", 2)]
		[ProviderDisplayName(@"Legend Color")]
		[ProviderDescription(@"Legend Color")]
		[PropertyOrder(1)]
		public ColorGradient LegendColor
		{
			get { return _data.LegendColor; }
			set
			{
				_data.LegendColor = value;
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

		#region Protected Properties

		protected override EffectTypeModuleData EffectModuleData
		{
			get { return _data; }
		}

		#endregion

		#region Protected Methods

		protected override void CleanUpRender()
		{
			// Nothing to clean up
		}

		#endregion

		#region Protected Methods

		protected override void RenderEffectByLocation(int numFrames, PixelLocationFrameBuffer frameBuffer)
		{
		}

		/// <summary>
		/// Perform calculations that only need to be performed once per rendering.
		/// </summary>
		protected override void SetupRender()
		{
		}

		/// <summary>
		/// Renders the effect in string mode.
		/// </summary>
		/// <param name="frame">Current frame number</param>
		/// <param name="frameBuffer">Frame buffer to render in</param>
		protected override void RenderEffect(int frame, IPixelFrameBuffer frameBuffer)
		{
			Preview.VixenPreview.Fixtures.OpenGL.MovingHeadOpenGL.MovingHead.BeamColor = BeamColor.GetColorAt(0);

			if (MovingHeadWPF.MovingHead != null)
			{
				MovingHeadWPF.MovingHead.BeamColor = BeamColor.GetColorAt(0);
			}

			Preview.VixenPreview.Fixtures.OpenGL.MovingHeadOpenGL.MovingHead.LegendColor = LegendColor.GetColorAt(0);

			if (MovingHeadWPF.MovingHead != null)
			{
				MovingHeadWPF.MovingHead.LegendColor = LegendColor.GetColorAt(0);
			}
		}

		#endregion

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Pan")]
		[ProviderDescription(@"Pan")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-180, 180, 1)]
		[PropertyOrder(1)]
		public int Pan
		{
			get { return _data.Pan; }
			set
			{
				_data.Pan = value;
				IsDirty = true;
				OnPropertyChanged();

				if (MovingHeadWPF.MovingHead != null)
				{
					MovingHeadWPF.MovingHead.PanAngle = _data.Pan;
				}

				Preview.VixenPreview.Fixtures.OpenGL.MovingHeadOpenGL.MovingHead.PanAngle = _data.Pan;
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Tilt")]
		[ProviderDescription(@"Tilt")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(-180, 180, 1)]
		[PropertyOrder(2)]
		public int Tilt
		{
			get { return _data.Tilt; }
			set
			{
				_data.Tilt = value;
				IsDirty = true;
				OnPropertyChanged();

				if (MovingHeadWPF.MovingHead != null)
				{
					MovingHeadWPF.MovingHead.TiltAngle = _data.Tilt;
				}

				Preview.VixenPreview.Fixtures.OpenGL.MovingHeadOpenGL.MovingHead.TiltAngle = _data.Tilt;
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Shutter (On/Off)")]
		[ProviderDescription(@"Shutter (On/Off)")]
		[PropertyOrder(3)]
		public bool Highlight
		{
			get { return _data.OnOff; }
			set
			{
				_data.OnOff = value;
				Preview.VixenPreview.Fixtures.OpenGL.MovingHeadOpenGL.MovingHead.OnOff = value;

				if (MovingHeadWPF.MovingHead != null)
				{
					MovingHeadWPF.MovingHead.OnOff = value;
				}

				IsDirty = true;
				OnPropertyChanged();
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Beam Length")]
		[ProviderDescription(@"Beam Length")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(4)]
		public int BeamLength
		{
			get { return _data.BeamLength; }
			set
			{
				_data.BeamLength = value;
				IsDirty = true;
				OnPropertyChanged();

				if (MovingHeadWPF.MovingHead != null)
				{
					MovingHeadWPF.MovingHead.BeamLength = _data.BeamLength;
				}

				Preview.VixenPreview.Fixtures.OpenGL.MovingHeadOpenGL.MovingHead.BeamLength = _data.BeamLength;
			}
		}


		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Beam Focus")]
		[ProviderDescription(@"Beam Focus")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(5)]
		public int Focus
		{
			get { return _data.Focus; }
			set
			{
				_data.Focus = value;
				IsDirty = true;
				OnPropertyChanged();

				if (MovingHeadWPF.MovingHead != null)
				{
					MovingHeadWPF.MovingHead.Focus = _data.Focus;
				}

				VixenModules.Preview.VixenPreview.Fixtures.OpenGL.MovingHeadOpenGL.MovingHead.Focus = _data.Focus;
			}
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Beam Intensity")]
		[ProviderDescription(@"Beam Intensity")]
		[PropertyEditor("SliderEditor")]
		[NumberRange(1, 100, 1)]
		[PropertyOrder(6)]
		public int Intensity
		{
			get { return _data.Intensity; }
			set
			{
				_data.Intensity = value;
				IsDirty = true;
				OnPropertyChanged();

				if (MovingHeadWPF.MovingHead != null)
				{
					MovingHeadWPF.MovingHead.Intensity = _data.Intensity;
				}

				Preview.VixenPreview.Fixtures.OpenGL.MovingHeadOpenGL.MovingHead.Intensity = _data.Intensity;
			}
		}


		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Include Legend")]
		[ProviderDescription(@"Include Legend")]
		[PropertyOrder(7)]
		public bool IncludeLegend
		{
			get { return _data.IncludeLegend; }
			set
			{
				_data.IncludeLegend = value;

				Preview.VixenPreview.Fixtures.OpenGL.MovingHeadOpenGL.MovingHead.IncludeLegend = value;

				if (MovingHeadWPF.MovingHead != null)
				{
					MovingHeadWPF.MovingHead.IncludeLegend = value;
				}
			}
		}
					
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Prism")]
		[ProviderDescription(@"Prism")]
		[PropertyOrder(8)]
		public PrismType Prism
		{
			get { return _data.Prism; }
			set
			{
				_data.Prism = value;
				UpdateLegend();
			}
		}

		private string GetLegend()
		{
			string legend = string.Empty;

			string gobo = string.Empty;

			switch (_data.Gobo)
			{
				case Gobos.Open:
					gobo = "0";
					break;
				case Gobos.Gobo1:
					gobo = "1";
					break;
				case Gobos.Gobo2:
					gobo = "2";
					break;
				case Gobos.Gobo3:
					gobo = "3";
					break;
				case Gobos.Gobo4:
					gobo = "4";
					break;
				case Gobos.Gobo5:
					gobo = "5";
					break;
				case Gobos.Gobo6:
					gobo = "6";
					break;
				case Gobos.Gobo7:
					gobo = "7";
					break;
				case Gobos.Gobo8:
					gobo = "8";
					break;
				case Gobos.Gobo9:
					gobo = "9";
					break;
				case Gobos.Gobo10:
					gobo = "10";
					break;
				case Gobos.Gobo11:
					gobo = "11";
					break;
				case Gobos.Gobo12:
					gobo = "12";
					break;
				case Gobos.Gobo13:
					gobo = "13";
					break;
				case Gobos.Gobo14:
					gobo = "14";
					break;
			}

			string prism = "O";

			if (_data.Prism == PrismType.Close)
			{
				prism = "C";
			}

			legend = "G:" + gobo + " P:" + prism;

			return legend;
		}

		private void UpdateLegend()
		{
			if (MovingHeadWPF.MovingHead != null)
			{
				MovingHeadWPF.MovingHead.Legend = GetLegend();
			}

			Preview.VixenPreview.Fixtures.OpenGL.MovingHeadOpenGL.MovingHead.Legend = GetLegend();
		}

		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Gobo")]
		[ProviderDescription(@"Gobo")]
		[PropertyOrder(9)]
		public Gobos BarType
		{
			get { return _data.Gobo; }
			set
			{
				_data.Gobo = value;

				UpdateLegend();
			}
		}
	
		[Value]
		[ProviderCategory(@"Config", 1)]
		[ProviderDisplayName(@"Enable GDI")]
		[ProviderDescription(@"Enable GDI")]
		[PropertyOrder(10)]
		public bool EnableGDI
		{
			get { return _data.EnableGDI; }
			set
			{
				_data.EnableGDI = value;

				Preview.VixenPreview.Fixtures.OpenGL.MovingHeadOpenGL.MovingHead.EnableGDI = value;
				MovingHeadWPF.MovingHead.EnableGDI = value;				
			}
		}		
	}
}