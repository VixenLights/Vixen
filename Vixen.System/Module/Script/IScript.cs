namespace Vixen.Module.Script {
	public interface IScript {
		IScriptSkeletonGenerator SkeletonGenerator { get; set; }
		IScriptFrameworkGenerator FrameworkGenerator { get; set; }
		IScriptCodeProvider CodeProvider { get; set; }
		string FileExtension { get; }
	}
}
