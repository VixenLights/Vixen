using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using Catel.Data;
using Catel.IoC;
using Catel.MVVM;
using Catel.Services;
using Common.Controls;
using Common.Controls.NameGeneration;
using Common.WPFCommon.Services;
using GongSolutions.Wpf.DragDrop;
using GongSolutions.Wpf.DragDrop.Utilities;
using VixenModules.App.CustomPropEditor.Converters;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using Clipboard = System.Windows.Clipboard;
using DataFormats = System.Windows.Forms.DataFormats;
using DataObject = System.Windows.DataObject;
using DragDropEffects = System.Windows.DragDropEffects;
using IDataObject = System.Windows.IDataObject;
using IDropTarget = GongSolutions.Wpf.DragDrop.IDropTarget;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	public sealed class ElementTreeViewModel : ViewModelBase,  IDropTarget, IDragSource, IDisposable
	{
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		public event EventHandler ModelsChanged;
		
		private static readonly DataFormats.Format ClipboardFormatName = DataFormats.GetFormat(typeof(List<ElementModel>).FullName);

		public ElementTreeViewModel(Prop prop)
		{
			Prop = prop;
			ElementModelViewModel vm = new ElementModelViewModel(Prop.RootNode, null);
			RootNodesViewModels = new ObservableCollection<ElementModelViewModel>(new[] { vm });
			SelectedItems = new ObservableCollection<ElementModelViewModel>();
			SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
		}


		#region Prop model property

		/// <summary>
		/// Gets or sets the Prop value.
		/// </summary>
		[Model]
		public Prop Prop
		{
			get { return GetValue<Prop>(PropProperty); }
			private set { SetValue(PropProperty, value); }
		}

		/// <summary>
		/// Prop property data.
		/// </summary>
		public static readonly PropertyData PropProperty = RegisterProperty("Prop", typeof(Prop));

		#endregion

		#region RootNodes property

		/// <summary>
		/// Gets or sets the RootNodes value.
		/// </summary>
		[ViewModelToModel("Prop", "RootNode", ConverterType = typeof(RootNodeToCollectionMapping))]
		public ObservableCollection<ElementModel> RootNodes
		{
			get { return GetValue<ObservableCollection<ElementModel>>(RootNodesProperty); }
			set { SetValue(RootNodesProperty, value); }
		}

		/// <summary>
		/// RootNodes property data.
		/// </summary>
		public static readonly PropertyData RootNodesProperty = RegisterProperty("RootNodes", typeof(ObservableCollection<ElementModel>), null);

		#endregion

		#region RootNodesViewModels property

		/// <summary>
		/// Gets or sets the RootNodesViewModels value.
		/// </summary>
		public ObservableCollection<ElementModelViewModel> RootNodesViewModels
		{
			get { return GetValue<ObservableCollection<ElementModelViewModel>>(RootNodesViewModelsProperty); }
			set { SetValue(RootNodesViewModelsProperty, value); }
		}

		/// <summary>
		/// RootNodesViewModels property data.
		/// </summary>
		public static readonly PropertyData RootNodesViewModelsProperty = RegisterProperty("RootNodesViewModels", typeof(ObservableCollection<ElementModelViewModel>));

		#endregion

		#region SelectedItems property

		/// <summary>
		/// Gets or sets the SelectedItems value.
		/// </summary>
		public ObservableCollection<ElementModelViewModel> SelectedItems
		{
			get { return GetValue<ObservableCollection<ElementModelViewModel>>(SelectedItemsProperty); }
			set { SetValue(SelectedItemsProperty, value); }
		}

		/// <summary>
		/// SelectedItems property data.
		/// </summary>
		public static readonly PropertyData SelectedItemsProperty =
			RegisterProperty("SelectedItems", typeof(ObservableCollection<ElementModelViewModel>), null);


		private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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
		}


		#endregion

		#region SelectedItem property

		/// <summary>
		/// Gets or sets the SelectedItem value.
		/// </summary>
		public ElementModelViewModel SelectedItem
		{
			get { return GetValue<ElementModelViewModel>(SelectedItemProperty); }
			set { SetValue(SelectedItemProperty, value); }
		}

		/// <summary>
		/// SelectedItem property data.
		/// </summary>
		public static readonly PropertyData SelectedItemProperty = RegisterProperty("SelectedItem", typeof(ElementModelViewModel));

		#endregion

		#region Commands

		#region CreateGroup command

		private Command _createGroupCommand;

		/// <summary>
		/// Gets the CreateGroup command.
		/// </summary>
		public Command CreateGroupCommand
		{
			get { return _createGroupCommand ?? (_createGroupCommand = new Command(CreateGroup, CanCreateGroup)); }
		}

		/// <summary>
		/// Method to invoke when the CreateGroup command is executed.
		/// </summary>
		private void CreateGroup()
		{
			var result = RequestNewGroupName(String.Empty);
			if (result.Result == MessageResult.OK)
			{
				var elementsToGroup = SelectedItems.Select(x => x.ElementModel).Distinct().ToList();
				DeselectAll();
				PropModelServices.Instance().CreateGroupForElementModels(result.Response, elementsToGroup);
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

		#endregion

		#region MoveToGroup command

		private Command _moveToGroupCommand;

		/// <summary>
		/// Gets the MoveToGroup command.
		/// </summary>
		public Command MoveToGroupCommand
		{
			get { return _moveToGroupCommand ?? (_moveToGroupCommand = new Command(MoveToGroup, CanMoveToGroup)); }
		}

		/// <summary>
		/// Method to invoke when the MoveToGroup command is executed.
		/// </summary>
		private void MoveToGroup()
		{
			var result = RequestNewGroupName(String.Empty);
			if (result.Result == MessageResult.OK)
			{
				var pms = PropModelServices.Instance();

				//See if we are moving all items and can place the new group them under our parent group.
				ElementModel parentForGroup = null;
				var firstParentViewModel = SelectedItems.First().ParentViewModel as ElementModelViewModel;
				if(firstParentViewModel!= null && !firstParentViewModel.Children.Except(SelectedItems.Select(x => x.ElementModel)).Any())
				{
					parentForGroup = firstParentViewModel?.ElementModel;
				}

				var parentToJoin = pms.CreateNode(result.Response, parentForGroup);
				foreach (var elementModelViewModel in SelectedItems.ToList())
				{
					elementModelViewModel.IsSelected = false;
					ElementModel parentToLeave = (elementModelViewModel.ParentViewModel as ElementModelViewModel)?.ElementModel;
					if (parentToLeave != null)
					{
						pms.AddToParent(elementModelViewModel.ElementModel, parentToJoin);
						pms.RemoveFromParent(elementModelViewModel.ElementModel, parentToLeave);
					}
				}
				OnModelsChanged();

				DeselectAll();
				var newModel = ElementModelLookUpService.Instance.GetModels(parentToJoin.Id);
				if (newModel != null && newModel.Any())
				{
					newModel.First().IsExpanded = true;
					SelectModels(newModel);
				}
				
				
			}
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
			get { return _substitutionRenameCommand ?? (_substitutionRenameCommand = new Command(SubstitutionRename, CanSubstitutionRename)); }
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
			if (SelectedItems.Count <= 1)
				return false;

			List<string> oldNames = new List<string>(SelectedItems.Select(x => x.Name).ToArray());
			SubstitutionRenamer renamer = new SubstitutionRenamer(oldNames);
			if (renamer.ShowDialog() == DialogResult.OK)
			{
				for (int i = 0; i < SelectedItems.Count; i++)
				{
					if (i >= renamer.Names.Count)
					{
						Logging.Warn("Bulk renaming elements, and ran out of new names!");
						break;
					}


					SelectedItems[i].Name = PropModelServices.Instance().Uniquify(renamer.Names[i], 2, SelectedItems[i].ElementModel);
				}

				return true;
			}

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
			get { return _renameCommand ?? (_renameCommand = new Command(Rename)); }
		}

		/// <summary>
		/// Method to invoke when the Rename command is executed.
		/// </summary>
		private void Rename()
		{
			if (SelectedItems.Count == 1)
			{
				MessageBoxService mbs = new MessageBoxService();
				var result = mbs.GetUserInput("Please enter the new name.", "Rename", SelectedItem.Name);
				if (result.Result == MessageResult.OK)
				{
					SelectedItems.First().Name = PropModelServices.Instance().Uniquify(result.Response);
				}
			}
			else
			{
				PatternRenameSelectedItems();
			}
		}


		public bool PatternRenameSelectedItems()
		{
			if (SelectedItems.Count <= 1)
				return false;

			List<string> oldNames = new List<string>(SelectedItems.Select(x => x.ElementModel.Name).ToArray());
			NameGenerator renamer = new NameGenerator(oldNames);
			if (renamer.ShowDialog() == DialogResult.OK)
			{
				for (int i = 0; i < SelectedItems.Count; i++)
				{
					if (i >= renamer.Names.Count)
					{
						Logging.Warn("Bulk renaming elements, and ran out of new names!");
						break;
					}


					SelectedItems[i].Name = PropModelServices.Instance().Uniquify(renamer.Names[i], 2, SelectedItems[i].ElementModel);
				}

				return true;
			}

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
			get { return _createNodeCommand ?? (_createNodeCommand = new Command(CreateNode, CanCreateNode)); }
		}

		/// <summary>
		/// Method to invoke when the CreateNode command is executed.
		/// </summary>
		private void CreateNode()
		{
			var result = RequestNewGroupName(PropModelServices.Instance().Uniquify(SelectedItem.ElementModel.Name));

			if (result.Result == MessageResult.OK)
			{
				PropModelServices.Instance().CreateNode(result.Response, SelectedItem.ElementModel);
				//Ensure parent is expanded
				SelectedItem.IsExpanded = true;
			}
			
		}

		/// <summary>
		/// Method to check whether the CreateNode command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanCreateNode()
		{
			return SelectedItems.Count == 1 && SelectedItem.ElementModel.IsGroupNode && SelectedItem.Children.All(c => c.IsGroupNode);
		}

		#endregion

		#region Cut command

		private Command _cutCommand;

		/// <summary>
		/// Gets the Cut command.
		/// </summary>
		public Command CutCommand
		{
			get { return _cutCommand ?? (_cutCommand = new Command(Cut, CanCut)); }
		}

		/// <summary>
		/// Method to invoke when the Cut command is executed.
		/// </summary>
		private void Cut()
		{
			List<ElementModel> clipData = new List<ElementModel>();
			clipData.AddRange(SelectedItems.Select(x => x.ElementModel).ToList());

			IDataObject dataObject = new DataObject(ClipboardFormatName);
			dataObject.SetData(clipData);
			Clipboard.SetDataObject(dataObject, true);
			var itemsToCut = SelectedItems.ToList();
			DeselectAll();
			foreach (var elementModelViewModel in itemsToCut)
			{
				var parentToLeave = elementModelViewModel.ParentViewModel as ElementModelViewModel;
				if (parentToLeave != null)
				{
					PropModelServices.Instance().RemoveFromParent(elementModelViewModel.ElementModel, parentToLeave.ElementModel);
				}
			}

			OnModelsChanged();
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
			get { return _copyCommand ?? (_copyCommand = new Command(Copy, CanCopy)); }
		}

		/// <summary>
		/// Method to invoke when the Copy command is executed.
		/// </summary>
		private void Copy()
		{
			List<ElementModel> clipData = new List<ElementModel>();
			clipData.AddRange(SelectedItems.Select(x => x.ElementModel).ToList());

			IDataObject dataObject = new DataObject(ClipboardFormatName);
			dataObject.SetData(clipData);
			Clipboard.SetDataObject(dataObject, true);
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
			get { return _pasteCommand ?? (_pasteCommand = new Command(Paste, CanPaste)); }
		}

		/// <summary>
		/// Method to invoke when the Paste command is executed.
		/// </summary>
		private void Paste()
		{
			System.Windows.Forms.IDataObject dataObject = System.Windows.Forms.Clipboard.GetDataObject();

			if (dataObject != null && SelectedItems.Count == 1)
			{
				if (dataObject.GetDataPresent(ClipboardFormatName.Name))
				{
					var parent = SelectedItem;
					DeselectAll();
					var data = dataObject.GetData(ClipboardFormatName.Name) as List<ElementModel>;

					if (data != null)
					{
						foreach (var elementModel in data)
						{
							if(parent.ElementModel.Equals(elementModel) || parent.Children.Contains(elementModel)) continue;
							//if(elementModel.Parents.Contains(parent.ElementModel.Id)) continue; //Don't add another copy 
							PropModelServices.Instance().FindOrCreateElementModelTree(elementModel, parent.ElementModel);
						}
					}

					OnModelsChanged();
					SelectModels(new[] { parent });
				}
			}
		}

		/// <summary>
		/// Method to check whether the Paste command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanPaste()
		{
			System.Windows.Forms.IDataObject dataObject = System.Windows.Forms.Clipboard.GetDataObject();

			if (dataObject!=null && SelectedItems.Count == 1)
			{
				if (dataObject.GetDataPresent(ClipboardFormatName.Name))
				{
					var data = dataObject.GetData(ClipboardFormatName.Name) as List<ElementModel>;

					if (data != null)
					{
						if (data.All(x => x.IsLightNode && !SelectedItem.ElementModel.Children.Contains(x)) && SelectedItem.ElementModel.CanAddLightNodes)
						{
							//We can paste our contents
							return true;
						}
						if (data.All(x => x.IsGroupNode && !SelectedItem.ElementModel.Children.Contains(x)) && SelectedItem.ElementModel.CanAddGroupNodes)
						{
							//We can paste our contents
							return true;
						}
					}
				}
			}

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
			get { return _pasteAsNewCommand ?? (_pasteAsNewCommand = new Command(PasteAsNew, CanPasteAsNew)); }
		}

		/// <summary>
		/// Method to invoke when the PasteAsNew command is executed.
		/// </summary>
		private void PasteAsNew()
		{
			System.Windows.Forms.IDataObject dataObject = System.Windows.Forms.Clipboard.GetDataObject();

			if (dataObject != null && SelectedItems.Count == 1)
			{
				if (dataObject.GetDataPresent(ClipboardFormatName.Name))
				{
					var parent = SelectedItem;
					MessageBoxService mbs = new MessageBoxService();
					var result = mbs.GetUserInput("Please enter the new name.", "Paste As New", PropModelServices.Instance().Uniquify(parent.Name));
					if (result.Result == MessageResult.OK)
					{
						DeselectAll();
						var newElementModels = new List<ElementModelViewModel>();
						var data = dataObject.GetData(ClipboardFormatName.Name) as List<ElementModel>;

						if (data != null)
						{
							foreach (var elementModel in data)
							{
								var em = PropModelServices.Instance().CreateElementModelTree(elementModel, parent.ElementModel, result.Response);
								var evm = ElementModelLookUpService.Instance.GetModels(em.Id);
								if (evm != null)
								{
									newElementModels.AddRange(evm);
								}
							}
						}

						OnModelsChanged();
						SelectModels(newElementModels);
					}

					
				}
			}
		}

		/// <summary>
		/// Method to check whether the PasteAsNew command can be executed.
		/// </summary>
		/// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
		private bool CanPasteAsNew()
		{
			System.Windows.Forms.IDataObject dataObject = System.Windows.Forms.Clipboard.GetDataObject();

			if (dataObject != null && SelectedItems.Count == 1)
			{
				if (dataObject.GetDataPresent(ClipboardFormatName.Name))
				{
					var data = dataObject.GetData(ClipboardFormatName.Name) as List<ElementModel>;

					if (data != null)
					{
						if (data.All(x => x.IsLightNode && SelectedItem.ElementModel.CanAddLightNodes))
						{
							//We can paste our contents
							return true;
						}
						if (data.All(x => x.IsGroupNode && SelectedItem.ElementModel.CanAddGroupNodes))
						{
							//We can paste our contents
							return true;
						}
					}
				}
			}

			return false;
		}

		#endregion


		#endregion

		public bool IsElementsDirty
		{
			get
			{
				return ElementModelLookUpService.Instance.GetAllModels().Any(x => x.IsDirty);
			}
		}

		public void ClearIsDirty()
		{
			this.ClearIsDirtyOnAllChildren();
		}

		public void DeselectAll()
		{
			SelectedItems.ToList().ForEach(x => x.IsSelected = false);
			SelectedItems.Clear();
		}

		public void SelectModels(IEnumerable<ElementModelViewModel> elementModels)
		{
			foreach (var elementModelViewModel in elementModels)
			{
				elementModelViewModel.IsSelected = true;
				var parent = elementModelViewModel.ParentViewModel as ElementModelViewModel;
				if (parent != null)
				{
					parent.IsExpanded = true;
				}
			}
		}

		public void DeselectModels(IEnumerable<ElementModelViewModel> elementModels)
		{
			foreach (var elementModelViewModel in elementModels)
			{
				elementModelViewModel.IsSelected = false;
			}
		}

		private bool CanGroup()
		{
			var type = SelectedItems.FirstOrDefault()?.ElementModel.ElementType;
			return SelectedItems.Any() &&
			       SelectedItems.Select(x => x.ElementModel).All(x => x != Prop.RootNode && x.ElementType == type);
		}

		private MessageBoxResponse RequestNewGroupName(string suggestedName)
		{
			var dependencyResolver = this.GetDependencyResolver();
			var mbs = dependencyResolver.Resolve<IMessageBoxService>();
			return mbs.GetUserInput("Please enter the group name.", "Create Group", suggestedName);
		}

		private void OnModelsChanged()
		{
			ModelsChanged?.Invoke(this, EventArgs.Empty);
		}

		public void Dispose()
		{
		}

		
		#region Implementation of IDropTarget

	    public static bool CanAcceptData(IDropInfo dropInfo)
	    {
	        if (dropInfo?.DragInfo == null)
	        {
	            return false;
	        }

	        if (!dropInfo.IsSameDragDropContextAsSource)
	        {
	            return false;
	        }

			var isTreeViewItem = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter)
	                             && dropInfo.VisualTargetItem is TreeViewItem;
	        if (isTreeViewItem && dropInfo.VisualTargetItem == dropInfo.DragInfo.VisualSourceItem)
	        {
	            return false;
	        }

	        IList<ElementModelViewModel> elementModelViewModels = dropInfo.Data as IList<ElementModelViewModel>;
	        var evmTarget = dropInfo.TargetItem as ElementModelViewModel;
		    if (evmTarget == null)
		    {
			    return false;
		    }

		   // Console.Out.WriteLine($"Target {evmTarget.ElementModel.Name},IsGroupNode {evmTarget.ElementModel.IsGroupNode} Position {dropInfo.InsertPosition}");

			if (elementModelViewModels != null)
	        {
	            var isGroups = elementModelViewModels.Any(x => x.ElementModel.IsGroupNode);
	            var isLeafs = elementModelViewModels.Any(x => x.IsLeaf);
		        var isLightNodes = elementModelViewModels.Any(x => x.ElementModel.IsLightNode);

				if (isGroups && isLightNodes)
	            {
	                return false;
	            }

		        if (dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.BeforeTargetItem))
		        {
			        if (evmTarget.ElementModel.IsRootNode)
			        {
				        //Can't insert above the root node.
						return false;
			        }
			        if (evmTarget.ElementModel.IsGroupNode && elementModelViewModels.Any(x => x.IsLightNode))
			        {
				        //Can't add lights to group of groups
				        return false;
			        }

					
			        if (evmTarget.ParentViewModel is ElementModelViewModel parent)
			        {
				        if (elementModelViewModels.Any(x => x.ParentViewModel != parent))
				        {
					        if (parent.Children.Intersect(elementModelViewModels.Select(x => x.ElementModel)).Any())
					        {
						        //already part of this group.
						        return false;
					        }
						}
					}

				}

		        if (dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.AfterTargetItem))
		        {
			        if (elementModelViewModels.Any(x => x.IsLightNode) && evmTarget.ElementModel.IsGroupNode && !evmTarget.ElementModel.CanAddLightNodes)
			        {
				        //Can't add lights to group of groups
				        return false;
			        }

					if (evmTarget.ParentViewModel is ElementModelViewModel parent)
					{
						if (elementModelViewModels.Any(x => x.ParentViewModel != parent))
						{
							if (parent.Children.Intersect(elementModelViewModels.Select(x => x.ElementModel)).Any())
							{
								//already part of this group.
								return false;
							}
						}
					}
				}

				if (isTreeViewItem && dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter))
	            {
	                
	                if (!evmTarget.ElementModel.IsGroupNode)
	                {
	                    return false;
	                }

		            if (evmTarget.ElementModel.IsRootNode && elementModelViewModels.Any(x => x.IsLightNode))
		            {
						//Can't add lights to the root node
			            return false;
		            }

	                if (isGroups && !evmTarget.ElementModel.CanAddGroupNodes)
	                {
	                    return false;
	                }

	                if (isLeafs && !evmTarget.ElementModel.CanAddLeafNodes)
	                {
	                    return false;
	                }

		            if (elementModelViewModels.Any(x => x.ElementModel.Parents.Contains(evmTarget.ElementModel.Id)))
		            {
						//Can't add to the same parent.
			            return false;
		            }
                }
            }

	        if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
	        {
	            var targetList = dropInfo.TargetCollection.TryGetList();
		        return targetList != null;
	        }

	        if (dropInfo.TargetCollection == null)
	        {
	            return false;
	        }
	       
	        if (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data))
	        {
	            var isChildOf = IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
		        return !isChildOf;
	        }
	        
	        return false;
	    }


	    /// <summary>
	    /// Determines whether the data of the drag drop action should be copied otherwise moved.
	    /// </summary>
	    /// <param name="dropInfo">The DropInfo with a valid DragInfo.</param>
	    public static bool ShouldCopyData(IDropInfo dropInfo)
	    {
	        // default should always the move action/effect
	        if (dropInfo?.DragInfo == null)
	        {
	            return false;
	        }
	        var copyData = ((dropInfo.DragInfo.DragDropCopyKeyState != default(DragDropKeyStates)) && dropInfo.KeyStates.HasFlag(dropInfo.DragInfo.DragDropCopyKeyState))
	                       || dropInfo.DragInfo.DragDropCopyKeyState.HasFlag(DragDropKeyStates.LeftMouseButton);
	        copyData = copyData
	                   && !(dropInfo.DragInfo.SourceItem is HeaderedContentControl)
	                   && !(dropInfo.DragInfo.SourceItem is HeaderedItemsControl)
	                   && !(dropInfo.DragInfo.SourceItem is ListBoxItem);
	        return copyData;
	    }

        /// <inheritdoc />
        public void DragOver(IDropInfo dropInfo)
		{
		    if (CanAcceptData(dropInfo))
		    {
		        dropInfo.Effects = ShouldCopyData(dropInfo) ? DragDropEffects.Copy : DragDropEffects.Move;

			    if (dropInfo.InsertPosition == RelativeInsertPosition.None)
			    {
				    dropInfo.DropTargetAdorner = null;
				    return;
			    }

		        var isTreeViewItem = dropInfo.InsertPosition.HasFlag(RelativeInsertPosition.TargetItemCenter) && dropInfo.VisualTargetItem is TreeViewItem;
		        dropInfo.DropTargetAdorner = isTreeViewItem ? DropTargetAdorners.Highlight : DropTargetAdorners.Insert;
			}
		    else
		    {
			    dropInfo.DropTargetAdorner = null;
		    }
        }

		/// <inheritdoc />
		public void Drop(IDropInfo dropInfo)
		{
			var models = dropInfo.Data as IList<ElementModelViewModel>;
			if (models != null)
			{
				var pms = PropModelServices.Instance();
				var targetModel = dropInfo.TargetItem as ElementModelViewModel;
				var targetModelParent = targetModel?.ParentViewModel as ElementModelViewModel;

				SelectedItems.Clear();

				if (targetModel != null)
				{
					bool reverse = dropInfo.KeyStates == DragDropKeyStates.ControlKey;
					var elementIndex = 0;
					foreach (var elementModelViewModel in reverse?models.Reverse():models)
					{
					    elementModelViewModel.IsSelected = false;
                        if (dropInfo.Effects == DragDropEffects.Move)
						{
						   //Get our parent 
						    var sourceModelParent = elementModelViewModel.ParentViewModel as ElementModelViewModel;

                            if (dropInfo.InsertPosition == RelativeInsertPosition.BeforeTargetItem)
						    {
						        //We are inserting into a range.
                                //Ensure the parent is a group node.
                                if (sourceModelParent != null && sourceModelParent.ElementModel.IsGroupNode && targetModelParent != null)
						        {
						            if (sourceModelParent == targetModelParent)
						            {
                                        //Our parent is the same so we can just move it within the parent
                                        pms.MoveWithinParent(sourceModelParent.ElementModel, elementModelViewModel.ElementModel, dropInfo.InsertIndex+elementIndex);
						                elementModelViewModel.IsSelected = true;
						            }
						            else
						            {
                                        //We are moving to a new parent
						                pms.InsertToParent(elementModelViewModel.ElementModel, targetModelParent.ElementModel, dropInfo.InsertIndex+elementIndex);
                                        pms.RemoveFromParent(elementModelViewModel.ElementModel, sourceModelParent.ElementModel);
										SelectModelWithParent(elementModelViewModel, targetModelParent);
						            }
                                }
                            }
						    else if(dropInfo.InsertPosition == RelativeInsertPosition.AfterTargetItem)
						    {
                                //We are inserting into a range.
						        //Ensure the parent is a group node.
						        if (sourceModelParent != null && sourceModelParent.ElementModel.IsGroupNode && targetModelParent != null)
						        {
                                    if (sourceModelParent == targetModelParent)
						            {
						                //We can just move it
						                pms.MoveWithinParent(sourceModelParent.ElementModel, elementModelViewModel.ElementModel, dropInfo.InsertIndex-1);
						                elementModelViewModel.IsSelected = true;
						            }
						            else
						            {
						                //We are moving to a new parent
						                pms.InsertToParent(elementModelViewModel.ElementModel, targetModelParent.ElementModel, dropInfo.InsertIndex+elementIndex);
						                pms.RemoveFromParent(elementModelViewModel.ElementModel, sourceModelParent.ElementModel);
										SelectModelWithParent(elementModelViewModel, targetModelParent);
                                    }
                                }
                            }
						    else
						    {
                                //We are on the center and adding to a group hopefully
						        //Ensure the target is a group node.
						        if (targetModel.ElementModel.IsGroupNode && sourceModelParent != null &&
						            targetModel.ElementModel != sourceModelParent.ElementModel) //We are not adding to our own parent.
						        {
						            pms.AddToParent(elementModelViewModel.ElementModel, targetModel.ElementModel);
                                    pms.RemoveFromParent(elementModelViewModel.ElementModel, sourceModelParent.ElementModel);
						            SelectModelWithParent(elementModelViewModel, targetModel);
                                }
						        else
						        {
							        Logging.Warn($"Attempt to add item {elementModelViewModel.ElementModel.Name} to a non group node.");
						        }
                            }
						}

                        elementIndex++;
					}
				}
			}

		}

	    private static void SelectModelWithParent(ElementModelViewModel elementModelViewModel, ElementModelViewModel targetModelParent)
	    {
	        var newModel = ElementModelLookUpService.Instance.GetModels(elementModelViewModel.ElementModel.Id)?
	            .FirstOrDefault(e => e.ParentViewModel == targetModelParent);
	        if (newModel != null)
	        {
	            newModel.IsSelected = true;
		        //ensure parent is expanded
				targetModelParent.IsExpanded = true;
	        }
	    }

	    private static bool IsChildOf(UIElement targetItem, UIElement sourceItem)
	    {
	        var parent = ItemsControl.ItemsControlFromItemContainer(targetItem);

	        while (parent != null)
	        {
	            if (parent == sourceItem)
	            {
	                return true;
	            }

	            parent = ItemsControl.ItemsControlFromItemContainer(parent);
	        }

	        return false;
	    }

	    private static bool TestCompatibleTypes(IEnumerable target, object data)
	    {
	        TypeFilter filter = (t, o) => { return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>)); };

	        var enumerableInterfaces = target.GetType().FindInterfaces(filter, null);
	        var enumerableTypes = from i in enumerableInterfaces
	            select i.GetGenericArguments().Single();

	        if (enumerableTypes.Any())
	        {
	            var dataType = TypeUtilities.GetCommonBaseClass(ExtractData(data));
	            return enumerableTypes.Any(t => t.IsAssignableFrom(dataType));
	        }
	        else
	        {
	            return target is IList;
	        }
	    }

	    public static IEnumerable ExtractData(object data)
	    {
	        if (data is IEnumerable && !(data is string))
	        {
	            return (IEnumerable)data;
	        }
	        else
	        {
	            return Enumerable.Repeat(data, 1);
	        }
	    }

        #endregion

        #region Implementation of IDragSource

        /// <inheritdoc />
        public void StartDrag(IDragInfo dragInfo)
		{

            //In our case, the Treeview does not support multiple items, so the drag behavior can't figure it out
            //So we will take care of it ourselves.
		    var itemCount = dragInfo.SourceItems.Cast<object>().Count();

		    if (itemCount == 1 && SelectedItems.Count == 1)
		    {
		        dragInfo.Data = TypeUtilities.CreateDynamicallyTypedList(new[] { dragInfo.SourceItems.Cast<object>().First() });
			}
		    else if(itemCount == 1 && SelectedItems.Count == 0)
		    {
			    var dragItem = dragInfo.SourceItems.Cast<object>().First() as ElementModelViewModel;
			    if (dragItem != null)
			    {
					dragInfo.Data = TypeUtilities.CreateDynamicallyTypedList(new[] { dragItem });
					dragItem.IsSelected = true;
				}
			    
		    }
		    else if (itemCount > 1)
		    {
		        dragInfo.Data = TypeUtilities.CreateDynamicallyTypedList(dragInfo.SourceItems);
            }
            else if (SelectedItems.Count > 1 && itemCount == 1)
		    {
		        if (SelectedItems.Contains(dragInfo.SourceItems.Cast<object>().First()))
		        {
		            dragInfo.Data = TypeUtilities.CreateDynamicallyTypedList(SelectedItems);
				}
		        else
		        {
			        var dragItem = dragInfo.SourceItems.Cast<object>().First() as ElementModelViewModel;
			        if (dragItem != null)
			        {
				        dragInfo.Data = TypeUtilities.CreateDynamicallyTypedList(new[] { dragItem });
				        foreach (var elementModelViewModel in SelectedItems.ToList())
				        {
					        elementModelViewModel.IsSelected = false;
				        }

				        dragItem.IsSelected = true;
			        }
					
					
				}
		    }

		    dragInfo.Effects = (dragInfo.Data != null) ? DragDropEffects.Copy | DragDropEffects.Move : DragDropEffects.None;
        }

		/// <inheritdoc />
		public bool CanStartDrag(IDragInfo dragInfo)
		{
		    return true;
		}

		/// <inheritdoc />
		public void Dropped(IDropInfo dropInfo)
		{
			
		}

		/// <inheritdoc />
		public void DragDropOperationFinished(DragDropEffects operationResult, IDragInfo dragInfo)
		{
			
		}

		/// <inheritdoc />
		public void DragCancelled()
		{
			
		}

		/// <inheritdoc />
		public bool TryCatchOccurredException(Exception exception)
		{
		    return false;
        }

		#endregion
	}
}
