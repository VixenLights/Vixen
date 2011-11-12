namespace VixenModules.App.DisplayPreview.Model
{
    using System.ComponentModel;
    using System.Runtime.Serialization;

    [DataContract]
    public class Preferences : INotifyPropertyChanged
    {
        private int _channelHeightDefault;
        private int _channelWidthDefault;

        private int _displayHeightDefault;

        private int _displayItemHeightDefault;
        private int _displayItemWidthDefault;
        private int _displayWidthDefault;
        private double _opacityDefault;
        private bool _keepVisualizerWindowOpen;

        public event PropertyChangedEventHandler PropertyChanged;

        public static Preferences DefaultSettings
        {
            get
            {
                var preferences = new Preferences
                                  {
                                      ChannelHeightDefault = 20, 
                                      ChannelWidthDefault = 20, 
                                      DisplayItemHeightDefault = 150, 
                                      DisplayItemWidthDefault = 150, 
                                      DisplayWidthDefault = 640, 
                                      DisplayHeightDefault = 480,
                                      OpacityDefault = .75,
                                      KeepVisualizerWindowOpen = false, 
                                  };
                return preferences;
            }
        }

        [DataMember]
        public int ChannelHeightDefault
        {
            get
            {
                return _channelHeightDefault;
            }

            set
            {
                _channelHeightDefault = value <= 0 ? 1 : value;
                PropertyChanged.NotifyPropertyChanged("MinChannelHeight", this);
            }
        }

        [DataMember]
        public int ChannelWidthDefault
        {
            get
            {
                return _channelWidthDefault;
            }

            set
            {
                _channelWidthDefault = value <= 0 ? 1 : value;
                PropertyChanged.NotifyPropertyChanged("MinChannelWidth", this);
            }
        }

        [DataMember]
        public int DisplayHeightDefault
        {
            get
            {
                return _displayHeightDefault;
            }

            set
            {
                _displayHeightDefault = value <= 0 ? 1 : value;
                PropertyChanged.NotifyPropertyChanged("MinDisplayHeight", this);
            }
        }

        [DataMember]
        public int DisplayItemHeightDefault
        {
            get
            {
                return _displayItemHeightDefault;
            }

            set
            {
                _displayItemHeightDefault = value <= 0 ? 1 : value;
                PropertyChanged.NotifyPropertyChanged("MinDisplayItemHeight", this);
            }
        }

        [DataMember]
        public int DisplayItemWidthDefault
        {
            get
            {
                return _displayItemWidthDefault;
            }

            set
            {
                _displayItemWidthDefault = value <= 0 ? 1 : value;
                PropertyChanged.NotifyPropertyChanged("MinDisplayItemWidth", this);
            }
        }

        [DataMember]
        public int DisplayWidthDefault
        {
            get
            {
                return _displayWidthDefault;
            }

            set
            {
                _displayWidthDefault = value <= 0 ? 1 : value;
                PropertyChanged.NotifyPropertyChanged("MinDisplayWidth", this);
            }
        }

        [DataMember]
        public double OpacityDefault
        {
            get
            {
                return _opacityDefault;
            }

            set
            {
                _opacityDefault = value < 0 ? 0 : value;
                PropertyChanged.NotifyPropertyChanged("DefaultOpacity", this);
            }
        }

        [DataMember]
        public bool KeepVisualizerWindowOpen
        {
            get
            {
                return _keepVisualizerWindowOpen;
            }

            set
            {
                _keepVisualizerWindowOpen = value;
                PropertyChanged.NotifyPropertyChanged("KeepVisualizerWindowOpen", this);
            }
        }

        public Preferences Clone()
        {
            var preferences = new Preferences
                              {                                    
                                  ChannelHeightDefault = ChannelHeightDefault, 
                                  ChannelWidthDefault = ChannelWidthDefault, 
                                  DisplayHeightDefault = DisplayHeightDefault, 
                                  DisplayItemHeightDefault = DisplayHeightDefault, 
                                  DisplayItemWidthDefault = DisplayWidthDefault, 
                                  DisplayWidthDefault = DisplayWidthDefault,
                                  OpacityDefault = OpacityDefault,
                                  KeepVisualizerWindowOpen = KeepVisualizerWindowOpen,
                              };
            return preferences;
        }
    }
}
