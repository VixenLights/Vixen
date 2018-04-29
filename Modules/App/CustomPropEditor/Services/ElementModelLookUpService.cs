using System;
using System.Collections.Generic;
using System.Linq;
using VixenModules.App.CustomPropEditor.ViewModels;

namespace VixenModules.App.CustomPropEditor.Services
{
	/// <summary>
	/// This class allows model to view modeal associations for ElementModels to facilite navigating the tree.
	/// </summary>
	public class ElementModelLookUpService
	{
		private static ElementModelLookUpService _instance;
		private readonly Dictionary<Guid, List<ElementModelViewModel>> _instances = new Dictionary<Guid, List<ElementModelViewModel>>();

		protected ElementModelLookUpService()
		{
			
		}
		public static ElementModelLookUpService Instance => _instance ?? (_instance = new ElementModelLookUpService());

	    public void Reset()
	    {
            _instances.Clear();
	    }

		/// <summary>
		/// Create an association between the ElementModel and the View Model 
		/// </summary>
		/// <param name="id"></param>
		/// <param name="model"></param>
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

		/// <summary>
		/// Remove the model association from the the ElementModel
		/// </summary>
		/// <param name="id"></param>
		/// <param name="model"></param>
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

		public IEnumerable<ElementModelViewModel> GetAllModels()
		{
			return _instances.Values.SelectMany(x => x);
		}
	}
}
