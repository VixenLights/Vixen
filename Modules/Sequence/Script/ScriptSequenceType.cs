using System.IO;

namespace VixenModules.SequenceType.Script {
	public class ScriptSequenceType : Common.ScriptSequence.ScriptSequence {
		public override string FilePath {
			get { return base.FilePath; }
			set {
				base.FilePath = value;
				if(SequenceData != null) {
					((ScriptData)SequenceData).SourceFileDirectory = Path.Combine(ScriptDescriptor.ScriptSourceDirectory, Name);
				}
			}
		}

		public static string Extension = ".scr";

		public override string FileExtension
		{
			get { return Extension; }
		}
	}
}
