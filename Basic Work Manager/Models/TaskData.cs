namespace BasicWorkManager.Models;
public class TaskData
{
	public string Company { get; set; }
	public string Username { get; set; }
	public string Task { get; set; }
	public DateTime Date { get; set; }
	public Address Address { get; set; }
	public string AddressString { get; set; }
	public string Data { get; set; }

	public void ParseAddress()
	{
		Address = Address.ParseAddress(AddressString);
	}
}
