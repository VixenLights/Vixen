using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls.WpfPropertyGrid;
using Catel.Data;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Model;
using VixenModules.App.CustomPropEditor.Services;
using PropertyData = Catel.Data.PropertyData;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
	[DisplayName("Element Model")]
	public sealed class ElementModelViewModel : ViewModelBase, ISelectableExpander, IDisposable
	{
		public ElementModelViewModel(ElementModel model, ElementModelViewModel parent)
		{
			ElementModel = model;
			ChildrenViewModels = new ElementViewModelCollection(model.Children, this);
			ElementModelLookUpService.Instance.AddModel(model.Id, this);
			((IRelationalViewModel)this).SetParentViewModel(parent);
			DeferValidationUntilFirstSaveCall = false;
		}

		#region ElementModel model property

		/// <summary>
		/// Gets or sets the ElementModel value.
		/// </summary>
		[Browsable(false)]
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
		[PropertyOrder(0)]
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
		[PropertyOrder(1)]
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

		#region ElementType property

		/// <summary>
		/// Gets or sets the IsString value.
		/// </summary>
		[DisplayName("String Type")]
		[PropertyOrder(2)]
		[ViewModelToModel("ElementModel")]
		public ElementType ElementType => GetValue<ElementType>(IsStringProperty);

		/// <summary>
		/// IsString property data.
		/// </summary>
		public static readonly PropertyData IsStringProperty = RegisterProperty("ElementType", typeof(ElementType), null);

		#endregion

		#region LightCount property

		/// <summary>
		/// Gets or sets the LightCount value.
		/// </summary>
		[DisplayName("Light Count")]
		[PropertyOrder(3)]
		[ViewModelToModel("ElementModel")]
		public int LightCount
		{
			get { return GetValue<int>(LightCountProperty); }
			set { SetValue(LightCountProperty, value); }
		}

		/// <summary>
		/// LightCount property data.
		/// </summary>
		public static readonly PropertyData LightCountProperty = RegisterProperty("LightCount", typeof(int), null);

		#endregion

		#region LightSize property

		/// <summary>
		/// Gets or sets the LightSize value.
		/// </summary>
		[DisplayName("Light Size")]
		[PropertyOrder(4)]
		[ViewModelToModel("ElementModel")]
		public int LightSize
		{
			get { return GetValue<int>(LightSizeProperty); }
			set { SetValue(LightSizeProperty, value); }
		}

		/// <summary>
		/// LightSize property data.
		/// </summary>
		public static readonly PropertyData LightSizeProperty = RegisterProperty("LightSize", typeof(int), null);

		#endregion

		#region Parents property

		/// <summary>
		/// Gets or sets the Parents value.
		/// </summary>
		[Browsable(false)]
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
		[Browsable(false)]
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

		#region ChildrenViewModels property

		/// <summary>
		/// Gets or sets the ChildrenViewModels value.
		/// </summary>
		[Browsable(false)]
		public ElementViewModelCollection ChildrenViewModels
		{
			get { return GetValue<ElementViewModelCollection>(ChildrenViewModelsProperty); }
			set { SetValue(ChildrenViewModelsProperty, value); }
		}

		/// <summary>
		/// ChildrenViewModels property data.
		/// </summary>
		public static readonly PropertyData ChildrenViewModelsProperty = RegisterProperty("ChildrenViewModels", typeof(ElementViewModelCollection));

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
				//if (IsLeaf)
				//{
				//	return LightViewModels.Any(l => l.IsSelected);
				//}

				return GetValue<bool>(IsSelectedProperty);
			}
			set
			{
				//if (IsLeaf)
				//{
				//	LightViewModels.ForEach(l => l.IsSelected = value);
				//}

				SetValue(IsSelectedProperty, value);
			}
		}

		/// <summary>
		/// IsSelected property data.
		/// </summary>
		public static readonly PropertyData IsSelectedProperty = RegisterProperty("IsSelected", typeof(bool));

		#endregion

		#region IsExpanded property

		/// <summary>
		/// Gets or sets the IsExpanded value.
		/// </summary>
		[Browsable(false)]
		public bool IsExpanded
		{
			get
			{
				return GetValue<bool>(IsExpandedProperty);
			}
			set { SetValue(IsExpandedProperty, value); }
		}

		/// <summary>
		/// IsExpanded property data.
		/// </summary>
		public static readonly PropertyData IsExpandedProperty = RegisterProperty("IsExpanded", typeof(bool));

		#endregion

		#region IsLeaf property

		/// <summary>
		/// Gets or sets the IsLeaf value.
		/// </summary>
		[Browsable(false)]
		public bool IsLeaf => ElementModel.IsLeaf;

		#endregion



		#region Overrides

		//We are not using these properties in the view so hiding them so the property giris does not expose them.

		[Browsable(false)]
		public new DateTime ViewModelConstructionTime => base.ViewModelConstructionTime;

		[Browsable(false)]
		public new int UniqueIdentifier => base.UniqueIdentifier;

		[Browsable(false)]
		public new string Title => base.Title;

		[Browsable(false)]
		public new bool IsClosed => base.IsClosed;

		[Browsable(false)]
		public new IViewModel ParentViewModel => base.ParentViewModel;

		#endregion

		public void RemoveFromParent()
		{
			var parentVm = ParentViewModel as ElementModelViewModel;
			if (parentVm != null)
			{
				PropModelServices.Instance().RemoveFromParent(ElementModel, parentVm.ElementModel);
			}
		}

		public IEnumerable<Guid> GetParentIds()
		{
			return ElementModel.Parents.Select(x => x.Id);
		}

		public IEnumerable<Guid> GetChildrenIds()
		{
			return ElementModel.Children.Select(x => x.Id);
		}

		public IEnumerable<ElementModelViewModel> GetLeafEnumerator()
		{
			if (IsLeaf)
			{
				return (new[] { this });
			}

			return ChildrenViewModels.SelectMany(x => x.GetLeafEnumerator());
		}


		protected override void ValidateFields(List<IFieldValidationResult> validationResults)
		{
			if (string.IsNullOrEmpty(Name))
			{
				validationResults.Add(FieldValidationResult.CreateError(nameof(Name), "Name can not be empty"));
			}
			else if (PropModelServices.Instance().IsNameDuplicated(Name))
			{
				validationResults.Add(FieldValidationResult.CreateError(nameof(Name), "Duplicate name"));
			}

			if (LightSize <= 0)
			{
				validationResults.Add(FieldValidationResult.CreateError(nameof(LightSize), "Light size must be > 0"));
			}
		}

		public void Dispose()
		{
			((IRelationalViewModel)this).SetParentViewModel(null);
			ElementModelLookUpService.Instance.RemoveModel(ElementModel.Id, this);
		}


	}
}
