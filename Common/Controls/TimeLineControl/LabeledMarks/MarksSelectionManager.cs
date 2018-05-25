using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VixenModules.App.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksSelectionManager
	{
		private static MarksSelectionManager _manager;
		private static List<Mark> _selectedMarks;
		public event EventHandler SelectionChanged ;

		private MarksSelectionManager()
		{
			_selectedMarks = new List<Mark>();
			SelectedMarks = _selectedMarks.AsReadOnly();
		}

		public static MarksSelectionManager Manager()
		{
			return _manager ?? (_manager = new MarksSelectionManager());
		}

		public ReadOnlyCollection<Mark> SelectedMarks { get; }

		public void ClearSelected()
		{
			_selectedMarks.Clear();
			OnSelectionChanged();
		}

		public void Select(Mark mark)
		{
			if (!SelectedMarks.Contains(mark))
			{
				_selectedMarks.Add(mark);
				OnSelectionChanged();
			}
		}

		public bool IsSelected(Mark mark)
		{
			return _selectedMarks.Contains(mark);
		}

		public void DeSelect(Mark mark)
		{
			_selectedMarks.Remove(mark);
			OnSelectionChanged();
		}

		private void OnSelectionChanged()
		{
			SelectionChanged?.Invoke(this, EventArgs.Empty);
		}
	}
}
