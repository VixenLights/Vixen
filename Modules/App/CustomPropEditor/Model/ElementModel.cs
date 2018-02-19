using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.WpfPropertyGrid;
using Catel.Collections;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
    /// <summary>
    /// Symbolic of an ElementNode in Vixen Core
    /// </summary>
    [Catel.ComponentModel.DisplayName("Element Model")]
	public class ElementModel : BindableBase, IEqualityComparer<ElementModel>, IEquatable<ElementModel>
	{
        private const int DefaultLightSize = 3;
	    private ObservableCollection<Light> _lights;
	    private ObservableCollection<ElementModel> _children;
	    private ObservableCollection<ElementModel> _parents;
        private int _order;
	    private string _name;
	    private int _lightSize;
	    

	    public ElementModel()
		{
			Lights = new ObservableCollection<Light>();
            Children = new ObservableCollection<ElementModel>();
            Parents = new ObservableCollection<ElementModel>();
            Id = Guid.NewGuid();
		    LightSize = DefaultLightSize;
		}

	    public ElementModel(string name):this()
	    {
	        Name = name;
	    }

	    public ElementModel(string name, int order) : this(name)
	    {
	        Name = name;
	        Order = order;
	    }

        public ElementModel(string name, ElementModel parent):this(name)
	    {
            Parents.Add(parent);
	    }
	    
        [Browsable(false)]
		public Guid Id { get; protected set; }

	    [PropertyOrder(0)]
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

	    [PropertyOrder(1)]
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

	    [DisplayName("String Type")]
	    [PropertyOrder(2)]
	    public bool IsString
	    {
	        get { return _lights.Count > 1; }
	    }

	    [DisplayName("Light Count")]
	    [PropertyOrder(3)]
	    public int LightCount
	    {
	        get { return _lights.Count; }
	    }

	    [DisplayName("Light Size")]
	    [PropertyOrder(4)]
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

	    [Browsable(false)]
	    public bool IsLeaf
	    {
	        get { return !Children.Any(); }
	    }


        [Browsable(false)]
        public ObservableCollection<ElementModel> Children
	    {
	        get { return _children; }
	        set
	        {
	            if (Equals(value, _children)) return;
	            _children = value;
	            OnPropertyChanged(nameof(Children));
	        }
	    }

	    [Browsable(false)]
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

	    [Browsable(false)]
        public ObservableCollection<Light> Lights
		{
			get { return _lights; }
			set
			{
			    if (Equals(value, _lights)) return;
			    _lights = value;
			    OnPropertyChanged(nameof(Lights));
			    OnPropertyChanged(nameof(IsString));
                OnPropertyChanged(nameof(LightCount));
			}
		}

	    public bool RemoveFromParent(ElementModel parent)
	    {
	        return Parents.Remove(parent);
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

	    public Light AddLight(Point center)
	    {
	        if (IsLeaf && Parents.Any())
	        {
	            return AddLight(center, LightSize);
            }
	        
            ElementModel em = new ElementModel("New Name", this);
            AddChild(em);

	        return em.AddLight(center);
	        
	    }

        protected Light AddLight(Point center, double size)
	    {
	        if (Children.Any())
	        {
                throw new ArgumentException("Non leaf not cannot have lights!");
	        }
            var ln = new Light(center, size);
            Lights.Add(ln);
	        OnPropertyChanged(nameof(IsString));
	        OnPropertyChanged(nameof(LightCount));
	        return ln;
	    }

	    public bool RemoveLight(Light light)
	    {
	        var success = Lights.Remove(light);
	        OnPropertyChanged(nameof(IsString));
	        OnPropertyChanged(nameof(LightCount));
	        return success;
	    }

	    
	    private void UpdateLightSize()
	    {
	        Lights.ForEach(x => x.Size = LightSize);
	    }

	    public IEnumerable<ElementModel> GetLeafEnumerator()
	    {
	        if (IsLeaf)
	        {
	            // Element is already an enumerable, so AsEnumerable<> won't work.
	            return (new[] { this });
	        }
	        else
	        {
	            return Children.SelectMany(x => x.GetLeafEnumerator());
	        }
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
