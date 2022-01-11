using BasicWorkManager.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using System.Security.Claims;

namespace BasicWorkManager.Models;

public enum UserRole { Regular = 0, Supervisor = 1, Boss = 2 }

public class User
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string Username { get; set; }
    public string Hash { get; set; }
    public string? Company { get; set; }
    public UserRole UserRole { get; set; }
    public int AuthenticationAttempts { get; set; }
    public bool EmailConfirmed { get; set; }
    public bool AccountLocked { get; set; }
    public DateTime? LastAccountLock { get; set; }
    public DateTime AccountCreationDate { get; set; }

	//Custom props
	public List<TaskData> TaskDataList { get; set; } = new();

	public float SumData(string _taskName, DateTime _date)
	{
		var specificTasks = TaskDataList.Where(d => d.Date == _date).Where(d => d.Task == _taskName).ToList();

		float sum = 0f;

		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

		foreach (var task in specificTasks)
			sum += float.Parse(task.Data);

		return sum;
	}

	public float SumData(string _taskName, DateTime[] _dates)
	{
		var specificTasks = new List<List<TaskData>>();

		for (int i = 0; i < specificTasks.Count; i++)
			specificTasks[i] = TaskDataList.Where(d => d.Date == _dates[i]).Where(d => d.Task == _taskName).ToList();

		float sum = 0;

		Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

		foreach (var taskList in specificTasks)
			foreach (var task in taskList)
				sum += float.Parse(task.Data);

		return sum;
	}

	public static async Task<User> GetUserByCookie(PageModel _pageModel)
	{
		var db = new DataBaseManager();
		var user = await db.GetUser(_pageModel.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value);
		return user;
	}

	public static async Task<User> GetUserByCookie(PageModel _pageModel, DataBaseManager _db)
	{
		var user = await _db.GetUser(_pageModel.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value);
		return user;
	}
}