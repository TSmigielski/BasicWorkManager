using BasicWorkManager.Models;
using BasicWorkManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BasicWorkManager.Pages;

public class RegisterModel : PageModel
{
    [BindProperty]
    public Registration Registration { get; set; }
    public string UsernameError { get; private set; }
    public string EmailError { get; private set; }

	public RegisterTranslations localizer;

	public IActionResult OnGet()
    {
        if (User.Identity.IsAuthenticated)
            return RedirectToPage("/Index");

		localizer = new();

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
		localizer = new();

		if (Registration.Password != Registration.PasswordConf)
            return Page(); //todo - limit characters as in DB

        DataBaseManager db = new();

        if (await db.GetUser(Registration.Username) != null)
        {
            UsernameError = localizer.GetString("UsernameInUse", this);

            if (await db.GetUser(Registration.EmailAddress) != null)
            {
                EmailError = localizer.GetString("EmailInUse", this);
                return Page();
            }
            return Page();
        }
        else if (await db.GetUser(Registration.EmailAddress) != null)
        {
            EmailError = localizer.GetString("EmailInUse", this);
            return Page();
        }

        User user = new()
        {
            FirstName = Registration.FirstName,
            LastName = Registration.LastName,
            EmailAddress = Registration.EmailAddress,
            Username = Registration.Username,
            Hash = BCrypt.Net.BCrypt.EnhancedHashPassword(Registration.Password),
            UserRole = UserRole.Regular
        };

        await db.CreateUser(user);

        await TokenHandler.GenerateToken(await db.GetUser(user.Username), TokenType.AccountVerification);

        return RedirectToPage($"/Account/Message", new { msgId = 0, email = user.EmailAddress});
    }
}

public class Registration
{
    [Required]
    [DataType(DataType.Text)]
    public string FirstName { get; set; }

    [Required]
    [DataType(DataType.Text)]
    public string LastName { get; set; }

    [Required]
    [DataType(DataType.Text)]
    public string Username { get; set; }

    [Required]
    [DataType(DataType.EmailAddress)]
    public string EmailAddress { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = "";

    [Required]
    [DataType(DataType.Password)]
    public string PasswordConf { get; set; } = "";
}

public class RegisterTranslations : MyLocalizer
{
	internal override Dictionary<string, string> English
	{
		get
		{
			return new Dictionary<string, string> {
				{ "RegisterButton", "Register" },
				{ "BackButton", "Back" },
				{ "Title", "Create a new account" },
				{ "FirstName", "First Name" },
				{ "LastName", "Last Name" },
				{ "EmailAddress", "Email Address" },
				{ "Username", "Username" },
				{ "Password", "Password" },
				{ "ConfPassword", "Confirm Password" },
				{ "UsernameInUse", "This username is already in use" },
				{ "EmailInUse", "This email address is already in use" },
			};
		}
	}

	internal override Dictionary<string, string> Dutch
	{
		get
		{
			return new Dictionary<string, string> {
				{ "RegisterButton", "Registreren" },
				{ "BackButton", "Terug" },
				{ "Title", "Maak een nieuw account aan" },
				{ "FirstName", "Voornaam" },
				{ "LastName", "Achternaam" },
				{ "EmailAddress", "Email Adres" },
				{ "Username", "Gebruikersnaam" },
				{ "Password", "Wachtwoord" },
				{ "ConfPassword", "Bevestig Wachtwoord" },
				{ "UsernameInUse", "Deze gebruikersnaam is al in gebruik" },
				{ "EmailInUse", "Dit email adres is al in gebruik" },
			};
		}
	}

	internal override Dictionary<string, string> Polish
	{
		get
		{
			return new Dictionary<string, string> {
				{ "RegisterButton", "Zarejestruj Siê" },
				{ "BackButton", "Powrót" },
				{ "Title", "Stwórz nowe konto" },
				{ "FirstName", "Imiê" },
				{ "LastName", "Nazwisko" },
				{ "EmailAddress", "Adres Email" },
				{ "Username", "Nazwa U¿ytkownika" },
				{ "Password", "Has³o" },
				{ "ConfPassword", "PotwierdŸ Has³o" },
				{ "UsernameInUse", "Ta nazwa u¿ytkownika jest ju¿ zajêta" },
				{ "EmailInUse", "Ten adres email jest ju¿ w u¿yciu" },
			};
		}
	}
}