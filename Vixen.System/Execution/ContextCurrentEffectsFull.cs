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
		private List<IEffectNode> _currentEffects;
		private HashSet<Guid> _currentAffectedElements;

		public ContextCurrentEffectsFull()
		{
			_currentEffects = new List<IEffectNode>();
			_currentAffectedElements = new HashSet<Guid>();
		}

		/// <summary>
		/// Updates the collection of current affects, returning the ids of the affected elements.
		/// </summary>
		/// <returns>Ids of the affected elements.</returns>
		public bool UpdateCurrentEffects(IDataSource dataSource, TimeSpan currentTime)
		{
			// Get the entirety of the new state.
			IEffectNode[] newState = dataSource.GetDataAt(currentTime).ToArray();
			// Get the elements affected by this new state.
			HashSet<Guid> nowAffectedElements = _GetAffectedElements(newState);
			// New and expiring effects affect the state, so get the union of
			// the previous state and the current state.
			//HashSet<Guid> allAffectedElements = new HashSet<Guid>(_currentAffectedElements.Concat(newAffectedElements));
			_currentAffectedElements.UnionWith(nowAffectedElements);
			//HashSet<Guid> allAffectedElements = _currentAffectedElements;
			// Set the new state.
			_currentEffects = new List<IEffectNode>(newState);
			_currentAffectedElements = nowAffectedElements;

			return _currentAffectedElements.Count>0;
		}

		public void RemoveEffects(IEnumerable<IEffectNode> nodes)
		{
			foreach (var effectNode in nodes)
			{
				_currentEffects.Remove(effectNode);
			}
		}

		public void Reset()
		{
			_currentEffects.Clear();
		}

		public bool Resetting()
		{
			return false;
		}

		private HashSet<Guid> _GetAffectedElements(IEnumerable<IEffectNode> effectNodes)
		{
			return new HashSet<Guid>(effectNodes.SelectMany(x => x.Effect.TargetNodes).SelectMany(y => y.GetElementEnumerator()).Select(z => z.Id));
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