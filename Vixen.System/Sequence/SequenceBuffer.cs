using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Hardware;
using Vixen.Common;
using Vixen.Module.Sequence;

namespace Vixen.Sequence {
	public class SequenceBuffer : IDisposable {
		private OutputController[] _controllers;
		private List<ControllerOutputUpdate> _inBuffer = new List<ControllerOutputUpdate>();
		// Need to have unique output references in a set.
		private Dictionary<ControllerReference, CommandData> _outBuffer = new Dictionary<ControllerReference, CommandData>();
		private ISequence _sequence;

		public SequenceBuffer(ISequence sequence, OutputController[] controllers) {
			_sequence = sequence;
			// Get a reference to the controllers that the sequence references for
			// notification purposes.
			_controllers = controllers;
			// For each controller to be referenced by this buffer, create a list of incoming and
			// outgoing updates.
			foreach(OutputController controller in controllers) {
				for(int i = 0; i < controller.OutputCount; i++) {
					// An output without a state will have CommandData.IsEmpty == true.
					_outBuffer[new ControllerReference(controller.Id, i)] = default(CommandData);
				}
			}
		}

		public void BeginUpdate() {
			if(IsDisposed) throw new ObjectDisposedException(null);

			lock(_inBuffer) {
				_inBuffer.Clear();
			}
		}

		public void AddUpdate(ControllerReference controllerReference, CommandData command) {
			if(IsDisposed) throw new ObjectDisposedException(null);

			// Add to the update buffer.
			lock(_inBuffer) { //vs enumerating a copy of the collection
				_inBuffer.Add(new ControllerOutputUpdate() { ControllerReference = controllerReference, Command = command });
			}
		}

		public void EndUpdate() {
			if(IsDisposed) throw new ObjectDisposedException(null);

			// Copy from the update the out buffer state.
			// In buffer is only referenced by the sequence executor, but both that and
			// controllers make use of the out buffer.
			lock(_inBuffer) {
				// Set states with values.
				//TODO: Can this be parallelized?
				foreach(ControllerOutputUpdate update in _inBuffer) {
					_outBuffer[update.ControllerReference] = update.Command;
				}
				_inBuffer.Clear();
			}

			// Notify the controllers that there is a new state.
			foreach(OutputController controller in _controllers) {
				controller.UpdateFrom(this);
			}
		}

		public Tuple<ControllerReference,CommandData>[] GetControllerState(Guid controllerId) {
			if(IsDisposed) throw new ObjectDisposedException(null);

			lock(_outBuffer) {
				return _outBuffer.Where(x => x.Key.ControllerId == controllerId).Select(x => new Tuple<ControllerReference, CommandData>(x.Key, x.Value)).ToArray();
			}
		}

		public void Dispose() {
			if(IsDisposed) throw new ObjectDisposedException(null);
			IsDisposed = true;
		}

		public bool IsDisposed { get; private set; }
	}
}
