using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Module.Sequence;

// This is the same for either language, but a copy needs to be in each namespace
// because they are part of the partial class completed by the T4 class.

namespace Vixen.Script.VB {
	public partial class Skeleton : IScriptSkeletonGenerator {
		private ScriptSequenceBase _sequence;

		public Skeleton() {
			Namespace = ScriptHostGenerator.UserScriptNamespace;
		}

		public Skeleton(ScriptSequenceBase sequence)
			: this() {
			Sequence = sequence;
		}

		public ScriptSequenceBase Sequence {
			get { return _sequence; }
			set {
				_sequence = value;
				ClassName = ScriptHostGenerator.Mangle(_sequence.Name);
			}
		}

		public string Namespace { get; private set; }

		public string ClassName { get; private set; }
	}
}
