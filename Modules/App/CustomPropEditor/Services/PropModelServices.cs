using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using VixenModules.App.CustomPropEditor.Model;

namespace VixenModules.App.CustomPropEditor.Services
{
    public class PropModelServices
    {
        private static PropModelServices _instance;
        private Prop _prop;
        private Dictionary<Guid, ElementModel> _models = new Dictionary<Guid, ElementModel>();

        private PropModelServices()
        {
            
        }

        public static PropModelServices Instance()
        {
            if (_instance == null)
            {
                _instance = new PropModelServices();
               // _instance.CreateProp();
            }

            return _instance;
        }

        public Prop CreateProp(string name="Default")
        {
            _prop = new Prop(name);
            _models.Clear();
            _models.Add(_prop.RootNode.Id, _prop.RootNode);
            return _prop;
        }

        public IEnumerable<ElementModel> GetLeafNodes()
        {
            return _prop.GetLeafNodes();
        }

        public ElementModel CreateNode(string name, ElementModel parent = null)
        {
            if (parent == null)
            {
                parent = _prop.RootNode;
            }
            ElementModel em = new ElementModel(Uniquify(name), parent);
            parent.AddChild(em);
            _models.Add(em.Id, em);
            return em;
        }

        public void AddLightNode(ElementModel target, Point p, int? order = null, int? size=null)
        {
            if (target == null)
            {
                target = _prop.RootNode;
            }
            else if (target.Lights.Any())
            {
                throw new ArgumentException("Cannot add light node to leaf element with lights.");
            }

            if (order == null)
            {
                order = GetNextOrder();
            }

            ElementModel em = new ElementModel(Uniquify($"{target.Name}-{order}"), order.Value, target);
            target.AddChild(em);
            _models.Add(em.Id, em);
            if (size == null)
            {
                size = em.LightSize;
            }
            em.AddLight(CreateLight(p, size.Value));
        }

        public void AddLight(ElementModel target, Point p, int? order=null)
        {
            if (target == null)
            {
                target = _prop.RootNode;
            }
            else if (target.IsLeaf && target.Parents.Any())
            {
                AddLight(target, p);
            }

            if (order == null)
            {
                order = GetNextOrder();
            }
            
            ElementModel em = new ElementModel(Uniquify($"{target.Name}-{order}"), order.Value, target);
            target.AddChild(em);
            _models.Add(em.Id, em);

            em.AddLight(CreateLight(p, em.LightSize));
            
        }

        public Prop Prop => _prop;

        private Light CreateLight(Point p, double size)
        {
            return new Light(p, size);
        }

        public bool IsNameDuplicated(string name)
        {
            return _models.Values.Count(x => x.Name == name)>1;
        }

        private string Uniquify(string name)
        {
            if (_models.Values.Any(x => x.Name == name))
            {
                string originalName = name;
                bool unique;
                int counter = 2;
                do
                {
                    name = $"{originalName} - {counter++}";
                    unique = _models.Values.All(x => x.Name != name);
                } while (!unique);
            }
            return name;
        }

        private int GetNextOrder()
        {
            return _prop.RootNode.GetLeafEnumerator().Max(x => x.Order) + 1;
        }

    }
}
