using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Vixen.Sys;
using System.IO;
using Vixen.Common;

namespace Vixen.IO {
    class ProgramReader {
        public bool Read(string filePath) {
            using(FileStream stream = new FileStream(filePath, FileMode.Open)) {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.IgnoreWhitespace = true;
                using(XmlReader reader = XmlReader.Create(stream, settings)) {

                    try {
                        Program program = new Program();

                        reader.Read(); // Need to start with this to seed the parser.

                        if(reader.NodeType == XmlNodeType.XmlDeclaration) {
                            reader.Read();
                        }

                        //...Any attributes go here...
                        program.Name = reader.GetAttribute("name");

                        if(reader.ElementsExistWithin("Program")) { // Entity element
                            //...

                            // Sequences
                            if(reader.ElementsExistWithin("Sequences")) { // Container element for child entity
                                while(reader.NodeType == XmlNodeType.Element) {
									program.Add(Vixen.Sys.Sequence.Load(reader.GetAttribute("fileName")));
                                    // Still sitting at the Sequence element, need to pass it.
                                    reader.Skip();
                                }
                                reader.ReadEndElement(); // Sequences
                            }

                            reader.ReadEndElement(); // Program
                            this.Program = program;
                        }
                        return true;
                    } catch {
                    }
                    return false;
                }
            }
        }

        public Program Program { get; private set; }
    }
}
