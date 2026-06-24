using System.Data;

namespace Kutxa.UnitTests.SqlInterview;

public class SqlInterviewTests : SqlInterviewTestBase
{
    /// <summary>
    /// How many trips for each type exist
    /// </summary>
    [Fact]
    public void CountTripsType()
    {
        var sql = @"
            SELECT Type, COUNT(*) AS TripsTotal
            FROM Trips
            GROUP BY Type";

        var rows = ExecuteQuery(sql);

        var parsedResult = rows.Select(x => new TripsByType
        {
            Type = x.Field<string>("Type"),
            TripsTotal = x.Field<int>("TripsTotal"),
        }).ToList();
    }

    /// <summary>
    /// Different type of trips (unduplicated).
    /// </summary>
    [Fact]
    public void DiferentTripTypes()
    {
        var sql = @"
        SELECT DISTINCT Type
        FROM Trips";

        var rows = ExecuteQuery(sql);

        var parsedResult = rows.Select(x => x.Field<string>("Type")).ToList();
    }

    /// <summary>
    /// Trip types that have more than 5 trips.
    /// </summary>
    [Fact]
    public void TypesWithMoreThanNTrips()
    {
        var sql = @"
        SELECT Type, COUNT(*) AS TripsTotal
        FROM Trips
        GROUP BY Type
        HAVING COUNT(*) > 5";

        var rows = ExecuteQuery(sql);

        var parsedResult = rows.Select(x => new TripsByType
        {
            Type = x.Field<string>("Type"),
            TripsTotal = x.Field<int>("TripsTotal"),
        }).ToList();
    }

    /// <summary>
    /// Number of trips booked by each client.
    /// </summary>
    [Fact]
    public void TripsCountByClient()
    {
        var sql = @"
            SELECT c.Name AS ClientName, COUNT(t.TripId) AS TripsTotal
            FROM Clients c
            LEFT JOIN Trips t ON c.ClientId = t.ClientId
            GROUP BY c.Name";

        var rows = ExecuteQuery(sql);

        var parsedResult = rows.Select(x => new TripsByClient
        {
            ClientName = x.Field<string>("ClientName"),
            TripsTotal = x.Field<int>("TripsTotal"),
        }).ToList();
    }

    /// <summary>
    /// The 3 most expensive trips.
    /// </summary>
    [Fact]
    public void TopExpensiveTrips()
    {
        var sql = @"
        SELECT TOP 3 Destination, Price
        FROM Trips
        ORDER BY Price DESC";

        var rows = ExecuteQuery(sql);

        var parsedResult = rows.Select(x => new TripPrice
        {
            Destination = x.Field<string>("Destination"),
            Price = x.Field<decimal>("Price"),
        }).ToList();
    }
}