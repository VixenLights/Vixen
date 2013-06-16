using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Vixen.Data.Flow;
using Vixen.Sys.Instrumentation;

namespace Vixen.Sys.Managers {
	public class ElementManager : IEnumerable<Element> {
		private ElementUpdateTimeValue _elementUpdateTimeValue;
		private Stopwatch _stopwatch;
		private ElementDataFlowAdapterFactory _dataFlowAdapters;

		// a mapping of element  GUIDs to element instances. Used for quick reverse mapping at runtime.
		private Dictionary<Guid, Element> _instances;

		// a mapping of elements back to their containing element nodes. Used in a few special cases, particularly for runtime, so we can
		// quickly and easily find the node that a particular element references (eg. if we're previewing the rendered data on a virtual display,
		// or anything else where we need to actually 'reverse' the rendering process).
		private Dictionary<Element, ElementNode> _elementToElementNode;

		public ElementManager() {
			_instances = new Dictionary<Guid,Element>();
			_elementToElementNode = new Dictionary<Element, ElementNode>();
			_SetupInstrumentation();
			_dataFlowAdapters = new ElementDataFlowAdapterFactory();
		}

		public ElementManager(IEnumerable<Element> elements)
			: this() {
			AddElements(elements);
		}

		public Element AddElement(string elementName) {
			elementName = _Uniquify(elementName);
			Element element = new Element(elementName);
			AddElement(element);
			return element;
		}

		public void AddElement(Element element) {
			if(element != null) {
				if(_instances.ContainsKey(element.Id))
					VixenSystem.Logging.Error("ElementManager: Adding a element, but it's already in the instance map!");

				lock(_instances) {
					_instances[element.Id] = element;
				}

				_AddDataFlowParticipant(element);
			}
		}

		public void AddElements(IEnumerable<Element> elements) {
			foreach(Element element in elements) {
				AddElement(element);
			}
		}

		public void RemoveElement(Element element) {
			lock(_instances) {
				_instances.Remove(element.Id);
			}

			_RemoveDataFlowParticipant(element);

			if (_elementToElementNode.ContainsKey(element)) {
				_elementToElementNode.Remove(element);
			}
		}

		public Element GetElement(Guid id) {
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

		public void Update() {
			lock(_instances) {
				_stopwatch.Restart();

				_instances.Values.AsParallel().ForAll(x => x.Update());

				_elementUpdateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
			}
		}

		private void _AddDataFlowParticipant(Element element) {
			VixenSystem.DataFlow.AddComponent(_dataFlowAdapters.GetAdapter(element));
		}

		private void _RemoveDataFlowParticipant(Element element) {
			VixenSystem.DataFlow.RemoveComponent(_dataFlowAdapters.GetAdapter(element));
		}

		public IDataFlowComponent GetDataFlowComponentForElement(Element element) {
			return _dataFlowAdapters.GetAdapter(element);
		}

		private string _Uniquify(string name) {
			if(_instances.Values.Any(x => x.Name == name)) {
				string originalName = name;
				bool unique;
				int counter = 2;
				do {
					name = originalName + "-" + counter++;
					unique = !_instances.Values.Any(x => x.Name == name);
				} while(!unique);
			}
			return name;
		}

		private void _SetupInstrumentation() {
			_elementUpdateTimeValue = new ElementUpdateTimeValue();
			VixenSystem.Instrumentation.AddValue(_elementUpdateTimeValue);
			_stopwatch = Stopwatch.StartNew();
		}

		public IEnumerator<Element> GetEnumerator()
		{
			lock(_instances) {
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
