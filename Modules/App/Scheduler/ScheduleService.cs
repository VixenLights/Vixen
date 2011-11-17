using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VixenModules.App.Scheduler {
	class ScheduleService {
		private SchedulerData _data;

		public ScheduleService(SchedulerData data) {
			_data = data;
		}

		/// <summary>
		/// Used for events with MonthlyDateOfMonth recurrence.
		/// </summary>
		public const int LastDay = -1;

		/// <summary>
		/// Gets specific DateTime instances of ScheduleItems that will occur within the specified time frame.
		/// </summary>
		/// <param name="events">Collection of ScheduleItems the scheduler will operate upon</param>
		/// <param name="startDateTime">Start date and time of the window within which schedule events should be considered valid.</param>
		/// <param name="endDateTime">End date and time of the window within which schedule events should be considered valid.  This value is inclusive.</param>
		/// <returns></returns>
		public DateTime[] GetEventInstances(IEnumerable<ScheduleItem> events, DateTime startDateTime, DateTime endDateTime)
		{
			List<DateTime> eventInstances = new List<DateTime>();

			if(events != null)
			{
				// Daily
				eventInstances.AddRange(
					events
						.Where(x => x.RecurrenceType == (int)RecurrenceType.Daily)
						.SelectMany<ScheduleItem, DateTime>(x => _GetDailyInstances(x, startDateTime, endDateTime))
						);

				// Weekly
				eventInstances.AddRange(
					events
						.Where(x => x.RecurrenceType == RecurrenceType.Weekly)
						.SelectMany<ScheduleItem, DateTime>(x => _GetWeeklyInstances(x, startDateTime, endDateTime))
						);

				// Monthly - Date of month
				eventInstances.AddRange(
					events
						.Where(x => x.RecurrenceType == RecurrenceType.MonthlyDateOfMonth)
						.SelectMany<ScheduleItem, DateTime>(x => _GetMonthlyDOMInstances(x, startDateTime, endDateTime))
						);

				// Monthly - Day of week
				eventInstances.AddRange(
					events
						.Where(x => x.RecurrenceType == RecurrenceType.MonthlyDayOfWeek)
						.SelectMany<ScheduleItem, DateTime>(x => _GetMonthlyDOWInstances(x, startDateTime, endDateTime))
						);
			}

			return eventInstances.ToArray();
		}

		/// <summary>
		/// Get the ScheduleItem instances from all events that are scheduled to be valid right now,
		/// within the timeWindow span.
		/// </summary>
		/// <param name="events">Collection of ScheduleItems the scheduler will operate upon</param>
		/// <param name="timeWindow">Span of time starting at the current time
		/// within which schedule events should be considered valid.</param>
		/// <returns></returns>
		public ScheduleItem[] GetValidEvents(TimeSpan timeWindow)
		{
			return GetValidEvents(_data.Events, DateTime.Now, DateTime.Now + timeWindow);
		}

		/// <summary>
		/// Get the ScheduleItem instances from the provided collection that are scheduled to be valid right now,
		/// within the timeWindow span.
		/// </summary>
		/// <param name="events">Collection of ScheduleItems the scheduler will operate upon</param>
		/// <param name="timeWindow">Span of time starting at the current time
		/// within which schedule events should be considered valid.</param>
		/// <returns></returns>
		public ScheduleItem[] GetValidEvents(IEnumerable<ScheduleItem> events, TimeSpan timeWindow)
		{
			return GetValidEvents(events, DateTime.Now, DateTime.Now + timeWindow);
		}
		
		/// <summary>
		/// Get the ScheduleItem instances from all events that are scheduled within the dates 
		/// specified, inclusively.
		/// </summary>
		/// <param name="events">Collection of ScheduleItems the scheduler will operate upon</param>
		/// <param name="startDateTime">Start date and time of the window within which schedule events should be considered valid.</param>
		/// <param name="endDateTime">End date and time of the window within which schedule events should be considered valid.  This value is inclusive.</param>
		/// <returns></returns>
		public ScheduleItem[] GetValidEvents(DateTime startDateTime, DateTime endDateTime)
		{
			return GetValidEvents(_data.Events, startDateTime, endDateTime);
		}

		/// <summary>
		/// Get the ScheduleItem instances from the provided collection that are scheduled within the dates 
		/// specified, inclusively.
		/// </summary>
		/// <param name="events">Collection of ScheduleItems the scheduler will operate upon</param>
		/// <param name="startDateTime">Start date and time of the window within which schedule events should be considered valid.</param>
		/// <param name="endDateTime">End date and time of the window within which schedule events should be considered valid.  This value is inclusive.</param>
		/// <returns></returns>
		public ScheduleItem[] GetValidEvents(IEnumerable<ScheduleItem> events, DateTime startDateTime, DateTime endDateTime)
		{
			List<ScheduleItem> validEvents = new List<ScheduleItem>();

			if(events != null)
			{
				foreach(ScheduleItem scheduleEvent in events)
				{
					// If the event's start date has not yet arrived or
					// the event's time is not within the time window, 
					// it's automatically invalid.
					if(startDateTime >= scheduleEvent.StartDate &&
						scheduleEvent.RunTime >= startDateTime.TimeOfDay &&
						scheduleEvent.RunTime <= endDateTime.TimeOfDay)
					{
						switch((RecurrenceType)scheduleEvent.RecurrenceType)
						{
							case RecurrenceType.Daily:
								if(_IsDailyValid(scheduleEvent, startDateTime, endDateTime))
								{
									validEvents.Add(scheduleEvent);
								}
								break;
							case RecurrenceType.Weekly:
								if(_IsWeeklyValid(scheduleEvent, startDateTime, endDateTime))
								{
									validEvents.Add(scheduleEvent);
								}
								break;
							case RecurrenceType.MonthlyDateOfMonth:
								if(_IsMonthlyDOMValid(scheduleEvent, startDateTime, endDateTime))
								{
									validEvents.Add(scheduleEvent);
								}
								break;
							case RecurrenceType.MonthlyDayOfWeek:
								if(_IsMonthlyDOWValid(scheduleEvent, startDateTime, endDateTime))
								{
									validEvents.Add(scheduleEvent);
								}
								break;
						}
					}
				}

			}

			return validEvents.ToArray();
		}

		/// <summary>
		/// Provides a readable text summary of a schedule event.
		/// </summary>
		/// <param name="scheduleEvent"></param>
		/// <returns></returns>
		public string GetEventSummary(ScheduleItem scheduleEvent)
		{
			StringBuilder sb = new StringBuilder();
			RecurrenceType recurrenceType = (RecurrenceType)scheduleEvent.RecurrenceType;

			string unit = "";
			switch(recurrenceType)
			{
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
			if(scheduleEvent.UnitCount > 1)
			{
				sb.AppendFormat(" {0} {1}s", scheduleEvent.UnitCount, unit);
			}
			else
			{
				sb.Append(unit);
			}

			switch(recurrenceType)
			{
				case RecurrenceType.Daily:
					// Nothing to do
					break;
				case RecurrenceType.Weekly:
					sb.Append(" on " + (DayOfWeek.Sunday + scheduleEvent.DayDate).ToString());
					break;
				case RecurrenceType.MonthlyDateOfMonth:
					if(scheduleEvent.DayDate != LastDay)
					{
						sb.Append(" on the " + _AppendCount(scheduleEvent.DayDate));
					}
					else
					{
						sb.Append(" on the last day of the month ");
					}
					break;
				case RecurrenceType.MonthlyDayOfWeek:
					sb.AppendFormat(" on the {0} {1}", _AppendCount(scheduleEvent.DayDateCount), (DayOfWeek.Sunday + scheduleEvent.DayDate).ToString());
					break;
			}

			sb.AppendFormat(" at " + scheduleEvent.RunTime.ToString("hh:mm tt"));

			sb.AppendFormat(" starting on " + scheduleEvent.StartDate.ToShortDateString());

			return sb.ToString();
		}





		#region Private
		private string _AppendCount(int count)
		{
			int dateRemainder = count % 10;
			string appendage;

			if(dateRemainder == 1)
			{
				appendage = "st";
			}
			else if(dateRemainder == 2)
			{
				appendage = "nd";
			}
			else if(dateRemainder == 3)
			{
				appendage = "rd";
			}
			else
			{
				appendage = "th";
			}

			return count.ToString() + appendage;
		}

		private bool _IsDailyValid(ScheduleItem scheduleEvent, DateTime windowStart, DateTime windowEnd)
		{
			return _WindowIsValidForEvent(scheduleEvent, windowStart, windowEnd);
		}

		private bool _IsWeeklyValid(ScheduleItem scheduleEvent, DateTime windowStart, DateTime windowEnd)
		{
			if(_WindowIsValidForEvent(scheduleEvent, windowStart, windowEnd))
			{
				// The window could potentially cross a day boundary.
				// The event is valid if the window crosses into the event's day.
				return (int)windowStart.DayOfWeek == scheduleEvent.DayDate || (int)windowEnd.DayOfWeek == scheduleEvent.DayDate;
			}
			return false;
		}

		private bool _IsMonthlyDOMValid(ScheduleItem scheduleEvent, DateTime windowStart, DateTime windowEnd)
		{
			if(_WindowIsValidForEvent(scheduleEvent, windowStart, windowEnd))
			{
				// The window could potentially cross a date boundary.
				// The event is valid if the window crosses into the event's date.
				if(scheduleEvent.DayDate != LastDay)
				{
					return windowStart.Day == scheduleEvent.DayDate || windowEnd.Day == scheduleEvent.DayDate;
				}
				else
				{
					// If the event's DayDate value is LastDay, adjust it for the last day of 
					// the current month.
					return windowStart.Day == DateTime.DaysInMonth(windowStart.Year, windowStart.Month) || windowEnd.Day == DateTime.DaysInMonth(windowEnd.Year, windowEnd.Month);
				}
			}
			return false;
		}

		private bool _IsMonthlyDOWValid(ScheduleItem scheduleEvent, DateTime windowStart, DateTime windowEnd)
		{
			if(_WindowIsValidForEvent(scheduleEvent, windowStart, windowEnd))
			{
				// The window could potentially cross a day boundary.
				// The event is valid if the window crosses into the event's day.
				if((int)windowStart.DayOfWeek == scheduleEvent.DayDate || (int)windowEnd.DayOfWeek == scheduleEvent.DayDate)
				{
					// Window crosses the correct day.
					// Does it cross the correct iteration of the day?
					return
						_IterationOfDayOfWeek(windowStart.Day) == scheduleEvent.DayDateCount ||
						_IterationOfDayOfWeek(windowEnd.Day) == scheduleEvent.DayDateCount;
				}
			}
			return false;
		}

		private IEnumerable<DateTime> _GetDailyInstances(ScheduleItem scheduleEvent, DateTime windowStart, DateTime windowEnd)
		{
			IEnumerable<int> intervals = _GetEventOccurenceIntervals(scheduleEvent, windowStart, windowEnd);
			// Create DateTime instances for each occurrence.
			DateTime instance = scheduleEvent.StartDate + scheduleEvent.RunTime;
			return
				from interval in intervals
				select instance.AddDays(interval * scheduleEvent.UnitCount);
		}

		private IEnumerable<DateTime> _GetWeeklyInstances(ScheduleItem scheduleEvent, DateTime windowStart, DateTime windowEnd)
		{
			IEnumerable<int> intervals = _GetEventOccurenceIntervals(scheduleEvent, windowStart, windowEnd);
			// Create DateTime instances for each occurrence.
			DateTime instance = scheduleEvent.StartDate + scheduleEvent.RunTime;
			// Need to start on the correct day of the week at/after the event start date.
			instance = instance.AddDays(_GetDayOfWeekAdjustment(scheduleEvent));
			return
				from interval in intervals
				select instance.AddDays(7 * interval * scheduleEvent.UnitCount);
		}

		private IEnumerable<DateTime> _GetMonthlyDOMInstances(ScheduleItem scheduleEvent, DateTime windowStart, DateTime windowEnd)
		{
			// Create DateTime instances for each occurrence.
			IEnumerable<int> intervals = _GetEventOccurenceIntervals(scheduleEvent, windowStart, windowEnd);
			// Create DateTime instances for each occurrence.
			// This is the only event type where the event date is absolute rather
			// than relative to the start date.
			DateTime startDate = scheduleEvent.StartDate;
			if(scheduleEvent.DayDate != LastDay)
			{
				DateTime instance = new DateTime(startDate.Year, startDate.Month, scheduleEvent.DayDate) + scheduleEvent.RunTime;
				return
					from interval in intervals
					select instance.AddMonths(interval * scheduleEvent.UnitCount);
			}
			else
			{
				// If the event's DayDate value is LastDay, adjust each for the last day of 
				// the month.

				// Start with an initial date based on the event start date.  Day of the
				// month is unimportant.
				DateTime instance = new DateTime(startDate.Year, startDate.Month, 1);
				return intervals.Select<int, DateTime>(interval =>
				{
					// Offset the initial date by an appropriate number of months.
					DateTime offsetInstance = instance.AddMonths(interval * scheduleEvent.UnitCount);
					// With the date now being in the correct year and month, create a
					// new date with the day set correctly.
					return new DateTime(offsetInstance.Year, offsetInstance.Month, DateTime.DaysInMonth(offsetInstance.Year, offsetInstance.Month)) + scheduleEvent.RunTime;
				});
			}
		}

		private IEnumerable<DateTime> _GetMonthlyDOWInstances(ScheduleItem scheduleEvent, DateTime windowStart, DateTime windowEnd)
		{
			// Create DateTime instances for each occurrence.
			List<int> intervals = new List<int>(_GetEventOccurenceIntervals(scheduleEvent, windowStart, windowEnd));
			// Start in the month of the event's start date.
			DateTime instance = _GetDOWOccurrenceDate(scheduleEvent, scheduleEvent.StartDate);
			return
				from interval in intervals
				select _GetDOWOccurrenceDate(scheduleEvent, instance.AddMonths(interval * scheduleEvent.UnitCount));
		}

		private DateTime _GetDOWOccurrenceDate(ScheduleItem scheduleEvent, DateTime monthDate)
		{
			return _GetDOWOccurrenceDate(scheduleEvent, monthDate.Year, monthDate.Month);
		}

		private DateTime _GetDOWOccurrenceDate(ScheduleItem scheduleEvent, int year, int month)
		{
			// An example event occurrence would be: 3rd Tuesday

			// Get the day of the week for the first day of the month.
			// (i.e. Month starts on a Thursday)
			int startingDayOfWeek = (int)new DateTime(year, month, 1).DayOfWeek;
			
			// Get the date of the first of the event's day-of-week for the month.
			// (i.e. Event happens on a Tuesday, get the first Tuesday of the month after the start of the month)
			int date;
			if(scheduleEvent.DayDate < startingDayOfWeek)
			{
				// Event day-of-week is before the start of the month.  Move to the next week.
				date = 7 - (startingDayOfWeek - scheduleEvent.DayDate) + 1;
			}
			else
			{
				date = scheduleEvent.DayDate - startingDayOfWeek + 1;
			}

			// If there is no day/date count, assume 1.
			int dayDateCount = Math.Max(1, scheduleEvent.DayDateCount);
			// Get to the correct iteration of the day of the week.
			date += (dayDateCount - 1) * 7;

			// If the date exceeds the number of days in the month, back off one week.
			// *** Desired behavior?
			//     Make it configurable? (back date off, ignore instance, last day of month)
			if(date > DateTime.DaysInMonth(year, month))
			{
				date -= 7;
			}

			return new DateTime(year, month, date);
		}

		private DateTime _GetDOWLatestOccurrenceDate(ScheduleItem scheduleEvent, DateTime monthDate)
		{
			int iterations = (monthDate.Month - scheduleEvent.StartDate.Month + (monthDate.Year - scheduleEvent.StartDate.Year) * 12) / scheduleEvent.UnitCount;
			DateTime lastEventMonth = scheduleEvent.StartDate.AddMonths(iterations * scheduleEvent.UnitCount);
			return _GetDOWOccurrenceDate(scheduleEvent, lastEventMonth);
		}

		/// <summary>
		/// Gets the last DateTime occurrence of the event between the event's start and the provided month/year date.
		/// </summary>
		/// <param name="scheduleEvent"></param>
		/// <param name="monthDate">Month and year, day of the month is not considered.</param>
		/// <returns>Returned DateTime may occur before the provided DateTime.</returns>
		private DateTime _GetDOMLatestOccurrenceDate(ScheduleItem scheduleEvent, DateTime monthDate)
		{
			int iterations = (monthDate.Month - scheduleEvent.StartDate.Month + (monthDate.Year - scheduleEvent.StartDate.Year) * 12) / scheduleEvent.UnitCount;
			int day = scheduleEvent.DayDate == LastDay ? DateTime.DaysInMonth(monthDate.Year, monthDate.Month) : scheduleEvent.DayDate;
			DateTime lastEvent = scheduleEvent.StartDate.AddMonths(iterations * scheduleEvent.UnitCount);
			return new DateTime(lastEvent.Year, lastEvent.Month, day);
		}

		private double _MonthSpan(DateTime earlierDate, DateTime laterDate)
		{
			return (double)(laterDate.Month - earlierDate.Month + (laterDate.Year - earlierDate.Year) * 12);
		}

		private double _GetWindowStartRepetitions(ScheduleItem scheduleEvent, DateTime windowStart)
		{
			double value = 0;

			switch((RecurrenceType)scheduleEvent.RecurrenceType)
			{
				case RecurrenceType.Daily:
					value = (double)(windowStart.Date - scheduleEvent.StartDate.Date).Days / scheduleEvent.UnitCount;
					break;
				case RecurrenceType.Weekly:
					// Adjust for the start date not being the first day the event occurs.
					value = (windowStart.Date - scheduleEvent.StartDate.Date).Days - _GetDayOfWeekAdjustment(scheduleEvent);
					value = value / 7 / scheduleEvent.UnitCount;
					break;
				case RecurrenceType.MonthlyDateOfMonth:
					value = _MonthSpan(scheduleEvent.StartDate, windowStart) / scheduleEvent.UnitCount;
					// If the first event occurs before the window, trim it.
					if(scheduleEvent.DayDate != LastDay && scheduleEvent.DayDate < windowStart.Day)
					{
						value++;
					}
					break;
				case RecurrenceType.MonthlyDayOfWeek:
					value = (double)(windowStart.Month - scheduleEvent.StartDate.Month + (windowStart.Year - scheduleEvent.StartDate.Year) * 12) / scheduleEvent.UnitCount;
					// If the first event occurs before the window, trim it.
					if(_GetDOWOccurrenceDate(scheduleEvent, windowStart) < windowStart) {
						value++;
					}
					break;
			}
			return value;
		}

		private double _GetWindowEndRepetitions(ScheduleItem scheduleEvent, DateTime windowEnd)
		{
			double value = 0;

			switch((RecurrenceType)scheduleEvent.RecurrenceType)
			{
				case RecurrenceType.Daily:
					value = (double)(windowEnd.Date - scheduleEvent.StartDate.Date).Days / scheduleEvent.UnitCount;
					break;
				case RecurrenceType.Weekly:
					// Adjust for the start date not being the first day the event occurs.
					value = (windowEnd.Date - scheduleEvent.StartDate.Date).Days - _GetDayOfWeekAdjustment(scheduleEvent);
					value = value / 7 / scheduleEvent.UnitCount;
					break;
				case RecurrenceType.MonthlyDateOfMonth:
					value =  _MonthSpan(scheduleEvent.StartDate, windowEnd) / scheduleEvent.UnitCount;
					// If the last event occurs after the window, trim it.
					if(_GetDOMLatestOccurrenceDate(scheduleEvent, windowEnd) > windowEnd)
					{
						value--;
					}
					break;
				case RecurrenceType.MonthlyDayOfWeek:
					value = (double)(windowEnd.Month - scheduleEvent.StartDate.Month + (windowEnd.Year - scheduleEvent.StartDate.Year) * 12) / scheduleEvent.UnitCount;
					// If the last event occurs after the window, trim it.
					if(_GetDOWLatestOccurrenceDate(scheduleEvent, windowEnd) > windowEnd) {
						value--;
					}
					break;
			}
			return value;
		}

		private int _GetDayOfWeekAdjustment(ScheduleItem scheduleEvent)
		{
			// The start date of the event may not fall on the same day of the week
			// that the event occurs on.
			if((int)scheduleEvent.StartDate.DayOfWeek != scheduleEvent.DayDate)
			{
				return 7 - Math.Abs((int)scheduleEvent.StartDate.DayOfWeek - scheduleEvent.DayDate);
			}
			return 0;
		}

		private IEnumerable<int> _GetWindowOccurrenceIndexes(double windowStartRepetitions, double windowEndRepetitions)
		{
			// Get intervals that occur within the window.
			int startIndex = (int)Math.Ceiling(windowStartRepetitions);
			int count = (int)Math.Floor(windowEndRepetitions) - startIndex + 1; // Inclusive

			// A negative index means that the time window starts before the scheduled event
			// is valid.
			if(startIndex < 0)
			{
				count += startIndex;
				startIndex = 0;
			}

			return Enumerable.Range(startIndex, count);
		}

		private IEnumerable<int> _GetEventOccurenceIntervals(ScheduleItem scheduleEvent, DateTime windowStart, DateTime windowEnd)
		{
			double windowStartRepetitions = _GetWindowStartRepetitions(scheduleEvent, windowStart);
			double windowEndRepetitions = _GetWindowEndRepetitions(scheduleEvent, windowEnd);

			// The window could potentially cross a time frame (day/week/month) boundary.
			bool windowIsValid = _IncludesIntegerBoundary(windowStartRepetitions, windowEndRepetitions);
			if(windowIsValid)
			{
				return _GetWindowOccurrenceIndexes(windowStartRepetitions, windowEndRepetitions);
			}
			return Enumerable.Empty<int>();
		}

		private bool _WindowIsValidForEvent(ScheduleItem scheduleEvent, DateTime windowStart, DateTime windowEnd)
		{
			double windowStartRepetitions = _GetWindowStartRepetitions(scheduleEvent, windowStart);
			double windowEndRepetitions = _GetWindowEndRepetitions(scheduleEvent, windowEnd);

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
			return (dayOfMonth - 1) / 7 + 1;
		}
		#endregion
	
	}
}
