-- Create a non-clustered index to optimize queries that calculate
-- the average tip amount for each pickup location.
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CabTrips_PULocationId_TipAmount' AND object_id = OBJECT_ID('Trips'))
    CREATE NONCLUSTERED INDEX IX_CabTrips_PULocationId_TipAmount ON Trips (PuLocationId, TipAmount);

-- Create a non-clustered index to optimize queries that find the top
-- longest fares based on trip distance. The index is in descending order
-- to optimize queries that use 'ORDER BY ... DESC'.       
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CabTrips_TripDistance' AND object_id = OBJECT_ID('Trips'))
    CREATE NONCLUSTERED INDEX IX_CabTrips_TripDistance ON Trips (TripDistance DESC);

-- Create a non-clustered index to optimize queries that find the top
-- longest fares based on the time spent traveling. This index uses both 
-- pickup and dropoff datetimes.       
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CabTrips_TripTime' AND object_id = OBJECT_ID('Trips'))
    CREATE NONCLUSTERED INDEX IX_CabTrips_TripTime ON Trips (PickupDatetime, DropOffDatetime);
       
-- Create a non-clustered index to optimize queries that use
-- PULocationId in their WHERE clause.       
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_CabTrips_PULocationId' AND object_id = OBJECT_ID('Trips'))
    CREATE NONCLUSTERED INDEX IX_CabTrips_PULocationId ON Trips (PuLocationId);