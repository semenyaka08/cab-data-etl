using CsvHelper.Configuration.Attributes;

namespace CabTripETL;

public class CabTripModel
{
    [Name("tpep_pickup_datetime")]
    public DateTime PickupDatetime { get; set; }

    [Name("tpep_dropoff_datetime")]
    public DateTime DropOffDatetime { get; set; }

    [Name("passenger_count")]
    public int? PassengerCount { get; set; }

    [Name("trip_distance")]
    public decimal TripDistance { get; set; }

    [Name("store_and_fwd_flag")]
    public string StoreAndFwdFlag { get; set; } = string.Empty;

    [Name("PULocationID")]
    public int PuLocationId { get; set; }

    [Name("DOLocationID")]
    public int DoLocationId { get; set; }

    [Name("fare_amount")]
    public decimal FareAmount { get; set; }

    [Name("tip_amount")]
    public decimal TipAmount { get; set; }
}