using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.OutputFilter.TaggedFilter.Outputs;

namespace VixenModules.OutputFilter.TaggedFilter
{
	/// <summary>
	/// Base class for tagged filter data model.
	/// </summary>
    public abstract class TaggedFilterDataBase : ModuleDataModelBase, ITaggedFilterData
	{
		#region Public Properties

		/// <summary>
		/// Tag associated with the filter.
		/// </summary>
		[DataMember]
		public string Tag { get; set; }

		#endregion
	}
}
