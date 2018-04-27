using System;
using System.Runtime.Serialization;

namespace Vixen.Sys.Marks
{
	public class Mark:IComparable<Mark>, IEquatable<Mark>, ICloneable
	{
		public Mark() : this(TimeSpan.Zero)
		{

		}

		public Mark(TimeSpan startTime)
		{
			StartTime = startTime;
			Duration = TimeSpan.FromMilliseconds(500);
			Text = @"Mark"; //What is the reasonable deafault?
		}

		public string Text { get; set; }

		[DataMember]
		public TimeSpan StartTime { get; set; }

		[DataMember]
		public TimeSpan Duration { get; set; }

		public TimeSpan EndTime => StartTime + Duration;

		public int CompareTo(Mark other)
		{
			int rv = StartTime.CompareTo(other.StartTime);
			if (rv != 0)
			{
				return rv;
			}

			return EndTime.CompareTo(other.EndTime);
		}

		#region Implementation of ICloneable

		/// <inheritdoc />
		public object Clone()
		{
			return new Mark(StartTime)
			{
				Duration = Duration,
				Text = Text
			};
		}

		#endregion

		#region Equality members

		/// <inheritdoc />
		public bool Equals(Mark other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return StartTime.Equals(other.StartTime) && Duration.Equals(other.Duration);
		}

		/// <inheritdoc />
		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Mark) obj);
		}

		/// <inheritdoc />
		public override int GetHashCode()
		{
			unchecked
			{
				return (StartTime.GetHashCode() * 397) ^ Duration.GetHashCode();
			}
		}

		#endregion
	}
}
