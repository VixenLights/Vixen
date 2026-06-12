using System.Collections.ObjectModel;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.Property.State;

namespace VixenModules.App.CustomPropEditor.Services
{
	/// <summary>
	/// Maps Custom Prop Editor State models to and from the Vixen State property data model.
	/// </summary>
	internal static class CustomPropStateDataMapper
	{
		public static StateData ToStateData(ElementModel model)
		{
			ArgumentNullException.ThrowIfNull(model);

			model.NormalizeStateModelData();

			return new StateData
			{
				Id = model.StatePropertyId,
				StateDefinitions = model.StateDefinitionModels
					.Select(ToStateDefinitionData)
					.ToList()
			};
		}

		public static void ApplyToModel(ElementModel model, StateData stateData)
		{
			ArgumentNullException.ThrowIfNull(model);
			ArgumentNullException.ThrowIfNull(stateData);

			model.StatePropertyId = stateData.Id == Guid.Empty ? Guid.NewGuid() : stateData.Id;
			model.StateDefinitionModels = new ObservableCollection<StateDefinitionModel>(
				(stateData.StateDefinitions ?? []).Select(ToStateDefinitionModel));
			model.NormalizeStateModelData();
		}

		public static StateDefinitionData ToStateDefinitionData(StateDefinitionModel definition)
		{
			ArgumentNullException.ThrowIfNull(definition);

			definition.Normalize();

			return new StateDefinitionData
			{
				Id = definition.Id,
				Name = definition.Name,
				Description = definition.Description,
				Items = definition.Items.Select(ToStateItemData).ToList()
			};
		}

		public static StateDefinitionModel ToStateDefinitionModel(StateDefinitionData definition)
		{
			ArgumentNullException.ThrowIfNull(definition);

			return new StateDefinitionModel
			{
				Id = definition.Id == Guid.Empty ? Guid.NewGuid() : definition.Id,
				Name = string.IsNullOrWhiteSpace(definition.Name)
					? StateDefinitionModel.DefaultName
					: definition.Name.Trim(),
				Description = definition.Description ?? string.Empty,
				Items = new ObservableCollection<StateItemModel>(
					(definition.Items ?? []).Select(ToStateItemModel))
			};
		}

		public static StateItemData ToStateItemData(StateItemModel item)
		{
			ArgumentNullException.ThrowIfNull(item);

			item.Normalize();

			return new StateItemData
			{
				Id = item.Id,
				Name = item.Name,
				Color = item.Color,
				ElementNodeIds = item.ElementModelIds.Distinct().ToList()
			};
		}

		public static StateItemModel ToStateItemModel(StateItemData item)
		{
			ArgumentNullException.ThrowIfNull(item);

			return new StateItemModel
			{
				Id = item.Id == Guid.Empty ? Guid.NewGuid() : item.Id,
				Name = string.IsNullOrWhiteSpace(item.Name)
					? StateItemModel.DefaultName
					: item.Name.Trim(),
				Color = item.Color,
				ElementModelIds = new ObservableCollection<Guid>((item.ElementNodeIds ?? []).Distinct())
			};
		}
	}
}
