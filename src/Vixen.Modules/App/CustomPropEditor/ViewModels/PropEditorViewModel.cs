﻿using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.WpfPropertyGrid;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Newtonsoft.Json.Linq;
using NLog;
using Common.WPFCommon.Services;
using Vixen.Sys;
using VixenModules.App.CustomPropEditor.Import;
using VixenModules.App.CustomPropEditor.Import.XLights;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Model.ExternalVendorInventory;
using VixenModules.App.CustomPropEditor.Model.InternalVendorInventory;
using VixenModules.App.CustomPropEditor.Services;
using PropertyData = Catel.Data.PropertyData;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	public class PropEditorViewModel : ViewModelBase
	{
		private static Logger Logging = LogManager.GetCurrentClassLogger();
		private bool _selectionChanging;
		
		public PropEditorViewModel()
		{
			FilePath = String.Empty;
			Prop = PropModelServices.Instance().CreateProp();
		}

		#region Prop model property

		/// <summary>
		/// Gets or sets the Prop value.
		/// </summary>
		[Browsable(false)]
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
				ElementOrderViewModel = new ElementOrderViewModel(value);
				RegisterModelEvents();
			}
		}

		/// <summary>
		/// Prop property data.
		/// </summary>
		public static readonly PropertyData PropProperty = RegisterProperty("Prop", typeof(Prop));

		#endregion

		#region Name property

		/// <summary>
		/// Gets or sets the Name value.
		/// </summary>
		[ViewModelToModel("Prop")]
		public string Name
		{
			get { return GetValue<string>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly PropertyData NameProperty = RegisterProperty("Name", typeof(string), null);

		#endregion

		#region Category property

		/// <summary>
		/// Gets or sets the Category value.
		/// </summary>
		[PropertyOrder(18)]
		[Category("Details")]
		[Description("A useful category like Christmas, Halloween, General")]
		[ViewModelToModel("Prop")]
		public string Type
		{
			get { return GetValue<string>(CategoryProperty); }
			set { SetValue(CategoryProperty, value); }
		}

		/// <summary>
		/// Category property data.
		/// </summary>
		public static readonly PropertyData CategoryProperty = RegisterProperty("Type", typeof(string), null);

		#endregion

		#region CreatedBy property

		/// <summary>
		/// Gets or sets the CreatedBy value.
		/// </summary>
		[DisplayName("Created By")]
		[PropertyOrder(20)]
		[Category("Details")]
		[ViewModelToModel("Prop")]
		public string CreatedBy
		{
			get { return GetValue<string>(CreatedByProperty); }
			set { SetValue(CreatedByProperty, value); }
		}

		/// <summary>
		/// CreatedBy property data.
		/// </summary>
		public static readonly PropertyData CreatedByProperty = RegisterProperty("CreatedBy", typeof(string), null);

		#endregion

		#region CreationDate property

		/// <summary>
		/// Gets or sets the CreationDate value.
		/// </summary>
		[DisplayName("Creation Date")]
		[Category("Details")]
		[PropertyOrder(20)]
		[ViewModelToModel("Prop")]
		public DateTime CreationDate
		{
			get { return GetValue<DateTime>(CreationDateProperty); }
			internal set { SetValue(CreationDateProperty, value); }
		}

		/// <summary>
		/// CreationDate property data.
		/// </summary>
		public static readonly PropertyData CreationDateProperty = RegisterProperty("CreationDate", typeof(DateTime), null);

		#endregion

		#region ModifiedDate property

		/// <summary>
		/// Gets or sets the ModifiedDate value.
		/// </summary>
		
		[DisplayName("Modified Date")]
		[Category("Details")]
		[PropertyOrder(21)]
		[ViewModelToModel("Prop")]
		public DateTime ModifiedDate
		{
			get { return GetValue<DateTime>(ModifiedDateProperty); }
			set { SetValue(ModifiedDateProperty, value); }
		}

		/// <summary>
		/// ModifiedDate property data.
		/// </summary>
		public static readonly PropertyData ModifiedDateProperty = RegisterProperty("ModifiedDate", typeof(DateTime), null);

		#endregion

		#region VendorMetadata property

		/// <summary>
		/// Gets or sets the VendorMetadata value.
		/// </summary>
		[Browsable(false)]
		[ViewModelToModel("Prop")]
		public VendorMetadata VendorMetadata
		{
			get { return GetValue<VendorMetadata>(VendorMetadataProperty); }
			set { SetValue(VendorMetadataProperty, value); }
		}

		/// <summary>
		/// VendorMetadata property data.
		/// </summary>
		public static readonly PropertyData VendorMetadataProperty = RegisterProperty("VendorMetadata", typeof(VendorMetadata), null);

		#endregion

		#region PhysicalMetadata property

		/// <summary>
		/// Gets or sets the PhysicalMetadata value.
		/// </summary>
		[Browsable(false)]
		[ViewModelToModel("Prop")]
		public PhysicalMetadata PhysicalMetadata
		{
			get { return GetValue<PhysicalMetadata>(PhysicalMetadataProperty); }
			set { SetValue(PhysicalMetadataProperty, value); }
		}

		/// <summary>
		/// PhysicalMetadata property data.
		/// </summary>
		public static readonly PropertyData PhysicalMetadataProperty = RegisterProperty("PhysicalMetadata", typeof(PhysicalMetadata), null);

		#endregion

		#region InformationMetadata property

		/// <summary>
		/// Gets or sets the InformationMetadata value.
		/// </summary>
		[Browsable(false)]
		[ViewModelToModel("Prop")]
		public InformationMetadata InformationMetadata
		{
			get { return GetValue<InformationMetadata>(InformationMetadataProperty); }
			set { SetValue(InformationMetadataProperty, value); }
		}

		/// <summary>
		/// InformationMetadata property data.
		/// </summary>
		public static readonly PropertyData InformationMetadataProperty = RegisterProperty("InformationMetadata", typeof(InformationMetadata), null);

		#endregion

		#region Overrides

		//We are not using these properties in the view so hiding them so the property grid does not expose them.

		[Browsable(false)]
		public new DateTime ViewModelConstructionTime => base.ViewModelConstructionTime;

		[Browsable(false)]
		public new int UniqueIdentifier => base.UniqueIdentifier;

		[Browsable(false)]
		public new string Title => base.Title;

		[Browsable(false)]
		public new bool IsClosed => base.IsClosed;

		[Browsable(false)]
		public new IViewModel ParentViewModel => base.ParentViewModel;

		[Browsable(false)]
		public new bool IsCanceled => base.IsCanceled;

		[Browsable(false)]
		public new bool IsSaved => base.IsSaved;

		#endregion

		#region DrawingPanelViewModel property

		/// <summary>
		/// Gets or sets the DrawingPanelViewModel value.
		/// </summary>
		[Browsable(false)]
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
		[Browsable(false)]
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

		#region ElementOrderViewModel property

		/// <summary>
		/// Gets or sets the ElementOrderViewModel value.
		/// </summary>
		[Browsable(false)]
		public ElementOrderViewModel ElementOrderViewModel
		{
			get { return GetValue<ElementOrderViewModel>(ElementOrderViewModelProperty); }
			set { SetValue(ElementOrderViewModelProperty, value); }
		}

		/// <summary>
		/// ElementOrderViewModel property data.
		/// </summary>
		public static readonly PropertyData ElementOrderViewModelProperty = RegisterProperty("ElementOrderViewModel", typeof(ElementOrderViewModel));

		#endregion

		#region FilePath property

		/// <summary>
		/// Gets or sets the FilePath value.
		/// </summary>
		[Browsable(false)]
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
		public static readonly PropertyData SelectedTabIndexProperty =
			RegisterProperty("SelectedTabIndex", typeof(int), null, (sender, e) => ((PropEditorViewModel) sender).OnSelectedTabIndexChanged());

		/// <summary>
		/// Called when the SelectedTabIndex property has changed.
		/// </summary>
		private void OnSelectedTabIndexChanged()
		{
			if (SelectedTabIndex == 0)
			{
				//var selectedModelIds = ElementTreeViewModel.SelectedItems.Select(e => e.ElementModel.Id).Distinct();
				ElementTreeViewModel.DeselectAll();
				//ElementOrderViewModel.Select(selectedModelIds);
			}
			else if(SelectedTabIndex == 1)
			{
				//var selectedModelIds = ElementOrderViewModel.SelectedItems.Select(e => e.ElementModel.Id).Distinct();
				ElementOrderViewModel.DeselectAll();
				//ElementTreeViewModel.Select(selectedModelIds);
			}
		}

		#endregion

		#region Events
		
		private void RegisterModelEvents()
		{

			ElementTreeViewModel.SelectedItems.CollectionChanged += ElementViewModel_SelectedItemsChanged;
			DrawingPanelViewModel.SelectedItems.CollectionChanged += DrawingViewModel_SelectedItemsChanged;
			ElementOrderViewModel.SelectedItems.CollectionChanged += ElementOrderViewModel_SelectedItemsChanged;
			ElementTreeViewModel.ModelsChanged += ElementTreeViewModel_ModelsChanged;
			DrawingPanelViewModel.LightModelsChanged += DrawingPanelViewModelsLightModelsChanged;
		}

		private void UnregisterModelEvents()
		{
			if (ElementTreeViewModel != null)
			{
				ElementTreeViewModel.SelectedItems.CollectionChanged -= ElementViewModel_SelectedItemsChanged;
				ElementTreeViewModel.ModelsChanged -= ElementTreeViewModel_ModelsChanged;
				DrawingPanelViewModel.LightModelsChanged -= DrawingPanelViewModelsLightModelsChanged;
			}

			if (DrawingPanelViewModel != null)
			{
				DrawingPanelViewModel.SelectedItems.CollectionChanged -= DrawingViewModel_SelectedItemsChanged;
			}
			if (ElementOrderViewModel != null)
			{
				ElementOrderViewModel.SelectedItems.CollectionChanged -= ElementOrderViewModel_SelectedItemsChanged;
			}

		}

		private void DrawingPanelViewModelsLightModelsChanged(object sender, EventArgs e)
		{
			ElementOrderViewModel.RefreshElementLeafViewModels();
		}

		private void ElementTreeViewModel_ModelsChanged(object sender, EventArgs e)
		{
			ElementOrderViewModel.RefreshElementLeafViewModels();
			DrawingPanelViewModel.RefreshLightViewModels();
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
					ElementOrderViewModel.DeselectAll();
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

		private void ElementOrderViewModel_SelectedItemsChanged(object sender, NotifyCollectionChangedEventArgs e)
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

		private bool TestIsDirty()
		{
			return IsDirty || ElementTreeViewModel.IsElementsDirty || DrawingPanelViewModel.IsLightsDirty;
		}

		internal void ClearDirtyFlag()
		{
			IsDirty = false;
			ElementTreeViewModel.ClearIsDirty();
			DrawingPanelViewModel.ClearIsDirty();
		}

		#endregion

		#region Delete command

		private Command _deleteCommand;

		/// <summary>
		/// Gets the Delete command.
		/// </summary
		[Browsable(false)]
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
			ElementOrderViewModel.RefreshElementLeafViewModels();
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
		[Browsable(false)]
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
			openFileService.Filter = "Prop Files(*.prp)|*.prp";
			openFileService.FileName = string.Empty;
			if (await openFileService.DetermineFileAsync())
			{
				string path = openFileService.FileNames.First();
				if (!string.IsNullOrEmpty(path))
				{
					openFileService.InitialDirectory = Path.GetDirectoryName(path);
					LoadPropFromPath(path);
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
			get { return _saveModelCommand ?? (_saveModelCommand = new Command(SaveModel)); }
		}

		/// <summary>
		/// Method to invoke when the SaveModel command is executed.
		/// </summary>
		private async void SaveModel()
		{
			ModifiedDate = DateTime.Now;
			if (string.IsNullOrEmpty(FilePath))
			{
				SaveModelAs();
			}
			else
			{
				PropModelPersistenceService.UpdateModel(Prop, FilePath);
				ClearDirtyFlag();
			}
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
			get { return _saveModelAsCommand ?? (_saveModelAsCommand = new Command(SaveModelAs)); }
		}

		/// <summary>
		/// Method to invoke when the SaveModelAs command is executed.
		/// </summary>
		private async void SaveModelAs()
		{
			ModifiedDate = DateTime.Now;
			var dependencyResolver = this.GetDependencyResolver();
			var saveFileService = dependencyResolver.Resolve<ISaveFileService>();
			saveFileService.Filter = "Prop Files(*.prp)|*.prp";
			saveFileService.CheckPathExists = true;
			saveFileService.FileName = CleanseNameString(Prop.Name);
			if (await saveFileService.DetermineFileAsync())
			{
				saveFileService.InitialDirectory = Path.GetDirectoryName(saveFileService.FileName);
				// User selected a file
				if (PropModelPersistenceService.SaveModel(Prop, saveFileService.FileName))
				{
					FilePath = saveFileService.FileName;
					ClearDirtyFlag();
				}
			}
		}

		#endregion

		#region Menu Commands

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
			if (TestIsDirty())
			{
				var dependencyResolver = this.GetDependencyResolver();
				var mbs = dependencyResolver.Resolve<IMessageBoxService>();
				var response = mbs.GetUserConfirmation($"Save Prop \"{CleanseNameString(Prop.Name)}\" ", "Save");
				if (response.Result == MessageResult.OK)
				{
					SaveModel();
				}
				else if (response.Result == MessageResult.Cancel)
				{
					e.Cancel = true;
				}
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
			openFileService.IsMultiSelect = false;
			if (openFileService.InitialDirectory == null)
			{
				openFileService.InitialDirectory = Paths.DataRootPath;
			}
			openFileService.Filter = "xModel (*.xmodel)|*.xmodel";
			if (await openFileService.DetermineFileAsync())
			{
				string path = openFileService.FileName;
				openFileService.InitialDirectory = Path.GetDirectoryName(path);
				if (!string.IsNullOrEmpty(path))
				{
					await ImportProp(path);
				}
			}
		}

		private async Task<bool> ImportProp(string path)
		{
			var dependencyResolver = this.GetDependencyResolver();
			var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
			pleaseWaitService.Show();
			try
			{
				IModelImport import = new XModelImport();
				var p = await import.ImportAsync(path);
				if (p != null)
				{
					Prop = p;
					FilePath = String.Empty;
					//Switch to selection mode VIX-2784
					DrawingPanelViewModel.IsDrawing = false;
				}
			}
			catch (Exception e)
			{
				pleaseWaitService.Hide();
				Logging.Error(e, "An error occuring importing the xModel.");
				var mbs = new MessageBoxService();
				mbs.ShowError($"An error occurred importing the xModel. Please notify the Vixen Team.", "Error Importing xModel");
				return false;
			}

			pleaseWaitService.Hide();

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
			get { return _openVendorBrowserCommand ?? (_openVendorBrowserCommand = new TaskCommand(OpenVendorBrowserAsync)); }
		}

		/// <summary>
		/// Method to invoke when the OpenVendorBrowserCommand command is executed.
		/// </summary>
		private async Task OpenVendorBrowserAsync()
		{
			XModelInventoryImporter mi = new XModelInventoryImporter();

			List<ModelInventory> vendorInventories = new List<ModelInventory>();

			var vendorLinks = await GetVendorUrls();
			if (!vendorLinks.Any()) { return; }
			

			var dependencyResolver = this.GetDependencyResolver();
			var ds = dependencyResolver.Resolve<IDownloadService>();

			foreach (var vendorLink in vendorLinks)
			{
				try
				{
					var xml = await ds.GetFileAsStringAsync(new Uri(vendorLink.Url));
					vendorInventories.Add(await mi.Import(xml));
				}
				catch (Exception e)
				{
					Logging.Error(e, $"An error occurred retrieving the inventory from: {vendorLink.Name}, {vendorLink.Url}");
					var mbs = dependencyResolver.Resolve<IMessageBoxService>();
					mbs.ShowError($"Unable to retrieve inventory from {vendorLink.Name}\nEnsure you have an active internet connection.", "Error Retrieving Inventory");
				}
			}

			if (!vendorInventories.Any()) { return; }
			var uiVisualizerService = dependencyResolver.Resolve<IUIVisualizerService>();
			var vm = new VendorInventoryWindowViewModel(vendorInventories, dependencyResolver.Resolve<IProcessService>());
			bool? result = await uiVisualizerService.ShowDialogAsync(vm);

			if (result.HasValue && result.Value)
			{
				var status = await LoadVendorModel(vm.SelectedModelLink);

				if (status.Item1)
				{
					if (status.Item2 == ModelType.XModel)
					{
						Prop.PhysicalMetadata.Width = vm.SelectedProduct.Width;
						Prop.PhysicalMetadata.Height = vm.SelectedProduct.Height;
						Prop.PhysicalMetadata.Depth = vm.SelectedProduct.Thickness;
						Prop.PhysicalMetadata.Material = vm.SelectedProduct.Material;
						Prop.PhysicalMetadata.BulbType = vm.SelectedProduct.PixelDescription;
						Prop.PhysicalMetadata.NodeCount = vm.SelectedProduct.PixelCount.ToString();
						Prop.InformationMetadata.Notes = vm.SelectedProduct.Notes;
						Prop.Type = vm.SelectedProduct.ProductType;
					}
					
					//Ensure the Vendor info is populated
					if (string.IsNullOrEmpty(Prop.VendorMetadata.Name))
					{
						Prop.VendorMetadata.Name = vm.SelectedInventory.Vendor.Name;
					}

					if (string.IsNullOrEmpty(Prop.VendorMetadata.Contact))
					{
						Prop.VendorMetadata.Contact = vm.SelectedInventory.Vendor.Contact;
					}

					if (string.IsNullOrEmpty(Prop.VendorMetadata.Email))
					{
						Prop.VendorMetadata.Email = vm.SelectedInventory.Vendor.Email;
					}

					if (string.IsNullOrEmpty(Prop.VendorMetadata.Phone))
					{
						Prop.VendorMetadata.Phone = vm.SelectedInventory.Vendor.Phone;
					}

					if (string.IsNullOrEmpty(Prop.VendorMetadata.Website))
					{
						var website = vm.SelectedInventory.Vendor.WebLinks.Where(x => x.Name.Equals("Website"));
						if (website.Any())
						{
							Prop.VendorMetadata.Website = website.First().Link.AbsoluteUri;
						}
					}
				}
				
			}
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
			get { return _newPropCommand ?? (_newPropCommand = new Command(NewProp)); }
		}

		private const string TokenPattern = @"{[0-9]+}";
		/// <summary>
		/// Method to invoke when the NewProp command is executed.
		/// </summary>
		private void NewProp()
		{
			MessageBoxService mbs = new MessageBoxService();
			var result = mbs.GetUserInput("Please enter the model name.", "Create Model", "New Prop");
			if (result.Result == MessageResult.OK)
			{
				var name = result.Response;
				if (!Regex.IsMatch(name, TokenPattern))
				{
					name = $"{name} {{1}}";
				}
				Prop = PropModelServices.Instance().CreateProp(name);
				FilePath = string.Empty;
			}
		}

		#endregion

		#region AddLight command

		private Command<Point> _addLightCommand;

		/// <summary>
		/// Gets the AddLight command.
		/// </summary>
		[Browsable(false)]
		public Command<Point> AddLightCommand
		{
			get { return _addLightCommand ?? (_addLightCommand = new Command<Point>(AddLight)); }
		}

		/// <summary>
		/// Method to invoke when the AddLight command is executed.
		/// </summary>
		private void AddLight(Point p)
		{
			var target = ElementTreeViewModel.SelectedItem;

			var model = PropModelServices.Instance().AddLight(target?.ElementModel, p);

			if (model == null) return;

			DrawingPanelViewModel.RefreshLightViewModels();
			ElementOrderViewModel.RefreshElementLeafViewModels();

			if (model == target?.ElementModel)
			{
				ElementTreeViewModel.SelectModels(new[] { target });
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

		#endregion

		#region LoadImage command

		private Command _loadImageCommand;

		/// <summary>
		/// Gets the LoadImage command.
		/// </summary>
		[Browsable(false)]
		public Command LoadImageCommand
		{
			get { return _loadImageCommand ?? (_loadImageCommand = new Command(LoadImage)); }
		}

		/// <summary>
		/// Method to invoke when the LoadImage command is executed.
		/// </summary>
		private async void LoadImage()
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

		#endregion

		#region Help command

		private Command _helpCommand;

		/// <summary>
		/// Gets the Help command.
		/// </summary>
		[Browsable(false)]
		public Command HelpCommand
		{
			get { return _helpCommand ?? (_helpCommand = new Command(Help)); }
		}

		/// <summary>
		/// Method to invoke when the Help command is executed.
		/// </summary>
		private void Help()
		{
			var url = "http://www.vixenlights.com/vixen-3-documentation/preview/custom-prop-editor/";
			System.Diagnostics.Process.Start(url);
		}

		#endregion

		#region ColorOptions command

		private Command _colorOptionsCommand;

		/// <summary>
		/// Gets the ColorOptions command.
		/// </summary>
		[Browsable(false)]
		public Command ColorOptionsCommand
		{
			get { return _colorOptionsCommand ?? (_colorOptionsCommand = new Command(ColorOptions)); }
		}

		/// <summary>
		/// Method to invoke when the ColorOptions command is executed.
		/// </summary>
		private async void ColorOptions()
		{
			ConfigurationWindowViewModel vm = new ConfigurationWindowViewModel();
			var dependencyResolver = this.GetDependencyResolver();
			var uiVisualizerService = dependencyResolver.Resolve<UIVisualizerService>();
			await uiVisualizerService.ShowDialogAsync(vm);
			await VixenSystem.SaveModuleConfigAsync();
		}

		#endregion

		#endregion

		private void LoadPropFromPath(string path)
		{
			Prop p = PropModelServices.Instance().LoadProp(path);
			if (p != null)
			{
				Prop = p;
				FilePath = path;
				ClearDirtyFlag();
			}
			else
			{
				//Alert user
			}
		}

		private async Task<Tuple<bool, ModelType>> LoadVendorModel(ModelLink modelLink)
		{
			var targetPath = Path.Combine(Path.GetTempPath() + Guid.NewGuid());
			var dependencyResolver = this.GetDependencyResolver();
			var ds = dependencyResolver.Resolve<IDownloadService>();
			var pleaseWaitService = dependencyResolver.Resolve<IPleaseWaitService>();
			var mbs = dependencyResolver.Resolve<IMessageBoxService>();

			var status = new Tuple<bool, ModelType>(false, ModelType.XModel);

			pleaseWaitService.Show();

			if (modelLink.Software == ModelType.Prop)
			{
				bool success = await ds.GetFileAsync(modelLink.Link, targetPath);
				if (success)
				{
					LoadPropFromPath(targetPath);
					status = new Tuple<bool, ModelType>(true, ModelType.Prop); ;
				}
				else
				{
					mbs.ShowError("Unable to download the model from the vendor.\nEnsure you have an active internet connection.", "Error Downloading Model.");
				}
			}
			else
			{
				//The vendors provide a link to the xModel, but may have a Vixen prop of the same name alongside it.
				//If it exists we will prefer it.
				string propUrl = modelLink.Link.AbsoluteUri.Replace(@".xmodel", @".prp");

				bool success = await ds.GetFileAsync(new Uri(propUrl), targetPath);
				
				if (success)
				{
					LoadPropFromPath(targetPath);
					status = new Tuple<bool, ModelType>(true, ModelType.Prop); ;
				}
				else
				{
					success = await ds.GetFileAsync(modelLink.Link, targetPath);
					if (success)
					{
						await ImportProp(targetPath);
						status = new Tuple<bool, ModelType>(true, ModelType.XModel);
					}
					else
					{
						mbs.ShowError("Unable to download the model from the vendor.\nEnsure you have an active internet connection.", "Error Downloading Model.");
					}
				}
			}

			

			pleaseWaitService.Hide();
			return status;
		}

		private async Task<VendorLink[]> GetVendorUrls()
		{
			var dependencyResolver = this.GetDependencyResolver();
			var ds = dependencyResolver.Resolve<IDownloadService>();

			try
			{
				var vendorJson = await ds.GetFileAsStringAsync(new Uri(@"https://app.vixenlights.com/vendor.json"));
				var o = JArray.Parse(vendorJson);

				var vendors = o.Select(p => new VendorLink()
				{
					Name = (string)p["Name"],
					Url = (string)p["Url"]
				}).ToArray();

				return vendors;
			}
			catch (Exception e)
			{
				Logging.Error(e, "Error retrieving the vendor list.");
				var mbs = dependencyResolver.Resolve<IMessageBoxService>();
				mbs.ShowError("Unable to retrieve vendor list. Ensure you have an active internet connection.", "Error Retrieving Vendors");
			}
			
			return new VendorLink[0];
		}

		private string CleanseNameString(string name)
		{
			Regex tokenRegex = new Regex(@"{\d+}");
			Regex spaceRegex = new Regex(@"[ ]{2,}");
			var returnValue = name;
			var match = tokenRegex.Match(name);
			while (match.Success)
			{
				returnValue = returnValue.Replace(match.Value, string.Empty);
				match = match.NextMatch();
			}

			//Trim trailing spaces.
			returnValue = returnValue.Trim();

			//Remove consecutive spaces.
			match = spaceRegex.Match(returnValue);
			while (match.Success)
			{
				returnValue = returnValue.Replace(match.Value, " ");
				match = match.NextMatch();
			}

			return returnValue;
		}

	}


}
