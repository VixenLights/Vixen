#nullable enable
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Vixen.Sys.Props.Components
{
	public interface IPropComponent: ITargetNodeProvider, INotifyPropertyChanged
	{
		/// <summary>
		/// Gets the unique identifier for the component.
		/// </summary>
		/// <value>
		/// A <see cref="Guid"/> representing the unique identifier of the component.
		/// </value>
		Guid Id { get; }
		
		/// <summary>
		/// Gets or sets the name of the prop component.
		/// </summary>
		/// <remarks>
		/// The name is used to uniquely identify the component within a prop.
		/// </remarks>
		string Name { get; set; }
		
		/// <summary>
		/// Gets the type of the prop component, indicating whether it is defined by the system or user-defined.
		/// </summary>
		/// <value>
		/// A <see cref="PropComponentType"/> value representing the type of the component.
		/// </value>
		PropComponentType ComponentType { get; }
		
		/// <summary>
		/// Adds a collection of <see cref="IElementNode"/> instances to the component.
		/// </summary>
		/// <param name="nodes">The collection of <see cref="IElementNode"/> instances to add.</param>
		/// <remarks>
		/// If any <see cref="IElementNode"/> is successfully added, the <see cref="TargetNodes"/> property will be updated, 
		/// and the <see cref="INotifyPropertyChanged.PropertyChanged"/> event will be raised.
		/// </remarks>
		void AddElementNodes(IEnumerable<IElementNode> nodes);

		/// <summary>
		/// Attempts to add the specified <see cref="IElementNode"/> to the component.
		/// </summary>
		/// <param name="node">The <see cref="IElementNode"/> to add.</param>
		/// <returns>
		/// <see langword="true"/> if the <see cref="IElementNode"/> is successfully added; 
		/// otherwise, <see langword="false"/>.
		/// </returns>
		bool TryAdd(IElementNode node);

		/// <summary>
		/// Attempts to remove an <see cref="IElementNode"/> by its unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier of the <see cref="IElementNode"/> to remove.</param>
		/// <param name="node">
		/// When this method returns, contains the removed <see cref="IElementNode"/> if the operation is successful; 
		/// otherwise, <see langword="null"/>. This parameter is passed uninitialized.
		/// </param>
		/// <returns>
		/// <see langword="true"/> if an <see cref="IElementNode"/> with the specified identifier is successfully removed; 
		/// otherwise, <see langword="false"/>.
		/// </returns>
		bool TryRemove(Guid id, [MaybeNullWhen(false)]out IElementNode node);

		/// <summary>
		/// Attempts to retrieve an <see cref="IElementNode"/> by its unique identifier.
		/// </summary>
		/// <param name="id">The unique identifier of the <see cref="IElementNode"/> to retrieve.</param>
		/// <param name="node">
		/// When this method returns, contains the <see cref="IElementNode"/> associated with the specified identifier, 
		/// if the identifier is found; otherwise, <see langword="null"/>. This parameter is passed uninitialized.
		/// </param>
		/// <returns>
		/// <see langword="true"/> if an <see cref="IElementNode"/> with the specified identifier is found; 
		/// otherwise, <see langword="false"/>.
		/// </returns>
		bool TryGet(Guid id, [MaybeNullWhen(false)] out IElementNode node);

		/// <summary>
		/// Removes all <see cref="IElementNode"/> instances from the component.
		/// </summary>
		/// <remarks>
		/// This method clears the component's collection of <see cref="IElementNode"/> instances 
		/// and raises the <see cref="INotifyPropertyChanged.PropertyChanged"/> event for the 
		/// <see cref="TargetNodes"/> property.
		/// </remarks>
		void Clear();
		
		/// <summary>
		/// Gets a value indicating whether the component is defined by the user.
		/// </summary>
		/// <value>
		/// <c>true</c> if the component is user-defined; otherwise, <c>false</c>.
		/// </value>
		bool IsUserDefined { get; }

	}
}