using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Common {
    public interface ITimingSourceFactory {
        ITimingSource CreateTimingSource();
    }
}
