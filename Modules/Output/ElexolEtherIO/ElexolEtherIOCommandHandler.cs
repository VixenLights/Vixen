using System;
using System.Collections.Generic;
using Vixen.Sys.Dispatch;
using Vixen.Commands;

namespace VixenModules.Output.ElexolEtherIO
{
	class ElexolEtherIOCommandHandler : CommandDispatch
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
