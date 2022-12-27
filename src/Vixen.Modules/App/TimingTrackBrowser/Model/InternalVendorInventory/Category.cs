using Common.WPFCommon.ViewModel;

namespace VixenModules.App.TimingTrackBrowser.Model.InternalVendorInventory
{
	public class Category : BindableBase
	{
		private string _id;
		private string _name;
		private List<Category> _categories;
		private List<Song> _songs;

		public Category()
		{
			Songs = new List<Song>();
            Categories = new List<Category>();
        }

		/// <summary>
		/// Category id that is unique to each vendor
		/// </summary>
		public string Id
		{
			get => _id;
			set
			{
				if (value == _id) return;
				_id = value;
				OnPropertyChanged(nameof(Id));
			}
		}

		/// <summary>
		/// Category name
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
		/// Sub categories
		/// </summary>
		public List<Category> Categories
		{
			get => _categories;
			set
			{
				if (Equals(value, _categories)) return;
				_categories = value;
				OnPropertyChanged(nameof(Categories));
			}
		}

		/// <summary>
		/// Songs belonging to this category
		/// </summary>
		public List<Song> Songs
		{
			get => _songs;
			set
			{
				if (Equals(value, _songs)) return;
				_songs = value;
				OnPropertyChanged(nameof(Songs));
			}
		}

        #region Overrides of Object

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{Name} from toString";
        }

        #endregion
    }
}
