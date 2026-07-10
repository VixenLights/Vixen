using Vixen.Commands;

namespace Vixen.Sys.Dispatch
{
	public abstract class CommandDispatch : IAnyCommandHandler
	{
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

		public virtual void Handle(StringCommand obj)
		{
		}
	}
}