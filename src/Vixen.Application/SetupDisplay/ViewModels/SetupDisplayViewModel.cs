#nullable enable
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;

using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;

using Common.WPFCommon.Command;
using Common.WPFCommon.Services;

using NLog;

using Vixen.Sys;
using Vixen.Sys.Managers;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Model;

using VixenApplication.SetupDisplay.OpenGL;

using VixenModules.App.Props.Models.Arch;
using VixenModules.App.Props.Models.Tree;

using Window = System.Windows.Window;

namespace VixenApplication.SetupDisplay.ViewModels
{	
	/// <summary>
	/// Maintains the Display Setup view model.
	/// </summary>
	public class SetupDisplayViewModel : ViewModelBase
	{
		#region Fields

		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private const int LayoutViewTab = 0;
		private const int PropViewTab = 1;

		#endregion 

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public SetupDisplayViewModel()
		{
			PropPreviewNodePoints = new();

			//Initial creation to mock. Remove once VixenSystem can load and save
			//if (!VixenSystem.Props.RootNodes.Any())
			//         {
			//             MockPropManager();
			//         }
			//         else
			//         {
			//             VixenSystem.Props.RootNodes.Clear();
			//             MockPropManager();
			//}

			PropNodeTreeViewModel = new();
			PropNodeTreeViewModel.PropertyChanged += PropNodeTreeViewModel_PropertyChanged;
			PropNodeTreePropViewModel = new(PropNodeTreeViewModel);
			PropNodeTreePropViewModel.PropertyChanged += PropNodeTreePropViewModel_PropertyChanged;
			IsTwoD = true;
			IsThreeD = true;

			// Initialize the command to center the OpenGL preview
			CenterPreview = new RelayCommand(ExecuteCenterPreview);

			// Create the collection of view model rotations
			Rotations = new();

			// Create the X Axis rotation view model
			AxisRotationViewModel xRotation = new AxisRotationViewModel();
			xRotation.Axis = "X";
			xRotation.RotationChanged += OnRotationChanged;
			Rotations.Add(xRotation);

			// Create the Y Axis rotation view model
			AxisRotationViewModel yRotation = new AxisRotationViewModel();
			yRotation.Axis = "Y";
			yRotation.RotationChanged += OnRotationChanged;
			Rotations.Add(yRotation);

			// Create the Z Axis rotation view model
			AxisRotationViewModel zRotation = new AxisRotationViewModel();
			zRotation.Axis = "Z";
			zRotation.RotationChanged += OnRotationChanged;						
			Rotations.Add(zRotation);
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Command to centers the preview.
		/// </summary>
		public ICommand CenterPreview { get; private set; }

		/// <summary>
		/// Gets or sets the SelectedProp value.
		/// </summary>
		public int ZRotation
		{
			get { return GetValue<int>(ZRotationProperty); }
			set { SetValue(ZRotationProperty, value); }
		}

		/// <summary>
		/// SelectedProp property data.
		/// </summary>
		public static readonly IPropertyData ZRotationProperty = RegisterProperty<int>(nameof(ZRotation));

		/// <summary>
		/// Collection of rotations to support rotating the props around the x,y, and z axis.
		/// </summary>
		public ObservableCollection<AxisRotationViewModel> Rotations
		{
			get
			{
				return GetValue<ObservableCollection<AxisRotationViewModel>>(RotationsProperty);
			}
			set
			{
				SetValue(RotationsProperty, value);
			}
		}

		public static readonly IPropertyData RotationsProperty = RegisterProperty<ObservableCollection<AxisRotationViewModel>>(nameof(Rotations));

		/// <summary>
		/// Gets or sets the SelectedProp value.
		/// </summary>
		public bool IsTwoD
		{
			get { return GetValue<bool>(TwoDPropProperty); }
			set { SetValue(TwoDPropProperty, value); }
		}

		/// <summary>
		/// SelectedProp property data.
		/// </summary>
		public static readonly IPropertyData TwoDPropProperty = RegisterProperty<bool>(nameof(IsTwoD));

		/// <summary>
		/// Gets or sets the SelectedProp value.
		/// </summary>
		public bool IsThreeD
		{
			get { return GetValue<bool>(ThreeDPropProperty); }
			set { SetValue(ThreeDPropProperty, value); }
		}

		/// <summary>
		/// SelectedProp property data.
		/// </summary>
		public static readonly IPropertyData ThreeDPropProperty = RegisterProperty<bool>(nameof(IsThreeD));

		#endregion

		#region Private Methods

		/// <summary>
		/// Center preview command handler.
		/// </summary>
		private void ExecuteCenterPreview()
		{
			// Center the preview and return the new width and height
			/*ClientSize =*/ DrawingEngine.ExecuteCenterPreview();
		}
	
		private void PropNodeTreePropViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (SelectedTabIndex == PropViewTab && nameof(PropNodeTreePropViewModel.SelectedItem).Equals(e.PropertyName))
			{
				if (PropNodeTreePropViewModel.SelectedItem is { PropNode.IsProp: true, PropNode.Prop: not null })
				{
					SelectedProp = PropNodeTreePropViewModel.SelectedItem.PropNode.Prop;
					UpdatePreviewModel(SelectedProp);
					UpdatePropComponentTreeViewModel(SelectedProp);
				}
				else
				{
					SelectedProp = null;
					ClearPreviewModel();
					ClearPropComponentTreeViewModel();
					UpdatePreviewModel(SelectedProp);
				}
			}
		}

