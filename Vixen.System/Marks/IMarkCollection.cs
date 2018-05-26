using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Vixen.Marks
{
	public interface IMarkCollection : INotifyPropertyChanged, ICloneable
	{
		Guid Id { get; set; }

		string Name { get; set; }

		bool IsDefault { get; set; }

		bool IsVisible { get; }

		bool ShowGridLines { get; set; }

		bool ShowMarkBar { get; set; }

		int Level { get; set; }

		ReadOnlyCollection<IMark> Marks { get;  }

		IMarkDecorator Decorator { get; set; }

		void RemoveMark(IMark mark);

		void RemoveAll(Predicate<IMark> match);

		void AddMark(IMark mark);

		void AddMarks(IEnumerable<IMark> marks);

		void EnsureOrder();

		/// <summary>
		/// Returns all marks that have a start time that is inclusive of the start end range
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		IEnumerable<IMark> MarksInclusiveOfTime(TimeSpan start, TimeSpan end);
	}
}
