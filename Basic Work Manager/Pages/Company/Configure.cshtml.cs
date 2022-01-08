using System;
using BasicWorkManager.Models;
using BasicWorkManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Pages.Company;

[BindProperties]
[Authorize(Policy = "Admin")]
public class ConfigureModel : PageModel
{
	public Models.Company Company { get; set; }
	public string EmailAddress { get; set; }

	public async Task<IActionResult> OnGet()
	{
		await UpdateContent();

		return Page();
	}

	public ConfigureCompanyTranslations localizer;

	public async Task<IActionResult> OnPostInviteEmployee()
	{
		var db = new DataBaseManager();
		var employee = await db.GetUser(EmailAddress);
		
		if (employee == null)
		{
			await UpdateContent(db);
			return Page();
		}

		var user = await Models.User.GetUserByCookie(this, db);

		await TokenHandler.GenerateToken(employee, TokenType.CompanyInvitation, user.Company);

		await UpdateContent(db);
		return Page();
	}

	public async Task<IActionResult> OnPostRemoveEmployee(string _username)
	{
		var db = new DataBaseManager();
		await db.ChangeCompany(_username, null);
		await db.ChangeUserRole(_username, UserRole.Regular);

		await UpdateContent(db);
		return Page();
	}

	public async Task<IActionResult> OnPostChangeRole(string _username, UserRole _userRole)
	{
		var db = new DataBaseManager();
		await db.ChangeUserRole(_username, _userRole);

		await UpdateContent(db);
		return Page();
	}

	public async Task<IActionResult> OnPostCreateAddress(string _city, string _street, string _houseNumber, string _country = null, string _postalCode = null)
	{
		var db = new DataBaseManager();
		var company = await Models.Company.GetCompanyByCookie(this, db);
		await db.CreateAddress(company.Name, _city, _street, _houseNumber, _country, _postalCode);

		await UpdateContent(db);
		return Page();
	}

	public async Task<IActionResult> OnPostRemoveAddress(string _city, string _street, string _houseNumber)
	{
		var db = new DataBaseManager();
		var company = await Models.Company.GetCompanyByCookie(this, db);
		await db.RemoveAddress(company.Name, _city, _street, _houseNumber);

		await UpdateContent(db);
		return Page();
	}

	public async Task<IActionResult> OnPostChangeTaskFrequency(TaskFrequency _taskFrequency)
	{
		var db = new DataBaseManager();
		var company = await Models.Company.GetCompanyByCookie(this, db);
		await db.ChangeTaskFrequency(company.Name, _taskFrequency);

		await UpdateContent(db);
		return Page();
}

	public async Task<IActionResult> OnPostCreateTask(string _taskName, Models.ValueType _valueType, string _taskDescription = null, int _order = 0)
	{
		var db = new DataBaseManager();
		var company = await Models.Company.GetCompanyByCookie(this, db);
		await db.CreateTask(company.Name, _taskName, _taskDescription, _order, _valueType);

		await UpdateContent(db);
		return Page();
	}

	public async Task<IActionResult> OnPostRemoveTask(string _taskName)
	{
		var db = new DataBaseManager();
		var company = await Models.Company.GetCompanyByCookie(this, db);
		await db.RemoveTask(company.Name, _taskName);

		await UpdateContent(db);
		return Page();
	}
	
	public async Task<IActionResult> OnPostDeleteCompany()
	{
		var db = new DataBaseManager();
		var user = await Models.User.GetUserByCookie(this, db);
		await db.DeleteCompany(user.Company);

		return RedirectToPage("/Company/Message", new { msgID = 2 });
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
	}

	private async System.Threading.Tasks.Task UpdateContent(DataBaseManager _db)
	{
		localizer = new();

		Company = await Models.Company.GetCompanyByCookie(this, _db);

		Company.Users = await _db.GetEmployees(Company.Name);
		Company.Addresses = await _db.GetAddresses(Company.Name);
		Company.Tasks = await _db.GetTasks(Company.Name);
		Company.SortContent();
	}
}

