using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Script {
	class ScriptTypeImplementation {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="language"></param>
		/// <param name="fileExtension"></param>
		/// <param name="skeletonFileGenerator">IScriptSkeletonGenerator implementation.</param>
		/// <param name="scriptFrameworkGenerator">IScriptFrameworkGenerator implementation.</param>
		/// <param name="codeProvider">ICodeProvider implementation.</param>
		public ScriptTypeImplementation(string language, string fileExtension, Type skeletonFileGenerator, Type scriptFrameworkGenerator, Type codeProvider) {
			Language = language;
			FileExtension = fileExtension;
			SkeletonFileGenerator = skeletonFileGenerator;
			ScriptFrameworkGenerator = scriptFrameworkGenerator;
			CodeProvider = codeProvider;
		}

		public string Language;
		public string FileExtension;
		public Type SkeletonFileGenerator;
		public Type ScriptFrameworkGenerator;
		public Type CodeProvider;
	}
}
