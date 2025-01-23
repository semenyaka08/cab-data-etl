﻿using System.Globalization;
using CsvHelper;

namespace CabTripETL;

public static class CsvProcessor
{
    public static void Hi()
    {
        Console.WriteLine("Hello");
    }

    public static IEnumerable<CabTripModel> ReadCsv(string filePath, string duplicatesFilePath)
    {
        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        using var duplicatesWriter = new StreamWriter(duplicatesFilePath);
        using var csvDuplicatesWriter = new CsvWriter(duplicatesWriter, CultureInfo.InvariantCulture);

        var seenKeys = new HashSet<string>();

        var records = csv.GetRecords<CabTripModel>()
            .Where(record => record.PassengerCount != null)
            .Select(record =>
            {
                record.StoreAndFwdFlag = record.StoreAndFwdFlag.Trim();

                record.StoreAndFwdFlag = record.StoreAndFwdFlag switch
                {
                    "N" => "No",
                    "Y" => "Yes",
                    _ => record.StoreAndFwdFlag
                };

                record.PickupDatetime = ConvertFromEstToUtc(record.PickupDatetime);
                record.DropoffDatetime = ConvertFromEstToUtc(record.DropoffDatetime);

                return record;
            });

        csvDuplicatesWriter.WriteHeader<CabTripModel>();
        csvDuplicatesWriter.NextRecord();

        foreach (var record in records)
        {
            var key = $"{record.PickupDatetime:O}|{record.DropoffDatetime:O}|{record.PassengerCount}";

            if (!seenKeys.Add(key))
            {
                csvDuplicatesWriter.WriteRecord(record);
                csvDuplicatesWriter.NextRecord();
            }
            else
            {
                yield return record;
            }
        }
    }

    private static DateTime ConvertFromEstToUtc(DateTime estDateTime)
    {
        TimeZoneInfo estZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        DateTime utcDateTime = TimeZoneInfo.ConvertTimeToUtc(estDateTime, estZone);
        return utcDateTime;
    }
}