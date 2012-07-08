namespace VixenModules.App.DisplayPreview.Model
{
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows.Media.Imaging;
    using Vixen.Module;

    [DataContract]
    [KnownType(typeof(BitmapImage))]
    public class DisplayPreviewModuleDataModel : ModuleDataModelBase, INotifyPropertyChanged
    {
        private string _backgroundImage;
        private int _displayHeight;
        private ObservableCollection<DisplayItem> _displayItems;
        private int _displayWidth;
        private double _opacity;
        private Preferences _preferences;
        private bool _isEnabled;

        [OnDeserializing]
        private void OnDeserializing(StreamingContext context)
        {
            _isEnabled = true;
        }

        [DataMember]
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                PropertyChanged.NotifyPropertyChanged("IsEnabled", this);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [DataMember]
        public string BackgroundImage
        {
            get
            {
                return _backgroundImage;
            }

            set
            {
                _backgroundImage = value;
                PropertyChanged.NotifyPropertyChanged("BackgroundImage", this);
            }
        }

        [DataMember]
        public int DisplayHeight
        {
            get
            {
                if (_displayHeight <= 0)
                {
                    _displayHeight = Preferences.DisplayHeightDefault;
                }

                return _displayHeight;
            }

            set
            {
                _displayHeight = value;
                PropertyChanged.NotifyPropertyChanged("DisplayHeight", this);
            }
        }

        [DataMember]
        public ObservableCollection<DisplayItem> DisplayItems
        {
            get
            {
                return _displayItems ?? (_displayItems = new ObservableCollection<DisplayItem>());
            }

            set
            {
                _displayItems = value ?? new ObservableCollection<DisplayItem>();
                PropertyChanged.NotifyPropertyChanged("DisplayItems", this);
            }
        }

        [DataMember]
        public int DisplayWidth
        {
            get
            {
                if (_displayWidth <= 0)
                {
                    _displayWidth = Preferences.DisplayWidthDefault;
                }

                return _displayWidth;
            }

            set
            {
                _displayWidth = value;
                PropertyChanged.NotifyPropertyChanged("DisplayWidth", this);
            }
        }

        [DataMember]
        public double Opacity
        {
            get
            {
                if (_opacity <= 0)
                {
                    _opacity = Preferences.OpacityDefault;
                }

                return _opacity;
            }

            set
            {
                _opacity = value;
                PropertyChanged.NotifyPropertyChanged("Opacity", this);
            }
        }

        [DataMember]
        public Preferences Preferences
        {
            get
            {
                return _preferences ?? (_preferences = Preferences.CurrentPreferences);
            }

            set
            {
                _preferences = value;
                PropertyChanged.NotifyPropertyChanged("Preferences", this);
            }
        }

        public override IModuleDataModel Clone()
        {
            return new DisplayPreviewModuleDataModel
                   {
                       BackgroundImage = BackgroundImage, 
                       DisplayItems =
                           new ObservableCollection<DisplayItem>(DisplayItems.Select(displayItem => displayItem.Clone()).ToList()), 
                       DisplayHeight = DisplayHeight, 
                       DisplayWidth = DisplayWidth, 
                       Opacity = Opacity, 
                       Preferences = Preferences.Clone(), 
                   };
        }
    }
}
