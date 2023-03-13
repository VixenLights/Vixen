using Common.WPFCommon.ViewModel;

namespace VixenModules.App.TimingTrackBrowser.Model.InternalVendorInventory
{
	public class SongLink: BindableBase
	{
		private Uri _timingLink;
		private Uri _songTrackLink;
		private string _name;
		private string _description;

		public SongLink()
		{
			Name = "Default Model";
            Description = @"No description provided.";
        }

		public SongLink(string name, string description, Uri timingLink, Uri songTrackLink):this()
		{
			if (!string.IsNullOrEmpty(name))
			{
				Name = name;
			}

			if (!string.IsNullOrEmpty(description))
			{
				Description = description;
			}
			TimingLink = timingLink;
            SongTrackLink = songTrackLink;
        }

		/// <summary>
		/// The name of the model link
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
		/// Description for the model link
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
		/// Link to the model file
		/// </summary>
		public Uri TimingLink
		{
			get => _timingLink;
			set
			{
				if (Equals(value, _timingLink)) return;
				_timingLink = value;
				OnPropertyChanged(nameof(TimingLink));
			}
		}

        /// <summary>
        /// Link to the model file
        /// </summary>
        public Uri SongTrackLink
        {
            get => _songTrackLink;
            set
            {
                if (Equals(value, _songTrackLink)) return;
                _songTrackLink = value;
                OnPropertyChanged(nameof(SongTrackLink));
            }
        }
	}
}
