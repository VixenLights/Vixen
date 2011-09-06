using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public interface IPackageFileContent {
		string FilePath { get; }
		byte[] FileContent { get; }
	}
}
