using Common.WPFCommon.ViewModel;

namespace VixenModules.App.TimingTrackBrowser.Model.InternalVendorInventory
{
    public class Song:BindableBase, IComparable
    {
        private string _artist;
        private string _title;
        private string _creator;
        private Uri _timingLink;
        private string _categoryId;
        private Uri _songLink;

        public Song()
        {
            _artist = string.Empty;
            _title = string.Empty;
            _creator = string.Empty;
            _categoryId = string.Empty;
        }

        public string Artist
        {
            get => _artist;
            set
            {
                if (value == _artist) return;
                _artist = value;
                OnPropertyChanged(nameof(_artist));
            }
        }


        public string Title
        {
            get => _title;
            set
            {
                if (value == _title) return;
                _title = value;
                OnPropertyChanged(nameof(_title));
            }
        }


        public string Creator
        {
            get => _creator;
            set
            {
                if (value == _creator) return;
                _creator = value;
                OnPropertyChanged(nameof(_creator));
            }
        }


        public Uri TimingLink
        {
            get => _timingLink;
            set
            {
                if (value == _timingLink) return;
                _timingLink = value;
                OnPropertyChanged(nameof(_timingLink));
            }
        }


        public string CategoryId
        {
            get => _categoryId;
            set
            {
                if (value == _categoryId) return;
                _categoryId = value;
                OnPropertyChanged(nameof(_categoryId));
            }
        }


        public Uri SongLink
        {
            get => _songLink;
            set
            {
                if (value == _songLink) return;
                _songLink = value;
                OnPropertyChanged(nameof(_songLink));
            }
        }

        #region Implementation of IComparable

        /// <inheritdoc />
        public int CompareTo(object obj)
        {
            if (obj is Song s)
            {
                return -String.Compare(s.Title, this.Title, StringComparison.CurrentCulture);
            }

            return 0;
        }

        #endregion
    }
}
