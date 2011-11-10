namespace VixenModules.App.DisplayPreview.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using VixenModules.App.DisplayPreview.Model;
    using Vixen.Sys;

    public class DisplayItemEditorViewModel : ViewModelBase
    {
        private ObservableCollection<ChannelSource> _channelSources;
        private DisplayItem _displayItem;

        public DisplayItemEditorViewModel()
        {
            var rootNodes = VixenSystem.Nodes.GetRootNodes().Select(x => new ChannelSource(x));
            ChannelSources = new ObservableCollection<ChannelSource>(rootNodes);
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
    }
}
