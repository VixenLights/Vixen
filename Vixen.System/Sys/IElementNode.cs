using System;
using System.Collections.Generic;

namespace Vixen.Sys
{
	public interface IElementNode : IGroupNode
	{
		Element Element { get; set; }

		Guid Id { get; }

		string Name { get; set; }

		IEnumerable<IElementNode> Children { get; }

		IEnumerable<IElementNode> Parents { get; }

		bool IsLeaf { get; }

		int GetMaxChildDepth();

		IEnumerable<IElementNode> GetLeafEnumerator();

		IEnumerable<Element> GetElementEnumerator();

		IEnumerable<IElementNode> GetNodeEnumerator();

		IEnumerable<IElementNode> GetNonLeafEnumerator();

		//IEnumerable<IElementNode> GetAllParentNodes();

		PropertyManager Properties { get; }

		bool IsProxy { get; }
	}
}