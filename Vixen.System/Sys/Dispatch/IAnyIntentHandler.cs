using System;
using System.Drawing;

namespace Vixen.Sys.Dispatch {
	interface IAnyIntentHandler : IHandler<IIntent<float>>, IHandler<IIntent<DateTime>>, IHandler<IIntent<Color>>, IHandler<IIntent<long>>, IHandler<IIntent<double>>, IHandler<IIntent<LightingValue>> {
	}
}
