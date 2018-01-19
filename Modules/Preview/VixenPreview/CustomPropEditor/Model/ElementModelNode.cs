using System.Collections.Generic;
using Vixen.Sys;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.Model
{
	public class ElementModelNode: GroupNode<ElementModel>
	{
		public ElementModelNode(string name, IEnumerable<GroupNode<ElementModel>> content) : base(name, content)
		{
		}

		public ElementModelNode(string name, params GroupNode<ElementModel>[] content) : base(name, content)
		{
		}
	}
}
