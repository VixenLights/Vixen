﻿using Vixen.Module.Editor;
using VixenModules.Sequence.Timed;

namespace VixenModules.Editor.TimedSequenceEditor
{
	internal class TimedSequenceEditorDescriptor : EditorModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{d342eedd-ae39-4b30-b557-b9329e6d3a7c}");
		internal static Guid _timedSequenceId = new Guid("{296bdba2-9bf3-4bff-a9f2-13efac5c8ecb}");
		internal static Guid _audioMediaId = new Guid("{fe460392-3fab-4c63-99dd-d76c48354150}");

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
			get { return typeof (TimedSequenceEditor); }
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
			get { return "1.0"; }
		}

		public override Type EditorUserInterfaceClass
		{
			get { return typeof (TimedSequenceEditorForm); }
		}

		public override Type SequenceType
		{
			get { return typeof (TimedSequence); }
		}

		public override Guid[] Dependencies
		{
			get { return new Guid[] {_timedSequenceId, _audioMediaId}; }
		}
	}
}