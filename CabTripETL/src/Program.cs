using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace CabTripETL;

class Program
{
    static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("E:\\TestTask\\CabTripETL\\CabTripETL\\src\\appsettings.json", optional: false,
                reloadOnChange: false)
            .Build();

        var connectionString = config.GetConnectionString("DefaultConnection");

        const string inputCsvFilePath = "E:\\TestTask\\CabTripETL\\CabTripETL\\data\\sample-cab-data.csv";
        const string duplicatesCsvFilePath = "E:\\TestTask\\CabTripETL\\CabTripETL\\data\\duplicates.csv";

        var cabTrips = CsvProcessor.ReadCsv(inputCsvFilePath, duplicatesCsvFilePath);

        if (connectionString != null)
        {
            SaveToDatabase(cabTrips, connectionString);
        }
    }

    static void SaveToDatabase(IEnumerable<CabTripModel> cabTrips, string connectionString)
    {
        using var connection = new SqlConnection(connectionString);
        connection.Open();

        using var transaction = connection.BeginTransaction();
        try
        {
            foreach (var trip in cabTrips)
            {
                var sql = @"
                    INSERT INTO Trips ( 
                        tpep_pickup_datetime,
                        tpep_dropoff_datetime,
                        passenger_count,
                        trip_distance,
                        store_and_fwd_flag,
                        PULocationID,
                        DOLocationID,
                        fare_amount,
                        tip_amount
                    )
                    VALUES (
                        @tpep_pickup_datetime,
                        @tpep_dropoff_datetime,
                        @passenger_count,
                        @trip_distance,
                        @store_and_fwd_flag,
                        @PULocationID,
                        @DOLocationID,
                        @fare_amount,
                        @tip_amount
                    )";

                using var command = new SqlCommand(sql, connection, transaction);

                command.Parameters.Add("@tpep_pickup_datetime", SqlDbType.DateTime).Value = trip.PickupDatetime;
                command.Parameters.Add("@tpep_dropoff_datetime", SqlDbType.DateTime).Value = trip.DropoffDatetime;
                command.Parameters.Add("@passenger_count", SqlDbType.Int).Value = trip.PassengerCount;
                command.Parameters.Add("@trip_distance", SqlDbType.Decimal).Value = trip.TripDistance;
                command.Parameters.Add("@store_and_fwd_flag", SqlDbType.NVarChar).Value = trip.StoreAndFwdFlag;
                command.Parameters.Add("@PULocationID", SqlDbType.Int).Value = trip.PULocationID;
                command.Parameters.Add("@DOLocationID", SqlDbType.Int).Value = trip.DOLocationID;
                command.Parameters.Add("@fare_amount", SqlDbType.Decimal).Value = trip.FareAmount;
                command.Parameters.Add("@tip_amount", SqlDbType.Decimal).Value = trip.TipAmount;

                command.ExecuteNonQuery();
            }

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