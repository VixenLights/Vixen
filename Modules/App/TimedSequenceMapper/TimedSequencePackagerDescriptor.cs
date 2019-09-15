using System;
using Vixen.Module.App;

namespace VixenModules.App.TimedSequenceMapper
{
	public class TimedSequencePackagerDescriptor : AppModuleDescriptorBase
	{
		public override string TypeName => "TimedSequencePackager";

		public override Guid TypeId { get; } = new Guid("{B13997C2-D4B5-4B5A-9047-1DCED97AD99B}");

		public override string Author => "Jeff Uchitjil";

		public override string Description => "Import / Export / Map Timed Sequences between profiles.";

		public override string Version => "1.0";

		public override Type ModuleClass => typeof(TimedSequencePackagerModule);

		public override Type ModuleStaticDataClass => typeof(TimedSequencePackagerData);
	}
}