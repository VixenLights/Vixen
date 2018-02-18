using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Common.WPFCommon.ViewModel;
using Point = System.Windows.Point;

namespace VixenModules.App.CustomPropEditor.Model
{
	public class Prop: BindableBase
	{
		private Bitmap _image;
	    private string _name;
	    private ElementModel _rootNode;
	    private int _height;
	    private int _width;
	    private Dictionary<Guid, ElementModel> _instances;

        public Prop(string name):this()
	    {
	        Name = name;
        }

	    public Prop()
	    {
	        _rootNode = new ElementModel();
            _instances = new Dictionary<Guid, ElementModel>();
	        Width = 800;
	        Height = 600;
	        Name = "Default";
        }

	    public void LoadTestData()
	    {
	        _rootNode = new ElementModel();
            Name = "Snowflake";
	        Image = new Bitmap(800, 600);
	        
	        var branches = new ElementModel
	        {
	            Name = "Branches"
	        };

	        _rootNode.Children.Add(branches);

	        var branch1 = new ElementModel
	        {
	            Name = "Branch 1"
	        };
	        branches.Children.Add(branch1);


	        var model = new ElementModel
	        {
	            Name = "Px-1"
	        };
	        model.Lights.Add(new Light(new Point(10, 20), 6));
	        branch1.Children.Add(model);

	        model = new ElementModel
	        {
	            Name = "Px-2"
	        };
	        model.Lights.Add(new Light(new Point(20, 20), 6));
	        branch1.Children.Add(model);

	        var branch2 = new ElementModel
	        {
	            Name = "Branch 2"
	        };
	        branches.Children.Add(branch2);


	        model = new ElementModel
	        {
	            Name = "Px-3"
	        };
	        model.Lights.Add(new Light(new Point(30, 20), 6));
	        branch2.Children.Add(model);

	        model = new ElementModel
	        {
	            Name = "Px-4"
	        };
	        model.Lights.Add(new Light(new Point(40, 20), 6));
	        branch2.Children.Add(model);

	        var branch3 = new ElementModel
	        {
	            Name = "Branch 3"
	        };
	        branches.Children.Add(branch3);

	        model = new ElementModel
	        {
	            Name = "Px-5"
	        };
	        model.Lights.Add(new Light(new Point(40, 10), 6));
	        branch3.Children.Add(model);

	        model = new ElementModel
	        {
	            Name = "Px-6"
	        };
	        model.Lights.Add(new Light(new Point(40, 40), 6));
	        branch3.Children.Add(model);

	        //OnPropertyChanged("ElementCandidates");
	    }

	    public ElementModel RootNode
	    {
	        get { return _rootNode; }
	        private set
	        {
	            if (Equals(value, _rootNode)) return;
	            _rootNode = value;
	            OnPropertyChanged(nameof(RootNode));
	        }
	    }


	    public void AddElementModel(ElementModel ec)
	    {
            _rootNode.Children.Add(ec);
	    }

	    public void AddElementModels(IEnumerable<ElementModel> elementCandidates)
	    {
	        foreach (var elementCandidate in elementCandidates)
	        {
	            _rootNode.Children.Add(elementCandidate);
	        }
	    }

	    public bool RemoveFromParent(ElementModel em, ElementModel parent)
	    {
	        return em.RemoveFromParent(parent);
	    }

	    public string Name
	    {
	        get { return _name; }
	        set
	        {
	            if (value == _name) return;
	            _name = value;
	            _rootNode.Name = value;
	            OnPropertyChanged(nameof(Name));
	        }
	    }

	    public Bitmap Image
		{
			get { return _image; }
			set
			{
				if (value != null && !value.Equals(_image))
				{
					_image = value;
					OnPropertyChanged(nameof(Image));
				    Height = _image.Height;
				    Width = _image.Width;
				}
			}
		}

	    public int Height
	    {
	        get { return _height; }
	        set
	        {
	            if (value == _height) return;
	            _height = value;
	            OnPropertyChanged(nameof(Height));
	        }
	    }

	    public int Width
	    {
	        get { return _width; }
	        set
	        {
	            _width = value;
	            OnPropertyChanged(nameof(Width));
	        }
	    }

	    public IEnumerable<ElementModel> GetLeafNodes()
	    {
	        // Don't want to return the root node.
	        // note: this may very well return duplicate nodes, if they are part of different groups.
	        return _rootNode.Children.SelectMany(x => x.GetLeafEnumerator());
	    }

	    #region Utilities

	    private int GetMaxOrder()
	    {
	        return _rootNode.GetLeafEnumerator().Max(x => x.Order);
	    }

	    #endregion
    }
}
