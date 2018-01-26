using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using Common.WPFCommon.Command;
using Common.WPFCommon.ViewModel;
using VixenModules.Preview.VixenPreview.CustomPropEditor.Import;
using VixenModules.Preview.VixenPreview.CustomPropEditor.Import.XLights;
using VixenModules.Preview.VixenPreview.CustomPropEditor.Model;
using VixenModules.Preview.VixenPreview.CustomPropEditor.Services;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.ViewModel
{
	public class PropEditorViewModel: BindableBase
	{
		private ElementCandidate _selectedItem;
	    private List<LightNode> _lightNodes;
	    private Prop _prop;

	    public PropEditorViewModel()
	    {
	        ImportCommand = new RelayCommand<string>(ImportModel);
        }

	    public async void LoadProp()
	    {
            Prop = new Prop();
            Prop.LoadTestData();
	        SelectedItem = new ElementCandidate();
	        LightNodes = Prop.GetLeafNodes().SelectMany(x => x.Lights).ToList();
	    }

	    public Prop Prop
	    {
	        get { return _prop; }
	        set
	        {
	            if (Equals(value, _prop)) return;
	            _prop = value;
	            LightNodes = Prop.GetLeafNodes().SelectMany(x => x.Lights).ToList();
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

		public void MoveLight(LightNode node, double horizontalChange, double verticalChange)
		{
			var halfSize = node.Size / 2;

			var x = node.Center.X + horizontalChange;
			if (x > _prop.Width - halfSize)
			{
				x = _prop.Width - halfSize;
			}
			else if (x < halfSize)
			{
				x = halfSize;
			}
			var y = node.Center.Y + verticalChange;
			if (y > _prop.Height - halfSize)
			{
				y = _prop.Height - halfSize;
			}
			else if (y < halfSize)
			{
				y = halfSize;
			}

			Point p = new Point(x, y);
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
