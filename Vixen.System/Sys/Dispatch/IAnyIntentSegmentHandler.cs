using System;
using System.Drawing;

namespace Vixen.Sys.Dispatch {
	interface IAnyIntentSegmentHandler : IHandler<IIntentSegment<float>>, IHandler<IIntentSegment<DateTime>>, IHandler<IIntentSegment<Color>>, IHandler<IIntentSegment<long>>, IHandler<IIntentSegment<double>>, IHandler<IIntentSegment<LightingValue>> {
	}
}
