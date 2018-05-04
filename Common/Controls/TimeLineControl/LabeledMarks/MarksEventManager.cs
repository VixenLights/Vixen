using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksEventManager
	{
		private static MarksEventManager _manager;

		public event EventHandler<MarksMovedEventArgs> MarksMoved;
		public event EventHandler<MarksMovingEventArgs> MarksMoving;
		public event EventHandler<MarksDeletedEventArgs> DeleteMark;
		public event EventHandler<SelectedMarkMoveEventArgs> SelectedMarkMove;

		private MarksEventManager()
		{
			
		}

		public static MarksEventManager Manager => _manager ?? (_manager = new MarksEventManager());

		public void OnSelectedMarkMove(SelectedMarkMoveEventArgs e)
		{
			SelectedMarkMove?.Invoke(this, e);
		}

		public void OnMarkMoved(MarksMovedEventArgs e)
		{
			MarksMoved?.Invoke(this, e);
		}

		public void OnMarksMoving(MarksMovingEventArgs e)
		{
			MarksMoving?.Invoke(this, e);
		}

		public void OnDeleteMark(MarksDeletedEventArgs e)
		{
			DeleteMark?.Invoke(this, e);
		}

	}
}
