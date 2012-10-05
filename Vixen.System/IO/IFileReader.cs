using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.IO {
	interface IFileReader<out T> : IFileReader
		where T : class {
		T ReadFile(string filePath);
	}

	interface IFileReader {
		object ReadFile(string filePath);
	}
}
