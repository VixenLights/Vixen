using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
//using Vixen.Sequence;
using Vixen.Sys;
using Vixen.Common;
using Vixen.Script;

using Vixen.Module.Sequence;

namespace Vixen.IO {
    public class ScriptSequenceReader : SequenceReader<ScriptSequenceBase> {
        protected override void ReadSequenceAttributes(XmlReader reader, ScriptSequenceBase sequence) { }

        protected override void ReadSequenceBody(XmlReader reader, ScriptSequenceBase sequence) {
			string fileName;
			
			// Source files

            // Read the source files and their references.
            sequence.SourceFiles.Clear();
            if(reader.ElementsExistWithin("SourceFiles")) { // Container element for child entity
                string sourcePath = Path.Combine(Sequence.SourceDirectory, Sequence.Name);
                while((fileName = reader.GetAttribute("name")) != null) {
                    fileName = Path.Combine(sourcePath, fileName);
                    sequence.SourceFiles.Add(SourceFile.Load(fileName));
                    reader.ReadStartElement("SourceFile");
                }
                reader.ReadEndElement(); // SourceFiles
            }

			// Framework assemblies
			sequence.FrameworkAssemblies.Clear();
			if(reader.ElementsExistWithin("FrameworkAssemblies")) {
				while((fileName = reader.GetAttribute("name")) != null) {
					sequence.FrameworkAssemblies.Add(fileName);
					reader.ReadStartElement("Assembly");
				}
				reader.ReadEndElement(); // FrameworkAssemblies
			}

			// External assemblies
			sequence.ExternalAssemblies.Clear();
			if(reader.ElementsExistWithin("ExternalAssemblies")) {
				while((fileName = reader.GetAttribute("name")) != null) {
					sequence.ExternalAssemblies.Add(fileName);
					reader.ReadStartElement("Assembly");
				}
				reader.ReadEndElement(); // FrameworkAssemblies
			}
		}
    }
}
