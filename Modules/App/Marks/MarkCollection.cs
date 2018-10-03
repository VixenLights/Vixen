using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using Vixen.Extensions;
using Vixen.Marks;

namespace VixenModules.App.Marks
{
	[DataContract]
	[KnownType(typeof(MarkDecorator))]
	[KnownType(typeof(Mark))]
	public class MarkCollection: BindableBase, IMarkCollection
	{
		[DataMember(Name = "Marks")]
		private ObservableCollection<IMark> _marks;

		private string _name;
		private int _level;
		private IMarkDecorator _decorator;
		private bool _isDefault;
		private bool _showMarkBar;
		private bool _showGridLines;
		private MarkCollectionType _collectionType = MarkCollectionType.Generic;

		public MarkCollection()
		{
			Name = @"Mark Collection";
			Decorator = new MarkDecorator();
			_marks = new ObservableCollection<IMark>();
			Marks = new ReadOnlyObservableCollection<IMark>(_marks);
			Id = Guid.NewGuid();
			Level = 1;
			ShowGridLines = true;
			ShowMarkBar = false;
			CollectionType = MarkCollectionType.Generic;
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
		public ReadOnlyObservableCollection<IMark> Marks { get; private set; }

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

		/// <inheritdoc />
		[DataMember]
		public MarkCollectionType CollectionType
		{
			get { return _collectionType; }
			set
			{
				if (value == _collectionType) return;
				_collectionType = value;
				OnPropertyChanged(nameof(CollectionType));
			}
		}

		/// <inheritdoc />
		[DataMember]
		public Guid LinkedMarkCollectionId { get; set; }

		public bool IsVisible => ShowGridLines || ShowMarkBar;

		public void AddMark(IMark mark)
		{
			_marks.Add(mark);
			mark.Parent = this;
			EnsureOrder();
			OnPropertyChanged(nameof(Marks));
		}

		public void AddMarks(IEnumerable<IMark> marks)
		{
			foreach (var mark in marks)
			{
				mark.Parent = this;
			}
			_marks.AddRange(marks);
			EnsureOrder();
			OnPropertyChanged(nameof(Marks));
		}

		public void RemoveMark(IMark mark)
		{
			_marks.Remove(mark);
			EnsureOrder();
			OnPropertyChanged(nameof(Marks));
		}

		public void RemoveAll(Func<IMark, bool> condition)
		{
			_marks.RemoveAll(condition);
			EnsureOrder();
			OnPropertyChanged(nameof(Marks));
		}

		public void EnsureOrder()
		{
			_marks.Sort();
		}

		/// <inheritdoc />
		public List<IMark> MarksInclusiveOfTime(TimeSpan start, TimeSpan end)
		{
			List<IMark> marks = new List<IMark>();
			foreach (var mark in _marks.ToArray())
			{
				if (mark.EndTime < start) continue;
				if (mark.StartTime > end) break;

				marks.Add(mark);
			}

			return marks;

		}

		public void FillGapTimes(IMark mark)
		{
			EnsureOrder();
			var index = Marks.IndexOf(mark);

			for (int i = index - 1; i >= 0; i--)
			{
				var time = Marks[i].EndTime;
				if (time < mark.StartTime)
				{
					mark.StartTime = time + TimeSpan.FromMilliseconds(1);
					break;
				}
			}

			if (index != Marks.Count - 1)
			{
				mark.Duration = Marks[index + 1].StartTime - TimeSpan.FromMilliseconds(1) - mark.StartTime;
			}
		}

		[OnDeserialized]
		void OnDeserialized(StreamingContext c)
		{
			if (Marks == null)
			{
				Marks = new ReadOnlyObservableCollection<IMark>(_marks);
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
				_marks = new ObservableCollection<IMark>(Marks.Select(x => (IMark)x.Clone())),
				Decorator = (MarkDecorator)Decorator.Clone(),
				CollectionType = CollectionType
			};
		}

		#endregion
	}
}
