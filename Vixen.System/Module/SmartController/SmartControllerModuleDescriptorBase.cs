using System;
using System.Collections.Generic;

namespace Vixen.Module.SmartController
{
	public abstract class SmartControllerModuleDescriptorBase : ModuleDescriptorBase, ISmartControllerModuleDescriptor,
	                                                            IEqualityComparer<ISmartControllerModuleDescriptor>,
	                                                            IEquatable<ISmartControllerModuleDescriptor>,
	                                                            IEqualityComparer<SmartControllerModuleDescriptorBase>,
	                                                            IEquatable<SmartControllerModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public virtual int UpdateInterval
		{
			get { return Vixen.Sys.VixenSystem.DefaultUpdateInterval; }
		}

		public bool Equals(ISmartControllerModuleDescriptor x, ISmartControllerModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(ISmartControllerModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(ISmartControllerModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(SmartControllerModuleDescriptorBase x, SmartControllerModuleDescriptorBase y)
		{
			return Equals(x as ISmartControllerModuleDescriptor, y as ISmartControllerModuleDescriptor);
		}

		public int GetHashCode(SmartControllerModuleDescriptorBase obj)
		{
			return GetHashCode(obj as ISmartControllerModuleDescriptor);
		}

		public bool Equals(SmartControllerModuleDescriptorBase other)
		{
			return Equals(other is ISmartControllerModuleDescriptor);
		}
	}
}