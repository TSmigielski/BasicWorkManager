using BasicWorkManager.Services;
using FluentEmail.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Pages.Account;

public enum AccountMessageID { VerificationSent = 0, VerificationSuccessfull = 1,
PasswordResetSent = 2, PasswordResetSuccessfull = 3 }

public class AccountMessageModel : PageModel
{
	private MessageTranslations localizer;

    [FromQuery(Name = "msgId")]
    public AccountMessageID? MsgID { get; set; }

    [FromQuery(Name = "email")]
    public string Email { get; set; }

    public string Title { get; private set; }
    public string Message { get; private set; }
    public string ButtonText { get; private set; }

    public IActionResult OnGet()
    {
        if (MsgID == null)
            return RedirectToPage("/Index");

		localizer = new();

		Title = GetTitle();
        Message = GetMessage();
        ButtonText = GetButtonText();

        return Page();
    }

    private string GetTitle()
    {
        return MsgID switch
        {
            AccountMessageID.VerificationSent => localizer.GetString("EmailSent", this),
            AccountMessageID.VerificationSuccessfull => localizer.GetString("EmailVerified", this),
            AccountMessageID.PasswordResetSent => localizer.GetString("EmailSent", this),
			AccountMessageID.PasswordResetSuccessfull => localizer.GetString("PasswordReset", this),
			_ => "",
        };
    }

    private string GetMessage()
    {
        return MsgID switch
        {
            AccountMessageID.VerificationSent => localizer.GetString("VerificationSent", this).Replace("{0}", Email),
			AccountMessageID.VerificationSuccessfull => localizer.GetString("VerifiedSuccessfully", this),
            AccountMessageID.PasswordResetSent => localizer.GetString("PasswordResetSent", this).Replace("{0}", Email),
            AccountMessageID.PasswordResetSuccessfull => localizer.GetString("PasswordResetSuccessful", this),
            _ => "",
        };
    }

    private string GetButtonText()
    {
        return MsgID switch
        {
            AccountMessageID.VerificationSent => localizer.GetString("Back2Login", this),
            AccountMessageID.VerificationSuccessfull => localizer.GetString("Click2Login", this),
            AccountMessageID.PasswordResetSent => localizer.GetString("Back2Login", this),
            AccountMessageID.PasswordResetSuccessfull => localizer.GetString("Click2Login", this),
            _ => "",
        };
    }
}

public class MessageTranslations : MyLocalizer
{
	internal override Dictionary<string, string> English
	{
		get {
			return new Dictionary<string, string>
			{
				{ "EmailSent", "Email Sent" },
				{ "EmailVerified", "Account Activated Successfully" },
				{ "PasswordReset", "Password Reset Successful" },
				{ "VerificationSent", "We have sent a activation email to \"{0}\". Please activate your account with the link provided in that email." },
				{ "VerifiedSuccessfully", "You have successfully activated your account. Click the button below to log in." },
				{ "PasswordResetSent", "We have sent a password reset email to \"{0}\". Please use the link provided in that email to reset your password." },
				{ "PasswordResetSuccessful", "You have successfully reset your password. Click the button below to log in." },
				{ "Back2Login", "Back to login" },
				{ "Click2Login", "Click here to log in" },
			};
		}
	}

	internal override Dictionary<string, string> Dutch
	{
		get {
			return new Dictionary<string, string>
			{
				{ "EmailSent", "Email Verstuurd" },
				{ "EmailVerified", "Account Geactiveerd" },
				{ "PasswordReset", "Wachtwoord Reset Succesvol" },
				{ "VerificationSent", "We hebben een activatie email verstuurd naar \"{0}\". Activeer uw account met de link in die email." },
				{ "VerifiedSuccessfully", "U heeft uw account succesvol geactiveerd. Klik op onderstaande knop om in te loggen." },
				{ "PasswordResetSent", "We hebben een wachtwoord reset email verstuurd naar \"{0}\". Stel uw wachtwoord opnieuw in me de link in die email." },
				{ "PasswordResetSuccessful", "U heeft uw wachtwoord succesvol opnieuw ingesteld. Klik op onderstaande knop om in te loggen." },
				{ "Back2Login", "Terug naar login" },
				{ "Click2Login", "Klik hier om in te loggen" },
			};
		}
	}

	internal override Dictionary<string, string> Polish
	{
		get {
			return new Dictionary<string, string>
			{
				{ "EmailSent", "Email Wys³any" },
				{ "EmailVerified", "Konto Aktywowane Pomyœlnie" },
				{ "PasswordReset", "Reset Has³a Pomyœlny" },
				{ "VerificationSent", "Wys³aliœmy wiadomoœæ aktywacyjn¹ email na adres \"{0}\". Aktywuj swoje konto za pomoc¹ linku podanego w tym emailu." },
				{ "VerifiedSuccessfully", "Pomyœlnie aktywowa³eœ swoje konto. Kliknij przycisk poni¿ej, aby siê zalogowaæ." },
				{ "PasswordResetSent", "Wys³aliœmy wiadomoœæ email dotycz¹c¹ zmiany has³a na adres \"{0}\". Zmieñ swoje has³o za pomoc¹ linku podanego w tym emailu." },
				{ "PasswordResetSuccessful", "Pomyœlnie zresetowa³eœ swoje has³o. Kliknij przycisk poni¿ej, aby siê zalogowaæ." },
				{ "Back2Login", "Powrót do logowania" },
				{ "Click2Login", "Kliknij tutaj aby siê zalogowaæ" },
			};
		}
	}
}