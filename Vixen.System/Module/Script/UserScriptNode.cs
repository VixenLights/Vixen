using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using Vixen.Sys;

namespace Vixen.Module.Script
{
	// Public for script frameworks' usage.
	public class UserScriptNode : DynamicObject, IEnumerable<UserScriptNode>
	{
		// Node name : Node (branch or leaf)
		private Dictionary<string, UserScriptNode> _children = new Dictionary<string, UserScriptNode>();

		public UserScriptNode(ElementNode node)
		{
			Node = node;
			_children = node.Children.ToDictionary(x => x.Name, x => new UserScriptNode(x));
		}

		public ElementNode Node { get; private set; }

		public IEnumerator<UserScriptNode> GetEnumerator()
		{
			yield return this;
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}