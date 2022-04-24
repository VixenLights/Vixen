using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace VixenModules.App.Fixture
{
    /// <summary>
    /// Base class for fixture meta-data.
    /// </summary>
    [DataContract]
    public abstract class FixtureItem
    {
        /// <summary>
        /// Name of the item.
        /// </summary>
        [DataMember]
        public string Name { get; set; }
    }
}
