using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.Editor.TimedSequenceEditor
{

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
