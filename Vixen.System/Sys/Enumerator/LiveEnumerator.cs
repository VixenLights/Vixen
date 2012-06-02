using System.Collections.Generic;

namespace Vixen.Sys.Enumerator {
	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T">Element type</typeparam>
	/// <typeparam name="U">Collection type</typeparam>
	abstract class LiveEnumerator<T,U> : IEnumerator<T> {
		private T _current;
		private T _next;
		private bool _nextIsValid;
		private bool _currentIsValid;

		abstract protected bool _GetNext(out T value);
		protected U Collection { get; private set; }

		public LiveEnumerator(U collection) {
			Collection = collection;
			Reset();
		}

		public T Current {
			get { return _current; }
		}

		public void Dispose() { }

		object System.Collections.IEnumerator.Current {
			get { return Current; }
		}

		public bool MoveNext() {
			_current = _next;
			// The default of CommandData is valid data being used to clear outputs.
			_currentIsValid = _nextIsValid;
			_nextIsValid = _GetNext(out _next);
			// Must be done this way because the compiler doesn't know if T is a value type
			// or a reference type.  _current.Equals cannot be used because the value may
			// be null.  "==" operator cannot be used because the compiler won't know to
			// do a reference comparison or value comparison.
			// See:
			// http://stackoverflow.com/questions/65351/null-or-default-comparsion-of-generic-argument-in-c
			//return !object.Equals(_current, default(T));
			return _currentIsValid;
		}

		public void Reset() {
			_current = default(T);
			_currentIsValid = false;
			// Iterate once to get the next ptr set, not the current ptr.
			_nextIsValid = _GetNext(out _next);
		}
	}
}
