using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sys;

namespace Vixen.Module.Editor {
	public interface IEditor {
		ISequence Sequence { get; set; }
		ISelection Selection { get; }
		/// <summary>
		/// Creates a new sequence within the editor.
		/// </summary>
		void NewSequence();
		void Save(string filePath = null);
		void Refresh();
		EditorValues EditorValues { get; }
		bool IsModified { get; }
	}
}
