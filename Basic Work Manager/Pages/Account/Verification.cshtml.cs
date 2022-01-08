using BasicWorkManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Pages.Account;

public class VerificationModel : PageModel
{
    [FromQuery(Name = "tokenid")]
    public int? TokenID { get; set; }

    [FromQuery(Name = "key")]
    public string? Key { get; set; }

    [FromQuery(Name = "iv")]
    public string? IV { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (TokenID == null || Key == null || IV == null)
            return RedirectToPage("/Index");

        return await Verify(); //todo - take who is logged in into consideration
    }

    private async Task<IActionResult> Verify()
    {
        var db = new DataBaseManager();

        var encryptedToken = await db.FindToken((int)TokenID);
		if (encryptedToken == null)
			return RedirectToPage("/Index");
		var token = TokenHandler.DecryptToken(encryptedToken.Value, Key, IV);

		return token.Type switch
		{
			TokenType.AccountVerification => await VerifyAccount(db, token),
			TokenType.PasswordReset => RedirectToPage("/Account/PasswordReset", new { tokenid = TokenID, key = Key, iv = IV }),
			TokenType.CompanyInvitation => await CompanyInvite(db, token),
			_ => throw new NotImplementedException(),
		};
    }

	private async Task<IActionResult> VerifyAccount(DataBaseManager _db, Token _token)
	{
		var tasks = new List<Task>();

		tasks.Add(_db.VerifyEmailAddress(_token.Username));
		tasks.Add(_db.RemoveToken((int)TokenID));

		await Task.WhenAll(tasks);

		return RedirectToPage("/Account/Message", new { msgId = 1 });
	}

	private async Task<IActionResult> CompanyInvite(DataBaseManager _db, Token _token)
	{
		await _db.ChangeCompany(_token.Username, _token.CompanyName);

		return RedirectToPage("/Company/Message", new { msgId = 1 });
	}
}
