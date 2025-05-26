using System.ComponentModel;
using Catel.Data;
using Catel.MVVM;
using Vixen.Sys;
using Vixen.Sys.Props;

namespace VixenApplication.SetupDisplay.ViewModels
{
    [DisplayName("PropNode View Model")]
	public sealed class PropNodeViewModel : ViewModelBase, ISelectableExpander
	{
        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		//private DateTime _selectedTime = DateTime.MaxValue;
        //private string _textHoldValue = String.Empty;

		public PropNodeViewModel(PropNode model, PropNodeViewModel? parent)
        {
            PropNode = model;
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
        public PropNode PropNode
        {
            get { return GetValue<PropNode>(PropNodeProperty); }
            private set { SetValue(PropNodeProperty, value); }
        }

        /// <summary>
        /// PropNode property data.
        /// </summary>
        public static readonly IPropertyData PropNodeProperty = RegisterProperty<PropNode>(nameof(PropNode));

        #endregion

		public bool IsSelected { get; set; }
        public bool IsExpanded { get; set; }
        public void Dispose()
        {
			((IRelationalViewModel)this).SetParentViewModel(null);
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

		public void RemoveFromParent()
        {
            if (ParentViewModel is PropNodeViewModel parentVm)
            {
                VixenSystem.Props.RemoveFromParent(PropNode, parentVm.PropNode);
                IsDirty = true;
            }
        }
	}
}