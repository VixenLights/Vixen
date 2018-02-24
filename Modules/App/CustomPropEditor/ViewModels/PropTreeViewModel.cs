using System.Collections.ObjectModel;
using Catel.Data;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Converters;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
    public class PropTreeViewModel: ViewModelBase
    {
        public PropTreeViewModel(Prop prop)
        {
            Prop = prop;
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
        /// Gets or sets the RootNode value.
        /// </summary>
        [ViewModelToModel("Prop", "RootNode", ConverterType = typeof(RootNodeToCollectionMapping))]
        public ObservableCollection<ElementModel> RootNodes
        {
            get { return GetValue<ObservableCollection<ElementModel>>(RootNodesProperty); }
            set { SetValue(RootNodesProperty, value); }
        }

        /// <summary>
        /// RootNode property data.
        /// </summary>
        public static readonly PropertyData RootNodesProperty = RegisterProperty("RootNodes", typeof(ObservableCollection<ElementModel>), null);

        #endregion
    }
}
