using System.Globalization;
using BasicWorkManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Pages;

[Authorize]
public class CalendarModel : PageModel
{
	[FromQuery(Name = "date")]
	public string DateString { get; set; }
	public int Week { get; set; }

	[BindProperty]
	public List<DateTime> DatesWithTaskDone { get; set; } = new();

	public CalendarTranslations localizer;

	public IActionResult OnGet()
	{
		localizer = new();

		if (DateString == null)
			DateString = DateStuff.GetDateString();

		return Page();
	}

	public IActionResult OnPost(string _dateString, int _months)
	{
		if (_dateString == null)
			_dateString = DateStuff.GetDateString();

		DateString = DateStuff.GetDateString(DateTimeExtensions.FirstDayOfAdjacentMonth(DateStuff.ParseDateString(_dateString), _months));

		return RedirectToPage("/Calendar", new { date = DateString });
	}

	public async Task<bool> CheckDate(DateTime _date)
	{
		DataBaseManager db = new();
		var user = await Models.User.GetUserByCookie(this, db);
		var tasks = await db.GetTaskData(user.Company, user.Username, _date);
		return tasks.Count != 0 && tasks[0] != null ? true : false;
	}

	public string GetMonthName()
	{
		var culture = Request.Cookies["lang"] switch
		{
			"Dutch" => "nl",
			"Polish" => "pl",
			_ => "en"
		};

		var month = new CultureInfo(culture).DateTimeFormat.GetMonthName(DateTime.Parse(DateString).Month);
		return char.ToUpper(month[0]) + month.Substring(1);
	}

	public int GetFirstWeek()
	{
		var week = ISOWeek.GetWeekOfYear(DateTimeExtensions.FirstDayOfMonth(DateTime.Parse(DateString)));

		if (week >= 52)
			week = 0;

		return week;
	}

	public int GetLastWeek()
	{
		var week = ISOWeek.GetWeekOfYear(DateTimeExtensions.LastDayOfMonth(DateTime.Parse(DateString)));

		if (week == 1)
			week = 53;

		return week;
	}
}

public class CalendarTranslations : MyLocalizer
{
	internal override Dictionary<string, string> English
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Calendar" },
				{ "CurrentMonth", "Go to today's month" },
				{ "Week", "Week" },
				{ "Monday", "Monday" },
				{ "Tuesday", "Tuesday" },
				{ "Wednesday", "Wednesday" },
				{ "Thursday", "Thursday" },
				{ "Friday", "Friday" },
				{ "Saturday", "Saturday" },
				{ "Sunday", "Sunday" },
				{ "ViewTasks", "View Tasks" },
			};
		}
	}

	internal override Dictionary<string, string> Dutch
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Kalender" },
				{ "CurrentMonth", "Ga naar de maand van vandaag" },
				{ "Week", "Week" },
				{ "Monday", "Maandag" },
				{ "Tuesday", "Dinsdag" },
				{ "Wednesday", "Woensdag" },
				{ "Thursday", "Donderdag" },
				{ "Friday", "Vrijdag" },
				{ "Saturday", "Zaterdag" },
				{ "Sunday", "Zondag" },
				{ "ViewTasks", "Bekijk Taken" },
			};
		}
	}

	internal override Dictionary<string, string> Polish
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Kalendarz" },
				{ "CurrentMonth", "PrzejdŸ do dzisiejszego miesi¹ca" },
				{ "Week", "Tydzieñ" },
				{ "Monday", "Poniedzia³ek" },
				{ "Tuesday", "Wtorek" },
				{ "Wednesday", "Œroda" },
				{ "Thursday", "Czwartek" },
				{ "Friday", "Pi¹tek" },
				{ "Saturday", "Sobota" },
				{ "Sunday", "Niedziela" },
				{ "ViewTasks", "Wyœwietl Zadania" },
			};
		}
	}
}