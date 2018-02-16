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
	    private ElementCandidate _rootNode;
	    private int _height;
	    private int _width;

	    private ObservableCollection<Light> _lightNodes;

	    public Prop(string name):this()
	    {
	        Name = name;
        }

	    public Prop()
	    {
	        _rootNode = new ElementCandidate(Name);
            _lightNodes = new ObservableCollection<Light>();

	        Name = "Default";
	        //ElementCandidates.Add(_rootNode);
	        Width = 800;
	        Height = 600;
	    }

	    public void LoadTestData()
	    {
	        _rootNode = new ElementCandidate();
            Name = "Snowflake";
	        Image = new Bitmap(800, 600);
	        
	        var branches = new ElementCandidate
	        {
	            Name = "Branches"
	        };

	        _rootNode.Children.Add(branches);

	        var branch1 = new ElementCandidate
	        {
	            Name = "Branch 1"
	        };
	        branches.Children.Add(branch1);


	        var model = new ElementCandidate
	        {
	            Name = "Px-1"
	        };
	        model.Lights.Add(new Light(new Point(10, 20), 6));
	        branch1.Children.Add(model);

	        model = new ElementCandidate
	        {
	            Name = "Px-2"
	        };
	        model.Lights.Add(new Light(new Point(20, 20), 6));
	        branch1.Children.Add(model);

	        var branch2 = new ElementCandidate
	        {
	            Name = "Branch 2"
	        };
	        branches.Children.Add(branch2);


	        model = new ElementCandidate
	        {
	            Name = "Px-3"
	        };
	        model.Lights.Add(new Light(new Point(30, 20), 6));
	        branch2.Children.Add(model);

	        model = new ElementCandidate
	        {
	            Name = "Px-4"
	        };
	        model.Lights.Add(new Light(new Point(40, 20), 6));
	        branch2.Children.Add(model);

	        var branch3 = new ElementCandidate
	        {
	            Name = "Branch 3"
	        };
	        branches.Children.Add(branch3);

	        model = new ElementCandidate
	        {
	            Name = "Px-5"
	        };
	        model.Lights.Add(new Light(new Point(40, 10), 6));
	        branch3.Children.Add(model);

	        model = new ElementCandidate
	        {
	            Name = "Px-6"
	        };
	        model.Lights.Add(new Light(new Point(40, 40), 6));
	        branch3.Children.Add(model);

	        //OnPropertyChanged("ElementCandidates");
	    }

	    public ElementCandidate RootNode
	    {
	        get { return _rootNode; }
	    }

	    public ObservableCollection<ElementCandidate> ElementCandidates
	    {
	        get { return new ObservableCollection<ElementCandidate>(new []{_rootNode}); }
	    }

        public void AddElementCandidate(ElementCandidate ec)
	    {
            _rootNode.Children.Add(ec);
	    }

	    public void AddElementCandidates(IEnumerable<ElementCandidate> elementCandidates)
	    {
	        foreach (var elementCandidate in elementCandidates)
	        {
	            _rootNode.Children.Add(elementCandidate);
	        }
	        //_rootNode.Children.AddRange(elementCandidates);
	    }

	    //public ElementCandidate FindElementCandiateForLightNode()
	    //{

	    //}

	    public string Name
	    {
	        get { return _name; }
	        set
	        {
	            if (value == _name) return;
	            _name = value;
	            _rootNode.Name = value;
	            OnPropertyChanged("Name");
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
					OnPropertyChanged("Image");
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
	            OnPropertyChanged("Height");
	        }
	    }

	    public int Width
	    {
	        get { return _width; }
	        set
	        {
	            _width = value;
	            OnPropertyChanged("Width");
	        }
	    }

	    public IEnumerable<ElementCandidate> GetLeafNodes()
	    {
	        // Don't want to return the root node.
	        // note: this may very well return duplicate nodes, if they are part of different groups.
	        return _rootNode.Children.SelectMany(x => x.GetLeafEnumerator());
	    }
    }
}
