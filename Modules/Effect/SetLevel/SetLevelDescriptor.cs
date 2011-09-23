using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using CommandStandard;
using CommandStandard.Types;
using Vixen.Module.Effect;

namespace VixenModules.Effect.SetLevel
{
	public class SetLevelModuleDescriptor : EffectModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{603E3297-994C-4705-9F17-02A62ECC14B5}");
		static internal Guid _rgbProperty = new Guid("{55960E71-2151-454c-885E-00B9713A93EF}");
		private CommandParameterSpecification[] _parameters = { new CommandParameterSpecification("Level", typeof(Level)) };

		public SetLevelModuleDescriptor()
		{
			PropertyDependencies = new[] {
				_rgbProperty
			};
		}

		override public string EffectName
		{
			get { return "Set Level"; }
		}

		override public CommandParameterSpecification[] Parameters
		{
			get { return _parameters; }
		}

		override public Guid TypeId
		{
			get { return _typeId; }
		}

		override public Type ModuleClass
		{
			get { return typeof(SetLevel); }
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
			get { return "Sets the target channels to a static output level."; }
		}

		override public string Version
		{
			get { return "0.1"; }
		}
	}
}
