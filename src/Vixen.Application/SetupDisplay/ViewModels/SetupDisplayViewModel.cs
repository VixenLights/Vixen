#nullable enable
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Common.WPFCommon.Services;
using NLog;
using Vixen.Sys;
using Vixen.Sys.Managers;
using Vixen.Sys.Props;

namespace VixenApplication.SetupDisplay.ViewModels
{
    public class SetupDisplayViewModel : ViewModelBase
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();

        public SetupDisplayViewModel()
        {
            //Initial creation to mock. Remove once VixenSystem can load and save
            if(!VixenSystem.Props.RootNodes.Any())
            {
                MockPropManager();
            }
            PropNodeTreeViewModel = new PropNodeTreeViewModel();
        }

        private void MockPropManager()
        {
            PropManager propManager = VixenSystem.Props;

			propManager.RootNode.AddChild(MockPropNodeGroup("Mini Tree"));
            
            propManager.RootNode.AddChild(MockPropNodeGroup("Arch"));
        }

        private PropNode MockPropNodeGroup(string name)
        {
            var plural = name.EndsWith('e') ? "s" : "es";
            var propNode = new PropNode($"{name}{plural}");

            var mtl = new PropNode($"{name}{plural} Left");

            for (int i = 0; i < 4; i++)
            {
				var propName = $"{name} {i + 1}";
                var prop = new Arch(propName);

                mtl.AddChild(new PropNode(prop));
			}

            var mtr = new PropNode($"{name}{plural} Right");
            for (int i = 0; i < 4; i++)
            {
                var propName = $"{name} {i + 1}";
                var prop = new Arch(propName);

				mtr.AddChild(new PropNode(prop));
            }

            propNode.AddChild(mtl);
            propNode.AddChild(mtr);

            return propNode;
        }


		#region PropNodeTreeViewModel property

		/// <summary>
		/// Gets or sets the ElementTreeViewModel value.
		/// </summary>
		[Browsable(false)]
        public PropNodeTreeViewModel PropNodeTreeViewModel
		{
            get { return GetValue<PropNodeTreeViewModel>(PropTreeViewModelProperty); }
            set { SetValue(PropTreeViewModelProperty, value); }
        }

        /// <summary>
        /// ElementTreeViewModel property data.
        /// </summary>
        public static readonly IPropertyData PropTreeViewModelProperty = RegisterProperty<PropNodeTreeViewModel>(nameof(PropNodeTreeViewModel));

		#endregion

		#region Menu Commands

        #region OpenProp command

        private Command _openPropCommand;

        /// <summary>
        /// Gets the OpenProp command.
        /// </summary>
        [Browsable(false)]
        public Command OpenPropCommand
        {
            get { return _openPropCommand ??= new Command(OpenProp); }
        }

        /// <summary>
        /// Method to invoke when the OpenProp command is executed.
        /// </summary>
        private async void OpenProp()
        {
            var dependencyResolver = this.GetDependencyResolver();
            var openFileService = dependencyResolver.Resolve<IOpenFileService>();
            var determineFileContext = new DetermineOpenFileContext()
            {
                IsMultiSelect = false,
                Filter = "Prop Files(*.prp)|*.prp",
                FileName = String.Empty
            };

            var result = await openFileService.DetermineFileAsync(determineFileContext);

            if (result.Result)
            {

                string path = result.FileNames.First();
                if (!string.IsNullOrEmpty(path))
                {
                    var pleaseWaitService = dependencyResolver.Resolve<IBusyIndicatorService>();
                    pleaseWaitService.Show();
                   // LoadPropFromPath(path);
                    pleaseWaitService.Hide();
                }
            }
        }

        #endregion

		#region SaveModel command

		private Command _saveModelCommand;

        /// <summary>
        /// Gets the SaveModel command.
        /// </summary>
        [Browsable(false)]
        public Command SaveModelCommand
        {
            get { return _saveModelCommand ??= new Command(SaveModel); }
        }

