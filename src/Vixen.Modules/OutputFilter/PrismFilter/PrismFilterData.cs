using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.PrismFilter
{
	/// <summary>
	/// Maintains the prism filter data.
	/// </summary>
	public class PrismFilterData : TaggedFilterDataBase
	{
		#region Public Methods

		/// <summary>
		/// Clones the prism filter data model.
		/// </summary>
		/// <returns>A copy of the prism filter data model</returns>
		public override IModuleDataModel Clone()
		{
			// Create a new prism filter data instance
			PrismFilterData newInstance = new PrismFilterData();
			
			// Copy the function tag
			newInstance.Tag = Tag;

			// Copy whether to convert prism intents ito Open Prism Intents
			newInstance.ConvertPrismIntentsIntoOpenPrismIntents = ConvertPrismIntentsIntoOpenPrismIntents;

			// Copy the open prism index command value
			newInstance.OpenPrismIndexValue = OpenPrismIndexValue;

			// Copy the close prism index command value
			newInstance.ClosePrismIndexValue = ClosePrismIndexValue;

			// Copy the Associated (Prism) Function Name
			newInstance.AssociatedFunctionName = AssociatedFunctionName;

			// Return the new prism filter data instance
			return newInstance;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Flag which determines if prism intents are converted into open prism commands.
		/// </summary>
		[DataMember]
		public bool ConvertPrismIntentsIntoOpenPrismIntents { get; set; }

		/// <summary>
		/// Open prism index command value.
		/// </summary>
		[DataMember]
		public byte OpenPrismIndexValue { get; set; }

		/// <summary>
		/// Close prism index command value.
		/// </summary>
		[DataMember]
		public byte ClosePrismIndexValue { get; set; }

		/// <summary>
		/// Function name of the associated prism function.
		/// </summary>
		[DataMember]
		public string AssociatedFunctionName { get; set; }

		#endregion
	}
}