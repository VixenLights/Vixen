using System;
using System.Drawing;

namespace Timeline
{
    public class TimelineElement
    {
        public TimelineElement()
        {
        }

        public TimelineRow Row { get; set; }

        public TimeSpan Offset { get; set; }
        public TimeSpan Duration { get; set; }
        public Color BackColor { get; set; }

        public object Tag { get; set; }
    }
}