        /// <summary>
        /// Method to invoke when the SaveModel command is executed.
        /// </summary>
        private void SaveModel()
        {
           
        }

        #endregion

        #region SaveModelAs command

        private Command _saveModelAsCommand;

        /// <summary>
        /// Gets the SaveModelAs command.
        /// </summary
        [Browsable(false)]
        public Command SaveModelAsCommand
        {
            get { return _saveModelAsCommand ??= new Command(SaveModelAs); }
        }

        /// <summary>
        /// Method to invoke when the SaveModelAs command is executed.
        /// </summary>
        private async void SaveModelAs()
        {
           
        }

        #endregion

		#region Exit command

		private Command<Window> _exitCommand;

        /// <summary>
        /// Gets the Exit command.
        /// </summary>
        [Browsable(false)]
        public Command<Window> ExitCommand
        {
            get { return _exitCommand ?? (_exitCommand = new Command<Window>(Exit)); }
        }

        /// <summary>
        /// Method to invoke when the Exit command is executed.
        /// </summary>
        private void Exit(Window window)
        {
            window?.Close();
        }

		#endregion

		#region Closing command

        private Command<CancelEventArgs> _closingCommand;

        /// <summary>
        /// Gets the Closing command.
        /// </summary>
        [Browsable(false)]
        public Command<CancelEventArgs> ClosingCommand
        {
            get { return _closingCommand ?? (_closingCommand = new Command<CancelEventArgs>(Closing)); }
        }

        /// <summary>
        /// Method to invoke when the Closing command is executed.
        /// </summary>
        private void Closing(CancelEventArgs e)
        {
            //if (TestIsDirty())
            //{
            //    var dependencyResolver = this.GetDependencyResolver();
            //    var mbs = dependencyResolver.Resolve<IMessageBoxService>();
            //    var response = mbs.GetUserConfirmation($"Save Prop \"{CleanseNameString(Prop.Name)}\" ", "Save");
            //    if (response.Result == MessageResult.OK)
            //    {
            //        SaveModel();
            //    }
            //    else if (response.Result == MessageResult.Cancel)
            //    {
            //        e.Cancel = true;
            //    }
            //}
        }


        #endregion

		#region NewProp command

		private Command _newPropCommand;

        /// <summary>
        /// Gets the NewProp command.
        /// </summary>
        [Browsable(false)]
        public Command NewPropCommand
        {
            get { return _newPropCommand ??= new Command(NewProp); }
        }

        private const string TokenPattern = @"{[0-9]+}";
        /// <summary>
        /// Method to invoke when the NewProp command is executed.
        /// </summary>
        private void NewProp()
        {
            MessageBoxService mbs = new MessageBoxService();
            var result = mbs.GetUserInput("Please enter the Prop name.", "Create Prop", "New Prop");
            if (result.Result == MessageResult.OK)
            {
                var name = result.Response;
                if (!Regex.IsMatch(name, TokenPattern))
                {
                    name = $"{name} {{1}}";
                }

                //Call Prop creation wizard
                // Prop = PropModelServices.Instance().CreateProp(name);
               
            }
        }

		#endregion

		#region Import command

		private Command<string> _importCommand;

		/// <summary>
		/// Gets the Import command.
		/// </summary>
		[Browsable(false)]
		public Command<string> ImportCommand
		{
			get { return _importCommand ?? (_importCommand = new Command<string>(Import)); }
		}

