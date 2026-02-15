using Catel.Collections;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Common.WPFCommon.Services;
using GongSolutions.Wpf.DragDrop;
using Orc.Theming;
using Orc.Wizard;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using Vixen.Extensions;
using Vixen.Sys;
using Vixen.Sys.Managers;
using Vixen.Sys.Props;
using VixenApplication.SetupDisplay.Wizards.Factory;
using VixenApplication.SetupDisplay.Wizards.Pages;
using VixenApplication.SetupDisplay.Wizards.PropFactories;
using VixenApplication.SetupDisplay.Wizards.Wizard;
using VixenModules.Editor.PropWizard;
using DragDropEffects = System.Windows.DragDropEffects;
using IDropTarget = GongSolutions.Wpf.DragDrop.IDropTarget;

namespace VixenApplication.SetupDisplay.ViewModels
{
	public class PropTypeMenuItem
	{
		public PropType Id { get; set; } 
		public required string DisplayName { get; set; }
	}

	public class PropNodeTreeViewModel : ViewModelBase, IDropTarget, IDragSource
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		public event EventHandler ModelsChanged;

		public ObservableCollection<PropTypeMenuItem> AvailableProps { get; }
			= new ObservableCollection<PropTypeMenuItem>();

		public PropNodeTreeViewModel()
		{
			PropManager = VixenSystem.Props;
			PropNodeViewModel vm = new(PropManager.RootNode, null);
			RootNodeViewModel = [vm];

			PropComponentNodeViewModel pcvm = new (VixenSystem.PropComponents.RootNode, null);
			PropComponentNodeViewModels = [pcvm];

			PropNodes = new();
			Catel.Collections.CollectionExtensions.AddRange(PropNodes, RootNodeViewModel.SelectMany(x => x.GetPropEnumerator()));

			PropManager.PropCollectionChanged += PropManager_PropCollectionChanged;
			SelectedItems = new();
			SelectedItems.CollectionChanged += SelectedItemsCollectionChanged;

			foreach (PropType menuItem in Enum.GetValues(typeof(PropType)))
			{
				AvailableProps.Add(new PropTypeMenuItem { Id = menuItem, DisplayName = menuItem.GetEnumDescription() });
			}
		}

		private void PropManager_PropCollectionChanged(object? sender, EventArgs e)
		{
			PropNodes.Clear();
			Catel.Collections.CollectionExtensions.AddRange(PropNodes, RootNodeViewModel.SelectMany(x => x.GetPropEnumerator()));
		}

		#region PropManager model property

		/// <summary>
		/// Gets or sets the PropManager value.
		/// </summary>
		public PropManager PropManager
		{
			get { return GetValue<PropManager>(PropManagerProperty); }
			private init { SetValue(PropManagerProperty, value); }
		}

		/// <summary>
		/// Prop property data.
		/// </summary>
		public static readonly IPropertyData PropManagerProperty = RegisterProperty<PropManager>(nameof(PropManager));

		#endregion

		#region RootNodeViewModels property

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

		#region PropComponentNodeViewModels

		#region PropComponentNodeViewModels property

		/// <summary>
		/// Gets or sets the PropComponentNodeViewModels value.
		/// </summary>
		public ObservableCollection<PropComponentNodeViewModel> PropComponentNodeViewModels
		{
			get { return GetValue<ObservableCollection<PropComponentNodeViewModel>>(PropComponentNodeViewModelsProperty); }
			set { SetValue(PropComponentNodeViewModelsProperty, value); }
		}

		/// <summary>
		/// PropComponentNodeViewModels property data.
		/// </summary>
		public static readonly IPropertyData PropComponentNodeViewModelsProperty = RegisterProperty<ObservableCollection<PropComponentNodeViewModel>>(nameof(PropComponentNodeViewModels));

		#endregion

		#endregion

		#region PropNodes property

		/// <summary>
		/// Collection of the actual nodes that contain Props.
		/// </summary>
		public ObservableCollection<PropNodeViewModel> PropNodes
		{
			get { return GetValue<ObservableCollection<PropNodeViewModel>>(PropNodesProperty); }
			private init { SetValue(PropNodesProperty, value); }
		}

