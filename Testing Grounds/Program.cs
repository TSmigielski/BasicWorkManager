using Newtonsoft.Json;

var x = JsonConvert.DeserializeObject<byte[]>("\"xs/BJ29ai5rxfmLAoFImUd2d9eAS/bZFZFdhbXtNP8o=\"");

foreach (var y in x)
{
	Console.WriteLine(y);
}

Console.ReadKey();

//https://localhost:7079/Account/Verification?token=972259051&key=%22Sl5TzOKu+l1NK328TJ5dUqguta8/D/hl4WqsPyTxS68=%22&iv=%22vmq486XcUqEDJQR3nU5mTw==%22