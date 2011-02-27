using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace Vixen.IO {
    class ModulePackageDirectory : IEnumerable<ModulePackageDirectoryEntry> {
        public ModulePackageDirectory() {
            LocationTable = new List<string>();
            OtherEntries = new List<ModulePackageDirectoryEntry>();
        }

        public List<string> LocationTable { get; private set; }
        public ModulePackageDirectoryEntry ModuleEntry { get; set; }
        public List<ModulePackageDirectoryEntry> OtherEntries { get; private set; }

        private ModulePackageDirectoryEntry CreateEntry(ModulePackageFile file) {
            ModulePackageDirectoryEntry entry = new ModulePackageDirectoryEntry();

            int fileLocationIndex = LocationTable.IndexOf(file.FileLocation);
            if(fileLocationIndex == -1) {
                fileLocationIndex = LocationTable.Count;
                LocationTable.Add(file.FileLocation);
            }

            entry.FileLocationIndex = (byte)fileLocationIndex;
            entry.FileName = file.FileName;
            entry.FileSize = (uint)file.FileContents.Length;

            return entry;
        }

        public void SetModuleEntry(ModulePackageFile file) {
            ModuleEntry = CreateEntry(file);
        }

        public void AddFile(ModulePackageFile file) {
            OtherEntries.Add(CreateEntry(file));
        }

        public IEnumerator<ModulePackageDirectoryEntry> GetEnumerator() {
            return OtherEntries.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }
    }
}
