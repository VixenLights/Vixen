using System.Collections.ObjectModel;
using System.ComponentModel;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Common.Controls.Wizard;
using Common.WPFCommon.Services;
using GongSolutions.Wpf.DragDrop;
using Orc.Theming;
using Orc.Wizard;
using Vixen.Sys;
using Vixen.Sys.Managers;
using Vixen.Sys.Props;
using Vixen.Sys.Props.Model;
using VixenApplication.SetupDisplay.Wizards.Factory;
using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenApplication.SetupDisplay.Wizards.Wizard;
using DragDropEffects = System.Windows.DragDropEffects;
using IDropTarget = GongSolutions.Wpf.DragDrop.IDropTarget;

namespace VixenApplication.SetupDisplay.ViewModels
{
    public class PropNodeTreeViewModel : ViewModelBase, IDropTarget, IDragSource, IDisposable
	{
        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		public event EventHandler ModelsChanged;

		public PropNodeTreeViewModel()
        {
            PropManager = VixenSystem.Props;
			PropNodeViewModel vm = new(PropManager.RootNode, null);
            RootNodeViewModel = [vm];
           // RootNodesViewModels = new ObservableCollection<PropNodeViewModel>(RootNodes.Select(x => new PropNodeViewModel(x, null)));
            SelectedItems = new();
			SelectedItemNodePoints = new();
            SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
		}

        #region PropManager model property

        /// <summary>
        /// Gets or sets the PropManager value.
        /// </summary>
        [Model]
        public PropManager PropManager
        {
            get { return GetValue<PropManager>(PropManagerProperty); }
            private set { SetValue(PropManagerProperty, value); }
        }

        /// <summary>
        /// Prop property data.
        /// </summary>
        public static readonly IPropertyData PropManagerProperty = RegisterProperty<PropManager>(nameof(PropManager));

		#endregion

		#region RootNodeViewModes property

		/// <summary>
		/// Gets or sets the RootNodesViewModels value.
		/// </summary>
		public ObservableCollection<PropNodeViewModel> RootNodeViewModel
		{
			get { return GetValue<ObservableCollection<PropNodeViewModel>>(RootNodeViewModelProperty); }
			set { SetValue(RootNodeViewModelProperty, value); }
		}

		/// <summary>
		/// RootNodesViewModels property data.
		/// </summary>
		public static readonly IPropertyData RootNodeViewModelProperty = RegisterProperty<ObservableCollection<PropNodeViewModel>>(nameof(RootNodeViewModel));

		#endregion

		#region LeafNodes property

		/// <summary>
		/// Gets or sets the LeafNodes value.
        /// </summary>
		public ObservableCollection<PropNodeViewModel> LeafNodes
		{
            get
            {
                return new(RootNodeViewModel.SelectMany(x => x.GetLeafEnumerator()));
            }
        }

		#endregion

		#region SelectedItems property

