using BasicWorkManager.Models;
using BasicWorkManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Pages;

[BindProperties]
[Authorize]
public class TasksModel : PageModel
{
	public User MyUser { get; set; }
	public Models.Company Company { get; set; }
	public Dictionary<string, string> Data { get; set;} = new();

	[FromQuery(Name = "date")]
	public string DateString { get; set; }

	public TasksTranslations localizer;

	public async Task<IActionResult> OnGet()
	{
		if (DateString == null)
			DateString = DateOnly.FromDateTime(DateTime.Now).ToString();

		await UpdateContent();
		return Page();
	}

	public async Task<IActionResult> OnPost()
	{
		if (DateString == null)
			DateString = DateOnly.FromDateTime(DateTime.Now).ToString();

		var db = new DataBaseManager();
		var user = await Models.User.GetUserByCookie(this, db);

		var tasks = new List<System.Threading.Tasks.Task>();

		foreach (var d in Data)
		{
			var keys = d.Key.Split('/');

			if (d.Value == null)
			{
				tasks.Add(db.DeleteTaskData(user.Company, user.Username, keys[2], DateOnly.Parse(keys[0]), keys[1]));
				continue;
			}

			tasks.Add(db.InsertTaskData(user.Company, user.Username, keys[2], DateOnly.Parse(keys[0]), keys[1], d.Value));
		}

		await System.Threading.Tasks.Task.WhenAll(tasks);

		await UpdateContent(db);
		return Page();
	}

	public async Task<IActionResult> OnPostDateChange(string _dateString, int _days)
	{
		if (_dateString == null)
			_dateString = DateOnly.FromDateTime(DateTime.Now).ToString();

		DateString = DateOnly.Parse(_dateString).AddDays(_days).ToString();

		await UpdateContent();
		return RedirectToPage("/Tasks", new { date = DateString });
	}

	private async System.Threading.Tasks.Task UpdateContent()
	{
		localizer = new();

		DataBaseManager db = new();
		MyUser = await Models.User.GetUserByCookie(this, db);
		Company = await Models.Company.GetCompanyByCookie(this, db);

		MyUser.TaskDataList = await db.GetTaskData(MyUser.Company, MyUser.Username, DateOnly.Parse(DateString));

		for (int i = MyUser.TaskDataList.Count - 1; i >= 0; i--)
		{
			if (MyUser.TaskDataList[i].Data == null)
				MyUser.TaskDataList.RemoveAt(i);

			MyUser.TaskDataList[i].ParseProperties();
		}

		Company.Addresses = await db.GetAddresses(Company.Name);
		Company.Tasks = await db.GetTasks(Company.Name);
		Company.SortContent();

		InitializeDataFields();
	}

	private async System.Threading.Tasks.Task UpdateContent(DataBaseManager _db)
	{
		localizer = new();

		MyUser = await Models.User.GetUserByCookie(this, _db);
		Company = await Models.Company.GetCompanyByCookie(this, _db);

		MyUser.TaskDataList = await _db.GetTaskData(MyUser.Company, MyUser.Username, DateOnly.Parse(DateString));

		for (int i = MyUser.TaskDataList.Count - 1; i >= 0; i--)
		{
			if (MyUser.TaskDataList[i].Data == null)
				MyUser.TaskDataList.RemoveAt(i);

			MyUser.TaskDataList[i].ParseProperties();
		}

		Company.Addresses = await _db.GetAddresses(Company.Name);
		Company.Tasks = await _db.GetTasks(Company.Name);
		Company.SortContent();

		InitializeDataFields();
	}

	private void InitializeDataFields()
	{
		foreach (var address in Company.Addresses)
		{
			foreach (var task in Company.Tasks)
			{
				if (!Data.ContainsKey($"{DateOnly.Parse(DateString)}/{address.WriteFullAddress()}/{task.Name}"))
					Data.Add($"{DateOnly.Parse(DateString)}/{address.WriteFullAddress()}/{task.Name}", "");
			}
		}
	}
}

public class TasksTranslations : MyLocalizer
{
	internal override Dictionary<string, string> English
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Tasks" },
				{ "Go2Today", "Go to today" },
				{ "Week", "Week" },
				{ "Address", "Address" },
				{ "Total", "Total" },
				{ "Save", "Save" },
			};
		}
	}

	internal override Dictionary<string, string> Dutch
	{
		get
		{
			return new Dictionary<string, string>
			{
				{ "Title", "Taken" },
				{ "Go2Today", "Ga naar vandaag" },
				{ "Week", "Week" },
				{ "Address", "Adres" },
				{ "Total", "Totaal" },
				{ "Save", "Opslaan" },
			};
		}
	}

	internal override Dictionary<string, string> Polish
	{
		get
		{
			return new Dictionary<string, string>
			{
				{ "Title", "Zadania" },
				{ "Go2Today", "IdŸ do dzisiejszego dnia" },
				{ "Week", "Tydzieñ" },
				{ "Address", "Adres" },
				{ "Total", "Suma" },
				{ "Save", "Zapisz" },
			};
		}
	}
}