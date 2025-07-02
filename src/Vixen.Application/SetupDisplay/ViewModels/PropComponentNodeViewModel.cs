using System.ComponentModel;
using Catel.Data;
using Catel.MVVM;
using Vixen.Sys.Props.Components;

namespace VixenApplication.SetupDisplay.ViewModels
{
	public class PropComponentNodeViewModel:ViewModelBase
	{
		public PropComponentNodeViewModel(PropComponentNode model, PropComponentNodeViewModel? parent)
		{
			PropComponentNode = model;
			ChildrenViewModels = new PropComponentNodeViewModelCollection(model.Children, this);
			((IRelationalViewModel)this).SetParentViewModel(parent);
			DeferValidationUntilFirstSaveCall = false;
			AlwaysInvokeNotifyChanged = true;
		}

		#region PropComponentNode model property

		/// <summary>
		/// Gets or sets the PropNode value.
		/// </summary>
		[Browsable(false)]
		[Model]
		public PropComponentNode PropComponentNode
		{
			get { return GetValue<PropComponentNode>(PropNodeProperty); }
			init { SetValue(PropNodeProperty, value); }
		}

		/// <summary>
		/// PropNode property data.
		/// </summary>
		public static readonly IPropertyData PropNodeProperty = RegisterProperty<PropComponentNode>(nameof(PropComponentNode));

		#endregion

		#region ChildrenViewModels property

		/// <summary>
		/// Gets or sets the ChildrenViewModels value.
		/// </summary>
		[Browsable(false)]
		public PropComponentNodeViewModelCollection ChildrenViewModels
		{
			get { return GetValue<PropComponentNodeViewModelCollection>(ChildrenViewModelsProperty); }
			set { SetValue(ChildrenViewModelsProperty, value); }
		}

		/// <summary>
		/// ChildrenViewModels property data.
		/// </summary>
		public static readonly IPropertyData ChildrenViewModelsProperty = RegisterProperty<PropComponentNodeViewModelCollection>(nameof(ChildrenViewModels));

		#endregion

		#region Name

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

		public void Dispose()
		{
			((IRelationalViewModel)this).SetParentViewModel(null);
		}
	}
}