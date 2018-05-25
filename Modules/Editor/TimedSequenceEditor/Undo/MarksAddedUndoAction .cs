using System.Collections.Generic;
using Vixen.Marks;
using VixenModules.App.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class MarksAddedUndoAction : MarksAddedRemovedUndoAction
	{
		public MarksAddedUndoAction(TimedSequenceEditorForm form, Dictionary<IMark, IMarkCollection> markCollections)
			: base(form, markCollections)
		{
		}

		public MarksAddedUndoAction(TimedSequenceEditorForm form, IMark mark, IMarkCollection mc)
			: base(form, new Dictionary<IMark, IMarkCollection>(){{mark, mc}})
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