public class ConfigureCompanyTranslations : MyLocalizer
{
	internal override Dictionary<string, string> English
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Yes", "Yes" },
				{ "No", "No" },
				{ "Submit", "Submit" },
				{ "Back", "Back" },
				{ "Employees", "Employees" },
				{ "NewBoss", "Appoint A New Boss" },
				{ "MakeSupervisor", "Make Supervisor" },
				{ "DismissSupervisor", "Dismiss Supervisor" },
				{ "RemoveEmployee", "Remove Employee" },
				{ "InviteEmployee", "Invite a new employee" },
				{ "EmployeesPage", "Go to the Employees page" },
				{ "Addresses", "Addresses" },
				{ "RemoveAddress", "Remove Address" },
				{ "AddAddress", "Add a new address" },
				{ "Tasks", "Tasks" },
				{ "RemoveTask", "Remove Task" },
				{ "AddTask", "Add a new task" },
				{ "TaskFrequency", "How often should your employees fill out their tasks?" },
				{ "Whenever", "Whenever" },
				{ "Daily", "Daily" },
				{ "WorkDays", "Work Days" },
				{ "Weekly", "Weekly" },
				{ "BiWeekly", "Bi-Weekly" },
				{ "Monthly", "Monthly" },
				{ "DeleteCompany", "Delete company" },
				{ "AddEmployeeMessage", "Enter his/her email address in the field below. We will sent him/her an invitation on your behalf. The person you are inviting <u>must</u> have an Basic Work Manager account for this to work." },
				{ "EmailAddress", "Email Address" },
				{ "Country", "Country" },
				{ "City", "City" },
				{ "PostalCode", "Postal Code" },
				{ "Street", "Street" },
				{ "HouseNumber", "House Number" },
				{ "TaskName", "Task Name" },
				{ "TaskDescription", "Task Description" },
				{ "TaskOrder", "Order" },
				{ "TaskType", "Task Type:" },
				{ "TaskNumber", "Number" },
				{ "TaskText", "Text" },
				{ "DeleteYourCompany", "Delete your company" },
				{ "DelCompanyConf", "Are you sure that you want to delete your company?" },
			};
		}
	}

	internal override Dictionary<string, string> Dutch
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Yes", "Ja" },
				{ "No", "Nee" },
				{ "Submit", "Indienen" },
				{ "Back", "Terug" },
				{ "Employees", "Medewerkers" },
				{ "NewBoss", "Benoem Een Nieuwe Baas" },
				{ "MakeSupervisor", "Supervisor Maken" },
				{ "DismissSupervisor", "Supervisor Ontslaan" },
				{ "RemoveEmployee", "Werknemer Verwijderen" },
				{ "InviteEmployee", "Nodig een nieuwe medewerker uit" },
				{ "EmployeesPage", "Ga naar de Medewerkers pagina" },
				{ "Addresses", "Adressen" },
				{ "RemoveAddress", "Verwijder Adres" },
				{ "AddAddress", "Voeg een nieuw adres toe" },
				{ "Tasks", "Taken" },
				{ "RemoveTask", "Verwijder Taak" },
				{ "AddTask", "Voeg een nieuw Taak toe" },
				{ "TaskFrequency", "Hoe vaak moeten uw medewerkers hun taken uitvoeren?" },
				{ "Whenever", "Maakt niet uit" },
				{ "Daily", "Dagelijks" },
				{ "WorkDays", "Werkdagen" },
				{ "Weekly", "Weekelijks" },
				{ "BiWeekly", "Tweewekelijks" },
				{ "Monthly", "Maandelijks" },
				{ "DeleteCompany", "Bedrijf verwijderen" },
				{ "AddEmployeeMessage", "Vul in het onderstaande veld zijn/haar email adres in. Wij sturen hem/haar namens u een uitnodiging. De persoon die u uitnodigt <u>moet</u> een Basic Work Manager account hebben om dit te laten werken." },
				{ "EmailAddress", "Email Adres" },
				{ "Country", "Land" },
				{ "City", "Stad" },
				{ "PostalCode", "Postcode" },
				{ "Street", "Straat" },
				{ "HouseNumber", "Huis Numer" },
				{ "TaskName", "Taak Naam" },
				{ "TaskDescription", "Taak Beschrijving" },
				{ "TaskOrder", "Volgorde" },
				{ "TaskType", "Taak Type:" },
				{ "TaskNumber", "Numer" },
				{ "TaskText", "Tekst" },
				{ "DeleteYourCompany", "Verwijder uw bedrijf" },
				{ "DelCompanyConf", "Weet u zeker dat u uw bedrijf wilt verwijderen?" },
			};
		}
	}

	internal override Dictionary<string, string> Polish
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Yes", "Tak" },
				{ "No", "Nie" },
				{ "Submit", "ZatwierdŸ" },
				{ "Back", "Powrót" },
				{ "Employees", "Pracownicy" },
				{ "NewBoss", "Wyznacz Nowego Szefa" },
				{ "MakeSupervisor", "Uczyñ Nadzorcê" },
				{ "DismissSupervisor", "Odrzuæ Nadzorcê" },
				{ "RemoveEmployee", "Usuñ Pracownika" },
				{ "InviteEmployee", "Zaproœ nowego pracownika" },
				{ "EmployeesPage", "PrzejdŸ do strony Pracowników" },
				{ "Addresses", "Adresy" },
				{ "RemoveAddress", "Usuñ Adres" },
				{ "AddAddress", "Dodaj nowy adres" },
				{ "Tasks", "Zadania" },
				{ "RemoveTask", "Usuñ Zadanie" },
				{ "AddTask", "Dodaj nowe zadanie" },
				{ "TaskFrequency", "Jak czêsto Twoi pracownicy powinni wype³niaæ swoje zadania?" },
				{ "Whenever", "Kiedykolwiek" },
				{ "Daily", "Codziennie" },
				{ "WorkDays", "Dni Robocze" },
				{ "Weekly", "Tygodniowo" },
				{ "BiWeekly", "Dwutygodniowo" },
				{ "Monthly", "Miesiêcznie" },
				{ "DeleteCompany", "Usuñ firmê" },
				{ "AddEmployeeMessage", "Wpisz jego/jej adres email w polu poni¿ej. Wyœlemy mu/jej zaproszenie w Twoim imieniu. Aby to zadzia³a³o, osoba któr¹ zapraszasz, <u>musi</u> mieæ konto Basic Work Manager." },
				{ "EmailAddress", "Adres Email" },
				{ "Country", "Kraj" },
				{ "City", "Miasto" },
				{ "PostalCode", "Kod Pocztowy" },
				{ "Street", "Ulica" },
				{ "HouseNumber", "Numer Domu" },
				{ "TaskName", "Nazwa Zadania" },
				{ "TaskDescription", "Opis Zadania" },
				{ "TaskOrder", "Kolejnoœæ" },
				{ "TaskType", "Typ Zadania:" },
				{ "TaskNumber", "Numer" },
				{ "TaskText", "Tekst" },
				{ "DeleteYourCompany", "Usuñ swoj¹ firmê" },
				{ "DelCompanyConf", "Czy na pewno chcesz usun¹æ swoj¹ firmê?" },
			};
		}
	}
}