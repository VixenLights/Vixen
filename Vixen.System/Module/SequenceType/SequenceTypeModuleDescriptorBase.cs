using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.SequenceType {
	abstract public class SequenceTypeModuleDescriptorBase : ModuleDescriptorBase, ISequenceTypeModuleDescriptor, IEqualityComparer<ISequenceTypeModuleDescriptor>, IEquatable<ISequenceTypeModuleDescriptor>, IEqualityComparer<SequenceTypeModuleDescriptorBase>, IEquatable<SequenceTypeModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public string FileExtension { get; }

		abstract public int ClassVersion { get; }

		///// <summary>
		///// Must implement ISequenceTypeDataModel.
		///// </summary>
		//abstract public Type SequenceDataType { get; }

		// Default to true unless overridden in derived class
		virtual public bool CanCreateNew { get { return true; }	}

		public bool Equals(ISequenceTypeModuleDescriptor x, ISequenceTypeModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ISequenceTypeModuleDescriptor obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(ISequenceTypeModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(SequenceTypeModuleDescriptorBase x, SequenceTypeModuleDescriptorBase y) {
			return Equals(x as ISequenceTypeModuleDescriptor, y as ISequenceTypeModuleDescriptor);
		}

		public int GetHashCode(SequenceTypeModuleDescriptorBase obj) {
			return GetHashCode(obj as ISequenceTypeModuleDescriptor);
		}

		public bool Equals(SequenceTypeModuleDescriptorBase other) {
			return Equals(other as ISequenceTypeModuleDescriptor);
		}

	}
}
