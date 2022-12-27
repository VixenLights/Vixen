using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace VixenModules.Output.CommandController
{

	internal class CommandHandler : CommandDispatch
	{
		public StringCommand Value { get; private set; }

		public void Reset()
		{
			Value.CommandValue= null;
		}

		public override void Handle(StringCommand obj)
		{
			Value = obj;
		}
	}
}
