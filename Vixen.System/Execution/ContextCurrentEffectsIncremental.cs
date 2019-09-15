using System;
using System.Collections.Generic;
using Vixen.Execution.DataSource;
using Vixen.Sys;

namespace Vixen.Execution
{
	/// <summary>
	/// Maintains a list of current effects for a context.
	/// The IDataSource is expected to provide only newly qualified effects, not every qualifying effect.
	/// </summary>
	internal class ContextCurrentEffectsIncremental : List<IEffectNode>, IContextCurrentEffects
	{
		private bool _reset;

		/// <summary>
		/// Updates the collection of current effects, returning the ids of the affected elements.
		/// </summary>
		/// <returns>Ids of the affected elements.</returns>
		public bool UpdateCurrentEffects(IDataSource dataSource, TimeSpan currentTime)
		{
			_reset = true;
			Clear();
			_reset = false;
			// Get the effects that are newly qualified.
			// Add them to the current effect list.
			AddRange(dataSource.GetDataAt(currentTime));
			
			return Count>0;
		}

		public void Reset(bool now)
		{
			//Nothing to do we reset every time.
		}

		public bool Resetting()
		{
			return _reset;
		}

		public void RemoveEffects(IEnumerable<IEffectNode> nodes)
		{
			foreach (var effectNode in nodes)
			{
				Remove(effectNode);
			}
		}
		
	}
}