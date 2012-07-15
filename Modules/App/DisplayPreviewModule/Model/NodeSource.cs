namespace VixenModules.App.DisplayPreview.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Vixen.Sys;
    using VixenModules.App.DisplayPreview.Behaviors;

    public class NodeSource
    {
        private readonly ChannelNode _node;
        private IDragSource _source;

        public NodeSource(ChannelNode node)
        {
            _node = node;
            Children = _node.Children.Select(x => new NodeSource(x));
            NodeName = node.Name;
        }

        public string NodeName { get; private set; }

        public IEnumerable<NodeSource> Children { get; private set; }

        public IDragSource Source
        {
            get
            {
                return _source ?? (_source = new DragSource<NodeSource>(GetDragEffects, GetData));
            }
        }

        private static object GetData(NodeSource nodeSource)
        {
            return nodeSource._node;
        }

        private DragDropEffects GetDragEffects(NodeSource nodeSource)
        {
            return _node != null ? DragDropEffects.Move : DragDropEffects.None;
        }
    }
}
