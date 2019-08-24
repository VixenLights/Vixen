using System;
using Vixen.Sys;

namespace Vixen.Commands
{
	[Serializable]
	public class _64BitCommand : Dispatchable<_64BitCommand>, ICommand
	{
		public _64BitCommand(ulong value)
		{
			CommandValue = value;
			//SignedValue = (long) value;
			//UnsignedValue = value;
		}

		public _64BitCommand(byte value)
			: this((ulong) value)
		{
		}

		public _64BitCommand(short value)
			: this((ulong) value)
		{
		}

		public _64BitCommand(int value)
			: this((ulong) value)
		{
		}

		public _64BitCommand(float value)
			: this((ulong) value)
		{
		}

		public _64BitCommand(double value)
			: this((ulong) value)
		{
		}

		public ulong CommandValue { get; set; }

		//public long SignedValue { get; private set; }

		//public ulong UnsignedValue { get; private set; }

		object ICommand.CommandValue
		{
			get { return CommandValue; }
			//set { CommandValue = (ulong) value; }
		}
	}
}