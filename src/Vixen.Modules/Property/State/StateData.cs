using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Property.State
{
	/// <summary>
	/// Stores a named state definition and its configured state items.
	/// </summary>
	[DataContract]
	public sealed class StateData : ModuleDataModelBase
	{
		private const string DefaultName = "State Name 1";

		/// <summary>
		/// Initializes a new instance of the <see cref="StateData"/> class.
		/// </summary>
		public StateData()
		{
			Id = Guid.NewGuid();
			Name = DefaultName;
			Description = string.Empty;
			Items = [];
		}

		/// <summary>
		/// Gets or sets the stable identifier for the attached State property.
		/// </summary>
		/// <value>The stable identifier for the attached State property.</value>
		[DataMember]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the name that identifies the overall state definition.
		/// </summary>
		/// <value>The name that identifies the overall state definition.</value>
		[DataMember]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the user-provided description of the state definition.
		/// </summary>
		/// <value>The user-provided description of the state definition.</value>
		[DataMember]
		public string Description { get; set; }

		/// <summary>
		/// Gets or sets the configured state items.
		/// </summary>
		/// <value>The configured state items.</value>
		[DataMember]
		public List<StateItemData> Items { get; set; }

		/// <inheritdoc />
		public override IModuleDataModel Clone()
		{
			return new StateData
			{
				Id = Id,
				Name = Name,
				Description = Description,
				Items = Items.Select(item => item.Clone()).ToList()
			};
		}

		/// <summary>
		/// Creates a deep copy for a distinct attached State property.
		/// </summary>
		/// <returns>A deep copy with a new attached property identifier.</returns>
		internal StateData CloneForNewProperty()
		{
			var clone = (StateData)Clone();
			clone.Id = Guid.NewGuid();
			return clone;
		}

		internal void Normalize()
		{
			if (Id == Guid.Empty)
			{
				Id = Guid.NewGuid();
			}

			Name ??= DefaultName;
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
