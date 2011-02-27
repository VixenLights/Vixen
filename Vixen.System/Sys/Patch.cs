using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Common;
using Vixen.Sequence;

namespace Vixen.Sys {
    public class Patch {
        private HashSet<ControllerReference> _controllerReferences = new HashSet<ControllerReference>();

        public Patch() {
            Enabled = true;
        }

        public bool Enabled { get; set; }

        public void Add(Guid controllerId, int outputIndex) {
			ControllerReference controllerReference = new ControllerReference(controllerId, outputIndex);
            _controllerReferences.Add(controllerReference);
        }

        public void Remove(Guid controllerId, int outputIndex) {
			ControllerReference controllerReference = new ControllerReference(controllerId, outputIndex);
            _controllerReferences.Remove(controllerReference);
        }

        public void Clear() {
            _controllerReferences.Clear();
        }

        public void Write(CommandData command, SequenceBuffer updateBuffer) {
            if(Enabled) {
                foreach(ControllerReference controllerReference in _controllerReferences) {
                    updateBuffer.AddUpdate(controllerReference, command);
                }
            }
        }

        public IEnumerable<ControllerReference> ControllerReferences {
            get { return _controllerReferences; }
        }

		public Patch Clone() {
			Patch other = new Patch();
			other._controllerReferences = new HashSet<ControllerReference>(
				this._controllerReferences.Select(x => new ControllerReference(x.ControllerId, x.OutputIndex))
				);
			return other;
		}
    }
}
