using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Commands;
using Vixen.Data.Value;
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
