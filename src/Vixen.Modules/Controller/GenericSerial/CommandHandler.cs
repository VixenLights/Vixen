using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace VixenModules.Output.GenericSerial
{
	internal class CommandHandler : CommandDispatch
	{
		public byte Value { get; private set; }

		public void Reset()
		{
			Value = 0;
		}

		public override void Handle(_8BitCommand obj)
		{
			Value = obj.CommandValue;
		}
	}
}