using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Sys;

namespace Vixen.Execution {
	class ContextCurrentEffects : IEnumerable<EffectNode> {
		private List<EffectNode> _currentEffects;

		public ContextCurrentEffects() {
			_currentEffects = new List<EffectNode>();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="dataSource"></param>
		/// <param name="currentTime"></param>
		/// <returns>Ids of the affected channels.</returns>
		public Guid[] UpdateCurrentEffects(IDataSource dataSource, TimeSpan currentTime) {
			// Get the effects that are newly qualified.
			IEnumerable<EffectNode> newQualifiedEffects = dataSource.GetDataAt(currentTime);
			// Add them to the current effect list.
			_currentEffects.AddRange(newQualifiedEffects);
			// Get the distinct list of channels affected by the effects in the list 
			// (current and expired effects affect state).
			Guid[] affectedChannels = _GetChannelsAffected(_currentEffects);
			_RemoveExpiredEffects(currentTime);

			return affectedChannels;
		}

		private Guid[] _GetChannelsAffected(IEnumerable<EffectNode> effects) {
			return effects.SelectMany(x => x.Effect.TargetNodes.Select(y => y.Channel.Id)).Distinct().ToArray();
		}

		private void _RemoveExpiredEffects(TimeSpan currentTime) {
			// Remove expired effects.
			foreach(EffectNode effectNode in _currentEffects.ToArray()) {
				if(_IsExpired(currentTime, effectNode)) {
					_currentEffects.Remove(effectNode);
				}
			}
		}

		private bool _IsExpired(TimeSpan currentTime, EffectNode effectNode) {
			return currentTime > effectNode.EndTime;
		}


		public IEnumerator<EffectNode> GetEnumerator() {
			return _currentEffects.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
