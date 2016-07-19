using System;
using System.Collections.Generic;
using Vixen.Execution.DataSource;
using Vixen.Sys;

namespace Vixen.Execution
{
	/// <summary>
	/// Maintains a list of current effects for a context.
	/// The IDataSource is expected to provide every qualifying effect, not just newly qualified effects.
	/// </summary>
	internal class ContextCurrentEffectsFull : List<IEffectNode> ,IContextCurrentEffects
	{
		private TimeSpan _lastUpdateTime = TimeSpan.Zero;
		private bool _reset = false;

		/// <summary>
		/// Updates the collection of current effects, returning the ids of the affected elements.
		/// </summary>
		/// <returns>Ids of the affected elements.</returns>
		public bool UpdateCurrentEffects(IDataSource dataSource, TimeSpan currentTime)
		{
			if (_lastUpdateTime > currentTime)
			{
				//Make sure the current effects are cleared if we go back to a earlier time.
				Clear();
			}
			_lastUpdateTime = currentTime;
			// Get the effects that are newly qualified.
			IEnumerable<IEffectNode> newQualifiedEffects = dataSource.GetDataAt(currentTime);
			// Add them to the current effect list.
			AddRange(newQualifiedEffects);
			// Get the distinct list of all elements affected by all effects in the list.
			// List has current effects as well as effects that may be expiring.
			// Current and expired effects affect state.
			//_GetElementsAffected();
			_RemoveExpiredEffects(currentTime);

			return Count > 0;
		}

		public void Reset()
		{
			_reset = true;
		}

		public bool Resetting()
		{
			return _reset;
		}

		private void _RemoveExpiredEffects(TimeSpan currentTime)
		{
			if (_reset)
			{
				Clear();
				_reset = false;
				return;
			}
			// Remove expired effects.
			foreach (var effectNode1 in this.ToArray())
			{
				var effectNode = (EffectNode)effectNode1;
				if (_IsExpired(currentTime, effectNode))
				{
					Remove(effectNode);
				}
			}
		}

		public void RemoveEffects(IEnumerable<IEffectNode> nodes)
		{
			foreach (var effectNode in nodes)
			{
				Remove(effectNode);
			}
		}

		private bool _IsExpired(TimeSpan currentTime, EffectNode effectNode)
		{
			return currentTime > effectNode.EndTime;
		}

	}
}