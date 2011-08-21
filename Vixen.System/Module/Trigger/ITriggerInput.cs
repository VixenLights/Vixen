using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

namespace Vixen.Module.Trigger {
    public interface ITriggerInput {
        TriggerInputType Type { get; }
        Guid Id { get; set; }
        string Name { get; set; }
		double Value { get; set; }
		event EventHandler Set;
	}
}
