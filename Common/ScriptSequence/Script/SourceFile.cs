using System;
using System.IO;

namespace Common.ScriptSequence.Script
{
	public class SourceFile
	{
		public SourceFile(string name)
		{
			Name = name;
		}

		public static SourceFile Load(string filePath)
		{
			if (File.Exists(filePath)) {
				SourceFile sourceFile = new SourceFile(Path.GetFileName(filePath));
				sourceFile.Contents = File.ReadAllText(filePath);
				return sourceFile;
			}
			throw new ArgumentException("File does not exist.");
		}

		public string Name { get; set; }

		public string Contents { get; set; }

		public void Save(string directory)
		{
			File.WriteAllText(Path.Combine(directory, Name), Contents);
		}
	}
}