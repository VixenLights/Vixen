using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Hardware;
using Vixen.Commands;

namespace Vixen.Sys {
    public class Patch : IDisposable, IOutputStateSource, IEnumerable<ControllerReference> {
        private HashSet<ControllerReference> _controllerReferences = new HashSet<ControllerReference>();
		private Command _state = null;

        public Patch() {
            Enabled = true;
        }

		public Patch(Guid controllerId, int outputIndex)
			: this() {
			Add(controllerId, outputIndex);
		}

		public Patch(Patch sourcePatch)
			: this() {
			Add(sourcePatch);
		}

		public Patch(IEnumerable<ControllerReference> controllerReferences)
			: this() {
			Add(controllerReferences);
		}

        public bool Enabled { get; set; }

        public void Add(Guid controllerId, int outputIndex) {
			_Add(new ControllerReference(controllerId, outputIndex));
        }

		public void Add(ControllerReference controllerReference) {
			_Add(controllerReference);
		}

		public void Add(IEnumerable<ControllerReference> controllerReferences) {
			foreach(ControllerReference controllerReference in controllerReferences) {
				_Add(controllerReference);
			}
		}

		private void _Add(ControllerReference controllerReference) {
			// _controllerReferences is a HashSet, not a list.
			// Going to allow invalid references to remain.  The controller may be back later.
			//if(Hardware.OutputController.IsValidReference(controllerReference) && _controllerReferences.Add(controllerReference)) {
			if(_controllerReferences.Add(controllerReference)) {
				OutputController.AddSource(this, controllerReference);
			}
		}

		public void Remove(ControllerReference controllerReference) {
			_Remove(controllerReference);
		}

		public void Remove(Guid controllerId, int outputIndex) {
			ControllerReference controllerReference = new ControllerReference(controllerId, outputIndex);
			_Remove(controllerReference);
        }

		private void _Remove(ControllerReference controllerReference) {
			_controllerReferences.Remove(controllerReference);
			OutputController.RemoveSource(this, controllerReference);
		}

        public void Clear() {
			foreach(ControllerReference controllerReference in _controllerReferences.ToArray()) {
				_Remove(controllerReference);
			}
		}

		// This is of Patch, not of the IOutputStateSource interface.
		public void Write(Command command) {
			SourceState = command;
		}

		// This is of the IOutputStateSource interface, not of Patch.
		public Command SourceState {
			get {
				if(Enabled) {
					return _state;
				}
				return null;
			}
			set {
				if(Enabled) {
					_state = value;
				}
			}
		}

        public IEnumerable<ControllerReference> ControllerReferences {
            get { return _controllerReferences; }
        }

		public Patch Clone() {
			Patch other = new Patch(this);
			return other;
		}

		public IEnumerator<ControllerReference> GetEnumerator() {
			return _controllerReferences.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		public void Dispose() {
			Clear();
			GC.SuppressFinalize(this);
		}

		~Patch() {
			Clear();
		}
	}
}
