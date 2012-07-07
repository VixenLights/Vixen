using Vixen.Module.Script;

namespace VB {
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
