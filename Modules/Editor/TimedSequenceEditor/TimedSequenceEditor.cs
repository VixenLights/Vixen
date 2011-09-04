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


		#region IEditor implementation
		// these effectively all get passed through to the form.

		public override EditorValues EditorValues
		{
			get { return _form.EditorValues; }
		}

		public override bool IsModified
		{
			get { return _form.IsModified; }
		}

		public override void NewSequence()
		{
			_form.NewSequence();
		}

		public override void Refresh()
		{
			_form.RefreshSequence();
		}

		public override void Save(string filePath = null)
		{
			_form.Save(filePath);
		}

		public override ISelection Selection
		{
			get { return _form.Selection; }
		}

		public override Vixen.Sys.ISequence Sequence
		{
			get { return _form.Sequence; }
			set { _form.Sequence = value; }
		}

		#endregion


	}
}
