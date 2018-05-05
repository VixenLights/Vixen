using System.Collections.Generic;
using Vixen.Sys.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksMoveResizeInfo
	{
		public MarksMoveResizeInfo(IEnumerable<Mark> modifyingMarks)
		{
			
			OriginalMarks = new Dictionary<Mark, MarkTimeInfo>();
			foreach (var mark in modifyingMarks)
			{
				OriginalMarks.Add(mark, new MarkTimeInfo(mark));
			}
		}
			
		///<summary>All marks being modified and their original parameters.</summary>
		public Dictionary<Mark, MarkTimeInfo> OriginalMarks { get; private set; }

	}
	
}
