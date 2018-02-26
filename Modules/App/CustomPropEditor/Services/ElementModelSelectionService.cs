using System;
using System.Collections.Generic;
using System.Linq;
using Catel.Collections;
using VixenModules.App.CustomPropEditor.ViewModel;
using VixenModules.App.CustomPropEditor.ViewModels;

namespace VixenModules.App.CustomPropEditor.Services
{
	public class ElementModelSelectionService
	{
		private static ElementModelSelectionService _instance;
		private readonly Dictionary<Guid, ElementModelViewModel> _instances = new Dictionary<Guid, ElementModelViewModel>();

		protected ElementModelSelectionService()
		{
			
		}

		public static ElementModelSelectionService Instance()
		{
			return _instance ?? (_instance = new ElementModelSelectionService());
		}

		public void AddModel(Guid id, ElementModelViewModel model)
		{
			_instances.Add(id, model);
		}

		public void RemoveModel(Guid id)
		{
			_instances.Remove(id);
		}

		public ElementModelViewModel GetModel(Guid id)
		{
			ElementModelViewModel model;
			_instances.TryGetValue(id, out model);

			return model;
		}

		public IEnumerable<LightViewModel> GetSelectedLights()
		{
			return _instances.Values.Where(x => x.IsLeaf).SelectMany(m => m.LightViewModels.Where(l => l.IsSelected));
		}

		public IEnumerable<LightViewModel> SelectModelLights(IEnumerable<Guid> modelIds, bool selected = true)
		{
			List<LightViewModel> lvm = new List<LightViewModel>();
			foreach (var modelId in modelIds)
			{
				ElementModelViewModel model;
				if (_instances.TryGetValue(modelId, out model))
				{
					foreach (var modelLightViewModel in model.LightViewModels)
					{
						modelLightViewModel.IsSelected = selected;
						lvm.Add(modelLightViewModel);
					}
				}
			}

			return lvm;
		}

		public IEnumerable<ElementModelViewModel> SelectModels(IEnumerable<Guid> modelIds, bool selected=true, bool expandParents=false)
		{
			List<ElementModelViewModel> models = new List<ElementModelViewModel>();
			foreach (var modelId in modelIds)
			{
				ElementModelViewModel model;
				if (_instances.TryGetValue(modelId, out model))
				{
					models.Add(model);
					model.IsSelected = selected;
					if (expandParents)
					{
						ExpandModels(model.GetParentIds());
					}
				}
			}

			return models;
		}

		public void ExpandModels(IEnumerable<Guid> modelIds, bool expand=true)
		{
			foreach (var modelId in modelIds)
			{
				ElementModelViewModel model;
				if (_instances.TryGetValue(modelId, out model))
				{
					model.IsExpanded = expand;
				}
			}
		}
	}
}
