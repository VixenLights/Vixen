using System;
using System.Collections.Generic;
using Common.Controls;
using Common.Controls.Timeline;
using Common.Controls.TimelineControl.LabeledMarks;
using Vixen.Marks;
using VixenModules.App.Marks;

namespace VixenModules.Editor.TimedSequenceEditor.Undo
{
	public class MarksTimeChangedUndoAction : UndoAction
	{
		private Dictionary<IMark, MarkTimeInfo> _changedMarks;
		private readonly ElementMoveType _moveType;
		private readonly TimedSequenceEditorForm _form;

		public MarksTimeChangedUndoAction(TimedSequenceEditorForm form, Dictionary<IMark, MarkTimeInfo> changedMarks, ElementMoveType moveType)
			: base()
		{
			_changedMarks = changedMarks;
			_moveType = moveType;
			_form = form;
		}

		public override void Undo()
		{
			_form.SwapPlaces(_changedMarks);

			base.Undo();
		}

		public override void Redo()
		{
			//Check to see if our element still exists, or has been replaced because of a delete and add manuver
			var validatedMarks = new Dictionary<IMark, MarkTimeInfo>();
			foreach (var markTimeInfo in _changedMarks)
			{
				if (markTimeInfo.Key.Parent.Marks.Contains(markTimeInfo.Key))
				{
					validatedMarks.Add(markTimeInfo.Key, markTimeInfo.Value);
				}
			}

			_changedMarks = validatedMarks;

			_form.SwapPlaces(_changedMarks);

			base.Redo();
		}

		public override string Description
		{
			get
			{
				string s = (_changedMarks.Count == 1 ? string.Empty : "s");
				switch (_moveType)
				{
					case ElementMoveType.Move:
						return $"Moved {_changedMarks.Count} effect{s}";
					case ElementMoveType.Resize:
						return $"Resize {_changedMarks.Count} effect{s}";
					default:
						throw new Exception("Unknown ElementMoveType!");
				}
			}
		}
	}
}
