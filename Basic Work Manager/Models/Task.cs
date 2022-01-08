namespace BasicWorkManager.Models;

public enum ValueType
{
	Number = 0,
	Text = 1,
}

public class Task
{
	public string Company { get; set; }
	public string Name { get; set; }
	public string? Description { get; set; }
	public int? Order { get; set; }
	public ValueType ValueType { get; set; }
}