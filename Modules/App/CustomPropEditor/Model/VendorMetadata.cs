using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
    public class VendorMetadata: BindableBase
    {
	    private string _name;
	    private string _website;
	    private string _email;
	    private string _phone;
	    private string _contact;

	    #region Vendor Info

		public string Name
	    {
		    get { return _name; }
		    set
		    {
			    if (value == _name) return;
			    _name = value;
			    OnPropertyChanged(nameof(Name));
		    }
	    }

	    public string Website
	    {
		    get { return _website; }
		    set
		    {
			    if (value == _website) return;
			    _website = value;
			    OnPropertyChanged(nameof(Website));
		    }
	    }

	    public string Contact
	    {
		    get { return _contact; }
		    set
		    {
			    if (value == _contact) return;
			    _contact = value;
			    OnPropertyChanged(nameof(Contact));
		    }
	    }

	    public string Email
	    {
		    get { return _email; }
		    set
		    {
			    if (value == _email) return;
			    _email = value;
			    OnPropertyChanged(nameof(Email));
		    }
	    }

	    public string Phone
	    {
		    get { return _phone; }
		    set
		    {
			    if (value == _phone) return;
			    _phone = value;
			    OnPropertyChanged(nameof(Phone));
		    }
	    }

		#endregion
	}
}
