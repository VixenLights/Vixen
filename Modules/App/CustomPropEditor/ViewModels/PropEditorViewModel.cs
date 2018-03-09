using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Windows;
using Catel.Collections;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Common.WPFCommon.Command;
using Vixen.Sys;
using VixenModules.App.CustomPropEditor.Import;
using VixenModules.App.CustomPropEditor.Import.XLights;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	public class PropEditorViewModel : ViewModelBase
	{
		private bool _selectionChanging;
		public PropEditorViewModel()
		{
			ImportCommand = new RelayCommand<string>(ImportModel);
			NewPropCommand = new RelayCommand(NewProp);
			AddLightCommand = new RelayCommand<Point>(AddLightAt);
			LoadImageCommand = new RelayCommand(LoadImage);
			FilePath = String.Empty;
			Prop = PropModelServices.Instance().CreateProp();
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
			    ElementModelLookUpService.Instance.Reset();
			    UnregisterModelEvents();
                SetValue(PropProperty, value);
				ElementTreeViewModel = new ElementTreeViewModel(value);
				DrawingPanelViewModel = new DrawingPanelViewModel(ElementTreeViewModel);
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

		#region FilePath property

		/// <summary>
		/// Gets or sets the FilePath value.
		/// </summary>
		public string FilePath
		{
			get { return GetValue<string>(FilePathProperty); }
			set { SetValue(FilePathProperty, value); }
		}

		/// <summary>
		/// FilePath property data.
		/// </summary>
		public static readonly PropertyData FilePathProperty = RegisterProperty("FilePath", typeof(string));

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

		private void DrawingViewModel_SelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!_selectionChanging)
			{
				_selectionChanging = true;

				//Console.Out.WriteLine($"Drawing View Model changed {e.Action}");

				if (e.Action == NotifyCollectionChangedAction.Reset)
				{
					ElementTreeViewModel.DeselectAll();
				}

				if (e.Action == NotifyCollectionChangedAction.Remove)
				{
					if (e.OldItems != null)
					{
						var parents = e.OldItems.Cast<LightViewModel>().SelectMany(l => ElementModelLookUpService.Instance.GetModels(l.Light.ParentModelId));
						ElementTreeViewModel.DeselectModels(parents);
					}
				}

				if (e.Action == NotifyCollectionChangedAction.Add)
				{
					var parents = e.NewItems.Cast<LightViewModel>().SelectMany(l => ElementModelLookUpService.Instance.GetModels(l.Light.ParentModelId));
					ElementTreeViewModel.SelectModels(parents);
				}
				_selectionChanging = false;
			}

		}

		private void ElementViewModel_SelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (!_selectionChanging)
			{
				_selectionChanging = true;
				//Console.Out.WriteLine($"Element View Model changed {e.Action}");

				if (e.Action == NotifyCollectionChangedAction.Reset)
				{
					DrawingPanelViewModel.DeselectAll();
				}

				if (e.Action == NotifyCollectionChangedAction.Remove)
				{
					var lvm = e.OldItems.Cast<ElementModelViewModel>().SelectMany(x => x.GetLeafEnumerator());
					DrawingPanelViewModel.Deselect(lvm);
				}

				if (e.Action == NotifyCollectionChangedAction.Add)
				{
					var models = e.NewItems.Cast<ElementModelViewModel>().SelectMany(x => x.GetLeafEnumerator());
					DrawingPanelViewModel.Select(models);
				}

				_selectionChanging = false;
			}
		}



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
					FilePath = String.Empty;
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
			//PropModelServices.Instance().RemoveElementModels(ElementTreeViewModel.SelectedItems.Select(x => x.ElementModel));
		    var itemsToDelete = ElementTreeViewModel.SelectedItems.ToList();
            ElementTreeViewModel.SelectedItems.Clear();
			itemsToDelete.ForEach(x => x.RemoveFromParent());
			DrawingPanelViewModel.DeselectAll();
			DrawingPanelViewModel.RefreshLightViewModels();
		}

		/// <summary>
		/// Method to check whether the Delete command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanDelete()
		{
			return !ElementTreeViewModel.SelectedItems.Any(x => x.Equals(Prop.RootNode));
		}

		#endregion

		#region OpenProp command

		private Command _openPropCommand;

		/// <summary>
		/// Gets the OpenProp command.
		/// </summary>
		public Command OpenPropCommand
		{
			get { return _openPropCommand ?? (_openPropCommand = new Command(OpenProp)); }
		}

		/// <summary>
		/// Method to invoke when the OpenProp command is executed.
		/// </summary>
		private async void OpenProp()
		{
			var dependencyResolver = this.GetDependencyResolver();
			var openFileService = dependencyResolver.Resolve<IOpenFileService>();
			openFileService.IsMultiSelect = false;
			openFileService.InitialDirectory = PropModelServices.Instance().ModelsFolder;
			openFileService.Filter = "Prop Files(*.prp)|*.prp";
			if (await openFileService.DetermineFileAsync())
			{
				string path = openFileService.FileNames.First();
				if (!string.IsNullOrEmpty(path))
				{
					Prop p = PropModelServices.Instance().LoadProp(path);
					if (p != null)
					{
						Prop = p;
						FilePath = path;
					}
					else
					{
						//Alert user
					}
				}
			}
		}

		#endregion

		#region SaveModel command

		private Command _saveModelCommand;

		/// <summary>
		/// Gets the SaveModel command.
		/// </summary>
		public Command SaveModelCommand
		{
			get { return _saveModelCommand ?? (_saveModelCommand = new Command(SaveModel)); }
		}

		/// <summary>
		/// Method to invoke when the SaveModel command is executed.
		/// </summary>
		private async void SaveModel()
		{
			PropModelServices.Instance().EnsureModelDirectory();
			if (string.IsNullOrEmpty(FilePath))
			{
				SaveModelAs();
			}
			else
			{
				PropModelPersistenceService.UpdateModel(Prop, FilePath);
			}
		}

		#endregion

		#region SaveModelAs command

		private Command _saveModelAsCommand;

		/// <summary>
		/// Gets the SaveModelAs command.
		/// </summary>
		public Command SaveModelAsCommand
		{
			get { return _saveModelAsCommand ?? (_saveModelAsCommand = new Command(SaveModelAs)); }
		}

		/// <summary>
		/// Method to invoke when the SaveModelAs command is executed.
		/// </summary>
		private async void SaveModelAs()
		{
			PropModelServices.Instance().EnsureModelDirectory();
			var dependencyResolver = this.GetDependencyResolver();
			var saveFileService = dependencyResolver.Resolve<ISaveFileService>();
			saveFileService.Filter = "Prop Files(*.prp)|*.prp";
			saveFileService.CheckPathExists = true;
			saveFileService.InitialDirectory = PropModelServices.Instance().ModelsFolder;
			saveFileService.FileName = Prop.Name;
			if (await saveFileService.DetermineFileAsync())
			{
				// User selected a file
				if (PropModelPersistenceService.SaveModel(Prop, saveFileService.FileName))
				{
					FilePath = saveFileService.FileName;
				}
			}
		}

		#endregion



		private void NewProp()
		{
			MessageBoxService mbs = new MessageBoxService();
			var result = mbs.GetUserInput("Please enter the model name.", "Create Model", "New Prop");
			if (result.Result == MessageResult.OK)
			{
                Prop = PropModelServices.Instance().CreateProp(result.Response);
				FilePath = string.Empty;
			}
		}

		public void AddLightAt(Point p)
		{
			var target = ElementTreeViewModel.SelectedItem;

			var model = PropModelServices.Instance().AddLight(target?.ElementModel, p);

			if (model == null) return;

			DrawingPanelViewModel.RefreshLightViewModels();

			if (model == target?.ElementModel)
			{
				ElementTreeViewModel.SelectModels(new []{target});
			}
			else 
			{
				var vms = ElementModelLookUpService.Instance.GetModels(model.Id);
				var viewModel = vms.First();
				var parent = viewModel.ParentViewModel as ElementModelViewModel;
				if (parent != null)
				{
					ElementTreeViewModel.SelectModels(new[] { parent });
					parent.IsExpanded = true;
				}

				
			}
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

		#region Exit command

		private RelayCommand<Window> _exitCommand;

		/// <summary>
		/// Gets the Exit command.
		/// </summary>
		public RelayCommand<Window> ExitCommand
		{
			get { return _exitCommand ?? (_exitCommand = new RelayCommand<Window>(Exit)); }
		}

		/// <summary>
		/// Method to invoke when the Exit command is executed.
		/// </summary>
		private void Exit(Window window)
		{
			window?.Close();
		}

		#endregion


		public RelayCommand<string> ImportCommand { get; private set; }

		public RelayCommand NewPropCommand { get; private set; }

		public RelayCommand<Point> AddLightCommand { get; private set; }

		public RelayCommand LoadImageCommand { get; private set; }

		#endregion

	}


}
