using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Vixen.Data.Flow;
using Vixen.Pool;
using Vixen.Sys.Instrumentation;

namespace Vixen.Sys.Managers
{
	public class ElementManager : IEnumerable<Element>
	{
		private readonly MillisecondsValue _elementUpdateTimeValue = new MillisecondsValue("   Elements update");
		private readonly Stopwatch _stopwatch = Stopwatch.StartNew();
		private readonly ElementDataFlowAdapterFactory _dataFlowAdapters;
		private static readonly NLog.Logger Logging = NLog.LogManager.GetCurrentClassLogger();

		// a mapping of element  GUIDs to element instances. Used for quick reverse mapping at runtime.
		//This was a ConcurrentDictionary for a while, but grabing an instance enumerator can be costly as it makes a read only copy
		//General locking on a Dictionary is sufficient here and more performant for iterating which is the highest traffic
		private readonly Dictionary<Guid, Element> _instances;

		// a mapping of elements back to their containing element nodes. Used in a few special cases, particularly for runtime, so we can
		// quickly and easily find the node that a particular element references (eg. if we're previewing the rendered data on a virtual display,
		// or anything else where we need to actually 'reverse' the rendering process).
		private readonly ConcurrentDictionary<Element, ElementNode> _elementToElementNode;
		private Enumerator<Element> _enumerator;
		private bool _enumeratorInvalid = true;
		
		public ElementManager()
		{
			_instances = new Dictionary<Guid, Element>();
			_elementToElementNode = new ConcurrentDictionary<Element, ElementNode>();
			_dataFlowAdapters = new ElementDataFlowAdapterFactory();

			VixenSystem.Instrumentation.AddValue(_elementUpdateTimeValue);
			 
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
					_enumeratorInvalid = true;
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
			ElementNode en;
			lock (_instances)
			{
				_instances.Remove(element.Id);
				_enumeratorInvalid = true;
			}
			
			_RemoveDataFlowParticipant(element);
			_elementToElementNode.TryRemove(element, out en);
		}

		public Element GetElement(Guid id)
		{
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
		}

		public bool ElementsHaveState { get; set; }

		public void Update()
		{
			//Need to profile and see if parallelism here will improve this
			//At small element counts it is probably unneeded overhead, but at very large counts it may help.
			_stopwatch.Restart();
			lock (_instances)
			{
				Parallel.ForEach(_instances.Values, x =>
				{
					x.Update();
				});
			}
			ElementsHaveState = true;
			_elementUpdateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
		}

		public void ClearStates()
		{
			_stopwatch.Restart();
			lock (_instances)
			{
				Parallel.ForEach(_instances.Values, x =>
				{
					x.ClearStates();
				});
			}
			ElementsHaveState = false;
			_elementUpdateTimeValue.Set(_stopwatch.ElapsedMilliseconds);
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
					unique = _instances.Values.All(x => x.Name != name);
				} while (!unique);
			}
			return name;
		}

		IEnumerator<Element> IEnumerable<Element>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public Enumerator<Element> GetEnumerator()
		{
			if (_enumeratorInvalid)
			{
				_enumerator = new Enumerator<Element>(_instances.Values.ToList());
				_enumeratorInvalid = false;
			}
			return _enumerator;
		}

		public struct Enumerator<TElement> : IEnumerator<TElement>
		{
			int _nIndex;
			readonly List<TElement> _collection;
			internal Enumerator(List<TElement> coll)
			{
				_collection = coll;
				_nIndex = -1;
			}

			public void Reset()
			{
				_nIndex = -1;
			}

			public bool MoveNext()
			{
				_nIndex++;
				return (_nIndex < _collection.Count);
			}

			public TElement Current
			{
				get
				{
					return (_collection[_nIndex]);
				}
			}

			// The current property on the IEnumerator interface:
			object IEnumerator.Current
			{
				get
				{
					return (Current);
				}
			}

			public void Dispose()
			{

			}

		}
	}
}