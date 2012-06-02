using System;
using System.Drawing;

namespace Vixen.Sys.Dispatch {
	interface IAnyEvaluatorHandler : IHandler<IEvaluator<float>>, IHandler<IEvaluator<DateTime>>, IHandler<IEvaluator<Color>>, IHandler<IEvaluator<long>>, IHandler<IEvaluator<double>>, IHandler<IEvaluator<LightingValue>> {
	}
}
