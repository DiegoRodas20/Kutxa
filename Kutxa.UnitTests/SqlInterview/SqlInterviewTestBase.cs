using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

namespace Kutxa.UnitTests.SqlInterview;

public abstract class SqlInterviewTestBase : IDisposable
{
    private readonly SqliteConnection _connection;

    protected SqlInterviewTestBase()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();
        SetupDatabase();
    }

    public void Dispose() => _connection.Dispose();

    private void SetupDatabase()
    {
        void Execute(string sql)
        {
            using var cmd = _connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.ExecuteNonQuery();
        }

        Execute("""
            CREATE TABLE Clients (
                ClientId INTEGER PRIMARY KEY,
                Name     TEXT    NOT NULL
            )
            """);

        Execute("""
            CREATE TABLE Trips (
                TripId      INTEGER PRIMARY KEY,
                Type        TEXT    NOT NULL,
                Destination TEXT    NOT NULL,
                Price       DECIMAL NOT NULL,
                ClientId    INTEGER,
                FOREIGN KEY (ClientId) REFERENCES Clients(ClientId)
            )
            """);

        Execute("INSERT INTO Clients VALUES (1, 'Alice')");
        Execute("INSERT INTO Clients VALUES (2, 'Bob')");
        Execute("INSERT INTO Clients VALUES (3, 'Charlie')");

        // Adventure has 6 trips so TypesWithMoreThanNTrips returns it
        Execute("INSERT INTO Trips VALUES (1,  'Adventure', 'Everest',      5000, 1)");
        Execute("INSERT INTO Trips VALUES (2,  'Adventure', 'Patagonia',    4500, 2)");
        Execute("INSERT INTO Trips VALUES (3,  'Adventure', 'Kilimanjaro',  3000, 1)");
        Execute("INSERT INTO Trips VALUES (4,  'Adventure', 'Amazon',       2500, 3)");
        Execute("INSERT INTO Trips VALUES (5,  'Adventure', 'Grand Canyon', 1500, 2)");
        Execute("INSERT INTO Trips VALUES (6,  'Adventure', 'Sahara',        800, 1)");
        Execute("INSERT INTO Trips VALUES (7,  'Leisure',   'Maldives',     3500, 2)");
        Execute("INSERT INTO Trips VALUES (8,  'Leisure',   'Bali',         2000, 3)");
        Execute("INSERT INTO Trips VALUES (9,  'Leisure',   'Santorini',    1800, 1)");
        Execute("INSERT INTO Trips VALUES (10, 'Business',  'New York',     1200, 2)");
        Execute("INSERT INTO Trips VALUES (11, 'Business',  'London',        900, 3)");
    }

    /// <summary>
    /// Executes a SQL query against the in-memory database and returns typed DataRows.
    /// T-SQL TOP N syntax is automatically translated to SQLite LIMIT N.
    /// Column types are inferred from the SQLite declared type (DECIMAL, INTEGER) or
    /// from the actual runtime value, so Field&lt;int&gt; and Field&lt;decimal&gt; work as expected.
    /// </summary>
    protected IEnumerable<DataRow> ExecuteQuery(string sql)
    {
        // Translate T-SQL TOP N → SQLite LIMIT N so test SQL stays unchanged
        var topMatch = Regex.Match(sql, @"\bTOP\s+(\d+)\b", RegexOptions.IgnoreCase);
        if (topMatch.Success)
        {
            var limit = topMatch.Groups[1].Value;
            sql = Regex.Replace(sql, @"\bTOP\s+\d+\b\s*", string.Empty, RegexOptions.IgnoreCase);
            sql = sql.TrimEnd() + $"\nLIMIT {limit}";
        }

        using var cmd = _connection.CreateCommand();
        cmd.CommandText = sql;
        using var reader = cmd.ExecuteReader();

        // Capture column metadata while the reader is still open
        var fieldCount = reader.FieldCount;
        var names = new string[fieldCount];
        var declaredTypes = new string[fieldCount];
        for (var i = 0; i < fieldCount; i++)
        {
            names[i] = reader.GetName(i);
            declaredTypes[i] = reader.GetDataTypeName(i)?.ToUpperInvariant() ?? string.Empty;
        }

        // Read all raw values before the reader is disposed
        var rawRows = new List<object?[]>();
        while (reader.Read())
        {
            var raw = new object?[fieldCount];
            for (var i = 0; i < fieldCount; i++)
                raw[i] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            rawRows.Add(raw);
        }

        // Map SQLite types to the .NET types the tests expect:
        //   declared DECIMAL/NUMERIC → decimal (Field<decimal>)
        //   declared INTEGER/INT     → int     (Field<int>)
        //   computed (COUNT, etc.)   → infer from actual value: long→int, double→decimal
        var types = new Type[fieldCount];
        for (var i = 0; i < fieldCount; i++)
        {
            types[i] = declaredTypes[i] switch
            {
                "DECIMAL" or "NUMERIC" => typeof(decimal),
                "INTEGER" or "INT" => typeof(int),
                _ => rawRows.Select(r => r[i]).FirstOrDefault(v => v is not null) switch
                    {
                        long => typeof(int),
                        double => typeof(decimal),
                        _ => typeof(string)
                    }
            };
        }

        var table = new DataTable();
        for (var i = 0; i < fieldCount; i++)
            table.Columns.Add(names[i], types[i]);

        foreach (var rawRow in rawRows)
        {
            var tableRow = table.NewRow();
            for (var i = 0; i < fieldCount; i++)
                tableRow[i] = rawRow[i] is null
                    ? DBNull.Value
                    : Convert.ChangeType(rawRow[i], types[i]);
            table.Rows.Add(tableRow);
        }

        return table.Rows.Cast<DataRow>();
    }
}
