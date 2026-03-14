#nullable enable
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
using VixenApplication.SetupDisplay.OpenGL.Shapes;

using VixenModules.App.Props.Models.Arch;
using VixenModules.App.Props.Models.Tree;
using VixenModules.Preview.VixenPreview.OpenGL;

using Window = System.Windows.Window;

namespace VixenApplication.SetupDisplay.ViewModels
{
	/// <summary>
	/// Maintains the Display Setup view model.
	/// </summary>
	public class SetupDisplayViewModel : ViewModelBase
	{				
		#region Constants

		private const int LayoutViewTab = 0;
		private const int PropViewTab = 1;

		#endregion

		#region Fields

		/// <summary>
		/// Prop model to display in the prop preview on the next render cycle.
		/// </summary>
		private IProp? _nextPreviewProp;

		/// <summary>
		/// Prop model currently being displayed in the prop preview.
		/// </summary>
		private IProp? _currentPreviewProp;

		private static Logger Logging = LogManager.GetCurrentClassLogger();

		#endregion

		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		public SetupDisplayViewModel()
		{
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

			// Initialize the command to add a prop to the preview
			AddPropToPreview = new RelayCommand(AddSelectedPropToPreview, CanAddPropToPreview);

			// Initialize the command to center the OpenGL prop preview
			PropPreviewCenter = new RelayCommand(ExecuteCenterPropPreview);

			// Initialize the command to center the OpenGL display preview
			DisplayPreviewCenter = new RelayCommand(ExecuteCenterDisplayPreview);

			// Initialize the select background image command
			SelectBackgroundImage = new RelayCommand(SelectBackground);

			// Register for the Selected Props collection changed event
			DisplayPreviewDrawingEngine.SelectedProps.CollectionChanged += SelectedProps_CollectionChanged;

			// Initialize the commands to lock and unlock props
			Lock = new RelayCommand(LockSelectedProps, CanLockSelectedProps);
			Unlock = new RelayCommand(UnlockSelectedProps, CanUnlockSelectedProps);
			UnlockAll = new RelayCommand(UnlockAllProps, CanUnlockAllProps);

			// Intialize the command to delete selected preview props
			DeletePreviewProp = new RelayCommand(DeleteSelectedPreviewProps, CanDeleteSelectedPreviewProps);

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

			// Default the background to full intensity
			BackgroundBrightness = 1.0f;

			// Default the background brightness slider to disabled
			BackgroundBrightnessEnabled = false;
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// Command to delete a prop from the setup preview.
		/// </summary>
		public ICommand DeletePreviewProp { get; private set; }

		/// <summary>
		/// Command to add a prop to the preview;
		/// </summary>
		public ICommand AddPropToPreview { get; private set; }

		/// <summary>
		/// Command that centers the prop preview.
		/// </summary>
		public ICommand PropPreviewCenter { get; private set; }

		/// <summary>
		/// Command that centers the display preview.
		/// </summary>
		public ICommand DisplayPreviewCenter { get; private set; }

		/// <summary>
		/// Command for selecting the background image for the preview.
		/// </summary>
		public ICommand SelectBackgroundImage { get; private set; }

		/// <summary>
		/// Command for locking the selected props.
		/// </summary>
		public ICommand Lock { get; private set; }

		/// <summary>
		/// Command for unlocking the selected props.
		/// </summary>
		public ICommand Unlock { get; private set; }

		/// <summary>
		/// Command for unlocking all props.
		/// </summary>
		public ICommand UnlockAll { get; private set; }

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

		#endregion

		#region Catel Public Properties

		/// <summary>
		/// True when the BackgroundBrigtness slider is enabled.
		/// </summary>
		public bool BackgroundBrightnessEnabled
		{
			get { return GetValue<bool>(BackgroundEnabledProperty); }
			set { SetValue(BackgroundEnabledProperty, value); }			
		}
				
		/// <summary>
		/// Background Brightness Enabled property data.
		/// </summary>
		public static readonly IPropertyData BackgroundEnabledProperty = RegisterProperty<bool>(nameof(BackgroundBrightnessEnabled));

		/// <summary>
		/// Background Brightness value (0.0 - 1.0)
		/// </summary>
		/// <remarks>1.0 is full intensity</remarks>
		public float BackgroundBrightness
		{
			get { return GetValue<float>(BackgroundBrightnessProperty); }
			
			set 
			{ 
				SetValue(BackgroundBrightnessProperty, value);

				// If the background has been created on the drawing engine then...
				if (DisplayPreviewDrawingEngine.Background != null)
				{
					// Give the background intensity to the background
					DisplayPreviewDrawingEngine.Background.BackgroundBrightness = value;
				}
			}
		}

		/// <summary>
		/// Background Brightness property data.
		/// </summary>
		public static readonly IPropertyData BackgroundBrightnessProperty = RegisterProperty<float>(nameof(BackgroundBrightness));

		#endregion

		#region Private Methods

		/// <summary>
		/// Event handler when the selected props in the setup preview change.
		/// </summary>
		/// <param name="sender">The source of the event</param>
		/// <param name="e">Event arguments</param>
		private void SelectedProps_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			// Update the enabled/disabled status of the lock and unlock commands
			UpdateLockUnlockCommands();

			// Update the enabled/disabled status of the delete preview prop command
			((RelayCommand)DeletePreviewProp).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Updates the enabled/disabled status of the lock and unlock commands.
		/// </summary>
		private void UpdateLockUnlockCommands()
		{
			// Refresh the CanExecute status of the lock and unlock commands
			((RelayCommand)Lock).RaiseCanExecuteChanged();
			((RelayCommand)Unlock).RaiseCanExecuteChanged();
			((RelayCommand)UnlockAll).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Returns true if there are selected props to lock.
		/// </summary>
		/// <returns>True if there are selected props to lock</returns>
		private bool CanLockSelectedProps()
		{
			// Default to there not being any props to lock
			bool propsToLock = false;

			// Loop over the selected props
			foreach (IPropOpenGLData prop in DisplayPreviewDrawingEngine.SelectedProps)
			{
				// If the prop is unlocked then...
				if (!prop.Locked)
				{
					// Indicate the lock command should be enabled
					propsToLock = true;

					break;
				}
			}

			return propsToLock;
		}
		
		/// <summary>
		/// Locks the selected props.
		/// </summary>
		private void LockSelectedProps()
		{		
			// Loop over the selected props
			foreach(IPropOpenGLData prop in DisplayPreviewDrawingEngine.SelectedProps)
			{
				// Lock the prop
				prop.Locked = true;
			}

			// Refresh the lock / unlock commands
			UpdateLockUnlockCommands();
		}

		/// <summary>
		/// Returns true if there are selected props to unlock.
		/// </summary>
		/// <returns>True if there are selected props to unlock</returns>
		private bool CanUnlockSelectedProps()
		{
			// Default to there not being any props to unlock
			bool propsToUnlock = false;

			// Loop over the selected props
			foreach (IPropOpenGLData prop in DisplayPreviewDrawingEngine.SelectedProps)
			{
				// If the prop is locked then...
				if (prop.Locked)
				{
					// Indicate the unlock command should be enabled
					propsToUnlock = true;
				}
			}

			return propsToUnlock;
		}

		/// <summary>
		/// Locks the selected props.
		/// </summary>
		private void UnlockSelectedProps()
		{
			// Loop over the selected props
			foreach (IPropOpenGLData prop in DisplayPreviewDrawingEngine.SelectedProps)
			{
				// Lock the prop
				prop.Locked = false;

				// Break out of the loop
				break;
			}

			// Refresh the lock / unlock commands
			UpdateLockUnlockCommands();
		}

		/// <summary>
		/// Returns true if there are any props to unlock.
		/// </summary>
		/// <returns>True if there are any props to unlock</returns>
		private bool CanUnlockAllProps()
		{
			// Default to there not being any props to unlock
			bool propsToUnlock = false;

			// Loop over all props
			foreach (IPropOpenGLData prop in DisplayPreviewDrawingEngine.Props)
			{
				// If the prop is locked then...
				if (prop.Locked)
				{
					// Indicate the unlock all command should be enabled
					propsToUnlock = true;

					// Break out of the loop
					break;
				}
			}

			return propsToUnlock;
		}

		/// <summary>
		/// Unlocks all locked props.
		/// </summary>
		private void UnlockAllProps()
		{
			// Loop over all props
			foreach (IPropOpenGLData prop in DisplayPreviewDrawingEngine.Props)
			{
				// Set the prop to unlocked
				prop.Locked = false;
			}

			// Refresh the lock / unlock commands
			UpdateLockUnlockCommands();
		}

		/// <summary>
		/// Returns true when there are selected preview props to delete.
		/// </summary>
		/// <returns>True when there are selected preview props to delete</returns>
		private bool CanDeleteSelectedPreviewProps()
		{
			// Return true if there are any selected props
			return DisplayPreviewDrawingEngine.SelectedProps.Count > 0;
		}

		/// <summary>
		/// Deletes the selected props from the preview setup.
		/// </summary>
		private void DeleteSelectedPreviewProps()
		{
			// Remove the selected props from the preview setup
			DisplayPreviewDrawingEngine.RemoveProps(DisplayPreviewDrawingEngine.SelectedProps.ToList());

			// Clear the selected props collection
			DisplayPreviewDrawingEngine.SelectedProps.Clear();	
		}

		/// <summary>
		/// Selects the background image.
		/// </summary>
		private void SelectBackground()
		{
			// Create the Microsoft Open File Dialog
			OpenFileDialog dialog = new OpenFileDialog { Title = "Select a Background", Filter = "JPG Files (*.jpg)|*.jpg" };

			// Display the File Selection Dialog
			DialogResult? result = dialog.ShowDialog();

			// If the user selected a file then...
			if (result == DialogResult.OK)
			{
				// If the background class exists then...
				if (DisplayPreviewDrawingEngine.Background != null)
				{
					// Dispose of the previous background
					DisplayPreviewDrawingEngine.Background.Dispose();
				}

				// Assign the background image to the drawing engine
				DisplayPreviewDrawingEngine.Background = new PropPreviewBackground(dialog.FileName, 1.0f);

				// Since a background image has been selected enable the brightness slider
				BackgroundBrightnessEnabled = true;
			}
		}

		/// <summary>
		/// Adds the currently selected prop to the preview.
		/// </summary>
		private void AddSelectedPropToPreview()
		{
			if (SelectedProp is null ||
				SelectedProp.PropModel is null)
			{
				throw new InvalidOperationException("No Prop Selected to Add to Preview");
			}

			// Add the selected prop to the preview
			DisplayPreviewDrawingEngine.AddProps(new List<IPropModel>{ SelectedProp.PropModel });

			// Force the CanExecute delegate to run
			((RelayCommand)AddPropToPreview).RaiseCanExecuteChanged();
		}

		/// <summary>
		/// Center prop preview command handler.
		/// </summary>
		private void ExecuteCenterPropPreview()
		{
			if (PropPreviewDrawingEngine is null)
			{
				throw new Exception(nameof(PropPreviewDrawingEngine) + " is null!");
			}
			// Center the prop preview and return the new width and height
			/*ClientSize =*/ PropPreviewDrawingEngine.ExecuteCenterPreview();
		}

		/// <summary>
		/// Center display preview command handler.
		/// </summary>
		private void ExecuteCenterDisplayPreview()
		{
			// Center the display preview and return the new width and height
			/*ClientSize =*/DisplayPreviewDrawingEngine.ExecuteCenterPreview();
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
			if (PropPreviewDrawingEngine != null)
			{
				// Save off the width and height of the viewport
				width = PropPreviewDrawingEngine.OpenTkControl_Width;
				height = PropPreviewDrawingEngine.OpenTkControl_Height;

				// Dispose of the preview drawing engine
				PropPreviewDrawingEngine.Dispose();
				PropPreviewDrawingEngine = null;
			}

			// Create a new drawing engine passing it the prop to display
			PropPreviewDrawingEngine = new(propModels, height, 1.0f);
			PropPreviewDrawingEngine.OpenTkControl_Width = width;
			PropPreviewDrawingEngine.OpenTkControl_Height = height;

			// Initialize the drawing engine with the camera on the axis (0,0,0)
			PropPreviewDrawingEngine.Initialize(
				0.0f,
				0.0f,
				0.0f);
		}

		/// <summary>
		/// Returns true if the select prop can be added to the preview.
		/// </summary>
		/// <returns>True if the select prop can be added to the preview.</returns>
		private bool CanAddPropToPreview()
		{
			// If the Preview has been initialized and
			// there is a selected prop and
			// the preview does NOT already contain the select prop
			return DisplayPreviewDrawingEngine.Initialized &&
				SelectedProp?.PropModel != null &&
				!DisplayPreviewDrawingEngine.Props.Any(itm => itm.PropModelId == SelectedProp?.PropModel.Id);
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

				// Force the CanExecute delegate to run
				((RelayCommand)AddPropToPreview).RaiseCanExecuteChanged();				
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
		private void OnRotationChanged(object? sender, EventArgs e)
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
		public PropComponentTreeViewModel? PropComponentTreeViewModel
		{
			get { return GetValue<PropComponentTreeViewModel>(PropComponentTreeViewModelProperty); }
			set { SetValue(PropComponentTreeViewModelProperty, value); }
		}

		/// <summary>
		/// PropComponentTreeViewModel property data.
		/// </summary>
		public static readonly IPropertyData PropComponentTreeViewModelProperty = RegisterProperty<PropComponentTreeViewModel>(nameof(PropComponentTreeViewModel));

		#endregion
			
		#region Menu Commands

		#region OpenProp command

		private Command? _openPropCommand;

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

			ArgumentNullException.ThrowIfNull(openFileService);

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

					ArgumentNullException.ThrowIfNull(pleaseWaitService);
					
					pleaseWaitService.Show();
					// LoadPropFromPath(path);
					pleaseWaitService.Hide();
				}
			}
		}

