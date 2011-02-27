using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Vixen.IO {
    class ModulePackageWriter {
        private ModulePackageFile _moduleFile = null;
        private List<ModulePackageFile> _otherFiles = new List<ModulePackageFile>();
        private BinaryWriter _writer = null;
        private ModulePackageDirectory _directory = new ModulePackageDirectory();

        public ModulePackageWriter(string modulePackagePath) {
            FileStream fileStream = new FileStream(modulePackagePath, FileMode.Create);
            _writer = new BinaryWriter(fileStream);
        }

        public void SetModuleFile(string filePath, string fileDestinationLocation) {
            _moduleFile = CreatePackageFile(filePath, fileDestinationLocation);
            _directory.SetModuleEntry(_moduleFile);
        }

        public void AddFile(string filePath, string fileDestinationLocation) {
            ModulePackageFile file = CreatePackageFile(filePath, fileDestinationLocation);
            _otherFiles.Add(file);
            _directory.AddFile(file);
        }

        private ModulePackageFile CreatePackageFile(string filePath, string fileDestinationLocation) {
            return new ModulePackageFile() {
                FileName = Path.GetFileName(filePath),
                FileLocation = fileDestinationLocation,
                FileContents = File.ReadAllBytes(filePath)
            };
        }

        public void Write() {
            // Location table (all paths relative to binary directory).
            _writer.Write((byte)_directory.LocationTable.Count);
            foreach(string location in _directory.LocationTable) {
                _writer.Write(location);
            }
            
            // Module file entry.
            WriteDirectoryEntry(_directory.ModuleEntry);
            
            // Other file entries (likely go in Common, but not required).
            _writer.Write((byte)_otherFiles.Count);
            foreach(ModulePackageDirectoryEntry entry in _directory.OtherEntries) {
                WriteDirectoryEntry(entry);
            }
            
            // Module file:
            _writer.Write(_moduleFile.FileContents);
            
            // Other files:
            foreach(ModulePackageFile file in _otherFiles) {
                _writer.Write(file.FileContents);
            }
        }

        private void WriteDirectoryEntry(ModulePackageDirectoryEntry entry) {
            _writer.Write(entry.FileName);
            _writer.Write(entry.FileLocationIndex);
            _writer.Write(entry.FileSize);
        }

        ~ModulePackageWriter() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            if(_writer != null) {
                _writer.Close();
            }
        }
    }
}
