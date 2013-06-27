using System;
using Vixen.Module.Editor;
using VixenModules.SequenceType.Script;

namespace VixenModules.Editor.ScriptEditor
{
	public class ScriptEditor_Descriptor : EditorModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{CEFF9B1C-BB75-4f76-96C2-C0BBADB75035}");

		private Guid[] _dependencies = new[]
		                               	{
		                               		new Guid("{CD5CA8E5-10D8-4342-9A42-AED48209C7CC}") // ScriptSequence
		                               	};

		public override string TypeName
		{
			get { return "Script editor"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (ScriptEditor_Module); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "Editor for code-based script sequences."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type EditorUserInterfaceClass
		{
			get { return typeof (ScriptEditor); }
		}

		public override Type SequenceType
		{
			get { return typeof (ScriptSequenceType); }
		}

		public override Guid[] Dependencies
		{
			get { return _dependencies; }
		}
	}
}