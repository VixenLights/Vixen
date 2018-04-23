using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Controls.Timeline;
using VixenModules.Sequence.Timed;

namespace VixenModules.Editor.TimedSequenceEditor.Controls
{
	public sealed class MarksBar:TimelineControlBase
	{
		/// <inheritdoc />
		public MarksBar(TimeInfo timeinfo) : base(timeinfo)
		{
		}

		public List<MarkCollection> MarkCollection { get; set; }
	}
}
