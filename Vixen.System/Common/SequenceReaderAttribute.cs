using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.IO;

namespace Vixen.Common {
	[AttributeUsage(AttributeTargets.Class)]
	class SequenceReaderAttribute : Attribute {
		public SequenceReaderAttribute(Type sequenceReaderClass) {
			SequenceReaderClass = sequenceReaderClass;
		}

		public Type SequenceReaderClass { get; private set; }
	}

	// I so badly want it to be this way! But alas...
	// http://stackoverflow.com/questions/294216/why-does-c-forbid-generic-attribute-types
	//
	//[AttributeUsage(AttributeTargets.Class)]
	//class SequenceReaderAttribute<T> : Attribute
	//    where T : class, IReader {
	//    public SequenceReaderAttribute() {
	//        SequenceReaderClass = typeof(T);
	//    }

	//    public Type SequenceReaderClass { get; private set; }
	//}

}
