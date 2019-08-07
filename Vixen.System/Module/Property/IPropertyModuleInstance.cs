using System.Collections.Generic;
using Vixen.Rule;
using Vixen.Sys;

namespace Vixen.Module.Property
{
	public interface IPropertyModuleInstance : IProperty, IModuleInstance
	{
		bool HasElementSetupHelper { get; }

		bool SetupElements(IEnumerable<IElementNode> nodes);
	}
}