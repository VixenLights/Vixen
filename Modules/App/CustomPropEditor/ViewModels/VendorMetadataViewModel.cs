using System;
using System.ComponentModel;
using System.Windows.Controls.WpfPropertyGrid;
using Catel.MVVM;
using VixenModules.App.CustomPropEditor.Model;
using PropertyData = Catel.Data.PropertyData;

namespace VixenModules.App.CustomPropEditor.ViewModels
{
    public class VendorMetadataViewModel: ViewModelBase
    {
	    public VendorMetadataViewModel(VendorMetadata vendorMetadata)
	    {
		    VendorMetadata = vendorMetadata;
	    }

		#region VendorMetadata model property

		/// <summary>
		/// Gets or sets the VendorMetadata value.
		/// </summary>
		[Browsable(false)]
		[Model]
	    public VendorMetadata VendorMetadata
	    {
		    get { return GetValue<VendorMetadata>(VendorMetadataProperty); }
		    private set { SetValue(VendorMetadataProperty, value); }
	    }

	    /// <summary>
	    /// VendorMetadata property data.
	    /// </summary>
	    public static readonly PropertyData VendorMetadataProperty = RegisterProperty("VendorMetadata", typeof(VendorMetadata));

		#endregion

		#region Vendor property

		/// <summary>
		/// Gets or sets the Vendor value.
		/// </summary>
		[PropertyOrder(1)]
		[Category("Vendor")]
		[ViewModelToModel("VendorMetadata")]
		public string Vendor
		{
			get { return GetValue<string>(VendorProperty); }
			set { SetValue(VendorProperty, value); }
		}

		/// <summary>
		/// Vendor property data.
		/// </summary>
		public static readonly PropertyData VendorProperty = RegisterProperty("Vendor", typeof(string), null);

		#endregion

		#region VendorUrl property

		/// <summary>
		/// Gets or sets the VendorUrl value.
		/// </summary>
		[PropertyOrder(2)]
		[DisplayName("Vendor URL")]
		[Category("Vendor")]
		[ViewModelToModel("VendorMetadata")]
		public string VendorUrl
		{
			get { return GetValue<string>(VendorUrlProperty); }
			set { SetValue(VendorUrlProperty, value); }
		}

		/// <summary>
		/// VendorUrl property data.
		/// </summary>
		public static readonly PropertyData VendorUrlProperty = RegisterProperty("VendorUrl", typeof(string), null);

		#endregion

		#region VendorEmail property

		/// <summary>
		/// Gets or sets the VendorEmail value.
		/// </summary>
		[PropertyOrder(3)]
		[Category("Vendor")]
		[DisplayName("Vendor Email")]
		[ViewModelToModel("VendorMetadata")]
		public string VendorEmail
		{
			get { return GetValue<string>(VendorEmailProperty); }
			set { SetValue(VendorEmailProperty, value); }
		}

		/// <summary>
		/// VendorEmail property data.
		/// </summary>
		public static readonly PropertyData VendorEmailProperty = RegisterProperty("VendorEmail", typeof(string), null);

		#endregion

		#region Category property

		/// <summary>
		/// Gets or sets the Category value.
		/// </summary>
		[PropertyOrder(4)]
		[Category("Vendor")]
		[ViewModelToModel("VendorMetadata")]
		public string Category
		{
			get { return GetValue<string>(CategoryProperty); }
			set { SetValue(CategoryProperty, value); }
		}

		/// <summary>
		/// Category property data.
		/// </summary>
		public static readonly PropertyData CategoryProperty = RegisterProperty("Category", typeof(string), null);

		#endregion

		#region SubCategory property

		/// <summary>
		/// Gets or sets the SubCategory value.
		/// </summary>
		[DisplayName("Sub Category")]
		[PropertyOrder(5)]
		[Category("Vendor")]
		[ViewModelToModel("VendorMetadata")]
		public string SubCategory
		{
			get { return GetValue<string>(SubCategoryProperty); }
			set { SetValue(SubCategoryProperty, value); }
		}

		/// <summary>
		/// SubCategory property data.
		/// </summary>
		public static readonly PropertyData SubCategoryProperty = RegisterProperty("SubCategory", typeof(string), null);

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

	}
}
