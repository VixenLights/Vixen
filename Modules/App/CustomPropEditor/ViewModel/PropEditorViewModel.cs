using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
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
	    public PropEditorViewModel()
	    {
	        DrawingPanelViewModel = new DrawingPanelViewModel();
            ImportCommand = new RelayCommand<string>(ImportModel);
	        NewPropCommand = new RelayCommand(NewProp);
	        AddLightCommand = new RelayCommand<Point>(AddLightAt);
            Prop = new Prop();
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
                UnregisterModelEvents();
	            DrawingPanelViewModel = new DrawingPanelViewModel(value);
	            ElementTreeViewModel = new ElementTreeViewModel(value);
                RegisterModelEvents();
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

	    private void RegisterModelEvents()
	    {
            
            ElementTreeViewModel.SelectedItems.CollectionChanged += ElementViewModel_SelectedItemsChanged;
            DrawingPanelViewModel.SelectedItems.CollectionChanged += DrawingViewModel_SelectedItemsChanged;
	    }

	    private void UnregisterModelEvents()
	    {
	        if (ElementTreeViewModel != null)
	        {
	            ElementTreeViewModel.SelectedItems.CollectionChanged -= ElementViewModel_SelectedItemsChanged;
	        }

	        if (DrawingPanelViewModel != null)
	        {
	            DrawingPanelViewModel.SelectedItems.CollectionChanged -= DrawingViewModel_SelectedItemsChanged;
	        }

        }

        private void DrawingViewModel_SelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
	    {
	        
	    }

	    private void ElementViewModel_SelectedItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            DrawingPanelViewModel.SelectLights(ElementTreeViewModel.SelectedItems);
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

	    public void AddLightAt(Point p)
	    {
	        if (ElementTreeViewModel.SelectedItems.Count == 1)
	        {
	            var em = ElementTreeViewModel.SelectedItems.First();
	            if (em.IsLeaf && !em.Equals(Prop.RootNode))
	            {
	                em.AddLight(p);
	            }
	        }
	        Prop.RootNode.AddLight(p);
	        DrawingPanelViewModel.RefreshLightViewModels();
        }

        #region Menu Commands

        public RelayCommand<string> ImportCommand { get; private set; }

        public RelayCommand NewPropCommand { get; private set; }

	    public RelayCommand<Point> AddLightCommand { get; private set; }

        #endregion

    }


}
