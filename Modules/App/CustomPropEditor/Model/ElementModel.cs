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
	[Serializable]
	public class ElementModel : BindableBase, IEqualityComparer<ElementModel>, IEquatable<ElementModel>
	{
		private const int DefaultLightSize = 3;
		private ObservableCollection<Light> _lights;
		private ObservableCollection<ElementModel> _children;
		private ObservableCollection<Guid> _parents;
		private int _order;
		private string _name;
		private int _lightSize;
		private FaceComponent _faceComponent;

		#region Constructors

		public ElementModel()
		{
			Lights = new ObservableCollection<Light>();
			Children = new ObservableCollection<ElementModel>();
			Parents = new ObservableCollection<Guid>();
			Id = Guid.NewGuid();
			LightSize = DefaultLightSize;
			FaceComponent = FaceComponent.None;
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
			Parents.Add(parent.Id);
		}

		public ElementModel(string name, int order, ElementModel parent) : this(name)
		{
			Parents.Add(parent.Id);
			Order = order;
		}

		#endregion

		#region Id

		public Guid Id { get; internal set; }

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
			get
			{
				if (IsLeaf)
				{
					return _lightSize;
				}

				return GetLeafEnumerator().FirstOrDefault()?.LightSize??_lightSize;
			}
			set
			{
				if (value == _lightSize) return;
			    _lightSize = value>0?value:1;
				UpdateLightSize(_lightSize);
				OnPropertyChanged(nameof(LightSize));
			}
		}

		#endregion

		#region Face Component

		public FaceComponent FaceComponent
		{
			get => _faceComponent;
			set
			{
				if (Equals(value, _faceComponent)) return;
				_faceComponent = value;
				OnPropertyChanged(nameof(FaceComponent));
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

		public bool CanAddLightNodes => !IsRootNode && (!Children.Any() || IsLightNode || Children.All(c => !c.IsGroupNode));

		public bool IsRootNode => !Parents.Any();

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
				OnPropertyChanged(nameof(Order));
				OnPropertyChanged(nameof(IsLightNode));
			}
		}

		#endregion

		#region Parents

		public ObservableCollection<Guid> Parents
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
				OnPropertyChanged(nameof(IsLightNode));
			}
		}

		#endregion

		public static bool IsPhoneme(FaceComponent faceComponent)
		{
			switch (faceComponent)
			{
				case FaceComponent.None:
				case FaceComponent.EyesClosed:
				case FaceComponent.EyesOpen:
				case FaceComponent.Outlines:
					return false;
				default:
					return true;
			}
		}

		public bool RemoveParent(ElementModel parent)
		{
			bool success = Parents.Remove(parent.Id);
			parent.RemoveChild(this);
			if (!Parents.Any())
			{
				//We are now orphaned and need to clean up
				if (IsLeaf)
				{
					//We are at the bottom and just need to remove our lights
					Lights.Clear();
					OnPropertyChanged(nameof(LightCount));
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
			Parents.Add(parent.Id);
		}

		public void AddChild(ElementModel em)
		{
			Children.Add(em);
			OnPropertyChanged(nameof(LightSize));
		}

		public bool RemoveChild(ElementModel child)
		{
			var status =  Children.Remove(child);
			OnPropertyChanged(nameof(LightSize));
			return status;
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
			OnPropertyChanged(nameof(IsLightNode));
		}

		public bool RemoveLight(Light light)
		{
			var success = Lights.Remove(light);
			light.ParentModelId = Guid.Empty;
			OnPropertyChanged(nameof(ElementType));
			OnPropertyChanged(nameof(LightCount));
			OnPropertyChanged(nameof(IsLightNode));
			return success;
		}


		private void UpdateLightSize(int size)
		{
			if (IsLeaf)
			{
				Lights.ForEach(x => x.Size = size);
			}
			else
			{
				foreach (var elementModel in Children)
				{
					elementModel.LightSize = size;
				}
			}
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
