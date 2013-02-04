namespace VixenModules.Preview.DisplayPreview.Model
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    public class Preferences : INotifyPropertyChanged
    {
        private static Preferences _currentPreferences;

        private int _elementHeightDefault;

        private int _elementWidthDefault;

        private int _displayHeightDefault;

        private int _displayItemHeightDefault;

        private int _displayItemWidthDefault;

        private int _displayWidthDefault;

        private bool _keepVisualizerWindowOpen;

        private double _opacityDefault;

        public event PropertyChangedEventHandler PropertyChanged;

        public static Preferences CurrentPreferences
        {
            get
            {
                return _currentPreferences ?? (_currentPreferences = DefaultSettings);
            }

            set
            {
                _currentPreferences = value;
            }
        }

        [DataMember]
        public int ElementHeightDefault
        {
            get
            {
                return this._elementHeightDefault;
            }

            set
            {
                this._elementHeightDefault = value <= 0 ? 1 : value;
                this.PropertyChanged.NotifyPropertyChanged("MinElementHeight", this);
            }
        }

        [DataMember]
        public int ElementWidthDefault
        {
            get
            {
                return this._elementWidthDefault;
            }

            set
            {
                this._elementWidthDefault = value <= 0 ? 1 : value;
                this.PropertyChanged.NotifyPropertyChanged("MinElementWidth", this);
            }
        }

        [DataMember]
        public int DisplayHeightDefault
        {
            get
            {
                return this._displayHeightDefault;
            }

            set
            {
                this._displayHeightDefault = value <= 0 ? 1 : value;
                this.PropertyChanged.NotifyPropertyChanged("MinDisplayHeight", this);
            }
        }

        [DataMember]
        public int DisplayItemHeightDefault
        {
            get
            {
                return this._displayItemHeightDefault;
            }

            set
            {
                this._displayItemHeightDefault = value <= 0 ? 1 : value;
                this.PropertyChanged.NotifyPropertyChanged("MinDisplayItemHeight", this);
            }
        }

        [DataMember]
        public int DisplayItemWidthDefault
        {
            get
            {
                return this._displayItemWidthDefault;
            }

            set
            {
                this._displayItemWidthDefault = value <= 0 ? 1 : value;
                this.PropertyChanged.NotifyPropertyChanged("MinDisplayItemWidth", this);
            }
        }

        [DataMember]
        public int DisplayWidthDefault
        {
            get
            {
                return this._displayWidthDefault;
            }

            set
            {
                this._displayWidthDefault = value <= 0 ? 1 : value;
                this.PropertyChanged.NotifyPropertyChanged("MinDisplayWidth", this);
            }
        }

        [DataMember]
        public double OpacityDefault
        {
            get
            {
                return this._opacityDefault;
            }

            set
            {
                this._opacityDefault = value < 0 ? 0 : value;
                this.PropertyChanged.NotifyPropertyChanged("DefaultOpacity", this);
            }
        }

        private static Preferences DefaultSettings
        {
            get
            {
                var preferences = new Preferences
                    {
                        ElementHeightDefault = 20, 
                        ElementWidthDefault = 20, 
                        DisplayItemHeightDefault = 150, 
                        DisplayItemWidthDefault = 150, 
                        DisplayWidthDefault = 640, 
                        DisplayHeightDefault = 480, 
                        OpacityDefault = .75, 
                    };
                return preferences;
            }
        }

        public Preferences Clone()
        {
            var preferences = new Preferences
                {
                    ElementHeightDefault = this.ElementHeightDefault, 
                    ElementWidthDefault = this.ElementWidthDefault, 
                    DisplayHeightDefault = this.DisplayHeightDefault, 
                    DisplayItemHeightDefault = this.DisplayItemHeightDefault, 
                    DisplayItemWidthDefault = this.DisplayItemWidthDefault, 
                    DisplayWidthDefault = this.DisplayWidthDefault, 
                    OpacityDefault = this.OpacityDefault, 
                };
            return preferences;
        }
    }
}