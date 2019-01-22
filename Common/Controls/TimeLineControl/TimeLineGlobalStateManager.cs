using System;
using Common.Controls.TimelineControl.LabeledMarks;

namespace Common.Controls.TimelineControl
{
	public class TimeLineGlobalStateManager
	{

		private static TimeLineGlobalStateManager _manager;
		private TimeSpan _cursorPosition = TimeSpan.Zero;

		private TimeLineGlobalStateManager()
		{

		}

		public static TimeLineGlobalStateManager Manager => _manager ?? (_manager = new TimeLineGlobalStateManager());

		public TimeSpan CursorPosition
		{
			get => _cursorPosition;
			set
			{
				_cursorPosition = value;
				TimeLineGlobalEventManager.Manager.OnCursorMoved(value);
			}
		}
	}
}
