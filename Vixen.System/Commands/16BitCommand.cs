using System;
using Vixen.Sys;

namespace Vixen.Commands
{
	[Serializable]
	public class _16BitCommand : Dispatchable<_16BitCommand>, ICommand
	{
		public _16BitCommand(ushort value)
		{
			CommandValue = value;
			//SignedValue = (short) value;
			//UnsignedValue = value;
		}

		public _16BitCommand(byte value)
			: this((ushort) value)
		{
		}

		public _16BitCommand(int value)
			: this((ushort) value)
		{
		}

		public _16BitCommand(float value)
			: this((ushort) value)
		{
		}

		public _16BitCommand(double value)
			: this((ushort) value)
		{
		}

		public ushort CommandValue { get; set; }

		//public short SignedValue { get; private set; }

		//public ushort UnsignedValue { get; private set; }

		object ICommand.CommandValue
		{
			get { return CommandValue; }
			//set { CommandValue = (ushort) value; }
		}
	}
}