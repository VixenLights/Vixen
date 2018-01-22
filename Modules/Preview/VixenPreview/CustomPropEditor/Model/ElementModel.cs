using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace VixenModules.Preview.VixenPreview.CustomPropEditor.Model
{
	public class ElementModel : INotifyPropertyChanging, INotifyPropertyChanged, IEqualityComparer<ElementModel>, IEquatable<ElementModel>
	{
	    private ObservableCollection<LightNode> _lights;
	    public string Name { get; set; }

		public Guid Id { get; protected set; }

	    public ObservableCollection<LightNode> Lights
	    {
	        get { return _lights; }
	        set
	        {
	            var changing = value.Equals(_lights);
	            if (changing)
	                OnPropertyChanging(new PropertyChangingEventArgs("Location"));
	            _lights = value;
	            if (changing)
	                OnPropertyChanged(new PropertyChangedEventArgs("Location"));
            }
	    }

	    public bool IsPixel { get; set; }

		public override string ToString()
		{
			return Name;
		}

		public bool Equals(ElementModel x, ElementModel y)
		{
			return y != null && x != null && x.Id == y.Id;
		}

		public int GetHashCode(ElementModel obj)
		{
			return obj.Id.GetHashCode();
		}

		public bool Equals(ElementModel other)
		{
			return other != null && Id == other.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

	    #region INotifyPropertyChanged Members

	    public event PropertyChangedEventHandler PropertyChanged;
	    protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
	    {
	        if (PropertyChanged != null)
	            PropertyChanged(this, e);
	    }

	    #endregion

	    #region INotifyPropertyChanging Members

	    public event PropertyChangingEventHandler PropertyChanging;
	    protected virtual void OnPropertyChanging(PropertyChangingEventArgs e)
	    {
	        if (PropertyChanging != null)
	            PropertyChanging(this, e);
	    }

	    #endregion

    }
}
