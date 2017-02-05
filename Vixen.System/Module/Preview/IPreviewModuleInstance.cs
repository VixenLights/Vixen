using System;
using Vixen.Sys.Output;

namespace Vixen.Module.Preview
{
	public interface IPreviewModuleInstance : IOutputModule, IPreview
	{
		String Name { get; set; }
	}
}