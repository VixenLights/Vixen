using System;
using System.Collections.Generic;
using Vixen.Sys;
using VixenModules.Sequence.Timed;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class MarksAddedUndoAction : MarksAddedRemovedUndoAction
	{
		public MarksAddedUndoAction(TimedSequenceEditorForm form, Dictionary<TimeSpan, MarkCollection> markCollections)
			: base(form, markCollections)
		{
		}

		public override void Undo()
		{
			removeMark();
			base.Undo();
		}

		public override void Redo()
		{
			addMark();
			base.Redo();
		}

		public override string Description
		{
			get { return string.Format("Added {0} Mark{1}", Count, (Count == 1 ? string.Empty : "s")); }
		}
	}
}