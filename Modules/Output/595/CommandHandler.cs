using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace VixenModules.Output.Olsen595
{
	internal class CommandHandler : CommandDispatch
	{
		public short Value;

		public void Reset()
		{
			Value = 0;
		}

		public override void Handle(_8BitCommand obj)
		{
			Value = (obj.CommandValue > 0) ? (short) 1 : (short) 0;
		}
	}
}