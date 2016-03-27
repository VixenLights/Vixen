using System;
using System.Collections.Generic;
using Vixen.Data.StateCombinator;
using Vixen.Pool;

namespace Vixen.Sys
{
	// Need to implement IEquatable<T> so that IEnumerable<T>.Except() will not use
	// the default Object Equals() and GetHashCode().
	/// <summary>
	/// A logical channel of low-level CommandData that is intended to be executed by a controller.
	/// </summary>
	[Serializable]
	public class Element : IOutputStateSource, IEqualityComparer<Element>, IEquatable<Element>
	{
		private const int ElementStateListRatio = 3;
		private ushort _stateIndex;
		private readonly IStateCombinator _stateCombinator = new LayeredStateCombinator();
		private readonly IntentStateList[] _stateLists = new IntentStateList[ElementStateListRatio];
		
		bool _disposed;

		internal Element(string name)
			: this(Guid.NewGuid(), name)
		{
		}

		internal Element(Guid id, string name)
		{
			Id = id;
			Name = name;
			for (int i = 0; i < ElementStateListRatio; i++)
			{
				_stateLists[i] = new IntentStateList(4);
			}
		}

		public string Name { get; set; }

		public Guid Id { get; protected set; }

		public bool Masked { get; set; }

		public void Update()
		{
			_AggregateStateFromContexts();
		}

		public void ClearStates()
		{
			ResetState();
		}

		public IIntentStates State
		{
			get { return _stateLists[_stateIndex]; }
		}

		public override string ToString()
		{
			return Name;
		}

		public bool Equals(Element x, Element y)
		{
			return x.Id == y.Id;
		}

		public int GetHashCode(Element obj)
		{
			return obj.Id.GetHashCode();
		}

		// Both of these are required for Except(), Distinct(), Union() and Intersect().
		// Equals(<type>) for IEquatable and GetHashCode() because that's needed anytime
		// Equals(object) is overridden (which it really isn't, but this is what is said and
		// it doesn't work otherwise).
		public bool Equals(Element other)
		{
			return Id == other.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		private void ResetState()
		{
			if (_stateIndex < ElementStateListRatio - 1)
			{
				_stateLists[_stateIndex + 1].Clear();
				_stateIndex++;
			}
			else
			{
				_stateLists[0].Clear();
				_stateIndex = 0;
			}
		}

		private void _AggregateStateFromContexts()
		{
			ResetState();
			foreach (var ctx in VixenSystem.Contexts)
			{
				if (ctx.IsRunning)
				{
					var iss = ctx.GetState(Id);
					if (iss == null)
						continue;
					var states = GetCombinedState(iss);
					foreach (var intentState in states)
					{
						_stateLists[_stateIndex].Add(intentState);
					}
				}
			}
		}

		private List<IIntentState> GetCombinedState(IIntentStates states)
		{
			return _stateCombinator.Combine(states.AsList());
		}

	}
}