namespace Kutxa.UnitTests.SqlInterview;
public class TripsByType
{
    public string Type { get; set; }
    public int TripsTotal { get; set; }
}

public class TripsByClient
{
    public string ClientName { get; set; }
    public int TripsTotal { get; set; }
}

public class TripPrice
{
    public string Destination { get; set; }
    public decimal Price { get; set; }
}