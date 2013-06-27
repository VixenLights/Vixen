using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.App
{
	public abstract class AppModuleDescriptorBase : ModuleDescriptorBase, IAppModuleDescriptor,
	                                                IEqualityComparer<IAppModuleDescriptor>,
	                                                IEquatable<IAppModuleDescriptor>,
	                                                IEqualityComparer<AppModuleDescriptorBase>,
	                                                IEquatable<AppModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		// App module types can't have instance data classes; they're singletons, so don't let anyone set one.
		public override sealed Type ModuleDataClass
		{
			get { return null; }
		}

		public bool Equals(IAppModuleDescriptor x, IAppModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(IAppModuleDescriptor obj)
		{
			return base.GetHashCode();
		}

		public bool Equals(IAppModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(AppModuleDescriptorBase x, AppModuleDescriptorBase y)
		{
			return Equals(x as IAppModuleDescriptor, y as IAppModuleDescriptor);
		}

		public int GetHashCode(AppModuleDescriptorBase obj)
		{
			return GetHashCode(obj as IAppModuleDescriptor);
		}

		public bool Equals(AppModuleDescriptorBase other)
		{
			return Equals(other as IAppModuleDescriptor);
		}
	}
}