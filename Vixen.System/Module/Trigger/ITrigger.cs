using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Trigger
{
	public interface ITrigger
	{
		ITriggerInput[] TriggerInputs { get; }
		void UpdateState();
	}
}