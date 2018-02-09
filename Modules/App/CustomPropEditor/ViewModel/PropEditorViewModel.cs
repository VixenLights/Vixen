using System;
using System.Windows;
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
	    private Prop _prop;
	    private DrawingPanelViewModel _drawingPanelViewModel;
	    
	    public PropEditorViewModel()
	    {
	        DrawingPanelViewModel = new DrawingPanelViewModel();
            ImportCommand = new RelayCommand<string>(ImportModel);
	        NewPropCommand = new RelayCommand(NewProp);
            Prop = new Prop();
        }

	    

	    public async void LoadProp()
	    {
           
	    }

	    public Prop Prop
	    {
	        get { return _prop; }
	        set
	        {
	            if (Equals(value, _prop)) return;
	            _prop = value;
	            OnPropertyChanged("Prop");
	            DrawingPanelViewModel.Prop = _prop;
	        }
	    }

	    public ElementCandidate SelectedElementCandidate { get; set; }

	    public DrawingPanelViewModel DrawingPanelViewModel
	    {
	        get { return _drawingPanelViewModel; }
	        set
	        {
	            if (Equals(value, _drawingPanelViewModel)) return;
	            _drawingPanelViewModel = value;
	            OnPropertyChanged("DrawingPanelViewModel");
	        }
	    }

	    private async void ImportModel(string type)
	    {
            IOService service = new FileService();
	        string path = service.OpenFileDialog(Environment.SpecialFolder.MyDocuments.ToString(), "xModel (*.xmodel)|*.xmodel");
	        if (path != null)
	        {
	            IModelImport import = new XModelImport();
	            Prop = await import.ImportAsync(path);
            }
            
	    }

	    private void NewProp()
	    {
            //Write code to prompt for name
            Prop = new Prop("Default 1");
	    }

	    public void UpdateCoordinates(Point p)
	    {
	        //Coordinates = string.Format("{0}, {1}", Math.Floor(p.X), Math.Floor(p.Y));
	    }

        #region Menu Commands

        public RelayCommand<string> ImportCommand { get; private set; }

        public RelayCommand NewPropCommand { get; private set; }

        #endregion
        
    }


}
