using BasicWorkManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Pages.Company;

public enum CompanyMessageID
{
	CompanyCreated = 0,
	JoinedCompany = 1,
	CompanyDeleted = 2,
}

[Authorize]
public class CompanyMessageModel : PageModel
{
	[FromQuery(Name = "msgId")]
	public CompanyMessageID? MsgID { get; set; }

	public string Title { get; private set; }
	public string Message { get; private set; }
	public string ButtonText { get; private set; }
	public string ButtonAction { get; private set; }

	public CompanyMessageTranslations localizer;

	public IActionResult OnGet()
	{
		if (MsgID == null)
			return RedirectToPage("/Index");

		localizer = new();

		Title = GetTitle();
		Message = GetMessage();
		ButtonText = GetButtonText();
		ButtonAction = GetButtonAction();

		return Page();
	}

	private string GetTitle()
	{
		return MsgID switch
		{
			CompanyMessageID.CompanyCreated => localizer.GetString("CompanyCreated", this),
			CompanyMessageID.JoinedCompany => localizer.GetString("JoinedCompany", this),
			CompanyMessageID.CompanyDeleted => localizer.GetString("CompanyDeleted", this),
			_ => "",
		};
	}

	private string GetMessage()
	{
		return MsgID switch
		{
			CompanyMessageID.CompanyCreated => localizer.GetString("CompanyCreationSuccessful", this),
			CompanyMessageID.JoinedCompany => localizer.GetString("JoinedCompanySuccessfully", this),
			CompanyMessageID.CompanyDeleted => localizer.GetString("CompanyDeletedMsg", this),
			_ => "",
		};
	}

	private string GetButtonText()
	{
		return MsgID switch
		{
			CompanyMessageID.CompanyCreated => localizer.GetString("ConfigureCompany", this),
			CompanyMessageID.JoinedCompany => localizer.GetString("Tasks", this),
			CompanyMessageID.CompanyDeleted => localizer.GetString("JoinOrCreate", this),
			_ => "",
		};
	}

	private string GetButtonAction()
	{
		return MsgID switch
		{
			CompanyMessageID.CompanyCreated => "/Account/LogOut",
			CompanyMessageID.JoinedCompany => "/Tasks",
			CompanyMessageID.CompanyDeleted => "/Company/JoinOrCreate",
			_ => "",
		};
	}
}

public class CompanyMessageTranslations : MyLocalizer
{
	internal override Dictionary<string, string> English
	{
		get
		{
			return new Dictionary<string, string> {
				{ "CompanyCreated", "Company Created" },
				{ "JoinedCompany", "Joined Company" },
				{ "CompanyDeleted", "Company Deleted" },
				{ "CompanyCreationSuccessful", "You have successfully created your company. Click on the button below to configure your company." },
				{ "JoinedCompanySuccessfully", "You have successfully joined a company. Click the button below to go to your tasks." },
				{ "CompanyDeletedMsg", "You have successfully deleted your company. Click the button below to join or create another company." },
				{ "ConfigureCompany", "Configure Company" },
				{ "Tasks", "My Tasks" },
				{ "JoinOrCreate", "Join or Create" },
			};
		}
	}

	internal override Dictionary<string, string> Dutch
	{
		get
		{
			return new Dictionary<string, string> {
				{ "CompanyCreated", "Bedrijf Aangemaakt" },
				{ "JoinedCompany", "Bijgevoegd Aan Bedrijf" },
				{ "CompanyDeleted", "Bedrijf Verwijderd" },
				{ "CompanyCreationSuccessful", "U heeft uw bedrijf succesvol aangemaakt. Klik op onderstaande knop om uw bedrijf in te stellen." },
				{ "JoinedCompanySuccessfully", "U bent succesvol toegevoegd aan een bedrijf. Klik op onderstaande knop om naar u taken te gaan." },
				{ "CompanyDeletedMsg", "U heeft uw bedrijf succesvol verwijderd. Klik op de onderstaande knop om een lid te worden bij een andere bedrijf, of maak een nieuwe aan." },
				{ "ConfigureCompany", "Bedrijf Instellen" },
				{ "Tasks", "Mijn Taken" },
				{ "JoinOrCreate", "Word lid of maak een nieuw bedrijf aan" },
			};
		}
	}

	internal override Dictionary<string, string> Polish
	{
		get
		{
			return new Dictionary<string, string> {
				{ "CompanyCreated", "Za³o¿ono Firmê" },
				{ "JoinedCompany", "Do³¹czono Do Firmy" },
				{ "CompanyDeleted", "Usuniêto Firmê" },
				{ "CompanyCreationSuccessful", "Stworzy³eœ now¹ firmê pomyœlnie. Kliknij poni¿szy przycisk, aby skonfigurowaæ swoj¹ firmê." },
				{ "JoinedCompanySuccessfully", "Pomyœlnie do³¹czy³eœ do firmy. Kliknij przycisk poni¿ej, aby przejœæ do swoich zadañ." },
				{ "CompanyDeletedMsg", "Pomyœlnie usun¹³eœ swoj¹ firmê. Kliknij poni¿szy przycisk, aby do³¹czyæ lub utworzyæ inn¹ firmê." },
				{ "ConfigureCompany", "Konfiguruj Firmê" },
				{ "Tasks", "Moje Zadania" },
				{ "JoinOrCreate", "Do³¹cz lub utwórz firmê" },
			};
		}
	}
}