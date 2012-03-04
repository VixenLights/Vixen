using System;
using System.Drawing;

namespace Vixen.Sys.Dispatch {
	interface IAnyEvaluatorHandler : IHandler<IEvaluator<float>>, IHandler<IEvaluator<DateTime>>, IHandler<IEvaluator<Color>> {
	}
}
