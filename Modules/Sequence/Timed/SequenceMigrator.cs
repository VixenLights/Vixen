using System.Collections.Generic;
using System.Linq;
using Vixen.IO;
using Vixen.IO.Result;

namespace VixenModules.Sequence.Timed {
	//*** make sure the sequence type module for .tim bumps the version from 2 to 3 and moves the length out of an attribute
	//1 -> 2 = ?
	//2 -> 3 = conversion to base data model without attributes
	//*** would have to create an instance and reserialize it to get it correctly formatted
	public class SequenceMigrator : EmptyMigrator {
	}
}
