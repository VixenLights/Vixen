using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Catel.Collections;
using NLog;
using Vixen.Sys;
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
			ModelsFolder = Paths.DataRootPath;
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

		public Prop CreateProp(string name = "New Prop {1}")
		{
			_prop = new Prop(name);
			_models.Clear();
			_models.Add(_prop.RootNode.Id, _prop.RootNode);
			return _prop;
		}

		public Prop LoadProp(string path)
		{
			_models.Clear();
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
				BitmapImage bmi = new BitmapImage();
				bmi.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
				bmi.BeginInit();
				bmi.CacheOption = BitmapCacheOption.OnLoad;
				bmi.UriSource = new Uri(filePath, UriKind.Absolute);
				bmi.EndInit();
				_prop.Image = bmi.Clone(); //To ensure the cache is buted and reloading a modified version of the same uri works.
				bmi.UriSource = null;
			}
			catch (Exception ex)
			{
				Logging.Error(ex, "An error occurred loading the image for a prop.");
			}
		}

		public IEnumerable<ElementModel> GetLeafNodes()
		{
			return _prop.GetLeafNodes();
		}

		public ElementModel CreateNode(string name, ElementModel parent = null, bool oneBasedNaming = false)
		{
			if (parent == null)
			{
				parent = _prop.RootNode;
			}

			ElementModel em = new ElementModel(Uniquify(name, oneBasedNaming?1:2), parent);
			parent.AddChild(em);
			_models.Add(em.Id, em);
			return em;
		}

		private ElementModel CreateNode(string name, Guid id, ElementModel parent)
		{
			if (parent == null)
			{
				parent = _prop.RootNode;
			}
			ElementModel em = new ElementModel(Uniquify(name), parent)
			{
				Id = id
			}; 
			parent.AddChild(em);
			_models.Add(em.Id, em);
			return em;
		}

		public ElementModel FindOrCreateElementModelTree(ElementModel elementModel, ElementModel parent)
		{
			var em = GetModel(elementModel.Id);
			if (em == null)
			{
				em = CreateNode(elementModel.Name, elementModel.Id, parent);
				if (elementModel.IsLightNode)
				{
					em.LightSize = elementModel.LightSize;
					em.Lights = elementModel.Lights;
					em.Order = OrderExists(elementModel.Order) ? GetNextOrder() : elementModel.Order;
				}
				
				foreach (var child in elementModel.Children)
				{
					FindOrCreateElementModelTree(child, em);
				}
			}
			else
			{
				AddToParent(em, parent);
			}
			return em;
		}

		public ElementModel CreateElementModelTree(ElementModel elementModel, ElementModel parent, string name = null, bool recursed = false)
		{
			var em = CreateNode(recursed?parent.Name:name, parent, recursed);

			if (elementModel.IsLightNode)
			{
				em.LightSize = elementModel.LightSize;
				foreach (var elementModelLight in elementModel.Lights)
				{
					AddLightToTarget(new Point(elementModelLight.X, elementModelLight.Y), em);
				}
				em.Order = GetNextOrder();
			}
			
			foreach (var child in elementModel.Children)
			{
				CreateElementModelTree(child, em, string.Empty, true);
			}
			
			return em;
		}

		public ElementModel GetModel(Guid id)
		{
			ElementModel em;
			_models.TryGetValue(id, out em);
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

			ElementModel em = new ElementModel(Uniquify($"{target.Name}", 1), order.Value, target);
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

		public ElementModel AddLight(ElementModel target, Point p)
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

		public ElementModel GetElementModel(Guid id)
		{
			ElementModel em;
			_models.TryGetValue(id, out em);
			return em;
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

		public string Uniquify(string name, int orderIndex = 2, ElementModel renameModel=null)
		{
			if (_models.Values.Except(new []{renameModel}).Any(x => x.Name == name))
			{
				string originalName = name;
				bool unique;
				int counter = orderIndex;
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

		private bool OrderExists(int order)
		{
			return _prop.RootNode.GetLeafEnumerator().Any(x => x.Order==order);
		}

	}
}
