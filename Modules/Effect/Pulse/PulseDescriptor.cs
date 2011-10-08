using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using Vixen.Module.Effect;
using VixenModules.App.ColorGradients;
using VixenModules.App.Curves;

namespace VixenModules.Effect.Pulse
{
	public class PulseDescriptor : EffectModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{cbd76d3b-c924-40ff-bad6-d1437b3dbdc0}");
		static internal Guid _rgbProperty = new Guid("{55960E71-2151-454c-885E-00B9713A93EF}");
		private CommandParameterSignature _parameters = new CommandParameterSignature(
			new CommandParameterSpecification("Curve", typeof(Curve)),
			new CommandParameterSpecification("ColorGradient", typeof(ColorGradient))
			);

		public PulseDescriptor()
		{
			PropertyDependencies = new[] {
				_rgbProperty
			};
		}

		override public string EffectName
		{
			get { return "Pulse"; }
		}

		override public CommandParameterSignature Parameters
		{
			get { return _parameters; }
		}

		override public Guid TypeId
		{
			get { return _typeId; }
		}

		override public Type ModuleClass
		{
			get { return typeof(Pulse); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(PulseData); }
		}

		override public string Author
		{
			get { return "Vixen Team"; }
		}

		override public string TypeName
		{
			get { return EffectName; }
		}

		override public string Description
		{
			get { return "Applies a pulse with a variable level and/or color to the target channels."; }
		}

		override public string Version
		{
			get { return "0.1"; }
		}
	}
}
