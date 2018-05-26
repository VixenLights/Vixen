using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Marks;

namespace VixenModules.App.Marks
{
	[DataContract]
	[KnownType(typeof(MarkDecorator))]
	[KnownType(typeof(Mark))]
	public class MarkCollection: BindableBase, IMarkCollection
	{
		[DataMember(Name = "Marks")]
		private List<IMark> _marks;

		private string _name;
		private int _level;
		private IMarkDecorator _decorator;
		private bool _isDefault;
		private bool _showMarkBar;
		private bool _showGridLines;

		public MarkCollection()
		{
			Name = @"Mark Collection";
			Decorator = new MarkDecorator();
			_marks = new List<IMark>();
			Marks = _marks.AsReadOnly();
			Id = Guid.NewGuid();
			Level = 1;
			ShowGridLines = true;
			ShowMarkBar = false;
		}

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
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

		[DataMember]
		public bool ShowMarkBar
		{
			get { return _showMarkBar; }
			set
			{
				if (value == _showMarkBar) return;
				_showMarkBar = value;
				OnPropertyChanged(nameof(ShowMarkBar));
			}
		}

		[DataMember]
		public bool ShowGridLines
		{
			get { return _showGridLines; }
			set
			{
				if (value == _showGridLines) return;
				_showGridLines = value;
				OnPropertyChanged(nameof(ShowGridLines));
			}
		}

		[DataMember]
		public int Level
		{
			get { return _level; }
			set
			{
				if (value == _level) return;
				_level = value;
				OnPropertyChanged(nameof(Level));
			}
		}

		[DataMember]
		public bool IsDefault
		{
			get { return _isDefault; }
			set
			{
				if (value == _isDefault) return;
				_isDefault = value;
				OnPropertyChanged(nameof(IsDefault));
			}
		}


		[IgnoreDataMember]
		public ReadOnlyCollection<IMark> Marks { get; private set; }

		[DataMember]
		public IMarkDecorator Decorator
		{
			get { return _decorator; }
			set
			{
				if (Equals(value, _decorator)) return;
				_decorator = value;
				OnPropertyChanged(nameof(Decorator));
			}
		}

		public bool IsVisible => ShowGridLines || ShowMarkBar;

		public void AddMark(IMark mark)
		{
			_marks.Add(mark);
			mark.Parent = this;
			OnPropertyChanged(nameof(Marks));
		}

		public void AddMarks(IEnumerable<IMark> marks)
		{
			foreach (var mark in marks)
			{
				mark.Parent = this;
			}
			_marks.AddRange(marks);
			OnPropertyChanged(nameof(Marks));
		}

		public void RemoveMark(IMark mark)
		{
			_marks.Remove(mark);
			OnPropertyChanged(nameof(Marks));
		}

		public void RemoveAll(Predicate<IMark> match)
		{
			_marks.RemoveAll(match);
			OnPropertyChanged(nameof(Marks));
		}

		public void EnsureOrder()
		{
			_marks.Sort();
		}

		/// <inheritdoc />
		public IEnumerable<IMark> MarksInclusiveOfTime(TimeSpan start, TimeSpan end)
		{
			List<IMark> marks = new List<IMark>();
			foreach (var mark in _marks)
			{
				if (mark.StartTime >= start && mark.StartTime <= end)
				{
					marks.Add(mark);
				} 
			}

			return marks;

		}

		[OnDeserialized]
		void OnDeserialized(StreamingContext c)
		{
			if (Marks == null)
			{
				Marks = _marks.AsReadOnly();
			}

			foreach (var mark in _marks)
			{
				mark.Parent = this;
			}
		}

		#region Implementation of ICloneable

		/// <inheritdoc />
		public object Clone()
		{
			return new MarkCollection()
			{
				ShowMarkBar = ShowMarkBar,
				ShowGridLines = ShowGridLines,
				Level = Level,
				Name = Name,
				_marks = Marks.Select(x => (IMark)x.Clone()).ToList(),
				Decorator = (MarkDecorator)Decorator.Clone()
			};
		}

		#endregion
	}
}
