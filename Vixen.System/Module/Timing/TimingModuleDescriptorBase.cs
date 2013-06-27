using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Timing
{
	public abstract class TimingModuleDescriptorBase : ModuleDescriptorBase, ITimingModuleDescriptor,
	                                                   IEqualityComparer<ITimingModuleDescriptor>,
	                                                   IEquatable<ITimingModuleDescriptor>,
	                                                   IEqualityComparer<TimingModuleDescriptorBase>,
	                                                   IEquatable<TimingModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public bool Equals(ITimingModuleDescriptor x, ITimingModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(ITimingModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(ITimingModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(TimingModuleDescriptorBase x, TimingModuleDescriptorBase y)
		{
			return Equals(x as ITimingModuleDescriptor, y as ITimingModuleDescriptor);
		}

		public int GetHashCode(TimingModuleDescriptorBase obj)
		{
			return GetHashCode(obj as ITimingModuleDescriptor);
		}

		public bool Equals(TimingModuleDescriptorBase other)
		{
			return Equals(other as ITimingModuleDescriptor);
		}
	}
}