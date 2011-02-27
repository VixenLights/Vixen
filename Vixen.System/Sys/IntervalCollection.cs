using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public class IntervalCollection : IEnumerable<Interval> {
		// They need to be ordered without needing to be explicitly ordered every time, so a list.
		private List<Interval> _intervals = new List<Interval>();

		public IntervalCollection() {
		}

		/// <summary>
		/// Initially seeds the collection with intervals starting at 0 spanning length.
		/// </summary>
		/// <param name="interval"></param>
		/// <param name="length"></param>
		public IntervalCollection(int interval, int length) {
			List<int> times = new List<int>();
			for(int i = 0; i < length; i += interval) {
				times.Add(i);
			}
			InsertRange(times);
		}

		public void Insert(int time) {
			Interval newInterval = new Interval(time);

			if(_intervals.Count > 0) {
				Interval firstInterval = _intervals.First();
				Interval lastInterval = _intervals.Last();
				if(_GetIntervalAt(time) == null) {
					// Is the time after the last?
					if(time > lastInterval.Time) {
						_intervals.Add(newInterval);
					} else if(time < firstInterval.Time) {
						// Is it before the first?
						_intervals.Insert(0, newInterval);
					} else {
						// Then find its place.
						int insertIndex = _intervals.FindIndex(x => x.Time > time);
						_intervals.Insert(insertIndex, newInterval);
					}
				}
			} else {
				// There are no other marks to consider.
				_intervals.Add(newInterval);
			}
		}

		public void InsertRange(IEnumerable<int> times) {
			// Get the set of times that aren't already present in the collection.
			var goodTimes = times.Except(_intervals.Select(x => x.Time));
			// Insert the times.
			_intervals.AddRange(goodTimes.Select(x => new Interval(x)));
			// Sort.
			_intervals.Sort((left, right) => left.Time.CompareTo(right.Time));
		}

		public void Remove(int time) {
			Interval timeMark;
			if((timeMark = _GetIntervalAt(time)) != null) {
				_intervals.Remove(timeMark);
			}
		}

		public void RemoveRange(int startTime, int endTime) {
			_intervals = _intervals.Where(x => x.Time < startTime || x.Time >= endTime).ToList();
		}

		public void Add(int timeSpan, int interval) {
			if(interval == 0) return;

			int time;
			if(_intervals.Count == 0) {
				time = 0;
			} else {
				time = _intervals[_intervals.Count - 1].Time + interval;
			}
			for(int i = 0; i < timeSpan; i += interval, time += interval) {
				_intervals.Add(new Interval(time));
			}
		}

		public bool Adjust(int oldTime, int newTime) {
			Interval timeMark;
			if((timeMark = _GetIntervalAt(oldTime)) != null && _GetIntervalAt(newTime) == null) {
				timeMark.Time = newTime;
				return true;
			}
			return false;
		}

		public IEnumerable<int> GetCountedSpanTimes(int startTime, int count) {
			int startIndex = _intervals.FindIndex(x => x.Time >= startTime);
			if(startIndex != -1) {
				count = Math.Min(count, _intervals.Count - startIndex - 1);
				while(count-- > 0) {
					yield return _intervals[startIndex++].Time;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="timeSpan">Exclusive.</param>
		/// <returns></returns>
		public IEnumerable<int> GetSpanTimes(int startTime, int timeSpan) {
			int endTime = startTime + timeSpan;
			int startIndex = _intervals.FindIndex(x => x.Time >= startTime);
			if(startIndex != -1) {
				int endIndex = _intervals.FindIndex(x => x.Time >= endTime);
				if(endIndex == -1) {
					endIndex = _intervals.Count;
				}
				return GetCountedSpanTimes(startTime,  endIndex - startIndex);
			}
			return Enumerable.Empty<int>();
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="timeSpan">Exclusive.</param>
		/// <returns></returns>
		public int GetIntervalCount(int startTime, int timeSpan) {
			int endTime = startTime + timeSpan;
			int startIndex = _intervals.FindIndex(x => x.Time >= startTime);
			if(startIndex != -1) {
				int endIndex = _intervals.FindIndex(x => x.Time >= endTime);
				if(endIndex == -1) {
					endIndex = _intervals.Count;
				}
				// Exclusive.
				return endIndex - startIndex;
			}
			return 0;
		}

		public int Count {
			get { return _intervals.Count; }
		}

		public void Clear() {
			_intervals.Clear();
		}

		private Interval _GetIntervalAt(int time) {
			return _intervals.FirstOrDefault(x => x.Time == time);
		}

		public IEnumerator<Interval> GetEnumerator() {
			return _intervals.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