		/// <summary>
		/// PropNodes property data.
		/// </summary>
		public static readonly IPropertyData PropNodesProperty = RegisterProperty<ObservableCollection<PropNodeViewModel>>(nameof(PropNodes));

		#endregion

		public bool IsTopNode
		{
			get => GetValue<bool>(IsTopNodeProperty);
			set => SetValue(IsTopNodeProperty, value);
		}

		public static readonly IPropertyData IsTopNodeProperty =
			RegisterProperty<bool>(nameof(IsTopNode), true);

		public bool IsSubNode
		{
			get => GetValue<bool>(IsSubNodeProperty);
			set => SetValue(IsSubNodeProperty, value);
		}

		public static readonly IPropertyData IsSubNodeProperty =
			RegisterProperty<bool>(nameof(IsSubNode), false);


		private void OnIsTopNodeChanged()
		{
			RaisePropertyChanged(nameof(IsSubNode));
		}
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


		private void SelectedItemsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			if (SelectedItems.Count == 1)
			{
				SelectedItem = SelectedItems.First();
			}
			else
			{
				SelectedItem = null;
			}

			CopyCommand.RaiseCanExecuteChanged();
			CutCommand.RaiseCanExecuteChanged();
			PasteCommand.RaiseCanExecuteChanged();
			CreateGroupCommand.RaiseCanExecuteChanged();
			MoveToGroupCommand.RaiseCanExecuteChanged();
			CreateNodeCommand.RaiseCanExecuteChanged();
			CreatePropCommand.RaiseCanExecuteChanged();
			ChangePropCommand.RaiseCanExecuteChanged();
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

		#region Selection

		public void DeselectAll()
		{
			SelectedItems.ToList().ForEach(x => x.IsSelected = false);
			SelectedItems.Clear();
		}

		/// <summary>
		/// Selects the models.
		/// </summary>
		/// <param name="propNodeModels"></param>
		public void SelectModels(IEnumerable<PropNodeViewModel> propNodeModels)
		{
			foreach (var propNodeViewModel in propNodeModels)
			{
				if (!propNodeViewModel.IsSelected)
				{
					propNodeViewModel.IsSelected = true;
				}
				if (!SelectedItems.Contains(propNodeViewModel))
				{
					SelectedItems.Add(propNodeViewModel);
				}
				//ExpandTree(propNodeViewModel);
			}
		}

		public void DeselectModels(IEnumerable<PropNodeViewModel> propNodeModels)
		{
			foreach (var propNodeViewModel in propNodeModels)
			{
				if (propNodeViewModel.IsSelected)
				{
					propNodeViewModel.IsSelected = false;
					SelectedItems.Remove(propNodeViewModel);
				}
			}
		}

		public void ExpandSelectedItems()
		{
			foreach (var propNodeViewModel in SelectedItems)
			{
				ExpandTree(propNodeViewModel);
			}
		}

		private void ExpandTree(PropNodeViewModel propNodeViewModel)
		{
			if (propNodeViewModel.ParentViewModel is PropNodeViewModel parent)
			{
				if (!parent.IsExpanded)
				{
					parent.IsExpanded = true;
					if (parent.ParentViewModel != null)
					{
						ExpandTree(parent);
					}
				}
			}
		}

		#endregion

		#region Collapse

