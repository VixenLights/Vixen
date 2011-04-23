using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.IO {
	public interface IReader {
		object Read(string filePath);
	}
}
