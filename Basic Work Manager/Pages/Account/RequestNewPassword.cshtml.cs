using BasicWorkManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BasicWorkManager.Pages;

public class PasswordResetModel : PageModel
{
    [BindProperty]
    [DataType(DataType.Text)]
    public string UsernameOrEmail { get; set; }
    public string ErrorMessage { get; private set; }

	public PasswordRequestTranslations localizer;

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

		if (!ModelState.IsValid)
            return Page();

        var db = new DataBaseManager();
        var user = await db.GetUser(UsernameOrEmail);

        if (user == null)
        {
            ErrorMessage = "The referenced account does not exist";
            return Page();
        }

		await TokenHandler.GenerateToken(user, TokenType.PasswordReset);
		//await db.ChangeLockStatus(user.Username, 1);

		return RedirectToPage($"/Account/Message", new {msgId = 2, email = user.EmailAddress });
    }
}

public class PasswordRequestTranslations : MyLocalizer
{
	internal override Dictionary<string, string> English
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Request a new password" },
				{ "EmailOrUsername", "Your username or email address" },
				{ "SubmitButton", "Submit" },
				{ "BackButton", "Back" },
			};
		}
	}

	internal override Dictionary<string, string> Dutch
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Vraag een nieuw wachtwoord aan" },
				{ "EmailOrUsername", "Uw gebruikersnaam of email adres" },
				{ "SubmitButton", "Indienen" },
				{ "BackButton", "Terug" },
			};
		}
	}

	internal override Dictionary<string, string> Polish
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Poproœ o nowe has³o" },
				{ "EmailOrUsername", "Twoja nazwa u¿ytkownika lub adres email" },
				{ "SubmitButton", "ZatwierdŸ" },
				{ "BackButton", "Powrót" },
			};
		}
	}
}