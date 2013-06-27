using System;
using System.Collections.Generic;

namespace Vixen.Module.OutputFilter
{
	public abstract class OutputFilterModuleDescriptorBase : ModuleDescriptorBase, IOutputFilterModuleDescriptor,
	                                                         IEqualityComparer<IOutputFilterModuleDescriptor>,
	                                                         IEquatable<IOutputFilterModuleDescriptor>,
	                                                         IEqualityComparer<OutputFilterModuleDescriptorBase>,
	                                                         IEquatable<OutputFilterModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public bool Equals(IOutputFilterModuleDescriptor x, IOutputFilterModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IOutputFilterModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IOutputFilterModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(OutputFilterModuleDescriptorBase x, OutputFilterModuleDescriptorBase y)
		{
			return Equals(x as IOutputFilterModuleDescriptor, y as IOutputFilterModuleDescriptor);
		}

		public int GetHashCode(OutputFilterModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IOutputFilterModuleDescriptor);
		}

		public bool Equals(OutputFilterModuleDescriptorBase other)
		{
			return Equals(other as IOutputFilterModuleDescriptor);
		}
	}
}