using System;

namespace Vixen.Sys {
	public struct TimeNode : IEquatable<TimeNode>, IComparable<TimeNode>, ITimeNode {
		public TimeNode(TimeSpan startTime, TimeSpan timeSpan)
			: this() {
			StartTime = startTime;
			TimeSpan = timeSpan;
			EndTime = startTime + timeSpan;
		}

		public TimeSpan StartTime { get; private set; }
		public TimeSpan TimeSpan { get; private set; }
		public TimeSpan EndTime { get; private set; }

		public bool IntersectsInclusively(TimeSpan timeSpan) {
			return timeSpan >= StartTime && timeSpan <= EndTime;
		}

		public bool IntersectsExclusively(TimeSpan timeSpan) {
			return timeSpan > StartTime && timeSpan < EndTime;
		}

		public bool Intersects(ITimeNode timeNode) {
			return IntersectsExclusively(timeNode.StartTime) || IntersectsExclusively(timeNode.EndTime);
		}

		public bool Equals(TimeNode other) {
			return other.StartTime.Equals(StartTime) && other.TimeSpan.Equals(TimeSpan);
		}

		public override bool Equals(object obj) {
			if(ReferenceEquals(null, obj)) return false;
			if(obj.GetType() != typeof(TimeNode)) return false;
			return Equals((TimeNode)obj);
		}

		public override int GetHashCode() {
			unchecked {
				return (StartTime.GetHashCode()*397) ^ TimeSpan.GetHashCode();
			}
		}

		public static bool operator ==(TimeNode left, TimeNode right) {
			return left.Equals(right);
		}

		public static bool operator !=(TimeNode left, TimeNode right) {
			return !left.Equals(right);
		}

		//public static readonly TimeNode Empty = new TimeNode(TimeSpan.Zero, TimeSpan.Zero);
		public static readonly TimeNode Empty;

		public static ITimeNode Intersect(ITimeNode left, ITimeNode right) {
			TimeSpan intersectionStart = TimeSpan.FromMilliseconds(Math.Max(left.StartTime.TotalMilliseconds, right.StartTime.TotalMilliseconds));
			TimeSpan intersectionEnd = TimeSpan.FromMilliseconds(Math.Min(left.EndTime.TotalMilliseconds, right.EndTime.TotalMilliseconds));
			
			if(intersectionStart < intersectionEnd && intersectionStart > TimeSpan.Zero && intersectionEnd > TimeSpan.Zero) {
				return new TimeNode(intersectionStart, intersectionEnd - intersectionStart);
			}

			return Empty;
		}

		public static bool IntersectsExclusively(ITimeNode timeNode, TimeSpan timeSpan) {
			//Leaving this here as a reminder that it used to be this way,
			//but I'm not sure if it was for a reason.
			//return timeSpan > timeNode.StartTime && timeSpan < timeNode.EndTime;
			return timeSpan >= timeNode.StartTime && timeSpan < timeNode.EndTime;
		}

		public static bool IntersectsInclusively(ITimeNode timeNode, TimeSpan timeSpan) {
			return timeSpan >= timeNode.StartTime && timeSpan <= timeNode.EndTime;
		}

		public static bool IntersectsExclusively(ITimeNode timeNode1, ITimeNode timeNode2) {
			return !(timeNode1.EndTime <= timeNode2.StartTime || timeNode1.StartTime >= timeNode2.EndTime);
		}

		public static bool IntersectsInclusively(ITimeNode timeNode1, ITimeNode timeNode2) {
			return !(timeNode1.EndTime < timeNode2.StartTime || timeNode1.StartTime > timeNode2.EndTime);
		}

		//public static bool Intersects(ITimeNode timeNode1, ITimeNode timeNode2) {
		//    return !(timeNode1.EndTime < timeNode2.StartTime || timeNode1.StartTime > timeNode2.EndTime);
		//}

		public static ITimeNode FromTimeSpan(TimeSpan timeSpan) {
			return new TimeNode(timeSpan, TimeSpan.Zero);
		}

		public int CompareTo(TimeNode other) {
			return StartTime.CompareTo(other.StartTime);
		}
	}

	public interface ITimeNode {
		TimeSpan StartTime { get; }
		TimeSpan TimeSpan { get; }
		TimeSpan EndTime { get; }
	}
}
