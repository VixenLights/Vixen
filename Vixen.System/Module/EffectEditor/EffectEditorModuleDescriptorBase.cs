using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;

namespace Vixen.Module.EffectEditor
{
	public abstract class EffectEditorModuleDescriptorBase : ModuleDescriptorBase, IEffectEditorModuleDescriptor,
	                                                         IEqualityComparer<IEffectEditorModuleDescriptor>,
	                                                         IEquatable<IEffectEditorModuleDescriptor>,
	                                                         IEqualityComparer<EffectEditorModuleDescriptorBase>,
	                                                         IEquatable<EffectEditorModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public abstract Guid EffectTypeId { get; }

		public abstract Type[] ParameterSignature { get; }

		public bool Equals(IEffectEditorModuleDescriptor x, IEffectEditorModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IEffectEditorModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IEffectEditorModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(EffectEditorModuleDescriptorBase x, EffectEditorModuleDescriptorBase y)
		{
			return Equals(x as IEffectEditorModuleDescriptor, y as IEffectEditorModuleDescriptor);
		}

		public int GetHashCode(EffectEditorModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IEffectEditorModuleDescriptor);
		}

		public bool Equals(EffectEditorModuleDescriptorBase other)
		{
			return Equals(other as IEffectEditorModuleDescriptor);
		}
	}
}