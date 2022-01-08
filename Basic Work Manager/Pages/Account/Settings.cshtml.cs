using BasicWorkManager.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Pages.Account;

public class SettingsModel : PageModel
{
    public IActionResult OnGet()
    {
		return RedirectToPage("/Index");
    }

	public IActionResult OnPostChangeLanguage(SupportedLanguages _language, string _path = null)
	{
		if (string.IsNullOrWhiteSpace(_path))
			_path = HttpContext.Request.Path;

		var options = new CookieOptions();
		options.Expires = DateTime.Now.AddYears(2);

		Response.Cookies.Delete("lang");
		Response.Cookies.Append("lang", _language.ToString(), options);

		return RedirectToPage(_path);
	}
}
