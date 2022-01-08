using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;

namespace Localization_Test.Pages
{
    public class IndexModel : PageModel
    {
		private readonly IStringLocalizer<SharedResource> _localizer;

		public IndexModel(IStringLocalizer<SharedResource> localizer)
        {
			_localizer = localizer;
		}

        public void OnGet()
        {
			var x = _localizer["Key1"];
			Console.WriteLine(x);
        }
    }
}