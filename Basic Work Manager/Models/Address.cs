namespace BasicWorkManager.Models;

public class Address
{
    public string Company { get; set; }
    public string? Country { get; set; }
    public string City { get; set; }
    public string Street { get; set; }
    public string HouseNumber { get; set; }
    public string? PostalCode { get; set; }

	//Custom
	public bool IsLastAddress { get; set; } = false;

	public string WriteAddress()
	{
		return $"{PostalCode}, {Street} {HouseNumber}, {City}";
	}

	public string WriteFullAddress()
	{
		return $"{PostalCode}, {Street} {HouseNumber}, {City}, {Country}";
	}

	public static Address ParseAddress(string _addressString)
	{
		var array = _addressString.Split(", ");
		var lastSpace = array[1].LastIndexOf(" ");
		var street = array[1].Substring(0, lastSpace);
		var number = array[1].Substring(lastSpace, array[1].Length - lastSpace);
		number = number.Substring(1);
		var address = new Address()
		{
			PostalCode = array[0],
			Street = street,
			HouseNumber = number,
			City = array[2],
			Country = array[3],
		};

		return address;
	}

	public static bool CompareAddresses(Address _address1, Address _address2)
	{
		if (_address1.WriteFullAddress() == _address2.WriteFullAddress())
			return true;
		else
			return false;
	}
}
