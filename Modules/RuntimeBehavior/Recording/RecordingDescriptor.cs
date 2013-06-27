using System;
using Vixen.Module.RuntimeBehavior;

namespace Recording
{
	public class RecordingDescriptor : RuntimeBehaviorModuleDescriptorBase
	{
		private Guid _typeId = new Guid("{BEF934C3-B7C1-418f-8B25-CCD0566161FA}");

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (RecordingModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (RecordingData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string TypeName
		{
			get { return "Recording"; }
		}

		public override string Description
		{
			get { return "Provides recording functionality for sequences"; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}
}