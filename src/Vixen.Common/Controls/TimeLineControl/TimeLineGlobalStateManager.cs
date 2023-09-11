using Common.Controls.TimelineControl.LabeledMarks;

namespace Common.Controls.TimelineControl
{
	public class TimeLineGlobalStateManager
	{

		private TimeSpan _cursorPosition = TimeSpan.Zero;

		private static readonly Dictionary<Guid, TimeLineGlobalStateManager> Instances = new();

		private TimeLineGlobalStateManager(Guid id)
		{
			InstanceId = id;
		}

		public static TimeLineGlobalStateManager Manager(Guid id)
		{
			if (Instances.TryGetValue(id, out var instance))
			{
				return instance;
			}
			else
			{
				instance = new TimeLineGlobalStateManager(id);
				Instances.Add(id, instance);
			}

			return instance;
		}

		public static bool CloseManager(Guid id)
		{
			return Instances.Remove(id);
		}

		public Guid InstanceId { get; init; }

		public TimeSpan CursorPosition
		{
			get => _cursorPosition;
			set
			{
				_cursorPosition = value;
				TimeLineGlobalEventManager.Manager(InstanceId).OnCursorMoved(value);
			}
		}
	}
}
