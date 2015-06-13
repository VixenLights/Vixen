using System.Collections.Generic;
using System.Threading;
using NLog;
using Vixen.Attributes;
using Vixen.Module;
using Vixen.Module.Effect;
using Vixen.Sys;
using Vixen.Sys.Attribute;
using VixenModules.App.ColorGradients;
using VixenModules.EffectEditor.EffectDescriptorAttributes;

namespace VixenModules.Effect.Bars
{
	public class Bars:PixelEffectModuleBase
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private BarsData _data;
		private EffectIntents _elementData;

		public Bars()
		{
			_data = new BarsData();
		}

		protected override void _PreRender(CancellationTokenSource tokenSource = null)
		{
			_elementData = new EffectIntents();

			//DoRendering(tokenSource);
		}

		protected override EffectIntents _Render()
		{
			return _elementData;
		}

		[Value]
		[ProviderCategory(@"Direction", 0)]
		[ProviderDisplayName(@"Direction")]
		[ProviderDescription(@"Direction")]
		//[PropertyOrder(1)]
		public BarDirection Direction
		{
			get { return _data.Direction; }
			set
			{
				_data.Direction = value;
				IsDirty = true;
				OnPropertyChanged();
				//UpdateColorHandlingAttributes();
				//TypeDescriptor.Refresh(this);
			}
		}

		[Value]
		[ProviderCategory(@"Color", 0)]
		[ProviderDisplayName(@"Color")]
		[ProviderDescription(@"Color")]
		//[PropertyOrder(1)]
		public List<ColorGradient> Colors
		{
			get { return _data.Colors; }
			set
			{
				_data.Colors = value;
				IsDirty = true;
				OnPropertyChanged();
				//UpdateColorHandlingAttributes();
				//TypeDescriptor.Refresh(this);
			}
		}


		public override IModuleDataModel ModuleData
		{
			get { return _data; }
			set
			{
				_data = value as BarsData;
				IsDirty = true;
				//InitAllAttributes();
			}
		}
	}
}
