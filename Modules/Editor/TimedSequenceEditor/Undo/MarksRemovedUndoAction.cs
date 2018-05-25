using System.Collections.Generic;
using VixenModules.App.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class MarksRemovedUndoAction : MarksAddedRemovedUndoAction
	{
		public MarksRemovedUndoAction(TimedSequenceEditorForm form, Dictionary<Mark, MarkCollection> markCollections)
			: base(form, markCollections)
		{
		}

		public MarksRemovedUndoAction(TimedSequenceEditorForm form, Mark mark, MarkCollection mc)
			: base(form, new Dictionary<Mark, MarkCollection>() { { mark, mc } })
		{
		}
		public override void Undo()
		{
			AddMark();
			base.Undo();
		}

		public override void Redo()
		{
			RemoveMark();
			base.Redo();
		}

		public override string Description => $"Removed {Count} Mark{(Count == 1 ? string.Empty : "s")}";
	}
}