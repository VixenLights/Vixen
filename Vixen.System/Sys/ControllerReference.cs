using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Vixen.Sys {
    public class ControllerReference : IEquatable<ControllerReference> {
        public Guid ControllerId;
        public int OutputIndex;

		public ControllerReference(Guid controllerId, int outputIndex) {
			ControllerId = controllerId;
			OutputIndex = outputIndex;
		}

        public override int GetHashCode() {
            return (ControllerId.ToString() + OutputIndex).GetHashCode();
        }

        public override bool Equals(object obj) {
            if(obj is ControllerReference) {
                return GetHashCode() == obj.GetHashCode();
            } else {
                return base.Equals(obj);
            }
        }

        public bool Equals(ControllerReference other) {
            return this.GetHashCode() == other.GetHashCode();
        }

		public override string ToString() {
			// make the index human-friendly -- index it from 1.
			return Vixen.Hardware.OutputController.Get(ControllerId).Name + " [" + (OutputIndex + 1) + "]";
		}
	}
}
