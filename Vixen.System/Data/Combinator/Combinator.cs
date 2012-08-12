using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Combinator {
	abstract public class Combinator<T, ResultType> : Dispatchable<T>, ICombinator<ResultType>, IAnyCommandHandler
	    where T : Combinator<T, ResultType>  {
		public ICommand<ResultType> Combine(IEnumerable<ICommand> commands) {
			CombinatorValue = null;

			foreach(ICommand command in commands) {
				command.Dispatch(this);
			}

			return CombinatorValue;
		}

		ICommand ICombinator.Combine(IEnumerable<ICommand> commands) {
			return Combine(commands);
		}

		virtual public void Handle(_8BitCommand obj) { }

		virtual public void Handle(_16BitCommand obj) { }

		virtual public void Handle(_32BitCommand obj) { }

		virtual public void Handle(_64BitCommand obj) { }

		virtual public void Handle(ColorCommand obj) { }

		// ResultType generic parameter is used by the combinators so the value wrapped
		// by the command can be known.
		protected ICommand<ResultType> CombinatorValue { get; set; }
	}
}
