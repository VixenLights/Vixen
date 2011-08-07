using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.IO {
	//templates
	//version migrations
	abstract class WriterBase : IWriter {
		abstract public void Write(string filePath, object value);
	}
}
