using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.SequenceFilter
{
	public abstract class SequenceFilterModuleDescriptorBase : ModuleDescriptorBase, ISequenceFilterModuleDescriptor,
	                                                           IEqualityComparer<ISequenceFilterModuleDescriptor>,
	                                                           IEquatable<ISequenceFilterModuleDescriptor>,
	                                                           IEqualityComparer<SequenceFilterModuleDescriptorBase>,
	                                                           IEquatable<SequenceFilterModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public bool Equals(ISequenceFilterModuleDescriptor x, ISequenceFilterModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(ISequenceFilterModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(ISequenceFilterModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(SequenceFilterModuleDescriptorBase x, SequenceFilterModuleDescriptorBase y)
		{
			return Equals(x as ISequenceFilterModuleDescriptor, y as ISequenceFilterModuleDescriptor);
		}

		public int GetHashCode(SequenceFilterModuleDescriptorBase obj)
		{
			return GetHashCode(obj as ISequenceFilterModuleDescriptor);
		}

		public bool Equals(SequenceFilterModuleDescriptorBase other)
		{
			return Equals(other as ISequenceFilterModuleDescriptor);
		}
	}
}