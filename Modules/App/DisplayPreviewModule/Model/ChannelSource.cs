namespace Vixen.Modules.DisplayPreviewModule.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Vixen.Modules.DisplayPreviewModule.Behaviors;
    using Vixen.Sys;

    public class ChannelSource
    {
        private IDragSource _source;

        public ChannelSource(ChannelNode channelNode)
        {
            ChannelNode = channelNode;
            Children = ChannelNode.Children.Select(x => new ChannelSource(x));
            ChannelNodeName = channelNode.Name;
        }

        public ChannelNode ChannelNode { get; private set; }

        public string ChannelNodeName { get; private set; }

        public IEnumerable<ChannelSource> Children { get; private set; }

        public IDragSource Source
        {
            get
            {
                return _source ?? (_source = new DragSource<ChannelSource>(GetDragEffects, GetData));
            }
        }

        private object GetData(ChannelSource channelSource)
        {
            return channelSource.ChannelNode;
        }

        private DragDropEffects GetDragEffects(ChannelSource channelSource)
        {
            return ChannelNode != null ? DragDropEffects.Move : DragDropEffects.None;
        }
    }
}
