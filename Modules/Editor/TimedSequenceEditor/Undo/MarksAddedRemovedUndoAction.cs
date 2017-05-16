using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;
using Vixen.Sys.LayerMixing;
using VixenModules.Sequence.Timed;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class MarksAddedRemovedUndoAction : Common.Controls.UndoAction
	{
		private TimedSequenceEditorForm _form;
		private readonly Dictionary<TimeSpan, MarkCollection> _markCollections;
		private readonly int _count;

		public MarksAddedRemovedUndoAction(TimedSequenceEditorForm form, Dictionary<TimeSpan, MarkCollection> markCollections)
		{
			_form = form;
			_markCollections = markCollections;
			_count = _markCollections.Count();
		}

		protected void removeMark()
		{
			_form.RemoveMark(_markCollections);
		}

		protected void addMark()
		{
			_form.AddMark(_markCollections);
		}

		protected int Count
		{
			get { return _count; }
		}
	}
}