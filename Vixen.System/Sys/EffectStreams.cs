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
	public class EffectStreams {
		private ISequence _owner;
		// Data in these streams is pulled only at execution start.
		// There will always be at least one effect stream.
		private List<EffectStream> _effectStreams = new List<EffectStream>();
		private EffectStream _mainStream;
		private CommandNodeCollection _executingData = null;

		public EffectStreams(ISequence owner) {
			_owner = owner;
			// The sequence will have at least one effect stream to hold command data.
			_mainStream = new EffectStream("Main");
			_effectStreams.Add(_mainStream);
		}

		#region CommandNodes
		public IEnumerable<EffectNode> GetCommands() {
			return _mainStream;
		}

		public IEnumerable<EffectNode> GetCommands(Guid streamId) {
			return _effectStreams.Where(x => x.Id == streamId).FirstOrDefault();
		}

		// During authoring, as data is written to the sequence, it is stored in the
		// sequence's main effect stream.
		public void AddCommand(EffectNode data) {
			_mainStream.AddData(data);
		}

		// Not currently used.
		public void AddCommand(Guid streamId, EffectNode data) {
			EffectStream effectStream = _effectStreams.Where(x => x.Id == streamId).FirstOrDefault();
			if(effectStream != null) {
				effectStream.AddData(data);
			}
		}

		// Used by the recording behavior.
		public void AddCommands(IEnumerable<EffectNode> data) {
			_mainStream.AddData(data);
		}

		// Not currently used.
		public void AddCommands(Guid streamId, IEnumerable<EffectNode> data) {
			EffectStream effectStream = _effectStreams.Where(x => x.Id == streamId).FirstOrDefault();
			if(effectStream != null) {
				effectStream.AddData(data);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime">Inclusive.</param>
		/// <param name="endTime">Exclusive.</param>
		/// <returns>CommandNodes that overlap the time range, in StartTime order.</returns>
		public IEnumerable<EffectNode> GetCommandRange(TimeSpan startTime, TimeSpan endTime) {
			// Need any data that starts or ends within the time range.

			// Need to return the data in a concurrent collection with a live
			// enumerator so that scripts can dynamically add the effects they
			// generate.
			IEnumerable<EffectNode> data =
				_effectStreams
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
		public void AddLive(IEnumerable<EffectNode> data) {
			if(_executingData != null) {
				_executingData.AddRange(data);
			}
		}

		/// <summary>
		/// If a sequence is executing, data written to a sequence by way of this
		/// method will be considered by the effect enumerator.
		/// </summary>
		/// <param name="data"></param>
		public void AddLive(EffectNode data) {
			if(_executingData != null) {
				_executingData.Add(data);
			}
		}
		#endregion

		#region Effect Streams
		public Guid CreateEffectStream(string name) {
			EffectStream effectStream = new EffectStream(name);
			_effectStreams.Add(effectStream);
			return effectStream.Id;
		}

		public void ClearEffectStream(Guid effectStreamId) {
			//*** dictionary!
			EffectStream effectStream = _GetEffectStream(effectStreamId);
			if(effectStream != null) {
				effectStream.Clear();
			}
		}

		public void RemoveEffectStream(Guid effectStreamId) {
			EffectStream effectStream = _GetEffectStream(effectStreamId);
			if(effectStream != null) {
				_effectStreams.Remove(effectStream);
			}
		}
		#endregion

		private EffectStream _GetEffectStream(Guid effectStreamId) {
			return _effectStreams.Where(x => x.Id == effectStreamId).FirstOrDefault();
		}

		#region CommandNodeCollection
		class CommandNodeCollection : IEnumerable<EffectNode> {
			private ConcurrentQueue<EffectNode> _data;

			public CommandNodeCollection(IEnumerable<EffectNode> commandNodes) {
				_data = new ConcurrentQueue<EffectNode>(commandNodes);
			}

			public void Add(EffectNode value) {
				_data.Enqueue(value);
			}

			public void AddRange(IEnumerable<EffectNode> values) {
				foreach(EffectNode value in values) {
					_data.Enqueue(value);
				}
			}

			public IEnumerator<EffectNode> GetEnumerator() {
				// We need an enumerator that is live and does not operate upon a snapshot
				// of the data.
				return new ConcurrentQueueLiveEnumerator<EffectNode>(_data);
			}

			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return GetEnumerator();
			}
		}
		#endregion
	}
}
