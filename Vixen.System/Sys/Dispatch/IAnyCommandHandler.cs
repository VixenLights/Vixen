using Vixen.Commands;

namespace Vixen.Sys.Dispatch {
	interface IAnyCommandHandler : IHandler<ByteValueCommand>, IHandler<SignedShortValueCommand>, IHandler<UnsignedShortValueCommand>, IHandler<SignedIntValueCommand>, IHandler<UnsignedIntValueCommand>, IHandler<SignedLongValueCommand>, IHandler<UnsignedLongValueCommand>, IHandler<ColorValueCommand>, IHandler<LightingValueCommand>, IHandler<DoubleValueCommand> {
	}
}
