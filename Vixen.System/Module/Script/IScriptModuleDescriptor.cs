using System;

namespace Vixen.Module.Script
{
	public interface IScriptModuleDescriptor : IModuleDescriptor
	{
		string LanguageName { get; }
		string FileExtension { get; }
		Type SkeletonGenerator { get; }
		Type FrameworkGenerator { get; }
		Type CodeProvider { get; }
	}
}