﻿using Vixen.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksMoveResizeInfo
	{
		public MarksMoveResizeInfo(IEnumerable<IMark> modifyingMarks)
		{
			
			OriginalMarks = new Dictionary<IMark, MarkTimeInfo>();
			foreach (var mark in modifyingMarks)
			{
				if (mark != null)
				{
					OriginalMarks.Add(mark, new MarkTimeInfo(mark));
				}
			}
		}
			
		///<summary>All marks being modified and their original parameters.</summary>
		public Dictionary<IMark, MarkTimeInfo> OriginalMarks { get; private set; }

	}
	
}
