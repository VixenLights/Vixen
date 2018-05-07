using System.Collections.Generic;
using VixenModules.App.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class MarksAddedUndoAction : MarksAddedRemovedUndoAction
	{
		public MarksAddedUndoAction(TimedSequenceEditorForm form, Dictionary<Mark, MarkCollection> markCollections)
			: base(form, markCollections)
		{
		}

		public MarksAddedUndoAction(TimedSequenceEditorForm form, Mark mark, MarkCollection mc)
			: base(form, new Dictionary<Mark, MarkCollection>(){{mark, mc}})
		{
		}

		public override void Undo()
		{
			RemoveMark();
			base.Undo();
		}

		public override void Redo()
		{
			AddMark();
			base.Redo();
		}

		public override string Description => $"Added {Count} Mark{(Count == 1 ? string.Empty : "s")}";
	}
}