IF NOT EXISTS (
    SELECT 1
    FROM sys.databases
    WHERE name = N'CabTrips'
)
BEGIN
    CREATE DATABASE CabTrips;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.objects
    WHERE object_id = OBJECT_ID(N'[dbo].[Trips]') AND type = N'U'
)
BEGIN
    -- Create the Trips table
CREATE TABLE Trips
(
    Id                   INT IDENTITY(1,1) PRIMARY KEY,
    PickupDatetime   DATETIME NOT NULL,
    DropOffDatetime  DATETIME NOT NULL,
    PassengerCount       INT,
    TripDistance         DECIMAL(10, 2),
    StoreAndFwdFlag      NVARCHAR(3),
    PuLocationId         INT,
    DoLocationId         INT,
    FareAmount           DECIMAL(10, 2),
    TipAmount            DECIMAL(10, 2)
);
END;
