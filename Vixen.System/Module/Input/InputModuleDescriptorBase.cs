using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Input
{
	public abstract class InputModuleDescriptorBase : ModuleDescriptorBase, IInputModuleDescriptor,
	                                                  IEqualityComparer<IInputModuleDescriptor>,
	                                                  IEquatable<IInputModuleDescriptor>,
	                                                  IEqualityComparer<InputModuleDescriptorBase>,
	                                                  IEquatable<InputModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public bool Equals(IInputModuleDescriptor x, IInputModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IInputModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IInputModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(InputModuleDescriptorBase x, InputModuleDescriptorBase y)
		{
			return Equals(x as IInputModuleDescriptor, y as IInputModuleDescriptor);
		}

		public int GetHashCode(InputModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IInputModuleDescriptor);
		}

		public bool Equals(InputModuleDescriptorBase other)
		{
			return Equals(other as IInputModuleDescriptor);
		}
	}
}