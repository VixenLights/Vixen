using System;
using Vixen.Sys;

namespace Vixen.Module.Script
{
	internal class ScriptModuleRepository : GenericModuleRepository<IScriptModuleInstance>
	{
		public override IScriptModuleInstance Get(Guid id)
		{
			IScriptModuleDescriptor descriptor = Modules.GetDescriptorById<IScriptModuleDescriptor>(id);
			IScriptModuleInstance module = base.Get(id);

			module.SkeletonGenerator = (IScriptSkeletonGenerator) Activator.CreateInstance(descriptor.SkeletonGenerator);
			module.FrameworkGenerator = (IScriptFrameworkGenerator) Activator.CreateInstance(descriptor.FrameworkGenerator);
			module.CodeProvider = (IScriptCodeProvider) Activator.CreateInstance(descriptor.CodeProvider);

			return module;
		}
	}
}