using System.Collections.Generic;
using Vixen.Commands;
using Vixen.Sys;
using Vixen.Sys.Dispatch;

namespace Vixen.Data.Combinator
{
	public abstract class Combinator<T> : Dispatchable<T>, ICombinator, IAnyCommandHandler
		where T : Combinator<T>
	{
		public ICommand Combine(IEnumerable<ICommand> commands)
		{
			CombinatorValue = null;

			foreach (ICommand command in commands) {
				command.Dispatch(this);
			}

			return CombinatorValue;
		}

		ICommand ICombinator.Combine(IEnumerable<ICommand> commands)
		{
			return Combine(commands);
		}

		public virtual void Handle(_8BitCommand obj)
		{
		}

		public virtual void Handle(_16BitCommand obj)
		{
		}

		public virtual void Handle(_32BitCommand obj)
		{
		}

		public virtual void Handle(_64BitCommand obj)
		{
		}

		public virtual void Handle(ColorCommand obj)
		{
		}

		// ResultType generic parameter is used by the combinators so the value wrapped
		// by the command can be known.
		protected ICommand CombinatorValue { get; set; }
	}
}