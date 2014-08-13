using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

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
		private static readonly IIntentStates EmptyState = new IntentStateList();

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
			//In reality, all this needs to do is call GetElementState on each context
			//and put them all into a single collection.
			// NotNull() - filter out any null lists resulting from the context not yet having
			// a state for the element.  This versus creating a new list in
			// ElementStateSourceCollection.GetState (or maybe Context.GetState instead, may
			// make more sense there) on a dictionary miss.
			//IEnumerable<IIntentState> intentStates = _dataSource.Where(x => x != null).SelectMany(x => x.State);
			//return new IntentStateList(intentStates);

			//return new IntentStateList(_dataSource.Where(x => x != null).SelectMany(x => x.State));

			IntentStateList ret = new IntentStateList();
			foreach (var ctx in VixenSystem.Contexts.Where(x => x.IsRunning))
			{
				var iss = ctx.GetState(Id);
				if (iss == null)
					continue;
				ret.AddRange(iss.State);
			}
			return ret;

		}
	}
}