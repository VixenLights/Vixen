using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.RuntimeBehavior
{
	public abstract class RuntimeBehaviorModuleDescriptorBase : ModuleDescriptorBase, IRuntimeBehaviorModuleDescriptor,
	                                                            IEqualityComparer<IRuntimeBehaviorModuleDescriptor>,
	                                                            IEquatable<IRuntimeBehaviorModuleDescriptor>,
	                                                            IEqualityComparer<RuntimeBehaviorModuleDescriptorBase>,
	                                                            IEquatable<RuntimeBehaviorModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public bool Equals(IRuntimeBehaviorModuleDescriptor x, IRuntimeBehaviorModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IRuntimeBehaviorModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IRuntimeBehaviorModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(RuntimeBehaviorModuleDescriptorBase x, RuntimeBehaviorModuleDescriptorBase y)
		{
			return Equals(x as IRuntimeBehaviorModuleDescriptor, y as IRuntimeBehaviorModuleDescriptor);
		}

		public int GetHashCode(RuntimeBehaviorModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IRuntimeBehaviorModuleDescriptor);
		}

		public bool Equals(RuntimeBehaviorModuleDescriptorBase other)
		{
			return Equals(other as IRuntimeBehaviorModuleDescriptor);
		}
	}
}