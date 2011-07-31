using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;

namespace Vixen.Module.Transform {
	abstract public class TransformModuleInstanceBase : ModuleInstanceBase, ITransformModuleInstance, IEqualityComparer<ITransformModuleInstance>, IEquatable<ITransformModuleInstance>, IEqualityComparer<TransformModuleInstanceBase>, IEquatable<TransformModuleInstanceBase> {
		abstract public void Setup();

		/// <summary>
		/// Command identifier : Parameter affected.  Provided by the system.
		/// </summary>
		public CommandsAffected CommandsAffected {
			get { return (Descriptor as ITransformModuleDescriptor).CommandsAffected; }
		}

		abstract public void Transform(CommandData command);

		public bool Equals(ITransformModuleInstance x, ITransformModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(ITransformModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(ITransformModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(TransformModuleInstanceBase x, TransformModuleInstanceBase y) {
			return Equals(x as ITransformModuleInstance, y as ITransformModuleInstance);
		}

		public int GetHashCode(TransformModuleInstanceBase obj) {
			return GetHashCode(obj as ITransformModuleInstance);
		}

		public bool Equals(TransformModuleInstanceBase other) {
			return Equals(other as ITransformModuleInstance);
		}
	}
}
