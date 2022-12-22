using Common.Controls.Timeline;
using VixenModules.App.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class SelectedMarkMoveEventArgs
	{
		public SelectedMarkMoveEventArgs(bool active, Mark mark, ResizeZone resizeZone)
		{
			Active = active;
			Mark = mark;
			ResizeZone = resizeZone;
		}

		public bool Active { get; }

		public Mark Mark { get; }

		public ResizeZone ResizeZone { get; }
	}
}