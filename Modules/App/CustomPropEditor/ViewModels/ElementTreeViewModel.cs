using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Catel.Data;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Converters;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
    public sealed class ElementTreeViewModel:ViewModelBase, IDisposable
    {
        public ElementTreeViewModel(Prop prop)
        {
            Prop = prop;
			ElementModelViewModel vm = new ElementModelViewModel(Prop.RootNode, null);
			RootNodesViewModels = new ObservableCollection<ElementModelViewModel>(new []{vm});
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
            MessageBoxService mbs = new MessageBoxService();

            var name = string.Empty;

            while (string.IsNullOrEmpty(name))
            {
                name = mbs.GetUserInput("Please enter the group name.", "Create Group");
            }
           
            name = PropModelServices.Instance().Uniquify(name); 
            PropModelServices.Instance().CreateGroupForElementModels(name, SelectedItems.Select(x => x.ElementModel));
        }

        /// <summary>
        /// Method to check whether the CreateGroup command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool CanCreateGroup()
        {
            return SelectedItems.Any() && SelectedItems.Select(x => x.ElementModel).All(x => x != Prop.RootNode);
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
                var newName = mbs.GetUserInput("Please enter the new name.", "Rename");

                SelectedItems.First().Name = newName;
            }
        }

        #endregion

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

		public void Dispose()
	    {
	    }
    }
}
