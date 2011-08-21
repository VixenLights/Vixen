using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Vixen.Sys;

// Data is only pulled from this at execution start.
// Data written during sequence execution is handled by runtime behaviors and is written
// to the system for immediate execution at the appropriate time.

namespace Vixen.Sys {
	public class InputChannels {
		private ISequence _owner;
		// Data in these channels is pulled only at execution start.
		// There will always be at least one input channel.
		private List<InputChannel> _inputChannels = new List<InputChannel>();
		private InputChannel _mainChannel;
		private CommandNodeCollection _executingData = null;

		public InputChannels(ISequence owner) {
			_owner = owner;
			// The sequence will have at least one input channel to hold command data.
			_mainChannel = new InputChannel("Main");
			_inputChannels.Add(_mainChannel);
		}

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
		private InputChannel _GetInputChannel(Guid channelId) {
			return _inputChannels.Where(x => x.Id == channelId).FirstOrDefault();
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
