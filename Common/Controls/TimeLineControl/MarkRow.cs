using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Common.Controls.TimelineControl
{
	internal class MarkRow: IEnumerable<LabeledMark>
	{
		// The marks contained in this row. Must be kept sorted; however, we can't use a SortedList
		// or similar, as the elements within the list may have their times updated by the grid, which
		// puts their order out.
		private LabeledMarkCollection _labeledMarkCollection;

		public MarkRow(LabeledMarkCollection markCollectionCollection)
		{
			_labeledMarkCollection = markCollectionCollection;
			_labeledMarkCollection.Marks.Sort();
			Height = 20;
		}

		public int Height { get; set; }

		internal Color MarkColor => _labeledMarkCollection.Color;

		/// <summary>
		/// Set the stacking indexes for overlapping elements in the specific time range.
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		public void SetStackIndexes(TimeSpan startTime, TimeSpan endTime)
		{
			for (int i = 0; i < _labeledMarkCollection.Marks.Count; i++)
			{
				if (_labeledMarkCollection.Marks[i].EndTime < startTime) continue;
				if (_labeledMarkCollection.Marks[i].StartTime > endTime) break;
				List<LabeledMark> overlappingElements = GetOverlappingMarks(_labeledMarkCollection.Marks[i]);
				if (overlappingElements.Any())
				{
					List<List<LabeledMark>> stack = DetermineMarkStack(overlappingElements);
					int x = 0;
					foreach (var elementGroup in stack)
					{
						foreach (var element in elementGroup)
						{
							element.StackIndex = x;
							element.StackCount = stack.Count;
						}
						x++;
					}
				}
				else
				{
					_labeledMarkCollection.Marks[i].StackCount = 1;
					_labeledMarkCollection.Marks[i].StackIndex = 0;
				}
				i += overlappingElements.Count - overlappingElements.IndexOf(_labeledMarkCollection.Marks[i]) - 1;

			}


		}

		private List<List<LabeledMark>> DetermineMarkStack(List<LabeledMark> elements)
		{

			List<List<LabeledMark>> stack = new List<List<LabeledMark>>();
			stack.Add(new List<LabeledMark> { elements[0] });
			for (int i = 1; i < elements.Count; i++)
			{
				bool add = true;
				for (int x = 0; x < stack.Count; x++)
				{
					if (elements[i].StartTime >= stack[x].Last().EndTime)
					{
						stack[x].Add(elements[i]);
						add = false;
						break;
					}
				}
				if (add) stack.Add(new List<LabeledMark> { elements[i] });
			}

			return stack;

		}

		public List<LabeledMark> GetOverlappingMarks(LabeledMark elementMaster)
		{
			List<LabeledMark> elements = new List<LabeledMark>();
			elements.Add(elementMaster); //add our reference
			int startingIndex = IndexOfMark(elementMaster);
			TimeSpan startTime = elementMaster.StartTime;
			TimeSpan endTime = elementMaster.EndTime;

			//we start here and look backward and forward until no more overlap
			//Look forward.
			for (int i = startingIndex + 1; i < _labeledMarkCollection.Marks.Count; i++)
			{
				LabeledMark element = GetMarkAtIndex(i);
				if (element.StartTime < endTime)
				{
					elements.Add(element);
					endTime = element.EndTime > endTime ? element.EndTime : endTime;
				}
				else
				{
					break;
				}

			}

			//Look backward.
			for (int i = startingIndex - 1; i >= 0; i--)
			{
				LabeledMark element = GetMarkAtIndex(i);
				if (element.EndTime > startTime)
				{
					elements.Insert(0, element);
					startTime = element.StartTime < startTime ? element.StartTime : startTime;
				}

			}

			return elements;
		}

		public int IndexOfMark(LabeledMark element)
		{
			return _labeledMarkCollection.Marks.IndexOf(element);
		}

		public LabeledMark GetMarkAtIndex(int index)
		{
			if (index < 0 || index >= _labeledMarkCollection.Marks.Count)
				return null;

			return _labeledMarkCollection.Marks[index];
		}

		public int MarksCount
		{
			get { return _labeledMarkCollection.Marks.Count; }
		}


		#region Implementation of IEnumerable

		/// <inheritdoc />
		public IEnumerator<LabeledMark> GetEnumerator()
		{
			return _labeledMarkCollection.Marks.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
