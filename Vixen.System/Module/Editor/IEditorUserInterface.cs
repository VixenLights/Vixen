using System;
using System.ComponentModel;
using Vixen.Sys;

namespace Vixen.Module.Editor
{
	public interface IEditorUserInterface : IDisposable
	{
		event CancelEventHandler Closing;
		event EventHandler Activated;

		IEditorModuleInstance OwnerModule { get; set; }

		/// <summary>
		/// The editor's signal to show or otherwise start as the user interface.
		/// </summary>
		void StartEditor();

		/// <summary>
		/// The editor's signal to close the user interface. This is allowed to block and prompt for UI, if needed.
		/// </summary>
		void CloseEditor();

		/// <summary>
		/// Send to the editor by the main application when it is closing.
		/// </summary>
		void EditorClosing();

		ISequence Sequence { get; set; }
		ISelection Selection { get; }
		void Save(string filePath = null);
		void Refresh();
		//EditorValues EditorValues { get; }
		bool IsModified { get; }
		bool IsEditorStateModified { get; set; }
		bool IsDisposed { get; }
	}
}