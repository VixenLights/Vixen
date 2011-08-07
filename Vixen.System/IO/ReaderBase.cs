using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.IO {
	//templates
	//version migrations
	abstract class ReaderBase : IReader {
		abstract public object Read(string filePath);
	}
}
