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
	public class Element : IOutputStateSource, IEqualityComparer<Element>, IEquatable<Element>, IDisposable
	{
		private IntentStateList _state;
		private readonly IStateCombinator _stateCombinator = new LayeredStateCombinator();
		private const int ElementStateListRatio = 3;
		bool _disposed;

		internal Element(string name)
			: this(Guid.NewGuid(), name)
		{
		}

		internal Element(Guid id, string name)
		{
			Id = id;
			Name = name;
			IntentStateListPool.GetPool().Allocate(ElementStateListRatio);
			_state = IntentStateListPool.GetPool().GetObject();
		}

		public string Name { get; set; }

		public Guid Id { get; protected set; }

		public bool Masked { get; set; }

		public void Update()
		{
			var state = _AggregateStateFromContexts();
			IntentStateListPool.GetPool().PutObject(_state);
			_state = state;
		}

		public void ClearStates()
		{
			IntentStateListPool.GetPool().PutObject(_state);
			_state = IntentStateListPool.GetPool().GetObject(); 
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

		private IntentStateList _AggregateStateFromContexts()
		{
			IntentStateList ret = IntentStateListPool.GetPool().GetObject();
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
					iss.Clear();
				}
			}
			return ret;

		}

		private List<IIntentState> GetCombinedState(IIntentStates states)
		{
			return _stateCombinator.Combine(states.AsIIntentStateList());
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (_disposed)
				return;

			if (disposing)
			{
				IntentStateListPool.GetPool().DeAllocate(ElementStateListRatio);
			}

			// release any unmanaged objects
			// set the object references to null

			_disposed = true;
		}

		~Element()
		{
			Dispose(false);
		}
	}
}