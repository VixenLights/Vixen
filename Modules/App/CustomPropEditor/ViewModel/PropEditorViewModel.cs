using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Catel.Data;
using Catel.MVVM;
using Common.WPFCommon.Command;
using Common.WPFCommon.ViewModel;
using VixenModules.App.CustomPropEditor.Import;
using VixenModules.App.CustomPropEditor.Import.XLights;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;

namespace VixenModules.App.CustomPropEditor.ViewModel
{
	public class PropEditorViewModel: ViewModelBase
	{
	    private Prop _prop;
	    private DrawingPanelViewModel _drawingPanelViewModel;
	    private ObservableCollection<ElementModel> _selectedElementCandidates;

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

	    #region Prop model property

	    /// <summary>
	    /// Gets or sets the Prop value.
	    /// </summary>
	    [Model]
	    public Prop Prop
	    {
	        get { return GetValue<Prop>(PropProperty); }
	        private set
	        {
	            SetValue(PropProperty, value);
	            DrawingPanelViewModel = new DrawingPanelViewModel(value);
	            ElementTreeViewModel = new ElementTreeViewModel(value);
	        }
	    }

	    /// <summary>
	    /// Prop property data.
	    /// </summary>
	    public static readonly PropertyData PropProperty = RegisterProperty("Prop", typeof(Prop));

	    #endregion

	    #region DrawingPanelViewModel property

	    /// <summary>
	    /// Gets or sets the DrawingPanelViewModel value.
	    /// </summary>
	    public DrawingPanelViewModel DrawingPanelViewModel
	    {
	        get { return GetValue<DrawingPanelViewModel>(DrawingPanelViewModelProperty); }
	        set { SetValue(DrawingPanelViewModelProperty, value); }
	    }

	    /// <summary>
	    /// DrawingPanelViewModel property data.
	    /// </summary>
	    public static readonly PropertyData DrawingPanelViewModelProperty = RegisterProperty("DrawingPanelViewModel", typeof(DrawingPanelViewModel));

	    #endregion

	    #region ElementTreeViewModel property

	    /// <summary>
	    /// Gets or sets the ElementTreeViewModel value.
	    /// </summary>
	    public ElementTreeViewModel ElementTreeViewModel
	    {
	        get { return GetValue<ElementTreeViewModel>(ElementTreeViewModelProperty); }
	        set { SetValue(ElementTreeViewModelProperty, value); }
	    }

	    /// <summary>
	    /// ElementTreeViewModel property data.
	    /// </summary>
	    public static readonly PropertyData ElementTreeViewModelProperty = RegisterProperty("ElementTreeViewModel", typeof(ElementTreeViewModel));

	    #endregion

	   
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

	    #region Menu Commands

        public RelayCommand<string> ImportCommand { get; private set; }

        public RelayCommand NewPropCommand { get; private set; }

        #endregion
        
    }


}
