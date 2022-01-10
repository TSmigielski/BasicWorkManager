using BasicWorkManager.Models;
using Dapper;
using System.Data;
using System.Globalization;

namespace BasicWorkManager.Services;

public class DataBaseManager
{
	private readonly string CurrentDB;

	public DataBaseManager()
	{
		if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
		{
			//CurrentDB = "DevDB";
			CurrentDB = "ProductionDB_Remote";
		}
		else
		{
			CurrentDB = "ProductionDB";
		}
	}

    public async Task<User?> GetUser(string _usernameOrEmail)
    {
		if (string.IsNullOrWhiteSpace(_usernameOrEmail))
			throw new ArgumentException($"'{nameof(_usernameOrEmail)}' cannot be null or whitespace.", nameof(_usernameOrEmail));

		if (_usernameOrEmail.Contains('@'))
        {
            using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
            var output = await connection.QueryAsync<User>("dbo.FindEmailAddress @_emailAddress", new { _emailAddress = _usernameOrEmail });
            var array = output.ToArray();
            return array.Length == 1 ? array[0] : null;
        }
        else
        {
            using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
            var output = await connection.QueryAsync<User>("dbo.FindUsername @_username", new { _username = _usernameOrEmail });
            var array = output.ToArray();
            return array.Length == 1 ? array[0] : null;
        }
    }

    public async System.Threading.Tasks.Task CreateUser(User _user)
    {
        using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
        await connection.ExecuteAsync("dbo.CreateUser @_firstName, @_lastName, @_emailAddress, @_userName, @_hash", new
        {
            _firstName = _user.FirstName,
            _lastName = _user.LastName,
            _emailAddress = _user.EmailAddress,
            _userName = _user.Username,
            _hash = _user.Hash
        });
    }

    public async System.Threading.Tasks.Task CreateToken(int _id, string _value)
    {
        using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
        await connection.ExecuteAsync("dbo.CreateToken @_id, @_value", new { _id,  _value });
    }

    public async Task<TokenEncrypted?> FindToken(int _id)
    {
        using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
        var output = await connection.QueryAsync<TokenEncrypted>("dbo.FindToken @_id", new { _id });
        var array = output.ToArray();
        return array.Length == 1 ? array[0] : null;
    }

    public async System.Threading.Tasks.Task VerifyEmailAddress(string _username)
    {
        using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
        await connection.ExecuteAsync("dbo.VerifyEmailAddress @_username", new { _username });
    }

    public async System.Threading.Tasks.Task RemoveToken(int _id)
    {
        using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
        await connection.ExecuteAsync("dbo.RemoveToken @_id", new { _id });
    }

    public async System.Threading.Tasks.Task ChangePassword(string _username, string _hash)
    {
        using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
        await connection.ExecuteAsync("dbo.ChangePassword @_username, @_hash", new { _username, _hash });
    }
	
