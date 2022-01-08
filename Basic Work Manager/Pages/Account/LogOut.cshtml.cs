using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Pages.Account;

public class LogOutModel : PageModel
{
    public async Task<IActionResult> OnGet()
    {
		await HttpContext.SignOutAsync("SID");
		return RedirectToPage("/Index");
	}

    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync("SID");
        return RedirectToPage("/Index");
    }
}