using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Vixen.Sys.Marks
{
	[DataContract]
	public class MarkCollection: ICloneable
	{
		[DataMember(Name = "Marks")]
		private List<Mark> _marks;

		public MarkCollection()
		{
			Decorator = new MarkDecorator();
			_marks = new List<Mark>();
			Marks = _marks.AsReadOnly();
			Id = Guid.NewGuid();
			Level = 1;
			IsEnabled = true;
		}

		[DataMember]
		public Guid Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public bool IsEnabled { get; set; }

		[DataMember]
		public int Level { get; set; }

		public ReadOnlyCollection<Mark> Marks { get; private set; }

		[DataMember]
		public MarkDecorator Decorator { get; set; }

		public void AddMark(Mark mark)
		{
			_marks.Add(mark);
			mark.Parent = this;
		}

		public void AddMarks(IEnumerable<Mark> marks)
		{
			foreach (var mark in marks)
			{
				mark.Parent = this;
			}
			_marks.AddRange(marks);
		}

		public void RemoveMark(Mark mark)
		{
			_marks.Remove(mark);
		}

		public void RemoveAll(Predicate<Mark> match)
		{
			_marks.RemoveAll(match);
		}

		public void EnsureOrder()
		{
			_marks.Sort();
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
				IsEnabled = IsEnabled,
				Level = Level,
				Name = Name,
				_marks = Marks.Select(x => (Mark)x.Clone()).ToList(),
				Decorator = (MarkDecorator)Decorator.Clone()
			};
		}

		#endregion
	}
}
