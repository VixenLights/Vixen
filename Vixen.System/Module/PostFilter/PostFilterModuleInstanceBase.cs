using System;
using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace Vixen.Module.PostFilter {
	abstract public class PostFilterModuleInstanceBase : ModuleInstanceBase, IAnyCommandHandler, IPostFilterModuleInstance, IEqualityComparer<IPostFilterModuleInstance>, IEquatable<IPostFilterModuleInstance>, IEqualityComparer<PostFilterModuleInstanceBase>, IEquatable<PostFilterModuleInstanceBase> {
		virtual public bool HasSetup {
			get { return false; }
		}

		virtual public bool Setup() { return false; }

		abstract public ICommand Affect(ICommand command);

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

		public void Handle(ByteValue obj) {
			Affect(obj);
		}

		public void Handle(SignedShortValue obj) {
			Affect(obj);
		}

		public void Handle(UnsignedShortValue obj) {
			Affect(obj);
		}

		public void Handle(SignedIntValue obj) {
			Affect(obj);
		}

		public void Handle(UnsignedIntValue obj) {
			Affect(obj);
		}

		public void Handle(SignedLongValue obj) {
			Affect(obj);
		}

		public void Handle(UnsignedLongValue obj) {
			Affect(obj);
		}

		public void Handle(ColorValue obj) {
			Affect(obj);
		}

		public void Handle(LightingValueCommand obj) {
			Affect(obj);
		}
	}
}
