using System;
using System.Drawing;
using System.Runtime.Serialization;
using Vixen.Marks;

namespace VixenModules.App.Marks
{
	[DataContract]
	public class MarkDecorator:BindableBase, IMarkDecorator
	{
		private bool _isBold;
		private bool _isSolidLine;
		private Color _color;
		private bool _showLabels;
		private bool _showLines;

		public MarkDecorator()
		{
			Color = Color.White;
			IsBold = false;
			IsSolidLine = false;
		}

		[DataMember]
		public bool IsBold
		{
			get { return _isBold; }
			set
			{
				if (value == _isBold) return;
				_isBold = value;
				OnPropertyChanged(nameof(IsBold));
			}
		}

		[DataMember]
		public bool IsSolidLine
		{
			get { return _isSolidLine; }
			set
			{
				if (value == _isSolidLine) return;
				_isSolidLine = value;
				OnPropertyChanged(nameof(IsSolidLine));
			}
		}

		[DataMember]
		public Color Color
		{
			get { return _color; }
			set
			{
				if (value.Equals(_color)) return;
				_color = value;
				OnPropertyChanged(nameof(Color));
			}
		}

		#region Implementation of ICloneable

		/// <inheritdoc />
		public object Clone()
		{
			return new MarkDecorator()
			{
				Color = Color,
				IsBold = IsBold,
				IsSolidLine = IsSolidLine
			};
		}

		#endregion
	}
}
