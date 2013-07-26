using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Common.Controls.Timeline
{
	public class ElementDrawQueue
	{
		ConcurrentQueue<Element> _elements = new ConcurrentQueue<Element>();

		public void Add(Element element)
		{
			if (!_elements.Contains(element))
			{
				_elements.Enqueue(element);
			}
		}

		public int Count()
		{
			return _elements.Count();
		}

		public Element Next
		{
			get
			{
				Element element;
				if (_elements.TryDequeue(out element))
				{
					return element;
				}
				else
				{
					return null;
				}
			}
		}

		public bool TryNext(out Element element)
		{
			element = Next;
			if (element != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public void Clear() 
		{
			Element element;
			while (_elements.Count > 0)
				_elements.TryDequeue(out element);
		}
	}
}
