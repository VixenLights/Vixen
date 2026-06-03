using System.Runtime.Serialization;

namespace VixenModules.Property.State
{
	/// <summary>
	/// Stores one named State definition and its configured State items.
	/// </summary>
	[DataContract]
	public sealed class StateDefinitionData
	{
		internal const string DefaultName = "State - 1";

		/// <summary>
		/// Initializes a new instance of the <see cref="StateDefinitionData"/> class.
		/// </summary>
		public StateDefinitionData()
		{
			Id = Guid.NewGuid();
			Name = DefaultName;
			Description = string.Empty;
			Items = [];
		}

		/// <summary>
		/// Creates a default State definition for setup workflows.
		/// </summary>
		/// <param name="name">The user-visible State definition name.</param>
		/// <returns>A State definition containing one default State item.</returns>
		public static StateDefinitionData CreateDefault(string name)
		{
			return new StateDefinitionData
			{
				Name = string.IsNullOrWhiteSpace(name) ? DefaultName : name.Trim(),
				Items = [new StateItemData()]
			};
		}

		/// <summary>
		/// Gets or sets the stable identifier for the State definition.
		/// </summary>
		/// <value>The stable identifier for the State definition.</value>
		[DataMember]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the user-visible State definition name.
		/// </summary>
		/// <value>The user-visible State definition name.</value>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the user-provided State definition description.
		/// </summary>
		/// <value>The user-provided State definition description.</value>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the configured State items for this definition.
		/// </summary>
		/// <value>The configured State items for this definition.</value>
		[DataMember]
		public List<StateItemData> Items { get; set; }

		/// <summary>
		/// Creates a deep logical clone of this State definition.
		/// </summary>
		/// <returns>A deep clone that preserves stable identifiers.</returns>
		public StateDefinitionData Clone()
		{
			return new StateDefinitionData
			{
				Id = Id,
				Name = Name,
				Description = Description,
				Items = Items.Select(item => item.Clone()).ToList()
			};
		}

		/// <summary>
		/// Creates a deep copy for a distinct State definition.
		/// </summary>
		/// <param name="name">The name to assign to the copied State definition.</param>
		/// <returns>A deep copy with new stable identifiers.</returns>
		internal StateDefinitionData CloneAsNew(string name)
		{
			return new StateDefinitionData
			{
				Id = Guid.NewGuid(),
				Name = string.IsNullOrWhiteSpace(name) ? Name : name.Trim(),
				Description = Description,
				Items = Items.Select(item => item.CloneAsNew()).ToList()
			};
		}

		internal void Normalize()
		{
			if (Id == Guid.Empty)
			{
				Id = Guid.NewGuid();
			}

			Name = string.IsNullOrWhiteSpace(Name) ? DefaultName : Name.Trim();
			Description ??= string.Empty;
			Items ??= [];

			foreach (var item in Items)
			{
				item.Normalize();
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			Normalize();
		}
	}
}
