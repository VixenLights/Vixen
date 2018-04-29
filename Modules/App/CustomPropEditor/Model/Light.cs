using System;
using System.Windows;
using Common.WPFCommon.ViewModel;

namespace VixenModules.App.CustomPropEditor.Model
{
	[Serializable]
	public class Light : BindableBase
	{

		private double _size;
		private double _y;
		private double _x;
		private double _z;
		private Guid _id;

		internal Light()
		{

		}

		public Light(Point center, double size, Guid parentId)
		{
			Id = Guid.NewGuid();
			ParentModelId = parentId;
			X = center.X;
			Y = center.Y;
			_size = size;
		}

		public Guid Id
		{
			get { return _id; }
			set
			{
				if (value.Equals(_id)) return;
				_id = value;
				OnPropertyChanged(nameof(Id));
			}
		}

		public Guid ParentModelId { get; set; }

		public double Y
		{
			get { return _y; }
			set
			{
				if (value.Equals(_y)) return;
				_y = value;
				OnPropertyChanged("Y");
			}
		}

		public double X
		{
			get { return _x; }
			set
			{
				if (value.Equals(_x)) return;
				_x = value;
				OnPropertyChanged("X");
			}
		}

		public double Z
		{
			get { return _z; }
			set
			{
				if (value.Equals(_z)) return;
				_z = value;
				OnPropertyChanged("Z");
			}
		}

		public double Size
		{
			get { return _size; }
			set
			{
				if (!value.Equals(_size))
				{
					_size = value;
					OnPropertyChanged("Size");
				}
			}
		}

	}
}
