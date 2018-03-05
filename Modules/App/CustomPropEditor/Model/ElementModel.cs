using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Catel.Collections;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
	/// <summary>
	/// Symbolic of an ElementNode in Vixen Core
	/// </summary>
	public class ElementModel : BindableBase, IEqualityComparer<ElementModel>, IEquatable<ElementModel>
	{
		private const int DefaultLightSize = 3;
		private ObservableCollection<Light> _lights;
		private ObservableCollection<ElementModel> _children;
		private ObservableCollection<ElementModel> _parents;
		private int _order;
		private string _name;
		private int _lightSize;

		#region Constructors

		public ElementModel()
		{
			Lights = new ObservableCollection<Light>();
			Children = new ObservableCollection<ElementModel>();
			Parents = new ObservableCollection<ElementModel>();
			Id = Guid.NewGuid();
			LightSize = DefaultLightSize;
		}

		public ElementModel(string name) : this()
		{
			Name = name;
		}

		public ElementModel(string name, int order) : this(name)
		{
			Name = name;
			Order = order;
		}

		public ElementModel(string name, ElementModel parent) : this(name)
		{
			Parents.Add(parent);
		}

		public ElementModel(string name, int order, ElementModel parent) : this(name)
		{
			Parents.Add(parent);
			Order = order;
		}

		#endregion

		#region Id

		public Guid Id { get; protected set; }

		#endregion

		#region Name

		public string Name
		{
			get { return _name; }
			set
			{
				if (value == _name) return;
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		#endregion

		#region Order

		public int Order
		{
			get
			{
				if (IsLeaf)
				{
					return _order;
				}

				return -1;
			}
			set
			{
				if (value == _order) return;
				_order = value;
				OnPropertyChanged(nameof(Order));
			}
		}

		#endregion Order

		#region Element Type

		public ElementType ElementType
		{
			get
			{
				if (LightCount > 1)
				{
					return ElementType.String;
				}
			    if (LightCount == 1)
				{
					return ElementType.Node;
				}

				return ElementType.Group;

			}
		} 

		#endregion

		#region Light Count

		public int LightCount => _lights.Count;

		#endregion

		#region Light Size

		public int LightSize
		{
			get { return _lightSize; }
			set
			{
				if (value == _lightSize) return;
				_lightSize = value;
				UpdateLightSize();
				OnPropertyChanged(nameof(LightSize));
			}
		}

		#endregion

		#region IsLeaf

		public bool IsLeaf => !Children.Any();

		#endregion

		#region IsLightNode

		public bool IsLightNode => IsLeaf && Lights.Any();

		#endregion

		#region IsGroupNode

		public bool IsGroupNode => !Lights.Any();

	    public bool CanAddGroupNodes => IsGroupNode &&  (!Children.Any() || Children.Any(c => c.IsGroupNode));

		public bool CanAddLeafNodes => IsGroupNode && (!Children.Any() || Children.Any(c => c.IsLeaf));

		#endregion

		#region Children

		public ObservableCollection<ElementModel> Children
		{
			get { return _children; }
			set
			{
				if (Equals(value, _children)) return;
				_children = value;
				OnPropertyChanged(nameof(Children));
				OnPropertyChanged(nameof(IsLeaf));
				OnPropertyChanged(nameof(IsGroupNode));
			}
		}

		#endregion

		#region Parents

		public ObservableCollection<ElementModel> Parents
		{
			get { return _parents; }
			set
			{
				if (Equals(value, _parents)) return;
				_parents = value;
				OnPropertyChanged(nameof(Parents));
			}
		}

		#endregion

		#region Lights

		public ObservableCollection<Light> Lights
		{
			get { return _lights; }
			set
			{
				if (Equals(value, _lights)) return;
				_lights = value;
				OnPropertyChanged(nameof(Lights));
				OnPropertyChanged(nameof(LightCount));
				OnPropertyChanged(nameof(IsGroupNode));
			    OnPropertyChanged(nameof(ElementType));
            }
		}

		#endregion

		public bool RemoveParent(ElementModel parent)
		{
			bool success = Parents.Remove(parent);
			parent.RemoveChild(this);
			if (!Parents.Any())
			{
				//We are now orphaned and need to clean up
				if (IsLeaf)
				{
					//We are at the bottom and just need to remove our lights
					Lights.Clear();
				}
				else
				{
					foreach (var child in Children.ToList())
					{
						child.RemoveParent(this);
					}
				}
			}

			return success;
		}

		public void AddParent(ElementModel parent)
		{
			Parents.Add(parent);
		}

		public void AddChild(ElementModel em)
		{
			Children.Add(em);
		}

		public bool RemoveChild(ElementModel child)
		{
			return Children.Remove(child);
		}

		internal void AddLight(Light ln)
		{
			if (!IsLeaf)
			{
				throw new ArgumentException("Non leaf model cannot have lights!");
			}
			Lights.Add(ln);
			ln.ParentModelId = Id;
			OnPropertyChanged(nameof(ElementType));
			OnPropertyChanged(nameof(LightCount));
		}

		public bool RemoveLight(Light light)
		{
			var success = Lights.Remove(light);
			light.ParentModelId = Guid.Empty;
			OnPropertyChanged(nameof(ElementType));
			OnPropertyChanged(nameof(LightCount));
			return success;
		}


		private void UpdateLightSize()
		{
			Lights.ForEach(x => x.Size = LightSize);
		}

		public IEnumerable<ElementModel> GetNodeEnumerator()
		{
			return (new[] { this }).Concat(Children.SelectMany(x => x.GetNodeEnumerator()));
		}

		public IEnumerable<ElementModel> GetLeafEnumerator()
		{
			if (IsLeaf)
			{
				return (new[] { this });
			}

			return Children.SelectMany(x => x.GetLeafEnumerator());
		}

		public IEnumerable<ElementModel> GetNonLeafEnumerator()
		{
			if (IsLeaf)
			{
				return Enumerable.Empty<ElementModel>();
			}
			
			return (new[] { this }).Concat(Children.SelectMany(x => x.GetNonLeafEnumerator()));
		}

		public bool Equals(ElementModel x, ElementModel y)
		{
			return y != null && x != null && x.Id == y.Id;
		}

		public int GetHashCode(ElementModel obj)
		{
			return obj.Id.GetHashCode();
		}

		public bool Equals(ElementModel other)
		{
			return other != null && Id == other.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		
	}
}
