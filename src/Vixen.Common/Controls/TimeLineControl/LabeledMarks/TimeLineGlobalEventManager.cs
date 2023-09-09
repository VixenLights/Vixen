using System.Diagnostics;
using Common.Controls.Timeline;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class TimeLineGlobalEventManager
	{
		private static readonly Dictionary<Guid, TimeLineGlobalEventManager> Instances = new();

		public event EventHandler<MarksTextChangedEventArgs> MarksTextChanged;
		public event EventHandler<MarksMovedEventArgs> MarksMoved;
		public event EventHandler<MarksMovingEventArgs> MarksMoving;
		public event EventHandler<MarksDeletedEventArgs> DeleteMark;
		public event EventHandler<MarksPastedEventArgs> MarksPasted;
		public event EventHandler<AlignmentEventArgs> AlignmentActivity;
		public event EventHandler<PhonemeBreakdownEventArgs> PhonemeBreakdownAction;
		public event EventHandler<PlayRangeEventArgs> PlayRangeAction;
		public event EventHandler<TimeSpanEventArgs> CursorMoved;



		private TimeLineGlobalEventManager(Guid id)
		{
			InstanceId = id;
		}

		public static TimeLineGlobalEventManager Manager(Guid id)
		{
			if (Instances.TryGetValue(id, out var instance))
			{
				return instance;
			}
			else
			{
				instance = new TimeLineGlobalEventManager(id);
				Instances.Add(id, instance);
			}

			return instance;
		}

		public static bool CloseManager(Guid id)
		{
			return Instances.Remove(id);
		}

		public Guid InstanceId { get; init; }

		public void OnAlignmentActivity(AlignmentEventArgs e)
		{
			AlignmentActivity?.Invoke(this, e);
		}

		public void OnMarkMoved(MarksMovedEventArgs e)
		{
			MarksMoved?.Invoke(this, e);
		}

		public void OnMarksMoving(MarksMovingEventArgs e)
		{
			var t = e.Marks.Select(x => x.Parent).Distinct();
			foreach (var markCollection in t)
			{
				markCollection.EnsureOrder();
			}
			MarksMoving?.Invoke(this, e);
		}

		public void OnDeleteMark(MarksDeletedEventArgs e)
		{
			DeleteMark?.Invoke(this, e);
		}

		public void OnMarksPasted(MarksPastedEventArgs e)
		{
			MarksPasted?.Invoke(this, e);
		}

		public void OnMarksTextChanged(MarksTextChangedEventArgs e)
		{
			MarksTextChanged?.Invoke(this, e);
		}

		public void OnPhonemeBreakdownAction(PhonemeBreakdownEventArgs e)
		{
			PhonemeBreakdownAction?.Invoke(this, e);
		}

		public void OnPlayRange(PlayRangeEventArgs e)
		{
			PlayRangeAction?.Invoke(this, e);
		}

		public void OnCursorMoved(TimeSpan t)
		{
			CursorMoved?.Invoke(this, new TimeSpanEventArgs(t));
		}
		
	}
}
