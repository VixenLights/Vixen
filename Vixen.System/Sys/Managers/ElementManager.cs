using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vixen.Data.Flow;
using Vixen.Sys.Instrumentation;

namespace Vixen.Sys.Managers
{
	public class ElementManager : IEnumerable<Element>
	{
		private MillisecondsValue _elementUpdateTimeValue = new MillisecondsValue("   Elements update ms");
		private MillisecondsValue _elementUpdateWaitValue = new MillisecondsValue("   Elements wait ms");
		private MillisecondsValue _elementClearTimeValue = new MillisecondsValue("  Elements clear ms");
		private Stopwatch _stopwatch = Stopwatch.StartNew();
		private ElementDataFlowAdapterFactory _dataFlowAdapters;
		private static NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		// a mapping of element  GUIDs to element instances. Used for quick reverse mapping at runtime.
		private ConcurrentDictionary<Guid, Element> _instances;

		// a mapping of elements back to their containing element nodes. Used in a few special cases, particularly for runtime, so we can
		// quickly and easily find the node that a particular element references (eg. if we're previewing the rendered data on a virtual display,
		// or anything else where we need to actually 'reverse' the rendering process).
		private ConcurrentDictionary<Element, ElementNode> _elementToElementNode;

		public ElementManager()
		{
			_instances = new ConcurrentDictionary<Guid, Element>();
			_elementToElementNode = new ConcurrentDictionary<Element, ElementNode>();
			_dataFlowAdapters = new ElementDataFlowAdapterFactory();

			VixenSystem.Instrumentation.AddValue(_elementUpdateTimeValue);
			//VixenSystem.Instrumentation.AddValue(_elementUpdateWaitValue);
			//VixenSystem.Instrumentation.AddValue(_elementClearTimeValue);
		}

		public ElementManager(IEnumerable<Element> elements)
			: this()
		{
			AddElements(elements);
		}

		public Element AddElement(string elementName)
		{
			elementName = _Uniquify(elementName);
			Element element = new Element(elementName);
			AddElement(element);
			return element;
		}

		public void AddElement(Element element)
		{
			if (element != null) {
				if (_instances.ContainsKey(element.Id))
					Logging.Error("ElementManager: Adding a element, but it's already in the instance map!");

				lock (_instances) {
					_instances[element.Id] = element;
				}

				_AddDataFlowParticipant(element);
			}
		}

		public void AddElements(IEnumerable<Element> elements)
		{
			foreach (Element element in elements) {
				AddElement(element);
			}
		}

		public void RemoveElement(Element element)
		{
			Element e;
			ElementNode en;
			//lock (_instances) {
			//_instances.Remove(element.Id);

			_instances.TryRemove(element.Id, out e);

			_RemoveDataFlowParticipant(element);

			_elementToElementNode.TryRemove(element, out en);
			//if (_elementToElementNode.ContainsKey(element)) {
			//	_elementToElementNode.Remove(element);
			//}
		}

		public Element GetElement(Guid id)
		{
			//if (_instances.ContainsKey(id)) {
			//    return _instances[id];
			//}
			//return null;
			Element element;
			_instances.TryGetValue(id, out element);
			return element;
		}

		public bool SetElementNodeForElement(Element element, ElementNode node)
		{
			if (element == null)
				return false;

			bool rv = _elementToElementNode.ContainsKey(element);

			_elementToElementNode[element] = node;
			return rv;
		}

		public ElementNode GetElementNodeForElement(Element element)
		{
			if (element == null)
				return null;

			ElementNode node;
			_elementToElementNode.TryGetValue(element, out node);
			return node;
			//if (_elementToElementNode.ContainsKey(element))
			//    return _elementToElementNode[element];

			//return null;
		}

		public void Update()
		{
			_stopwatch.Restart();
			lock (_instances)
			{
				_elementUpdateWaitValue.Set(_stopwatch.ElapsedMilliseconds);

				//_instances.Values.AsParallel().ForAll(x => x.Update());
				foreach( var x in _instances.Values) x.Update();

				_elementUpdateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
			 }
		}

		public void ClearStates()
		{
			_stopwatch.Restart();
			lock (_instances)
			{
				//_instances.Values.AsParallel().ForAll(x => x.Update());
				foreach (var x in _instances.Values) x.ClearStates();

				_elementClearTimeValue.Set(_stopwatch.ElapsedMilliseconds);
			}
		}

		private void _AddDataFlowParticipant(Element element)
		{
			VixenSystem.DataFlow.AddComponent(_dataFlowAdapters.GetAdapter(element));
		}

		private void _RemoveDataFlowParticipant(Element element)
		{
			VixenSystem.DataFlow.RemoveComponent(_dataFlowAdapters.GetAdapter(element));
		}

		public IDataFlowComponent GetDataFlowComponentForElement(Element element)
		{
			return _dataFlowAdapters.GetAdapter(element);
		}

		private string _Uniquify(string name)
		{
			if (_instances.Values.Any(x => x.Name == name)) {
				string originalName = name;
				bool unique;
				int counter = 2;
				do {
					name = string.Format("{0}-{1}", originalName, counter++);
					unique = !_instances.Values.Any(x => x.Name == name);
				} while (!unique);
			}
			return name;
		}

		public IEnumerator<Element> GetEnumerator()
		{
			lock (_instances) {
				Element[] elements = _instances.Values.ToArray();
				return ((IEnumerable<Element>)elements).GetEnumerator();
			}
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}