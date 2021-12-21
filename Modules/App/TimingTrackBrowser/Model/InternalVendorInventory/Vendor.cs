using System;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.TimingTrackBrowser.Model.InternalVendorInventory
{
	public class Vendor : BindableBase
	{
		private string _name;
        private Uri _logo;
		private Uri _website;
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
		/// Link for the vendors web presence
		/// </summary>
		public Uri Website
		{
			get => _website;
			set
			{
				if (Equals(value, _website)) return;
				_website = value;
				OnPropertyChanged(nameof(Website));
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
