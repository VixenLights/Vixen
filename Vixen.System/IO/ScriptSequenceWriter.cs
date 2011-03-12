using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Vixen.Sys;
using System.IO;
using Vixen.Common;
using Vixen.Sequence;
using Vixen.Script;

using Vixen.Module.Sequence;

namespace Vixen.IO {
    public class ScriptSequenceWriter : SequenceWriter<ScriptSequenceBase> {
        protected override void WriteSequenceAttributes(XmlWriter writer) { }

        protected override void WriteSequenceBody(XmlWriter writer) {
			// Language
			writer.WriteElementString("Language", Sequence.Language);

            // Source files

            // Make sure source directory exists.
            string sourcePath = Path.Combine(Sequence.SourceDirectory, Sequence.Name);
            Helper.EnsureDirectory(sourcePath);

            // Write the source files and their references.
            writer.WriteStartElement("SourceFiles");
            foreach(SourceFile sourceFile in Sequence.SourceFiles) {
                sourceFile.Save(sourcePath);
                _WriteSourceFileReference(writer, sourceFile);
            }
            writer.WriteEndElement(); // SourceFiles

			// Framework assemblies
			writer.WriteStartElement("FrameworkAssemblies");
			foreach(string file in Sequence.FrameworkAssemblies) {
				_WriteAssemblyReference(writer, file);
			}
			writer.WriteEndElement(); // FrameworkAssemblies

			// External assemblies
			writer.WriteStartElement("ExternalAssemblies");
			foreach(string file in Sequence.ExternalAssemblies) {
				_WriteAssemblyReference(writer, file);
			}
			writer.WriteEndElement(); // ExternalAssemblies
		}

        private void _WriteSourceFileReference(XmlWriter writer, SourceFile sourceFile) {
            writer.WriteStartElement("SourceFile");
            writer.WriteAttributeString("name", sourceFile.Name);
            writer.WriteEndElement(); // SourceFile
        }

		private void _WriteAssemblyReference(XmlWriter writer, string fileName) {
			writer.WriteStartElement("Assembly");
			writer.WriteAttributeString("name", fileName);
			writer.WriteEndElement(); // Assembly
		}
    }
}
