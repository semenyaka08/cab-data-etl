using System.Data;
using Microsoft.Data.SqlClient;

namespace CabTripETL;

public class DatabaseService(string connectionString, string masterConnectionString)
{
    public void InitializeDatabase(string scriptsDirectory)
    {
        var sqlFiles = Directory.GetFiles(scriptsDirectory, "*.sql");

        using (var masterConnection = new SqlConnection(masterConnectionString))
        {
            masterConnection.Open();

            var createDatabaseScriptPath = sqlFiles.FirstOrDefault(file => file.Contains("CreateDatabase", StringComparison.OrdinalIgnoreCase));
            if (createDatabaseScriptPath != null)
            {
                var createDatabaseScript = File.ReadAllText(createDatabaseScriptPath);
                using var command = new SqlCommand(createDatabaseScript, masterConnection);
                command.ExecuteNonQuery();
            }
        }
        
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        foreach (var file in sqlFiles)
        {
            var script = File.ReadAllText(file);
            using var command = new SqlCommand(script, connection);
            command.ExecuteNonQuery();
        }
    }
    
    public void SaveToDatabase(IEnumerable<CabTripModel> cabTrips)
    {
        DataTable dataTable = new DataTable();
        dataTable.Columns.Add(nameof(CabTripModel.PickupDatetime), typeof(DateTime));
        dataTable.Columns.Add(nameof(CabTripModel.DropOffDatetime), typeof(DateTime));
        dataTable.Columns.Add(nameof(CabTripModel.PassengerCount), typeof(int));
        dataTable.Columns.Add(nameof(CabTripModel.TripDistance), typeof(decimal));
        dataTable.Columns.Add(nameof(CabTripModel.StoreAndFwdFlag), typeof(string));
        dataTable.Columns.Add(nameof(CabTripModel.PuLocationId), typeof(int));
        dataTable.Columns.Add(nameof(CabTripModel.DoLocationId), typeof(int));
        dataTable.Columns.Add(nameof(CabTripModel.FareAmount), typeof(decimal));
        dataTable.Columns.Add(nameof(CabTripModel.TipAmount), typeof(decimal));

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
        
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            using var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction);
            
            bulkCopy.DestinationTableName = "Trips";

            bulkCopy.ColumnMappings.Add(nameof(CabTripModel.PickupDatetime), nameof(CabTripModel.PickupDatetime));
            bulkCopy.ColumnMappings.Add(nameof(CabTripModel.DropOffDatetime), nameof(CabTripModel.DropOffDatetime));
            bulkCopy.ColumnMappings.Add(nameof(CabTripModel.PassengerCount), nameof(CabTripModel.PassengerCount));
            bulkCopy.ColumnMappings.Add(nameof(CabTripModel.TripDistance), nameof(CabTripModel.TripDistance));
            bulkCopy.ColumnMappings.Add(nameof(CabTripModel.StoreAndFwdFlag), nameof(CabTripModel.StoreAndFwdFlag));
            bulkCopy.ColumnMappings.Add(nameof(CabTripModel.PuLocationId), nameof(CabTripModel.PuLocationId));
            bulkCopy.ColumnMappings.Add(nameof(CabTripModel.DoLocationId), nameof(CabTripModel.DoLocationId));
            bulkCopy.ColumnMappings.Add(nameof(CabTripModel.FareAmount), nameof(CabTripModel.FareAmount));
            bulkCopy.ColumnMappings.Add(nameof(CabTripModel.TipAmount), nameof(CabTripModel.TipAmount));
            
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