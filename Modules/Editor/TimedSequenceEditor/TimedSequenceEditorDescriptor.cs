using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;
using Vixen.Module.Editor;

namespace TimedSequenceEditor
{
	class TimedSequenceEditorDescriptor : EditorModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{d342eedd-ae39-4b30-b557-b9329e6d3a7c}");
		private string[] _extensions = new string[] { ".tim" };
		private Guid[] _dependencies = new Guid[] { new Guid("{4C258A3B-E725-4AE7-B50B-103F6AB8121E}") };

		public override string TypeName
		{
			get { return "Timed Sequence Editor"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(TimedSequenceEditor); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof(TimedSequenceEditorDataModel); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "An editor for a standard timed sequence"; }
		}

		public override string Version
		{
			get { return "0.0.1"; }
		}

		public override string[] FileExtensions
		{
			get { return _extensions; }
		}

		public override Guid[] Dependencies
		{
			get { return _dependencies; }
		}
	}
}
