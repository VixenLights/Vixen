using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.SequenceType
{
	public abstract class SequenceTypeModuleDescriptorBase : ModuleDescriptorBase, ISequenceTypeModuleDescriptor,
	                                                         IEqualityComparer<ISequenceTypeModuleDescriptor>,
	                                                         IEquatable<ISequenceTypeModuleDescriptor>,
	                                                         IEqualityComparer<SequenceTypeModuleDescriptorBase>,
	                                                         IEquatable<SequenceTypeModuleDescriptorBase>
	{
		public abstract override string TypeName { get; }

		public abstract override Guid TypeId { get; }

		public abstract override Type ModuleClass { get; }

		public abstract override string Author { get; }

		public abstract override string Description { get; }

		public abstract override string Version { get; }

		public abstract string FileExtension { get; }

		public abstract int ObjectVersion { get; }

		// Default to true unless overridden in derived class
		public virtual bool CanCreateNew
		{
			get { return true; }
		}

		public bool Equals(ISequenceTypeModuleDescriptor x, ISequenceTypeModuleDescriptor y)
		{
			return base.Equals(x, y);
		}

		public int GetHashCode(ISequenceTypeModuleDescriptor obj)
		{
			return base.GetHashCode(obj);
		}

		public bool Equals(ISequenceTypeModuleDescriptor other)
		{
			return base.Equals(other);
		}

		public bool Equals(SequenceTypeModuleDescriptorBase x, SequenceTypeModuleDescriptorBase y)
		{
			return Equals(x as ISequenceTypeModuleDescriptor, y as ISequenceTypeModuleDescriptor);
		}

		public int GetHashCode(SequenceTypeModuleDescriptorBase obj)
		{
			return GetHashCode(obj as ISequenceTypeModuleDescriptor);
		}

		public bool Equals(SequenceTypeModuleDescriptorBase other)
		{
			return Equals(other as ISequenceTypeModuleDescriptor);
		}
	}
}