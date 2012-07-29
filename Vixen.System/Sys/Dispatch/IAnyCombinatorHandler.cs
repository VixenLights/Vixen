using System.Drawing;

namespace Vixen.Sys.Dispatch {
	interface IAnyCombinatorHandler : IHandler<ICombinator<byte>>, IHandler<ICombinator<ushort>>, IHandler<ICombinator<uint>>, IHandler<ICombinator<ulong>>, IHandler<ICombinator<Color>> {
	}
}
