using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Pages;

public class IndexModel : PageModel
{
    public async Task<IActionResult> OnGet()
	{
		if (!User.Claims.Any())
			return RedirectToPage("/Account/LogIn");

		var user = await Models.User.GetUserByCookie(this);

		if (user.Company == null)
			return RedirectToPage("/Company/JoinOrCreate");

		return RedirectToPage("/Calendar");
	}
}