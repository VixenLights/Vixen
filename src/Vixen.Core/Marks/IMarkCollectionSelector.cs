namespace Vixen.Marks
{
	/// <summary>
	/// Represents an editor-facing Mark Collection selection property.
	/// </summary>
	/// <remarks>
	/// The property displays a Mark Collection name while the owning effect maps that name to a stable persisted identifier. <c>BaseEffect</c> raises <see cref="System.ComponentModel.INotifyPropertyChanged.PropertyChanged" /> for this property when the available Mark Collection names change.
	/// </remarks>
	public interface IMarkCollectionSelector
	{
		/// <summary>
		/// Gets or sets the displayed name of the selected Mark Collection.
		/// </summary>
		/// <value>The displayed Mark Collection name, mapped by the owning effect to its stable persisted identifier.</value>
		string MarkCollectionId { get; set; }
	}
}
