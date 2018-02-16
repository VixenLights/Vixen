using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace VixenModules.App.CustomPropEditor.Model
{
    public class ElementModelManager : IEnumerable<ElementModel>
    {
        private readonly Dictionary<Guid, ElementModel> _instances;

        // a mapping of ElementModels back to their containing ElementModel nodes. Used in a few special cases, particularly for runtime, so we can
        // quickly and easily find the node that a particular ElementModel references 
        // or anything else where we need to actually 'reverse' the rendering process).
        private readonly ConcurrentDictionary<ElementModel, ElementModelNode> _elementToElementNode;
        private Enumerator<ElementModel> _enumerator;
        private bool _enumeratorInvalid = true;

        public ElementModelManager()
        {
            _instances = new Dictionary<Guid, ElementModel>();
            _elementToElementNode = new ConcurrentDictionary<ElementModel, ElementModelNode>();
        }

        public ElementModelManager(IEnumerable<ElementModel> elements)
            : this()
        {
            AddElements(elements);
        }

        public ElementModel AddElement(string elementName)
        {
            elementName = _Uniquify(elementName);
            ElementModel ElementModel = new ElementModel(elementName);
            AddElement(ElementModel);
            return ElementModel;
        }

        public void AddElement(ElementModel ElementModel)
        {
            if (ElementModel != null)
            {
                //if (_instances.ContainsKey(ElementModel.Id))
                //    Logging.Error("ElementManager: Adding a ElementModel, but it's already in the instance map!");

                lock (_instances)
                {
                    _instances[ElementModel.Id] = ElementModel;
                    _enumeratorInvalid = true;
                }
            }
        }

        public void AddElements(IEnumerable<ElementModel> elements)
        {
            foreach (ElementModel ElementModel in elements)
            {
                AddElement(ElementModel);
            }
        }

        public void RemoveElement(ElementModel ElementModel)
        {
            ElementModelNode en;
            lock (_instances)
            {
                _instances.Remove(ElementModel.Id);
                _enumeratorInvalid = true;
            }

            _elementToElementNode.TryRemove(ElementModel, out en);
        }

        public ElementModel GetElement(Guid id)
        {
            ElementModel ElementModel;
            _instances.TryGetValue(id, out ElementModel);
            return ElementModel;
        }

        public bool SetElementNodeForElement(ElementModel ElementModel, ElementModelNode node)
        {
            if (ElementModel == null)
                return false;

            bool rv = _elementToElementNode.ContainsKey(ElementModel);

            _elementToElementNode[ElementModel] = node;
            return rv;
        }

        public ElementModelNode GetElementNodeForElement(ElementModel ElementModel)
        {
            if (ElementModel == null)
                return null;

            ElementModelNode node;
            _elementToElementNode.TryGetValue(ElementModel, out node);
            return node;
        }

        public bool ElementsHaveState { get; set; }

       

        private string _Uniquify(string name)
        {
            if (_instances.Values.Any(x => x.Name == name))
            {
                string originalName = name;
                bool unique;
                int counter = 2;
                do
                {
                    name = $"{originalName}-{counter++}";
                    unique = _instances.Values.All(x => x.Name != name);
                } while (!unique);
            }
            return name;
        }

        IEnumerator<ElementModel> IEnumerable<ElementModel>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Enumerator<ElementModel> GetEnumerator()
        {
            if (_enumeratorInvalid)
            {
                _enumerator = new Enumerator<ElementModel>(_instances.Values.ToList());
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

            public TElement Current => (_collection[_nIndex]);

            // The current property on the IEnumerator interface:
            object IEnumerator.Current => (Current);

            public void Dispose()
            {
                //Nothing to do
            }
        }
    }
}

