using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Services;

public enum SupportedLanguages
{
	English, Dutch, Polish
}

public abstract class MyLocalizer
{
	internal abstract Dictionary<string, string> English { get; }
	internal abstract Dictionary<string, string> Dutch { get; }
	internal abstract Dictionary<string, string> Polish { get; }

	virtual public string GetString(string _key, PageModel _pageModel)
	{
		if (!English.ContainsKey(_key))
			return null;

		string lang = _pageModel.Request.Cookies["lang"];

		return lang switch
		{
			//default is discard
			"Dutch" => Dutch[_key],
			"Polish" => Polish[_key],
			_ => English[_key]
		};
	}
}