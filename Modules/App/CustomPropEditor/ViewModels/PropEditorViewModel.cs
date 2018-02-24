using System;
using System.Linq;
using System.Windows;
using Catel;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Common.WPFCommon.Command;
using VixenModules.App.CustomPropEditor.Import;
using VixenModules.App.CustomPropEditor.Import.XLights;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;

namespace VixenModules.App.CustomPropEditor.ViewModels
{

   
    public class PropEditorViewModel:ViewModelBase
    {
        #region Fields
        private readonly IMessageService _messageService;
        private readonly IUIVisualizerService _uiVisualizerService;
        #endregion

        #region Constructors

        public PropEditorViewModel(IUIVisualizerService uiVisualizerService, IMessageService messageService)
        {
            Argument.IsNotNull(() => uiVisualizerService);
            Argument.IsNotNull(() => messageService);

            _uiVisualizerService = uiVisualizerService;
            _messageService = messageService;

            ImportCommand = new RelayCommand<string>(ImportModel);
            NewPropCommand = new RelayCommand(NewProp);
            AddLightCommand = new RelayCommand<Point>(AddLightAt);
            LoadImageCommand = new RelayCommand(LoadImage);
            Prop = PropModelServices.Instance().CreateProp();
        }

        #endregion

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
               // UnregisterModelEvents();
               // DrawingPanelViewModel = new DrawingPanelViewModel(value);
               // ElementTreeViewModel = new ElementTreeViewModel(value);
               // RegisterModelEvents();
            }
        }

        /// <summary>
        /// Prop property data.
        /// </summary>
        public static readonly PropertyData PropProperty = RegisterProperty("Prop", typeof(Prop));

        #endregion

        private async void ImportModel(string type)
        {
            var dependencyResolver = this.GetDependencyResolver();
            var openFileService = dependencyResolver.Resolve<IOpenFileService>();
            openFileService.IsMultiSelect = false;
            openFileService.InitialDirectory = Environment.SpecialFolder.MyDocuments.ToString();
            openFileService.Filter = "xModel (*.xmodel)|*.xmodel";
            if (await openFileService.DetermineFileAsync())
            {
                string path = openFileService.FileNames.First();
                if (!string.IsNullOrEmpty(path))
                {
                    IModelImport import = new XModelImport();
                    Prop = await import.ImportAsync(path);
                }
            }

        }

        #region Delete command

        private Command _deleteCommand;

        /// <summary>
        /// Gets the Delete command.
        /// </summary>
        public Command DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = new Command(Delete, CanDelete)); }
        }

        /// <summary>
        /// Method to invoke when the Delete command is executed.
        /// </summary>
        private void Delete()
        {
            //PropModelServices.Instance().RemoveElementModels(ElementTreeViewModel.SelectedItems);
            //DrawingPanelViewModel.RefreshLightViewModels();
        }

        /// <summary>
        /// Method to check whether the Delete command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool CanDelete()
        {
            return true;
            //return !ElementTreeViewModel.SelectedItems.Any(x => x.Equals(Prop.RootNode));
        }

        #endregion

        private void NewProp()
        {
            MessageBoxService mbs = new MessageBoxService();
            var name = mbs.GetUserInput("Please enter the model name.", "Create Model");

            Prop = PropModelServices.Instance().CreateProp(name);
        }

        public void AddLightAt(Point p)
        {
            //PropModelServices.Instance().AddLight(ElementTreeViewModel.SelectedItems.FirstOrDefault(), p);

           // DrawingPanelViewModel.RefreshLightViewModels();
        }

        public async void LoadImage()
        {
            var dependencyResolver = this.GetDependencyResolver();
            var openFileService = dependencyResolver.Resolve<IOpenFileService>();
            openFileService.IsMultiSelect = false;
            openFileService.InitialDirectory = Environment.SpecialFolder.MyPictures.ToString();
            openFileService.Filter = "Image Files(*.JPG;*.GIF;*.PNG)|*.JPG;*.GIF;*.PNG|All files (*.*)|*.*";
            if (await openFileService.DetermineFileAsync())
            {
                string path = openFileService.FileNames.First();
                if (!string.IsNullOrEmpty(path))
                {
                    PropModelServices.Instance().SetImage(path);
                }
            }
        }

        #region Menu Commands

        public RelayCommand<string> ImportCommand { get; private set; }

        public RelayCommand NewPropCommand { get; private set; }

        public RelayCommand<Point> AddLightCommand { get; private set; }

        public RelayCommand LoadImageCommand { get; private set; }

        #endregion


    }
}
