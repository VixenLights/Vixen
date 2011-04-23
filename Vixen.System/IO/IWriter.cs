using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.IO {
	public interface IWriter {
		void Write(string filePath, object value);
	}
}
