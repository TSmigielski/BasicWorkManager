using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Pages;

[Authorize]
public class ArchiveModel : PageModel
{
    public void OnGet()
    {
    }
}
