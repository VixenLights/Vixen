using Vixen.Commands;

namespace Vixen.Sys.Dispatch {
	interface IAnyCommandHandler : IHandler<ByteValue>, IHandler<SignedShortValue>, IHandler<UnsignedShortValue>, IHandler<SignedIntValue>, IHandler<UnsignedIntValue>, IHandler<SignedLongValue>, IHandler<UnsignedLongValue>, IHandler<ColorValue>, IHandler<LightingValueCommand> {
	}
}
