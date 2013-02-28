using System;
using System.Collections.Generic;
using Vixen.Sys;
using Vixen.Module;
using Vixen.Module.RuntimeBehavior;

namespace Recording {
	public class RecordingModule : RuntimeBehaviorModuleInstanceBase {
		private Tuple<string, Action>[] _actions;
		private ISequence _sequence;
		private List<IEffectNode> _buffer = new List<IEffectNode>();
		private int _bufferItems;
		//private Guid _sequenceChannelId = Guid.Empty;
		private DataStream _recordingStream;
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
				//// Get a channel into the sequence's input channels.
				//Guid sequenceChannelId = _GetSequenceChannelId();
				//// Clear it.
				//_sequence.SequenceData.DataStreams.ClearStream(sequenceChannelId);
				//// Copy our buffered data to it.
				//_sequence.SequenceData.DataStreams.AddData(sequenceChannelId, _buffer);
				// Get the data stream for the recorded data.
				DataStream recordingStream = _GetRecordingStream();
				// Clear it.
				recordingStream.Clear();
				// Copy our buffered data to it.
				recordingStream.AddData(_buffer);
			}
		}

		private DataStream _GetRecordingStream() {
			if(_recordingStream == null) {
				_recordingStream = _sequence.SequenceData.DataStreams.CreateStream("Recording");
			}
			return _recordingStream;
		}

		//private Guid _GetSequenceChannelId() {
		//    if(_sequenceChannelId == Guid.Empty) {
		//        _sequenceChannelId = _sequence.SequenceData.DataStreams.CreateStream("Recording");
		//    }
		//    return _sequenceChannelId;
		//}

		override public void Handle(IEffectNode effectNode) {
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
			//_RemoveSequenceChannel();
			_RemoveRecordingStream();
			_buffer.Clear();
		}

		public void Commit() {
			//_RemoveSequenceChannel();
			_RemoveRecordingStream();
			_sequence.InsertData(_buffer);
			_buffer.Clear();
		}

		//private void _RemoveSequenceChannel() {
		//    if(_sequenceChannelId != Guid.Empty) {
		//        _sequence.DataStreams.RemoveStream(_sequenceChannelId);
		//        _sequenceChannelId = Guid.Empty;
		//    }
		//}
		private void _RemoveRecordingStream() {
			if(_recordingStream != null) {
				_sequence.SequenceData.DataStreams.RemoveStream(_recordingStream);
				_recordingStream = null;
			}
		}

		// If they want to preview (neither action taken), the input channel remains
		// (so execution will grab the data) and the buffer remains (so that they can add to it).
	}
}
