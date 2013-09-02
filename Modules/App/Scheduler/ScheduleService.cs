using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.App.Scheduler
{
	internal class ScheduleService
	{
		/// <summary>
		/// Used for items with MonthlyDateOfMonth recurrence.
		/// </summary>
		public const int LastDay = -1;

		/// <summary>
		/// Get the ScheduleItem instances from the provided collection that are scheduled to be valid right now,
		/// within the timeWindow span.
		/// </summary>
		/// <param name="items">Collection of ScheduleItems the scheduler will operate upon</param>
		/// <param name="timeWindow">Span of time starting at the current time
		/// within which schedule items should be considered valid.</param>
		/// <returns></returns>
		public IEnumerable<IScheduleItem> GetItems(IEnumerable<IScheduleItem> items, TimeSpan timeWindow)
		{
			return GetItems(items, DateTime.Now, DateTime.Now + timeWindow);
		}

		/// <summary>
		/// Get the ScheduleItem instances from the provided collection that are scheduled within the dates 
		/// specified, inclusively.
		/// </summary>
		/// <param name="items">Collection of ScheduleItems the scheduler will operate upon</param>
		/// <param name="startDateTime">Start date and time of the window within which schedule items should be considered valid.</param>
		/// <param name="endDateTime">End date and time of the window within which schedule items should be considered valid.  This value is inclusive.</param>
		/// <returns></returns>
		public IEnumerable<IScheduleItem> GetItems(IEnumerable<IScheduleItem> items, DateTime startDateTime,
		                                           DateTime endDateTime)
		{
			if (items == null) throw new ArgumentNullException("items");

			return _GetItemsByDate(items, startDateTime, endDateTime);
		}

		public IEnumerable<IScheduleItem> GetQualifiedItems(IEnumerable<IScheduleItem> items)
		{
			return GetQualifiedItems(items, DateTime.Now);
		}

		public IEnumerable<IScheduleItem> GetQualifiedItems(IEnumerable<IScheduleItem> items, DateTime qualifyingDateTime)
		{
			return items.Where(x =>
			                   (x.RepeatsOnInterval && _IntervalRepeatingQualifier(x, qualifyingDateTime)) ||
			                   (x.RepeatsWithinBlock && _RepeatingQualifier(x, qualifyingDateTime)) ||
			                   _OneTimeTimeQualifier(x, qualifyingDateTime)).ToArray();
		}

		/// <summary>
		/// Provides a readable text summary of a schedule item.
		/// </summary>
		/// <param name="scheduleItem"></param>
		/// <returns></returns>
		public string GetItemSummary(IScheduleItem scheduleItem)
		{
			StringBuilder sb = new StringBuilder();
			RecurrenceType recurrenceType = (RecurrenceType) scheduleItem.RecurrenceType;

			string unit = string.Empty;
			switch (recurrenceType) {
				case RecurrenceType.Daily:
					unit = "day";
					break;
				case RecurrenceType.Weekly:
					unit = "week";
					break;
				case RecurrenceType.MonthlyDateOfMonth:
					unit = "month";
					break;
				case RecurrenceType.MonthlyDayOfWeek:
					unit = "month";
					break;
			}

			sb.Append("Every ");
			if (scheduleItem.DateUnitCount > 1) {
				sb.AppendFormat(" {0} {1}s", scheduleItem.DateUnitCount, unit);
			}
			else {
				sb.Append(unit);
			}

			switch (recurrenceType) {
				case RecurrenceType.Daily:
					// Nothing to do
					break;
				case RecurrenceType.Weekly:
					sb.Append(" on " + (DayOfWeek.Sunday + scheduleItem.DayDate).ToString());
					break;
				case RecurrenceType.MonthlyDateOfMonth:
					if (scheduleItem.DayDate != LastDay) {
						sb.Append(" on the " + _AppendCount(scheduleItem.DayDate));
					}
					else {
						sb.Append(" on the last day of the month ");
					}
					break;
				case RecurrenceType.MonthlyDayOfWeek:
					sb.AppendFormat(" on the {0} {1}", _AppendCount(scheduleItem.DayCount),
					                (DayOfWeek.Sunday + scheduleItem.DayDate).ToString());
					break;
			}

			sb.AppendFormat(" at " + scheduleItem.RunStartTime.ToString("hh:mm tt"));

			sb.AppendFormat(" starting on " + scheduleItem.StartDate.ToShortDateString());

			return sb.ToString();
		}

		#region Private

		private bool _IntervalRepeatingQualifier(IScheduleItem item, DateTime dateTime)
		{
			// If within the block of time...
			if (dateTime.TimeOfDay >= item.RunStartTime && dateTime.TimeOfDay < item.RunEndTime) {
				// Generate interval blocks
				for (TimeSpan time = item.RunStartTime;
				     time < item.RunEndTime;
				     time += TimeSpan.FromMinutes(item.RepeatIntervalMinutes)) {
					// Has to be within the scheduled minute.
					if (dateTime.TimeOfDay >= time && dateTime.TimeOfDay < dateTime.TimeOfDay + TimeSpan.FromMinutes(1)) {
						return true;
					}
				}
			}

			return false;
		}

		private bool _RepeatingQualifier(IScheduleItem item, DateTime dateTime)
		{
			return dateTime.TimeOfDay >= item.RunStartTime && dateTime.TimeOfDay < item.RunEndTime;
		}

		private bool _OneTimeTimeQualifier(IScheduleItem item, DateTime dateTime)
		{
			return dateTime.TimeOfDay >= item.RunStartTime && dateTime.TimeOfDay < (item.RunStartTime + TimeSpan.FromMinutes(1));
		}

		private IEnumerable<IScheduleItem> _GetItemsByDate(IEnumerable<IScheduleItem> items, DateTime startDateTime,
		                                                   DateTime endDateTime)
		{
			List<ScheduleItem> validItems = new List<ScheduleItem>();

			foreach (ScheduleItem scheduleItem in items) {
				if (startDateTime >= scheduleItem.EndDate ||
				    endDateTime <= scheduleItem.StartDate) continue;
				switch ((RecurrenceType) scheduleItem.RecurrenceType) {
					case RecurrenceType.None:
						if (_IsSingleValid(scheduleItem, startDateTime, endDateTime)) {
							validItems.Add(scheduleItem);
						}
						break;
					case RecurrenceType.Daily:
						if (_IsDailyValid(scheduleItem, startDateTime, endDateTime)) {
							validItems.Add(scheduleItem);
						}
						break;
					case RecurrenceType.Weekly:
						if (_IsWeeklyValid(scheduleItem, startDateTime, endDateTime)) {
							validItems.Add(scheduleItem);
						}
						break;
					case RecurrenceType.MonthlyDateOfMonth:
						if (_IsMonthlyDOMValid(scheduleItem, startDateTime, endDateTime)) {
							validItems.Add(scheduleItem);
						}
						break;
					case RecurrenceType.MonthlyDayOfWeek:
						if (_IsMonthlyDOWValid(scheduleItem, startDateTime, endDateTime)) {
							validItems.Add(scheduleItem);
						}
						break;
				}
			}

			return validItems;
		}

		private string _AppendCount(int count)
		{
			int dateRemainder = count%10;
			string appendage;

			if (dateRemainder == 1) {
				appendage = "st";
			}
			else if (dateRemainder == 2) {
				appendage = "nd";
			}
			else if (dateRemainder == 3) {
				appendage = "rd";
			}
			else {
				appendage = "th";
			}

			return count.ToString() + appendage;
		}

		private bool _IsSingleValid(IScheduleItem scheduleItem, DateTime windowStart, DateTime windowEnd)
		{
			return _WindowIsValidForItem(scheduleItem, windowStart, windowEnd);
		}

		private bool _IsDailyValid(IScheduleItem scheduleItem, DateTime windowStart, DateTime windowEnd)
		{
			return _WindowIsValidForItem(scheduleItem, windowStart, windowEnd);
		}

		private bool _IsWeeklyValid(IScheduleItem scheduleItem, DateTime windowStart, DateTime windowEnd)
		{
			if (_WindowIsValidForItem(scheduleItem, windowStart, windowEnd)) {
				// The window could potentially cross a day boundary.
				// The item is valid if the window crosses into the item's day.
				return (int) windowStart.DayOfWeek == scheduleItem.DayDate || (int) windowEnd.DayOfWeek == scheduleItem.DayDate;
			}
			return false;
		}

		private bool _IsMonthlyDOMValid(IScheduleItem scheduleItem, DateTime windowStart, DateTime windowEnd)
		{
			if (_WindowIsValidForItem(scheduleItem, windowStart, windowEnd)) {
				// The window could potentially cross a date boundary.
				// The item is valid if the window crosses into the item's date.
				if (scheduleItem.DayDate != LastDay) {
					return windowStart.Day == scheduleItem.DayDate || windowEnd.Day == scheduleItem.DayDate;
				}
				else {
					// If the item's DayDate value is LastDay, adjust it for the last day of 
					// the current month.
					return windowStart.Day == DateTime.DaysInMonth(windowStart.Year, windowStart.Month) ||
					       windowEnd.Day == DateTime.DaysInMonth(windowEnd.Year, windowEnd.Month);
				}
			}
			return false;
		}

		private bool _IsMonthlyDOWValid(IScheduleItem scheduleItem, DateTime windowStart, DateTime windowEnd)
		{
			if (_WindowIsValidForItem(scheduleItem, windowStart, windowEnd)) {
				// The window could potentially cross a day boundary.
				// The item is valid if the window crosses into the item's day.
				if ((int) windowStart.DayOfWeek == scheduleItem.DayDate || (int) windowEnd.DayOfWeek == scheduleItem.DayDate) {
					// Window crosses the correct day.
					// Does it cross the correct iteration of the day?
					return
						_IterationOfDayOfWeek(windowStart.Day) == scheduleItem.DayCount ||
						_IterationOfDayOfWeek(windowEnd.Day) == scheduleItem.DayCount;
				}
			}
			return false;
		}

		private IEnumerable<DateTime> _GetDailyInstances(IScheduleItem scheduleItem, DateTime windowStart, DateTime windowEnd)
		{
			IEnumerable<int> intervals = _GetItemOccurenceIntervals(scheduleItem, windowStart, windowEnd);
			// Create DateTime instances for each occurrence.
			DateTime instance = scheduleItem.StartDate + scheduleItem.RunStartTime;
			return
				from interval in intervals
				select instance.AddDays(interval*scheduleItem.DateUnitCount);
		}

		private IEnumerable<DateTime> _GetWeeklyInstances(IScheduleItem scheduleItem, DateTime windowStart, DateTime windowEnd)
		{
			IEnumerable<int> intervals = _GetItemOccurenceIntervals(scheduleItem, windowStart, windowEnd);
			// Create DateTime instances for each occurrence.
			DateTime instance = scheduleItem.StartDate + scheduleItem.RunStartTime;
			// Need to start on the correct day of the week at/after the item start date.
			instance = instance.AddDays(_GetDayOfWeekAdjustment(scheduleItem));
			return
				from interval in intervals
				select instance.AddDays(7*interval*scheduleItem.DateUnitCount);
		}

		private IEnumerable<DateTime> _GetMonthlyDOMInstances(IScheduleItem scheduleItem, DateTime windowStart,
		                                                      DateTime windowEnd)
		{
			// Create DateTime instances for each occurrence.
			IEnumerable<int> intervals = _GetItemOccurenceIntervals(scheduleItem, windowStart, windowEnd);
			// Create DateTime instances for each occurrence.
			// This is the only item type where the item date is absolute rather
			// than relative to the start date.
			DateTime startDate = scheduleItem.StartDate;
			if (scheduleItem.DayDate != LastDay) {
				DateTime instance = new DateTime(startDate.Year, startDate.Month, scheduleItem.DayDate) + scheduleItem.RunStartTime;
				return
					from interval in intervals
					select instance.AddMonths(interval*scheduleItem.DateUnitCount);
			}
			else {
				// If the item's DayDate value is LastDay, adjust each for the last day of 
				// the month.

				// Start with an initial date based on the item start date.  Day of the
				// month is unimportant.
				DateTime instance = new DateTime(startDate.Year, startDate.Month, 1);
				return intervals.Select<int, DateTime>(interval =>
				                                       	{
				                                       		// Offset the initial date by an appropriate number of months.
				                                       		DateTime offsetInstance =
				                                       			instance.AddMonths(interval*scheduleItem.DateUnitCount);
				                                       		// With the date now being in the correct year and month, create a
				                                       		// new date with the day set correctly.
				                                       		return
				                                       			new DateTime(offsetInstance.Year, offsetInstance.Month,
				                                       			             DateTime.DaysInMonth(offsetInstance.Year,
				                                       			                                  offsetInstance.Month)) +
				                                       			scheduleItem.RunStartTime;
				                                       	});
			}
		}

		private IEnumerable<DateTime> _GetMonthlyDOWInstances(IScheduleItem scheduleItem, DateTime windowStart,
		                                                      DateTime windowEnd)
		{
			// Create DateTime instances for each occurrence.
			List<int> intervals = new List<int>(_GetItemOccurenceIntervals(scheduleItem, windowStart, windowEnd));
			// Start in the month of the item's start date.
			DateTime instance = _GetDOWOccurrenceDate(scheduleItem, scheduleItem.StartDate);
			return
				from interval in intervals
				select _GetDOWOccurrenceDate(scheduleItem, instance.AddMonths(interval*scheduleItem.DateUnitCount));
		}

		private DateTime _GetDOWOccurrenceDate(IScheduleItem scheduleItem, DateTime monthDate)
		{
			return _GetDOWOccurrenceDate(scheduleItem, monthDate.Year, monthDate.Month);
		}

		private DateTime _GetDOWOccurrenceDate(IScheduleItem scheduleItem, int year, int month)
		{
			// An example item occurrence would be: 3rd Tuesday

			// Get the day of the week for the first day of the month.
			// (i.e. Month starts on a Thursday)
			int startingDayOfWeek = (int) new DateTime(year, month, 1).DayOfWeek;

			// Get the date of the first of the item's day-of-week for the month.
			// (i.e. Item happens on a Tuesday, get the first Tuesday of the month after the start of the month)
			int date;
			if (scheduleItem.DayDate < startingDayOfWeek) {
				// Item day-of-week is before the start of the month.  Move to the next week.
				date = 7 - (startingDayOfWeek - scheduleItem.DayDate) + 1;
			}
			else {
				date = scheduleItem.DayDate - startingDayOfWeek + 1;
			}

			// If there is no day/date count, assume 1.
			int dayDateCount = Math.Max(1, scheduleItem.DayCount);
			// Get to the correct iteration of the day of the week.
			date += (dayDateCount - 1)*7;

			// If the date exceeds the number of days in the month, back off one week.
			// *** Desired behavior?
			//     Make it configurable? (back date off, ignore instance, last day of month)
			if (date > DateTime.DaysInMonth(year, month)) {
				date -= 7;
			}

			return new DateTime(year, month, date);
		}

		private DateTime _GetDOWLatestOccurrenceDate(IScheduleItem scheduleItem, DateTime monthDate)
		{
			int iterations = (monthDate.Month - scheduleItem.StartDate.Month + (monthDate.Year - scheduleItem.StartDate.Year)*12)/
			                 scheduleItem.DateUnitCount;
			DateTime lastItemMonth = scheduleItem.StartDate.AddMonths(iterations*scheduleItem.DateUnitCount);
			return _GetDOWOccurrenceDate(scheduleItem, lastItemMonth);
		}

		/// <summary>
		/// Gets the last DateTime occurrence of the item between the item's start and the provided month/year date.
		/// </summary>
		/// <param name="scheduleItem"></param>
		/// <param name="monthDate">Month and year, day of the month is not considered.</param>
		/// <returns>Returned DateTime may occur before the provided DateTime.</returns>
		private DateTime _GetDOMLatestOccurrenceDate(IScheduleItem scheduleItem, DateTime monthDate)
		{
			int iterations = (monthDate.Month - scheduleItem.StartDate.Month + (monthDate.Year - scheduleItem.StartDate.Year)*12)/
			                 scheduleItem.DateUnitCount;
			int day = scheduleItem.DayDate == LastDay
			          	? DateTime.DaysInMonth(monthDate.Year, monthDate.Month)
			          	: scheduleItem.DayDate;
			DateTime lastItem = scheduleItem.StartDate.AddMonths(iterations*scheduleItem.DateUnitCount);
			return new DateTime(lastItem.Year, lastItem.Month, day);
		}

		private double _MonthSpan(DateTime earlierDate, DateTime laterDate)
		{
			return (double) (laterDate.Month - earlierDate.Month + (laterDate.Year - earlierDate.Year)*12);
		}

		private double _GetWindowStartRepetitions(IScheduleItem scheduleItem, DateTime windowStart)
		{
			double value = 0;

			switch ((RecurrenceType) scheduleItem.RecurrenceType) {
				case RecurrenceType.Daily:
					value = (double) (windowStart.Date - scheduleItem.StartDate.Date).Days/scheduleItem.DateUnitCount;
					break;
				case RecurrenceType.Weekly:
					// Adjust for the start date not being the first day the item occurs.
					value = (windowStart.Date - scheduleItem.StartDate.Date).Days - _GetDayOfWeekAdjustment(scheduleItem);
					value = value/7/scheduleItem.DateUnitCount;
					break;
				case RecurrenceType.MonthlyDateOfMonth:
					value = _MonthSpan(scheduleItem.StartDate, windowStart)/scheduleItem.DateUnitCount;
					// If the first item occurs before the window, trim it.
					if (scheduleItem.DayDate != LastDay && scheduleItem.DayDate < windowStart.Day) {
						value++;
					}
					break;
				case RecurrenceType.MonthlyDayOfWeek:
					value =
						(double) (windowStart.Month - scheduleItem.StartDate.Month + (windowStart.Year - scheduleItem.StartDate.Year)*12)/
						scheduleItem.DateUnitCount;
					// If the first item occurs before the window, trim it.
					if (_GetDOWOccurrenceDate(scheduleItem, windowStart) < windowStart) {
						value++;
					}
					break;
			}
			return value;
		}

		private double _GetWindowEndRepetitions(IScheduleItem scheduleItem, DateTime windowEnd)
		{
			double value = 0;

			switch ((RecurrenceType) scheduleItem.RecurrenceType) {
				case RecurrenceType.Daily:
					value = (double) (windowEnd.Date - scheduleItem.StartDate.Date).Days/scheduleItem.DateUnitCount;
					break;
				case RecurrenceType.Weekly:
					// Adjust for the start date not being the first day the item occurs.
					value = (windowEnd.Date - scheduleItem.StartDate.Date).Days - _GetDayOfWeekAdjustment(scheduleItem);
					value = value/7/scheduleItem.DateUnitCount;
					break;
				case RecurrenceType.MonthlyDateOfMonth:
					value = _MonthSpan(scheduleItem.StartDate, windowEnd)/scheduleItem.DateUnitCount;
					// If the last item occurs after the window, trim it.
					if (_GetDOMLatestOccurrenceDate(scheduleItem, windowEnd) > windowEnd) {
						value--;
					}
					break;
				case RecurrenceType.MonthlyDayOfWeek:
					value =
						(double) (windowEnd.Month - scheduleItem.StartDate.Month + (windowEnd.Year - scheduleItem.StartDate.Year)*12)/
						scheduleItem.DateUnitCount;
					// If the last item occurs after the window, trim it.
					if (_GetDOWLatestOccurrenceDate(scheduleItem, windowEnd) > windowEnd) {
						value--;
					}
					break;
			}
			return value;
		}

		private int _GetDayOfWeekAdjustment(IScheduleItem scheduleItem)
		{
			// The start date of the item may not fall on the same day of the week
			// that the item occurs on.
			if ((int) scheduleItem.StartDate.DayOfWeek != scheduleItem.DayDate) {
				return 7 - Math.Abs((int) scheduleItem.StartDate.DayOfWeek - scheduleItem.DayDate);
			}
			return 0;
		}

		private IEnumerable<int> _GetWindowOccurrenceIndexes(double windowStartRepetitions, double windowEndRepetitions)
		{
			// Get intervals that occur within the window.
			int startIndex = (int) Math.Ceiling(windowStartRepetitions);
			int count = (int) Math.Floor(windowEndRepetitions) - startIndex + 1; // Inclusive

			// A negative index means that the time window starts before the scheduled item
			// is valid.
			if (startIndex < 0) {
				count += startIndex;
				startIndex = 0;
			}

			return Enumerable.Range(startIndex, count);
		}

		private IEnumerable<int> _GetItemOccurenceIntervals(IScheduleItem scheduleItem, DateTime windowStart,
		                                                    DateTime windowEnd)
		{
			double windowStartRepetitions = _GetWindowStartRepetitions(scheduleItem, windowStart);
			double windowEndRepetitions = _GetWindowEndRepetitions(scheduleItem, windowEnd);

			// The window could potentially cross a time frame (day/week/month) boundary.
			bool windowIsValid = _IncludesIntegerBoundary(windowStartRepetitions, windowEndRepetitions);
			if (windowIsValid) {
				return _GetWindowOccurrenceIndexes(windowStartRepetitions, windowEndRepetitions);
			}
			return Enumerable.Empty<int>();
		}

		private bool _WindowIsValidForItem(IScheduleItem scheduleItem, DateTime windowStart, DateTime windowEnd)
		{
			double windowStartRepetitions = _GetWindowStartRepetitions(scheduleItem, windowStart);
			double windowEndRepetitions = _GetWindowEndRepetitions(scheduleItem, windowEnd);

			// The window could potentially cross a time frame (day/week/month) boundary.
			return _IncludesIntegerBoundary(windowStartRepetitions, windowEndRepetitions);
		}

		private bool _IncludesIntegerBoundary(double leftValue, double rightValue)
		{
			double temp = leftValue;
			leftValue = Math.Min(leftValue, rightValue);
			rightValue = Math.Max(temp, rightValue);

			return
				leftValue == 0
				||
				((Math.Floor(leftValue) != Math.Floor(rightValue)) ||
				 (Math.Ceiling(leftValue) != Math.Ceiling(rightValue)))
				||
				(Math.Floor(leftValue) == Math.Ceiling(rightValue));
		}

		private bool _IncludesSpecificIntegerBoundary(int boundary, double leftValue, double rightValue)
		{
			return (leftValue <= boundary) && (rightValue >= boundary);
		}

		private int _IterationOfDayOfWeek(int dayOfMonth)
		{
			return (dayOfMonth - 1)/7 + 1;
		}

		#endregion
	}
}