using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Controls;
using Common.Controls.Timeline;
using Vixen.Module;
using VixenModules.Sequence.Timed;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class MarksMovedUndoAction : UndoAction
	{
		private readonly TimedSequenceEditorForm _form;
		private readonly List<TimedSequenceEditorForm.MarkNdugeCollection> _markCollection;
		private readonly int _count;

		public MarksMovedUndoAction(TimedSequenceEditorForm form, List<TimedSequenceEditorForm.MarkNdugeCollection> markCollection)
		{
			_form = form;
			_markCollection = markCollection;
			_count = markCollection.Count();
		}

		public override void Undo()
		{
			foreach (TimedSequenceEditorForm.MarkNdugeCollection marks in _markCollection)
			{
				_form.MovedMark(marks.MarkCollection, marks.NewMark, marks.OldMark);
			}
			
			base.Undo();
		}

		public override void Redo()
		{
			foreach (TimedSequenceEditorForm.MarkNdugeCollection marks in _markCollection)
			{
				_form.MovedMark(marks.MarkCollection, marks.OldMark, marks.NewMark);
			}
			base.Redo();
		}

		protected int Count
		{
			get { return _count; }
		}

		public override string Description
		{
			get
			{
				return string.Format("Moved {0} Mark{1}", Count, (Count == 1 ? string.Empty : "s"));
			}
		}
	}
}
