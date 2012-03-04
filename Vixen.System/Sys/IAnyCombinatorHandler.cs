using System;
using System.Drawing;

namespace Vixen.Sys {
	interface IAnyCombinatorHandler : IHandler<ICombinator<float>>, IHandler<ICombinator<DateTime>>, IHandler<ICombinator<Color>> {
	}
}
