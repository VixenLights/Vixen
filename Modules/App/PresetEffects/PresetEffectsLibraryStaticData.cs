using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Module;
using System.Runtime.Serialization;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;
using Common.ValueTypes;
using VixenModules.Effect.Spin;
using VixenModules.Effect.Nutcracker;
using VixenModules.Effect.CustomValue;
using VixenModules.Effect.Wipe;
using VixenModules.Effect.Twinkle;
using VixenModules.Effect.Chase;

namespace VixenModules.App.PresetEffects
{

	[DataContract]
	[KnownType(typeof(Color))]
	[KnownType(typeof(ColorGradient))]
	[KnownType(typeof(Curve))]
	[KnownType(typeof(CustomValueType))]
	[KnownType(typeof(NutcrackerData))]
	[KnownType(typeof(PresetEffect))]
	[KnownType(typeof(Percentage))]
	[KnownType(typeof(SpinSpeedFormat))]
	[KnownType(typeof(SpinSpeedFormat))]
	[KnownType(typeof(SpinColorHandling))]
	[KnownType(typeof(SpinPulseLengthFormat))]
	[KnownType(typeof(WipeDirection))]
	[KnownType(typeof(TwinkleColorHandling))]
	[KnownType(typeof(ChaseColorHandling))]

	internal class PresetEffectsLibraryStaticData : ModuleDataModelBase
	{
		public PresetEffectsLibraryStaticData()
		{
			Library = new Dictionary<Guid, PresetEffect>();
		}

		[DataMember]
		private Dictionary<Guid, PresetEffect> _library;

		public Dictionary<Guid, PresetEffect> Library
		{
			get
			{
				if (_library == null)
					_library = new Dictionary<Guid, PresetEffect>();

				return _library;
			}
			set { _library = value; }
		}

		public override IModuleDataModel Clone()
		{
			PresetEffectsLibraryStaticData result = new PresetEffectsLibraryStaticData();
			result.Library = new Dictionary<Guid, PresetEffect>(Library);
			return result;
		}
	}
}
