namespace Vixen.Module.RuntimeBehavior
{
	//[TypeOfModule("RuntimeBehavior")]
	internal class RuntimeBehaviorModuleImplementation : ModuleImplementation<IRuntimeBehaviorModuleInstance>
	{
		public RuntimeBehaviorModuleImplementation()
			: base(new RuntimeBehaviorModuleManagement(), new RuntimeBehaviorModuleRepository())
		{
		}
	}
}