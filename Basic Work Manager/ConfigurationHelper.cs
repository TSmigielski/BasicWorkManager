namespace BasicWorkManager;

public static class ConfigurationHelper
{
    public static string GetConnectionString(string _connectionStringName)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var connectionStrings = config.GetSection("ConnectionStrings");
        return connectionStrings.GetSection(_connectionStringName).Value;
    }
}
