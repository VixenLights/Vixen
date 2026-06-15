using System.Runtime.Serialization;
using Vixen.Module;

namespace VixenModules.Property.State
{
	/// <summary>
	/// Stores State definitions for an attached State property.
	/// </summary>
	[DataContract]
	public sealed class StateData : ModuleDataModelBase
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StateData"/> class.
		/// </summary>
		public StateData()
		{
			Id = Guid.NewGuid();
			StateDefinitions = [StateDefinitionData.CreateDefault(StateNamingRules.GetNextStateDefinitionName([]))];
		}

		/// <summary>
		/// Gets or sets the stable identifier for the attached State property.
		/// </summary>
		/// <value>The stable identifier for the attached State property.</value>
		[DataMember]
		public Guid Id { get; set; }

		/// <summary>
		/// Gets or sets the configured State definitions.
		/// </summary>
		/// <value>The configured State definitions.</value>
		[DataMember]
		public List<StateDefinitionData> StateDefinitions { get; set; }

		/// <inheritdoc />
		public override IModuleDataModel Clone()
		{
			return new StateData
			{
				Id = Id,
				StateDefinitions = StateDefinitions.Select(definition => definition.Clone()).ToList()
			};
		}

		/// <summary>
		/// Creates a deep copy for a distinct attached State property.
		/// </summary>
		/// <returns>A deep copy with a new attached property identifier.</returns>
		internal StateData CloneForNewProperty()
		{
			return new StateData
			{
				Id = Guid.NewGuid(),
				StateDefinitions = StateDefinitions
					.Select(definition => definition.CloneAsNew(definition.Name))
					.ToList()
			};
		}

		internal void Normalize()
		{
			if (Id == Guid.Empty)
			{
				Id = Guid.NewGuid();
			}

			StateDefinitions ??= [];

			if (StateDefinitions.Count == 0)
			{
				StateDefinitions.Add(StateDefinitionData.CreateDefault(StateNamingRules.GetNextStateDefinitionName([])));
			}

			foreach (var definition in StateDefinitions)
			{
				definition.Normalize();
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			Normalize();
		}
	}
}
