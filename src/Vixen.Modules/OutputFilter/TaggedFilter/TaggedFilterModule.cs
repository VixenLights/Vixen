using VixenModules.OutputFilter.TaggedFilter.Outputs;

namespace VixenModules.OutputFilter.TaggedFilter
{
	/// <summary>
	/// Maintains a tagged filter module.
	/// </summary>
	public class TaggedFilterModule : TaggedFilterModuleBase<TaggedFilterData, TaggedFilterOutput, TaggedFilterDescriptor>
	{		
		#region Protected Methods

		/// <summary>
		/// Refer to base class documentation.
		/// </summary>
		protected override TaggedFilterOutput CreateOutputInternal()
		{
			// Create the tagged filter output
			return new TaggedFilterOutput(Data.Tag);
		}

		#endregion
	}
}