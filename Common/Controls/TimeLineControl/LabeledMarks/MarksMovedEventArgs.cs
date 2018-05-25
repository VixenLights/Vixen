using System;
using System.Collections.Generic;
using Common.Controls.Timeline;
using VixenModules.App.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksMovedEventArgs : EventArgs
	{
		public MarksMovedEventArgs(MarksMoveResizeInfo marksMoveResizeInfo, ElementMoveType type)
		{
			MoveType = type;
			MoveResizeInfo = new Dictionary<Mark, MarkTimeInfo>();
			foreach (var originalMark in marksMoveResizeInfo.OriginalMarks)
			{
				MoveResizeInfo.Add(originalMark.Key, originalMark.Value);	
			}
		}

		public Dictionary<Mark, MarkTimeInfo> MoveResizeInfo { get; }

		public ElementMoveType MoveType { get; }
		
	}
}