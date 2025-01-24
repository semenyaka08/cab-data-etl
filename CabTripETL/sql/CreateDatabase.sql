IF NOT EXISTS (
    SELECT 1
    FROM sys.databases
    WHERE name = N'CabTrips'
)
BEGIN
    CREATE DATABASE CabTrips;
END;

