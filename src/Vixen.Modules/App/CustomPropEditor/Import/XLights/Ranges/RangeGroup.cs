namespace VixenModules.App.CustomPropEditor.Import.XLights.Ranges
{
    public class RangeGroup
    {
        public RangeGroup(List<Range> ranges)
        {
            Ranges = ranges;
        }

        public List<Range> Ranges { get; set; }
    }
}