		private void PropNodeTreeViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
			if (SelectedTabIndex == LayoutViewTab && nameof(PropNodeTreeViewModel.SelectedItem).Equals(e.PropertyName))
			{
				if (PropNodeTreeViewModel.SelectedItem is { PropNode.IsProp: true, PropNode.Prop: not null })
				{
					SelectedProp = PropNodeTreeViewModel.SelectedItem.PropNode.Prop;
					UpdatePreviewModel(SelectedProp);
					UpdatePropComponentTreeViewModel(SelectedProp);
				}
				else
				{
					SelectedProp = null;
					ClearPreviewModel();
					ClearPropComponentTreeViewModel();
					UpdatePreviewModel(SelectedProp);
				}
			}
		}

		/// <summary>
		/// Draws the specified prop model in an OpenTK viewport.
		/// </summary>
		/// <param name="propModel">Prop model to draw</param>
		private void DrawProp(IPropModel? propModel)
		{
			// Wrap the prop in a collection
			List<IPropModel> propModels = new List<IPropModel>();

			// If a prop model was passed in then...
			if (propModel != null)
			{
				// Add the prop model to the collection
				propModels.Add(propModel);
			}

			int width = 0;
			int height = 0;

			// If the drawing engine already exists then...
			if (DrawingEngine != null)
			{
				// Save off the width and height of the viewport
				width = DrawingEngine.OpenTkControl_Width;
				height = DrawingEngine.OpenTkControl_Height;

				// Dispose of the preview drawing engine
				DrawingEngine.Dispose();
				DrawingEngine = null;
			}

			// Create a new drawing engine passing it the prop to display
			DrawingEngine = new(propModels);
			DrawingEngine.OpenTkControl_Width = width;
			DrawingEngine.OpenTkControl_Height = height;

			// Initialize the drawing engine with the camera on the axis (0,0,0)
			DrawingEngine.Initialize(
				0.0f,
				0.0f,
				0.0f);
		}

		#endregion

		#region Mock Code

		//TODO remove this when the real tree can be used.
		private void MockPropManager()
		{
			PropManager propManager = VixenSystem.Props;

			propManager.RootNode.AddChild(MockPropNodeGroup<Tree>("Mini Tree"));

			propManager.RootNode.AddChild(MockPropNodeGroup<Arch>("Arch"));

			propManager.RootNode.AddChild(MockPropNodeGroup<IntelligentFixtureProp>("Intelligent Fixture"));
		}

		private PropNode MockPropNodeGroup<T>(string name) where T : IProp, new()
		{
			var plural = name.EndsWith('e') ? "s" : "es";
			var propNode = new PropNode($"{name}{plural}");

			var mtl = new PropNode($"{name}{plural} Left");

			for (int i = 0; i < 4; i++)
			{
				var propName = $"{name} {i + 1}";

				var prop = VixenSystem.Props.CreateProp<T>(propName);

				mtl.AddChild(new PropNode(prop));
			}

			var mtr = new PropNode($"{name}{plural} Right");
			for (int i = 0; i < 4; i++)
			{
				var propName = $"{name} {i + 1}";
				var prop = VixenSystem.Props.CreateProp<T>(propName);

				mtr.AddChild(new PropNode(prop));
			}

			propNode.AddChild(mtl);
			propNode.AddChild(mtr);

			return propNode;
		}

		#endregion

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

		#region PropNodeTreePropViewModel property

		/// <summary>
		/// Gets or sets the PropNodeTreePropViewModel value.
		/// </summary>;
		public PropNodeTreePropViewModel PropNodeTreePropViewModel
		{
			get { return GetValue<PropNodeTreePropViewModel>(PropNodePropTreeViewModelProperty); }
			private init { SetValue(PropNodePropTreeViewModelProperty, value); }
		}

		/// <summary>;
		/// PropNodeTreePropViewModel property data.
		/// </summary>;
		public static readonly IPropertyData PropNodePropTreeViewModelProperty = RegisterProperty<PropNodeTreePropViewModel>(nameof(PropNodeTreePropViewModel));

		#endregion

		#region SelectedProp property

		/// <summary>
		/// Gets or sets the SelectedProp value.
		/// </summary>
		public IProp? SelectedProp
		{
			get { return GetValue<IProp>(SelectedPropProperty); }
			set 
			{
				IsThreeD = (value is IntelligentFixtureProp);
				IsTwoD = !IsThreeD;

				// If the prop is a light based prop then...
				if (value?.PropModel is ILightPropModel lightPropModel)
				{
					// Transfer the rotations from the model to the view model
					int index = 0;
					foreach (AxisRotationModel rotationModel in lightPropModel.Rotations)
					{
						Rotations[index].Axis = GetAxis(rotationModel.Axis);
						Rotations[index].RotationAngle = rotationModel.RotationAngle;	
						index++;
					}
				}
				SetValue(SelectedPropProperty, value);
			}
		}

