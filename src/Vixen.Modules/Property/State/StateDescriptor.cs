using Vixen.Module.Property;

namespace VixenModules.Property.State {
	public	class StateDescriptor  : PropertyModuleDescriptorBase
	{
		public static Guid Id = new Guid("{35B893C3-D10C-4359-82BC-929C0762E809}");

		public override string TypeName => "State";

		public override Guid TypeId => Id;

		public static Guid ModuleId => Id;

		public override Type ModuleClass => typeof (StateModule);

		public override string Author => "Jeff Uchitjil";

		public override string Description => "Provides mechanism for assigning State mapping of display elements";

		public override string Version => "1.0";

		public override Type ModuleDataClass => typeof (StateData);
	}
}

