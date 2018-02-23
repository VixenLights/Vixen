using System.Collections.ObjectModel;
using System.Linq;
using Catel.Data;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Converters;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;

namespace VixenModules.App.CustomPropEditor.ViewModel
{
    public class ElementTreeViewModel:ViewModelBase
    {
        public ElementTreeViewModel(Prop prop)
        {
            Prop = prop;
            SelectedItems = new ObservableCollection<ElementModel>();
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

        #region SelectedItems property

        /// <summary>
        /// Gets or sets the SelectedItems value.
        /// </summary>
        public ObservableCollection<ElementModel> SelectedItems
        {
            get { return GetValue<ObservableCollection<ElementModel>>(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }

        /// <summary>
        /// SelectedItems property data.
        /// </summary>
        public static readonly PropertyData SelectedItemsProperty =
            RegisterProperty("SelectedItems", typeof(ObservableCollection<ElementModel>), null);


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
        public ElementModel SelectedItem
        {
            get { return GetValue<ElementModel>(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// SelectedItem property data.
        /// </summary>
        public static readonly PropertyData SelectedItemProperty = RegisterProperty("SelectedItem", typeof(ElementModel));

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
            PropModelServices.Instance().CreateGroupForElementModels(name, SelectedItems);
        }

        /// <summary>
        /// Method to check whether the CreateGroup command can be executed.
        /// </summary>
        /// <returns><c>true</c> if the command can be executed; otherwise <c>false</c></returns>
        private bool CanCreateGroup()
        {
            return SelectedItems.Any();
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


    }
}
