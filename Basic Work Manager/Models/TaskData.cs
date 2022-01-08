namespace BasicWorkManager.Models;
public class TaskData
{
	public string Company { get; set; }
	public string Username { get; set; }
	public string Task { get; set; }
	public DateOnly Date { get; set; }
	public string DateString { get; set; }
	public Address Address { get; set; }
	public string AddressString { get; set; }
	public string Data { get; set; }

	public void ParseProperties()
	{
		Date = DateOnly.Parse(DateString);
		Address = Address.ParseAddress(AddressString);
	}
}
