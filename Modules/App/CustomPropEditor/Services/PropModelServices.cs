using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Catel.Collections;
using NLog;
using VixenModules.App.CustomPropEditor.Model;
using Point = System.Windows.Point;

namespace VixenModules.App.CustomPropEditor.Services
{
	public class PropModelServices
	{
		protected static Logger Logging = LogManager.GetCurrentClassLogger();
		private static PropModelServices _instance;
		private Prop _prop;
		private readonly Dictionary<Guid, ElementModel> _models = new Dictionary<Guid, ElementModel>();
		
		private PropModelServices()
		{
			ModelsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
		}

		public static PropModelServices Instance()
		{
			if (_instance == null)
			{
				_instance = new PropModelServices();
			}

			return _instance;
		}

		public string ModelsFolder { get; set; }

		public bool EnsureModelDirectory()
		{
			if (!Directory.Exists(ModelsFolder))
			{
				try
				{
					Directory.CreateDirectory(ModelsFolder);
					return true;
				}
				catch
				{
					return false;
				}
			}
			return true;
		}

		public Prop CreateProp(string name = "New Prop")
		{
			_prop = new Prop(name);
			_models.Clear();
			_models.Add(_prop.RootNode.Id, _prop.RootNode);
			return _prop;
		}

		public Prop LoadProp(string path)
		{
			var p = PropModelPersistenceService.GetModel(path);
			if (p != null)
			{
				_prop = p;
				ExtractModels();
			}

			return _prop;
		}

		private void ExtractModels()
		{
			_models.Clear();
			var allModels = _prop.GetAll();
			
			allModels.Distinct().ForEach(x => _models.Add(x.Id, x));
		}

		public void SetImage(string filePath)
		{
			try
			{
				var image = new BitmapImage(new Uri(filePath, UriKind.Absolute));
				_prop.Image = image;
			}
			catch (Exception ex)
			{
				Logging.Error(ex, "An error occured loading the image for a prop.");
			}
		}

		public IEnumerable<ElementModel> GetLeafNodes()
		{
			return _prop.GetLeafNodes();
		}

		public ElementModel CreateNode(string name, ElementModel parent = null)
		{
			if (parent == null)
			{
				parent = _prop.RootNode;
			}
			ElementModel em = new ElementModel(Uniquify(name), parent);
			parent.AddChild(em);
			_models.Add(em.Id, em);
			return em;
		}

		public void CreateGroupForElementModels(string name, IEnumerable<ElementModel> elementModels)
		{
			var em = CreateNode(name);
			foreach (var elementModel in elementModels)
			{
				em.Children.Add(elementModel);
				elementModel.Parents.Add(em.Id);
			}
		}

		public void AddToParent(ElementModel model, ElementModel parentToJoin)
		{
			parentToJoin.Children.Add(model);
			model.Parents.Add(parentToJoin.Id);
		}

	    public void MoveWithinParent(ElementModel parent, ElementModel model, int newIndex)
	    {
	        int oldIndex = parent.Children.IndexOf(model);
	        if (oldIndex >= 0)
	        {
	            parent.Children.Move(oldIndex, newIndex);
	        }
	    }

		public void InsertToParent(ElementModel model, ElementModel parentToJoin, int index)
		{
			parentToJoin.Children.Insert(index, model);
			model.Parents.Add(parentToJoin.Id);
		}

		public void RemoveFromParent(ElementModel model, ElementModel parentToLeave)
		{
			model.RemoveParent(parentToLeave);
			parentToLeave.RemoveChild(model);
			if (!model.Parents.Any())
			{
				_models.Remove(model.Id);
			}
		}
		
		public ElementModel AddLightNode(ElementModel target, Point p, int? order = null, int? size = null)
		{
			if (target == null || target.IsRootNode)
			{
				target = FindOrCreateTargetGroupForLight();
			}
			else if (target.Lights.Any())
			{
				throw new ArgumentException("Cannot add light node to leaf element with lights.");
			}

			if (order == null)
			{
				order = GetNextOrder();
			}

			ElementModel em = new ElementModel(Uniquify($"{target.Name} - {order}"), order.Value, target);
			target.AddChild(em);
			_models.Add(em.Id, em);
			if (size == null)
			{
				size = em.LightSize;
			}
			else
			{
				em.LightSize = size.Value;
			}

			var light = CreateLight(p, size.Value, em.Id);
			em.AddLight(light);
			
			return em;

		}

		public ElementModel AddLight(ElementModel target, Point p, int? order = null)
		{
			if (target != null && !target.IsGroupNode && target.Parents.Any())
			{
				AddLightToTarget(p, target);
				return target;
			}

			return AddLightNode(target, p);
		}

		private ElementModel FindOrCreateTargetGroupForLight()
		{
			ElementModel target;
			if (_prop.RootNode.IsLeaf)
			{
				//Create a child group in the root to hold our light
				target = CreateNode(_prop.RootNode.Name + " Group", _prop.RootNode);
			}
			else
			{
				target = FindNearestLightGroupNode(_prop.RootNode) ?? CreateNode(_prop.RootNode.Name + " Group", _prop.RootNode);
			}

			return target;
		}

		

		private ElementModel FindNearestLightGroupNode(ElementModel element)
		{
			return element.GetNodeEnumerator().First(x => x.CanAddLightNodes);
		}

		private void AddLightToTarget(Point p, ElementModel em)
		{
			var light = CreateLight(p, em.LightSize, em.Id);
			em.AddLight(light);
		}

		public void RemoveLights(IEnumerable<Light> lights)
		{
			foreach (var light in lights)
			{
				RemoveLight(light);
			}
		}

		public void RemoveLight(Light light)
		{
			if (light.ParentModelId != Guid.Empty)
			{
				ElementModel em;
				if (_models.TryGetValue(light.ParentModelId, out em))
				{
					RemoveLight(em, light);
				}
			}
		}

		public void RemoveLight(ElementModel target, Light light)
		{
			if (target == null)
			{
				throw new ArgumentNullException(nameof(target));
			}

			if (light == null)
			{
				throw new ArgumentNullException(nameof(light));
			}

			if (target.IsLeaf)
			{
				var success = target.RemoveLight(light);
				//_lightToModel.Remove(light.Id);
				if (success)
				{
					if (!target.Lights.Any())
					{
						//remove me from all my parents so I can be deleted
						foreach (var parentId in target.Parents.ToList())
						{
							ElementModel parentElementModel;
							if (_models.TryGetValue(parentId, out parentElementModel))
							{
								parentElementModel.RemoveChild(target);
								target.RemoveParent(parentElementModel);
							}
							
						}

						//Remove me
						_models.Remove(target.Id);
					}
				}
			}
		}
		
		public Prop Prop => _prop;

		private Light CreateLight(Point p, double size, Guid parentModelId)
		{
			return new Light(p, size, parentModelId);
		}

		public bool IsNameDuplicated(string name)
		{
			return _models.Values.Count(x => x.Name == name) > 1;
		}

		public string Uniquify(string name)
		{
			if (_models.Values.Any(x => x.Name == name))
			{
				string originalName = name;
				bool unique;
				int counter = 2;
				do
				{
					name = $"{originalName} - {counter++}";
					unique = _models.Values.All(x => x.Name != name);
				} while (!unique);
			}
			return name;
		}

		private int GetNextOrder()
		{
			var max = _prop.RootNode.GetLeafEnumerator().Max(x => x.Order);
			if (max <= 0)
			{
				max = 1;
			}
			else
			{
				max++;
			}
			return max;
		}

	}
}
