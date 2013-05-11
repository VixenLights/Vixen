namespace VixenModules.Preview.DisplayPreview.ViewModels
{
    using VixenModules.Preview.DisplayPreview.Model;

    public class NodeEditorViewModel : ViewModelBase
    {
        public NodeEditorViewModel(NodeLayout nodeLayout)
        {
            NodeLayout = nodeLayout;
        }

        public NodeLayout NodeLayout { get; set; }
    }
}
