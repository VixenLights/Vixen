using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace VixenModules.Output.RDSController
{

	internal class CommandHandler : CommandDispatch
	{
		public CustomCommand Value { get; private set; }

		public void Reset()
		{
			Value = null;
		}

		public override void Handle(CustomCommand obj)
		{
			Value = obj;
		}
	}
}
