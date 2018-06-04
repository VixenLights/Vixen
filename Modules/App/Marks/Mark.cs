using System;
using System.Runtime.Serialization;
using Vixen.Marks;

namespace VixenModules.App.Marks
{
	[DataContract]
	public class Mark:BindableBase, IMark
	{
		private TimeSpan _startTime;
		private string _text;
		private TimeSpan _duration;

		public Mark() : this(TimeSpan.Zero)
		{

		}

		public Mark(TimeSpan startTime)
		{
			_startTime = startTime;
			_duration = TimeSpan.FromMilliseconds(450);
			_text = String.Empty;
		}

		[DataMember]
		public string Text
		{
			get { return _text; }
			set
			{
				if (value == _text) return;
				_text = value;
				OnPropertyChanged(nameof(Text));
			}
		}

		[DataMember]
		public TimeSpan StartTime
		{
			get { return _startTime; }
			set
			{
				if (value.Equals(_startTime)) return;
				_startTime = value;
				if (Duration < TimeSpan.FromMilliseconds(25))
				{
					Duration = TimeSpan.FromMilliseconds(25);
				}
				OnPropertyChanged(nameof(StartTime));
				OnPropertyChanged(nameof(EndTime));
			}
		}

		[DataMember]
		public TimeSpan Duration
		{
			get { return _duration; }
			set
			{
				if (value.Equals(_duration)) return;
				_duration = value;
				if (_duration < TimeSpan.FromMilliseconds(25))
				{
					_duration = TimeSpan.FromMilliseconds(25);
				}
				OnPropertyChanged(nameof(Duration));
				OnPropertyChanged(nameof(EndTime));
			}
		}

		public TimeSpan EndTime => StartTime + Duration;

		[IgnoreDataMember]
		public IMarkCollection Parent { get; set; }

		public int CompareTo(IMark other)
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


		
	}
}
