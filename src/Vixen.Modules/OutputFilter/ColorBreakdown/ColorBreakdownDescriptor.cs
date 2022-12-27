using Vixen.Module.OutputFilter;

namespace VixenModules.OutputFilter.ColorBreakdown
{
	public class ColorBreakdownDescriptor : OutputFilterModuleDescriptorBase
	{
		private static readonly Guid _typeId = new Guid("{ab38a16f-0de1-4f6e-a8c0-ae5b20d347e0}");

		public override string TypeName
		{
			get { return "Color Breakdown"; }
		}

		public override Guid TypeId
		{
			get { return _typeId; }
		}

		public static Guid ModuleId
		{
			get { return _typeId; }
		}

		public override Type ModuleClass
		{
			get { return typeof (ColorBreakdownModule); }
		}

		public override Type ModuleDataClass
		{
			get { return typeof (ColorBreakdownData); }
		}

		public override string Author
		{
			get { return "Vixen Team"; }
		}

		public override string Description
		{
			get { return "An output filter that breaks down color intents into discrete color components."; }
		}

		public override string Version
		{
			get { return "1.0"; }
		}
	}


}
