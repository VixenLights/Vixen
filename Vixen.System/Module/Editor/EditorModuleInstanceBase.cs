using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Editor {
	abstract public class EditorModuleInstanceBase : ModuleInstanceBase, IEditorModuleInstance, IEqualityComparer<IEditorModuleInstance>, IEquatable<IEditorModuleInstance>, IEqualityComparer<EditorModuleInstanceBase>, IEquatable<EditorModuleInstanceBase> {
		virtual public ISequence Sequence { get; set; }

		abstract public ISelection Selection { get; }

		abstract public void NewSequence();

		abstract public void Save(string filePath = null);

		abstract public void Refresh();

		abstract public EditorValues EditorValues { get; }

		abstract public bool IsModified { get; }

		public bool Equals(IEditorModuleInstance x, IEditorModuleInstance y) {
			return base.Equals(x, y);
		}

		public int GetHashCode(IEditorModuleInstance obj) {
			return base.GetHashCode(obj);
		}

		public bool Equals(IEditorModuleInstance other) {
			return base.Equals(other);
		}

		public bool Equals(EditorModuleInstanceBase x, EditorModuleInstanceBase y) {
			return Equals(x as IEditorModuleInstance, y as IEditorModuleInstance);
		}

		public int GetHashCode(EditorModuleInstanceBase obj) {
			return GetHashCode(obj as IEditorModuleInstance);
		}

		public bool Equals(EditorModuleInstanceBase other) {
			return Equals(other is IEditorModuleInstance);
		}
	}
}
