using System.Drawing;

namespace VixenModules.App.CustomPropEditor.Import.XLights.Ranges
{
    public class NodeRange : RangeGroup
    {
        /// <inheritdoc />
        public NodeRange(List<Range> ranges) : base(ranges)
        {
        }

        public Color Color { get; set; }

        public string Name { get; set; }
    }
}