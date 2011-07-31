using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Transform {
	abstract public class TransformModuleDescriptorBase : ModuleDescriptorBase, ITransformModuleDescriptor, IEqualityComparer<ITransformModuleDescriptor>, IEquatable<ITransformModuleDescriptor>, IEqualityComparer<TransformModuleDescriptorBase>, IEquatable<TransformModuleDescriptorBase> {
		abstract public override string TypeName { get; }

		abstract public override Guid TypeId { get; }

		abstract public override Type ModuleClass { get; }

		abstract public override Type ModuleDataClass { get; }

		abstract public override string Author { get; }

		abstract public override string Description { get; }

		abstract public override string Version { get; }

		abstract public Type[] TypesAffected { get; }

		public CommandsAffected CommandsAffected { get; set; }

		abstract public void Setup(ITransform[] transforms);

		public bool Equals(ITransformModuleDescriptor x, ITransformModuleDescriptor y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ITransformModuleDescriptor obj) {
			return base.GetHashCode();
		}

		public bool Equals(ITransformModuleDescriptor other) {
			return base.Equals(other);
		}

		public bool Equals(TransformModuleDescriptorBase x, TransformModuleDescriptorBase y) {
			return Equals(x as ITransformModuleDescriptor, y as ITransformModuleDescriptor);
		}

		public int GetHashCode(TransformModuleDescriptorBase obj) {
			return GetHashCode(obj as ITransformModuleDescriptor);
		}

		public bool Equals(TransformModuleDescriptorBase other) {
			return Equals(other as ITransformModuleDescriptor);
		}
	}
}
