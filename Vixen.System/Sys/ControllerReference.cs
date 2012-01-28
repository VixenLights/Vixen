using System;

namespace Vixen.Sys {
    public class ControllerReference : IEquatable<ControllerReference> {
    	public ControllerReference(Guid controllerId, int outputIndex) {
			ControllerId = controllerId;
			OutputIndex = outputIndex;
		}

		public Guid ControllerId { get; private set; }
		public int OutputIndex { get; private set; }

		//public override int GetHashCode() {
		//    return (ControllerId.ToString() + OutputIndex).GetHashCode();
		//}

		//public override bool Equals(object obj) {
		//    if(obj is ControllerReference) {
		//        return GetHashCode() == obj.GetHashCode();
		//    }
		//    return base.Equals(obj);
		//}

		//public bool Equals(ControllerReference other) {
		//    return this.GetHashCode() == other.GetHashCode();
		//}

		public override string ToString() {
			// make the index human-friendly -- index it from 1.
			return ToString(false);
		}

		public string ToString(bool indexFromZero) {
			OutputController controller = VixenSystem.Controllers.Get(ControllerId);
			string controllerName = (controller != null) ?
					controller.Name :
					"(Unknown)";

			int indexOffset = indexFromZero ? 0 : 1;
			return controllerName + " [" + (OutputIndex + indexOffset) + "]";
		}

    	public bool Equals(ControllerReference other) {
    		if(ReferenceEquals(null, other)) return false;
    		if(ReferenceEquals(this, other)) return true;
    		return other.ControllerId.Equals(ControllerId) && other.OutputIndex == OutputIndex;
    	}

    	public override bool Equals(object obj) {
    		if(ReferenceEquals(null, obj)) return false;
    		if(ReferenceEquals(this, obj)) return true;
    		if(obj.GetType() != typeof(ControllerReference)) return false;
    		return Equals((ControllerReference)obj);
    	}

    	public override int GetHashCode() {
    		unchecked {
    			return (ControllerId.GetHashCode()*397) ^ OutputIndex;
    		}
    	}
    }
}
