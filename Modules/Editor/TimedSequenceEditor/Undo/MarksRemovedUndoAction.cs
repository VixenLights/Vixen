using System;
using System.Collections.Generic;
using Vixen.Sys;
using VixenModules.Sequence.Timed;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class MarksRemovedUndoAction : MarksAddedRemovedUndoAction
	{
		public MarksRemovedUndoAction(TimedSequenceEditorForm form, Dictionary<TimeSpan, MarkCollection> markCollections)
			: base(form, markCollections)
		{
		}

		public override void Undo()
		{
			addMark();
			base.Undo();
		}

		public override void Redo()
		{
			removeMark();
			base.Redo();
		}

		public override string Description
		{
			get { return string.Format("Removed {0} Mark{1}", Count, (Count == 1 ? string.Empty : "s")); }
		}
	}
}