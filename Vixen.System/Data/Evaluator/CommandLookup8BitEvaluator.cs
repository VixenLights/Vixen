using System;
using System.Collections.Generic;
using Vixen.Commands;

namespace Vixen.Data.Evaluator
{
	public class CommandLookup8BitEvaluator
	{
		public static readonly Dictionary<byte, _8BitCommand> CommandLookup = new Dictionary<byte, _8BitCommand>();

		static CommandLookup8BitEvaluator()
		{
			for (int i = 0; i <= byte.MaxValue; i++)
			{
				CommandLookup.Add((byte)i, new _8BitCommand(i));
			}
		}
	}
}
