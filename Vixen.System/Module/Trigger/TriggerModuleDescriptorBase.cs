using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Trigger
{
	public abstract class TriggerModuleDescriptorBase : ModuleDescriptorBase, ITriggerModuleDescriptor,
	                                                    IEqualityComparer<ITriggerModuleDescriptor>,
	                                                    IEquatable<ITriggerModuleDescriptor>,
	                                                    IEqualityComparer<TriggerModuleDescriptorBase>,
	                                                    IEquatable<TriggerModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public bool Equals(ITriggerModuleDescriptor x, ITriggerModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(ITriggerModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(ITriggerModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(TriggerModuleDescriptorBase x, TriggerModuleDescriptorBase y)
		{
			return Equals(x as ITriggerModuleDescriptor, y as ITriggerModuleDescriptor);
		}

		public int GetHashCode(TriggerModuleDescriptorBase obj)
		{
			return GetHashCode(obj as ITriggerModuleDescriptor);
		}

		public bool Equals(TriggerModuleDescriptorBase other)
		{
			return Equals(other as ITriggerModuleDescriptor);
		}
	}
}