using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Sys {
	public interface ITypedDataMover {
		string TypeOfData { get; }
		bool CanHandle(object obj);
		void Copy(object source, object destination, bool overwrite);
		void Move(object source, object destination, bool overwrite);
	}
}
