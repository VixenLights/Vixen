﻿using Vixen.Module.Editor;
using Vixen.Sys;

namespace Vixen.Services
{
	public class EditorService
	{
		private static EditorService _instance;

		private EditorService()
		{
		}

		public static EditorService Instance
		{
			get { return _instance ?? (_instance = new EditorService()); }
		}

		public IEditorUserInterface CreateEditor(string sequenceFilePath)
		{
			// Create or load a sequence.
			ISequence sequence;
			if (File.Exists(sequenceFilePath)) {
				sequence = SequenceService.Instance.Load(sequenceFilePath);
			}
			else {
				sequence = SequenceService.Instance.CreateNew(sequenceFilePath);
			}

			if (sequence == null)
				return null;

			// Get the editor.
			IEditorUserInterface editor = null;
			EditorModuleManagement manager = Modules.GetManager<IEditorModuleInstance, EditorModuleManagement>();
			if (manager != null) {
				editor = manager.Get(sequence.GetType());
			}

			if (editor != null) {
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