using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.Editor.TimedSequenceEditor
{

	public class ElementTimeChangedUndoAction : CommonElements.UndoAction
	{
		private TimedSequenceElement m_element;
		private TimeSpan m_start, m_duration;

		public ElementTimeChangedUndoAction(TimedSequenceElement element, TimeSpan oldStart, TimeSpan oldDuration)
		{
			m_element = element;
			m_start = oldStart;
			m_duration = oldDuration;
		}

		public override void Undo()
		{
			// swap values - save new vals for redo
			TimeSpan temp;
			
			temp = m_element.StartTime;
			m_element.StartTime = m_start;
			m_start = temp;

			temp = m_element.Duration;
			m_element.Duration = m_duration;
			m_duration = temp;

			base.Undo();
		}

		public override void Redo()
		{
			// restore values from before undo
			m_element.StartTime = m_start;
			m_element.Duration = m_duration;

			base.Redo();
		}
	}

    public class ElementChangedUndoAction : CommonElements.UndoAction
    {
        public ElementChangedUndoAction(TimedSequenceElement element)
        {
            
        }

        public override void Undo()
        {
            base.Undo();
        }

        public override void Redo()
        {
            base.Redo();
        }
    }
    
}
