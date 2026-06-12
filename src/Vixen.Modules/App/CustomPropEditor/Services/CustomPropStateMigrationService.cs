using System.Collections.ObjectModel;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Services
{
	/// <summary>
	/// Migrates legacy Custom Prop Editor State data into the authored State definition model.
	/// </summary>
	internal static class CustomPropStateMigrationService
	{
		public static bool MigrateLegacyStateData(Prop prop)
		{
			ArgumentNullException.ThrowIfNull(prop);

			var migrated = false;

			foreach (var model in prop.GetAll())
			{
				model.NormalizeStateModelData();

				if (model.StateDefinitionModels.Count == 0 && model.StateDefinitions.Any())
				{
					model.StateDefinitionModels = new ObservableCollection<StateDefinitionModel>(
						ConvertLegacyImportedRows(model.StateDefinitions));
					model.NormalizeStateModelData();
					migrated = true;
				}
			}

			var modelElement = PropStateModelResolver.GetModelElement(prop);
			modelElement.NormalizeStateModelData();

			if (modelElement.StateDefinitionModels.Count == 0)
			{
				var elementLevelDefinitions = ConvertLegacyElementDefinitions(prop);
				if (elementLevelDefinitions.Any())
				{
					modelElement.StateDefinitionModels = new ObservableCollection<StateDefinitionModel>(elementLevelDefinitions);
					modelElement.NormalizeStateModelData();
					migrated = true;
				}
			}

			return migrated;
		}

		private static IEnumerable<StateDefinitionModel> ConvertLegacyImportedRows(IEnumerable<StateDefinition> legacyRows)
		{
			return legacyRows
				.Where(row => row != null)
				.GroupBy(row => string.IsNullOrWhiteSpace(row.StateDefinitionName)
					? StateDefinitionModel.DefaultName
					: row.StateDefinitionName.Trim())
				.Select(group => new StateDefinitionModel
				{
					Name = group.Key,
					Description = string.Empty,
					Items = new ObservableCollection<StateItemModel>(
						group
							.OrderBy(row => row.Index)
							.Select(ConvertLegacyImportedRow)
							.Where(item => item.ElementModelIds.Any()))
				})
				.Where(definition => definition.Items.Any());
		}

		private static StateItemModel ConvertLegacyImportedRow(StateDefinition legacyRow)
		{
			return new StateItemModel
			{
				Name = string.IsNullOrWhiteSpace(legacyRow.Name) ? StateItemModel.DefaultName : legacyRow.Name.Trim(),
				Color = legacyRow.DefaultColor,
				ElementModelIds = new ObservableCollection<Guid>((legacyRow.ElementModelIds ?? []).Distinct())
			};
		}

		private static IEnumerable<StateDefinitionModel> ConvertLegacyElementDefinitions(Prop prop)
		{
			return prop.GetAll()
				.Where(model => model.StateDefinition != null)
				.GroupBy(model => string.IsNullOrWhiteSpace(model.StateDefinition.StateDefinitionName)
					? StateDefinitionModel.DefaultName
					: model.StateDefinition.StateDefinitionName.Trim())
				.Select(group => new StateDefinitionModel
				{
					Name = group.Key,
					Description = string.Empty,
					Items = new ObservableCollection<StateItemModel>(
						group.Select(ConvertLegacyElementDefinition))
				})
				.Where(definition => definition.Items.Any());
		}

		private static StateItemModel ConvertLegacyElementDefinition(ElementModel model)
		{
			return new StateItemModel
			{
				Name = string.IsNullOrWhiteSpace(model.StateDefinition.Name)
					? StateItemModel.DefaultName
					: model.StateDefinition.Name.Trim(),
				Color = model.StateDefinition.DefaultColor,
				ElementModelIds = new ObservableCollection<Guid> { model.Id }
			};
		}
	}
}
