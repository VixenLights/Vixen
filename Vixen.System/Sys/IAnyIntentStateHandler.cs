using System;
using System.Drawing;

namespace Vixen.Sys {
	interface IAnyIntentStateHandler : IHandler<IIntentState<float>>, IHandler<IIntentState<DateTime>>, IHandler<IIntentState<Color>> {
	}
}
