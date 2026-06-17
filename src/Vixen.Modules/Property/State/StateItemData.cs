using System.Runtime.Serialization;

namespace VixenModules.Property.State
{
	/// <summary>
	/// Stores one named state item, its display color, and its assigned element nodes.
	/// </summary>
	[DataContract]
	public sealed class StateItemData
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StateItemData"/> class.
		/// </summary>
		public StateItemData()
		{
			Id = Guid.NewGuid();
			Name = StateNamingRules.GetNextStateItemName([]);
			Color = System.Drawing.Color.White;
			ElementNodeIds = [];
		}

		/// <summary>
		/// Gets or sets the stable identifier for the state item.
		/// </summary>
		/// <value>The stable identifier for the state item.</value>
		[DataMember]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the user-visible name of the state item.
		/// </summary>
		/// <value>The user-visible name of the state item.</value>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the display color associated with the state item.
		/// </summary>
		/// <value>The display color associated with the state item.</value>
		[DataMember]
		public System.Drawing.Color Color { get; set; }

		/// <summary>
		/// Gets or sets the stable identifiers of the assigned element nodes.
		/// </summary>
		/// <value>The stable identifiers of the assigned element nodes.</value>
		[DataMember]
		public List<Guid> ElementNodeIds { get; set; }

		/// <summary>
		/// Creates a deep copy of this state item.
		/// </summary>
		/// <returns>A deep copy of this state item.</returns>
		public StateItemData Clone()
		{
			return new StateItemData
			{
				Id = Id,
				Name = Name,
				Color = Color,
				ElementNodeIds = [.. ElementNodeIds]
			};
		}

		/// <summary>
		/// Creates a deep copy for a distinct State definition.
		/// </summary>
		/// <returns>A deep copy with a new stable identifier.</returns>
		internal StateItemData CloneAsNew()
		{
			return new StateItemData
			{
				Id = Guid.NewGuid(),
				Name = Name,
				Color = Color,
				ElementNodeIds = [.. ElementNodeIds]
			};
		}

		internal void Normalize()
		{
			if (Id == Guid.Empty)
			{
				Id = Guid.NewGuid();
			}

			Name ??= StateNamingRules.GetNextStateItemName([]);
			ElementNodeIds ??= [];
		}
	}
}
