using System;
using System.Drawing;

namespace Vixen.Sys {
	interface IAnyEvaluatorHandler : IHandler<IEvaluator<float>>, IHandler<IEvaluator<DateTime>>, IHandler<IEvaluator<Color>> {
	}
}
