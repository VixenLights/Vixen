using System;
using System.Collections.Generic;

namespace Vixen.Sys.Managers {
	public class ChannelOutputPatchManager : IEnumerable<ChannelOutputPatch> {
		private Dictionary<Guid, ChannelOutputPatch> _instances;

		public ChannelOutputPatchManager() {
			_instances = new Dictionary<Guid, ChannelOutputPatch>();
		}

		public void AddPatches(IEnumerable<ChannelOutputPatch> patches) {
			foreach(ChannelOutputPatch patch in patches) {
				AddPatch(patch.ChannelId, patch);
			}
		}

		public void AddPatches(Guid channelId, IEnumerable<ControllerReference> controllerReferences) {
			ChannelOutputPatch channelPatch = _GetChannelPatch(channelId);
			foreach(ControllerReference controllerReference in controllerReferences) {
				channelPatch.Add(controllerReference);
			}
		}

		public void AddPatch(Guid channelId, ControllerReference controllerReference) {
			ChannelOutputPatch channelPatch = _GetChannelPatch(channelId);
			channelPatch.Add(controllerReference);
		}

		public void AddPatch(Guid channelId, ChannelOutputPatch patch) {
			_instances[channelId] = patch;
		}

		public void ClearPatching(Guid channelId) {
			ChannelOutputPatch channelPatch = _GetChannelPatch(channelId);
			channelPatch.Clear();
			_instances.Remove(channelId);
		}

		public void RemovePatch(Guid channelId, ControllerReference controllerReference) {
			ChannelOutputPatch channelPatch = _GetChannelPatch(channelId);
			channelPatch.Remove(controllerReference);
		}

		public ChannelOutputPatch GetChannelPatch(Guid channelId) {
			return _GetChannelPatch(channelId);
		}

		public IEnumerator<ChannelOutputPatch> GetEnumerator() {
			return _instances.Values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}

		private ChannelOutputPatch _GetChannelPatch(Guid channelId) {
			ChannelOutputPatch channelPatch;

			if(!_instances.TryGetValue(channelId, out channelPatch)) {
				channelPatch = new ChannelOutputPatch(channelId);
				_instances[channelId] = channelPatch;
			}

			return channelPatch;
		}
	}
}
