using System.Collections.ObjectModel;
using System.Windows;
using Catel.Data;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
    public class ElementModelViewModel:ViewModelBase
    {
        public ElementModelViewModel(ElementModel model)
        {
            ElementModel = model;
            Test = "Some string";
        }

        #region ElementModel model property

        /// <summary>
        /// Gets or sets the ElementModel value.
        /// </summary>
        [Model]
        public ElementModel ElementModel
        {
            get { return GetValue<ElementModel>(ElementModelProperty); }
            private set { SetValue(ElementModelProperty, value); }
        }

        /// <summary>
        /// ElementModel property data.
        /// </summary>
        public static readonly PropertyData ElementModelProperty = RegisterProperty("ElementModel", typeof(ElementModel));

        #endregion

        #region Name property

        /// <summary>
        /// Gets or sets the Name value.
        /// </summary>
        [ViewModelToModel("ElementModel")]
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

        #region Order property

        /// <summary>
        /// Gets or sets the Order value.
        /// </summary>
        [ViewModelToModel("ElementModel")]
        public int Order
        {
            get { return GetValue<int>(OrderProperty); }
            set { SetValue(OrderProperty, value); }
        }

        /// <summary>
        /// Order property data.
        /// </summary>
        public static readonly PropertyData OrderProperty = RegisterProperty("Order", typeof(int), null);

        #endregion


        #region Parents property

        /// <summary>
        /// Gets or sets the Parents value.
        /// </summary>
        [ViewModelToModel("ElementModel")]
        public ObservableCollection<ElementModel> Parents
        {
            get { return GetValue<ObservableCollection<ElementModel>>(ParentsProperty); }
            set { SetValue(ParentsProperty, value); }
        }

        /// <summary>
        /// Parents property data.
        /// </summary>
        public static readonly PropertyData ParentsProperty = RegisterProperty("Parents", typeof(ObservableCollection<ElementModel>), null);

        #endregion

        #region Children property

        /// <summary>
        /// Gets or sets the Children value.
        /// </summary>
        [ViewModelToModel("ElementModel")]
        public ObservableCollection<ElementModel> Children
        {
            get { return GetValue<ObservableCollection<ElementModel>>(ChildrenProperty); }
            set { SetValue(ChildrenProperty, value); }
        }

        /// <summary>
        /// Children property data.
        /// </summary>
        public static readonly PropertyData ChildrenProperty = RegisterProperty("Children", typeof(ObservableCollection<ElementModel>), null);

        #endregion

        #region Test property

        /// <summary>
        /// Gets or sets the Test value.
        /// </summary>
        public string Test
        {
            get { return GetValue<string>(TestProperty); }
            set { SetValue(TestProperty, value); }
        }

        /// <summary>
        /// Test property data.
        /// </summary>
        public static readonly PropertyData TestProperty = RegisterProperty("Test", typeof(string));

        #endregion



        #region IsSelected property

        /// <summary>
        /// Gets or sets the IsSelected value.
        /// </summary>
        public bool IsSelected
        {
            get { return GetValue<bool>(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        /// <summary>
        /// IsSelected property data.
        /// </summary>
        public static readonly PropertyData IsSelectedProperty = RegisterProperty("IsSelected", typeof(bool));

        #endregion

        #region IsLeaf property

        /// <summary>
        /// Gets or sets the IsLeaf value.
        /// </summary>
        [ViewModelToModel("ElementModel")]
        public bool IsLeaf
        {
            get { return GetValue<bool>(IsLeafProperty); }
            set { SetValue(IsLeafProperty, value); }
        }

        /// <summary>
        /// IsLeaf property data.
        /// </summary>
        public static readonly PropertyData IsLeafProperty = RegisterProperty("IsLeaf", typeof(bool), null);

        #endregion
    }
}
