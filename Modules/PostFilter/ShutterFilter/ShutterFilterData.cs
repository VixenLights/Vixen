using System.Runtime.Serialization;
using Vixen.Module;
using VixenModules.OutputFilter.TaggedFilter;

namespace VixenModules.OutputFilter.ShutterFilter
{
	/// <summary>
	/// Maintains the shutter filter data.
	/// </summary>
	public class ShutterFilterData : TaggedFilterDataBase
	{
		#region Public Methods

		/// <summary>
		/// Clones the shutter filter data model.
		/// </summary>
		/// <returns>A copy of the shutter filter data model</returns>
		public override IModuleDataModel Clone()
		{
			// Create a new shutter filter data instance
			ShutterFilterData newInstance = new ShutterFilterData();
			
			// Copy the function tag
			newInstance.Tag = Tag;

			// Copy whether to convert RGB colors into shutter open intents
			newInstance.ConvertColorIntoShutterIntents = ConvertColorIntoShutterIntents;

			// Copy the open shutter index command value
			newInstance.OpenShutterIndexValue = OpenShutterIndexValue;

			// Copy the close shutter index command value
			newInstance.CloseShutterIndexValue = CloseShutterIndexValue;

			// Return the new shutter filter data instance
			return newInstance;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Flag which determines if RGB colors are converted into shutter commands.
		/// </summary>
		[DataMember]
		public bool ConvertColorIntoShutterIntents { get; set; }

		/// <summary>
		/// Open Shutter index command value.
		/// </summary>
		[DataMember]
		public byte OpenShutterIndexValue { get; set; }

		/// <summary>
		/// Open Shutter index command value.
		/// </summary>
		[DataMember]
		public byte CloseShutterIndexValue { get; set; }

		#endregion
	}
}