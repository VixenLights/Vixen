using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys.Output;

//*** use this when changing patching in the UI for any reason; get rid of reloads on cancel
namespace Vixen.Sys {
	class ChannelPatchingChangeSet {
		//Could also keep a copy of the original state and as patches are added,removed,cleared,
		//see if it matches the original state and updated the hashset based on that.
		//private Dictionary<Guid, HashSet<ControllerReference>> _channelPatches;
		private Dictionary<Guid, ChannelOutputPatch> _channelPatches;
		private HashSet<Guid> _changedChannels;

		public ChannelPatchingChangeSet() {
			_channelPatches = new Dictionary<Guid, ChannelOutputPatch>();
			_changedChannels = new HashSet<Guid>();
		}

		public void AddChannelPatches(Guid channelId, IEnumerable<ControllerReference> controllerReference) {
			_GetChannelPatches(channelId).AddRange(controllerReference);
			_AddChangedChannel(channelId);
		}

		public void AddChannelPatch(Guid channelId, ControllerReference controllerReference) {
			_GetChannelPatches(channelId).Add(controllerReference);
			_AddChangedChannel(channelId);
		}

		public void RemoveChannelPatch(Guid channelId, ControllerReference controllerReference) {
			_GetChannelPatches(channelId).Remove(controllerReference);
			_AddChangedChannel(channelId);
		}

		public void ClearChannelPatches(Guid channelId) {
			_GetChannelPatches(channelId).Clear();
			_AddChangedChannel(channelId);
		}

		public void Commit(bool clearExisting = false) {
			// Don't want to know about outputs affected while patches are being thrown about.
			// Only want to know about outputs affected by:
			// 1: The original patches of the affected channels.
			// 2: The new patches of the affected channels.
			OutputSourceChangeSet outputSourceChangeSet = new OutputSourceChangeSet();
			Dictionary<Guid, ChannelOutputPatch> originalChannelPatches = VixenSystem.ChannelPatching.ToDictionary(x => x.ChannelId, x => x);

			IEnumerable<ControllerReference> originalReferences = _changedChannels.SelectMany(x => originalChannelPatches[x].Select(y => y));
			IEnumerable<ControllerReference> newReferences = _changedChannels.SelectMany(x => _channelPatches[x].Select(y => y));
			IEnumerable<ControllerReference> allControllerReferences = originalReferences.Concat(newReferences);

			IEnumerable<OutputController> referencedControllers = VixenSystem.Controllers.GetControllers(allControllerReferences).ToArray();
			VixenSystem.Controllers.Pause(referencedControllers);
			
			foreach(Guid channelId in _changedChannels) {
				_AffectOutputs(originalChannelPatches[channelId], outputSourceChangeSet);
				_CommitPatches(channelId, clearExisting);
				_AffectOutputs(_channelPatches[channelId], outputSourceChangeSet);
			}

			VixenSystem.Controllers.Resume(referencedControllers);

			outputSourceChangeSet.Commit();
		}

		private void _CommitPatches(Guid channelId, bool clearExisting) {
			if(clearExisting) {
				VixenSystem.ChannelPatching.ClearPatching(channelId);
			}
			VixenSystem.ChannelPatching.AddPatches(channelId, _channelPatches[channelId]);
		}

		private void _AffectOutputs(IEnumerable<ControllerReference> controllerReferences, OutputSourceChangeSet outputSourceChangeSet) {
			foreach(ControllerReference controllerReference in controllerReferences) {
				outputSourceChangeSet.AffectOutput(controllerReference);
			}
		}

		private void _AddChangedChannel(Guid channelId) {
			_changedChannels.Add(channelId);
		}

		private HashSet<ControllerReference> _GetChannelPatches(Guid channelId) {
			ChannelOutputPatch channelPatches;

			if(!_channelPatches.TryGetValue(channelId, out channelPatches)) {
				channelPatches = new ChannelOutputPatch(channelId);
				_channelPatches[channelId] = channelPatches;
			}

			return channelPatches;
		}
	}
}