		/// <summary>
		/// Method to invoke when the Import command is executed.
		/// </summary>
		private async void Import(string type)
		{
			var dependencyResolver = this.GetDependencyResolver();
			var openFileService = dependencyResolver.Resolve<IOpenFileService>();

			var determineFileContext = new DetermineOpenFileContext()
			{
				IsMultiSelect = false,
				Filter = "xModel (*.xmodel)|*.xmodel",
				FileName = String.Empty,
				InitialDirectory = Paths.DataRootPath
			};

			var result = await openFileService.DetermineFileAsync(determineFileContext);

			if (result.Result)
			{
				string path = result.FileName;
				if (!string.IsNullOrEmpty(path))
				{
					var pleaseWaitService = dependencyResolver.Resolve<IBusyIndicatorService>();
					pleaseWaitService.Show();
					await ImportProp(path);
					pleaseWaitService.Hide();
				}
			}
		}

		private async Task<bool> ImportProp(string path)
		{
			try
			{
				//IModelImport import = new XModelImport();
				//var p = await import.ImportAsync(path);
				//if (p != null)
				//{
				//	Prop = p;
				//	FilePath = String.Empty;
				//	//Switch to selection mode VIX-2784
				//	DrawingPanelViewModel.IsDrawing = false;
				//}
			}
			catch (Exception e)
			{

				Logging.Error(e, "An error occuring importing the xModel.");
				var mbs = new MessageBoxService();
				mbs.ShowError($"An error occurred importing the xModel. Please notify the Vixen Team.", "Error Importing xModel");
				return false;
			}

			return true;
		}

		#endregion

		#region ImportVendorXModel command

		private TaskCommand _openVendorBrowserCommand;

		/// <summary>
		/// Gets the ImportVendorXModel command.
		/// </summary>
		[Browsable(false)]
		public TaskCommand OpenVendorBrowserCommand
		{
			get { return _openVendorBrowserCommand ??= new TaskCommand(OpenVendorBrowserAsync); }
		}

		/// <summary>
		/// Method to invoke when the OpenVendorBrowserCommand command is executed.
		/// </summary>
		private async Task OpenVendorBrowserAsync()
		{
			
		}

		#endregion

		#region Help command

		private Command? _helpCommand;

        /// <summary>
        /// Gets the Help command.
        /// </summary>
        [Browsable(false)]
        public Command HelpCommand
        {
            get { return _helpCommand ??= new Command(Help); }
        }

        /// <summary>
        /// Method to invoke when the Help command is executed.
        /// </summary>
        private void Help()
        {
            var url = "http://www.vixenlights.com/vixen-3-documentation/preview/custom-prop-editor/";
            var psi = new ProcessStartInfo()
            {
                FileName = url,
                UseShellExecute = true
            };
            Process.Start(psi);
        }

		#endregion

		#endregion

		#region SelectedTabIndex property

        /// <summary>
        /// Gets or sets the SelectedTabIndex value.
        /// </summary>
        [Browsable(false)]
        public int SelectedTabIndex
        {
            get { return GetValue<int>(SelectedTabIndexProperty); }
            set { SetValue(SelectedTabIndexProperty, value); }
        }

        /// <summary>
        /// SelectedTabIndex property data.
        /// </summary>
        public static readonly IPropertyData SelectedTabIndexProperty =
            RegisterProperty<int>(nameof(SelectedTabIndex), null, (sender, e) => ((SetupDisplayViewModel)sender).OnSelectedTabIndexChanged());

        /// <summary>
        /// Called when the SelectedTabIndex property has changed.
        /// </summary>
        private void OnSelectedTabIndexChanged()
        {
            if (SelectedTabIndex == 0)
            {
                //var selectedModelIds = ElementTreeViewModel.SelectedItems.Select(e => e.ElementModel.Id).Distinct();
                //ElementTreeViewModel.DeselectAll();
                //ElementOrderViewModel.Select(selectedModelIds);
            }
            else if (SelectedTabIndex == 1)
            {
                //var selectedModelIds = ElementOrderViewModel.SelectedItems.Select(e => e.ElementModel.Id).Distinct();
                //ElementOrderViewModel.DeselectAll();
                //ElementTreeViewModel.Select(selectedModelIds);
            }
        }

        #endregion
	}
}