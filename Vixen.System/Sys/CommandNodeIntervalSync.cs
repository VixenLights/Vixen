using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sequence;

namespace Vixen.Sys {
	public class CommandNodeIntervalSync {
		private ISequence _owner;
		private List<InputChannel> _inputChannels = new List<InputChannel>();
		private IntervalCollection _intervals;
		private int _intervalValue;
		private InputChannel _mainChannel;

		public CommandNodeIntervalSync(ISequence owner, List<InputChannel> inputChannels, IntervalCollection intervals) {
			_owner = owner;
			_intervals = intervals;
			// The sequence will have at least one input channel to hold command data.
			if(inputChannels.Count == 0) {
				inputChannels.Add(new InputChannel(true));
			}
			_inputChannels.AddRange(inputChannels);
			_mainChannel = _inputChannels[0];
		}

		#region Intervals
		public int TimingInterval {
			get { return _intervalValue; }
			set {
				_intervalValue = value;

				if(!_owner.IsUntimed) {
					// Create a new interval collection based on the interval.
					IntervalCollection newCollection = new IntervalCollection(_intervalValue, _owner.Length);
					// Pull data over.
					// Quantize each command's start time and time span to the new intervals.
					_QuantizeCommands(_inputChannels.SelectMany(x => x), newCollection);
					// Reference the new collection.
					_intervals = newCollection;
				}
			}
		}

