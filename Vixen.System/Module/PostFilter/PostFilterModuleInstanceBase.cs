using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Commands;

namespace Vixen.Module.PostFilter {
	abstract public class PostFilterModuleInstanceBase : ModuleInstanceBase, IPostFilterModuleInstance, IEqualityComparer<IPostFilterModuleInstance>, IEquatable<IPostFilterModuleInstance>, IEqualityComparer<PostFilterModuleInstanceBase>, IEquatable<PostFilterModuleInstanceBase> {
		virtual public bool HasSetup {
			get { return false; }
		}

		virtual public bool Setup() { return false; }

		virtual public void Affect(ICommand<float> value) { }

		virtual public void Affect(ICommand<Color> value) { }

		virtual public void Affect(ICommand<DateTime> value) { }

		virtual public void Affect(ICommand<long> value) { }

		virtual public void Affect(ICommand<double> value) { }

		public bool Equals(IPostFilterModuleInstance x, IPostFilterModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IPostFilterModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IPostFilterModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(PostFilterModuleInstanceBase x, PostFilterModuleInstanceBase y) {
			return Equals(x as IPostFilterModuleInstance, y as IPostFilterModuleInstance);
		}

		public int GetHashCode(PostFilterModuleInstanceBase obj) {
			return GetHashCode(obj as IPostFilterModuleInstance);
		}

		public bool Equals(PostFilterModuleInstanceBase other) {
			return Equals(other as IPostFilterModuleInstance);
		}
	}
}
