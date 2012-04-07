using System;
using System.Collections.Generic;

namespace Vixen.Sys.CombinationOperation {
	abstract public class BooleanOperation : ICombinationOperation {
		private Dictionary<Type, ICombiner> _combiners;

		protected BooleanOperation() {
			_combiners = new Dictionary<Type, ICombiner>();
		}

		protected void RegisterCombiner<T>(ICombiner<T> combiner) {
			_combiners[typeof(T)] = combiner;
		}

		public T Combine<T>(T value, IIntentState intentState) {
			// Been trying to avoid casting, but it was unavoidable at this point.
			// Once you expand a generic, the cat is out of the bag and you can't
			// go back once you cross that line, so I've been trying to carry
			// generics down as far as possible.
			// When the generic was expanded to concrete values higher up, no casting
			// was necessary, but then everything from that point on has to also
			// use the concrete types, making more of a maintenance nightmare.
			// Trying to carry generics down as far as possible to lessen future
			// maintenance.  However, since actual types are being passed in here and
			// not a type wrapped in another class or interface, double dispatch
			// is not possible.
			// So, we cast.
			ICombiner combiner;
			if(_combiners.TryGetValue(typeof(T), out combiner)) {
				return ((ICombiner<T>)combiner).Combine(value, intentState);
			}
			return value;
		}
	}
}
