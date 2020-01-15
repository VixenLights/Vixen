using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace Launcher
{
	public class  Descriptor : EffectModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{635364FD-B942-47E0-8B82-55D5E77A27C8}");

		public override string EffectName
		{
			get { return "Launcher"; }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Device; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(Module); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(Data); }
		}

		public override string Author
		{
			get { return "Darren McDaniel"; }
		}

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override string Description
		{
			get { return "Launch External Commands"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Guid[] Dependencies
		{
			get { return new Guid[] { }; }
		}



		public override Vixen.Sys.ParameterSignature Parameters
		{
			get
			{
				return new ParameterSignature(
					new ParameterSpecification("Description", typeof(string)),
					new ParameterSpecification("Executable", typeof(string)),
						new ParameterSpecification("Arguments", typeof(string))
						);
			}
		}
	}

}
