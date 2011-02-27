using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Vixen.IO {
    // Forward-only read.
    class ModulePackageReader {
        private ModulePackageDirectory _directory;
        private BinaryReader _reader = null;

        public ModulePackageReader(string modulePackagePath) {
            FileStream fileStream = new FileStream(modulePackagePath, FileMode.Open);
            _reader = new BinaryReader(fileStream);
            
            byte count;
            
            _directory = new ModulePackageDirectory();

            // Location table (all paths relative to binary directory).
            count = _reader.ReadByte();
            while(count-- > 0) {
                _directory.LocationTable.Add(_reader.ReadString());
            }

            // Module entry.
            _directory.ModuleEntry = ReadDirectoryEntry();

            // Other file entries.
            count = _reader.ReadByte();
            while(count-- > 0) {
                _directory.OtherEntries.Add(ReadDirectoryEntry());
            }

            // Read the module file so the reader can be positioned at the
            // other files and reading the module file won't disturb it.
            ModuleFile = ReadFile(_directory.ModuleEntry);
        }

        private ModulePackageDirectoryEntry ReadDirectoryEntry() {
            ModulePackageDirectoryEntry entry;
            entry = new ModulePackageDirectoryEntry();
            entry.FileName = _reader.ReadString();
            entry.FileLocationIndex = _reader.ReadByte();
            entry.FileSize = _reader.ReadUInt32();
            return entry;
        }

        // Assumes reader is positioned at that file.
        private ModulePackageFile ReadFile(ModulePackageDirectoryEntry entry) {
            ModulePackageFile file;
            byte[] fileBytes;

            fileBytes = new byte[entry.FileSize];
            _reader.Read(fileBytes, 0, (int)entry.FileSize);
            file = new ModulePackageFile();
            file.FileName = entry.FileName;
            file.FileLocation = _directory.LocationTable[entry.FileLocationIndex];
            file.FileContents = fileBytes;
            return file;
        }

        public ModulePackageFile ModuleFile { get; private set; }

        // Forward-only read.
        public IEnumerable<ModulePackageFile> GetCommonFiles() {
            foreach(ModulePackageDirectoryEntry entry in _directory) {
                yield return ReadFile(entry);
            }
        }

        ~ModulePackageReader() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if(_reader != null) {
                _reader.Close();
            }
        }
    }
}
