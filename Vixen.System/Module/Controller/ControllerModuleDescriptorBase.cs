using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Controller
{
	public abstract class ControllerModuleDescriptorBase : ModuleDescriptorBase, IControllerModuleDescriptor,
	                                                       IEqualityComparer<IControllerModuleDescriptor>,
	                                                       IEquatable<IControllerModuleDescriptor>,
	                                                       IEqualityComparer<ControllerModuleDescriptorBase>,
	                                                       IEquatable<ControllerModuleDescriptorBase>
	{
		private const int DEFAULT_UPDATE_INTERVAL = 50;

		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public virtual int UpdateInterval
		{
			get { return DEFAULT_UPDATE_INTERVAL; }
		}

		public bool Equals(IControllerModuleDescriptor x, IControllerModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IControllerModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IControllerModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(ControllerModuleDescriptorBase x, ControllerModuleDescriptorBase y)
		{
			return Equals(x as IControllerModuleDescriptor, y as IControllerModuleDescriptor);
		}

		public int GetHashCode(ControllerModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IControllerModuleDescriptor);
		}

		public bool Equals(ControllerModuleDescriptorBase other)
		{
			return Equals(other is IControllerModuleDescriptor);
		}
	}
}