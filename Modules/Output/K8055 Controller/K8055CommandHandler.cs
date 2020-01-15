using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Vixen.Commands;
using Vixen.Sys.Dispatch;

namespace VixenModules.Output.K8055_Controller
{
	class K8055CommandHandler : CommandDispatch
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
