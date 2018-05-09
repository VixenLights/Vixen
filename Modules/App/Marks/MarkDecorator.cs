using System;
using System.Drawing;
using System.Runtime.Serialization;

namespace VixenModules.App.Marks
{
	[DataContract]
	public class MarkDecorator:BindableBase, ICloneable
	{
		private bool _isBold;
		private bool _isSolidLine;
		private Color _color;
		private bool _compactMode;

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

		[DataMember]
		public bool CompactMode
		{
			get { return _compactMode; }
			set
			{
				if (value == _compactMode) return;
				_compactMode = value;
				OnPropertyChanged(nameof(CompactMode));
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
				IsSolidLine = IsSolidLine,
				CompactMode = CompactMode
			};
		}

		#endregion
	}
}