		#endregion

		#region SaveModel command

		private Command? _saveModelCommand;

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

		private Command? _saveModelAsCommand;

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
			// TODO
			await Task.CompletedTask;
		}

		#endregion

		#region Exit command

		private Command<Window>? _exitCommand;

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
		private void Exit(Window? window)
		{
			window?.Close();
		}

		#endregion

		#region Closing command

		private Command<CancelEventArgs>? _closingCommand;

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
		private void Closing(CancelEventArgs? e)
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

		private Command? _newPropCommand;

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

		private Command<string>? _importCommand;

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
		private async void Import(string? type)
		{
			var dependencyResolver = this.GetDependencyResolver();
			var openFileService = dependencyResolver.Resolve<IOpenFileService>();

			ArgumentNullException.ThrowIfNull(openFileService);
			
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
				string? path = result.FileName;
				if (!string.IsNullOrEmpty(path))
				{
					var pleaseWaitService = dependencyResolver.Resolve<IBusyIndicatorService>();

					ArgumentNullException.ThrowIfNull(pleaseWaitService);

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

				// TODO
				await Task.CompletedTask;
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

		private TaskCommand? _openVendorBrowserCommand;

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
			// TODO
			await Task.CompletedTask;
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
			RegisterProperty<int>(nameof(SelectedTabIndex), null, OnSelectedTabIndexChanged);

		private static void OnSelectedTabIndexChanged(object? sender, EventArgs e)
		{
			ArgumentNullException.ThrowIfNull(sender);
			
			((SetupDisplayViewModel)sender).OnSelectedTabIndexChanged();
		}

		/// <summary>
		/// Called when the SelectedTabIndex property has changed.
		/// </summary>
		private void OnSelectedTabIndexChanged()
		{

		}

		#endregion

		#region PropPreview Methods
		
		/// <summary>
		/// Updates the prop displayed in the prop preview.
		/// </summary>
		/// <param name="prop">Prop to display in the prop preview</param>
		internal void UpdatePreviewModel(IProp? prop)
		{
			// Save off the prop model to display in the prop preview
			_nextPreviewProp = prop;			
		}

		/// <summary>
		/// Draws the currently selected prop in the prop preview.
		/// </summary>
		/// <remarks>
		/// This method needs to be called within the OpenTK control render method.
		/// Otherwise the two OpenTK controls get confused and the prop preview won't render properly or consistently.
		/// </remarks>
		public void DrawProp()
		{
			// If the prop preview model has changed then...
			if (_nextPreviewProp != _currentPreviewProp)
			{
				// Draw the prop in the OpenGL prop preview
				DrawProp(_nextPreviewProp?.PropModel);

				// Save off the current prop preview prop model
				_currentPreviewProp = _nextPreviewProp;	
			}
		}

		/// <summary>
		/// Refreshes CanExecute commands.
		/// </summary>
		public void RefreshCanExecuteCommands()
		{
			// Force the CanExecute delegate to run
			((RelayCommand)AddPropToPreview).RaiseCanExecuteChanged();
		}

		#region Public Properties

		/// <summary>
		/// Drawing engine for the prop preview.
		/// </summary>
		public OpenGLPropDrawingEngine? PropPreviewDrawingEngine { get; set; } = new OpenGLPropDrawingEngine(new List<IPropModel>(), 1, 100.0f);

		/// <summary>
		/// Drawing engine for the display preview.
		/// </summary>
		public OpenGLSetupPreviewDrawingEngine DisplayPreviewDrawingEngine { get; set; } = new OpenGLSetupPreviewDrawingEngine(new List<IPropModel>());

		#endregion

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