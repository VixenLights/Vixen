using System;

namespace Vixen.Sys {
	public struct TimeNode : IEquatable<TimeNode>, IComparable<TimeNode> {
		public TimeNode(TimeSpan startTime, TimeSpan timeSpan) {
			StartTime = startTime;
			TimeSpan = timeSpan;
			EndTime = startTime + timeSpan;
		}

		public readonly TimeSpan StartTime;
		public readonly TimeSpan TimeSpan;
		public readonly TimeSpan EndTime;

		public bool Intersects(TimeSpan timeSpan) {
			return timeSpan > StartTime && timeSpan < EndTime;
		}

		public bool Intersects(TimeNode timeNode) {
			return Intersects(timeNode.StartTime) || Intersects(timeNode.EndTime);
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

		public static readonly TimeNode Empty = new TimeNode(TimeSpan.Zero, TimeSpan.Zero);

		public static TimeNode Intersect(TimeNode left, TimeNode right) {
			TimeSpan intersectionStart = TimeSpan.FromMilliseconds(Math.Max(left.StartTime.TotalMilliseconds, right.StartTime.TotalMilliseconds));
			TimeSpan intersectionEnd = TimeSpan.FromMilliseconds(Math.Min(left.EndTime.TotalMilliseconds, right.EndTime.TotalMilliseconds));
			
			if(intersectionStart < intersectionEnd && intersectionStart > TimeSpan.Zero && intersectionEnd > TimeSpan.Zero) {
				return new TimeNode(intersectionStart, intersectionEnd - intersectionStart);
			}

			return Empty;
		}

		public int CompareTo(TimeNode other) {
			return StartTime.CompareTo(other.StartTime);
		}
	}
}
