using System.Collections.Generic;
using Vixen.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class MarksRemovedUndoAction : MarksAddedRemovedUndoAction
	{
		public MarksRemovedUndoAction(TimedSequenceEditorForm form, Dictionary<IMark, IMarkCollection> markCollections)
			: base(form, markCollections)
		{
		}

		public MarksRemovedUndoAction(TimedSequenceEditorForm form, IMark mark, IMarkCollection mc)
			: base(form, new Dictionary<IMark, IMarkCollection>() { { mark, mc } })
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