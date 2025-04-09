using System.Collections.ObjectModel;
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
		private bool _showTailGridLines;
		private bool _locked;
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
			ShowTailGridLines = false;
			ShowMarkBar = false;
			Locked = false;
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
		public bool ShowTailGridLines
		{
			get { return _showTailGridLines; }
			set
			{
				if (value == _showTailGridLines) return;
				_showTailGridLines = value;
				OnPropertyChanged(nameof(ShowTailGridLines));
			}
		}

		[DataMember]
		public bool Locked
		{
			get { return _locked; }
			set
			{
				if (value == _locked) return;
				_locked = value;
				OnPropertyChanged(nameof(Locked));
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

		public void SwapPlaces(IMark lhs, IMark rhs)
		{
			TimeSpan temp = lhs.StartTime;
			lhs.StartTime = rhs.StartTime;
			rhs.StartTime = temp;

			temp = lhs.Duration;
			lhs.Duration = rhs.Duration;
			rhs.Duration = temp;

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

		public MarkCollection MarksWithinRange(TimeSpan start, TimeSpan end)
		{
			MarkCollection marks = new MarkCollection();

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

			OnPropertyChanged(nameof(Marks));
		}

		public void OffsetMarksByTime(TimeSpan time)
		{
			foreach (var m in Marks)
			{
				m.StartTime = m.StartTime + time;
			}
		}

		/// <summary>
		/// Offsets the Marks in the collection by a given time.
		/// </summary>
		/// <param name="start">Specifies the inclusive start of the time window</param>
		/// <param name="end">Specifies the inclusive end time of the time window</param>
		/// <param name="offset">
		/// Specifies the amount of time to shift the Marks that are within the time window. A positive value adds time and a negative
		/// value decreases time.
		/// </param>
		/// <param name="maxTime">Spcifies the ending time of the Sequence</param>
		/// <returns>Returns all the Marks affected</returns>
		public Dictionary<IMark, IMark> OffsetMarksByTime(TimeSpan start, TimeSpan end, TimeSpan offset, TimeSpan maxTime)
        {
			IMark marksPreMove;
			var marksChange = new Dictionary<IMark, IMark>();
			int index = 0;

			if (_marks.Count == 0)
				return null;

			do
			{
				// If the starting time of the Mark is within the time window, then move the mark.
				if (start <= _marks[index].StartTime && _marks[index].StartTime <= end)
				{
					marksPreMove = (IMark)_marks[index].Clone();

					// If after moving the Mark, the whole Mark duration is prior to zero time, then delete the Mark.
					if (_marks[index].EndTime + offset < TimeSpan.Zero)
					{
						_marks[index].Parent.RemoveMark(_marks[index]);
						// Note: do not advance 'index' since deleting the mark causes all the remaining Marks move up.
						marksChange.Add(marksPreMove, null);
					}

					// If after moving the Mark, the whole Mark duration is after the maximum time, then delete the Mark.
					else if (_marks[index].StartTime + offset > maxTime)
					{
						_marks[index].Parent.RemoveMark(_marks[index]);
						// Note: do not advance 'index' since deleting the mark causes all the remaining Marks move up.
						marksChange.Add(marksPreMove, null);
					}

					// else we'll simply adjust the starting time of the Mark and potentially the duration
					else
					{
						_marks[index].StartTime = _marks[index].StartTime + offset;

						// Don't let the Mark's start time be less than zero...
						if (_marks[index].StartTime < TimeSpan.Zero)
						{
							// So clip the beginning of the Mark
							_marks[index].Duration += _marks[index].StartTime;
							_marks[index].StartTime = TimeSpan.Zero;
						}
						// Don't let the Mark's end time be greater than the end of the Sequence...
						else if (_marks[index].EndTime > maxTime)
						{
							// So clip the ending of the Mark
							_marks[index].Duration += _marks[index].EndTime;
						}

						marksChange.Add(marksPreMove, (IMark)_marks[index]);
					
						// Move to the next Mark.
						index++;
					}

				}
				else
				{
					// Move to the next Mark.
					index++;
				}

			} while (index < _marks.Count);

			OnPropertyChanged(nameof(Marks));

			return marksChange;
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
				ShowTailGridLines = ShowTailGridLines,
				Locked = Locked,
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
