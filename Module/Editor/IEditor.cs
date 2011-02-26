using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sequence;
using Vixen.Module.Sequence;

namespace Vixen.Module.Editor {
	public interface IEditor {
		ISequenceModuleInstance Sequence { get; set; }
		ISelection Selection { get; }
		/// <summary>
		/// Creates a new sequence within the editor.
		/// </summary>
		void NewSequence();
		void SaveSequence();
		void LoadSequence(string fileName);
		void Refresh();
		Dictionary<string, string> EditorValues { get; }
	}
}
