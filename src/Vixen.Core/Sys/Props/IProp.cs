using System.Collections.ObjectModel;
using System.ComponentModel;
using Vixen.Sys.Props.Components;

namespace Vixen.Sys.Props
{
	public interface IProp : INotifyPropertyChanged
	{
		/// <summary>
		/// Unique id of the Prop
		/// </summary>
		Guid Id { get; init; }

		/// <summary>
		/// Friendly name of the Prop
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Created by username
		/// </summary>
		string CreatedBy { get; set; }

		/// <summary>
		/// Creation date of the Prop
		/// </summary>
		DateTime CreationDate { get; }

		/// <summary>
		/// Last modified date of the Prop
		/// </summary>
		DateTime ModifiedDate { get; set; }

		/// <summary>
		/// Defined type of the Prop.
		/// </summary>
		PropType PropType { get; }

		/// <summary>
		/// Model for rendering the visual
		/// </summary>
		IPropModel PropModel { get; }
		
		/// <summary>
		/// Gets the collection of components associated with the prop.
		/// </summary>
		/// <remarks>
		/// Each component in the collection represents a distinct part of the prop, 
		/// allowing for modular and flexible configuration of the prop's structure and behavior.
		/// These are defined internally by the Prop.
		/// </remarks>
		ObservableCollection<IPropComponent> PropComponents { get; }

		/// <summary>
		/// Gets the collection of user-defined components associated with the prop.
		/// </summary>
		/// <remarks>
		/// User-defined components are custom components created by the user, 
		/// which can be added to the prop to extend its functionality or representation.
		/// </remarks>
		ObservableCollection<IPropComponent> UserDefinedPropComponents { get; }

		/// <summary>
		/// Should be called when the IProp is being removed so it can clean up any related items
		/// </summary>
		void CleanUp();
	}
}