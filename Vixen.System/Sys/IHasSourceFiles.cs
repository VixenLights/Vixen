using System.Collections.Generic;
using Vixen.Script;

namespace Vixen.Sys {
	public interface IHasSourceFiles {
		void AddSourceFile(SourceFile sourceFile);
		void AddSourceFiles(IEnumerable<SourceFile> sourceFiles);
		bool RemoveSourceFile(SourceFile sourceFile);
		IEnumerable<SourceFile> GetAllSourceFiles();
		void ClearSourceFiles();
		string SourceFileDirectory { get; set; }
	}
}
