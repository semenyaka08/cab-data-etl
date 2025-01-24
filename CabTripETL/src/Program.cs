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
        var masterConnection = config.GetConnectionString("MasterConnection");
        
        if (connectionString == null || masterConnection == null) return;
        
        var inputCsvFilePath = Path.Combine(projectDirectory, config["DataFiles:InputCsv"]!);
        var duplicatesCsvFilePath = Path.Combine(projectDirectory, config["DataFiles:DuplicatesCsv"]!);
        
        var sqlScriptsDirectory = Path.Combine(projectDirectory, config["SqlFiles"]!);
        
        var databaseService = new DatabaseService(connectionString, masterConnection);
        
        databaseService.InitializeDatabase(sqlScriptsDirectory);
        
        var cabTrips = CsvProcessor.FilterDuplicatesAndProcessCsv(inputCsvFilePath, duplicatesCsvFilePath);
        
        databaseService.SaveToDatabase(cabTrips);
    }
}