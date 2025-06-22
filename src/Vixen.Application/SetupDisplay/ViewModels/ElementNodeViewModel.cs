using System.Collections.ObjectModel;
using Catel.Data;
using Catel.MVVM;
using System.ComponentModel;
using Vixen.Sys;

namespace VixenApplication.SetupDisplay.ViewModels
{
	public class ElementNodeViewModel : ViewModelBase, ISelectableExpander
	{
		public ElementNodeViewModel(IElementNode elementNode) 
		{
			ElementNode = elementNode;
			Children = new ObservableCollection<ElementNodeViewModel>(elementNode.Children.Select(x => new ElementNodeViewModel(x)));
		}

		#region Name property

		/// <summary>
		/// Gets or sets the Name value.
		/// </summary>
		[ViewModelToModel]
		public String Name
		{
			get { return GetValue<String>(NameProperty); }
			set { SetValue(NameProperty, value); }
		}

		/// <summary>
		/// Name property data.
		/// </summary>
		public static readonly IPropertyData NameProperty = RegisterProperty<String>(nameof(Name));

		#endregion

		#region Children property

		/// <summary>
		/// Gets or sets the Children value.
		/// </summary>
		public ObservableCollection<ElementNodeViewModel> Children
		{
			get { return GetValue<ObservableCollection<ElementNodeViewModel>>(ChildrenProperty); }
			set { SetValue(ChildrenProperty, value); }
		}

		/// <summary>
		/// Children property data.
		/// </summary>
		public static readonly IPropertyData ChildrenProperty = RegisterProperty<ObservableCollection<ElementNodeViewModel>>(nameof(Children));

		#endregion
		
		#region ElementNode property

		/// <summary>
		/// Gets or sets the ElementNode value.
		/// </summary>
		[Model]
		public IElementNode ElementNode
		{
			get { return GetValue<IElementNode>(ElementNodeProperty); }
			set { SetValue(ElementNodeProperty, value); }
		}

		/// <summary>
		/// ElementNode property data.
		/// </summary>
		public static readonly IPropertyData ElementNodeProperty = RegisterProperty<IElementNode>(nameof(ElementNode));

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
				SetValue(IsSelectedProperty, value);
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
	}
}