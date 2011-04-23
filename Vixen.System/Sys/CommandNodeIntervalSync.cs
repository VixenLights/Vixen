using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Vixen.Common;

// Data is only pulled from this at execution start.
// Data written during sequence execution is handled by runtime behaviors and is written
// to the system for immediate execution at the appropriate time.

namespace Vixen.Sys {
	public class CommandNodeIntervalSync {
		private ISequence _owner;
		// Data in these channels is pulled only at execution start.
		// There will always be at least one input channel.
		private List<InputChannel> _inputChannels = new List<InputChannel>();
		private IntervalCollection _intervals;
		private long _intervalValue;
		private InputChannel _mainChannel;

		public CommandNodeIntervalSync(ISequence owner, List<InputChannel> inputChannels, IntervalCollection intervals) {
			_owner = owner;
			_intervals = intervals;
			// The sequence will have at least one input channel to hold command data.
			if(inputChannels.Count == 0) {
				inputChannels.Add(new InputChannel("Main"));
			}
			_inputChannels.AddRange(inputChannels);
			_mainChannel = _inputChannels[0];
		}

		#region Intervals
		public long TimingInterval {
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

		public IEnumerable<long> IntervalValues {
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
		public IEnumerable<long> GetIntervalRange(long startTime, long endTime) {
			return _intervals.GetSpanTimes(startTime, endTime - startTime);
		}

		public void AdjustInterval(long intervalTime, long newIntervalTime) {
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

		public void InsertInterval(long time) {
			_intervals.Insert(time);
		}

		public void InsertIntervals(IEnumerable<long> times) {
			_intervals.InsertRange(times);
		}

		/// <summary>
		/// Adds intervals to the end of the collection.
		/// </summary>
		/// <param name="timeSpan"></param>
		/// <param name="interval"></param>
		public void AddIntervals(long timeSpan, long interval) {
			_intervals.Add(timeSpan, interval);
		}

		public void RemoveInterval(long time) {
			_intervals.Remove(time);
		}

		public void RemoveIntervals(long startTime, long endTime) {
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

		// During authoring, as data is written to the sequence, it is stored in the
		// sequence's main input channel.
		public void AddCommand(CommandNode data) {
			_mainChannel.AddData(data);
		}

		// Not currently used.
		public void AddCommand(Guid channelId, CommandNode data) {
			InputChannel channel = _inputChannels.Where(x => x.Id == channelId).FirstOrDefault();
			if(channel != null) {
				channel.AddData(data);
			}
		}

		// Why isn't this used during authoring instead of AddCommand(data)?
		public void AddCommandWithSync(CommandNode data) {
			_AddCommandsWithSyncInternal(_mainChannel, new CommandNode[] { data });
		}

		// Not currently used.
		public void AddCommandWithSync(Guid channelId, CommandNode data) {
			// If there are intervals, the sync will sync the data.  If there aren't, it won't.
			InputChannel channel = _GetInputChannel(channelId);
			_AddCommandsWithSyncInternal(channel, new CommandNode[] { data });
		}

		// Used by the recording behavior.
		public void AddCommands(IEnumerable<CommandNode> data) {
			_mainChannel.AddData(data);
		}

		// Not currently used.
		public void AddCommands(Guid channelId, IEnumerable<CommandNode> data) {
			InputChannel channel = _inputChannels.Where(x => x.Id == channelId).FirstOrDefault();
			if(channel != null) {
				channel.AddData(data);
			}
		}

		// Not currently used.
		public void AddCommandsWithSync(IEnumerable<CommandNode> data) {
			// If there are intervals, the sync will sync the data.  If there aren't, it won't.
			_AddCommandsWithSyncInternal(_mainChannel, data);
		}

		// Used by the recording behavior.
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
		/// <returns>CommandNodes that overlap the time range, in StartTime order.</returns>
		public IEnumerable<CommandNode> GetCommandRange(long startTime, long endTime) {
			// Need any data that starts or ends within the time range.

			// Need to return the data in a concurrent collection with a live
			// enumerator so that scripts can dynamically add the effects they
			// generate.
			IEnumerable<CommandNode> data =
				_inputChannels
					.SelectMany(x => x.Where(y =>
						(y.StartTime >= startTime && y.StartTime < endTime) ||
						(y.EndTime >= startTime && y.EndTime < endTime))
						)
					.OrderBy(x => x.StartTime);
			_executingData = new CommandNodeCollection(data);
			return _executingData;
		}
		private CommandNodeCollection _executingData = null;

		/// <summary>
		/// If a sequence is executing, data written to a sequence by way of this
		/// method will be considered by the effect enumerator.
		/// </summary>
		/// <param name="data"></param>
		public void AddLive(IEnumerable<CommandNode> data) {
			if(_executingData != null) {
				_executingData.AddRange(data);
			}
		}

		/// <summary>
		/// If a sequence is executing, data written to a sequence by way of this
		/// method will be considered by the effect enumerator.
		/// </summary>
		/// <param name="data"></param>
		public void AddLive(CommandNode data) {
			if(_executingData != null) {
				_executingData.Add(data);
			}
		}
		#endregion

		#region Input Channels
		public Guid CreateInputChannel(string name) {
			InputChannel channel = new InputChannel(name);
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
			Dictionary<long, Interval> intervalTimes = intervals.ToDictionary(x => x.Time, x => x);
			// Also create a list of ordered interval times.
			List<long> orderedTimes = intervalTimes.Keys.OrderBy(x => x).ToList();
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

		private long _FindClosest(List<long> times, long time) {
			if(times.Count == 0) return time;

			int index;
			long before, after;

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

		#region CommandNodeCollection
		class CommandNodeCollection : IEnumerable<CommandNode> {
			private ConcurrentQueue<CommandNode> _data;

			public CommandNodeCollection(IEnumerable<CommandNode> commandNodes) {
				_data = new ConcurrentQueue<CommandNode>(commandNodes);
			}

			public void Add(CommandNode value) {
				_data.Enqueue(value);
			}

			public void AddRange(IEnumerable<CommandNode> values) {
				foreach(CommandNode value in values) {
					_data.Enqueue(value);
				}
			}

			public IEnumerator<CommandNode> GetEnumerator() {
				// We need an enumerator that is live and does not operate upon a snapshot
				// of the data.
				return new ConcurrentQueueLiveEnumerator<CommandNode>(_data);
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
		}
		#endregion
	}
}
