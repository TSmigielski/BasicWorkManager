using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;

namespace BasicWorkManager.Services;

public static class DateStuff
{
	public static string GetDayOfWeekName(DateTime _date, PageModel _page)
	{
		var culture = _page.Request.Cookies["lang"] switch
		{
			"Dutch" => "nl",
			"Polish" => "pl",
			_ => "en"
		};

		var day = new CultureInfo(culture).DateTimeFormat.GetDayName(_date.DayOfWeek);
		return char.ToUpper(day[0]) + day.Substring(1);
	}

	public static string GetDayOfWeekName(string _dateString, PageModel _page)
	{
		var culture = _page.Request.Cookies["lang"] switch
		{
			"Dutch" => "nl",
			"Polish" => "pl",
			_ => "en"
		};

		var day = new CultureInfo(culture).DateTimeFormat.GetDayName(DateTime.Parse(_dateString).DayOfWeek);
		return char.ToUpper(day[0]) + day.Substring(1);
	}

	public static string GetDateString(bool _reverse = false)
	{
		var x = DateTime.Now;
		if (!_reverse)
			return $"{x.Year}.{x.Month}.{x.Day}";
		else
			return $"{x.Day}.{x.Month}.{x.Year}";
	}

	public static string GetDateString(DateTime _date, bool _reverse = false)
	{
		var x = _date;
		if (!_reverse)
			return $"{x.Year}.{x.Month}.{x.Day}";
		else
			return $"{x.Day}.{x.Month}.{x.Year}";
	}

	public static string GetDateString(DateOnly _date, bool _reverse = false)
	{
		var x = _date;
		if (!_reverse)
			return $"{x.Year}.{x.Month}.{x.Day}";
		else
			return $"{x.Day}.{x.Month}.{x.Year}";
	}

	public static string GetDateString(string _dateString, bool _reverse = false)
	{
		var x = ParseDateString(_dateString);
		if (!_reverse)
			return $"{x.Year}.{x.Month}.{x.Day}";
		else
			return $"{x.Day}.{x.Month}.{x.Year}";
	}

	public static DateOnly ParseDateString(string _dateString)
	{
		var x = DateOnly.Parse(_dateString, CultureInfo.InvariantCulture);
		return x;
	}
}
