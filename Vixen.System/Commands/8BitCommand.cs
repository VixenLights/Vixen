using System;
using Vixen.Sys;

namespace Vixen.Commands
{
	[Serializable]
	public class _8BitCommand : Dispatchable<_8BitCommand>, ICommand
	{
		public _8BitCommand(byte value)
		{
			CommandValue = value;
			//SignedValue = (sbyte) value;
			//UnsignedValue = value;
		}

		public _8BitCommand(short value)
			: this((byte) value)
		{
		}

		public _8BitCommand(int value)
			: this((byte) value)
		{
		}

		public _8BitCommand(long value)
			: this((byte) value)
		{
		}

		public _8BitCommand(float value)
			: this((byte) value)
		{
		}

		public _8BitCommand(double value)
			: this((byte) value)
		{
		}

		public byte CommandValue { get; }

		//public sbyte SignedValue { get; private set; }

		//public byte UnsignedValue { get; private set; }

		object ICommand.CommandValue
		{
			get { return CommandValue; }
			//set { CommandValue = (byte) value; }
		}
	}
}