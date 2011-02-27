using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Common {
    public interface ITimingSource {
        int Position { get; }
    }
}
