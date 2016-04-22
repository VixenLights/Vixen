using System;
using Vixen.Module.Effect;
using Vixen.Sys;

namespace VixenModules.Effect.Tree
{
	public class TreeDescriptor : EffectModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("0140d215-0f5e-48b1-af2a-5fa87b685293");

		public override ParameterSignature Parameters
		{
			get { return new ParameterSignature(); }
		}

		public override EffectGroups EffectGroup
		{
			get { return EffectGroups.Pixel; }
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
			get { return typeof(Tree); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(TreeData); }
		}

		public override string Author
		{
			get { return "Geoff Armstrong"; }
		}

		public override string Description
		{
			get { return "Applies a Tree like effect to pixel elments"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string EffectName
		{
			get { return "Tree"; }
		}
	}
}
