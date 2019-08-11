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
		private bool _reset;

		/// <summary>
		/// Updates the collection of current effects, returning the ids of the affected elements.
		/// </summary>
		/// <returns>Ids of the affected elements.</returns>
		public bool UpdateCurrentEffects(IDataSource dataSource, TimeSpan currentTime)
		{
			if (_reset)
			{
#if DEBUG
				Logging.Info("Resetting effect list. Current time: {0}", currentTime);
#endif
				ResetEffectList();
			}

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

		public void Reset(bool now = false)
		{
#if DEBUG
			Logging.Info("Current effects reset requested.");
#endif
			if (now)
			{
				ResetEffectList();
			}
			else
			{
				_reset = true;
			}
			
		}

		public bool Resetting()
		{
			return _reset;
		}

		private void ResetEffectList()
		{
			Clear();
			_reset = false;
#if DEBUG
			Logging.Info("Current effects reset request satisfied.");
#endif
		}

		private void _RemoveExpiredEffects(TimeSpan currentTime)
		{
			// Remove expired effects.
			for (int i = Count - 1; i >= 0; i--)
			{
				if (_IsExpired(currentTime, this[i]))
				{
					RemoveAt(i);
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

		private bool _IsExpired(TimeSpan currentTime, IEffectNode effectNode)
		{
			return currentTime > effectNode.EndTime;
		}

	}
}