using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;
//using Vixen.Module.Sequence;
using Vixen.Module.Timing;

namespace TestRuntimeBehaviors {
	public class Recording : IRuntimeBehaviorModuleInstance {
		private Tuple<string, Action>[] _actions;
		private ISequence _sequence;
		private ITiming _timingSource;
		private List<CommandNode> _buffer = new List<CommandNode>();
		private int _bufferItems;
		private Guid _sequenceChannelId = Guid.Empty;
		private RecordingData _moduleData;

		public Recording() {
			_actions = new Tuple<string, Action>[] {
				new Tuple<string,Action>("Clear buffer", Clear),
				new Tuple<string,Action>("Commit", Commit)
			};
		}

		public void Startup(ISequence sequence, ITiming timingSource) {
			_sequence = sequence;
			_timingSource = timingSource;

			_bufferItems = _buffer.Count;
		}

		public void Shutdown() {
			// If data in the buffer has changed...
			if(_bufferItems != _buffer.Count) {
				// Get a channel into the sequence's input channels.
				Guid sequenceChannelId = _GetSequenceChannelId();
				// Clear it.
				_sequence.Data.ClearInputChannel(sequenceChannelId);
				// Copy our buffered data to it.
				_sequence.Data.AddCommandsWithSync(sequenceChannelId, _buffer);
			}
		}

		private Guid _GetSequenceChannelId() {
			if(_sequenceChannelId == Guid.Empty) {
				_sequenceChannelId = _sequence.Data.CreateInputChannel("Recording");
			}
			return _sequenceChannelId;
		}

		////public IEnumerable<CommandNode> GenerateCommandNodes(InsertDataParameters parameters) {
		//public void GenerateCommandNodes(InsertDataParameters parameters) {
		//    CommandNode commandNode = new CommandNode(parameters.Command, parameters.Channels, _timingSource.Position, parameters.TimeSpan);
		//    //commandNode.IsRequired = true;
		//    _buffer.Add(commandNode);
			
		//    //// Put nothing into the sequence.
		//    //return Enumerable.Empty<CommandNode>();
		//}
		public void Handle(CommandNode commandNode) {
			_buffer.Add(commandNode);
		}

		public bool Enabled {
			get { return _moduleData.Enabled; }
			set { _moduleData.Enabled = value; }
		}

		public Tuple<string, Action>[] BehaviorActions {
			get { return _actions; }
		}

		public Guid TypeId {
			get { return RecordingModule._typeId; }
		}

		public Guid InstanceId { get; set; }

		public IModuleDataModel ModuleData { 
			get { return _moduleData; }
			set { _moduleData = value as RecordingData; }
		}

		public string TypeName { get; set; }

		public void Dispose() { }


		// ------- Actions -------
		public void Clear() {
			_RemoveSequenceChannel();
			_buffer.Clear();
		}

		public void Commit() {
			_RemoveSequenceChannel();
			_sequence.Data.AddCommands(_buffer);
			_buffer.Clear();
		}

		private void _RemoveSequenceChannel() {
			if(_sequenceChannelId != Guid.Empty) {
				_sequence.Data.RemoveInputChannel(_sequenceChannelId);
				_sequenceChannelId = Guid.Empty;
			}
		}

		// If they want to preview (neither action taken), the input channel remains
		// (so execution will grab the data) and the buffer remains (so that they can add to it).
	}
}
