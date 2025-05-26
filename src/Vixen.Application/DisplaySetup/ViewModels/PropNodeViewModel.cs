using System.ComponentModel;
using Catel.Data;
using Catel.MVVM;
using Vixen.Sys.Props;

namespace VixenApplication.DisplaySetup.ViewModels
{
    [DisplayName("PropNode View Model")]
	public sealed class PropNodeViewModel : ViewModelBase, ISelectableExpander, IDisposable
	{
        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
        public event EventHandler ModelsChanged;

        public PropNodeViewModel(PropNode model, PropNodeViewModel parent)
        {
            //ElementModel = model;
            ChildrenViewModels = new PropNodeViewModelCollection(model.Children, this);
            //ElementModelLookUpService.Instance.AddModel(model.Id, this);
            ((IRelationalViewModel)this).SetParentViewModel(parent);
            DeferValidationUntilFirstSaveCall = false;
            AlwaysInvokeNotifyChanged = true;

        }

        #region PropNode model property

        /// <summary>
        /// Gets or sets the PropNode value.
        /// </summary>
        [Browsable(false)]
        [Model]
        public PropNode ElementModel
        {
            get { return GetValue<PropNode>(PropNodeProperty); }
            private set { SetValue(PropNodeProperty, value); }
        }

        /// <summary>
        /// ElementModel property data.
        /// </summary>
        public static readonly IPropertyData PropNodeProperty = RegisterProperty<PropNode>(nameof(ElementModel));

        #endregion

		public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #region ChildrenViewModels property

        /// <summary>
        /// Gets or sets the ChildrenViewModels value.
        /// </summary>
        [Browsable(false)]
        public PropNodeViewModelCollection ChildrenViewModels
        {
            get { return GetValue<PropNodeViewModelCollection>(ChildrenViewModelsProperty); }
            set { SetValue(ChildrenViewModelsProperty, value); }
        }

        /// <summary>
        /// ChildrenViewModels property data.
        /// </summary>
        public static readonly IPropertyData ChildrenViewModelsProperty = RegisterProperty<PropNodeViewModelCollection>(nameof(ChildrenViewModels));

        #endregion
	}
}