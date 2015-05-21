using System;
using System.Collections.Generic;
using System.Linq;
using Vixen.Execution.DataSource;
using Vixen.Sys;

namespace Vixen.Execution
{
	/// <summary>
	/// Maintains a list of current effects for a context.
	/// The IDataSource is expected to provide only newly qualified effects, not every qualifying effect.
	/// </summary>
	internal class ContextCurrentEffectsIncremental : IContextCurrentEffects
	{
		private readonly List<IEffectNode> _currentEffects;
		private readonly HashSet<Guid> _affectedElements;
		private TimeSpan _lastUpdateTime = TimeSpan.Zero;
		private bool _reset = false;

		public ContextCurrentEffectsIncremental()
		{
			_affectedElements = new HashSet<Guid>(VixenSystem.Elements.Select(x => x.Id));
			_affectedElements.Clear();
			_currentEffects = new List<IEffectNode>();
		}

		/// <summary>
		/// Updates the collection of current effects, returning the ids of the affected elements.
		/// </summary>
		/// <returns>Ids of the affected elements.</returns>
		public HashSet<Guid> UpdateCurrentEffects(IDataSource dataSource, TimeSpan currentTime)
		{
			if (_lastUpdateTime > currentTime)
			{
				//Make sure the current effects are cleared if we go back to a earlier time.
				Reset();
			}
			_lastUpdateTime = currentTime;
			// Get the effects that are newly qualified.
			IEnumerable<IEffectNode> newQualifiedEffects = dataSource.GetDataAt(currentTime);
			// Add them to the current effect list.
			_currentEffects.AddRange(newQualifiedEffects);
			// Get the distinct list of all elements affected by all effects in the list.
			// List has current effects as well as effects that may be expiring.
			// Current and expired effects affect state.
			_GetElementsAffected(_currentEffects);
			_RemoveExpiredEffects(currentTime);

			return _affectedElements;
		}

		public void Reset()
		{
			_reset = true;
		}

		private void _GetElementsAffected(IEnumerable<IEffectNode> effects)
		{
			_affectedElements.Clear();
			_affectedElements.UnionWith(effects.SelectMany(x => x.Effect.EffectedElementIds));
			//return new HashSet<Guid>(effects.SelectMany(x => x.Effect.TargetNodes).SelectMany(y => y.GetElementEnumerator()).Select(z => z.Id).Distinct());	
		}

		private void _RemoveExpiredEffects(TimeSpan currentTime)
		{
			if (_reset)
			{
				_currentEffects.Clear();
				_reset = false;
				return;
			}
			// Remove expired effects.
			foreach (var effectNode1 in _currentEffects.ToArray())
			{
				var effectNode = (EffectNode) effectNode1;
				if (_IsExpired(currentTime, effectNode)) {
					_currentEffects.Remove(effectNode);
				}
			}
		}

		public void RemoveEffects(IEnumerable<IEffectNode> nodes)
		{
			foreach (var effectNode in nodes)
			{
				_currentEffects.Remove(effectNode);
			}
		}

		private bool _IsExpired(TimeSpan currentTime, EffectNode effectNode)
		{
			return currentTime > effectNode.EndTime;
		}

		public IEnumerator<IEffectNode> GetEnumerator()
		{
			return _currentEffects.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}