namespace VixenModules.App.DisplayPreview.ViewModels
{
    using VixenModules.App.DisplayPreview.Model;

    public class NodeEditorViewModel : ViewModelBase
    {
        public NodeEditorViewModel(NodeLayout nodeLayout)
        {
            NodeLayout = nodeLayout;
        }

        public NodeLayout NodeLayout { get; set; }
    }
}