		public IEnumerable<int> IntervalValues {
			get { return _intervals.Select(x => x.Time); }
			set {
				if(!_owner.IsUntimed) {
					// Create a new interval collection based on the collection.
					IntervalCollection newCollection = new IntervalCollection();
					newCollection.InsertRange(value);
					// Pull data over.
					// Quantize each command's start time and time span to the new intervals.
					_QuantizeCommands(_inputChannels.SelectMany(x => x), newCollection);
					// Reference the new collection.
					_intervals = newCollection;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime">Inclusive.</param>
		/// <param name="endTime">Exclusive.</param>
		/// <returns></returns>
		public IEnumerable<int> GetIntervalRange(int startTime, int endTime) {
			return _intervals.GetSpanTimes(startTime, endTime - startTime);
		}

		public void AdjustInterval(int intervalTime, int newIntervalTime) {
			if(_intervals.Adjust(intervalTime, newIntervalTime)) {
				foreach(CommandNode commandNode in _inputChannels.SelectMany(x => x).Where(x => x.StartTime == intervalTime)) {
					commandNode.TimeSpan -= (newIntervalTime - commandNode.StartTime);
					commandNode.StartTime = newIntervalTime;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime">Inclusive.</param>
		/// <param name="endTime">Exclusive.</param>
		/// <param name="delta"></param>
		public void AdjustIntervals(int startTime, int endTime, int delta) {
			// Create a new collection based on the existing collection.
			IntervalCollection newCollection = new IntervalCollection();
			newCollection.InsertRange(IntervalValues);
			// Adjust the values in the new collection.
			foreach(Interval interval in newCollection.Where(x => x.Time >= startTime && x.Time < endTime)) {
				interval.Time += delta;
			}
			// Adjust the data.
			// Quantize each command's start time and time span to the new intervals.
			_QuantizeCommands(_inputChannels.SelectMany(x => x), newCollection);
			// Reference the new collection.
			_intervals = newCollection;
		}

		public void InsertInterval(int time) {
			_intervals.Insert(time);
		}

		public void InsertIntervals(IEnumerable<int> times) {
			_intervals.InsertRange(times);
		}

		/// <summary>
		/// Adds intervals to the end of the collection.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="interval"></param>
		public void AddIntervals(int timeSpan, int interval) {
			_intervals.Add(timeSpan, interval);
		}

		public void RemoveInterval(int time) {
			_intervals.Remove(time);
		}

		public void RemoveIntervals(int startTime, int endTime) {
			_intervals.RemoveRange(startTime, endTime);
		}

		public void ClearIntervals() {
			_intervals.Clear();
		}
		#endregion

		#region CommandNodes
		public IEnumerable<CommandNode> GetCommands() {
			return _mainChannel;
		}

		public IEnumerable<CommandNode> GetCommands(Guid channelId) {
			return _inputChannels.Where(x => x.Id == channelId).FirstOrDefault();
		}

		public void AddCommand(CommandNode data) {
			_mainChannel.AddData(data);
		}

		public void AddCommand(Guid channelId, CommandNode data) {
			InputChannel channel = _inputChannels.Where(x => x.Id == channelId).FirstOrDefault();
			if(channel != null) {
				channel.AddData(data);
			}
		}

		public void AddCommandWithSync(CommandNode data) {
			_AddCommandsWithSyncInternal(_mainChannel, new CommandNode[] { data });
		}

		public void AddCommandWithSync(Guid channelId, CommandNode data) {
			// If there are intervals, the sync will sync the data.  If there aren't, it won't.
			InputChannel channel = _GetInputChannel(channelId);
			_AddCommandsWithSyncInternal(channel, new CommandNode[] { data });
		}

		public void AddCommands(IEnumerable<CommandNode> data) {
			_mainChannel.AddData(data);
		}

		public void AddCommands(Guid channelId, IEnumerable<CommandNode> data) {
			InputChannel channel = _inputChannels.Where(x => x.Id == channelId).FirstOrDefault();
			if(channel != null) {
				channel.AddData(data);
			}
		}

		public void AddCommandsWithSync(IEnumerable<CommandNode> data) {
			// If there are intervals, the sync will sync the data.  If there aren't, it won't.
			_AddCommandsWithSyncInternal(_mainChannel, data);
		}

		public void AddCommandsWithSync(Guid channelId, IEnumerable<CommandNode> data) {
			// If there are intervals, the sync will sync the data.  If there aren't, it won't.
			InputChannel channel = _GetInputChannel(channelId);
			_AddCommandsWithSyncInternal(channel, data);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime">Inclusive.</param>
		/// <param name="endTime">Exclusive.</param>
		/// <returns></returns>
		public IEnumerable<CommandNode> GetCommandRange(int startTime, int endTime) {
			// Need any data that starts or ends within the time range.
			return _inputChannels.SelectMany(x => x.Where(y => 
				(y.StartTime >= startTime && y.StartTime < endTime) ||
				(y.EndTime >= startTime && y.EndTime < endTime))
				);
		}
		#endregion

		#region Input Channels
		public Guid CreateInputChannel() {
			InputChannel channel = new InputChannel(true);
			_inputChannels.Add(channel);
			return channel.Id;
		}

		public void ClearInputChannel(Guid channelId) {
			//*** dictionary!
			InputChannel channel = _GetInputChannel(channelId);
			if(channel != null) {
				channel.Clear();
			}
		}

		public void RemoveInputChannel(Guid channelId) {
			InputChannel channel = _GetInputChannel(channelId);
			if(channel != null) {
				_inputChannels.Remove(channel);
			}
		}
		#endregion

		#region Private
		private void _QuantizeCommands(IEnumerable<CommandNode> commandNodes, IntervalCollection intervals) {
			// Start by creating a dictionary of intervals by time from the new intervals.
			Dictionary<int, Interval> intervalTimes = intervals.ToDictionary(x => x.Time, x => x);
			// Also create a list of ordered interval times.
			List<int> orderedTimes = intervalTimes.Keys.OrderBy(x => x).ToList();
			// Quantize each command's start time and time span to the intervals.
			Interval dummy;
			foreach(CommandNode commandNode in commandNodes) {
				// (TryGetValue is faster than ContainsKey)
				if(!intervalTimes.TryGetValue(commandNode.StartTime, out dummy)) {
					commandNode.StartTime = _FindClosest(orderedTimes, commandNode.StartTime);
				}
				if(!intervalTimes.TryGetValue(commandNode.EndTime, out dummy)) {
					commandNode.TimeSpan = _FindClosest(orderedTimes, commandNode.EndTime) - commandNode.StartTime;
				}
			}
		}

		private int _FindClosest(List<int> times, int time) {
			int index;
			int before, after;

			// Start by finding the index of the first time that exceeds it.
			index = times.FindIndex(x => x > time);
			// If nothing exceeds it, return the last value.
			if(index == -1) return times.Last();
			// If it's the first value that exceeds it, return the first value.
			if(index == 0) return times.First();
			// Get the value and the prior value.
			after = times[index];
			before = times[index - 1];
			// Is the time closer to the prior value or the one that exceeds it?
			if((after - before) / 2 > time) return before;
			return after;
		}

		private InputChannel _GetInputChannel(Guid channelId) {
			return _inputChannels.Where(x => x.Id == channelId).FirstOrDefault();
		}

		private void _AddCommandsWithSyncInternal(InputChannel channel, IEnumerable<CommandNode> data) {
			// If there are intervals, the sync will sync the data.  If there aren't, it won't.
			if(channel != null) {
				if(_intervals.Count > 0) {
					_QuantizeCommands(data, _intervals);
				}
				channel.AddData(data);
			}
		}

		#endregion
	}
}
