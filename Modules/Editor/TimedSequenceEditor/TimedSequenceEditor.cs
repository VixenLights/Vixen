using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Editor;
using System.Windows.Forms;

namespace VixenModules.Editor.TimedSequenceEditor
{
	class TimedSequenceEditor : EditorModuleInstanceBase
	{
		private TimedSequenceEditorForm _form;

		public TimedSequenceEditor()
		{
			_form = new TimedSequenceEditorForm();
		}
	}
}
