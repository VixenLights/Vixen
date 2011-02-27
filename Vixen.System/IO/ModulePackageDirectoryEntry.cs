using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.IO {
    class ModulePackageDirectoryEntry {
        public string FileName { get; set; }
        public byte FileLocationIndex { get; set; }
        public uint FileSize { get; set; }
        public uint EntrySize {
            get { return (uint)(FileName.Length + sizeof(byte) + sizeof(uint)); }
        }
    }
}
