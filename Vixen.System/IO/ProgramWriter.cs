using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Vixen.Sys;

namespace Vixen.IO {
    class ProgramWriter {
        public void Write(string filePath) {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using(XmlWriter writer = XmlWriter.Create(filePath, settings)) {

                if(Program != null) {
                    writer.WriteStartElement("Program");

                    // Attributes go here...
                    writer.WriteAttributeString("name", Program.Name);

                    // Sequence references
                    writer.WriteStartElement("Sequences");
					foreach(ISequence sequence in Program.Sequences)
					{
                        writer.WriteStartElement("Sequence");
                        writer.WriteAttributeString("fileName", sequence.Name.ToString());
                        writer.WriteEndElement(); // Sequence
                    }
                    writer.WriteEndElement(); // Sequences

                    writer.WriteEndElement(); // Program
                }
            }
        }

        public Program Program { get; set; }
    }
}
