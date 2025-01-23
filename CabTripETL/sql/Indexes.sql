-- Create a non-clustered index to optimize queries that calculate
-- the average tip amount for each pickup location.
CREATE NONCLUSTERED INDEX IX_CabTrips_PULocationId_TipAmount
ON Trips (PULocationId, tip_amount);

-- Create a non-clustered index to optimize queries that find the top
-- longest fares based on trip distance. The index is in descending order
-- to optimize queries that use 'ORDER BY ... DESC'.       
CREATE NONCLUSTERED INDEX IX_CabTrips_TripDistance
ON Trips (trip_distance DESC);

-- Create a non-clustered index to optimize queries that find the top
-- longest fares based on the time spent traveling. This index uses both 
-- pickup and dropoff datetimes.       
CREATE NONCLUSTERED INDEX IX_CabTrips_TripTime
ON Trips (tpep_pickup_datetime, tpep_dropoff_datetime);
       
-- Create a non-clustered index to optimize queries that use
-- PULocationId in their WHERE clause.       
CREATE NONCLUSTERED INDEX IX_CabTrips_PULocationId
ON Trips (PULocationId);