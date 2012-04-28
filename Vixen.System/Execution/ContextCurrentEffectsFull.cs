using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Execution {
	/// <summary>
	/// Maintains a list of current effects for a context.
	/// The IDataSource is expected to provide every qualifying effect, not just newly qualified effects.
	/// </summary>
	class ContextCurrentEffectsFull : IContextCurrentEffects {
		private List<IEffectNode> _currentEffects;
		private HashSet<Guid> _currentAffectedChannels;

		public ContextCurrentEffectsFull() {
			_currentEffects = new List<IEffectNode>();
			_currentAffectedChannels = new HashSet<Guid>();
		}

		/// <summary>
		/// Updates the collection of current affects, returning the ids of the affected channels.
		/// </summary>
		/// <returns>Ids of the affected channels.</returns>
		public Guid[] UpdateCurrentEffects(IDataSource dataSource, TimeSpan currentTime) {
			// Get the entirety of the new state.
			IEffectNode[] newState = dataSource.GetDataAt(currentTime).ToArray();
			// Get the channels affected by this new state.
			Guid[] nowAffectedChannels = _GetAffectedChannels(newState).ToArray();
			// New and expiring effects affect the state, so get the union of
			// the previous state and the current state.
			//HashSet<Guid> allAffectedChannels = new HashSet<Guid>(_currentAffectedChannels.Concat(newAffectedChannels));
			IEnumerable<Guid> allAffectedChannels = _currentAffectedChannels.Concat(nowAffectedChannels).Distinct();
			// Set the new state.
			_currentEffects = new List<IEffectNode>(newState);
			_currentAffectedChannels = new HashSet<Guid>(nowAffectedChannels);

			return allAffectedChannels.ToArray();
		}

		private IEnumerable<Guid> _GetAffectedChannels(IEnumerable<IEffectNode> effectNodes) {
			return effectNodes.SelectMany(x => x.Effect.TargetNodes).SelectMany(y => y.GetChannelEnumerator()).Select(z => z.Id);
		}

		public IEnumerator<IEffectNode> GetEnumerator() {
			return _currentEffects.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
