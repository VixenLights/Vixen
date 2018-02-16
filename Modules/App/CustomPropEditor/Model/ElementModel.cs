using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
    /// <summary>
    /// Symbolic of an Element in Vixen core. Defines a model element that contains 1 to many lights
    /// </summary>
    public class ElementModel : BindableBase, IEqualityComparer<ElementModel>, IEquatable<ElementModel>
    {
        private string _name;
        private int _order;

        internal ElementModel(string name)
            : this(Guid.NewGuid(), name)
        {
        }

        internal ElementModel(Guid id, string name)
        {
            Id = id;
            Name = name;
            Lights = new ObservableCollection<Light>();
        }

        public Guid Id { get; protected set; }

        public string Name
        {
            get { return _name; }
            set
            {
                if (value == _name) return;
                _name = value;
                OnPropertyChanged(nameof(Name));
            }
        }

        public ObservableCollection<Light> Lights { get; set; }

        public Light AddLight(Point center, double size)
        {
            var ln = new Light(center, size);
            Lights.Add(ln);
            OnPropertyChanged(nameof(IsString));
            OnPropertyChanged(nameof(LightCount));
            return ln;
        }

        public int Order
        {
            get { return _order; }
            set
            {
                if (value == _order) return;
                _order = value;
                OnPropertyChanged(nameof(Order));
            }
        }

        public bool IsString => Lights.Count > 1;

        public int LightCount => Lights.Count;

        public bool Equals(ElementModel x, ElementModel y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(ElementModel obj)
        {
            return obj.Id.GetHashCode();
        }

        public bool Equals(ElementModel other)
        {
            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
