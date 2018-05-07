using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VixenModules.App.Marks;

namespace Common.Controls.TimelineControl
{
	internal class MarkRow: IEnumerable<Mark>
	{
		// The marks contained in this row. Must be kept sorted; however, we can't use a SortedList
		// or similar, as the elements within the list may have their times updated by the grid, which
		// puts their order out.
		private readonly MarkCollection _labeledMarkCollection;
		private Dictionary<Mark, MarkStack> _stackIndexes = new Dictionary<Mark, MarkStack>();

		public MarkRow(MarkCollection markCollection)
		{
			_labeledMarkCollection = markCollection;
			_labeledMarkCollection.EnsureOrder();
			Height = 20;
		}

		public int Height { get; set; }

		public int DisplayTop { get; set; }

		public bool Visible => _labeledMarkCollection.Decorator.Mode == Mode.Full;

		internal MarkDecorator MarkDecorator => _labeledMarkCollection.Decorator;

		internal MarkStack GetStackForMark(Mark mark)
		{
			MarkStack ms;
			if (_stackIndexes.TryGetValue(mark, out ms))
			{
				return ms;
			}
			return new MarkStack(0, 1);
		}

		/// <summary>
		/// Set the stacking indexes for overlapping elements in the specific time range.
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		public void SetStackIndexes(TimeSpan startTime, TimeSpan endTime)
		{
			_stackIndexes.Clear();
			_labeledMarkCollection.EnsureOrder();
			for (int i = 0; i < _labeledMarkCollection.Marks.Count; i++)
			{
				if (_labeledMarkCollection.Marks[i].EndTime < startTime) continue;
				if (_labeledMarkCollection.Marks[i].StartTime > endTime) break;
				List<Mark> overlappingElements = GetOverlappingMarks(_labeledMarkCollection.Marks[i]);
				if (overlappingElements.Any())
				{
					List<List<Mark>> stack = DetermineMarkStack(overlappingElements);
					int x = 0;
					foreach (var elementGroup in stack)
					{
						foreach (var element in elementGroup)
						{
							if (_stackIndexes.ContainsKey(element))
							{
								var ms = _stackIndexes[element];
								ms.StackCount = stack.Count;
								ms.StackIndex = x;
							}
							else
							{
								_stackIndexes.Add(element, new MarkStack(x, stack.Count));
							}
						}
						x++;
					}
				}
				else
				{
					_stackIndexes.Add(_labeledMarkCollection.Marks[i], new MarkStack(0, 1));
					//_labeledMarkCollection.Marks[i].StackCount = 1;
					//_labeledMarkCollection.Marks[i].StackIndex = 0;
				}
				i += overlappingElements.Count - overlappingElements.IndexOf(_labeledMarkCollection.Marks[i]) - 1;

			}


		}

		private List<List<Mark>> DetermineMarkStack(List<Mark> elements)
		{

			List<List<Mark>> stack = new List<List<Mark>>();
			stack.Add(new List<Mark> { elements[0] });
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
				if (add) stack.Add(new List<Mark> { elements[i] });
			}

			return stack;

		}

		public List<Mark> GetOverlappingMarks(Mark elementMaster)
		{
			List<Mark> elements = new List<Mark>();
			elements.Add(elementMaster); //add our reference
			int startingIndex = IndexOfMark(elementMaster);
			TimeSpan startTime = elementMaster.StartTime;
			TimeSpan endTime = elementMaster.EndTime;

			//we start here and look backward and forward until no more overlap
			//Look forward.
			for (int i = startingIndex + 1; i < _labeledMarkCollection.Marks.Count; i++)
			{
				Mark element = GetMarkAtIndex(i);
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
				Mark element = GetMarkAtIndex(i);
				if (element.EndTime > startTime)
				{
					elements.Insert(0, element);
					startTime = element.StartTime < startTime ? element.StartTime : startTime;
				}

			}

			return elements;
		}

		public int IndexOfMark(Mark element)
		{
			return _labeledMarkCollection.Marks.IndexOf(element);
		}

		public Mark GetMarkAtIndex(int index)
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
		public IEnumerator<Mark> GetEnumerator()
		{
			return _labeledMarkCollection.Marks.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		public struct MarkStack
		{
			public MarkStack(int stackIndex, int stackCount)
			{
				StackCount = stackCount;
				StackIndex = stackIndex;
			}

			public int StackCount { get; set; }

			public int StackIndex { get; set; }
		}
	}
}
