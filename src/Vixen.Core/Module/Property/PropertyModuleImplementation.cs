﻿using Vixen.Sys.Attribute;

namespace Vixen.Module.Property
{
	[TypeOfModule("Property")]
	internal class PropertyModuleImplementation : ModuleImplementation<IPropertyModuleInstance>
	{
		public PropertyModuleImplementation()
			: base(new PropertyModuleManagement(), new PropertyModuleRepository())
		{
		}
	}
}