using System.IO;
using Vixen.Module.Editor;
using Vixen.Sys;

namespace Vixen.Services {
	public class EditorService {
		static private EditorService _instance;

		private EditorService() { }

		public static EditorService Instance {
			get { return _instance ?? (_instance = new EditorService()); }
		}

		public IEditorUserInterface CreateEditor(string sequenceFilePath) {
			// Create or load a sequence.
			ISequence sequence;
			if(File.Exists(sequenceFilePath)) {
				sequence = SequenceService.Instance.Load(sequenceFilePath);
			} else {
				sequence = SequenceService.Instance.CreateNew(sequenceFilePath);
			}

			// Get the editor.
			IEditorUserInterface editor = null;
			EditorModuleManagement manager = Modules.GetManager<IEditorModuleInstance, EditorModuleManagement>();
			if(manager != null) {
				editor = manager.Get(sequenceFilePath);
			}

			if(editor != null && sequence != null) {
				// Get any editor module data from the sequence.
				//...serious LoD violation...
				sequence.SequenceData.LocalDataSet.AssignModuleTypeData(editor.OwnerModule);

				// Assign the sequence to the editor.
				editor.Sequence = sequence;
			}

			return editor;
		}
	}
}
