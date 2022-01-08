using BasicWorkManager.Models;
using Newtonsoft.Json;

namespace BasicWorkManager.Services;

[Serializable]
public enum TokenType
{
	AccountVerification = 0,
	PasswordReset = 1,
	CompanyInvitation = 2
}

public class TokenHandler
{
    public static async System.Threading.Tasks.Task GenerateToken(User _user, TokenType _tokenType, string _companyName = null)
    {
		if (_tokenType == TokenType.CompanyInvitation && string.IsNullOrWhiteSpace(_companyName))
			throw new ArgumentException($"'{nameof(_companyName)}' cannot be null or whitespace.", nameof(_companyName));

		Random random = new();
        DataBaseManager db = new();
        int id;
        
        do
        {
            id = random.Next(1, 1000000000);
        } while (await db.FindToken(id) != null);

        var token = new Token
        {
            Username = _user.Username,
            Type = _tokenType,
			CompanyName = _companyName,
        };

        var json = JsonConvert.SerializeObject(token);

        var encryptedJson = Encryption.Encrypt_Aes(json);
		var key = encryptedJson[0];
		var iv = encryptedJson[1];
		var value = encryptedJson[2];

		await db.CreateToken(id, value);

		var email = new EmailClient();
		await email.SendTokenEmail(_user, _tokenType, id, key, iv, _companyName);
	}

	public static Token DecryptToken(string _value, string _key, string _iv)
	{
		var decryptedJson = Encryption.Decrypt_Aes(_key, _iv, _value);

		return JsonConvert.DeserializeObject<Token>(decryptedJson);
    }
}

[Serializable]
public class Token
{
    public string Username { get; set; }
    public TokenType Type { get; set; }
    public string CompanyName { get; set; }
}

public class TokenEncrypted
{
    public int ID { get; set; }
    public string Value { get; set; }
}