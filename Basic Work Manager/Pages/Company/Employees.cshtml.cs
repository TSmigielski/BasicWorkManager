using System.Globalization;
using System.Text.RegularExpressions;
using BasicWorkManager.Models;
using BasicWorkManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Pages.Account;

[Authorize(Policy = "Moderator")]
[BindProperties]
public class EmployeesModel : PageModel
{
	public Models.Company Company { get; set; }

	[FromQuery(Name = "week")]
	public int? Week { get; set; }
	[FromQuery(Name = "year")]
	public int? Year { get; set; }

	public List<DateTime> DatesInWeek = new();
	public float Total = 0f;

	public EmployeesTranslations localizer;

	public async Task<IActionResult> OnGet()
	{
		if (Week == null || Week == 0 || Year == null || Year == 0)
		{
			var now = DateTime.Now;
			Week = ISOWeek.GetWeekOfYear(now);
			Year = now.Year;
		}

		await UpdateContent();
		return Page();
	}

	public async Task<IActionResult> OnPostChangeWeek(int _week, int _year, int _weeksToAdd)
	{
		if (_week == 0 || _year == 0)
		{
			var now = DateTime.Now;
			_week = ISOWeek.GetWeekOfYear(now);
			_year = now.Year;
		}

		var date = DateTimeExtensions.FirstDayOfWeek(_year, _week);
		date = date.AddDays(7 * _weeksToAdd);

		Week = ISOWeek.GetWeekOfYear(date);
		Year = date.Year;

		await UpdateContent();
		return RedirectToPage("/Company/Employees", new { week = Week, year = Year });
	}

	public string GetUniqueUserID(User _user)
	{
		string x = _user.Username + _user.FirstName + _user.LastName + _user.Company;
		var y = Regex.Replace(x, @"[^0-9a-zA-Z]+", "");
		while (char.IsNumber(y[0]))
			y = y.Substring(1);

		return y;
	}

	public async Task<List<Address>> GetUsedAddresses(User _user, DateTime _date)
	{
		var db = new DataBaseManager();
		var tasks = await db.GetTaskData(_user.Company, _user.Username, _date);

		List<Address> addresses = new();
		foreach (var task in tasks)
		{
			task.ParseAddress();

			if (!addresses.Any(a => a.WriteFullAddress() == task.Address.WriteFullAddress()))
				addresses.Add(task.Address);
		}

		if (addresses.Count > 0)
			addresses[addresses.Count - 1].IsLastAddress = true;

		return addresses;
	}

	private async System.Threading.Tasks.Task UpdateContent()
	{
		localizer = new();

		DataBaseManager db = new();
		Company = await Models.Company.GetCompanyByCookie(this, db);

		Company.Users = await db.GetEmployees(Company.Name);
		Company.Addresses = await db.GetAddresses(Company.Name);
		Company.Tasks = await db.GetTasks(Company.Name);
		Company.SortContent();

		foreach (var user in Company.Users)
		{
			for (DateTime date = DateTimeExtensions.FirstDayOfWeek((int)Year, (int)Week); date <= DateTimeExtensions.LastDayOfWeek((int)Year, (int)Week); date = date.AddDays(1))
			{
				user.TaskDataList.AddRange(await db.GetTaskData(user.Company, user.Username, date));
			}
			foreach (var data in user.TaskDataList)
			{
				data.ParseAddress();
			}
		}
	}

	//private async Task UpdateContent(DataBaseManager _db)
	//{
	//	Company = await Models.Company.GetCompanyByCookie(this, _db);

	//	Company.Users = await _db.GetEmployees(Company.Name);
	//	Company.Addresses = await _db.GetAddresses(Company.Name);
	//	Company.Tasks = await _db.GetTasks(Company.Name);
	//	Company.SortContent();
	//}
}

public class EmployeesTranslations : MyLocalizer
{
	internal override Dictionary<string, string> English
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Employees" },
				{ "Date", "Date" },
				{ "Address", "Address" },
				{ "Week", "Week" },
				{ "Monday", "Monday" },
				{ "Tuesday", "Tuesday" },
				{ "Wednesday", "Wednesday" },
				{ "Thursday", "Thursday" },
				{ "Friday", "Friday" },
				{ "Saturday", "Saturday" },
				{ "Sunday", "Sunday" },
				{ "SubTotal", "Sub Total" },
				{ "Total", "Total" },
			};
		}
	}

	internal override Dictionary<string, string> Dutch
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Medewerkers" },
				{ "Date", "Datum" },
				{ "Address", "Adres" },
				{ "Week", "Week" },
				{ "Monday", "Maandag" },
				{ "Tuesday", "Dinsdag" },
				{ "Wednesday", "Woensdag" },
				{ "Thursday", "Donderdag" },
				{ "Friday", "Vrijdag" },
				{ "Saturday", "Zaterdag" },
				{ "Sunday", "Zondag" },
				{ "SubTotal", "Subtotaal" },
				{ "Total", "Totaal" },
			};
		}
	}

	internal override Dictionary<string, string> Polish
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Pracownicy" },
				{ "Date", "Data" },
				{ "Address", "Adres" },
				{ "Week", "Tydzieñ" },
				{ "Monday", "Poniedzia³ek" },
				{ "Tuesday", "Wtorek" },
				{ "Wednesday", "Œroda" },
				{ "Thursday", "Czwartek" },
				{ "Friday", "Pi¹tek" },
				{ "Saturday", "Sobota" },
				{ "Sunday", "Niedziela" },
				{ "SubTotal", "Suma Czêœciowa" },
				{ "Total", "Suma Ca³kowita" },
			};
		}
	}
}