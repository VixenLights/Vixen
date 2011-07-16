using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vixen.Script;
using Vixen.Sys;

namespace TestScript {
	public partial class VB_Skeleton : IScriptSkeletonGenerator {
		public string Generate(string nameSpace, string className) {
			Namespace = nameSpace;
			ClassName = className;
			return TransformText();
		}

		public string Namespace { get; set; }

		public string ClassName { get; set; }
	}
}
