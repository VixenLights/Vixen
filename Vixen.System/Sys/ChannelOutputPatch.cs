using System;
using System.Collections.Generic;

namespace Vixen.Sys {
	public class ChannelOutputPatch : HashSet<ControllerReference> {// IEnumerable<ControllerReference> {
		//private HashSet<ControllerReference> _controllerReferences;

		public ChannelOutputPatch(Guid channelId) {
			ChannelId = channelId;
			//_controllerReferences = new HashSet<ControllerReference>();
		}

		public ChannelOutputPatch(Guid channelId, IEnumerable<ControllerReference> controllerReferences)
			: this(channelId) {
			//_controllerReferences.AddRange(controllerReferences);
			this.AddRange(controllerReferences);
		}

		public Guid ChannelId { get; private set; }

		//public IEnumerable<ControllerReference> ControllerReferences {
		//    get { return _controllerReferences; }
		//    set { _controllerReferences = new HashSet<ControllerReference>(value); }
		//}

		//public void Add(ControllerReference controllerReference) {
		//    _controllerReferences.Add(controllerReference);
		//}

		//public void Remove(ControllerReference controllerReference) {
		//    _controllerReferences.Remove(controllerReference);
		//}

		public void Remove(Guid controllerId) {
			//_controllerReferences.RemoveWhere(x => x.ControllerId == controllerId);
			RemoveWhere(x => x.ControllerId == controllerId);
		}

		//public void Clear() {
		//    _controllerReferences.Clear();
		//}

		//public IEnumerator<ControllerReference> GetEnumerator() {
		//    return _controllerReferences.GetEnumerator();
		//}

		//System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
		//    return GetEnumerator();
		//}
	}
}
