using System.Drawing;
using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace VixenModules.Output.DummyLighting
{
	internal class CommandHandler : CommandDispatch
	{
		public void Reset()
		{
			ByteValue = 0;
			ColorValue = Color.Transparent;
		}

		public override void Handle(_8BitCommand obj)
		{
			ByteValue = obj.CommandValue;
		}

		public override void Handle(ColorCommand obj)
		{
			ColorValue = obj.CommandValue;
		}

		public byte ByteValue;
		public Color ColorValue;
	}
}