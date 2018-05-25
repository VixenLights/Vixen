using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Vixen.Marks;
using VixenModules.App.Marks;

namespace Common.Controls.TimelineControl
{
	internal class MarkRow: IEnumerable<IMark>, IDisposable
	{
		// The marks contained in this row. Must be kept sorted; however, we can't use a SortedList
		// or similar, as the elements within the list may have their times updated by the grid, which
		// puts their order out.
		
		private readonly Dictionary<IMark, MarkStack> _stackIndexes = new Dictionary<IMark, MarkStack>();

		public MarkRow(IMarkCollection markCollection)
		{
			MarkCollection = markCollection;
			MarkCollection.EnsureOrder();
			MarkCollection.PropertyChanged += MarkCollection_PropertyChanged;
			MarkCollection.Decorator.PropertyChanged += Decorator_PropertyChanged;
			Height = 20;
			StackCount = 1;
		}

		public IMarkCollection MarkCollection { get; private set; }

		public int StackCount { get; private set; }

		#region Events

		public static event EventHandler MarkRowChanged;

		#endregion

		#region Event Handlers

		private void OnMarkRowChanged()
		{
			MarkRowChanged?.Invoke(this, EventArgs.Empty);
		}

		#endregion

		private void Decorator_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnMarkRowChanged();
		}

		private void MarkCollection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			OnMarkRowChanged();
		}

		public int Height { get; set; }

		public int DisplayTop { get; set; }

		public bool Visible => MarkCollection.ShowMarkBar;

		internal IMarkDecorator MarkDecorator => MarkCollection.Decorator;

		internal MarkStack GetStackForMark(IMark mark)
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
			int maxStack = 1;
			_stackIndexes.Clear();
			
			for (int i = 0; i < MarkCollection.Marks.Count; i++)
			{
				if (MarkCollection.Marks[i].EndTime < startTime) continue;
				if (MarkCollection.Marks[i].StartTime > endTime) break;
				List<IMark> overlappingElements = GetOverlappingMarks(MarkCollection.Marks[i]);
				if (overlappingElements.Count > 1)
				{
					List<List<IMark>> stack = DetermineMarkStack(overlappingElements);
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

					maxStack = Math.Max(maxStack, stack.Count);
				}
				else
				{
					_stackIndexes.Add(MarkCollection.Marks[i], new MarkStack(0, 1));
				}
				i += overlappingElements.Count - overlappingElements.IndexOf(MarkCollection.Marks[i]) - 1;

			}

			StackCount = maxStack;
			Height = 20 * maxStack;

		}

		private List<List<IMark>> DetermineMarkStack(List<IMark> elements)
		{

			List<List<IMark>> stack = new List<List<IMark>>();
			stack.Add(new List<IMark> { elements[0] });
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
				if (add) stack.Add(new List<IMark> { elements[i] });
			}

			return stack;

		}

		public List<IMark> GetOverlappingMarks(IMark elementMaster)
		{
			List<IMark> elements = new List<IMark>();
			elements.Add(elementMaster); //add our reference
			int startingIndex = IndexOfMark(elementMaster);
			TimeSpan startTime = elementMaster.StartTime;
			TimeSpan endTime = elementMaster.EndTime;

			//we start here and look backward and forward until no more overlap
			//Look forward.
			for (int i = startingIndex + 1; i < MarkCollection.Marks.Count; i++)
			{
				var element = GetMarkAtIndex(i);
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
				var element = GetMarkAtIndex(i);
				if (element.EndTime > startTime)
				{
					elements.Insert(0, element);
					startTime = element.StartTime < startTime ? element.StartTime : startTime;
				}

			}

			return elements;
		}

		public int IndexOfMark(IMark element)
		{
			return MarkCollection.Marks.IndexOf(element);
		}

		public IMark GetMarkAtIndex(int index)
		{
			if (index < 0 || index >= MarkCollection.Marks.Count)
				return null;

			return MarkCollection.Marks[index];
		}

		public int MarksCount
		{
			get { return MarkCollection.Marks.Count; }
		}


		#region Implementation of IEnumerable

		/// <inheritdoc />
		public IEnumerator<IMark> GetEnumerator()
		{
			return MarkCollection.Marks.GetEnumerator();
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		#endregion

		#region IDisposable

		/// <inheritdoc />
		public void Dispose()
		{
			MarkCollection.PropertyChanged -= MarkCollection_PropertyChanged;
			MarkCollection.Decorator.PropertyChanged -= Decorator_PropertyChanged;
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
