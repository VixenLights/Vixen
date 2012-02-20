using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Vixen.Sys {
	abstract class FilePackage : IEnumerable<IPackageFileContent> {
		protected FilePackage(Guid sourceIdentity) {
			SourceIdentity = sourceIdentity;
		}

		private List<IPackageFileContent> _files = new List<IPackageFileContent>();

		public Guid SourceIdentity { get; set; }

		public void AddFile(IPackageFileContent contextFile) {
			_files.Add(contextFile);
		}

		public byte[] GetFile(int index) {
			return _files[index].FileContent;
		}

		public int Count {
			get { return _files.Count; }
		}

		public IEnumerator<IPackageFileContent> GetEnumerator() {
			return _files.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
			return GetEnumerator();
		}
	}
}
