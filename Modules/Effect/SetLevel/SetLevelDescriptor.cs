using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Commands;
using Vixen.Commands.KnownDataTypes;
using Vixen.Module.Effect;

namespace VixenModules.Effect.SetLevel
{
	public class SetLevelDescriptor : EffectModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{32cff8e0-5b10-4466-a093-0d232c55aac0}");
		static internal Guid RGBProperty = new Guid("{5c31be79-a6a7-4864-a660-4e0215ad4778}");
		private CommandParameterSignature _parameters = new CommandParameterSignature(new CommandParameterSpecification("Level", typeof(Level)));

		public SetLevelDescriptor()
		{
			PropertyDependencies = new[] {
				RGBProperty
			};
		}

		override public string EffectName
		{
			get { return "Set Level"; }
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
			get { return typeof(SetLevel); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(SetLevelData); }
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
