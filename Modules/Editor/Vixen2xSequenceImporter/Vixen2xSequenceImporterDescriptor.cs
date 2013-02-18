using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Editor;
using VixenModules.Editor.TimedSequenceEditor;

namespace VixenModules.Editor.Vixen2xSequenceImporter
{
	class Vixen2xSequenceImporterDescriptor : EditorModuleDescriptorBase
	{
		private static Guid _typeId = new Guid("{f61cc3c0-de29-49cf-aa01-ef01cc205b7d}");
		internal static Guid _importedSequenceId = new Guid("{92BBD2CB-B750-437F-8A88-49864D569AB4}");
		internal static Guid _audioMediaId = new Guid("{fe460392-3fab-4c63-99dd-d76c48354150}");
		private static string[] _extensions = new string[] { ".tim", ".vix" }; // .tim must be first to save properly

		public override string TypeName
		{
			get { return "Vixen2.x Importer Editor"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof(Vixen2xSequenceImporter); }
		}

		public override string Author
		{
			get { return "John McAdams"; }
		}

		public override string Description
		{
			get { return "The UI portion of a Vixen 2.x import"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override string[] FileExtensions
		{
			get { return _extensions; }
		}

		public override Type EditorUserInterfaceClass
		{
			get { return typeof(TimedSequenceEditorForm); }
		}

		public override Guid[] Dependencies
		{
			get { return new Guid[] { _audioMediaId }; }
		}
	}
}
