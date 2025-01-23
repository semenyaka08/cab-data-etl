-- Create the database
CREATE DATABASE CabTrips;
GO

-- Use the database
USE CabTrips;
GO

-- Create the Trips table
CREATE TABLE Trips
(
    Id                    INT IDENTITY(1,1) PRIMARY KEY,
    tpep_pickup_datetime  DATETIME NOT NULL,
    tpep_dropoff_datetime DATETIME NOT NULL,
    passenger_count       INT,
    trip_distance         DECIMAL(10, 2),
    store_and_fwd_flag    NVARCHAR(3),
    PULocationID          INT,
    DOLocationID          INT,
    fare_amount           DECIMAL(10, 2),
    tip_amount            DECIMAL(10, 2)
);
GO