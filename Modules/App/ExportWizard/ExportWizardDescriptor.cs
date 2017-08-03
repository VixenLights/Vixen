using System;
using Vixen.Module.App;

namespace VixenModules.App.ExportWizard
{
	public class ExportWizardDescriptor : AppModuleDescriptorBase
	{
		private readonly Guid _typeId = new Guid("{8BF8D0C5-B7D6-417C-9EFF-EC1C5E1EF9AB}");

		public override string TypeName
		{
			get { return "Export Wizard"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override string Author
		{
			get { return "Jeff Uchitjil"; }
		}

		public override string Description
		{
			get { return "Wizard to export sequences to other formats."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}

		public override Type ModuleClass
		{
			get { return typeof (ExportWizardModule); }
		}

		public override Type ModuleStaticDataClass
		{
			get { return typeof(BulkExportWizardData); }
		}
	}
}