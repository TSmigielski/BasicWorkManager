using System.ComponentModel.DataAnnotations;
using BasicWorkManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Pages.Company;

[Authorize]
public class JoinOrCreateModel : PageModel
{
	[BindProperty]
	public CompanyForm CompanyForm { get; set; }

	public JoinOrCreateTranslations localizer;

	public async Task<IActionResult> OnGet()
    {
		localizer = new();

		var user = await Models.User.GetUserByCookie(this);
		if (user.Company != null)
			return RedirectToPage("/Index");
		return Page();
	}

	public async Task<IActionResult> OnPost()
	{
		localizer = new();

		if (string.IsNullOrEmpty(CompanyForm.Name))
			return JoinCompany();

		if (string.IsNullOrEmpty(CompanyForm.NameOrCode))
			return await CreateCompany();

		return Page();
	}

	private IActionResult JoinCompany()
	{
		throw new NotImplementedException(); // todo - invite codes

		//return RedirectToPage("/Tasks");
	}

	private async Task<IActionResult> CreateCompany()
	{
		var db = new DataBaseManager();
		var user = await Models.User.GetUserByCookie(this, db);

		await db.CreateCompany(CompanyForm.Name, CompanyForm.Description, user.Username);

		return RedirectToPage("/Company/Message", new { msgId = 0 });
	}
}

public class CompanyForm
{
	[Required]
	[DataType(DataType.Text)]
	public string NameOrCode { get; set; }

	[Required]
	[DataType(DataType.Text)]
	public string Name { get; set; }

	[Required]
	[DataType(DataType.Text)]
	public string Description { get; set; }
}

public class JoinOrCreateTranslations : MyLocalizer
{
	internal override Dictionary<string, string> English
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "It seems that you are not a part of a company yet." },
				{ "JoinCompany", "Join an existing company" },
				{ "CreateCompany", "Create a new company" },
				{ "CompanyName", "Company name" },
				{ "CompanyDescription", "Company description" },
				{ "Submit", "Submit" },
				{ "Create", "Create" },
				{ "Back", "Back" },
			};
		}
	}

	internal override Dictionary<string, string> Dutch
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Het lijkt erop dat u nog bij geen bedrijf bent." },
				{ "JoinCompany", "Word lid van een bestaand bedrijf" },
				{ "CreateCompany", "Maak een nieuw bedrijf aan" },
				{ "CompanyName", "Bedrijfsnaam" },
				{ "CompanyDescription", "Bedrijfsomschrijving" },
				{ "Submit", "Indienen" },
				{ "Create", "Creëren" },
				{ "Back", "Terug" },
			};
		}
	}

	internal override Dictionary<string, string> Polish
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Wygl¹da na to, ¿e nie do³¹czy³eœ jeszcze do ¿adnej firmy." },
				{ "JoinCompany", "Do³¹cz do istniej¹cej firmy" },
				{ "CreateCompany", "Za³ó¿ now¹ firmê" },
				{ "CompanyName", "Nazwa firmy" },
				{ "CompanyDescription", "Opis firmy" },
				{ "Submit", "ZatwierdŸ" },
				{ "Create", "Stwórz" },
				{ "Back", "Powrót" },
			};
		}
	}
}