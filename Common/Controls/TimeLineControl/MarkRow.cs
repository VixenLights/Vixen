using System;
using System.Collections;

namespace Common.Controls.TimelineControl
{
	internal class MarkRow: IEnumerable<LabeledMark>
	{

		// The marks contained in this row. Must be kept sorted; however, we can't use a SortedList
		// or similar, as the elements within the list may have their times updated by the grid, which
		// puts their order out.
		protected List<LabeledMark> LabeledMarks;

		public MarkRow(MarkCollection markCollection)
		{
			LabeledMarks = markCollection.LabeledMarks;
			Height = 12;
		}

		public int Height { get; set; }

		/// <summary>
		/// Set the stacking indexes for overlapping elements in the specific time range.
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="endTime"></param>
		public void SetStackIndexes(TimeSpan startTime, TimeSpan endTime)
		{
			for (int i = 0; i < LabeledMarks.Count; i++)
			{
				if (LabeledMarks[i].EndTime < startTime) continue;
				if (LabeledMarks[i].StartTime > endTime) break;
				List<LabeledMark> overlappingElements = GetOverlappingMarks(LabeledMarks[i]);
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
					LabeledMarks[i].StackCount = 1;
					LabeledMarks[i].StackIndex = 0;
				}
				i += overlappingElements.Count - overlappingElements.IndexOf(LabeledMarks[i]) - 1;

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
			for (int i = startingIndex + 1; i < LabeledMarks.Count; i++)
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
			return LabeledMarks.IndexOf(element);
		}

		public LabeledMark GetMarkAtIndex(int index)
		{
			if (index < 0 || index >= LabeledMarks.Count)
				return null;

			return LabeledMarks[index];
		}


		#region Implementation of IEnumerable

		/// <inheritdoc />
		public IEnumerator<LabeledMark> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion
	}
}
