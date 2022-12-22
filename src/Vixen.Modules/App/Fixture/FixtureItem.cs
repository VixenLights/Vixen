using System.Runtime.Serialization;

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
