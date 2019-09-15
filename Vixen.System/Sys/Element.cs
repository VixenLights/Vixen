using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Vixen.Data.StateCombinator;

namespace Vixen.Sys
{
	/// <summary>
	/// A logical representation of an element within a prop.
	/// </summary>
	[Serializable]
	public class Element : IOutputStateSource, IEqualityComparer<Element>, IEquatable<Element>
	{
		private const ushort ElementStateListRatio = 3;
		private int _stateIndex;
		private readonly IStateCombinator _stateCombinator = new LayeredStateCombinator();
		private readonly IntentStateList[] _stateLists = new IntentStateList[ElementStateListRatio];
		private readonly IntentStateList _contextStates = new IntentStateList();
		
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
			var nextIndex = GetNextStateIndex();
			_stateLists[nextIndex].Clear();
			_stateIndex = nextIndex;
		}

		[JsonIgnore]
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

		
		private int GetNextStateIndex()
		{
			if (_stateIndex < ElementStateListRatio - 1)
			{
				return _stateIndex + 1;
			}
			else
			{
				return 0;
			}
		}

		private void _AggregateStateFromContexts()
		{
			var nextIndex = GetNextStateIndex();
			_stateLists[nextIndex].Clear();
			_contextStates.Clear();
			foreach (var ctx in VixenSystem.Contexts)
			{
				if (ctx.IsRunning)
				{
					var iss = ctx.GetState(Id);
					if (iss == null)
						continue;
					_contextStates.AddRangeIntentState(iss.AsList());
				}
			}
			var states = GetCombinedState(_contextStates);
			foreach (var intentState in states)
			{
				_stateLists[nextIndex].Add(intentState);
			}
			_stateIndex = nextIndex;
		}

		private List<IIntentState> GetCombinedState(IIntentStates states)
		{
			return _stateCombinator.Combine(states.AsList());
		}

	}
}