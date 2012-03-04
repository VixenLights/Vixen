using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Commands;

namespace Vixen.Sys {
	public interface IOutputStateSource : IStateSource<IIntentStateList> {
		//Command SourceState { get; }
	}
}
