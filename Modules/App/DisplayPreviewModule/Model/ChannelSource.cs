namespace VixenModules.App.DisplayPreview.Model
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using Vixen.Sys;
    using VixenModules.App.DisplayPreview.Behaviors;

    public class ChannelSource
    {
        private readonly ChannelNode _channelNode;
        private IDragSource _source;

        public ChannelSource(ChannelNode channelNode)
        {
            _channelNode = channelNode;
            Children = _channelNode.Children.Select(x => new ChannelSource(x));
            ChannelNodeName = channelNode.Name;
        }

        public string ChannelNodeName { get; private set; }

        public IEnumerable<ChannelSource> Children { get; private set; }

        public IDragSource Source
        {
            get
            {
                return _source ?? (_source = new DragSource<ChannelSource>(GetDragEffects, GetData));
            }
        }

        private static object GetData(ChannelSource channelSource)
        {
            return channelSource._channelNode;
        }

        private DragDropEffects GetDragEffects(ChannelSource channelSource)
        {
            return _channelNode != null ? DragDropEffects.Move : DragDropEffects.None;
        }
    }
}
