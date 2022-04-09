using Vixen.Module;

namespace VixenModules.OutputFilter.TaggedFilter
{
    /// <summary>
    /// Data associated with a tagged filter.
    /// </summary>
    public class TaggedFilterData : TaggedFilterDataBase
	{
		#region Public Methods

		/// <summary>
		/// Clones the filter data.
		/// </summary>
		/// <returns>Clone of the filter data</returns>
		public override IModuleDataModel Clone()
		{
			// Clone all the class members
			TaggedFilterData newInstance = (TaggedFilterData)MemberwiseClone();

			return newInstance;
		}

		#endregion
	}
}