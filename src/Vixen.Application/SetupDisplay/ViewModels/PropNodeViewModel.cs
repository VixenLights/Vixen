using System.ComponentModel;
using System.Diagnostics;
using Catel.Data;
using Catel.MVVM;
using Vixen.Sys;
using Vixen.Sys.Props;

namespace VixenApplication.SetupDisplay.ViewModels
{
    [DisplayName("PropNode View Model")]
	public sealed class PropNodeViewModel : ViewModelBase, ISelectableExpander, IEquatable<PropNodeViewModel>
	{
        private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();
		private DateTime _selectedTime = DateTime.MaxValue;
        private string _textHoldValue = String.Empty;

		public PropNodeViewModel(PropNode model, PropNodeViewModel? parent)
        {
            PropNode = model;
            ChildrenViewModels = new PropNodeViewModelCollection(model.Children, this);
            ((IRelationalViewModel)this).SetParentViewModel(parent);
            DeferValidationUntilFirstSaveCall = false;
            AlwaysInvokeNotifyChanged = true;
			PropertyChanged += PropNodeViewModel_PropertyChanged;
        }

		private void PropNodeViewModel_PropertyChanged(object? sender, PropertyChangedEventArgs e)
		{
            if (nameof(IsSelected).Equals(e.PropertyName))
            {
                //If we become selected, ensure that our parents are marked expanded
                if (IsSelected)
                {
                   ExpandTree(this);
                }
			}
			
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
            init { SetValue(PropNodeProperty, value); }
        }

        /// <summary>
        /// PropNode property data.
        /// </summary>
        public static readonly IPropertyData PropNodeProperty = RegisterProperty<PropNode>(nameof(PropNode));

		#endregion

        /// <summary>
        /// Gets or sets the Name value.
        /// </summary>;
        [ViewModelToModel]
        public string Name
        {
            get { return GetValue<string>(NameProperty); }
            set { SetValue(NameProperty, value); }
        }

        /// <summary>;
        /// Name property data.
        /// </summary>;
        public static readonly IPropertyData NameProperty = RegisterProperty<string>(nameof(Name));

		#region IsLeaf

        /// <summary>
        /// Gets or sets the IsLeaf of the PropNode.
        /// </summary>
        [ViewModelToModel]
        public bool IsLeaf
        {
            get { return GetValue<bool>(IsLeafProperty); }
            set { SetValue(IsLeafProperty, value); }
        }

        /// <summary>
        /// Register the IsLeaf property so it is known in the class.
        /// </summary>
        public static readonly IPropertyData IsLeafProperty = RegisterProperty<bool>(nameof(IsLeaf));

		#endregion

		#region IsGroupNode property

		/// <summary>
		/// Gets or sets the IsGroupNode value.
		/// </summary>
		[ViewModelToModel()]
		public bool IsGroupNode
		{
			get { return GetValue<bool>(IsGroupNodeProperty); }
			set { SetValue(IsGroupNodeProperty, value); }
		}

		/// <summary>
		/// IsGroupNode property data.
		/// </summary>
		public static readonly IPropertyData IsGroupNodeProperty = RegisterProperty<bool>(nameof(IsGroupNode));

		#endregion

		#region IsSelected property

		/// <summary>
		/// Gets or sets the IsSelected value.
		/// </summary>
		[Browsable(false)]
        public bool IsSelected
        {
            get
            {
                return GetValue<bool>(IsSelectedProperty);
            }
            set
            {
                bool tempDirty = IsDirty;
                SetValue(IsSelectedProperty, value);
                _selectedTime = DateTime.Now;
                IsDirty = tempDirty;
            }
        }

        /// <summary>
        /// IsSelected property data.
        /// </summary>
        public static readonly IPropertyData IsSelectedProperty = RegisterProperty<bool>(nameof(IsSelected));

        #endregion

        #region IsExpanded property

        /// <summary>
        /// Gets or sets the IsExpanded value.
        /// </summary>
        public bool IsExpanded
        {
            get { return GetValue<bool>(IsExpandedProperty); }
            set { SetValue(IsExpandedProperty, value); }
        }

        /// <summary>
        /// IsExpanded property data.
        /// </summary>
        public static readonly IPropertyData IsExpandedProperty = RegisterProperty<bool>(nameof(IsExpanded));

        #endregion

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


        #region IsEditing property

        /// <summary>
        /// Gets or sets the IsEditing value.
        /// </summary>
        [Browsable(false)]
        public bool IsEditing
        {
            get { return GetValue<bool>(IsEditingProperty); }
            set { SetValue(IsEditingProperty, value); }
        }

        /// <summary>
        /// IsEditing property data.
        /// </summary>
        public static readonly IPropertyData IsEditingProperty = RegisterProperty<bool>(nameof(IsEditing));

        #endregion

		#region BeginEdit command

		private Command _beginEditCommand;

        /// <summary>
        /// Gets the LeftMouseUp command.
        /// </summary>
        [Browsable(false)]
        public Command BeginEditCommand
        {
            get { return _beginEditCommand ??= new Command(BeginEdit); }
        }

        /// <summary>
        /// Method to invoke when the LeftMouseUp command is executed.
        /// </summary>
        private void BeginEdit()
        {
            if (IsSelected && _selectedTime.AddMilliseconds(750) < DateTime.Now)
            {
                IsEditing = true;
                _textHoldValue = PropNode.Name;
            }
        }

        #endregion

        #region DoneEditing command

        private Command _doneEditingCommand;

        /// <summary>
        /// Gets the EditFocusLost command.
        /// </summary>
        [Browsable(false)]
        public Command DoneEditingCommand
        {
            get { return _doneEditingCommand ??= new Command(DoneEditing); }
        }

        /// <summary>
        /// Method to invoke when the EditFocusLost command is executed.
        /// </summary>
        private void DoneEditing()
        {
            //Check for unique name if necessary and update.
            IsEditing = false;
            IsDirty = true;
        }

        #endregion

        #region CancelEditing command

        private Command _cancelEditingCommand;

        /// <summary>
        /// Gets the CancelEditing command.
        /// </summary>
        [Browsable(false)]
        public Command CancelEditingCommand
        {
            get { return _cancelEditingCommand ?? (_cancelEditingCommand = new Command(CancelEditing)); }
        }

        /// <summary>
        /// Method to invoke when the CancelEditing command is executed.
        /// </summary>
        private void CancelEditing()
        {
            IsEditing = false;
            PropNode.Name = _textHoldValue;
        }

		#endregion

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

        /// <summary>
        /// Removes this node from its parent
        /// </summary>
		public void RemoveFromParent()
        {
            if (ParentViewModel is PropNodeViewModel parentVm)
            {
                VixenSystem.Props.RemoveFromParent(PropNode, parentVm.PropNode);
                IsDirty = true;
            }
        }

        public IEnumerable<PropNodeViewModel> GetLeafEnumerator()
        {
            if (PropNode.IsLeaf)
            {
                return [this];
            }

            return ChildrenViewModels.SelectMany(x => x.GetLeafEnumerator());
        }

        public bool Equals(PropNodeViewModel? other)
        {
            if (other is null)
            {
                return false;
            }

            return this.UniqueIdentifier == other.UniqueIdentifier;
        }

        public override bool Equals(object? obj)
        {
            return ReferenceEquals(this, obj) || obj is PropNodeViewModel other && Equals(other);
        }

        public override int GetHashCode() => UniqueIdentifier.GetHashCode();

	}
}