		/// <summary>
		/// Gets or sets the SelectedItems value.
		/// </summary>
		public ObservableCollection<PropNodeViewModel> SelectedItems
        {
            get { return GetValue<ObservableCollection<PropNodeViewModel>>(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        /// <summary>
        /// SelectedItems property data.
        /// </summary>
        public static readonly IPropertyData SelectedItemsProperty =
            RegisterProperty<ObservableCollection<PropNodeViewModel>>(nameof(SelectedItems));


        private void SelectedItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (SelectedItems.Count == 1)
            {
                SelectedItem = SelectedItems.First();
                if (SelectedItem.PropNode.IsLeaf && SelectedItem.PropNode.Prop != null &&
                    SelectedItem.PropNode.Prop.PropModel is ILightPropModel model)
                {
                    SelectedItemNodePoints = model.Nodes;
                }
				
            }
            else
            {
                SelectedItem = null;
				SelectedItemNodePoints.Clear();
            }

            CopyCommand.RaiseCanExecuteChanged();
            CutCommand.RaiseCanExecuteChanged();
            PasteCommand.RaiseCanExecuteChanged();
            CreateGroupCommand.RaiseCanExecuteChanged();
            MoveToGroupCommand.RaiseCanExecuteChanged();
            CreateNodeCommand.RaiseCanExecuteChanged();
        }


        #endregion

        #region SelectedItem property

        /// <summary>
        /// Gets or sets the SelectedItem value.
        /// </summary>
        public PropNodeViewModel? SelectedItem
        {
            get { return GetValue<PropNodeViewModel>(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// SelectedItem property data.
        /// </summary>
        public static readonly IPropertyData SelectedItemProperty = RegisterProperty<PropNodeViewModel>(nameof(SelectedItem));

		#endregion

        #region SelectedItemNodePoints

        /// <summary>
        /// Gets or sets the SelectedItemNodePoints value.
        /// </summary>;
        [Model]
        public ObservableCollection<NodePoint> SelectedItemNodePoints
        {
            get { return GetValue<ObservableCollection<NodePoint>>(SelectedItemNodesProperty); }
            private set { SetValue(SelectedItemNodesProperty, value); }
        }

        /// <summary>;
        /// SelectedItemNodePoints property data.
        /// </summary>;
        public static readonly IPropertyData SelectedItemNodesProperty = RegisterProperty<ObservableCollection<NodePoint>>(nameof(SelectedItemNodePoints));

        #endregion

		#region Selection

		public void DeselectAll()
        {
            SelectedItems.ToList().ForEach(x => x.IsSelected = false);
            SelectedItems.Clear();
        }

        public void SelectModels(IEnumerable<PropNodeViewModel> propNodeModels)
        {
            foreach (var elementModelViewModel in propNodeModels)
            {
                elementModelViewModel.IsSelected = true;
                if (elementModelViewModel.ParentViewModel is PropNodeViewModel parent)
                {
                    parent.IsExpanded = true;
                }
            }
        }

        public void DeselectModels(IEnumerable<PropNodeViewModel> propNodeModels)
        {
            foreach (var elementModelViewModel in propNodeModels)
            {
                elementModelViewModel.IsSelected = false;
            }
        }

		#endregion

		#region Commands

		#region CreateGroup command

		private Command _createGroupCommand;

		/// <summary>
		/// Gets the CreateGroup command.
		/// </summary>
		public Command CreateGroupCommand
		{
			get { return _createGroupCommand ??= new(CreateGroup, CanCreateGroup); }
		}

		/// <summary>
		/// Method to invoke when the CreateGroup command is executed.
		/// </summary>
		private void CreateGroup()
		{
			var result = RequestNewName(String.Empty);
			if (result.Result == MessageResult.OK)
			{
				var elementsToGroup = SelectedItems.Select(x => x.PropNode).Distinct().ToList();
				DeselectAll();
				PropManager.CreateGroupForPropNodes(result.Response, elementsToGroup);
				OnModelsChanged();
			}
		}

		/// <summary>
		/// Method to check whether the CreateGroup command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanCreateGroup()
		{
			return CanGroup();
		}

        private bool CanGroup()
        {
            return SelectedItems.Any();
        }

		#endregion

		#region MoveToGroup command

		private Command _moveToGroupCommand;

		/// <summary>
		/// Gets the MoveToGroup command.
		/// </summary>
		public Command MoveToGroupCommand
		{
			get { return _moveToGroupCommand ??= new(MoveToGroup, CanMoveToGroup); }
		}

		/// <summary>
		/// Method to invoke when the MoveToGroup command is executed.
		/// </summary>
		private void MoveToGroup()
		{
			//var result = RequestNewName(String.Empty);
			//if (result.Result == MessageResult.OK)
			//{
			//	var pms = PropModelServices.Instance();

			//	//See if we are moving all items and can place the new group them under our parent group.
			//	ElementModel parentForGroup = null;
			//	var firstParentViewModel = SelectedItems.First().ParentViewModel as ElementModelViewModel;
			//	if (firstParentViewModel != null && !firstParentViewModel.Children.Except(SelectedItems.Select(x => x.ElementModel)).Any())
			//	{
			//		parentForGroup = firstParentViewModel?.ElementModel;
			//	}

			//	var parentToJoin = pms.CreateNode(result.Response, parentForGroup);
			//	foreach (var elementModelViewModel in SelectedItems.ToList())
			//	{
			//		elementModelViewModel.IsSelected = false;
			//		ElementModel parentToLeave = (elementModelViewModel.ParentViewModel as ElementModelViewModel)?.ElementModel;
			//		if (parentToLeave != null)
			//		{
			//			pms.AddToParent(elementModelViewModel.ElementModel, parentToJoin);
			//			pms.RemoveFromParent(elementModelViewModel.ElementModel, parentToLeave);
			//		}
			//	}
			//	OnModelsChanged();

			//	DeselectAll();
			//	var newModel = ElementModelLookUpService.Instance.GetModels(parentToJoin.Id);
			//	if (newModel != null && newModel.Any())
			//	{
			//		newModel.First().IsExpanded = true;
			//		SelectModels(newModel);
			//	}


			//}
		}

		/// <summary>
		/// Method to check whether the MoveToGroup command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanMoveToGroup()
		{
			return CanGroup();
		}

		#endregion

		#region SubstitutionRename command

		private Command _substitutionRenameCommand;

		/// <summary>
		/// Gets the PatternRename command.
		/// </summary>
		public Command SubstitutionRenameCommand
		{
			get { return _substitutionRenameCommand ??= new(SubstitutionRename, CanSubstitutionRename); }
		}

		/// <summary>
		/// Method to invoke when the PatternRename command is executed.
		/// </summary>
		private void SubstitutionRename()
		{
			SubstitutionRenameSelectedItems();
		}

		/// <summary>
		/// Method to check whether the PatternRename command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanSubstitutionRename()
		{
			return SelectedItems.Count > 1;
		}

		public bool SubstitutionRenameSelectedItems()
		{
			//if (SelectedItems.Count <= 1)
			//	return false;

			//List<string> oldNames = new List<string>(SelectedItems.Select(x => x.Name).ToArray());
			//SubstitutionRenamer renamer = new SubstitutionRenamer(oldNames);
			//if (renamer.ShowDialog() == DialogResult.OK)
			//{
			//	for (int i = 0; i < SelectedItems.Count; i++)
			//	{
			//		if (i >= renamer.Names.Count)
			//		{
			//			Logging.Warn("Bulk renaming elements, and ran out of new names!");
			//			break;
			//		}


			//		SelectedItems[i].Name = PropModelServices.Instance().Uniquify(renamer.Names[i], 2, SelectedItems[i].ElementModel);
			//	}

			//	return true;
			//}

			return false;
		}

		#endregion

		#region Rename command

		private Command _renameCommand;

		/// <summary>
		/// Gets the Rename command.
		/// </summary>
		public Command RenameCommand
		{
			get { return _renameCommand ??= new(Rename); }
		}

		/// <summary>
		/// Method to invoke when the Rename command is executed.
		/// </summary>
		private void Rename()
		{
			//if (SelectedItems.Count == 1)
			//{
			//	MessageBoxService mbs = new MessageBoxService();
			//	var result = mbs.GetUserInput("Please enter the new name.", "Rename", SelectedItem.Name);
			//	if (result.Result == MessageResult.OK)
			//	{
			//		SelectedItems.First().Name = PropModelServices.Instance().Uniquify(result.Response);
			//	}
			//}
			//else
			//{
			//	PatternRenameSelectedItems();
			//}
		}


		public bool PatternRenameSelectedItems()
		{
			//if (SelectedItems.Count <= 1)
			//	return false;

			//List<string> oldNames = new List<string>(SelectedItems.Select(x => x.ElementModel.Name).ToArray());
			//NameGenerator renamer = new NameGenerator(oldNames);
			//if (renamer.ShowDialog() == DialogResult.OK)
			//{
			//	for (int i = 0; i < SelectedItems.Count; i++)
			//	{
			//		if (i >= renamer.Names.Count)
			//		{
			//			Logging.Warn("Bulk renaming elements, and ran out of new names!");
			//			break;
			//		}


			//		SelectedItems[i].Name = PropModelServices.Instance().Uniquify(renamer.Names[i], 2, SelectedItems[i].ElementModel);
			//	}

			//	return true;
			//}

			return false;
		}


		#endregion

		#region CreateNode command

		private Command _createNodeCommand;

		/// <summary>
		/// Gets the CreateNode command.
		/// </summary>
		public Command CreateNodeCommand
		{
			get { return _createNodeCommand ??= new(CreateNode, CanCreateNode); }
		}

		/// <summary>
		/// Method to invoke when the CreateNode command is executed.
		/// </summary>
		private void CreateNode()
		{
			var result = RequestNewName(string.Empty);

			if (result.Result == MessageResult.OK)
			{
				PropManager.CreateNode(result.Response, SelectedItem!=null?SelectedItem.PropNode:PropManager.RootNode);
				//Ensure parent is expanded
                if (SelectedItem != null)
                {
                    SelectedItem.IsExpanded = true;
                }
			}

		}

		/// <summary>
		/// Method to check whether the CreateNode command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanCreateNode()
        {
            return true;
        }

		#endregion

        #region CreateProp command

        private TaskCommand _createPropCommand;

        /// <summary>
        /// Gets the CreateProp command.
        /// </summary>
        public TaskCommand CreatePropCommand
        {
            get { return _createPropCommand ??= new(CreateProp, CanCreateProp); }
        }

        /// <summary>
        /// Method to invoke when the CreateNode command is executed.
        /// </summary>
        private async Task CreateProp()
        {
            var prop = await GenerateProp();

            if (prop != null)
            { 
                PropManager.AddProp(prop, SelectedItem != null ? SelectedItem.PropNode : PropManager.RootNode);
			   
                //Ensure parent is expanded
                if (SelectedItem != null)
                {
                    SelectedItem.IsExpanded = true;
                }
            }

        }

        /// <summary>
        /// Method to check whether the CreateNode command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool CanCreateProp()
        {
            return true;
        }

        #endregion

		#region Cut command

		private Command _cutCommand;

		/// <summary>
		/// Gets the Cut command.
		/// </summary>
		public Command CutCommand
		{
			get { return _cutCommand ??= new(Cut, CanCut); }
		}

		/// <summary>
		/// Method to invoke when the Cut command is executed.
		/// </summary>
		private void Cut()
		{
			//List<ElementModel> clipData = new List<ElementModel>();
			//clipData.AddRange(SelectedItems.Select(x => x.ElementModel).ToList());

			//IDataObject dataObject = new DataObject(ClipboardFormatName);
			//dataObject.SetData(clipData);
			//Clipboard.SetDataObject(dataObject, true);
			//var itemsToCut = SelectedItems.ToList();
			//DeselectAll();
			//foreach (var elementModelViewModel in itemsToCut)
			//{
			//	var parentToLeave = elementModelViewModel.ParentViewModel as ElementModelViewModel;
			//	if (parentToLeave != null)
			//	{
			//		PropModelServices.Instance().RemoveFromParent(elementModelViewModel.ElementModel, parentToLeave.ElementModel);
			//	}
			//}

			//OnModelsChanged();
		}

		/// <summary>
		/// Method to check whether the Cut command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanCut()
		{
			return SelectedItems.Any() && CanGroup();
		}

		#endregion

		#region Copy command

		private Command _copyCommand;

		/// <summary>
		/// Gets the Copy command.
		/// </summary>
		public Command CopyCommand
		{
			get { return _copyCommand ??= new(Copy, CanCopy); }
		}

		/// <summary>
		/// Method to invoke when the Copy command is executed.
		/// </summary>
		private void Copy()
		{
			//List<ElementModel> clipData = new List<ElementModel>();
			//clipData.AddRange(SelectedItems.Select(x => x.ElementModel).ToList());

			//IDataObject dataObject = new DataObject(ClipboardFormatName);
			//dataObject.SetData(clipData);
			//Clipboard.SetDataObject(dataObject, true);
		}

		/// <summary>
		/// Method to check whether the Copy command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanCopy()
		{
			return SelectedItems.Any() && CanGroup();
		}

		#endregion

		#region Paste command

		private Command _pasteCommand;

		/// <summary>
		/// Gets the Paste command.
		/// </summary>
		public Command PasteCommand
		{
			get { return _pasteCommand ??= new(Paste, CanPaste); }
		}

		/// <summary>
		/// Method to invoke when the Paste command is executed.
		/// </summary>
		private void Paste()
		{
			//System.Windows.Forms.IDataObject dataObject = System.Windows.Forms.Clipboard.GetDataObject();

			//if (dataObject != null && SelectedItems.Count == 1)
			//{
			//	if (dataObject.GetDataPresent(ClipboardFormatName.Name))
			//	{
			//		var parent = SelectedItem;
			//		DeselectAll();
			//		var data = dataObject.GetData(ClipboardFormatName.Name) as List<ElementModel>;

			//		if (data != null)
			//		{
			//			foreach (var elementModel in data)
			//			{
			//				if (parent.ElementModel.Equals(elementModel) || parent.Children.Contains(elementModel)) continue;
			//				//if(elementModel.Parents.Contains(parent.ElementModel.Id)) continue; //Don't add another copy 
			//				PropModelServices.Instance().FindOrCreateElementModelTree(elementModel, parent.ElementModel);
			//			}
			//		}

			//		OnModelsChanged();
			//		SelectModels(new[] { parent });
			//	}
			//}
		}

		/// <summary>
		/// Method to check whether the Paste command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanPaste()
		{
			//System.Windows.Forms.IDataObject dataObject = System.Windows.Forms.Clipboard.GetDataObject();

			//if (dataObject != null && SelectedItems.Count == 1)
			//{
			//	if (dataObject.GetDataPresent(ClipboardFormatName.Name))
			//	{
			//		var data = dataObject.GetData(ClipboardFormatName.Name) as List<ElementModel>;

			//		if (data != null)
			//		{
			//			if (data.All(x => x.IsLightNode && !SelectedItem.ElementModel.Children.Contains(x)) && SelectedItem.ElementModel.CanAddLightNodes)
			//			{
			//				//We can paste our contents
			//				return true;
			//			}
			//			if (data.All(x => x.IsGroupNode && !SelectedItem.ElementModel.Children.Contains(x)) && SelectedItem.ElementModel.CanAddGroupNodes)
			//			{
			//				//We can paste our contents
			//				return true;
			//			}
			//		}
			//	}
			//}

			return false;
		}

		#endregion

		#region PasteAsNew command

		private Command _pasteAsNewCommand;

		/// <summary>
		/// Gets the PasteAsNew command.
		/// </summary>
		public Command PasteAsNewCommand
		{
			get { return _pasteAsNewCommand ??= new(PasteAsNew, CanPasteAsNew); }
		}

		/// <summary>
		/// Method to invoke when the PasteAsNew command is executed.
		/// </summary>
		private void PasteAsNew()
		{
		//	System.Windows.Forms.IDataObject dataObject = System.Windows.Forms.Clipboard.GetDataObject();

		//	if (dataObject != null && SelectedItems.Count == 1)
		//	{
		//		if (dataObject.GetDataPresent(ClipboardFormatName.Name))
		//		{
		//			var parent = SelectedItem;
		//			MessageBoxService mbs = new MessageBoxService();
		//			var result = mbs.GetUserInput("Please enter the new name.", "Paste As New", PropModelServices.Instance().Uniquify(parent.Name));
		//			if (result.Result == MessageResult.OK)
		//			{
		//				DeselectAll();
		//				var newElementModels = new List<ElementModelViewModel>();
		//				var data = dataObject.GetData(ClipboardFormatName.Name) as List<ElementModel>;

		//				if (data != null)
		//				{
		//					foreach (var elementModel in data)
		//					{
		//						var em = PropModelServices.Instance().CreateElementModelTree(elementModel, parent.ElementModel, result.Response);
		//						var evm = ElementModelLookUpService.Instance.GetModels(em.Id);
		//						if (evm != null)
		//						{
		//							newElementModels.AddRange(evm);
		//						}
		//					}
		//				}

		//				OnModelsChanged();
		//				SelectModels(newElementModels);
		//			}


		//		}
		//	}
		}

		/// <summary>
		/// Method to check whether the PasteAsNew command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanPasteAsNew()
		{
			//System.Windows.Forms.IDataObject dataObject = System.Windows.Forms.Clipboard.GetDataObject();

			//if (dataObject != null && SelectedItems.Count == 1)
			//{
			//	if (dataObject.GetDataPresent(ClipboardFormatName.Name))
			//	{
			//		var data = dataObject.GetData(ClipboardFormatName.Name) as List<ElementModel>;

			//		if (data != null)
			//		{
			//			if (data.All(x => x.IsLightNode && SelectedItem.ElementModel.CanAddLightNodes))
			//			{
			//				//We can paste our contents
			//				return true;
			//			}
			//			if (data.All(x => x.IsGroupNode && SelectedItem.ElementModel.CanAddGroupNodes))
			//			{
			//				//We can paste our contents
			//				return true;
			//			}
			//		}
			//	}
			//}

			return false;
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
            get { return _deleteCommand ??= new(Delete, CanDelete); }
        }

        /// <summary>
        /// Method to invoke when the Delete command is executed.
        /// </summary>
        private void Delete()
        {
            
            var itemsToDelete = SelectedItems.ToList(); 
            SelectedItems.Clear();
            itemsToDelete.ForEach(x => x.RemoveFromParent());
        }

        /// <summary>
        /// Method to check whether the Delete command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool CanDelete()
        {
            return !SelectedItems.Any(x => x.PropNode.Equals(PropManager.RootNode));
        }

        #endregion


		#endregion

		#region Drag n Drop

		public void OnDragEnter(DragEventArgs e)
        {
            
        }

        public void OnDragLeave(EventArgs e)
        {
            
        }

        public void OnDragDrop(DragEventArgs e)
        {
           
        }

        public void OnDragOver(DragEventArgs e)
        {
            
        }

        public void StartDrag(IDragInfo dragInfo)
        {
            
        }

        public bool CanStartDrag(IDragInfo dragInfo)
        {
            return false;
        }

        public void Dropped(IDropInfo dropInfo)
        {
           
        }

        public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
        {
            
        }

        public void DragCancelled()
        {
            
        }

        public bool TryCatchOccurredException(Exception exception)
        {
            return false;
        }

        public void DragOver(IDropInfo dropInfo)
        {
           
        }

        public void Drop(IDropInfo dropInfo)
        {
            
        }

		#endregion

		#region Utility

		private MessageBoxResponse RequestNewName(string suggestedName, bool createGroup = true)
        {
            var nameType = createGroup ? "group" : "prop";
            var dependencyResolver = this.GetDependencyResolver();
            var mbs = dependencyResolver.Resolve<IMessageBoxService>();
            return mbs.GetUserInput($"Please enter the {nameType} name.", $"Create {nameType}", suggestedName);
        }

        private async Task<IProp?> GenerateProp()
        {
			//TODO ask the user what type of Prop We are going to assume Arch for now.

            var dependencyResolver = this.GetDependencyResolver();

			// Get the Catel type factory
			ITypeFactory typeFactory = this.GetTypeFactory();

            // Retrieve the color scheme service
            IBaseColorSchemeService baseColorService = (IBaseColorSchemeService)dependencyResolver.Resolve(typeof(IBaseColorSchemeService));

            // Select the dark color scheme
            baseColorService.SetBaseColorScheme("Dark");

			// Use the type factory to create the prop wizard
			
			var wizard = PropWizardFactory.CreateInstance(PropType.Arch, typeFactory);
			
            var ws = dependencyResolver.Resolve<IWizardService>();
            if (ws != null && wizard != null)
            {
				bool? result = (await ws.ShowWizardAsync(wizard)).DialogResult;
                // Determine if the wizard was cancelled 
                if (result.HasValue && result.Value)
                {
					//User did not cancel
                    var page = (IPropWizardFinalPage)wizard.Pages.Single(page => page is IPropWizardFinalPage);
                    return page.GetProp();
                }
            }

            return null;
        }

		#endregion

		#region Event Handling

		private void OnModelsChanged()
        {
            ModelsChanged?.Invoke(this, EventArgs.Empty);
        }

		#endregion

		#region Dispose

		public void Dispose()
        {
            
        }

		#endregion
	}
}