using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Vixen.Script {
    public class SourceFile {
        public SourceFile(string name) {
            Name = name;
        }

        static public SourceFile Load(string fileName) {
            if(File.Exists(fileName)) {
                SourceFile sourceFile = new SourceFile(Path.GetFileName(fileName));
                sourceFile.Contents = File.ReadAllText(fileName);
                return sourceFile;
            }
            throw new ArgumentException("File does not exist.");
        }

        public string Name { get; set; }

        public string Contents { get; set; }

        public void Save(string directory) {
            File.WriteAllText(Path.Combine(directory, Name), Contents);
        }
    }
}
