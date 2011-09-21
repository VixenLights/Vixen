using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CommonElements.Timeline;
using Vixen.Module.Editor;
using Vixen.Sys;

namespace VixenModules.Editor.TimedSequenceEditor
{
	class TimedSequenceElement : TimelineElement
	{
		public TimedSequenceElement(CommandNode commandNode)
		{
			StartTime = commandNode.StartTime;
			Duration = commandNode.TimeSpan;
			CommandNode = commandNode;
		}


		public CommandNode CommandNode { get; set; }
	}
}
