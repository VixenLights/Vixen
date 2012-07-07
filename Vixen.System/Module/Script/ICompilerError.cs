namespace Vixen.Module.Script {
	public interface ICompilerError {
		int Column { get; set; }
		string ErrorNumber { get; set; }
		string ErrorText { get; set; }
		string FileName { get; set; }
		bool IsWarning { get; set; }
		int Line { get; set; }
	}
}
