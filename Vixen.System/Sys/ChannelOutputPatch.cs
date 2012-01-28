using System;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys {
	public class ChannelOutputPatch : IEnumerable<ControllerReference> {
		private List<ControllerReference> _controllerReferences;

		public ChannelOutputPatch(Guid channelId) {
			ChannelId = channelId;
			_controllerReferences = new List<ControllerReference>();
		}

		public ChannelOutputPatch(Guid channelId, IEnumerable<ControllerReference> controllerReferences)
			: this(channelId) {
			_controllerReferences.AddRange(controllerReferences);
		}

		public Guid ChannelId { get; private set; }

		public IEnumerable<ControllerReference> ControllerReferences {
			get { return _controllerReferences; }
			set { _controllerReferences = value.ToList(); }
		}

		public void Add(ControllerReference controllerReference) {
			if(!_controllerReferences.Contains(controllerReference)) {
				_controllerReferences.Add(controllerReference);
			}
		}

		public void Remove(ControllerReference controllerReference) {
			_controllerReferences.Remove(controllerReference);
		}

		public void Remove(Guid controllerId) {
			_controllerReferences.RemoveAll(x => x.ControllerId == controllerId);
		}

		public void Clear() {
			_controllerReferences.Clear();
		}

		public IEnumerator<ControllerReference> GetEnumerator() {
			return _controllerReferences.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
