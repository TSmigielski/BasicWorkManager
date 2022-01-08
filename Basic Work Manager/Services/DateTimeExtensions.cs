using System.Globalization;

namespace BasicWorkManager.Services;
public static class DateTimeExtensions
{
	public static DateTime FirstDayOfWeek(this DateTime dt)
	{
		var culture = Thread.CurrentThread.CurrentCulture;
		var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;

		if (diff < 0)
		{
			diff += 7;
		}

		return dt.AddDays(-diff).Date;
	}

	public static DateTime LastDayOfWeek(this DateTime dt) =>
		dt.FirstDayOfWeek().AddDays(6);

	public static DateTime FirstDayOfMonth(this DateTime dt) =>
		new(dt.Year, dt.Month, 1);

	public static DateTime LastDayOfMonth(this DateTime dt) =>
		dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);

	public static DateTime FirstDayOfNextMonth(this DateTime dt) =>
		dt.FirstDayOfMonth().AddMonths(1);

	public static DateTime FirstDayOfAdjacentMonth(this DateTime dt, int months) =>
		dt.FirstDayOfMonth().AddMonths(months);

	public static DateTime FirstDayOfWeek(int year, int weekOfYear)
	{
		DateTime jan1 = new DateTime(year, 1, 1);
		int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

		// Use first Thursday in January to get first week of the year as
		// it will never be in Week 52/53
		DateTime firstThursday = jan1.AddDays(daysOffset);
		var cal = CultureInfo.CurrentCulture.Calendar;
		int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

		var weekNum = weekOfYear;
		// As we're adding days to a date in Week 1,
		// we need to subtract 1 in order to get the right date for week #1
		if (firstWeek == 1)
		{
			weekNum -= 1;
		}

		// Using the first Thursday as starting week ensures that we are starting in the right year
		// then we add number of weeks multiplied with days
		var result = firstThursday.AddDays(weekNum * 7);

		// Subtract 3 days from Thursday to get Monday, which is the first weekday in ISO8601
		return result.AddDays(-3);
	}

	public static DateTime LastDayOfWeek(int year, int weekOfYear) =>
		FirstDayOfWeek(year, weekOfYear).AddDays(6);
}
