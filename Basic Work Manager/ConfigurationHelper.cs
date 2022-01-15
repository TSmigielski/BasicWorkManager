using System.Net;

namespace BasicWorkManager;

public static class ConfigurationHelper
{
    public static string GetConnectionString(string _connectionStringName)
    {
		var config = GetConfig();

		var connectionStrings = config.GetSection("ConnectionStrings");

		return connectionStrings.GetSection(_connectionStringName).Value;
	}

	public static NetworkCredential GetMailCredential()
	{
		var config = GetConfig();

		var credentialObject = config.GetSection("MailCredential");

		return new NetworkCredential(credentialObject.GetSection("userName").Value, credentialObject.GetSection("password").Value);
	}

	private static IConfigurationRoot GetConfig()
	{
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			return new ConfigurationBuilder()
			.AddUserSecrets("0a6a70ef-2b52-4cf8-92af-762c26231cdc", true)
			.Build();
		}
		else
		{
			return new ConfigurationBuilder()
			.AddJsonFile("/home/bwm-core-app/secrets.json", false, true)
			.Build();
		}
	}
}
