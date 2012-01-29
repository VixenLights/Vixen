using System;
using System.Collections.Generic;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;

namespace Recording {
	public class RecordingModule : RuntimeBehaviorModuleInstanceBase {
		private Tuple<string, Action>[] _actions;
		private ISequence _sequence;
		private List<EffectNode> _buffer = new List<EffectNode>();
		private int _bufferItems;
		private Guid _sequenceChannelId = Guid.Empty;
		private RecordingData _moduleData;

		public RecordingModule() {
			_actions = new[] {
				new Tuple<string,Action>("Clear buffer", Clear),
				new Tuple<string,Action>("Commit", Commit)
			};
		}

		override public void Startup(ISequence sequence) {
			_sequence = sequence;
			_bufferItems = _buffer.Count;
		}

		override public void Shutdown() {
			// If data in the buffer has changed...
			if(_bufferItems != _buffer.Count) {
				// Get a channel into the sequence's input channels.
				Guid sequenceChannelId = _GetSequenceChannelId();
				// Clear it.
				_sequence.Data.ClearStream(sequenceChannelId);
				// Copy our buffered data to it.
				_sequence.Data.AddData(sequenceChannelId, _buffer);
			}
		}

		private Guid _GetSequenceChannelId() {
			if(_sequenceChannelId == Guid.Empty) {
				_sequenceChannelId = _sequence.Data.CreateStream("Recording");
			}
			return _sequenceChannelId;
		}

		override public void Handle(EffectNode effectNode) {
			_buffer.Add(effectNode);
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
			_sequence.InsertData(_buffer);
			_buffer.Clear();
		}

		private void _RemoveSequenceChannel() {
			if(_sequenceChannelId != Guid.Empty) {
				_sequence.Data.RemoveStream(_sequenceChannelId);
				_sequenceChannelId = Guid.Empty;
			}
		}

		// If they want to preview (neither action taken), the input channel remains
		// (so execution will grab the data) and the buffer remains (so that they can add to it).
	}
}
