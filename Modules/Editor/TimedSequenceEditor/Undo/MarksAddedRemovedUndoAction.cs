using System.Collections.Generic;
using System.Linq;
using Vixen.Marks;
using VixenModules.App.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class MarksAddedRemovedUndoAction : Common.Controls.UndoAction
	{
		private readonly TimedSequenceEditorForm _form;
		private readonly Dictionary<IMark, IMarkCollection> _markCollections;

		public MarksAddedRemovedUndoAction(TimedSequenceEditorForm form, Dictionary<IMark, IMarkCollection> markCollections)
		{
			_form = form;
			_markCollections = markCollections;
			Count = _markCollections.Count();
		}

		protected void RemoveMark()
		{
			_form.RemoveMark(_markCollections);
		}

		protected void AddMark()
		{
			_form.AddMark(_markCollections);
		}

		protected int Count { get; }
	}
}