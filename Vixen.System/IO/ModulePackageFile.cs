using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.IO {
    /// <summary>
    /// A file contained within the package.
    /// </summary>
    class ModulePackageFile {
        public string FileName { get; set; }
        public string FileLocation { get; set; }
        public byte[] FileContents { get; set; }
    }
}
