using System;
using System.Collections.Generic;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model.InternalVendorInventory
{
	public class Vendor : BindableBase
	{
		private string _name;
		private string _email;
		private string _contact;
		private string _phone;
		private string _description;
		private Uri _logo;
		private List<WebLink> _webLinks;
		private Guid _id;
		
		public Vendor()
		{
			Id = Guid.NewGuid();
		}

		public Guid Id
		{
			get => _id;
			set
			{
				if (value.Equals(_id)) return;
				_id = value;
				OnPropertyChanged(nameof(Id));
			}
		}

		/// <summary>
		/// Vendor Name
		/// </summary>
		public string Name
		{
			get => _name;
			set
			{
				if (value == _name) return;
				_name = value;
				OnPropertyChanged(nameof(Name));
			}
		}

		/// <summary>
		/// Primary contact
		/// </summary>
		public string Contact
		{
			get => _contact;
			set
			{
				if (value == _contact) return;
				_contact = value;
				OnPropertyChanged(nameof(Contact));
			}
		}

		/// <summary>
		/// Primary email address
		/// </summary>
		public string Email
		{
			get => _email;
			set
			{
				if (value == _email) return;
				_email = value;
				OnPropertyChanged(nameof(Email));
			}
		}

		/// <summary>
		/// Primary phone number
		/// </summary>
		public string Phone
		{
			get => _phone;
			set
			{
				if (value == _phone) return;
				_phone = value;
				OnPropertyChanged(nameof(Phone));
			}
		}

		/// <summary>
		/// Links for the vendors web presence
		/// </summary>
		public List<WebLink> WebLinks
		{
			get => _webLinks;
			set
			{
				if (Equals(value, _webLinks)) return;
				_webLinks = value;
				OnPropertyChanged(nameof(WebLinks));
			}
		}

		/// <summary>
		/// Vender description
		/// </summary>
		public string Description
		{
			get => _description;
			set
			{
				if (value == _description) return;
				_description = value;
				OnPropertyChanged(nameof(Description));
			}
		}

		/// <summary>
		/// Link to the vendor Logo
		/// </summary>
		public Uri Logo
		{
			get => _logo;
			set
			{
				if (value == _logo) return;
				_logo = value;
				OnPropertyChanged(nameof(Logo));
			}
		}
	}
}
