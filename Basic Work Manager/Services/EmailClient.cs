using System.Net;
using System.Net.Mail;
using BasicWorkManager.Models;
using FluentEmail.Core;
using FluentEmail.Smtp;

namespace BasicWorkManager.Services;

public class EmailClient
{
	public async System.Threading.Tasks.Task SendTokenEmail(User _user, TokenType _tokenType, int _id, string _key, string _iv, string _companyName = null)
	{
		if (_tokenType == TokenType.CompanyInvitation && string.IsNullOrWhiteSpace(_companyName))
			throw new ArgumentException($"'{nameof(_companyName)}' cannot be null or whitespace.", nameof(_companyName));

		var sender = GetSender();

		Email.DefaultSender = sender;

		await Email
			.From("no-reply@basicworkmanager.com")
			.To(_user.EmailAddress, $"{_user.FirstName} {_user.LastName}")
			.Subject(GetTokenSubject(_tokenType, _companyName))
			.Body(GetTokenBody(_tokenType, _id, _key, _iv, _companyName))
			.SendAsync();
	}

	private string GetTokenSubject(TokenType _tokenType, string _companyName)
	{
		return _tokenType switch
		{
			TokenType.AccountVerification => "Verify your Basic Work Manager account",
			TokenType.PasswordReset => "Password reset for your Basic Work Manager account",
			TokenType.CompanyInvitation => $"You were invited to a new company '{_companyName}' at Basic Work Manager",
			_ => "",
		};
	}

	private string GetTokenBody(TokenType _tokenType, int _id, string _key, string _iv, string _companyName)
{
		return _tokenType switch
		{
			TokenType.AccountVerification => "Click on the link below to verify your account:\n" +
			$"https://www.basicworkmanager.com/Account/Verification?tokenid={_id}&key={_key}&iv={_iv}",

			TokenType.PasswordReset => "Click on the link below to reset your password:\n" +
			$"https://www.basicworkmanager.com/Account/Verification?tokenid={_id}&key={_key}&iv={_iv}",

			TokenType.CompanyInvitation => $"Click on the link below to join {_companyName}:\n" +
			$"https://www.basicworkmanager.com/Account/Verification?tokenid={_id}&key={_key}&iv={_iv}",
			_ => "",
		};
	}

	private SmtpSender GetSender()
	{
		return new SmtpSender(() => new SmtpClient()
		{
			EnableSsl = true,
			Credentials = ConfigurationHelper.GetMailCredential(),
			Host = "smtp.gmail.com",
			Port = 587,
		});
	}
}
