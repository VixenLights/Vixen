using System.Collections.Generic;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.Model
{
	public class ElementModelNode: GroupNode<ElementCandidate>
	{
		public ElementModelNode(string name, IEnumerable<GroupNode<ElementCandidate>> content) : base(name, content)
		{
		}

		public ElementModelNode(string name, params GroupNode<ElementCandidate>[] content) : base(name, content)
		{
		}
	}
}
