using BasicWorkManager.Services;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BasicWorkManager.Models;

public enum TaskFrequency
{
	Whenever = 0,
	Daily = 1,
	WorkDays = 2,
	Weekly = 3,
	BiWeekly = 4,
	Monthly = 5,
}

public class Company
{
	//DB props
	public string Name { get; set; }
	public string? Description { get; set; }
	public string Boss { get; set; }
	public TaskFrequency TaskFrequency { get; set; }

	//Custom props
	public List<User> Users { get; set; } = new();
	public List<Address> Addresses { get; set; } = new();
	public List<Task> Tasks { get; set; } = new();

	public static async Task<Company> GetCompanyByCookie(PageModel _pageModel)
	{
		var db = new DataBaseManager();
		var company = await db.GetCompany(_pageModel.User.Claims.FirstOrDefault(c => c.Type == "Company").Value);
		return company;
	}

	public static async Task<Company> GetCompanyByCookie(PageModel _pageModel, DataBaseManager _db)
	{
		var company = await _db.GetCompany(_pageModel.User.Claims.FirstOrDefault(c => c.Type == "Company").Value);
		return company;
	}

	public void SortContent()
	{
		SortUsers();
		SortAddresses();
		SortTasks();
	}

	public void SortUsers()
	{
		Users = Users.OrderByDescending(u => u.UserRole).ThenBy(u => u.LastName).ThenBy(u => u.FirstName).ToList();
	}

	public void SortAddresses()
	{
		Addresses = Addresses.OrderBy(a => a.Country).ThenBy(a => a.PostalCode).ThenBy(a => a.City).ThenBy(a => a.Street).ThenBy(a => a.HouseNumber).ToList();
	}

	public void SortTasks()
	{
		Tasks = Tasks.OrderBy(t => t.Order).ThenBy(t => t.Name).ToList();
	}
}