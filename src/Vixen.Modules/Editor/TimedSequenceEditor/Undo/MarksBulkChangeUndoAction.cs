using Common.Controls;
using Common.Controls.Timeline;
using Common.Controls.TimelineControl.LabeledMarks;
using Vixen.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class MarksBulkChangeUndoAction : UndoAction
	{
		private MarksBulkChangeInfo _marksBulkChangeInfo;
		private readonly TimedSequenceEditorForm _form;

		public MarksBulkChangeUndoAction(TimedSequenceEditorForm form, MarksBulkChangeInfo marksBulkChangeInfo)
			: base()
		{
			_marksBulkChangeInfo = marksBulkChangeInfo;
			_form = form;
		}

		public override void Undo()
		{
			foreach (var mark in _marksBulkChangeInfo.OriginalMarks)
			{
				if (mark.Value == null)
				{
					mark.Key.Parent.AddMark(mark.Key);
				}
				else
				{
					mark.Key.Parent.SwapPlaces(mark.Key, mark.Value);
				}
			}

			base.Undo();
		}

		public override void Redo()
		{
			//Check to see if our element still exists, or has been replaced because of a delete and add manuver
			foreach (var mark in _marksBulkChangeInfo.OriginalMarks)
			{
				if (mark.Value == null)
				{
					mark.Key.Parent.RemoveMark(mark.Key);
				}
				else
				{
					mark.Key.Parent.SwapPlaces(mark.Key, mark.Value);
				}
			}

			base.Redo();
		}

		public override string Description
		{
			get
			{
				string s = (_marksBulkChangeInfo.OriginalMarks.Count > 1 ? "s" : String.Empty);
				return $"Bulk Change {_marksBulkChangeInfo.OriginalMarks.Count} Mark{s}";
			}
		}
	}
}
