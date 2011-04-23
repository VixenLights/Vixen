using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Vixen.Common {
    public class ControllerReference : IEquatable<ControllerReference> {
        public Guid ControllerId;
        public int OutputIndex;

		public ControllerReference(Guid controllerId, int outputIndex) {
			ControllerId = controllerId;
			OutputIndex = outputIndex;
		}

		static public XElement WriteXml(ControllerReference controllerReference) {
			XElement element = new XElement("ControllerReference",
				new XAttribute("controllerId", controllerReference.ControllerId),
				new XAttribute("outputIndex", controllerReference.OutputIndex));
			return element;
		}

		static public ControllerReference ReadXml(XElement element) {
			return new ControllerReference(
				new Guid(element.Attribute("controllerId").Value),
				int.Parse(element.Attribute("outputIndex").Value)
				);
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
    }
}
