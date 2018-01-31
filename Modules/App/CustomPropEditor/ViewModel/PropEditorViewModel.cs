using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Media.Media3D;
using Common.WPFCommon.Command;
using Common.WPFCommon.ViewModel;
using VixenModules.App.CustomPropEditor.Import;
using VixenModules.App.CustomPropEditor.Import.XLights;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;

namespace VixenModules.App.CustomPropEditor.ViewModel
{
	public class PropEditorViewModel: BindableBase
	{
	    private PerspectiveCamera _camera;
        private ElementCandidate _selectedItem;
	    private List<LightNode> _lightNodes;
	    private Prop _prop;
	    private ObservableCollection<Visual3D> _shapes = new ObservableCollection<Visual3D>();

	    

	    public PropEditorViewModel()
	    {
	        Camera = new PerspectiveCamera()
	        {
	            Position = new Point3D(0, 0, 10),
	            LookDirection = new Vector3D(0, 0, -10),
	            UpDirection = new Vector3D(0, 1, 0),
	            FieldOfView = 60,
	        };
            ImportCommand = new RelayCommand<string>(ImportModel);
        }

	    public async void LoadProp()
	    {
            Prop = new Prop();
            //Prop.LoadTestData();
	        SelectedItem = new ElementCandidate();
	        LightNodes = Prop.GetLeafNodes().SelectMany(x => x.Lights).ToList();
	        //PopulateLightShapes();
	    }

	    //private void PopulateLightShapes()
	    //{
     //       Shapes.Clear();
     //       Shapes.Add(new DefaultLights());
            
	    //    foreach (var lightNode in LightNodes)
	    //    {
	    //        Shapes.Add(new EllipsoidVisual3D
	    //        {
	    //            Center = new Point3D(lightNode.Center.X / 10, 1 - lightNode.Center.Y / 10, 0),
	    //            Material = new DiffuseMaterial(Brushes.White),
     //               BackMaterial = null,
     //               RadiusX = .1,
     //               RadiusY = .1,
     //               RadiusZ = 0
	    //        });
	    //    }
	    //}

	    public Prop Prop
	    {
	        get { return _prop; }
	        set
	        {
	            if (Equals(value, _prop)) return;
	            _prop = value;
	            LightNodes = Prop.GetLeafNodes().SelectMany(x => x.Lights).ToList();
                //PopulateLightShapes();
                OnPropertyChanged("Prop");
	        }
	    }


	    public List<LightNode> LightNodes
	    {
	        get { return _lightNodes; }
	        set
	        {
	            if (Equals(value, _lightNodes)) return;
	            _lightNodes = value;
	            OnPropertyChanged("LightNodes");
	        }
	    }

	    public ObservableCollection<Visual3D> Shapes
	    {
	        get { return _shapes; }
	        set
	        {
	            if (Equals(value, _shapes)) return;
	            _shapes = value;
	            OnPropertyChanged("Shapes");
	        }
	    }


	    public ElementCandidate SelectedItem
		{
			get { return _selectedItem; }
			set
			{
				if (_selectedItem != value)
				{
					_selectedItem = value;
					OnPropertyChanged("SelectedItem");
				}
			}
		}

	    public PerspectiveCamera Camera
	    {
	        get { return _camera; }
	        set
	        {
	            if (Equals(value, _camera)) return;
	            _camera = value;
	            OnPropertyChanged("Camera");
	        }
	    }

        public void MoveLight(LightNode node, Point p)
		{
			var halfSize = node.Size / 2;

			if (p.X > _prop.Width - halfSize)
			{
				p.X = _prop.Width - halfSize;
			}
			else if (p.X < halfSize)
			{
				p.X = halfSize;
			}
			//var y = node.Center.Y + verticalChange;
			if (p.Y > _prop.Height - halfSize)
			{
				p.Y = _prop.Height - halfSize;
			}
			else if (p.Y < halfSize)
			{
				p.Y = halfSize;
			}

			//Point p = new Point(horizontalChange, verticalChange);
			node.Center = p;
		}

	    private async void ImportModel(string type)
	    {
            IOService service = new FileService();
	        string path = service.OpenFileDialog(Environment.SpecialFolder.MyDocuments.ToString(), "xModel (*.xmodel)|*.xmodel");
            IModelImport import = new XModelImport();
	        Prop = await import.ImportAsync(path);
	    }

        #region Menu Commands

        public RelayCommand<string> ImportCommand { get; private set; }

        #endregion

    }


}
