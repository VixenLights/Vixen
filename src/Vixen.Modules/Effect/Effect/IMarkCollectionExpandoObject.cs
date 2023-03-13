using System.Collections.ObjectModel;
using Vixen.Marks;

namespace VixenModules.Effect.Effect
{
	/// <summary>
	/// Interface for an ExpandoObject that uses mark collections.
	/// </summary>
	public interface IMarkCollectionExpandoObject
	{
		/// <summary>
		/// Parent effect of the waveform.
		/// This property is used to register for mark events.
		/// </summary>
		BaseEffect Parent { get; set; }

		/// <summary>
		/// Collection of the mark collections.
		/// </summary>
		ObservableCollection<IMarkCollection> MarkCollections { get; set; }

		/// <summary>
		/// Clears the selected mark name collection for the ExpandoObject
		/// when it no longer exists.
		/// </summary>
		void UpdateSelectedMarkCollectionName();
	}
}
