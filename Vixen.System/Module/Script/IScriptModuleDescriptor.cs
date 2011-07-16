using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Script {
	public interface IScriptModuleDescriptor : IModuleDescriptor {
		string Language { get; }
		string FileExtension { get; }
		Type SkeletonGenerator { get; }
		Type FrameworkGenerator { get; }
		Type CodeProvider { get; }
	}
}
