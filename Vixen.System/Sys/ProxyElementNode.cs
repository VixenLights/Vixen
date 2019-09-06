using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Vixen.Sys
{
	public class ProxyElementNode:IElementNode
	{
		public ProxyElementNode(Guid id, string name)
		{
			Id = id;
			Name = string.IsNullOrEmpty(name)?"Unknown Name":name;
			Properties = new PropertyManager(this);
		}

		#region Implementation of IEnumerable

		/// <inheritdoc />
		public IEnumerator<Element> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region Implementation of IElementNode

		/// <inheritdoc />
		public Element Element { get; set; }

		/// <inheritdoc />
		public Guid Id { get; }

		/// <inheritdoc />
		public string Name { get; set; }

		/// <inheritdoc />
		public IEnumerable<IElementNode> Children => Enumerable.Empty<IElementNode>();

		/// <inheritdoc />
		public IEnumerable<IElementNode> Parents => Enumerable.Empty<IElementNode>();

		/// <inheritdoc />
		public bool IsLeaf => true;

		/// <inheritdoc />
		public int GetMaxChildDepth()
		{
			return 0;
		}

		/// <inheritdoc />
		public IEnumerable<IElementNode> GetLeafEnumerator()
		{
			return Enumerable.Empty<IElementNode>();
		}

		/// <inheritdoc />
		public IEnumerable<Element> GetElementEnumerator()
		{
			return Enumerable.Empty<Element>();
		}

		/// <inheritdoc />
		public IEnumerable<IElementNode> GetNodeEnumerator()
		{
			return Enumerable.Empty<IElementNode>();
		}

		/// <inheritdoc />
		public IEnumerable<IElementNode> GetNonLeafEnumerator()
		{
			return Enumerable.Empty<IElementNode>();
		}

		/// <inheritdoc />
		public PropertyManager Properties { get; }

		/// <inheritdoc />
		public bool IsProxy => true;

		#endregion
	}
}
