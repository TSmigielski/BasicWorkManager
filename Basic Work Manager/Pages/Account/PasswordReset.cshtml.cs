using BasicWorkManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace BasicWorkManager.Pages.Account;

public class PasswordResetModel : PageModel // todo - Add the check for first/last/user name etc in the razor page.
{
	[FromQuery(Name = "tokenid")]
	public int? TokenID { get; set; }

	[FromQuery(Name = "key")]
	public string? Key { get; set; }

	[FromQuery(Name = "iv")]
	public string? IV { get; set; }

	[FromQuery(Name = "username")]
	public string? Username { get; set; }

	[BindProperty]
	[DataType(DataType.Password)]
	public string Password { get; set; }

	[BindProperty]
	[DataType(DataType.Password)]
	public string PasswordConf { get; set; }

	public PasswordResetTranslations localizer;

	public async Task<IActionResult> OnGet()
	{
		if (TokenID == null || Key == null || IV == null)
			return RedirectToPage("/Index");

		localizer = new();

		if (Username != null)
			return Page();
		else
			return await Verify();
	}

	private async Task<IActionResult> Verify()
	{
		var db = new DataBaseManager();

		var encryptedToken = await db.FindToken((int)TokenID);
		var token = TokenHandler.DecryptToken(encryptedToken.Value, Key, IV);

		if (token.Type != TokenType.PasswordReset)
			RedirectToPage("/Account/Verification", new { tokenid = TokenID, key = Key, iv = IV });

		return RedirectToPage("/Account/PasswordReset", new { tokenid = TokenID, key = Key, iv = IV, username = token.Username });
	}

	public async Task<IActionResult> OnPost()
	{
		localizer = new();

		if (Password != PasswordConf)
			return Page(); //todo - limit characters as in DB

		DataBaseManager db = new();

		await db.ChangePassword(Username, BCrypt.Net.BCrypt.EnhancedHashPassword(Password));
		await db.ChangeLockStatus(Username, 0);
		await db.RemoveToken((int)TokenID);

		return RedirectToPage("/Account/Message", new { msgId = 3 });
	}
}

public class PasswordResetTranslations : MyLocalizer
{
	internal override Dictionary<string, string> English
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Reset your password" },
				{ "Password", "Password" },
				{ "ConfPassword", "Confirm Password" },
				{ "SubmitButton", "Reset password" },
			};
		}
	}

	internal override Dictionary<string, string> Dutch
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Wijzig uw wachtwoord" },
				{ "Password", "Wachtwoord" },
				{ "ConfPassword", "Bevestig Wachtwoord" },
				{ "SubmitButton", "Wijzig Wachtwoord" },
			};
		}
	}

	internal override Dictionary<string, string> Polish
	{
		get
		{
			return new Dictionary<string, string> {
				{ "Title", "Zresetuj swoje has³o" },
				{ "Password", "Has³o" },
				{ "ConfPassword", "Potwierd¿ Has³o" },
				{ "SubmitButton", "Zresetuj Has³o" },
			};
		}
	}
}