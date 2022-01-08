using BasicWorkManager.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BasicWorkManager.Pages.Account;

public class LogInModel : PageModel
{
	public LoginTranslations localizer;

	[BindProperty]
    public Credentials Credentials { get; set; }
    public string ErrorMessage { get; private set; }

    public IActionResult OnGet()
    {
		localizer = new();

		if (User.Identity.IsAuthenticated)
            return RedirectToPage("/Index");
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        var db = new DataBaseManager();
        var user = await db.GetUser(Credentials.UsernameOrEmail);
		localizer = new();

		if (user == null)
        {
            ErrorMessage = localizer.GetString("IncorrectUsernameOrPassword", this);
			return Page();
        }

        if (!user.EmailConfirmed)
		{
			ErrorMessage = localizer.GetString("AccountActivationMsg", this);
			return Page();
        }

        if (user.AccountLocked)
		{
			ErrorMessage = localizer.GetString("AccountLockedOut", this);
			return Page();
        }

        if (!BCrypt.Net.BCrypt.EnhancedVerify(Credentials.Password, user.Hash))
        {
			await db.ChangeAuthenticationAttempts(user.Username, 1);
            ErrorMessage = localizer.GetString("IncorrectUsernameOrPassword", this);
			return Page();
        }

		//todo - display login attempts with dates
		//todo - same thing but with locks
		await db.ChangeAuthenticationAttempts(user.Username, 0);

		var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Email, user.EmailAddress),
			new Claim(ClaimTypes.Role, ((int)user.UserRole).ToString()),
        };

		if (user.Company != null)
		{
			claims.Add(new Claim("HasCompany", "true"));
			claims.Add(new Claim("Company", user.Company));
		}

        var identity = new ClaimsIdentity(claims, "SID");
        ClaimsPrincipal principal = new ClaimsPrincipal(identity);

		var properties = new AuthenticationProperties
		{
			IsPersistent = Credentials.StaySignedIn
		};

		await HttpContext.SignInAsync("SID", principal, properties);

        return RedirectToPage("/Index");
    }
}

public class Credentials
{
    [Required]
    [DataType(DataType.Text)]
    public string UsernameOrEmail { get; set; }

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    public bool StaySignedIn { get; set; }
}

public class LoginTranslations : MyLocalizer
{
	internal override Dictionary<string, string> English
	{
		get { return new Dictionary<string, string> {
				{ "UsernameOrEmail", "Username or email address" },
				{ "Password", "Password" },
				{ "SignedInCheck", "Keep me signed in" },
				{ "PasswordRequest", "Request a new password" },
				{ "AccountRequest", "Create a new account" },
				{ "LogInButton", "Log In" },
				{ "IncorrectUsernameOrPassword", "Incorrect username or password" },
				{ "AccountActivationMsg", "You must activate your account first in order to log in" },
				{ "AccountLockedOut", "The referenced account is currently locked out and may not be logged on to" },
			};
		}
	}

	internal override Dictionary<string, string> Dutch
	{
		get { return new Dictionary<string, string> {
				{ "UsernameOrEmail", "Gebruikersnaam of email adres" },
				{ "Password", "Wachtwoord" },
				{ "SignedInCheck", "Houd mij aangemeld" },
				{ "PasswordRequest", "Vraag een nieuw wachtwoord aan" },
				{ "AccountRequest", "Maak een nieuw account aan" },
				{ "LogInButton", "Log In" },
				{ "IncorrectUsernameOrPassword", "Onjuiste gebruikersnaam of wachtwoord" },
				{ "AccountActivationMsg", "U moet eerst uw account activeren om in te loggen" },
				{ "AccountLockedOut", "Het account waarnaar wordt verwezen is momenteel vergrendeld en het is niet mogelijk om er op in te loggen" },
			};
		}
	}

	internal override Dictionary<string, string> Polish
	{
		get { return new Dictionary<string, string> {
				{ "UsernameOrEmail", "Nazwa u¿ytkownika albo adres email" },
				{ "Password", "Has³o" },
				{ "SignedInCheck", "Nie wylogowywuj mnie" },
				{ "PasswordRequest", "Poproœ o nowe has³o" },
				{ "AccountRequest", "Stwórz nowe konto" },
				{ "LogInButton", "Zaloguj Siê" },
				{ "IncorrectUsernameOrPassword", "Niepoprawna nazwa u¿ytkownika lub has³o" },
				{ "AccountActivationMsg", "Aby siê zalogowaæ, musisz najpierw aktywowaæ swoje konto" },
				{ "AccountLockedOut", "Wspomniane konto jest obecnie zablokowane i nie mo¿na siê na nie zalogowaæ" },
			};
		}
	}
}