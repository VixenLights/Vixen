namespace VixenModules.App.DisplayPreview.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using Vixen.Sys;
    using VixenModules.App.DisplayPreview.Model;
    using VixenModules.App.DisplayPreview.WPF;

    public class DisplayItemEditorViewModel : ViewModelBase
    {
        private ObservableCollection<ChannelSource> _channelSources;
        private DisplayItem _displayItem;

        private ChannelLocation _selectedChannelLocation;

        public DisplayItemEditorViewModel()
        {
            var rootNodes = VixenSystem.Nodes.GetRootNodes().Select(x => new ChannelSource(x));
            ChannelSources = new ObservableCollection<ChannelSource>(rootNodes);
            RemoveChannelCommand = new RelayCommand(x => RemoveChannel(), x => CanRemoveChannel());
        }

        public ObservableCollection<ChannelLocation> ChannelLocations
        {
            get
            {
                return _displayItem.ChannelLocations;
            }
        }

        public ObservableCollection<ChannelSource> ChannelSources
        {
            get
            {
                return _channelSources;
            }

            set
            {
                _channelSources = value;
                OnPropertyChanged("ChannelSources");
            }
        }

        public DisplayItem DisplayItem
        {
            get
            {
                return _displayItem;
            }

            set
            {
                _displayItem = value;
                OnPropertyChanged("DisplayItem");
            }
        }

        public ICommand RemoveChannelCommand { get; private set; }

        public ChannelLocation SelectedChannelLocation
        {
            get
            {
                return _selectedChannelLocation;
            }

            set
            {
                _selectedChannelLocation = value;
                OnPropertyChanged("SelectedChannelLocation");
            }
        }

        private bool CanRemoveChannel()
        {
            return SelectedChannelLocation != null;
        }

        private void RemoveChannel()
        {
            ChannelLocations.Remove(SelectedChannelLocation);
        }
    }
}
