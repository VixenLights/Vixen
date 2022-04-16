using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.DimmingFilter
{
	/// <summary>
	/// Maintains the dimming filter data.
	/// </summary>
	public class DimmingFilterData : TaggedFilterDataBase
	{
		#region Public Methods

		/// <summary>
		/// Clones the dimming filter data model.
		/// </summary>
		/// <returns>A copy of the dimming filter data model</returns>
		public override IModuleDataModel Clone()
		{
			// Create a new dimming filter data instance
			DimmingFilterData newInstance = new DimmingFilterData();
			
			// Copy the function tag
			newInstance.Tag = Tag;

			// Copy whether to convert RGB colors into dimming intents
			newInstance.ConvertRGBIntoDimmingIntents = ConvertRGBIntoDimmingIntents;

			// Return the new dimming filter data instance
			return newInstance;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Flag which determines if RGB colors are converted into color wheel index commands.
		/// </summary>
		[DataMember]
		public bool ConvertRGBIntoDimmingIntents { get; set; }

		#endregion
	}
}