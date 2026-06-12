using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Services
{
	/// <summary>
	/// Locates and updates the element model used for Custom Prop Editor State authoring.
	/// </summary>
	internal static class PropStateModelResolver
	{
		public static ElementModel GetModelElement(Prop prop)
		{
			ArgumentNullException.ThrowIfNull(prop);

			return GetExplicitModelElements(prop).FirstOrDefault() ?? prop.RootNode;
		}

		public static IReadOnlyList<ElementModel> GetExplicitModelElements(Prop prop)
		{
			ArgumentNullException.ThrowIfNull(prop);

			return prop.GetAll()
				.Where(model => model.ModelType == ElementModelType.Model)
				.ToList();
		}

		public static bool CanAssignModelType(ElementModel model, ElementModelType modelType)
		{
			ArgumentNullException.ThrowIfNull(model);

			return modelType != ElementModelType.Model ||
				!model.IsLeaf ||
				model.IsRootNode && !model.Children.Any();
		}

		public static bool TrySetModelType(Prop prop, ElementModel model, ElementModelType modelType)
		{
			ArgumentNullException.ThrowIfNull(prop);
			ArgumentNullException.ThrowIfNull(model);

			if (!CanAssignModelType(model, modelType))
			{
				return false;
			}

			if (modelType == ElementModelType.Model)
			{
				foreach (var existingModel in GetExplicitModelElements(prop).Where(existingModel => existingModel.Id != model.Id))
				{
					existingModel.ModelType = ElementModelType.None;
				}
			}

			model.ModelType = modelType;
			return true;
		}
	}
}
