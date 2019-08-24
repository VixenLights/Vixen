using System;
using Vixen.Sys;

namespace Vixen.Commands
{
	[Serializable]
	public class _32BitCommand : Dispatchable<_32BitCommand>, ICommand
	{
		public _32BitCommand(uint value)
		{
			CommandValue = value;
			//SignedValue = (int) value;
			//UnsignedValue = value;
		}

		public _32BitCommand(byte value)
			: this((uint) value)
		{
		}

		public _32BitCommand(short value)
			: this((uint) value)
		{
		}

		public _32BitCommand(long value)
			: this((uint) value)
		{
		}

		public _32BitCommand(float value)
			: this((uint) value)
		{
		}

		public _32BitCommand(double value)
			: this((uint) value)
		{
		}

		public uint CommandValue { get; set; }

		//public int SignedValue { get; private set; }

		//public uint UnsignedValue { get; private set; }

		object ICommand.CommandValue
		{
			get { return CommandValue; }
			//set { CommandValue = (uint) value; }
		}
	}
}