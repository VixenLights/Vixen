using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Sys;

// This is the same for either language, but a copy needs to be in each namespace
// because they are part of the partial class completed by the T4 class.

namespace Vixen.Script.CSharp {
	public partial class Skeleton : IScriptSkeletonGenerator {
		private ScriptSequence _sequence;

		public Skeleton() {
			Namespace = ScriptHostGenerator.UserScriptNamespace;
		}

		public Skeleton(ScriptSequence sequence)
			: this() {
			Sequence = sequence;
		}

		public ScriptSequence Sequence {
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
