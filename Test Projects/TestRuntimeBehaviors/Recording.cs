using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;
using Vixen.Module.Timing;

namespace TestRuntimeBehaviors {
	public class Recording : RuntimeBehaviorModuleInstanceBase {
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

		override public void Startup(ISequence sequence, ITiming timingSource) {
			_sequence = sequence;
			_timingSource = timingSource;

			_bufferItems = _buffer.Count;
		}

		override public void Shutdown() {
			// If data in the buffer has changed...
			if(_bufferItems != _buffer.Count) {
				// Get a channel into the sequence's input channels.
				Guid sequenceChannelId = _GetSequenceChannelId();
				// Clear it.
				_sequence.Data.ClearInputChannel(sequenceChannelId);
				// Copy our buffered data to it.
				_sequence.Data.AddCommands(sequenceChannelId, _buffer);
			}
		}

		private Guid _GetSequenceChannelId() {
			if(_sequenceChannelId == Guid.Empty) {
				_sequenceChannelId = _sequence.Data.CreateInputChannel("Recording");
			}
			return _sequenceChannelId;
		}

		override public void Handle(CommandNode commandNode) {
			_buffer.Add(commandNode);
		}

		override public bool Enabled {
			get { return _moduleData.Enabled; }
			set { _moduleData.Enabled = value; }
		}

		override public Tuple<string, Action>[] BehaviorActions {
			get { return _actions; }
		}

		override public IModuleDataModel ModuleData { 
			get { return _moduleData; }
			set { _moduleData = value as RecordingData; }
		}


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
