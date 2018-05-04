using System;
using System.Collections.Generic;
using System.Drawing;
using Vixen.Sys.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksMoveResizeInfo
	{
		public MarksMoveResizeInfo(Point initGridLocation, IEnumerable<Mark> modifyingMarks, TimeSpan visibleTimeStart)
		{
			InitialGridLocation = initGridLocation;
			VisibleTimeStart = visibleTimeStart;

			OriginalMarks = new Dictionary<Mark, MarkTimeInfo>();
			foreach (var mark in modifyingMarks)
			{
				OriginalMarks.Add(mark, new MarkTimeInfo(mark));
			}
		}
			
		///<summary>The point on the grid where the mouse first went down.</summary>
		public Point InitialGridLocation { get; private set; }

		///<summary>All elements being modified and their original parameters.</summary>
		public Dictionary<Mark, MarkTimeInfo> OriginalMarks { get; private set; }

		public TimeSpan VisibleTimeStart { get; private set; }
	}
	
}
