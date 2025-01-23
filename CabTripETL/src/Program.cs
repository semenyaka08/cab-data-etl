using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CabTripETL;

class Program
{
    static void Main()
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var relativePath = Path.Combine("..", "..", "..");
        var projectDirectory = Path.GetFullPath(Path.Combine(currentDirectory, relativePath));
        
        Console.WriteLine(projectDirectory);
        
        var config = new ConfigurationBuilder()
            .AddJsonFile($"{projectDirectory}\\src\\appsettings.json", optional: false,
                reloadOnChange: false)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        var inputCsvFilePath = Path.Combine(projectDirectory, config["DataFiles:InputCsv"]!);
        var duplicatesCsvFilePath = Path.Combine(projectDirectory, config["DataFiles:DuplicatesCsv"]!);

        var cabTrips = CsvProcessor.ReadCsv(inputCsvFilePath, duplicatesCsvFilePath);

        if (connectionString != null)
            SaveToDatabase(cabTrips, connectionString);
    }

    static void SaveToDatabase(IEnumerable<CabTripModel> cabTrips, string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("tpep_pickup_datetime", typeof(DateTime));
            dataTable.Columns.Add("tpep_dropoff_datetime", typeof(DateTime));
            dataTable.Columns.Add("passenger_count", typeof(int));
            dataTable.Columns.Add("trip_distance", typeof(decimal));
            dataTable.Columns.Add("store_and_fwd_flag", typeof(string));
            dataTable.Columns.Add("PULocationID", typeof(int));
            dataTable.Columns.Add("DOLocationID", typeof(int));
            dataTable.Columns.Add("fare_amount", typeof(decimal));
            dataTable.Columns.Add("tip_amount", typeof(decimal));

            foreach (var trip in cabTrips)
            {
                dataTable.Rows.Add(
                    trip.PickupDatetime,
                    trip.DropOffDatetime,
                    trip.PassengerCount,
                    trip.TripDistance,
                    trip.StoreAndFwdFlag,
                    trip.PuLocationId,
                    trip.DoLocationId,
                    trip.FareAmount,
                    trip.TipAmount
                );
            }
            
            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);
            
            bulkCopy.DestinationTableName = "Trips";

            bulkCopy.ColumnMappings.Add("tpep_pickup_datetime", "tpep_pickup_datetime");
            bulkCopy.ColumnMappings.Add("tpep_dropoff_datetime", "tpep_dropoff_datetime");
            bulkCopy.ColumnMappings.Add("passenger_count", "passenger_count");
            bulkCopy.ColumnMappings.Add("trip_distance", "trip_distance");
            bulkCopy.ColumnMappings.Add("store_and_fwd_flag", "store_and_fwd_flag");
            bulkCopy.ColumnMappings.Add("PULocationID", "PULocationID");
            bulkCopy.ColumnMappings.Add("DOLocationID", "DOLocationID");
            bulkCopy.ColumnMappings.Add("fare_amount", "fare_amount");
            bulkCopy.ColumnMappings.Add("tip_amount", "tip_amount");
            
            bulkCopy.WriteToServer(dataTable);
            
            transaction.Commit();
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            Console.WriteLine($"Error saving to the database: {ex.Message}");
            Console.WriteLine($"Error details: {ex}");
        }
        finally
        {
            connection.Close();
        }
    }
}