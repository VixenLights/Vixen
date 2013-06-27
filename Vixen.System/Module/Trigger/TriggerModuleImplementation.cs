using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys.Attribute;

namespace Vixen.Module.Trigger
{
	[TypeOfModule("Trigger")]
	internal class TriggerModuleImplementation : ModuleImplementation<ITriggerModuleInstance>
	{
		public TriggerModuleImplementation()
			: base(new TriggerModuleManagement(), new TriggerModuleRepository())
		{
		}
	}
}