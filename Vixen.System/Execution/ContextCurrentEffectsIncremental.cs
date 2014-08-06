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
		private List<IEffectNode> _currentEffects;

		public ContextCurrentEffectsIncremental()
		{
			_currentEffects = new List<IEffectNode>();
		}

		/// <summary>
		/// Updates the collection of current effects, returning the ids of the affected elements.
		/// </summary>
		/// <returns>Ids of the affected elements.</returns>
		public Guid[] UpdateCurrentEffects(IDataSource dataSource, TimeSpan currentTime)
		{
			// Get the effects that are newly qualified.
			IEnumerable<IEffectNode> newQualifiedEffects = dataSource.GetDataAt(currentTime);
			// Add them to the current effect list.
			_currentEffects.AddRange(newQualifiedEffects);
			// Get the distinct list of all elements affected by all effects in the list.
			// List has current effects as well as effects that may be expiring.
			// Current and expired effects affect state.
			Guid[] affectedElements = _GetElementsAffected(_currentEffects);
			_RemoveExpiredEffects(currentTime);

			return affectedElements;
		}

		public void Reset()
		{
			if (_currentEffects != null)
				_currentEffects.Clear();
		}

		private Guid[] _GetElementsAffected(IEnumerable<IEffectNode> effects)
		{
			// If there's nothin here, pass back emptiness
			if (!effects.Any())
				return new Guid[0];
			
			
			
			return
				effects.SelectMany(x => x.Effect.TargetNodes).SelectMany(y => y.GetElementEnumerator()).Where((y) => y != null).Select(z => z.Id).Distinct()
					.ToArray();


			//return effects.SelectMany(x => x.Effect.TargetNodes.Select(y => y.Element.Id)).Distinct().ToArray();
		}

		private void _RemoveExpiredEffects(TimeSpan currentTime)
		{
			// Remove expired effects.
			foreach (EffectNode effectNode in _currentEffects.ToArray()) {
				if (_IsExpired(currentTime, effectNode)) {
					_currentEffects.Remove(effectNode);
				}
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