using System.Security.Cryptography;

namespace BasicWorkManager.Services;

public static class Encryption
{

	/// <summary>
	/// Encrypts the provided string using AES256 CDC and returns a List of Hex strings.
	/// </summary>
	/// <returns>
	/// [0] == key;
	/// [1] == iv;
	/// [2] == value;
	/// </returns>
	public static List<string> Encrypt_Aes(string plainText)
	{
		// Check arguments.
		if (plainText == null || plainText == "")
			throw new ArgumentNullException("plainText");

		List<string> result = new();

		// Create an Aes object
		// with the specified key and IV.
		using (Aes aesAlg = Aes.Create())
		{
			result.Add(Convert.ToHexString(aesAlg.Key));
			result.Add(Convert.ToHexString(aesAlg.IV));

			// Create an encryptor to perform the stream transform.
			ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

			// Create the streams used for encryption.
			using (MemoryStream msEncrypt = new MemoryStream())
			{
				using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
				{
					using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
					{
						//Write all data to the stream.
						swEncrypt.Write(plainText);
					}
					result.Add(Convert.ToHexString(msEncrypt.ToArray()));
				}
			}
		}

		// Return the encrypted bytes from the memory stream.
		return result;
	}

	/// <param name="Key">Needs to be in Hex format</param>
	/// <param name="IV">Needs to be in Hex format</param>
	/// <param name="cipherText">Needs to be in Hex format</param>
	public static string Decrypt_Aes(string Key, string IV, string cipherText)
	{
		// Check arguments.
		if (Key == null || Key == "")
			throw new ArgumentNullException("cipherText");
		if (IV == null || IV == "")
			throw new ArgumentNullException("Key");
		if (cipherText == null || cipherText == "")
			throw new ArgumentNullException("IV");

		// Declare the string used to hold
		// the decrypted text.
		string plaintext = null;

		// Create an Aes object
		// with the specified key and IV.
		using (Aes aesAlg = Aes.Create())
		{
			aesAlg.Key = Convert.FromHexString(Key);
			aesAlg.IV = Convert.FromHexString(IV);

			// Create a decryptor to perform the stream transform.
			ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

			// Create the streams used for decryption.
			using (MemoryStream msDecrypt = new MemoryStream(Convert.FromHexString(cipherText)))
			{
				using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
				{
					using (StreamReader srDecrypt = new StreamReader(csDecrypt))
					{
						// Read the decrypted bytes from the decrypting stream
						// and place them in a string.
						plaintext = srDecrypt.ReadToEnd();
					}
				}
			}
		}

		return plaintext;
	}
}