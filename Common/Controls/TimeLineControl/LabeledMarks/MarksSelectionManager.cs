using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Vixen.Marks;
using VixenModules.App.Marks;

namespace Common.Controls.TimelineControl.LabeledMarks
{
	public class MarksSelectionManager
	{
		private static MarksSelectionManager _manager;
		private static List<IMark> _selectedMarks;
		public event EventHandler SelectionChanged ;

		private MarksSelectionManager()
		{
			_selectedMarks = new List<IMark>();
			SelectedMarks = _selectedMarks.AsReadOnly();
		}

		public static MarksSelectionManager Manager()
		{
			return _manager ?? (_manager = new MarksSelectionManager());
		}

		public ReadOnlyCollection<IMark> SelectedMarks { get; }

		public void ClearSelected()
		{
			_selectedMarks.Clear();
			OnSelectionChanged();
		}

		public void Select(IMark mark)
		{
			if (mark != null && !SelectedMarks.Contains(mark))
			{
				_selectedMarks.Add(mark);
				OnSelectionChanged();
			}
		}

		public void Select(List<IMark> marks)
		{
			if (!_selectedMarks.Any())
			{
				_selectedMarks.AddRange(marks);
				OnSelectionChanged();
				return;
			}

			var selectionChanged = false;
			foreach (var mark in marks)
			{
				if (!SelectedMarks.Contains(mark))
				{
					_selectedMarks.Add(mark);
					selectionChanged = true;
				}
			}

			if (selectionChanged)
			{
				OnSelectionChanged();
			}
		}

		public bool IsSelected(IMark mark)
		{
			return _selectedMarks.Contains(mark);
		}

		public void DeSelect(IMark mark)
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
