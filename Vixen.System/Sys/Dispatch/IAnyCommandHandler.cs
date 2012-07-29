using Vixen.Commands;

namespace Vixen.Sys.Dispatch {
	interface IAnyCommandHandler : IHandler<_8BitCommand>, IHandler<_16BitCommand>, IHandler<_32BitCommand>, IHandler<_64BitCommand>, IHandler<ColorCommand> {
	}
}
