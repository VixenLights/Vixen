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

		ReadOnlyObservableCollection<IMark> Marks { get;  }

		IMarkDecorator Decorator { get; set; }

		MarkCollectionType CollectionType { get; set; }

		Guid LinkedMarkCollectionId { get; set; }

		void RemoveMark(IMark mark);

		void RemoveAll(Func<IMark, bool> condition);

		void AddMark(IMark mark);

		void AddMarks(IEnumerable<IMark> marks);

		void EnsureOrder();

		/// <summary>
		/// Returns all marks that have some portion that is inclusive of the start end range
		/// </summary>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		List<IMark> MarksInclusiveOfTime(TimeSpan start, TimeSpan end);

		void FillGapTimes(IMark mark);
	}
}
