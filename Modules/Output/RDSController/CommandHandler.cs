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
		public DynamicCommand Value { get; private set; }

		public void Reset()
		{
			Value = null;
		}

		public override void Handle(DynamicCommand obj)
		{
			Value = obj;
		}
	}
}
