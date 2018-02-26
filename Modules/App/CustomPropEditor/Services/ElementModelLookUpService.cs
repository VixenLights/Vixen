using System;
using System.Collections.Generic;
using System.Linq;
using VixenModules.App.CustomPropEditor.ViewModels;

namespace VixenModules.App.CustomPropEditor.Services
{
	public class ElementModelLookUpService
	{
		private static ElementModelLookUpService _instance;
		private readonly Dictionary<Guid, List<ElementModelViewModel>> _instances = new Dictionary<Guid, List<ElementModelViewModel>>();

		protected ElementModelLookUpService()
		{
			
		}
		public static ElementModelLookUpService Instance => _instance ?? (_instance = new ElementModelLookUpService());

		public void AddModel(Guid id, ElementModelViewModel model)
		{
			var models = GetModels(id);
			if (models != null)
			{
				models.Add(model);
			}
			else
			{
				_instances.Add(id, new List<ElementModelViewModel>(new []{model}));
			}
		}

		public void RemoveModel(Guid id, ElementModelViewModel model)
		{
			var models = GetModels(id);
			if (models != null)
			{
				models.Remove(model);
				if (!models.Any())
				{
					_instances.Remove(id);
				}
			}
			
		}

		/// <summary>
		/// Gets all the view models associated to a ElementModel Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		public List<ElementModelViewModel> GetModels(Guid id)
		{
			List<ElementModelViewModel> model;
			_instances.TryGetValue(id, out model);

			return model;
		}

		//public IEnumerable<ElementModelViewModel> SelectModels(IEnumerable<Guid> modelIds, bool selected=true, bool expandParents=false)
		//{
		//	List<ElementModelViewModel> models = new List<ElementModelViewModel>();
		//	foreach (var modelId in modelIds)
		//	{
		//		ElementModelViewModel model;
		//		if (_instances.TryGetValue(modelId, out model))
		//		{
		//			models.Add(model);
		//			model.IsSelected = selected;
		//			if (expandParents)
		//			{
		//				ExpandModels(model.GetParentIds());
		//			}
		//		}
		//	}

		//	return models;
		//}

		//public void ExpandModels(IEnumerable<Guid> modelIds, bool expand=true)
		//{
		//	foreach (var modelId in modelIds)
		//	{
		//		ElementModelViewModel model;
		//		if (_instances.TryGetValue(modelId, out model))
		//		{
		//			model.IsExpanded = expand;
		//		}
		//	}
		//}
	}
}
