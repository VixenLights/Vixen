using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Common {
    /// <summary>
    /// Decorate properties and fields that return data directories that
    /// need to be present.
    /// The property or field must be static; public or private visibility is allowed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    class DataPathAttribute : Attribute {
    }
}
