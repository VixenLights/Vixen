using System;
using System.Collections.Generic;
using System.Linq;
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
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private TimeSpan _lastUpdateTime = TimeSpan.Zero;
		private bool _reset = false;
		private static TimeSpan _buffer = TimeSpan.FromMilliseconds(250);

		/// <summary>
		/// Updates the collection of current effects, returning the ids of the affected elements.
		/// </summary>
		/// <returns>Ids of the affected elements.</returns>
		public bool UpdateCurrentEffects(IDataSource dataSource, TimeSpan currentTime)
		{
			if (_lastUpdateTime - _buffer > currentTime)
			{
				//Make sure the current effects are cleared if we go back to a earlier time.
				Logging.Info("Last time is > current time. Resetting effect list. Last time: {0}, Current time: {1}", _lastUpdateTime, currentTime);
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
			Logging.Info("Current effects reset requested.");
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
				Logging.Info("Current effects reset request satisfied.");
				return;
			}
			// Remove expired effects.
			var nodes = ToArray();
			for (int i = nodes.Length - 1; i >= 0; i--)
			{
				if (_IsExpired(currentTime, this[i]))
				{
					RemoveAt(i);
				}
			}
		}

		public void RemoveEffects(IEnumerable<IEffectNode> nodes)
		{
			Logging.Info("Remove {0} effects requested.", nodes.Count());
			foreach (var effectNode in nodes)
			{
				Remove(effectNode);
			}
		}

		private bool _IsExpired(TimeSpan currentTime, IEffectNode effectNode)
		{
			return currentTime > effectNode.EndTime;
		}

	}
}