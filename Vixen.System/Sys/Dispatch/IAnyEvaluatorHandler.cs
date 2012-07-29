using System.Drawing;

namespace Vixen.Sys.Dispatch {
	interface IAnyEvaluatorHandler : IHandler<IEvaluator<byte>>, IHandler<IEvaluator<ushort>>, IHandler<IEvaluator<uint>>, IHandler<IEvaluator<ulong>>, IHandler<IEvaluator<Color>> {
	}
}
