using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Sys;

namespace Vixen.Module.PostFilter {
	abstract public class PostFilterModuleInstanceBase : ModuleInstanceBase, IPostFilterModuleInstance, IEqualityComparer<IPostFilterModuleInstance>, IEquatable<IPostFilterModuleInstance>, IEqualityComparer<PostFilterModuleInstanceBase>, IEquatable<PostFilterModuleInstanceBase> {
		//abstract public Command Affect(Command command);

		virtual public bool HasSetup {
			get { return false; }
		}

		virtual public bool Setup() { return false; }

		virtual public float Affect(float value) {
			return value;
		}

		virtual public Color Affect(Color value) {
			return value;
		}

		virtual public DateTime Affect(DateTime value) {
			return value;
		}

		virtual public IFilterState CreateFilterState() {
			return new PostFilterState(this);
		}

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
