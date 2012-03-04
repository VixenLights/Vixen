using System;
using System.Drawing;

namespace Vixen.Sys.Dispatch {
	interface IAnyCombinatorHandler : IHandler<ICombinator<float>>, IHandler<ICombinator<DateTime>>, IHandler<ICombinator<Color>> {
	}
}
