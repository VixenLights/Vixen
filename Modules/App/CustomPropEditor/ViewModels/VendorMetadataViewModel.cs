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

		#region Name property

		/// <summary>
		/// Gets or sets the Vendor value.
		/// </summary>
		[PropertyOrder(1)]
		[Category("Vendor")]
		[ViewModelToModel("VendorMetadata")]
		public string Name
		{
			get { return GetValue<string>(VendorProperty); }
			set { SetValue(VendorProperty, value); }
		}

		/// <summary>
		/// Vendor property data.
		/// </summary>
		public static readonly PropertyData VendorProperty = RegisterProperty("Name", typeof(string), null);

		#endregion

	    #region Contact property

	    /// <summary>
	    /// Gets or sets the Contact value.
	    /// </summary>
	    [PropertyOrder(2)]
	    [Category("Vendor")]
	    [ViewModelToModel("VendorMetadata")]
	    public string Contact
	    {
		    get { return GetValue<string>(ContactProperty); }
		    set { SetValue(ContactProperty, value); }
	    }

	    /// <summary>
	    /// Contact property data.
	    /// </summary>
	    public static readonly PropertyData ContactProperty = RegisterProperty("Contact", typeof(string), null);

	    #endregion

		#region Website property

		/// <summary>
		/// Gets or sets the VendorUrl value.
		/// </summary>
		[PropertyOrder(3)]
		[DisplayName("Website")]
		[Category("Vendor")]
		[ViewModelToModel("VendorMetadata")]
		public string Website
		{
			get { return GetValue<string>(VendorUrlProperty); }
			set { SetValue(VendorUrlProperty, value); }
		}

		/// <summary>
		/// VendorUrl property data.
		/// </summary>
		public static readonly PropertyData VendorUrlProperty = RegisterProperty("Website", typeof(string), null);

		#endregion

		#region Email property

		/// <summary>
		/// Gets or sets the VendorEmail value.
		/// </summary>
		[PropertyOrder(4)]
		[Category("Vendor")]
		[DisplayName("Email")]
		[ViewModelToModel("VendorMetadata")]
		public string Email
		{
			get { return GetValue<string>(VendorEmailProperty); }
			set { SetValue(VendorEmailProperty, value); }
		}

		/// <summary>
		/// VendorEmail property data.
		/// </summary>
		public static readonly PropertyData VendorEmailProperty = RegisterProperty("Email", typeof(string), null);

		#endregion

		#region Phone property

		/// <summary>
		/// Gets or sets the Phone value.
		/// </summary>
		[PropertyOrder(5)]
		[Category("Vendor")]
		[ViewModelToModel("VendorMetadata")]
	    public string Phone
	    {
		    get { return GetValue<string>(PhoneProperty); }
		    set { SetValue(PhoneProperty, value); }
	    }

	    /// <summary>
	    /// Phone property data.
	    /// </summary>
	    public static readonly PropertyData PhoneProperty = RegisterProperty("Phone", typeof(string), null);

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
		
	    [Browsable(false)]
	    public new bool IsCanceled => base.IsCanceled;

	    [Browsable(false)]
	    public new bool IsSaved => base.IsSaved;
		
	    #endregion

	}
}