		/// <summary>
		/// Converts from axis string to enumeration.
		/// </summary>
		/// <param name="axis">String to convert</param>
		/// <returns>Equivalent enumeration of the string</returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private Axis GetAxis(string axis)
		{			
			return axis switch
			{
				"X" => Axis.XAxis,
				"Y" => Axis.YAxis,
				"Z" => Axis.ZAxis,
				_ => throw new ArgumentOutOfRangeException(nameof(axis), "Unsupported rotation axis")
			};
		}

		/// <summary>
		/// Converts the enumeration into a display string.
		/// </summary>
		/// <param name="axis">Enumeration to convert</param>
		/// <returns></returns>
		/// <exception cref="ArgumentOutOfRangeException"></exception>
		private string GetAxis(Axis axis)
		{
			return axis switch
			{
				Axis.XAxis => "X",
				Axis.YAxis => "Y",	
				Axis.ZAxis => "Z",	
				_ => throw new ArgumentOutOfRangeException(nameof(axis), "Unsupported rotation axis")
			};
		}

		/// <summary>
		/// Event handler for when a prop rotation changed.
		/// </summary>
		/// <param name="sender">Event sender</param>
		/// <param name="e">Event arguments</param>
		private void OnRotationChanged(object sender, EventArgs e)
		{
			// If the selected prop is a light based prop then...
			if (SelectedProp?.PropModel is ILightPropModel lightPropModel)
			{
				// Transfer the rotations from the view model to the model
				int index = 0;
				foreach (AxisRotationViewModel rotationViewModel in Rotations)
				{
					AxisRotationModel rotationMdl = lightPropModel.Rotations[index];
					rotationMdl.Axis = GetAxis(rotationViewModel.Axis);
					rotationMdl.RotationAngle = rotationViewModel.RotationAngle;
					index++;
				}

				// Update the prop nodes
				lightPropModel.UpdatePropNodes();
			}
		}

		/// <summary>
		/// SelectedProp property data.
		/// </summary>
		public static readonly IPropertyData SelectedPropProperty = RegisterProperty<IProp>(nameof(SelectedProp));

		#endregion

		#region PropComponentTreeViewModel property

		/// <summary>
		/// Gets or sets the PropComponentTreeViewModel value.
		/// </summary>
		public PropComponentTreeViewModel PropComponentTreeViewModel
		{
			get { return GetValue<PropComponentTreeViewModel>(PropComponentTreeViewModelProperty); }
			set { SetValue(PropComponentTreeViewModelProperty, value); }
		}

		/// <summary>
		/// PropComponentTreeViewModel property data.
		/// </summary>
		public static readonly IPropertyData PropComponentTreeViewModelProperty = RegisterProperty<PropComponentTreeViewModel>(nameof(PropComponentTreeViewModel));

		#endregion

		#region PropPreviewNodePoints property

		/// <summary>
		/// Gets or sets the PropPreviewNodePoints value.
		/// </summary>
		public ObservableCollection<NodePoint>? PropPreviewNodePoints
		{
			get { return GetValue<ObservableCollection<NodePoint>>(SelectedItemNodesProperty); }
			private set { SetValue(SelectedItemNodesProperty, value); }
		}

		/// <summary>
		/// PropPreviewNodePoints property data.
		/// </summary>
		public static readonly IPropertyData SelectedItemNodesProperty = RegisterProperty<ObservableCollection<NodePoint>>(nameof(PropPreviewNodePoints));

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

		}

		#endregion

		#region PropPreview Methods

		internal void ClearPreviewModel()
		{
			PropPreviewNodePoints = null;

		}

		internal void UpdatePreviewModel(IProp prop)
		{			
			if (prop?.PropModel is ILightPropModel lightPropModel)
			{
				if (PropPreviewNodePoints != lightPropModel.Nodes)
				{
					PropPreviewNodePoints = lightPropModel.Nodes;
					
					// Draw the prop in OpenGL
					DrawProp(lightPropModel);
				}
			}
			else if (prop?.PropModel is IPropModel propModel)
			{				
				// Draw the prop in OpenGL
				DrawProp(propModel);				
			}			
			else
			{				
				DrawProp(null);
			}
		}

		public OpenGLPropDrawingEngine DrawingEngine { get; set; } = new OpenGLPropDrawingEngine(new List<IPropModel>());

		#endregion

		#region PropComponentView

		private void UpdatePropComponentTreeViewModel(IProp selectedProp)
		{
			PropComponentTreeViewModel = new PropComponentTreeViewModel(selectedProp);
		}

		internal void ClearPropComponentTreeViewModel()
		{
			PropComponentTreeViewModel = null;

		}

		#endregion]		
	}
}