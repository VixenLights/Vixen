using System;
using System.Drawing;

namespace Vixen.Sys.Dispatch {
	interface IAnyIntentStateHandler : IHandler<IIntentState<float>>, IHandler<IIntentState<DateTime>>, IHandler<IIntentState<Color>> {
	}
}