		public void CollapseAll()
		{
			foreach (var propNodeViewModel in RootNodeViewModel)
			{
				if (propNodeViewModel.IsExpanded)
				{
					propNodeViewModel.IsExpanded = false;
				}
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

			//	var parentToJoin = pms.CreatePropNode(result.Response, parentForGroup);
			//	foreach (var elementModelViewModel in SelectedItems.ToList())
			//	{
			//		elementModelViewModel.IsSelected = false;
			//		ElementModel parentToLeave = (elementModelViewModel.ParentViewModel as ElementModelViewModel)?.ElementModel;
			//		if (parentToLeave != null)
			//		{
			//			pms.AddToParent(elementModelViewModel.ElementModel, parentToJoin);
			//			pms.DeleteFromParent(elementModelViewModel.ElementModel, parentToLeave);
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

		#region CreatePropNode command

		private Command _createNodeCommand;

		/// <summary>
		/// Gets the CreatePropNode command.
		/// </summary>
		public Command CreateNodeCommand
		{
			get { return _createNodeCommand ??= new(CreateNode, CanCreateNode); }
		}

		/// <summary>
		/// Method to invoke when the CreatePropNode command is executed.
		/// </summary>
		private void CreateNode()
		{
			var result = RequestNewName(string.Empty);

			if (result.Result == MessageResult.OK)
			{
				PropManager.CreatePropNode(result.Response, SelectedItem != null ? SelectedItem.PropNode : PropManager.RootNode);
				//Ensure parent is expanded
				if (SelectedItem != null)
				{
					SelectedItem.IsExpanded = true;
				}
			}

		}

		/// <summary>
		/// Method to check whether the CreatePropNode command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanCreateNode()
		{
			return true;
		}

		#endregion

		#region CreateProp command

		private TaskCommand<PropType> _createPropCommand;

		/// <summary>
		/// Gets the CreateProp command.
		/// </summary>
		public TaskCommand<PropType> CreatePropCommand
		{
			get { return _createPropCommand ??= new(CreateProp, CanCreateProp); }
		}

		/// <summary>
		/// Method to invoke when the CreatePropNode command is executed.
		/// </summary>
		private async Task CreateProp(PropType result)
		{
			IPropGroup propGroup = await GeneratePropNodes(result);

			if (propGroup != null)
			{
				// Determine the parent of the group or props
				PropNode pNodeParent;
				if (SelectedItem != null)
				{
					pNodeParent = SelectedItem.PropNode;
				}
				else
				{
					pNodeParent = PropManager.RootNode;
				}

				// If the props are to be grouped then...
				PropNode groupNode;
				if (propGroup.CreateGroup)
				{
					// Create the group prop node
					groupNode = new(propGroup.GroupName);
						
					// Add the group node to the tree
					PropManager.AddPropNode(groupNode, pNodeParent);

					// Make the group node the parent
					pNodeParent = groupNode;
				}

				// Loop over the props
				foreach (IProp prop in propGroup.Props)
				{
					if (prop != null)
					{
						// Add the prop to the tree																					
						PropManager.AddProp(prop, pNodeParent);

						//Ensure parent is expanded
						if (SelectedItem != null)
						{
							SelectedItem.IsExpanded = true;
						}
					}
				}
			}
		}

		/// <summary>
		/// Method to check whether the CreatePropNode command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanCreateProp(PropType propType)
		{
			if (SelectedItems.Count == 1 && SelectedItem is { IsGroupNode: true })
			{
				return true;
			}

			return false;
		}

		#endregion





		#region ChangeProp command

		private TaskCommand<PropType> _changePropCommand;

		public TaskCommand<PropType> ChangePropCommand
		{
			get { return _changePropCommand ??= new(ChangeProp, CanChangeProp); }
		}

		private async Task ChangeProp(PropType result)
		{
			IPropGroup propGroup = await EditPropNodes(SelectedItems[0].PropNode.Prop);
			RaisePropertyChanged(nameof(SelectedItem));
		}

		private bool CanChangeProp(PropType propType)
		{
			if (SelectedItems.Count == 1 && SelectedItem is { IsGroupNode: true })
			{
				return false;
			}

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
			//		PropModelServices.Instance().DeleteFromParent(elementModelViewModel.ElementModel, parentToLeave.ElementModel);
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

		private async Task<IPropGroup> GeneratePropNodes(PropType propType)
		{			
			var dependencyResolver = this.GetDependencyResolver();

			// Get the Catel type factory
			ITypeFactory typeFactory = this.GetTypeFactory();

			IPropFactory newPropFactory = PropFactory.CreateInstance(propType);
			(IProp newProp, IPropGroup propGroup) = newPropFactory.CreateBaseProp();

			// Retrieve the color scheme service
			IBaseColorSchemeService baseColorService = (IBaseColorSchemeService)dependencyResolver.Resolve(typeof(IBaseColorSchemeService));

			// Select the dark color scheme
			baseColorService.SetBaseColorScheme("Dark");

			// Use the type factory to create the prop wizard
			(IPropWizard Wizard, IPropFactory Factory) propWizard = PropWizardFactory.CreateInstance(propType, typeFactory);


			// Configure the wizard window to show up in the Windows task bar
			propWizard.Wizard.ShowInTaskbarWrapper = true;

			// Enable the help button
			propWizard.Wizard.ShowHelpWrapper = true;

			// Configure the wizard to allow the user to jump between already visited pages
			propWizard.Wizard.AllowQuickNavigationWrapper = true;

			// Allow Catel to help determine when it is safe to transition to the next wizard page
			propWizard.Wizard.HandleNavigationStatesWrapper = true;

			// Configure the wizard to NOT cache views
			propWizard.Wizard.CacheViewsWrapper = false;

			// Configure the wizard with a navigation controller														
			propWizard.Wizard.NavigationControllerWrapper = typeFactory.CreateInstanceWithParametersAndAutoCompletion<PropWizardNavigationController>(propWizard.Wizard);

			newPropFactory.LoadWizard(newProp, propWizard.Wizard);

			var ws = dependencyResolver.Resolve<IWizardService>();
			if (ws != null && propWizard.Wizard != null)
			{
				bool? result = (await ws.ShowWizardAsync(propWizard.Wizard)).DialogResult;
				// Determine if the wizard was cancelled 
				if (result.HasValue && result.Value)
				{
					// Have the prop factory create the props from the wizard data
					newPropFactory.UpdateProp(newProp, propWizard.Wizard);

					// User did not cancel					
					return propGroup;  
				}
			}

			return null;
		}

		private async Task<IPropGroup> EditPropNodes(IProp newProp)
		{
			var dependencyResolver = this.GetDependencyResolver();

			// Get the Catel type factory
			ITypeFactory typeFactory = this.GetTypeFactory();

			var propType = newProp.PropType;
			IPropFactory newPropFactory = PropFactory.CreateInstance(propType);
			IPropGroup propGroup = newPropFactory.EditExistingProp(newProp);

			// Retrieve the color scheme service
			IBaseColorSchemeService baseColorService = (IBaseColorSchemeService)dependencyResolver.Resolve(typeof(IBaseColorSchemeService));

			// Select the dark color scheme
			baseColorService.SetBaseColorScheme("Dark");

			// Use the type factory to create the prop wizard
			(IPropWizard Wizard, IPropFactory Factory) propWizard = PropWizardFactory.CreateInstance(propType, typeFactory);


			// Configure the wizard window to show up in the Windows task bar
			propWizard.Wizard.ShowInTaskbarWrapper = true;

			// Enable the help button
			propWizard.Wizard.ShowHelpWrapper = true;

			// Configure the wizard to allow the user to jump between already visited pages
			propWizard.Wizard.AllowQuickNavigationWrapper = true;

			// Allow Catel to help determine when it is safe to transition to the next wizard page
			propWizard.Wizard.HandleNavigationStatesWrapper = true;

			// Configure the wizard to NOT cache views
			propWizard.Wizard.CacheViewsWrapper = false;

			// Configure the wizard with a navigation controller														
			propWizard.Wizard.NavigationControllerWrapper = typeFactory.CreateInstanceWithParametersAndAutoCompletion<PropWizardNavigationController>(propWizard.Wizard);

			newPropFactory.LoadWizard(newProp, propWizard.Wizard);

			var ws = dependencyResolver.Resolve<IWizardService>();
			if (ws != null && propWizard.Wizard != null)
			{
				bool? result = (await ws.ShowWizardAsync(propWizard.Wizard)).DialogResult;
				// Determine if the wizard was cancelled 
				if (result.HasValue && result.Value)
				{
					// Have the prop factory create the props from the wizard data
					newPropFactory.UpdateProp(newProp, propWizard.Wizard);

					// User did not cancel					
					return propGroup;
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

	}
}