	/// <param name="_value">0 for unlocked; 1 for locked</param>
	public async System.Threading.Tasks.Task ChangeLockStatus(string _username, int _value)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		await connection.ExecuteAsync("dbo.ChangeLockStatus @_username, @_value", new { _username, _value });
	}

	/// <param name="_value">0 for reset; 1 for increment</param>
	public async System.Threading.Tasks.Task ChangeAuthenticationAttempts(string _username, int _value)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		await connection.ExecuteAsync("dbo.ChangeAuthenticationAttempts @_username, @_value", new { _username, _value });
	}

	public async System.Threading.Tasks.Task CreateCompany(string _name, string _description, string _bossUsername)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		await connection.ExecuteAsync("dbo.CreateCompany @_name, @_description, @_bossUsername", new { _name, _description, _bossUsername });
	}

	public async Task<Company?> GetCompany(string _name)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		var output = await connection.QueryAsync<Company>("dbo.FindCompany @_name", new { _name });
		var array = output.ToArray();
		return array.Length == 1 ? array[0] : null;
	}

	public async Task<List<User>?> GetEmployees(string _name)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		var output = await connection.QueryAsync<User>("dbo.FindEmployees @_name", new { _name });
		return output.ToList();
	}

	public async System.Threading.Tasks.Task ChangeCompany(string _username, string _company)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		await connection.ExecuteAsync("dbo.ChangeCompany @_username, @_company", new { _username, _company });
	}

	public async System.Threading.Tasks.Task DeleteCompany(string _companyName)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		await connection.ExecuteAsync("dbo.DeleteCompany @_name", new { _name = _companyName });
	}

	/// <param name="_roleId">0 = regular; 1 = supervisor; 2 = boss</param>
	public async System.Threading.Tasks.Task ChangeUserRole(string _username, UserRole _userRole)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		await connection.ExecuteAsync("dbo.ChangeUserRole @_username, @_roleId", new { _username, _roleId = (int)_userRole });
	}

	public async System.Threading.Tasks.Task CreateAddress(string _companyName, string _city, string _street, string _houseNumber, string _country = null, string _postalCode = null)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		await connection.ExecuteAsync("dbo.CreateAddress @_company, @_country, @_city, @_street, @_houseNumber, @_postalCode", new
		{ _companyName, _country, _city, _street, _houseNumber, _postalCode });
	}

	public async Task<List<Address>?> GetAddresses(string _companyName)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		var output = await connection.QueryAsync<Address>("dbo.FindAddresses @_companyName", new { _companyName });
		return output.ToList();
	}

	public async System.Threading.Tasks.Task RemoveAddress(string _companyName, string _city, string _street, string _houseNumber)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		await connection.ExecuteAsync("dbo.RemoveAddress @_company, @_city, @_street, @_houseNumber", new
		{ _companyName, _city, _street, _houseNumber });
	}

	public async System.Threading.Tasks.Task ChangeTaskFrequency(string _companyName, TaskFrequency _newTaskFrequency)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		await connection.ExecuteAsync("dbo.ChangeTaskFrequency @_companyName, @_newTaskFrequency", new
		{ _companyName, _newTaskFrequency = (int)_newTaskFrequency });
	}

	public async System.Threading.Tasks.Task CreateTask(string _companyName, string _taskName, string _description, int _order, Models.ValueType _valueType)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		await connection.ExecuteAsync("dbo.CreateTask @_companyName, @_taskName, @_description, @_order, @_valueType", new
		{ _companyName, _taskName, _description, _order, _valueType = (int)_valueType });
	}

	public async System.Threading.Tasks.Task RemoveTask(string _companyName, string _taskName)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		await connection.ExecuteAsync("dbo.RemoveTask @_companyName, @_taskName", new { _companyName, _taskName });
	}

	public async Task<List<Models.Task>?> GetTasks(string _companyName)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		var output = await connection.QueryAsync<Models.Task>("dbo.FindTasks @_companyName", new { _companyName });
		return output.ToList();
	}

	/// <summary>
	///	This method will create a record of the data, or (if it exists) override it
	/// </summary>
	public async System.Threading.Tasks.Task InsertTaskData(string _companyName, string _username, string _taskName, DateOnly _date, string _addressString, string _data)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		await connection.ExecuteAsync("dbo.InsertTaskData @_companyName, @_username, @_taskName, @_dateString, @_addressString, @_data", new 
		{ _companyName, _username, _taskName, _dateString = _date.ToString(), _addressString, _data });
	}

	public async System.Threading.Tasks.Task DeleteTaskData(string _companyName, string _username, string _taskName, DateOnly _date, string _addressString)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		await connection.ExecuteAsync("dbo.DeleteTaskData @_companyName, @_username, @_taskName, @_dateString, @_addressString", new
		{ _companyName, _username, _taskName, _dateString = _date.ToString(), _addressString });
	}

	public async Task<List<TaskData>?> GetTaskData(string _companyName, string _username, string _taskName, DateOnly _date, string _addressString)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		var output = await connection.QueryAsync<TaskData>("dbo.FindTaskData @_companyName, @_username, @_taskName, @_dateString, @_addressString", new
		{ _companyName, _username, _taskName, _dateString = _date.ToString(), _addressString });
		return output.ToList();
	}

	public async Task<List<TaskData>?> GetTaskData(string _companyName, string _username, DateOnly _date)
	{
		using IDbConnection connection = new System.Data.SqlClient.SqlConnection(ConfigurationHelper.GetConnectionString(CurrentDB));
		var output = await connection.QueryAsync<TaskData>("dbo.FindTaskData @_companyName, @_username, @_taskName, @_dateString, @_addressString", new
		{ _companyName, _username, _taskName = (string)null, _dateString = _date.ToString(), _addressString = (string)null });
		return output.ToList();
	}
}