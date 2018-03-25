using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
    public class VendorMetadata: BindableBase
    {
	    private string _vendor;
	    private string _vendorUrl;
	    private string _vendorEmail;
	    private string _vendorPhone;
	    private string _category;
	    private string _subCategory;

		#region Vendor Info

		public string Vendor
	    {
		    get { return _vendor; }
		    set
		    {
			    if (value == _vendor) return;
			    _vendor = value;
			    OnPropertyChanged(nameof(Vendor));
		    }
	    }

	    public string VendorUrl
	    {
		    get { return _vendorUrl; }
		    set
		    {
			    if (value == _vendorUrl) return;
			    _vendorUrl = value;
			    OnPropertyChanged(nameof(VendorUrl));
		    }
	    }

	    public string VendorEmail
	    {
		    get { return _vendorEmail; }
		    set
		    {
			    if (value == _vendorEmail) return;
			    _vendorEmail = value;
			    OnPropertyChanged(nameof(VendorEmail));
		    }
	    }

	    public string VendorPhone
	    {
		    get { return _vendorPhone; }
		    set
		    {
			    if (value == _vendorPhone) return;
			    _vendorPhone = value;
			    OnPropertyChanged(nameof(VendorPhone));
		    }
	    }

	    public string Category
	    {
		    get { return _category; }
		    set
		    {
			    if (value == _category) return;
			    _category = value;
			    OnPropertyChanged(nameof(Category));
		    }
	    }

	    public string SubCategory
	    {
		    get { return _subCategory; }
		    set
		    {
			    if (value == _subCategory) return;
			    _subCategory = value;
			    OnPropertyChanged(nameof(SubCategory));
		    }
	    }

		#endregion
	}
}
