namespace VixenModules.Preview.DisplayPreview.ViewModels
{
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Windows.Input;
    using Vixen.Sys;
    using VixenModules.Preview.DisplayPreview.Model;
    using VixenModules.Preview.DisplayPreview.Views;
    using VixenModules.Preview.DisplayPreview.WPF;

    public class DisplayItemEditorViewModel : ViewModelBase
    {
        private ObservableCollection<NodeSource> _nodeSources;
        private DisplayItem _displayItem;
        private NodeLayout _selectedNodeLayout;

        public DisplayItemEditorViewModel()
        {
            var rootNodes = VixenSystem.Nodes.GetRootNodes().Select(x => new NodeSource(x));
            NodeSources = new ObservableCollection<NodeSource>(rootNodes);
            RemoveNodeCommand = new RelayCommand(x => RemoveNode(), x => CanRemoveNode());
            EditNodeCommand = new RelayCommand(x => EditNode(), x => CanEditNode());
        }

        public ObservableCollection<NodeLayout> NodeLayouts
        {
            get
            {
                return _displayItem.NodeLayouts;
            }
        }

        public ObservableCollection<NodeSource> NodeSources
        {
            get
            {
                return _nodeSources;
            }

            set
            {
                _nodeSources = value;
                OnPropertyChanged("NodeSources");
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

        public ICommand EditNodeCommand { get; private set; }

        public ICommand RemoveNodeCommand { get; private set; }

        public NodeLayout SelectedNodeLayout
        {
            get
            {
                return _selectedNodeLayout;
            }

            set
            {
                _selectedNodeLayout = value;
                OnPropertyChanged("SelectedNodeLayout");
            }
        }

        private bool CanEditNode()
        {
            return SelectedNodeLayout != null;
        }

        private bool CanRemoveNode()
        {
            return SelectedNodeLayout != null;
        }

        private void EditNode()
        {
            var viewModel = new NodeEditorViewModel(SelectedNodeLayout);
            var editor = new NodeEditorView { DataContext = viewModel };
            editor.ShowDialog();
        }

        private void RemoveNode()
        {
            NodeLayouts.Remove(SelectedNodeLayout);
        }
    }
}
