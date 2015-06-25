using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Fire
{
	public class FireDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("7e1b81bd-1b69-4238-9f55-06462cdc374c");

		public override ParameterSignature Parameters
		{
			get { return new ParameterSignature(); }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Basic; }
		}

		public override string TypeName
		{
			get { return EffectName; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(Fire); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(FireData); }
		}

		public override string Author
		{
			get { return "Jeff Uchitjil"; }
		}

		public override string Description
		{
			get { return "Applies a Fire like effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Fire"; }
		}
	}
}
