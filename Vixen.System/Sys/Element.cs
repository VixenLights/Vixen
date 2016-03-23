using System;
using System.Collections.Generic;
using Vixen.Data.StateCombinator;

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
		private IIntentStates _state;
		private static readonly IIntentStates EmptyState = new IntentStateList(1);
		private readonly IStateCombinator _stateCombinator = new LayeredStateCombinator();

		internal Element(string name)
			: this(Guid.NewGuid(), name)
		{
		}

		internal Element(Guid id, string name)
		{
			Id = id;
			Name = name;
			_state = EmptyState;
		}

		public string Name { get; set; }

		public Guid Id { get; protected set; }

		public bool Masked { get; set; }

		public void Update()
		{
			_state = _AggregateStateFromContexts();
		}

		public void ClearStates()
		{
			_state = EmptyState;
		}

		public IIntentStates State
		{
			get { return _state; }
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

		private IIntentStates _AggregateStateFromContexts()
		{
			IntentStateList ret = new IntentStateList(2);
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
						ret.Add(intentState);
					}
				}
			}
			return ret;

		}

		private List<IIntentState> GetCombinedState(IIntentStates states)
		{
			return _stateCombinator.Combine(states.AsIIntentStateList());
		}
	}
}